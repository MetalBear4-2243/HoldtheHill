namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// A short message that stands in for a HUD label for a few seconds, such as "Already max level".
    /// Plain C# so EditMode tests can cover it. The caller passes the time; the HUD passes unscaled
    /// time, so a notice also runs out under the pause menu.
    /// </summary>
    public sealed class TimedNotice
    {
        private string _text;
        private float _until;

        /// <summary>Shows <paramref name="text"/> from <paramref name="now"/> for <paramref name="seconds"/>, replacing any notice already up.</summary>
        public void Show(string text, float now, float seconds)
        {
            _text = text;
            _until = now + seconds;
        }

        /// <summary>The message at <paramref name="now"/>, or null once it has run out or was cleared.</summary>
        public string TextAt(float now)
        {
            return _text != null && now < _until ? _text : null;
        }

        /// <summary>Drops the message, so it does not carry into the next run.</summary>
        public void Clear()
        {
            _text = null;
            _until = 0f;
        }
    }
}
