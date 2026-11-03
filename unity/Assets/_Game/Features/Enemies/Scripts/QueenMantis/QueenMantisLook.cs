using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// How she looks: the White or Black top-down sprite for her shade, the Shift frames played
    /// through each pray pause (forwards into white, backwards into black), and her body turned to
    /// face the way she walks. Works on a child "Body" sprite, so it never moves her transform.
    /// </summary>
    [DisallowMultipleComponent]
    public class QueenMantisLook : QueenMantisPart
    {
        [Tooltip("Her body sprite, on a child object. The sprite faces up (+y).")]
        [SerializeField] private SpriteRenderer _body;

        private QueenMantisShade _from;
        private Vector3 _lastPosition;

        protected override void OnEnable()
        {
            base.OnEnable();
            _from = Boss.Shade;
            _lastPosition = transform.position;
            ShowStill();
        }

        protected override void OnShadeChanged(QueenMantisShade shade)
        {
            // The pray frames run from the old shade's sprite to the new one's.
            _from = shade == QueenMantisShade.White ? QueenMantisShade.Black : QueenMantisShade.White;
        }

        private void LateUpdate()
        {
            if (_body == null || Data == null)
            {
                return;
            }

            if (Boss.IsPraying)
            {
                _body.sprite = PrayFrame(Boss.PrayProgress);
            }
            else
            {
                ShowStill();
            }

            Face(transform.position - _lastPosition);
            _lastPosition = transform.position;
        }

        private void ShowStill()
        {
            if (_body != null && Data != null)
            {
                _body.sprite = Data.SpriteFor(Boss.Shade);
            }
        }

        // The sequence is the old sprite, the Shift frames in the right direction, then the new sprite.
        private Sprite PrayFrame(float progress)
        {
            Sprite[] shift = Data.ShiftFrames ?? System.Array.Empty<Sprite>();
            int steps = shift.Length + 2;
            int step = Mathf.Clamp((int)(progress * steps), 0, steps - 1);
            if (step == 0)
            {
                return Data.SpriteFor(_from);
            }

            if (step == steps - 1)
            {
                return Data.SpriteFor(Boss.Shade);
            }

            // ShiftFrames run black to white; into black they play backwards.
            int index = step - 1;
            Sprite frame = Boss.Shade == QueenMantisShade.White ? shift[index] : shift[shift.Length - 1 - index];
            return frame != null ? frame : Data.SpriteFor(Boss.Shade);
        }

        private void Face(Vector3 moved)
        {
            moved.z = 0f;
            if (moved.sqrMagnitude < 0.000001f)
            {
                return;
            }

            // The sprite's head points up, so straight up is 0 degrees.
            float target = Mathf.Atan2(moved.y, moved.x) * Mathf.Rad2Deg - 90f;
            float current = _body.transform.localEulerAngles.z;
            float next = Mathf.MoveTowardsAngle(current, target, Data.TurnDegreesPerSecond * Time.deltaTime);
            _body.transform.localRotation = Quaternion.Euler(0f, 0f, next);
        }
    }
}
