using System;
using System.Collections.Generic;
using System.Linq;

namespace DataCenter.Common
{
    public abstract class Repository
    {
        public static Action<string> logMessage = null;

        public static bool ExecuteParallelly = false;
    }

    public abstract class Repository<TKey> : Repository, IRepository<TKey>
    {
        protected Action clear;
        protected Func<TKey, bool> contains;
        protected Func<TKey, bool> remove;
        protected Func<ICollection<TKey>> getKeys;

        public void Clear()
        {
            clear();
        }

        public bool Contains(TKey key)
        {
            return contains(key);
        }

        public bool Remove(TKey key)
        {
            return remove(key);
        }

        public int Remove(Predicate<TKey> predicate)
        {
            TKey[] matchedKeys = getKeys().Where(k => predicate(k)).ToArray();
            int count = 0;
            foreach (var key in matchedKeys)
            {
                if (remove(key))
                    count++;
            }
            return count;
        }
    }

    /// <summary>
    /// Simple generic Repository implementation, not thread safe!!!
    /// </summary>
    /// <typeparam name="TKey">Type of the v1.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class Repository<TKey, TValue> : Repository<TKey>, IRepository<TKey, TValue>
    {
        protected readonly IDictionary<TKey, TValue> repository;
        public Func<TKey, TValue> DefaultFactory { get; }

        public Repository(Func<TKey, TValue> valueFactory,
            IDictionary<TKey, TValue> store = null)
        {
            repository = store ?? new Dictionary<TKey, TValue>();
            DefaultFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            initialize();
        }

        protected void initialize()
        {
            contains = repository.ContainsKey;
            clear = repository.Clear;
            remove = repository.Remove;
            getKeys = () => repository.Keys;
        }

        #region Get Operations
        protected TValue Get(TKey key, Func<TKey, TValue> optionalFactory)
        {
            optionalFactory = optionalFactory ?? DefaultFactory;

            TValue value = default(TValue);
            try
            {
                if (!repository.ContainsKey(key))
                {
                    value = optionalFactory.Invoke(key);
                    repository.Add(key, value);
                }
                else
                {
                    value = repository[key];
                }
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
            }
            return value;
        }

        public virtual bool Get(TKey key, out TValue value)
        {
            try
            {
                if (repository.ContainsKey(key))
                {
                    value = DefaultFactory.Invoke(key);
                    repository.Add(key, value);
                }
                else
                {
                    value = repository[key];
                }

                return true;
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                value = default(TValue);
                return false;
            }
        }

        public TValue this[TKey key] => Get(key);

        public TValue Get(TKey key)
        {
            return Get(key, DefaultFactory);
        }
        #endregion
    }

    public class Repository<TKey, TValue1, TValue2> : Repository<TKey>, IRepository<TKey, TValue1, TValue2>
    {
        #region Readonly Properties

        protected readonly IDictionary<TKey, Tuple<TValue1, TValue2>> repository;

        public GetValueDelegate<TKey, TValue1, TValue2> DefaultFactory { get; }

        #endregion

        #region Constructors
        public Repository(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2,
            IDictionary<TKey, Tuple<TValue1, TValue2>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2>.Wrap(f1, f2, ExecuteParallelly);
            initialize();
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2>.Wrap(getValueFactory);
            initialize();
        }

        #endregion

        protected void initialize()
        {
            contains = repository.ContainsKey;
            clear = repository.Clear;
            remove = repository.Remove;
            getKeys = () => repository.Keys;
        }

        #region Retrieve Functions
        protected virtual Tuple<TValue1, TValue2> Get(TKey key, GetValueDelegate<TKey, TValue1, TValue2> factory)
        {
            Tuple<TValue1, TValue2> tuple = null;
            try
            {
                if (!repository.ContainsKey(key))
                {
                    factory = factory ?? DefaultFactory;
                    if (factory.Invoke(key, out TValue1 value1, out TValue2 value2))
                    {
                        tuple = Tuple.Create(value1, value2);
                        repository.Add(key, tuple);
                    }
                }
                else
                    tuple = repository[key];
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
            }

            return tuple;
        }

        public virtual bool Get(TKey key, out TValue1 value1, out TValue2 value2)
        {
            try
            {
                if (repository.ContainsKey(key))
                {
                    var tuple = repository[key];
                    value1 = tuple.Item1;
                    value2 = tuple.Item2;
                    return true;
                }

                if (DefaultFactory.Invoke(key, out value1, out value2))
                {
                    var tuple = Tuple.Create(value1, value2);
                    repository.Add(key, tuple);
                    return true;
                }
                return FactoryWrapper<TKey, TValue1, TValue2>.ResetAll(key, out value1, out value2);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return FactoryWrapper<TKey, TValue1, TValue2>.ResetAll(key, out value1, out value2);
            }
        }

        public virtual Tuple<TValue1, TValue2> this[TKey key] => Get(key, DefaultFactory);

        public virtual Tuple<TValue1, TValue2> Get(TKey key) => Get(key, DefaultFactory);

        public virtual bool Get(TKey key, out TValue1 value1) => Get(key, out value1, out TValue2 value2);

        public virtual bool Get(TKey key, out TValue2 value2) => Get(key, out TValue1 value1, out value2);

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3> : Repository<TKey>,
        IRepository<TKey, TValue1, TValue2, TValue3>
    {
        #region Readonly Properties

        protected readonly IDictionary<TKey, Tuple<TValue1, TValue2, TValue3>> repository;

        public GetValueDelegate<TKey, TValue1, TValue2, TValue3> DefaultFactory { get; }

        #endregion

        #region Constructors
        public Repository(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3>.Wrap(f1, f2, f3);
            initialize();
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3>.Wrap(getValueFactory);
            initialize();
        }

        #endregion

        protected void initialize()
        {
            contains = repository.ContainsKey;
            clear = repository.Clear;
            remove = repository.Remove;
            getKeys = () => repository.Keys;
        }

        #region Retrieve Functions

        protected virtual Tuple<TValue1, TValue2, TValue3> Get(TKey key, GetValueDelegate<TKey, TValue1, TValue2, TValue3> factory)
        {
            Tuple<TValue1, TValue2, TValue3> tuple = null;
            try
            {
                if (!repository.ContainsKey(key))
                {
                    factory = factory ?? DefaultFactory;
                    if (factory.Invoke(key, out TValue1 v1, out TValue2 v2, out TValue3 v3))
                    {
                        tuple = Tuple.Create(v1, v2, v3);
                        repository.Add(key, tuple);
                    }
                }
                else
                    tuple = repository[key];
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
            }

            return tuple;
        }

        public virtual bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3)
        {
            try
            {
                if (repository.ContainsKey(key))
                {
                    var tuple = repository[key];
                    v1 = tuple.Item1;
                    v2 = tuple.Item2;
                    v3 = tuple.Item3;
                    return true;
                }

                if (DefaultFactory.Invoke(key, out v1, out v2, out v3))
                {
                    var tuple = Tuple.Create(v1, v2, v3);
                    repository.Add(key, tuple);
                    return true;
                }
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3>.ResetAll(key, out v1, out v2, out v3);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3>.ResetAll(key, out v1, out v2, out v3);
            }
        }

        public virtual Tuple<TValue1, TValue2, TValue3> this[TKey key]
            => Get(key, DefaultFactory);

        public virtual Tuple<TValue1, TValue2, TValue3> Get(TKey key) => Get(key, DefaultFactory);

        public virtual bool Get(TKey key, out TValue1 v1)
            => Get(key, out v1, out TValue2 value2, out TValue3 value3);

        public virtual bool Get(TKey key, out TValue2 value2)
            => Get(key, out TValue1 value1, out value2, out TValue3 value3);

        public virtual bool Get(TKey key, out TValue3 value3)
            => Get(key, out TValue1 value1, out TValue2 value2, out value3);

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3, TValue4>
        : Repository<TKey>, IRepository<TKey, TValue1, TValue2, TValue3, TValue4>
    {
        #region Readonly Properties

        protected readonly IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4>> repository;

        public GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> DefaultFactory { get; }

        #endregion

        #region Constructors
        public Repository(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>.Wrap(f1, f2, f3, f4);
            initialize();
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>.Wrap(getValueFactory);
            initialize();
        }

        #endregion

        protected void initialize()
        {
            contains = repository.ContainsKey;
            clear = repository.Clear;
            remove = repository.Remove;
            getKeys = () => repository.Keys;
        }


        #region Retrieve Functions

        protected virtual Tuple<TValue1, TValue2, TValue3, TValue4> Get(TKey key, GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> factory)
        {
            Tuple<TValue1, TValue2, TValue3, TValue4> tuple = null;
            try
            {
                if (!repository.ContainsKey(key))
                {
                    factory = factory ?? DefaultFactory;
                    if (factory.Invoke(key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                        out TValue4 v4))
                    {
                        tuple = Tuple.Create(v1, v2, v3, v4);
                        repository.Add(key, tuple);
                    }
                }
                else
                    tuple = repository[key];
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
            }

            return tuple;
        }

        public virtual bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4)
        {
            try
            {
                if (repository.ContainsKey(key))
                {
                    var tuple = repository[key];
                    v1 = tuple.Item1;
                    v2 = tuple.Item2;
                    v3 = tuple.Item3;
                    v4 = tuple.Item4;
                    return true;
                }

                if (DefaultFactory.Invoke(key, out v1, out v2, out v3, out v4))
                {
                    var tuple = Tuple.Create(v1, v2, v3, v4);
                    repository.Add(key, tuple);
                    return true;
                }
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>.ResetAll(key, out v1, out v2, out v3, out v4);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>.ResetAll(key, out v1, out v2, out v3, out v4);
            }
        }

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4> this[TKey key] => Get(key, DefaultFactory);

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4> Get(TKey key) => Get(key, DefaultFactory);

        public virtual bool Get(TKey key, out TValue1 v1) => Get(key, out v1, out TValue2 v2, out TValue3 v3, out TValue4 v4);

        public virtual bool Get(TKey key, out TValue2 v2) => Get(key, out TValue1 v1, out v2, out TValue3 v3, out TValue4 v4);

        public virtual bool Get(TKey key, out TValue3 v3) => Get(key, out TValue1 v1, out TValue2 v2, out v3, out TValue4 v4);

        public virtual bool Get(TKey key, out TValue4 v4) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out v4);

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
        : Repository<TKey>, IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
    {
        #region Readonly Properties

        protected readonly IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5>> repository;

        public GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> DefaultFactory { get; }

        #endregion

        #region Constructors
        public Repository(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>.Wrap(f1, f2, f3, f4, f5);
            initialize();
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>.Wrap(getValueFactory);
            initialize();
        }

        #endregion

        protected void initialize()
        {
            contains = repository.ContainsKey;
            clear = repository.Clear;
            remove = repository.Remove;
            getKeys = () => repository.Keys;
        }


        #region Retrieve Functions

        protected virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5> Get(TKey key, GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> factory)
        {
            Tuple<TValue1, TValue2, TValue3, TValue4, TValue5> tuple = null;
            try
            {
                if (!repository.ContainsKey(key))
                {
                    factory = factory ?? DefaultFactory;
                    if (factory.Invoke(key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                        out TValue4 v4, out TValue5 v5))
                    {
                        tuple = Tuple.Create(v1, v2, v3, v4, v5);
                        repository.Add(key, tuple);
                    }
                }
                else
                    tuple = repository[key];
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
            }

            return tuple;
        }

        public virtual bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5)
        {
            try
            {
                if (repository.ContainsKey(key))
                {
                    var tuple = repository[key];
                    v1 = tuple.Item1;
                    v2 = tuple.Item2;
                    v3 = tuple.Item3;
                    v4 = tuple.Item4;
                    v5 = tuple.Item5;
                    return true;
                }

                if (DefaultFactory.Invoke(key, out v1, out v2, out v3, out v4, out v5))
                {
                    var tuple = Tuple.Create(v1, v2, v3, v4, v5);
                    repository.Add(key, tuple);
                    return true;
                }
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>.ResetAll(key, out v1, out v2, out v3, out v4, out v5);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>.ResetAll(key, out v1, out v2, out v3, out v4, out v5);
            }
        }

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5> this[TKey key] => Get(key, DefaultFactory);

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5> Get(TKey key) => Get(key, DefaultFactory);

        public virtual bool Get(TKey key, out TValue1 v1) => Get(key, out v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5);

        public virtual bool Get(TKey key, out TValue2 v2) => Get(key, out TValue1 v1, out v2, out TValue3 v3, out TValue4 v4, out TValue5 v5);

        public virtual bool Get(TKey key, out TValue3 v3) => Get(key, out TValue1 v1, out TValue2 v2, out v3, out TValue4 v4, out TValue5 v5);

        public virtual bool Get(TKey key, out TValue4 v4) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out v4, out TValue5 v5);
        public virtual bool Get(TKey key, out TValue5 v5) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out v5);

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
        : Repository<TKey>, IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
    {
        #region Readonly Properties

        protected readonly IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> repository;

        public GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> DefaultFactory { get; }

        #endregion

        #region Constructors
        public Repository(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>.Wrap(f1, f2, f3, f4, f5, f6);
            initialize();
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>.Wrap(getValueFactory);
            initialize();
        }

        #endregion

        protected void initialize()
        {
            contains = repository.ContainsKey;
            clear = repository.Clear;
            remove = repository.Remove;
            getKeys = () => repository.Keys;
        }


        #region Retrieve Functions

        protected virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Get(TKey key, GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> factory)
        {
            Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> tuple = null;
            try
            {
                if (!repository.ContainsKey(key))
                {
                    factory = factory ?? DefaultFactory;
                    if (factory.Invoke(key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                        out TValue4 v4, out TValue5 v5, out TValue6 v6))
                    {
                        tuple = Tuple.Create(v1, v2, v3, v4, v5, v6);
                        repository.Add(key, tuple);
                    }
                }
                else
                    tuple = repository[key];
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
            }

            return tuple;
        }

        public virtual bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6)
        {
            try
            {
                if (repository.ContainsKey(key))
                {
                    var tuple = repository[key];
                    v1 = tuple.Item1;
                    v2 = tuple.Item2;
                    v3 = tuple.Item3;
                    v4 = tuple.Item4;
                    v5 = tuple.Item5;
                    v6 = tuple.Item6;
                    return true;
                }

                if (DefaultFactory.Invoke(key, out v1, out v2, out v3, out v4, out v5, out v6))
                {
                    var tuple = Tuple.Create(v1, v2, v3, v4, v5, v6);
                    repository.Add(key, tuple);
                    return true;
                }
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>.
                    ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>.
                    ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6);
            }
        }

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> this[TKey key] => Get(key, DefaultFactory);

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Get(TKey key) => Get(key, DefaultFactory);

        public virtual bool Get(TKey key, out TValue1 v1) => Get(key, out v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6);

        public virtual bool Get(TKey key, out TValue2 v2) => Get(key, out TValue1 v1, out v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6);

        public virtual bool Get(TKey key, out TValue3 v3) => Get(key, out TValue1 v1, out TValue2 v2, out v3, out TValue4 v4, out TValue5 v5, out TValue6 v6);

        public virtual bool Get(TKey key, out TValue4 v4) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out v4, out TValue5 v5, out TValue6 v6);
        public virtual bool Get(TKey key, out TValue5 v5) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out v5, out TValue6 v6);
        public virtual bool Get(TKey key, out TValue6 v6) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out v6);

        #endregion

    }

    public class Repository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>
        : Repository<TKey>, IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>
    {
        #region Readonly Properties

        protected readonly IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>> repository;

        public GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> DefaultFactory { get; }

        #endregion

        #region Constructors
        public Repository(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6, Func<TKey, TValue7> f7,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>.Wrap(f1, f2, f3, f4, f5, f6, f7);
            initialize();
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>>();
            DefaultFactory = FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>.Wrap(getValueFactory);
            initialize();
        }

        #endregion

        protected void initialize()
        {
            contains = repository.ContainsKey;
            clear = repository.Clear;
            remove = repository.Remove;
            getKeys = () => repository.Keys;
        }

        #region Retrieve Functions

        protected virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> Get(TKey key, GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> factory)
        {
            Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> tuple = null;
            try
            {
                if (!repository.ContainsKey(key))
                {
                    factory = factory ?? DefaultFactory;
                    if (factory.Invoke(key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                        out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7))
                    {
                        tuple = Tuple.Create(v1, v2, v3, v4, v5, v6, v7);
                        repository.Add(key, tuple);
                    }
                }
                else
                    tuple = repository[key];
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
            }

            return tuple;
        }

        public virtual bool Get(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7)
        {
            try
            {
                if (repository.ContainsKey(key))
                {
                    var tuple = repository[key];
                    v1 = tuple.Item1;
                    v2 = tuple.Item2;
                    v3 = tuple.Item3;
                    v4 = tuple.Item4;
                    v5 = tuple.Item5;
                    v6 = tuple.Item6;
                    v7 = tuple.Item7;
                    return true;
                }

                if (DefaultFactory.Invoke(key, out v1, out v2, out v3, out v4, out v5, out v6, out v7))
                {
                    var tuple = Tuple.Create(v1, v2, v3, v4, v5, v6, v7);
                    repository.Add(key, tuple);
                    return true;
                }
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>
                    .ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>
                    .ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
            }
        }

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> this[TKey key] => Get(key, DefaultFactory);

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> Get(TKey key) => Get(key, DefaultFactory);

        public virtual bool Get(TKey key, out TValue1 v1) => Get(key, out v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7);

        public virtual bool Get(TKey key, out TValue2 v2) => Get(key, out TValue1 v1, out v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7);

        public virtual bool Get(TKey key, out TValue3 v3) => Get(key, out TValue1 v1, out TValue2 v2, out v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7);

        public virtual bool Get(TKey key, out TValue4 v4) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out v4, out TValue5 v5, out TValue6 v6, out TValue7 v7);
        public virtual bool Get(TKey key, out TValue5 v5) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out v5, out TValue6 v6, out TValue7 v7);
        public virtual bool Get(TKey key, out TValue6 v6) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out v6, out TValue7 v7);
        public virtual bool Get(TKey key, out TValue7 v7) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out v7);

        #endregion
    }
}
