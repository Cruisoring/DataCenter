using System;

namespace DataCenter.Common
{
    public class ExpirableFactoryWrapper<TKey, TValue1, TValue2>
    {
        private readonly Func<TKey, DateTime> _funDateTime;
        private readonly GetValueDelegate<TKey, TValue1, TValue2> getAllValues;

        internal ExpirableFactoryWrapper(TimeSpan expiration, GetValueDelegate<TKey, TValue1, TValue2> getValues)
        {
            _funDateTime = key => DateTime.UtcNow.Add(expiration);
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runTogether(TKey key, out DateTime timestamp, out TValue1 v1, out TValue2 v2)
        {
            try
            {
                bool result = getAllValues.Invoke(key, out v1, out v2);
                timestamp = _funDateTime.Invoke(key);
                return result;
            }
            catch (Exception)
            {
                return FactoryWrapper<TKey, DateTime, TValue1, TValue2>
                    .ResetAll(key, out timestamp, out v1, out v2);
            }
        }
    }

    public class ExpirableFactoryWrapper<TKey, TValue1, TValue2, TValue3>
    {
        private readonly Func<TKey, DateTime> _funDateTime;
        private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3> getAllValues;

        internal ExpirableFactoryWrapper(TimeSpan expiration, GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValues)
        {
            _funDateTime = key => DateTime.UtcNow.Add(expiration);
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runTogether(TKey key, out DateTime timestamp, out TValue1 v1, out TValue2 v2, out TValue3 v3)
        {
            try
            {
                bool result = getAllValues.Invoke(key, out v1, out v2, out v3);
                timestamp = _funDateTime.Invoke(key);
                return result;
            }
            catch (Exception)
            {
                return FactoryWrapper<TKey, DateTime, TValue1, TValue2, TValue3>
                    .ResetAll(key, out timestamp, out v1, out v2, out v3);
            }
        }
    }

    public class ExpirableFactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>
    {
        private readonly Func<TKey, DateTime> _funDateTime;
        private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getAllValues;

        internal ExpirableFactoryWrapper(TimeSpan expiration, GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValues)
        {
            _funDateTime = key => DateTime.UtcNow.Add(expiration);
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runTogether(TKey key, out DateTime timestamp, out TValue1 v1, out TValue2 v2, out TValue3 v3,
            out TValue4 v4)
        {
            try
            {
                bool result = getAllValues.Invoke(key, out v1, out v2, out v3, out v4);
                timestamp = _funDateTime.Invoke(key);
                return result;
            }
            catch (Exception)
            {
                return FactoryWrapper<TKey, DateTime, TValue1, TValue2, TValue3, TValue4>
                    .ResetAll(key, out timestamp, out v1, out v2, out v3, out v4);
            }
        }
    }

    public class ExpirableFactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
    {
        private readonly Func<TKey, DateTime> _funDateTime;
        private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getAllValues;

        internal ExpirableFactoryWrapper(TimeSpan expiration, GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValues)
        {
            _funDateTime = key => DateTime.UtcNow.Add(expiration);
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runTogether(TKey key, out DateTime timestamp, out TValue1 v1, out TValue2 v2, out TValue3 v3,
            out TValue4 v4, out TValue5 v5)
        {
            try
            {
                bool result = getAllValues.Invoke(key, out v1, out v2, out v3, out v4, out v5);
                timestamp = _funDateTime.Invoke(key);
                return result;
            }
            catch (Exception)
            {
                return FactoryWrapper<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5>
                    .ResetAll(key, out timestamp, out v1, out v2, out v3, out v4, out v5);
            }
        }
    }

    public class ExpirableFactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
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
}