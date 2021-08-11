using DataCenter.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DataCenter.MultiKeys;

namespace DataCenter.UnitTest
{
    [TestClass]
    public class MRepositoryTests
    {
        public static Func<object, object, String> DefaultFactory = (a, b) => $"Default Value of {a} and {b}";

        [TestMethod]
        public void TestMultiKeyRepository_GetWithNewKey_ShallReturnSolidValue()
        {
            IDictionary<(object, object), string> preDictionary = new Dictionary<(object, object), string>()
            {
                {(1, 1), "Both ones"},
                {(1, "one"), "ones of dif types"},
            };
            MRepository<object, object, String> mRepository = new MRepository<object, object, string>(DefaultFactory, preDictionary);
            string v3 = mRepository.Get(3, 4);
            Assert.AreEqual(v3, "Default Value of 3 and 4");

            string v4 = mRepository[1, 1];
            Assert.AreEqual(v4, "Both ones");

            String v5 = mRepository[1, "one"];
            Assert.AreEqual(v5, "ones of dif types");

            String v6 = mRepository.Get(1, 2, (o1, o2) => $"{o1} {o2}");
            Assert.AreEqual(v6, "1 2");
        }

    }
}
