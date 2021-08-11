using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataCenter.Common;
using DataCenter.MultiKeys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataCenter.UnitTest
{
    public static class Demo
    {
        public static readonly Repository<System.Type, (PropertyInfo[], Dictionary<string, Func<object, object>>)> TypeProperties =
            new Repository<System.Type, (PropertyInfo[], Dictionary<string, Func<object, object>>)>(ParseType);

        internal static (PropertyInfo[], Dictionary<string, Func<object, object>>) ParseType(System.Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            Dictionary<string, Func<object, object>> extractors = propertyInfos.ToDictionary(
                pi => pi.Name,
                pi => (Func<object, object>)(obj => pi.GetValue(obj))
            );
            return (propertyInfos, extractors);
        }

        public static bool TryGetPropertyValue(object source, string propertyName, out object value)
        {
            if (source is null || propertyName is null)
            {
                value = source;
                return false;
            }

            (PropertyInfo[] _, Dictionary<string, Func<object, object>> extractors) = TypeProperties.Get(source.GetType());

            if (extractors.ContainsKey(propertyName))
            {
                value = extractors[propertyName](source);
                return true;
            }

            string caseIgnoredName =
                extractors.Keys.SingleOrDefault(k => k.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));

            if (caseIgnoredName is null)
            {
                value = source;
                Console.WriteLine($"Failed to locate property of {propertyName} from object {source}");
                return false;
            }

            value = extractors[caseIgnoredName](source);
            return true;
        }

        public static IDictionary<(Type, Type), Func<object, object, bool>> PredefinedComparers =
            new Dictionary<(Type, Type), Func<object, object, bool>>()
            {
                {(typeof(int), typeof(long)), (a, b) => Convert.ToInt64(a).Equals(Convert.ToInt64(b))},
                {(typeof(DateTime), typeof(string)), (a, b) => a.Equals(DateTime.Parse(b.ToString()))},
            };

        private static Func<object, object, bool> ComparerFactory(Type typeA, Type typeB)
        {
            if (typeA == typeB) 
                return (a, b) => a == b;
            //Build more complex comparer here, or use the default logic below to compare their string forms
            return (a, b) => a.ToString().Equals(b.ToString(), StringComparison.CurrentCulture);
        }

        public static MRepository<Type, Type, Func<object, object, bool>> TypeComparers =
            new MRepository<Type, Type, Func<object, object, bool>>(ComparerFactory, PredefinedComparers);

        public static bool CompareWithNull<T>(T obj)
        {
            return  (obj is null) || object.Equals(obj, default(T));
        }

        public static bool Compare(object a, object b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return CompareWithNull(a ?? b);
            (Type typeA, Type typeB) = (a.GetType(), b.GetType());
            Func<object, object, bool> comparer = TypeComparers.Contains((typeB, typeA)) ? TypeComparers.Get((typeB, typeA)) : TypeComparers.Get(typeA, typeB);
            return comparer(a, b);
        }
    }

    [TestClass]
    public class DemoTests
    {
        [TestMethod]
        public void TestDemo_TryGetPropertyValue()
        {
            DateTime oneDay = new DateTime(2021, 9, 15);

            Assert.IsTrue(Demo.TryGetPropertyValue(oneDay, "Year", out object year)
                && object.Equals(year, 2021));

            Assert.IsTrue(Demo.TryGetPropertyValue(oneDay, "month", out object month)
                && object.Equals(month, 9));
        }

        [TestMethod]
        public void TestCompare()
        {
            Assert.IsTrue(Demo.Compare(123, "123"));
            Assert.IsTrue(Demo.Compare(123L, 123)
                          && Demo.TypeComparers.Contains((typeof(int), typeof(long)))
                          && !Demo.TypeComparers.Contains((typeof(long), typeof(int))));
        }
    }
}
