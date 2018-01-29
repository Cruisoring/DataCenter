using System;
using System.Threading.Tasks;

namespace DataCenter.Common
{
    public delegate TKey FluentOut<TKey, TValue1>(TKey key, out TValue1 v1);

    public delegate TKey FluentOut<TKey, TValue1, TValue2>(TKey key, out TValue1 v1, out TValue2 v2);

    public static class extentions
    {
        public static FluentOut<K, V1, V2>
            ContinueWith<K, V1, V2>(this FluentOut<K, V1> f1, FluentOut<K, V2> f2) =>
            (K k, out V1 v1, out V2 v2) =>
            {
                f1(k, out v1);
                f2(k, out v2);
                return k;
            };

        public static FluentOut<K, V1, V2>
            ParallelInvoke<K, V1, V2>(this FluentOut<K, V1> f1, FluentOut<K, V2> f2) =>
            (K k, out V1 v1, out V2 v2) =>
            {
                V1 _v1 = default(V1);
                V2 _v2 = default(V2);

                Parallel.Invoke(
                    () => f1(k, out _v1),
                    () => f2(k, out _v2)
                );
                v1 = _v1;
                v2 = _v2;
                return k;
            };

//        public static Func<K, FluentOut<FluentOut<K, V1>, V2>> Curry<K, V1, V2>(
//            this FluentOut<K, V1> f1, FluentOut<K, V2> f2) =>
//
//            k => f1(K k, out V1 v1) => private f2()
    }

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2>(TKey key, out TValue1 value1, out TValue2 value2);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3>(TKey key,
        out TValue1 value1, out TValue2 value2, out TValue3 value3);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3, TValue4>(TKey key,
        out TValue1 value1, out TValue2 value2, out TValue3 value3, out TValue4 value4);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3, TValue4, TValue5>(TKey key,
        out TValue1 value1, out TValue2 value2, out TValue3 value3, out TValue4 value4, out TValue5 value5);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>(TKey key,
        out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6);

    public delegate bool GetValueDelegate<in TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>(TKey key,
        out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7);
}