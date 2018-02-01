using System;
using System.Collections.Generic;
using System.Text;

namespace DataCenter.MultiKeys
{
    public interface IMultiKeys<TKey1, TKey2>
    {
        bool Contains(TKey1 key1, TKey2 key2);

        bool Remove(TKey1 key1, TKey2 key2);

        int Remove(Func<TKey1, TKey2, bool> predicate);

    }
    public interface IMultiKeys<TKey1, TKey2, TKey3>
    {
        bool Contains(TKey1 key1, TKey2 key2, TKey3 key3);

        bool Remove(TKey1 key1, TKey2 key2, TKey3 key3);

        int Remove(Func<TKey1, TKey2, TKey3, bool> predicate);

    }
}
