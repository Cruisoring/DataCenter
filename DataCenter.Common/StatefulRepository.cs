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
            IDictionary<TKey, Tuple<TState, TValue>> store = null)
            : base(stateFunc, f1, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue> getValueFactory,
            IDictionary<TKey, Tuple<TState, TValue>> store = null)
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

        protected override Tuple<TState, TValue> Get(TKey key, GetValueDelegate<TKey, TState, TValue> factory)
        {
            factory = factory ?? DefaultFactory;
            if (repository.ContainsKey(key))
            {
                Tuple<TState, TValue> tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue value))
            {
                Tuple<TState, TValue> tuple = Tuple.Create(state, value);
                /*/
                Repository.Add(v1, tuple);
                /*/
                try
                {
                    repository.Add(key, tuple);
                    return tuple;
                }
                catch { }
                //*/
            }
            return null;
        }
    }

    public class StatefulRepository<TKey, TState, TValue1, TValue2> :
        Repository<TKey, TState, TValue1, TValue2>, IStatefulRepository<TKey, TState, TValue1, TValue2>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid, Func<TKey, TState> stateFunc,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2,
            IDictionary<TKey, Tuple<TState, TValue1, TValue2>> store = null)
            : base(stateFunc, f1, f2, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2> getValueFactory,
            IDictionary<TKey, Tuple<TState, TValue1, TValue2>> store = null)
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

        protected override Tuple<TState, TValue1, TValue2> Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2> factory)
        {
            factory = factory ?? DefaultFactory;
            if (repository.ContainsKey(key))
            {
                Tuple<TState, TValue1, TValue2> tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2))
            {
                Tuple<TState, TValue1, TValue2> tuple = Tuple.Create(state, v1, v2);
                /*/
                Repository.Add(v1, tuple);
                /*/
                try
                {
                    repository.Add(key, tuple);
                    return tuple;
                }
                catch { }
                //*/
            }
            return null;
        }
    }

    public class StatefulRepository<TKey, TState, TValue1, TValue2, TValue3> :
        Repository<TKey, TState, TValue1, TValue2, TValue3>,
        IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid, Func<TKey, TState> stateFunc,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            IDictionary<TKey, Tuple<TState, TValue1, TValue2, TValue3>> store = null)
            : base(stateFunc, f1, f2, f3, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3> getValueFactory,
            IDictionary<TKey, Tuple<TState, TValue1, TValue2, TValue3>> store = null)
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

        protected override Tuple<TState, TValue1, TValue2, TValue3> Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3> factory)
        {
            factory = factory ?? DefaultFactory;
            if (repository.ContainsKey(key))
            {
                var tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2, out TValue3 v3))
            {
                var tuple = Tuple.Create(state, v1, v2, v3);
                /*/
                Repository.Add(v1, tuple);
                /*/
                try
                {
                    repository.Add(key, tuple);
                    return tuple;
                }
                catch { }
                //*/
            }
            return null;
        }
    }

    public class StatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4> :
        Repository<TKey, TState, TValue1, TValue2, TValue3, TValue4>,
        IStatefulRepository<TKey, TState, TValue1, TValue2, TValue3, TValue4>
    {
        protected Func<TKey, TState, bool> isValid { get; }

        public StatefulRepository(Func<TKey, TState, bool> isValid, Func<TKey, TState> stateFunc,
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3, Func<TKey, TValue4> f4,
            IDictionary<TKey, Tuple<TState, TValue1, TValue2, TValue3, TValue4>> store = null)
            : base(stateFunc, f1, f2, f3, f4, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4> getValueFactory,
            IDictionary<TKey, Tuple<TState, TValue1, TValue2, TValue3, TValue4>> store = null)
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

        protected override Tuple<TState, TValue1, TValue2, TValue3, TValue4> Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4> factory)
        {
            factory = factory ?? DefaultFactory;
            if (repository.ContainsKey(key))
            {
                var tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4))
            {
                var tuple = Tuple.Create(state, v1, v2, v3, v4);
                /*/
                Repository.Add(v1, tuple);
                /*/
                try
                {
                    repository.Add(key, tuple);
                    return tuple;
                }
                catch { }
                //*/
            }
            return null;
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
            IDictionary<TKey, Tuple<TState, TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
            : base(stateFunc, f1, f2, f3, f4, f5, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5> getValueFactory,
            IDictionary<TKey, Tuple<TState, TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
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

        protected override Tuple<TState, TValue1, TValue2, TValue3, TValue4, TValue5> Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5> factory)
        {
            factory = factory ?? DefaultFactory;
            if (repository.ContainsKey(key))
            {
                var tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5))
            {
                var tuple = Tuple.Create(state, v1, v2, v3, v4, v5);
                /*/
                Repository.Add(v1, tuple);
                /*/
                try
                {
                    repository.Add(key, tuple);
                    return tuple;
                }
                catch { }
                //*/
            }
            return null;
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
            IDictionary<TKey, Tuple<TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
            : base(stateFunc, f1, f2, f3, f4, f5, f6, store)
        {
            this.isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
        }

        public StatefulRepository(Func<TKey, TState, bool> isValid,
            GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValueFactory,
            IDictionary<TKey, Tuple<TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
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

        protected override Tuple<TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Get(TKey key, GetValueDelegate<TKey, TState, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> factory)
        {
            factory = factory ?? DefaultFactory;
            if (repository.ContainsKey(key))
            {
                var tuple = repository[key];
                if (this.isValid(key, tuple.Item1))
                    return tuple;
                repository.Remove(key);
            }

            if (factory.Invoke(key, out TState state, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6))
            {
                var tuple = Tuple.Create(state, v1, v2, v3, v4, v5, v6);
                /*/
                Repository.Add(v1, tuple);
                /*/
                try
                {
                    repository.Add(key, tuple);
                    return tuple;
                }
                catch { }
                //*/
            }
            return null;
        }
    }
}