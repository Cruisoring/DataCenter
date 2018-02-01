using DataCenter.Common;
using System;
using System.Collections.Generic;


namespace DataCenter.MultiKeys
{
    public class MRepository<TKey1, TKey2, TValue> :
        Repository<Tuple<TKey1, TKey2>, TValue>, IMRepository<TKey1, TKey2, TValue>
    {
        public MRepository(Func<TKey1, TKey2, TValue> valueFactory,
            IDictionary<Tuple<TKey1, TKey2>, TValue> store = null)
            : base(tuple => valueFactory(tuple.Item1, tuple.Item2), store)
        { }

        public bool Contains(TKey1 key1, TKey2 key2) => Contains(Tuple.Create(key1, key2));

        public bool Remove(TKey1 key1, TKey2 key2) => Remove(Tuple.Create(key1, key2));

        public int Remove(Func<TKey1, TKey2, bool> predicate) =>
            Remove(tuple => predicate(tuple.Item1, tuple.Item2));

        public TValue Get(TKey1 key1, TKey2 key2, Func<TKey1, TKey2, TValue> factory) =>
            Get(Tuple.Create(key1, key2), tuple => factory.Invoke(tuple.Item1, tuple.Item2));

        public TValue Get(TKey1 key1, TKey2 key2) => Get(key1, key2, null);

        public TValue this[TKey1 key1, TKey2 key2] => Get(key1, key2, null);


    }
    public class MRepository<TKey1, TKey2, TKey3, TValue> :
        Repository<Tuple<TKey1, TKey2, TKey3>, TValue>, IMRepository<TKey1, TKey2, TKey3, TValue>
    {
        public MRepository(Func<TKey1, TKey2, TKey3, TValue> valueFactory,
            IDictionary<Tuple<TKey1, TKey2, TKey3>, TValue> store = null)
            : base(tuple => valueFactory(tuple.Item1, tuple.Item2, tuple.Item3), store)
        { }

        public bool Contains(TKey1 key1, TKey2 key2, TKey3 key3) => Contains(Tuple.Create(key1, key2, key3));

        public bool Remove(TKey1 key1, TKey2 key2, TKey3 key3) => Remove(Tuple.Create(key1, key2, key3));

        public int Remove(Func<TKey1, TKey2, TKey3, bool> predicate) =>
            Remove(tuple => predicate(tuple.Item1, tuple.Item2, tuple.Item3));

        public TValue Get(TKey1 key1, TKey2 key2, TKey3 key3, Func<TKey1, TKey2, TKey3, TValue> factory) =>
            Get(Tuple.Create(key1, key2, key3), tuple => factory.Invoke(tuple.Item1, tuple.Item2, tuple.Item3));

        public TValue Get(TKey1 key1, TKey2 key2, TKey3 key3) => Get(key1, key2, key3, null);

        public TValue this[TKey1 key1, TKey2 key2, TKey3 key3] => Get(key1, key2, key3, null);

    }
    public class MRepository2<TKey1, TKey2, TValue1, TValue2> :
        Repository<Tuple<TKey1, TKey2>, TValue1, TValue2>, IMRepository2<TKey1, TKey2, TValue1, TValue2>
    {
        public MRepository2(Func<TKey1, TKey2, TValue1> f1, Func<TKey1, TKey2, TValue2> f2,
            IDictionary<Tuple<TKey1, TKey2>, Tuple<TValue1, TValue2>> store = null)
            : base(tuple => f1(tuple.Item1, tuple.Item2), tuple => f2(tuple.Item1, tuple.Item2), store)
        { }

        public bool Contains(TKey1 key1, TKey2 key2) => Contains(Tuple.Create(key1, key2));

        public bool Remove(TKey1 key1, TKey2 key2) => Remove(Tuple.Create(key1, key2));

        public int Remove(Func<TKey1, TKey2, bool> predicate) =>
            Remove(tuple => predicate(tuple.Item1, tuple.Item2));

        public Tuple<TValue1, TValue2> Get(TKey1 key1, TKey2 key2,
            Func<TKey1, TKey2, TValue1> f1, Func<TKey1, TKey2, TValue2> f2) => Get(Tuple.Create(key1, key2),
                    FactoryWrapper<Tuple<TKey1, TKey2>, TValue1, TValue2>.Wrap(
                        t => f1.Invoke(t.Item1, t.Item2), t => f2.Invoke(t.Item1, t.Item2), ExecuteParallelly)
                );

        public Tuple<TValue1, TValue2> this[TKey1 key1, TKey2 key2] => Get(Tuple.Create(key1, key2));

        public Tuple<TValue1, TValue2> Get(TKey1 key1, TKey2 key2) => Get(Tuple.Create(key1, key2));

    }

    public class MRepository2<TKey1, TKey2, TKey3, TValue1, TValue2> :
        Repository<Tuple<TKey1, TKey2, TKey3>, TValue1, TValue2>, IMRepository2<TKey1, TKey2, TKey3, TValue1, TValue2>
    {
        public MRepository2(Func<TKey1, TKey2, TKey3, TValue1> f1, Func<TKey1, TKey2, TKey3, TValue2> f2,
            IDictionary<Tuple<TKey1, TKey2, TKey3>, Tuple<TValue1, TValue2>> store = null)
            : base(tuple => f1(tuple.Item1, tuple.Item2, tuple.Item3),
                tuple => f2(tuple.Item1, tuple.Item2, tuple.Item3), store)
        { }

        public bool Contains(TKey1 key1, TKey2 key2, TKey3 key3) => Contains(Tuple.Create(key1, key2, key3));

        public bool Remove(TKey1 key1, TKey2 key2, TKey3 key3) => Remove(Tuple.Create(key1, key2, key3));

        public int Remove(Func<TKey1, TKey2, TKey3, bool> predicate) =>
            Remove(tuple => predicate(tuple.Item1, tuple.Item2, tuple.Item3));

        public Tuple<TValue1, TValue2> Get(TKey1 key1, TKey2 key2, TKey3 key3,
            Func<TKey1, TKey2, TKey3, TValue1> f1, Func<TKey1, TKey2, TKey3, TValue2> f2) => Get(
            Tuple.Create(key1, key2, key3),
            FactoryWrapper<Tuple<TKey1, TKey2, TKey3>, TValue1, TValue2>.Wrap(
                t => f1.Invoke(t.Item1, t.Item2, t.Item3),
                t => f2.Invoke(t.Item1, t.Item2, t.Item3), ExecuteParallelly)
        );

        public Tuple<TValue1, TValue2> this[TKey1 key1, TKey2 key2, TKey3 key3] => Get(Tuple.Create(key1, key2, key3));

        public Tuple<TValue1, TValue2> Get(TKey1 key1, TKey2 key2, TKey3 key3) => Get(Tuple.Create(key1, key2, key3));

    }
}
