using System;
using System.Collections.Generic;
using System.Text;

namespace DataCenter.Common
{
    public interface IStateful { }

    public interface IStateful<in TKey, in TState> : IStateful
    {
        bool IsValid(TKey key);
    }
}
