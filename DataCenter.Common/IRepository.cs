using System;

namespace DataCenter.Common
{
    public interface IRepository<TKey> //: IDisposable
    {
        bool Contains(TKey key);

        bool Remove(TKey key);

        int Remove(Predicate<TKey> predicate);

        void Clear();
    }

    public interface IRepository<TKey, TValue> : IRepository<TKey>
    {
        Func<TKey, TValue> DefaultFactory { get; }

        TValue Get(TKey key);

        TValue this[TKey key] { get; }

        bool Get(TKey key, out TValue value);
    }

    public interface IRepository<TKey, TValue1, TValue2> : IRepository<TKey>
    {
        GetValueDelegate<TKey, TValue1, TValue2> DefaultFactory { get; }
        Tuple<TValue1, TValue2> this[TKey key] { get; }

        Tuple<TValue1, TValue2> Get(TKey key);

        bool Get(TKey key, out TValue1 value1, out TValue2 value2);

        bool Get(TKey key, out TValue1 value1);

        bool Get(TKey key, out TValue2 value2);
    }

    public interface IRepository<TKey, TValue1, TValue2, TValue3> : IRepository<TKey>
    {
        GetValueDelegate<TKey, TValue1, TValue2, TValue3> DefaultFactory { get; }
        Tuple<TValue1, TValue2, TValue3> this[TKey key] { get; }

        Tuple<TValue1, TValue2, TValue3> Get(TKey key);

        bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3);

        bool Get(TKey key, out TValue1 v1);

        bool Get(TKey key, out TValue2 value2);

        bool Get(TKey key, out TValue3 value3);
    }

    public interface IRepository<TKey, TValue1, TValue2, TValue3, TValue4> : IRepository<TKey>
    {
        GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> DefaultFactory { get; }
        Tuple<TValue1, TValue2, TValue3, TValue4> this[TKey key] { get; }

        Tuple<TValue1, TValue2, TValue3, TValue4> Get(TKey key);

        bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4);

        bool Get(TKey key, out TValue1 v1);

        bool Get(TKey key, out TValue2 v2);

        bool Get(TKey key, out TValue3 v3);

        bool Get(TKey key, out TValue4 v4);
    }

    public interface IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> : IRepository<TKey>
    {
        GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> DefaultFactory { get; }
        Tuple<TValue1, TValue2, TValue3, TValue4, TValue5> this[TKey key] { get; }

        Tuple<TValue1, TValue2, TValue3, TValue4, TValue5> Get(TKey key);

        bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5);

        bool Get(TKey key, out TValue1 v1);

        bool Get(TKey key, out TValue2 v2);

        bool Get(TKey key, out TValue3 v3);

        bool Get(TKey key, out TValue4 v4);

        bool Get(TKey key, out TValue5 v5);
    }

    public interface IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> : IRepository<TKey>
    {
        GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> DefaultFactory { get; }
        Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> this[TKey key] { get; }

        Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Get(TKey key);

        bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6);

        bool Get(TKey key, out TValue1 v1);

        bool Get(TKey key, out TValue2 v2);

        bool Get(TKey key, out TValue3 v3);

        bool Get(TKey key, out TValue4 v4);

        bool Get(TKey key, out TValue5 v5);

        bool Get(TKey key, out TValue6 v6);
    }

    public interface IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> : IRepository<TKey>
    {
        GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> DefaultFactory { get; }
        Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> this[TKey key] { get; }

        Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> Get(TKey key);

        bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7);

        bool Get(TKey key, out TValue1 v1);

        bool Get(TKey key, out TValue2 v2);

        bool Get(TKey key, out TValue3 v3);

        bool Get(TKey key, out TValue4 v4);

        bool Get(TKey key, out TValue5 v5);

        bool Get(TKey key, out TValue6 v6);

        bool Get(TKey key, out TValue7 v7);
    }
}