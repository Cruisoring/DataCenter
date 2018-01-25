using System;
using System.Collections.Generic;

namespace DataCenter.Common
{
    //    public class ExpirableRepository<TKey, TValue> :
    //        StatefulRepository<TKey, DateTime, TValue>, IExpirableRepository<TKey, TValue>
    //    {
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            Func<TKey, DateTime> stateFunc,
    //            Func<TKey, TValue> f1,
    //            IDictionary<TKey, Tuple<DateTime, TValue>> store = null)
    //            : base(isValid, stateFunc, f1, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            GetValueDelegate<TKey, DateTime, TValue> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue>> store = null)
    //            : base(isValid, getValueFactory, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            Func<TKey, TValue> f1,
    //            IDictionary<TKey, Tuple<DateTime, TValue>> store = null)
    //            : this((key, time) => time > DateTime.UtcNow, key => DateTime.UtcNow + expiration, f1, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            GetValueDelegate<TKey, DateTime, TValue> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue>> store = null)
    //            : base((key, time) => time > DateTime.UtcNow, getValueFactory, store)
    //        {
    //        }
    //    }
    //
    //    public class ExpirableRepository<TKey, TValue1, TValue2> :
    //        StatefulRepository<TKey, DateTime, TValue1, TValue2>, IExpirableRepository<TKey, TValue1, TValue2>
    //    {
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            Func<TKey, DateTime> stateFunc,
    //            Func<TKey, TValue1> f1,
    //            Func<TKey, TValue2> f2,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2>> store = null)
    //            : base(isValid, stateFunc, f1, f2, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            GetValueDelegate<TKey, DateTime, TValue1, TValue2> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2>> store = null)
    //            : base(isValid, getValueFactory, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2>> store = null)
    //            : this((key, time) => time > DateTime.UtcNow, key => DateTime.UtcNow + expiration, f1, f2, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            GetValueDelegate<TKey, DateTime, TValue1, TValue2> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2>> store = null)
    //            : base((key, time) => time > DateTime.UtcNow, getValueFactory, store)
    //        {
    //        }
    //    }
    //
    //    public class ExpirableRepository<TKey, TValue1, TValue2, TValue3> :
    //        StatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3>,
    //        IExpirableRepository<TKey, TValue1, TValue2, TValue3>
    //    {
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            Func<TKey, DateTime> stateFunc,
    //            Func<TKey, TValue1> f1,
    //            Func<TKey, TValue2> f2,
    //            Func<TKey, TValue3> f3,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3>> store = null)
    //            : base(isValid, stateFunc, f1, f2, f3, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3>> store = null)
    //            : base(isValid, getValueFactory, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3>> store = null)
    //            : this((key, time) => time > DateTime.UtcNow, key => DateTime.UtcNow + expiration, f1, f2, f3, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3>> store = null)
    //            : base((key, time) => time > DateTime.UtcNow, getValueFactory, store)
    //        {
    //        }
    //    }
    //
    //    public class ExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4> :
    //        StatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4>,
    //        IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4>
    //    {
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            Func<TKey, DateTime> stateFunc,
    //            Func<TKey, TValue1> f1,
    //            Func<TKey, TValue2> f2,
    //            Func<TKey, TValue3> f3,
    //            Func<TKey, TValue4> f4,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4>> store = null)
    //            : base(isValid, stateFunc, f1, f2, f3, f4, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4>> store = null)
    //            : base(isValid, getValueFactory, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
    //            Func<TKey, TValue4> f4,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4>> store = null)
    //            : this((key, time) => time > DateTime.UtcNow, key => DateTime.UtcNow + expiration, f1, f2, f3, f4, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4>> store = null)
    //            : base((key, time) => time > DateTime.UtcNow, getValueFactory, store)
    //        {
    //        }
    //    }
    //
    //    public class ExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> :
    //        StatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5>,
    //        IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
    //    {
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            Func<TKey, DateTime> stateFunc,
    //            Func<TKey, TValue1> f1,
    //            Func<TKey, TValue2> f2,
    //            Func<TKey, TValue3> f3,
    //            Func<TKey, TValue4> f4,
    //            Func<TKey, TValue5> f5,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
    //            : base(isValid, stateFunc, f1, f2, f3, f4, f5, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
    //            GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
    //            : base(isValid, getValueFactory, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
    //            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
    //            : this((key, time) => time > DateTime.UtcNow, key => DateTime.UtcNow + expiration,
    //                f1, f2, f3, f4, f5, store)
    //        {
    //        }
    //
    //        public ExpirableRepository(TimeSpan expiration,
    //            GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5> getValueFactory,
    //            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
    //            : base((key, time) => time > DateTime.UtcNow, getValueFactory, store)
    //        {
    //        }
    //    }

    public class ExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> :
        StatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>,
        IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
    {
        protected class ExpirableFactoryWrapper
        {
            private readonly Func<TKey, DateTime> _funDateTime;
            private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getAllValues;
            internal ExpirableFactoryWrapper(TimeSpan expiration, GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValues)
            {
                _funDateTime = key => DateTime.UtcNow.Add(expiration);
                getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));

            }

            internal bool runTogether(TKey key, out DateTime timestamp, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4, out TValue5 v5, out TValue6 v6)
            {
                try
                {
                    bool result = getAllValues.Invoke(key, out v1, out v2, out v3, out v4, out v5, out v6);
                    timestamp = _funDateTime.Invoke(key);
                    return result;
                }
                catch (Exception)
                {
                    return FactoryWrapper<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
                        .ResetAll(key, out timestamp, out v1, out v2, out v3, out v4, out v5, out v6);
                }
            }
        }

        public static GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Wrap(
            TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValues)
        {
            ExpirableFactoryWrapper factory = new ExpirableFactoryWrapper(expiration, getValues);
            return factory.runTogether;
        }

        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
            Func<TKey, DateTime> stateFunc,
            Func<TKey, TValue1> f1,
            Func<TKey, TValue2> f2,
            Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4,
            Func<TKey, TValue5> f5,
            Func<TKey, TValue6> f6,
            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
            : base(isValid, stateFunc, f1, f2, f3, f4, f5, f6, store)
        {
        }

        public ExpirableRepository(Func<TKey, DateTime, bool> isValid,
            GetValueDelegate<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValueFactory,
            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
            : base(isValid, getValueFactory, store)
        {
        }

        public ExpirableRepository(TimeSpan expiration,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6,
            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
            : this((key, time) => time > DateTime.UtcNow,
                key => DateTime.UtcNow + expiration,
                f1, f2, f3, f4, f5, f6, store)
        {
        }

        public ExpirableRepository(TimeSpan expiration,
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValueFactory,
            IDictionary<TKey, Tuple<DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
            : base((key, time) => time > DateTime.UtcNow, Wrap(expiration, getValueFactory), store)
        {
        }

    }
}
