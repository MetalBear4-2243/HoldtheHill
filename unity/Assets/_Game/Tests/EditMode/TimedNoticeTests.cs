using HoldTheHill.Features.UI;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class TimedNoticeTests
    {
        [Test]
        public void ANotice_ShowsForItsTime_ThenRunsOut()
        {
            var notice = new TimedNotice();
            notice.Show("Already max level", now: 10f, seconds: 2f);

            Assert.AreEqual("Already max level", notice.TextAt(10f));
            Assert.AreEqual("Already max level", notice.TextAt(11.9f));
            Assert.IsNull(notice.TextAt(12f), "gone at the end of its time, so the slot label comes back");
        }

        [Test]
        public void Clear_DropsTheNotice_SoTheNextRunStartsWithout()
        {
            var notice = new TimedNotice();
            notice.Show("Already max level", now: 10f, seconds: 2f);
            notice.Clear();

            Assert.IsNull(notice.TextAt(10.5f));
        }

        [Test]
        public void ANewNotice_ReplacesTheOneUp_AndRestartsTheTime()
        {
            var notice = new TimedNotice();
            notice.Show("Already max level", now: 10f, seconds: 2f);
            notice.Show("Slot full", now: 11f, seconds: 2f);

            Assert.AreEqual("Slot full", notice.TextAt(12.5f));
            Assert.IsNull(notice.TextAt(13f));
        }

        [Test]
        public void NothingShown_GivesNoText()
        {
            Assert.IsNull(new TimedNotice().TextAt(0f));
        }
    }
}
