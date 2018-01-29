using DataCenter.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace DataCenter.UnitTest
{
    [TestClass]
    public class ExpirableRepositoryTests
    {
        public static Func<DateTime, string> fShortString = t => t.ToShortDateString();

        public static Func<DateTime, int> fDay = t => t.Day;

        public static Func<DateTime, long> fTicks = t => t.Ticks;

        public static Func<DateTime, DayOfWeek> fDayOfWeek = t => t.DayOfWeek;

        public static Func<DateTime, DateTime> fDate = t => t.Date;

        public static Func<DateTime, TimeSpan> fTimeOfDay = t => t.TimeOfDay;

        public static bool getDateTimeDetails(DateTime t, out String shortString, out int day,
            out long ticks, out DayOfWeek dayOfWeek, out DateTime date, out TimeSpan timeOfDay)
        {
            shortString = fShortString(t);
            day = fDay(t);
            ticks = fTicks(t);
            dayOfWeek = fDayOfWeek(t);
            date = fDate(t);
            timeOfDay = fTimeOfDay(t);
            return true;
        }

        [TestMethod]
        public void TestExpirableRepository_WithTimeSpan_ExpirationAsExpected()
        {
            ExpirableRepository<DateTime, String, int, long, DayOfWeek, DateTime, TimeSpan> repository =
                new ExpirableRepository<DateTime, String, int, long, DayOfWeek, DateTime, TimeSpan>(
                    TimeSpan.FromSeconds(1), fShortString, fDay, fTicks, fDayOfWeek, fDate, fTimeOfDay);

            var firstValue = repository.Get(DateTime.Today);
            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            Assert.IsTrue(DateTime.UtcNow < firstValue.Item1);
            Assert.IsTrue(repository.IsValid(DateTime.Today));
            var secondValue = repository.Get(DateTime.Today);
            Assert.AreEqual(firstValue, secondValue);
            Thread.Sleep(TimeSpan.FromMilliseconds(800));
            Assert.IsFalse(repository.IsValid(DateTime.Today));
            var thirdValue = repository.Get(DateTime.Today);
            Assert.IsTrue(thirdValue.Item1 > firstValue.Item1);
        }

        [TestMethod]
        public void TestExpirableRepository_WithTimeSpan_TrimWouldRemoveExpiredItems()
        {
            ExpirableRepository<DateTime, String, int, long, DayOfWeek, DateTime, TimeSpan> repository =
                new ExpirableRepository<DateTime, string, int, long, DayOfWeek, DateTime, TimeSpan>(
                    TimeSpan.FromMilliseconds(1000), getDateTimeDetails);

            var firstValue = repository.Get(DateTime.Today);
            var secondValue = repository.Get(DateTime.Today.AddHours(1));
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(repository.Trim() == 0);
            var thirdValue = repository.Get(DateTime.Today.AddHours(2));
            Thread.Sleep(TimeSpan.FromMilliseconds(600));
            Assert.IsFalse(repository.IsValid(DateTime.Today));
            Assert.IsFalse(repository.IsValid(DateTime.Today.AddHours(1)));
            Assert.IsTrue(repository.IsValid(DateTime.Today.AddHours(2)));
            Assert.IsTrue(repository.Trim() == 2);
        }
    }
}
