using DataCenter.Common;
using System;

namespace DataCenter.MultiKeys
{
    public interface IMRepository<TKey1, TKey2, TValue> :
        IMultiKeys<TKey1, TKey2>,
        IRepository<Tuple<TKey1, TKey2>, TValue>
    {
        TValue Get(TKey1 key1, TKey2 key2, Func<TKey1, TKey2, TValue> factory);

        TValue Get(TKey1 key1, TKey2 key2);

        TValue this[TKey1 key1, TKey2 key2] { get; }

    }
    public interface IMRepository<TKey1, TKey2, TKey3, TValue> :
        IMultiKeys<TKey1, TKey2, TKey3>,
        IRepository<Tuple<TKey1, TKey2, TKey3>, TValue>
    {
        TValue Get(TKey1 key1, TKey2 key2, TKey3 key3, Func<TKey1, TKey2, TKey3, TValue> factory);

        TValue Get(TKey1 key1, TKey2 key2, TKey3 key3);

        TValue this[TKey1 key1, TKey2 key2, TKey3 key3] { get; }
    }

    public interface IMRepository2<TKey1, TKey2, TValue1, TValue2> :
        IMultiKeys<TKey1, TKey2>,
        IRepository<Tuple<TKey1, TKey2>, TValue1, TValue2>
    {
        Tuple<TValue1, TValue2> Get(TKey1 key1, TKey2 key2,
            Func<TKey1, TKey2, TValue1> f1, Func<TKey1, TKey2, TValue2> f2);

        Tuple<TValue1, TValue2> this[TKey1 key1, TKey2 key2] { get; }

        Tuple<TValue1, TValue2> Get(TKey1 key1, TKey2 key2);

    }
    public interface IMRepository2<TKey1, TKey2, TKey3, TValue1, TValue2> :
        IMultiKeys<TKey1, TKey2, TKey3>,
        IRepository<Tuple<TKey1, TKey2, TKey3>, TValue1, TValue2>
    {
        Tuple<TValue1, TValue2> Get(TKey1 key1, TKey2 key2, TKey3 key3,
            Func<TKey1, TKey2, TKey3, TValue1> f1, Func<TKey1, TKey2, TKey3, TValue2> f2);

        Tuple<TValue1, TValue2> this[TKey1 key1, TKey2 key2, TKey3 key3] { get; }

        Tuple<TValue1, TValue2> Get(TKey1 key1, TKey2 key2, TKey3 key3);
    }
}
