using System;
using System.Collections.Generic;
using System.Text;

namespace DataCenter.Common
{
    public interface IStatefulRepository<TKey, TState, TValue> :
        IStateful<TKey, TState>,
        IRepository<TKey, TState, TValue>
    {
    }

    public interface IStatefulRepository<TKey, TState, TValue1, TValue2> :
        IStateful<TKey, TState>,
        IRepository<TKey, TState, TValue1, TValue2>
    {
    }

    public interface IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3> :
        IStateful<TKey, TState>,
        IRepository<TKey, TState, TValue1, TValue2, TValue3>
    {
    }

    public interface IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4> :
        IStateful<TKey, TState>,
        IRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4>
    {
    }

    public interface IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5> :
        IStateful<TKey, TState>,
        IRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5>
    {
    }

    public interface IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> :
        IStateful<TKey, TState>,
        IRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
    {
    }
}
