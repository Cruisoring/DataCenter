using DataCenter.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace DataCenter.UnitTest
{
    [TestClass]
    public class BasicRepositoryTests
    {
        #region Single Value Repository Tests
        public static Func<int, String> AlternativeFunc = i => $"{i} with factory";

        [TestMethod]
        public void TestSingleValueRepository_GetWithNewKey_ShallReturnSolidValue()
        {
            Repository<int, String> intRepository = new Repository<int, string>(i => "Default Value of " + i);
            string v3 = intRepository.Get(3);
            Assert.AreEqual(v3, "Default Value of 3");

            string v4 = intRepository[4];
            Assert.AreEqual(v4, "Default Value of 4");

            //            String v5 = intRepository[5, AlternativeFunc];
            //            Assert.AreEqual(v5, "5 with factory");
        }

        [TestMethod]
        public void TestSingleValueRepository_GetWithExistingKey_ShallReturnExistingValue()
        {
            Repository<int, String> intRepository = new Repository<int, string>(i => "Default Value of " + i);
            string v3 = intRepository.Get(3);
            Assert.AreEqual(v3, "Default Value of 3");

            string temp = intRepository[3];
            Assert.AreEqual(temp, "Default Value of 3");

            //            temp = intRepository[3, AlternativeFunc];
            //            Assert.AreEqual(temp, "Default Value of 3");

        }

        [TestMethod]
        public void TestSingleValueRepository_RemoveOneOrMore_ShallBeSuccessful()
        {
            Repository<int, String> intRepository = new Repository<int, string>(i => "Default Value of " + i);
            for (int i = 0; i < 12; i++)
            {
                intRepository.Get(i);
            }
            String temp = intRepository.Get(3);
            Assert.AreEqual(temp, "Default Value of 3");

            intRepository.Remove(3);
            //            temp = intRepository[3, AlternativeFunc];
            //            Assert.AreEqual(temp, "3 with factory");

            int deleted = intRepository.Remove(i => i > 7);
            Assert.AreEqual(deleted, 4);
        }

        #endregion

        #region Dual Value Repository Tests

        private bool defaultFactory2(int i, out bool b, out string s)
        {
            b = i % 2 == 0;
            s = $"{i} is even: {b}";
            return true;
        }

        private bool alternativeFactory2(int i, out bool b, out string s)
        {
            b = i % 3 == 0;
            s = $"{i} can be divided by 3: {b}";
            return b;
        }

        [TestMethod]
        public void TestDualValueRepository_ConstructorWithFunctions_ShallReturnSolidValue()
        {
            Repository<int, bool, String> intRepository = new Repository<int, bool, string>(i => i % 2 == 0, i => $"{i} is even: {i % 2 == 0}");
            intRepository.Get(3, out bool b, out string s);
            Assert.AreEqual(b, false);
            Assert.AreEqual(s, "3 is even: False");

            intRepository.Get(4, out b);
            Assert.AreEqual(b, true);

            var result = intRepository.Get(22, out s);
            Assert.AreEqual(s, "22 is even: True");

            //            var tuple = intRepository[33, alternativeFactory2];
            //            Assert.AreEqual(true, tuple.Item1);
            //            Assert.AreEqual("33 can be divided by 3: True", tuple.Item2);
        }

        [TestMethod]
        public void TestDualValueRepository_GetWithNewKey_ShallReturnSolidValue()
        {
            Repository<int, bool, String> intRepository = new Repository<int, bool, string>(defaultFactory2);
            intRepository.Get(3, out bool b, out string s);
            Assert.AreEqual(b, false);
            Assert.AreEqual(s, "3 is even: False");

            intRepository.Get(4, out b);
            Assert.AreEqual(b, true);

            var result = intRepository.Get(22, out s);
            Assert.AreEqual(s, "22 is even: True");

            //            var tuple = intRepository[33, alternativeFactory2];
            //            Assert.AreEqual(true, tuple.Item1);
            //            Assert.AreEqual("33 can be divided by 3: True", tuple.Item2);
        }

        [TestMethod]
        public void TestDualValueRepository_GetWithExistingKey_ShallReturnExistingValue()
        {
            Repository<int, bool, String> intRepository = new Repository<int, bool, string>(defaultFactory2);
            intRepository.Get(3, out bool b, out string s);
            Assert.AreEqual(b, false);
            Assert.AreEqual(s, "3 is even: False");

            //            intRepository.Get(3, alternativeFactory2);
            //            Assert.AreEqual(b, false);
            //            Assert.AreEqual(s, "3 is even: False");
        }

        [TestMethod]
        public void TestDualValueRepository_RemoveOneOrMore_ShallBeSuccessful()
        {
            Repository<int, bool, String> intRepository = new Repository<int, bool, string>(defaultFactory2);
            for (int i = 0; i < 10; i++)
            {
                var vTuple = intRepository[i];
            }
            intRepository.Get(3, out bool b, out string s);
            Assert.AreEqual(b, false);
            Assert.AreEqual(s, "3 is even: False");

            intRepository.Remove(3);
            //            var tuple = intRepository.Get(3, alternativeFactory2);
            //            Assert.AreEqual(tuple.Item1, true);
            //            Assert.AreEqual(tuple.Item2, "3 can be divided by 3: True");

        }


        private bool factory3(int i, out string v1, out string v2)
        {
            v1 = $"{i} could be devided by 2: {i % 2 == 0}";
            v2 = $"{i} could be devided by 3: {i % 3 == 0}";
            return true;
        }

        private bool factory4(int i, out string v1, out string v2)
        {
            v1 = null;
            v2 = String.Empty;
            return true;
        }


        [TestMethod]
        public void TestDualValueRepositoryOfSameTypes_GetWithNewKey_ShallReturnSolidValue()
        {
            Repository<int, String, String> intRepository = new Repository<int, string, string>(factory3);
            intRepository.Get(3, out string value1, out string value2);
            Assert.AreEqual(value1, "3 could be devided by 2: False");
            Assert.AreEqual(value2, "3 could be devided by 3: True");

            //Following operation cannot be compiled
            //            intRepository.Get(4, out value1);
            //            var result = intRepository.Get(22, out String v3);

            //            var tuple = intRepository[33, factory4];
            //            Assert.AreEqual(null, tuple.Item1);
            //            Assert.AreEqual(String.Empty, tuple.Item2);
        }

        [TestMethod]
        public void TestDualValueRepositoryOfSameTypes_GetWithExistingKey_ShallReturnExistingValue()
        {
            Repository<int, String, String> intRepository = new Repository<int, string, string>(factory3);
            intRepository.Get(3, out string value1, out string value2);

            intRepository.Get(3, out value1, out value2);
            Assert.AreEqual(value1, "3 could be devided by 2: False");
            Assert.AreEqual(value2, "3 could be devided by 3: True");
        }

        [TestMethod]
        public void TestDualValueRepositoryOfSameTypes_RemoveOneOrMore_ShallBeSuccessful()
        {
            Repository<int, String, String> intRepository = new Repository<int, string, string>(factory3);
            for (int i = 0; i < 10; i++)
            {
                var vTuple = intRepository[i];
            }
            intRepository.Get(3, out string v1, out string v2);
            Assert.AreEqual(v1, "3 could be devided by 2: False");
            Assert.AreEqual(v2, "3 could be devided by 3: True");

            intRepository.Remove(3);
            //            var tuple = intRepository.Get(3, factory4);
            //            Assert.AreEqual(tuple.Item1, null);
            //            Assert.AreEqual(tuple.Item2, String.Empty);

            int deleted = intRepository.Remove(i => i % 4 == 0);
            Assert.AreEqual(3, deleted);
        }

        [TestMethod]
        public void TestDualValueRepository_MeasurePerformance_LogExecutedTime()
        {
            Repository<int, bool, String> intRepository = new Repository<int, bool, string>(defaultFactory2);
            int aMillion = 1000000;
            Stopwatch w = new Stopwatch();
            Console.WriteLine("Start getting 1 million entries");
            w.Start();
            for (int i = 0; i < aMillion; i++)
            {
                intRepository.Get(i);
            }
            w.Stop();
            Console.WriteLine($"     Performance: {w.ElapsedMilliseconds}/ms\r\n-");

            Console.WriteLine("1.2: 1 Million random Get calls");
            intRepository = new Repository<int, bool, string>(defaultFactory2);
            Random r = new Random();
            string val = "";
            bool bValue;
            w.Restart();
            for (long i = 0; i < aMillion; i++)
                intRepository.Get(r.Next(0, aMillion), out bValue, out val);
            w.Stop();

            Console.WriteLine($"     Performance: {w.ElapsedMilliseconds}/ms\r\n-");

            Console.WriteLine("1.3: Removing 1 Million entries (with exists check)");
            w.Restart();
            int deleted = 0;
            for (int i = 0; i < aMillion; i++)
                if (intRepository.Remove(i)) deleted++;
            w.Stop();

            Console.WriteLine($"  {deleted} entris deleted,    Performance: {w.ElapsedMilliseconds}/ms\r\n-");
        }

        #endregion

        #region Tri-Value Repository Tests

        private bool triFactory1(int i, out bool v1, out string v2, out DateTime v3)
        {
            v1 = i % 2 == 0;
            v2 = $"{i} is even: {v1}";
            v3 = DateTime.Now.AddDays(i);
            return true;
        }

        private bool triFactory2(int i, out bool v1, out string v2, out DateTime v3)
        {
            v1 = i % 3 == 0;
            v2 = $"{i} can be divided by 3: {v1}";
            v3 = DateTime.Now.AddHours(i);
            return true;
        }

        [TestMethod]
        public void TestTriValueRepository_ConstructorWithFunctions_ShallReturnSolidValue()
        {
            DateTime now = DateTime.Now;
            int hours = now.Hour;

            Repository<int, bool, String, DateTime> Repository = new Repository<int, bool, string, DateTime>(i => i % 2 == 0,
                i => $"{i} is even: {i % 2 == 0}", i => DateTime.Now.AddDays(i));
            Repository.Get(3, out bool v1, out string v2, out DateTime v3);
            Assert.AreEqual(v1, false);
            Assert.AreEqual(v2, "3 is even: False");
            Assert.AreEqual(v3.Hour, hours);

            Repository.Get(4, out v1);
            Assert.AreEqual(v1, true);

            var result = Repository.Get(22, out v2);
            Assert.AreEqual(v2, "22 is even: True");

            //            var tuple = Repository[33, triFactory2];
            //            Assert.AreEqual(true, tuple.Item1);
            //            Assert.AreEqual("33 can be divided by 3: True", tuple.Item2);
            //            Assert.AreEqual(tuple.Item3.Hour, (hours + 33) % 24);
        }

        [TestMethod]
        public void TestTriValueRepository_GetWithNewKey_ShallReturnSolidValue()
        {
            DateTime now = DateTime.Now;
            int hours = now.Hour;

            Repository<int, bool, String, DateTime> Repository = new Repository<int, bool, string, DateTime>(triFactory1);
            Repository.Get(3, out bool v1, out string v2, out DateTime v3);
            Assert.AreEqual(v1, false);
            Assert.AreEqual(v2, "3 is even: False");
            Assert.AreEqual(v3.Hour, hours);

            Repository.Get(4, out v1);
            Assert.AreEqual(v1, true);

            var result = Repository.Get(22, out v2);
            Assert.AreEqual(v2, "22 is even: True");

            //            var tuple = Repository[33, triFactory2];
            //            Assert.AreEqual(true, tuple.Item1);
            //            Assert.AreEqual("33 can be divided by 3: True", tuple.Item2);
            //            Assert.AreEqual(tuple.Item3.Hour, (hours + 33) % 24);
        }

        [TestMethod]
        public void TestTriValueRepository_GetWithExistingKey_ShallReturnExistingValue()
        {
            DateTime now = DateTime.Now;
            int hours = now.Hour;

            Repository<int, bool, String, DateTime> Repository = new Repository<int, bool, string, DateTime>(triFactory1);
            Repository.Get(3, out bool v1, out string v2, out DateTime v3);
            Assert.AreEqual(v1, false);
            Assert.AreEqual(v2, "3 is even: False");
            Assert.AreEqual(v3.Hour, hours);

            Repository.Get(3, out v1);
            Assert.AreEqual(v1, false);

            //Failed to update values of 3 with new GetValueDelegate
            //            var tuple = Repository[3, triFactory2];
            //            Assert.AreEqual(false, tuple.Item1);
            //            Assert.AreEqual("3 is even: False", tuple.Item2);
            //            Repository.Get(3, out v3);
            //            Assert.AreEqual(v3.Hour, hours);
        }

        [TestMethod]
        public void TestTriValueRepository_RemoveOneOrMore_ShallBeSuccessful()
        {
            DateTime now = DateTime.Now;
            int hours = now.Hour;

            Repository<int, bool, String, DateTime> Repository = new Repository<int, bool, string, DateTime>(triFactory1);
            for (int i = 0; i < 20; i++)
            {
                var vTuple = Repository[i];
            }
            Repository.Get(3, out bool v1, out string v2, out DateTime v3);
            Assert.AreEqual(v1, false);
            Assert.AreEqual(v2, "3 is even: False");
            Assert.AreEqual(v3.Hour, hours);

            Repository.Remove(3);
            //            var tuple = Repository.Get(3, triFactory2);
            //            Assert.AreEqual(tuple.Item1, true);
            //            Assert.AreEqual(tuple.Item2, "3 can be divided by 3: True");
            //            Assert.AreEqual(tuple.Item3.Hour, (hours + 3) % 24);

            int deleted = Repository.Remove(i => i % 5 == 0);
            Assert.AreEqual(deleted, 4);
        }


        private bool triFactory3(int i, out string v1, out string v2, out object v3)
        {
            v1 = $"{i} is even: {i % 2 == 0}";
            v2 = $"{i} can be devided by 3: {i % 3 == 0}";
            v3 = $"{i}";
            return true;
        }

        private bool triFactory4(int i, out string v1, out string v2, out object v3)
        {
            v1 = $"even: {i % 2 == 0}";
            v2 = $"be devided by 3: {i % 3 == 0}";
            v3 = $"{i - 1}";
            return true;
        }

        [TestMethod]
        public void TestTriValueRepositoryOfSameTypes_GetWithNewKey_ShallReturnSolidValue()
        {
            Repository<int, String, String, Object> Repository = new Repository<int, string, string, object>(triFactory3);
            Repository.Get(3, out string value1, out string value2, out object value3);
            Repository.Get(3, out Object objValue);
            Assert.AreEqual("3", objValue);

            //Following operation cannot be compiled
            //             Repository.Get(3, out string v);

        }

        [TestMethod]
        public void TestTriValueRepositoryOfSameTypes_GetWithExistingKey_ShallReturnExistingValue()
        {
            Repository<int, String, String, Object> Repository = new Repository<int, string, string, object>(triFactory3);
            Repository.Get(3, out string value1, out string value2, out object value3);
            Repository.Get(3, out Object objValue);
            Assert.AreEqual("3", objValue);

            //            var tuple = Repository.Get(3, triFactory4);
            //            Assert.AreEqual(tuple.Item1, "3 is even: False");
            //            Assert.AreEqual(tuple.Item3, "3");

            Repository.Get(3, out value3);
            Assert.AreEqual(value3, "3");
        }

        [TestMethod]
        public void TestTriValueRepositoryOfSameTypes_RemoveOneOrMore_ShallBeSuccessful()
        {
            Repository<int, String, String, Object> Repository = new Repository<int, string, string, object>(triFactory3);
            for (int i = 0; i < 10; i++)
            {
                var vTuple = Repository[i];
            }
            Repository.Get(3, out string v1, out string v2, out object v3);
            Assert.AreEqual(v1, "3 is even: False");
            Assert.AreEqual(v2, "3 can be devided by 3: True");
            Assert.AreEqual(v3, "3");

            Repository.Remove(3);
            //            var tuple = Repository.Get(3, triFactory4);
            //            Assert.AreEqual(tuple.Item3, "2");

            int deleted = Repository.Remove(i => i % 4 == 0);
            Assert.AreEqual(3, deleted);
        }

        [TestMethod]
        public void TestTriValueRepository_MeasurePerformance_LogExecutedTime()
        {
            Repository<int, String, String, Object> Repository = new Repository<int, string, string, object>(triFactory3);
            int aMillion = 1000000;
            Stopwatch w = new Stopwatch();
            Console.WriteLine("Start getting 1 million entries");
            w.Start();
            for (int i = 0; i < aMillion; i++)
            {
                Repository.Get(i);
            }
            w.Stop();
            Console.WriteLine($"     Performance: {w.ElapsedMilliseconds}/ms\r\n-");

            Console.WriteLine("1.2: 1 Million random Get calls");
            Repository = new Repository<int, string, string, object>(triFactory4);
            Random r = new Random();
            string v1, v2;
            object v3;
            w.Restart();
            for (long i = 0; i < aMillion; i++)
                Repository.Get(r.Next(0, aMillion), out v1, out v2, out v3);
            w.Stop();

            Console.WriteLine($"     Performance: {w.ElapsedMilliseconds}/ms\r\n-");

            Console.WriteLine("1.3: Removing 1 Million entries (with exists check)");
            w.Restart();
            int deleted = 0;
            for (int i = 0; i < aMillion; i++)
                if (Repository.Remove(i)) deleted++;
            w.Stop();

            Console.WriteLine($"  {deleted} entris deleted,    Performance: {w.ElapsedMilliseconds}/ms\r\n-");
        }

        #endregion

    }
}
