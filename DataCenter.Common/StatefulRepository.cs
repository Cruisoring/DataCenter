using System;
using System.Collections.Generic;
using System.Linq;

namespace DataCenter.Common
{
    public class StatefulRepository<TKey, TState, TValue> :
        Repository<TKey, TState, TValue>, IStatefulRepository<TKey, TState, TValue>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            Func<TKey, TState> stateFunc,
            Func<TKey, TValue> f1,
            IDictionary<TKey, (TState, TValue)> store = null)
            : base(stateFunc, f1, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue> getValueFactory,
            IDictionary<TKey, (TState, TValue)> store = null)
            : base(getValueFactory, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public virtual bool IsValid(TKey key)
        {
            if (!repository.ContainsKey(key))
                return false;
            var tuple = repository[key];
            return isValid(key, tuple.Item1);
        }

        public virtual int Trim()
        {
            int count = 0;
            var keys = repository.Keys.ToList();
            foreach (var key in keys)
            {
                if (!IsValid(key) && Remove(key))
                    count++;
            }

            return count;
        }

        public override (TState, TValue) Get(TKey key, GetValueDelegate<TKey, TState, TValue> factory)
        {
            factory ??= DefaultFactory;
            if (repository.ContainsKey(key))
            {
                (TState, TValue) tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue value))
            {
                repository.Add(key, (state, value));
            }
            //Let it throw exception if Invoke() failed
            return repository[key];
        }
    }

    public class StatefulRepository<TKey, TState, TValue1, TValue2> :
        Repository<TKey, TState, TValue1, TValue2>, IStatefulRepository<TKey, TState, TValue1, TValue2>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid, Func<TKey, TState> stateFunc,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2,
            IDictionary<TKey, (TState, TValue1, TValue2)> store = null)
            : base(stateFunc, f1, f2, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2> getValueFactory,
            IDictionary<TKey, (TState, TValue1, TValue2)> store = null)
            : base(getValueFactory, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public virtual bool IsValid(TKey key)
        {
            if (!repository.ContainsKey(key))
                return false;
            var tuple = repository[key];
            return isValid(key, tuple.Item1);
        }

        public virtual int Trim()
        {
            int count = 0;
            var keys = repository.Keys.ToList();
            foreach (var key in keys)
            {
                if (!IsValid(key) && Remove(key))
                    count++;
            }

            return count;
        }

        public override (TState, TValue1, TValue2) Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2> factory)
        {
            factory ??= DefaultFactory;
            if (repository.ContainsKey(key))
            {
                (TState, TValue1, TValue2) tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2))
            {
                repository.Add(key, (state, v1, v2));
            }
            //Let it throw exception if Invoke() failed
            return repository[key];
        }
    }

    public class StatefulRepository<TKey, TState, TValue1, TValue2, TValue3> :
        Repository<TKey, TState, TValue1, TValue2, TValue3>,
        IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid, Func<TKey, TState> stateFunc,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            IDictionary<TKey, (TState, TValue1, TValue2, TValue3)> store = null)
            : base(stateFunc, f1, f2, f3, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3> getValueFactory,
            IDictionary<TKey, (TState, TValue1, TValue2, TValue3)> store = null)
            : base(getValueFactory, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public virtual bool IsValid(TKey key)
        {
            if (!repository.ContainsKey(key))
                return false;
            var tuple = repository[key];
            return isValid(key, tuple.Item1);
        }

        public virtual int Trim()
        {
            int count = 0;
            var keys = repository.Keys.ToList();
            foreach (var key in keys)
            {
                if (!IsValid(key) && Remove(key))
                    count++;
            }

            return count;
        }

        public override (TState, TValue1, TValue2, TValue3) Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3> factory)
        {
            factory ??= DefaultFactory;
            if (repository.ContainsKey(key))
            {
                var tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2, out TValue3 v3))
            {
                repository.Add(key, (state, v1, v2, v3));
            }
            //Let it throw exception if Invoke() failed
            return repository[key];
        }
    }

    public class StatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4> :
        Repository<TKey, TState, TValue1, TValue2, TValue3, TValue4>,
        IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid, Func<TKey, TState> stateFunc,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3, Func<TKey, TValue4> f4,
            IDictionary<TKey, (TState, TValue1, TValue2, TValue3, TValue4)> store = null)
            : base(stateFunc, f1, f2, f3, f4, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4> getValueFactory,
            IDictionary<TKey, (TState, TValue1, TValue2, TValue3, TValue4)> store = null)
            : base(getValueFactory, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public virtual bool IsValid(TKey key)
        {
            if (!repository.ContainsKey(key))
                return false;
            var tuple = repository[key];
            return isValid(key, tuple.Item1);
        }

        public virtual int Trim()
        {
            int count = 0;
            var keys = repository.Keys.ToList();
            foreach (var key in keys)
            {
                if (!IsValid(key) && Remove(key))
                    count++;
            }

            return count;
        }

        public override (TState, TValue1, TValue2, TValue3, TValue4) Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4> factory)
        {
            factory ??= DefaultFactory;
            if (repository.ContainsKey(key))
            {
                var tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4))
            {
                repository.Add(key, (state, v1, v2, v3, v4));
            }
            //Let it throw exception if Invoke() failed
            return repository[key];
        }
    }

    public class StatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5> :
        Repository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5>,
        IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid, Func<TKey, TState> stateFunc,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5,
            IDictionary<TKey, (TState, TValue1, TValue2, TValue3, TValue4, TValue5)> store = null)
            : base(stateFunc, f1, f2, f3, f4, f5, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5> getValueFactory,
            IDictionary<TKey, (TState, TValue1, TValue2, TValue3, TValue4, TValue5)> store = null)
            : base(getValueFactory, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public virtual bool IsValid(TKey key)
        {
            if (!repository.ContainsKey(key))
                return false;
            var tuple = repository[key];
            return isValid(key, tuple.Item1);
        }

        public virtual int Trim()
        {
            int count = 0;
            var keys = repository.Keys.ToList();
            foreach (var key in keys)
            {
                if (!IsValid(key) && Remove(key))
                    count++;
            }

            return count;
        }

        public override (TState, TValue1, TValue2, TValue3, TValue4, TValue5) Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5> factory)
        {
            factory ??= DefaultFactory;
            if (repository.ContainsKey(key))
            {
                var tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5))
            {
                repository.Add(key, (state, v1, v2, v3, v4, v5));
            }
            //Let it throw exception if Invoke() failed
            return repository[key];
        }
    }

    public class StatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> :
        Repository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>,
        IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid, Func<TKey, TState> stateFunc,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6,
            IDictionary<TKey, (TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6)> store = null)
            : base(stateFunc, f1, f2, f3, f4, f5, f6, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValueFactory,
            IDictionary<TKey, (TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6)> store = null)
            : base(getValueFactory, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public virtual bool IsValid(TKey key)
        {
            if (!repository.ContainsKey(key))
                return false;
            var tuple = repository[key];
            return isValid(key, tuple.Item1);
        }

        public virtual int Trim()
        {
            int count = 0;
            var keys = repository.Keys.ToList();
            foreach (var key in keys)
            {
                if (!IsValid(key) && Remove(key))
                    count++;
            }

            return count;
        }

        public override (TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6) Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> factory)
        {
            factory ??= DefaultFactory;
            if (repository.ContainsKey(key))
            {
                var tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
                repository.Remove(key);
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6))
            {
                repository.Add(key, (state, v1, v2, v3, v4, v5, v6));
            }
            //Let it throw exception if Invoke() failed
            return repository[key];
        }
    }
}