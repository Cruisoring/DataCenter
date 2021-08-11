using DataCenter.Common;
using System;
using System.Collections.Generic;


namespace DataCenter.MultiKeys
{
    public class MRepository<TKey1, TKey2, TValue> :
        Repository<(TKey1, TKey2), TValue>, IMRepository<TKey1, TKey2, TValue>
    {
        public MRepository(Func<TKey1, TKey2, TValue> valueFactory,
            IDictionary<(TKey1, TKey2), TValue> store = null)
            : base(tuple => valueFactory(tuple.Item1, tuple.Item2), store)
        {
        }

        private Func<(TKey1, TKey2), TValue> wrap(Func<TKey1, TKey2, TValue> factory)
        {
            if (factory is null)
                return null;
            return tuple => factory.Invoke(tuple.Item1, tuple.Item2);
        }

        public bool Contains(TKey1 key1, TKey2 key2) => Contains((key1, key2));

        public bool Remove(TKey1 key1, TKey2 key2) => Remove((key1, key2));

        public int Remove(Func<TKey1, TKey2, bool> predicate) =>
            Remove(tuple => predicate(tuple.Item1, tuple.Item2));

        public TValue Get(TKey1 key1, TKey2 key2, Func<TKey1, TKey2, TValue> factory) =>
            Get((key1, key2), wrap(factory));

        public TValue Get(TKey1 key1, TKey2 key2) => Get((key1, key2), null);

        public TValue this[TKey1 key1, TKey2 key2] => Get((key1, key2), null);
    }

    public class MRepository<TKey1, TKey2, TKey3, TValue> :
        Repository<(TKey1, TKey2, TKey3), TValue>, IMRepository<TKey1, TKey2, TKey3, TValue>
    {
        public MRepository(Func<TKey1, TKey2, TKey3, TValue> valueFactory,
            IDictionary<(TKey1, TKey2, TKey3), TValue> store = null)
            : base(tuple => valueFactory(tuple.Item1, tuple.Item2, tuple.Item3), store)
        {
        }

        private Func<(TKey1, TKey2, TKey3), TValue> wrap(Func<TKey1, TKey2, TKey3, TValue> factory)
        {
            if (factory is null)
                return null;
            return tuple => factory.Invoke(tuple.Item1, tuple.Item2, tuple.Item3);
        }

        public bool Contains(TKey1 key1, TKey2 key2, TKey3 key3) => Contains((key1, key2, key3));

        public bool Remove(TKey1 key1, TKey2 key2, TKey3 key3) => Remove((key1, key2, key3));

        public int Remove(Func<TKey1, TKey2, TKey3, bool> predicate) =>
            Remove(tuple => predicate(tuple.Item1, tuple.Item2, tuple.Item3));

        public TValue Get(TKey1 key1, TKey2 key2, TKey3 key3, Func<TKey1, TKey2, TKey3, TValue> factory) =>
            Get((key1, key2, key3), wrap(factory));

        public TValue Get(TKey1 key1, TKey2 key2, TKey3 key3) => Get((key1, key2, key3), null);

        public TValue this[TKey1 key1, TKey2 key2, TKey3 key3] => Get((key1, key2, key3), null);
    }

}