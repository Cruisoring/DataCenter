using DataCenter.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace DataCenter.UnitTest
{
    [TestClass]
    public class RepositoryPerformanceTests
    {
        private static int funcDurationMills = 0;
        public static Func<DateTime, string> fShortString = t =>
        {
            Thread.Sleep(funcDurationMills); return t.ToShortDateString();
        };

        public static Func<DateTime, int> fDay = t =>
        {
            Thread.Sleep(funcDurationMills);
            return t.Day;
        };
        public static Func<DateTime, long> fTicks = t =>
        {
            Thread.Sleep(funcDurationMills);
            return t.Ticks;
        };

        public static Func<DateTime, DayOfWeek> fDayOfWeek = t =>
        {
            Thread.Sleep(funcDurationMills);
            return t.DayOfWeek;
        };

        public static Func<DateTime, DateTime> fDate = t =>
        {
            Thread.Sleep(funcDurationMills);
            return t.Date;
        };

        public static Func<DateTime, TimeSpan> fTimeOfDay = t =>
        {
            Thread.Sleep(funcDurationMills);
            return t.TimeOfDay;
        };

        public static Func<DateTime, double> fOADate = t =>
        {
            Thread.Sleep(funcDurationMills);
            return t.ToOADate();
        };


        public static bool getDateTimeDetails(DateTime t, out String shortString, out int day,
            out long ticks, out DayOfWeek dayOfWeek, out DateTime date, out TimeSpan timeOfDay, out double oaDate)
        {
            shortString = fShortString(t);
            day = fDay(t);
            ticks = fTicks(t);
            dayOfWeek = fDayOfWeek(t);
            date = fDate(t);
            timeOfDay = fTimeOfDay(t);
            oaDate = fOADate(t);
            return true;
        }

        private void logTime(String desc, Action action)
        {
            Stopwatch w = new Stopwatch();
            Trace.WriteLine("Start>>>" + desc);
            w.Start();
            action.Invoke();
            w.Stop();
            Trace.WriteLine($"----{desc}: {w.ElapsedMilliseconds}/ms");
        }

        private void repeatAdding(Repository<DateTime, String, int, long, DayOfWeek, DateTime, TimeSpan, double> repo1,
            Repository<DateTime, String, int, long, DayOfWeek, DateTime, TimeSpan, double> repo2, int times)
        {
            DateTime now = DateTime.Now;
            Trace.WriteLine($"++++++Adding {times} times when {nameof(funcDurationMills)}={funcDurationMills}");
            logTime(nameof(repo1), () =>
            {
                for (int i = 0; i < times; i++)
                {
                    repo1.Get(now.AddSeconds(i), out string shortString, out int day, out long ticks,
                        out DayOfWeek dayOfWeek, out DateTime date, out TimeSpan timeOfDay, out double oaDate);
                }
            });
            logTime(nameof(repo2), () =>
            {
                for (int i = 0; i < times; i++)
                {
                    repo2.Get(now.AddSeconds(i), out string shortString, out int day, out long ticks,
                        out DayOfWeek dayOfWeek, out DateTime date, out TimeSpan timeOfDay, out double oaDate);
                }
            });
            repo1.Clear();
            repo2.Clear();
        }

        [TestMethod]
        public void TestRepositoryPerformance_RunParallelEnabled()
        {
            Trace.WriteLine("Test Parallel execution");
            Repository.ExecuteParallelly = true;
            var repo1 = new Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double>(getDateTimeDetails);

            var repo2 = new Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double>(fShortString, fDay, fTicks, fDayOfWeek, fDate, fTimeOfDay, fOADate);

            funcDurationMills = 0;
            repeatAdding(repo1, repo2, 100);
            repeatAdding(repo1, repo2, 10000);

            funcDurationMills = 1;
            repeatAdding(repo1, repo2, 100);
            repeatAdding(repo1, repo2, 1000);
        }

        [TestMethod]
        public void TestRepositoryPerformance_RunParallelDisbled_CostSimilarTime()
        {
            Trace.WriteLine("Test Serial execution");
            Repository.ExecuteParallelly = false;
            var repo1 = new Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double>(getDateTimeDetails);

            var repo2 = new Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double>(fShortString, fDay, fTicks, fDayOfWeek, fDate, fTimeOfDay, fOADate);

            funcDurationMills = 0;
            repeatAdding(repo1, repo2, 100);
            repeatAdding(repo1, repo2, 10000);

            funcDurationMills = 1;
            repeatAdding(repo1, repo2, 100);
            repeatAdding(repo1, repo2, 1000);
        }

        [TestMethod]
        public void TestRepositoryPerformance_ComparedWithConcurrentDictionary_WithTime()
        {
            Trace.WriteLine("Test Serial execution");
            Repository.ExecuteParallelly = false;
            var repo1 = new Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double>(getDateTimeDetails);

            var repo2 = new Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double>(getDateTimeDetails,
                new ConcurrentDictionary<DateTime, Tuple<string, int, long, DayOfWeek, DateTime, TimeSpan, double>>());

            funcDurationMills = 0;
            repeatAdding(repo1, repo2, 100);
            repeatAdding(repo1, repo2, 10000);

            funcDurationMills = 1;
            repeatAdding(repo1, repo2, 100);
        }

        private void randomOperation(
            Repository<DateTime, String, int, long, DayOfWeek, DateTime, TimeSpan, double> repo,
            Random random, int times)
        {
            DateTime today = DateTime.Today;

            int deleted = 0;
            int deleteFailed = 0;
            int getFailed = 0;
            for (int i = 0; i < times; i++)
            {
                int next = random.Next(0, 10);
                DateTime time = today.AddMinutes(random.Next(0, 60));
                if (next < 5)
                {
                    if (repo.Get(time) == null)
                        getFailed++;
                }
                else
                {
                    if (!repo.Contains(time))
                        continue;
                    if (repo.Remove(time))
                        deleted++;
                    else
                        deleteFailed++;
                }
            }

            Trace.WriteLine($"Random read/write {times} times, getFailed={getFailed}, deleted={deleted}, deleteFailed={deleteFailed}.");
        }

        private void testMultiThread(Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double> repo, int threadCount, int times)
        {
            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() => randomOperation(repo, new Random(), times));
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        [TestMethod]
        public void TestRepository_Multithread()
        {
            Trace.WriteLine("Test Multi-thread execution");
            Repository.ExecuteParallelly = false;
            //*/
            var repo = new Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double>(getDateTimeDetails
                , new ConcurrentDictionary<DateTime, Tuple<string, int, long, DayOfWeek, DateTime, TimeSpan, double>>());
            logTime("MultiThread random read/write:", () => testMultiThread(repo, 200, 10000));
            /*/// No-threadSafe repository might hang test runner process forever
            var repo = new Repository<DateTime, String, int, long, DayOfWeek, DateTime,
                TimeSpan, double>(getDateTimeDetails);
            logTime("With Simple Repository", () => testMultiThread(repo, 20, 1000));
            //*/

        }
    }
}
