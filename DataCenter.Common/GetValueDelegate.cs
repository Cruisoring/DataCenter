using System;
using System.Collections.Generic;
using System.Text;

public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>(TKey key,
    out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7);

namespace DataCenter.Common
{
    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2>(TKey key, out TValue1 value1, out TValue2 value2);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3>(TKey key,
        out TValue1 value1, out TValue2 value2, out TValue3 value3);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3, TValue4>(TKey key,
        out TValue1 value1, out TValue2 value2, out TValue3 value3, out TValue4 value4);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3, TValue4, TValue5>(TKey key,
        out TValue1 value1, out TValue2 value2, out TValue3 value3, out TValue4 value4, out TValue5 value5);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>(TKey key,
        out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6);
}
