using System;
using System.Collections.Generic;

namespace DataCenter.Common
{
    public class ExpirableRepository<TKey, TValue> : StatefulRepository<TKey, DateTime, TValue>,
        IExpirableRepository<TKey, TValue>
    {
        public ExpirableRepository(TimeSpan expiration,
            Func<TKey, TValue> f1, IDictionary<TKey, (DateTime, TValue)> store = null)
            : base((key, time) => time > DateTime.UtcNow,
                key => DateTime.UtcNow + expiration,
                f1, store)
        {
        }
    }

    public class ExpirableRepository<TKey, TValue1, TValue2> :
        StatefulRepository<TKey, DateTime, TValue1, TValue2>,
        IExpirableRepository<TKey, TValue1, TValue2>
    {
        public static GetValueDelegate<TKey, DateTime, TValue1, TValue2> Wrap(
            TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2> getValues)
        {
            var factory = new ExpirableFactoryWrapper<TKey, TValue1, TValue2>(expiration, getValues);
            return factory.runTogether;
        }

        public ExpirableRepository(TimeSpan expiration,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2,
            IDictionary<TKey, (DateTime, TValue1, TValue2)> store = null)
            : base((key, time) => time > DateTime.UtcNow,
                key => DateTime.UtcNow + expiration,
                f1, f2, store)
        {
        }

        public ExpirableRepository(TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2> getValueFactory,
            IDictionary<TKey, (DateTime, TValue1, TValue2)> store = null)
            : base((key, time) => time > DateTime.UtcNow, Wrap(expiration, getValueFactory), store)
        {
        }
    }

    public class ExpirableRepository<TKey, TValue1, TValue2, TValue3> :
        StatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3>,
        IExpirableRepository<TKey, TValue1, TValue2, TValue3>
    {
        public static GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3> Wrap(
            TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValues)
        {
            var factory = new ExpirableFactoryWrapper<TKey, TValue1, TValue2, TValue3>(expiration, getValues);
            return factory.runTogether;
        }

        public ExpirableRepository(TimeSpan expiration,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            IDictionary<TKey, (DateTime, TValue1, TValue2, TValue3)> store = null)
            : base((key, time) => time > DateTime.UtcNow,
                key => DateTime.UtcNow + expiration,
                f1, f2, f3, store)
        {
        }

        public ExpirableRepository(TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValueFactory,
            IDictionary<TKey, (DateTime, TValue1, TValue2, TValue3)> store = null)
            : base((key, time) => time > DateTime.UtcNow, Wrap(expiration, getValueFactory), store)
        {
        }
    }

    public class ExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4> :
        StatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4>,
        IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4>
    {
        public static GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4> Wrap(
            TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValues)
        {
            var factory = new ExpirableFactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>(expiration, getValues);
            return factory.runTogether;
        }

        public ExpirableRepository(TimeSpan expiration,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4,
            IDictionary<TKey, (DateTime, TValue1, TValue2, TValue3, TValue4)> store = null)
            : base((key, time) => time > DateTime.UtcNow,
                key => DateTime.UtcNow + expiration,
                f1, f2, f3, f4, store)
        {
        }

        public ExpirableRepository(TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValueFactory,
            IDictionary<TKey, (DateTime, TValue1, TValue2, TValue3, TValue4)> store = null)
            : base((key, time) => time > DateTime.UtcNow, Wrap(expiration, getValueFactory), store)
        {
        }
    }

    public class ExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> :
        StatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5>,
        IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
    {
        public static GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5> Wrap(
            TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValues)
        {
            var factory = new ExpirableFactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>(expiration, getValues);
            return factory.runTogether;
        }

        public ExpirableRepository(TimeSpan expiration,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5,
            IDictionary<TKey, (DateTime, TValue1, TValue2, TValue3, TValue4, TValue5)> store = null)
            : base((key, time) => time > DateTime.UtcNow,
                key => DateTime.UtcNow + expiration,
                f1, f2, f3, f4, f5, store)
        {
        }

        public ExpirableRepository(TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValueFactory,
            IDictionary<TKey, (DateTime, TValue1, TValue2, TValue3, TValue4, TValue5)> store = null)
            : base((key, time) => time > DateTime.UtcNow, Wrap(expiration, getValueFactory), store)
        {
        }
    }

    public class ExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> :
        StatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>,
        IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
    {
        public static GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Wrap(
            TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValues)
        {
            var factory = new ExpirableFactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>(expiration, getValues);
            return factory.runTogether;
        }

        public ExpirableRepository(TimeSpan expiration,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6,
            IDictionary<TKey, (DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6)> store = null)
            : base((key, time) => time > DateTime.UtcNow,
                key => DateTime.UtcNow + expiration,
                f1, f2, f3, f4, f5, f6, store)
        {
        }

        public ExpirableRepository(TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValueFactory,
            IDictionary<TKey, (DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6)> store = null)
            : base((key, time) => time > DateTime.UtcNow, Wrap(expiration, getValueFactory), store)
        {
        }
    }
}