using System;

namespace DataCenter.Common
{
    public interface IExpirableRepository<TKey, TValue>
        : IStatefulRepository<TKey, DateTime, TValue>
    {
    }

    public interface IExpirableRepository<TKey, TValue1, TValue2>
        : IStatefulRepository<TKey, DateTime, TValue1, TValue2>
    {
    }

    public interface IExpirableRepository<TKey, TValue1, TValue2, TValue3>
        : IStatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3>
    {
    }

    public interface IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4>
        : IStatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4>
    {
    }

    public interface IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
        : IStatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5>
    {
    }

    public interface IExpirableRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
        : IStatefulRepository<TKey, DateTime, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
    {
    }
}