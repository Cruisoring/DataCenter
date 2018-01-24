using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataCenter.Common
{
    public abstract class Repository
    {
        public static Action<string> logMessage = null;

        public static bool ExecuteParallelly = false;
    }

    /// <summary>
    /// Simple generic Repository implementation, not thread safe!!!
    /// </summary>
    /// <typeparam name="TKey">Type of the v1.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class Repository<TKey, TValue> : Repository, IRepository<TKey, TValue>
    {
        protected readonly IDictionary<TKey, TValue> repository;
        public Func<TKey, TValue> DefaultFactory { get; }

        public Repository(Func<TKey, TValue> valueFactory,
            IDictionary<TKey, TValue> store = null)
        {
            repository = store ?? new Dictionary<TKey, TValue>();
            DefaultFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public bool Contains(TKey key)
        {
            return repository.ContainsKey(key);
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

        #region Remove Operations
        public bool Remove(TKey key)
        {
            return repository.Remove(key);
        }

        public int Remove(Predicate<TKey> predicate)
        {
            TKey[] matchedKeys = repository.Keys.Where(k => predicate(k)).ToArray();
            int count = 0;
            foreach (var key in matchedKeys)
            {
                if (Remove(key))
                    count++;
            }
            return count;
        }

        public void Clear()
        {
            repository.Clear();
        }
        #endregion

    }

    public class Repository<TKey, TValue1, TValue2> : Repository, IRepository<TKey, TValue1, TValue2>
    {
        protected class FactoryWrapper
        {
            private readonly Func<TKey, TValue1> _fun1;
            private readonly Func<TKey, TValue2> _fun2;
            private readonly GetValueDelegate<TKey, TValue1, TValue2> getAllValues;

            internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2)
            {
                _fun1 = f1 ?? throw new ArgumentNullException(nameof(f1));
                _fun2 = f2 ?? throw new ArgumentNullException(nameof(f2));
                if (ExecuteParallelly)
                    getAllValues = runParallel;
                else
                    getAllValues = runSequential;
            }

            internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2> getValues)
            {
                _fun1 = null;
                _fun2 = null;
                getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
            }

            internal bool runSequential(TKey key, out TValue1 value1, out TValue2 value2)
            {
                try
                {
                    value1 = _fun1.Invoke(key);
                    value2 = _fun2.Invoke(key);
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return ResetAll(key, out value1, out value2);
                }
            }

            internal bool runParallel(TKey key, out TValue1 value1, out TValue2 value2)
            {
                TValue1 v1 = default(TValue1);
                TValue2 v2 = default(TValue2);

                try
                {
                    Parallel.Invoke(
                        () => v1 = _fun1.Invoke(key),
                        () => v2 = _fun2.Invoke(key)
                    );
                    return true;
                }
                catch (AggregateException ex)
                {
                    ex.Handle(exception =>
                    {
                        Trace.WriteLine(exception);
                        return true; //handled
                    });
                    return false;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    value1 = v1;
                    value2 = v2;
                }
            }

            internal bool runTogether(TKey key, out TValue1 value1, out TValue2 value2)
            {
                try
                {
                    return getAllValues.Invoke(key, out value1, out value2);
                }
                catch (Exception)
                {
                    return ResetAll(key, out value1, out value2);
                }
            }
        }

        #region Static methods

        public static bool ResetAll(TKey key, out TValue1 value1, out TValue2 value2)
        {
            value1 = default(TValue1);
            value2 = default(TValue2);
            return false;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2)
        {
            FactoryWrapper factory = new FactoryWrapper(f1, f2);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2> getValues)
        {
            FactoryWrapper factory = new FactoryWrapper(getValues);
            return factory.runTogether;
        }

        #endregion

        #region Readonly Properties

        protected readonly IDictionary<TKey, Tuple<TValue1, TValue2>> repository;

        public GetValueDelegate<TKey, TValue1, TValue2> DefaultFactory { get; }

        #endregion

        #region Constructors
        public Repository(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2,
            IDictionary<TKey, Tuple<TValue1, TValue2>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2>>();
            DefaultFactory = Wrap(f1, f2);
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2>>();
            DefaultFactory = Wrap(getValueFactory);
        }

        #endregion

        public bool Contains(TKey key)
        {
            return repository.ContainsKey(key);
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
                return ResetAll(key, out value1, out value2);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return ResetAll(key, out value1, out value2);
            }
        }

        public virtual Tuple<TValue1, TValue2> this[TKey key] => Get(key, DefaultFactory);

        public virtual Tuple<TValue1, TValue2> Get(TKey key) => Get(key, DefaultFactory);

        public virtual bool Get(TKey key, out TValue1 value1) => Get(key, out value1, out TValue2 value2);

        public virtual bool Get(TKey key, out TValue2 value2) => Get(key, out TValue1 value1, out value2);

        #endregion

        #region Remove Functions

        public bool Remove(TKey key)
        {
            return repository.Remove(key);
        }

        public int Remove(Predicate<TKey> predicate)
        {
            TKey[] matchedKeys = repository.Keys.Where(k => predicate(k)).ToArray();
            int count = 0;
            foreach (var key in matchedKeys)
            {
                if (Remove(key))
                    count++;
            }
            return count;
        }

        public void Clear()
        {
            repository.Clear();
        }

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3> : Repository,
        IRepository<TKey, TValue1, TValue2, TValue3>
    {
        protected class FactoryWrapper
        {
            private readonly Func<TKey, TValue1> _fun1;
            private readonly Func<TKey, TValue2> _fun2;
            private readonly Func<TKey, TValue3> _fun3;
            private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3> getAllValues;

            internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3)
            {
                _fun1 = f1 ?? throw new ArgumentNullException(nameof(f1));
                _fun2 = f2 ?? throw new ArgumentNullException(nameof(f2));
                _fun3 = f3 ?? throw new ArgumentNullException(nameof(f3));
                if (ExecuteParallelly)
                    getAllValues = runParallel;
                else
                    getAllValues = runSequential;
            }

            internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValues)
            {
                _fun1 = null;
                _fun2 = null;
                _fun3 = null;
                getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
            }

            internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3)
            {
                try
                {
                    v1 = _fun1.Invoke(key);
                    v2 = _fun2.Invoke(key);
                    v3 = _fun3.Invoke(key);
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return ResetAll(key, out v1, out v2, out v3);
                }
            }

            internal bool runParallel(TKey key, out TValue1 value1, out TValue2 value2, out TValue3 value3)
            {
                TValue1 v1 = default(TValue1);
                TValue2 v2 = default(TValue2);
                TValue3 v3 = default(TValue3);

                try
                {
                    Parallel.Invoke(
                        () => v1 = _fun1.Invoke(key),
                        () => v2 = _fun2.Invoke(key),
                        () => v3 = _fun3.Invoke(key)
                    );
                    return true;
                }
                catch (AggregateException ex)
                {
                    ex.Handle(exception =>
                    {
                        Trace.WriteLine(exception);
                        return true; //handled
                    });
                    return false;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    value1 = v1;
                    value2 = v2;
                    value3 = v3;
                }
            }

            internal bool runTogether(TKey key, out TValue1 value1, out TValue2 value2, out TValue3 value3)
            {
                try
                {
                    return getAllValues.Invoke(key, out value1, out value2, out value3);
                }
                catch (Exception)
                {
                    return ResetAll(key, out value1, out value2, out value3);
                }
            }
        }

        #region Static methods

        public static bool ResetAll(TKey key, out TValue1 value1, out TValue2 value2, out TValue3 value3)
        {
            value1 = default(TValue1);
            value2 = default(TValue2);
            value3 = default(TValue3);
            return false;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3)
        {
            FactoryWrapper factory = new FactoryWrapper(f1, f2, f3);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValues)
        {
            FactoryWrapper factory = new FactoryWrapper(getValues);
            return factory.runTogether;
        }

        #endregion

        #region Readonly Properties

        protected readonly IDictionary<TKey, Tuple<TValue1, TValue2, TValue3>> repository;

        public GetValueDelegate<TKey, TValue1, TValue2, TValue3> DefaultFactory { get; }

        #endregion

        #region Constructors
        public Repository(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3>>();
            DefaultFactory = Wrap(f1, f2, f3);
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3>>();
            DefaultFactory = Wrap(getValueFactory);
        }

        #endregion

        public bool Contains(TKey key)
        {
            return repository.ContainsKey(key);
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
                return ResetAll(key, out v1, out v2, out v3);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return ResetAll(key, out v1, out v2, out v3);
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

        #region Remove Functions

        public bool Remove(TKey key)
        {
            return repository.Remove(key);
        }

        public int Remove(Predicate<TKey> predicate)
        {
            TKey[] matchedKeys = repository.Keys.Where(k => predicate(k)).ToArray();
            int count = 0;
            foreach (var key in matchedKeys)
            {
                if (Remove(key))
                    count++;
            }
            return count;
        }

        public void Clear()
        {
            repository.Clear();
        }

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3, TValue4>
        : Repository, IRepository<TKey, TValue1, TValue2, TValue3, TValue4>
    {
        protected class FactoryWrapper
        {
            private readonly Func<TKey, TValue1> _fun1;
            private readonly Func<TKey, TValue2> _fun2;
            private readonly Func<TKey, TValue3> _fun3;
            private readonly Func<TKey, TValue4> _fun4;
            private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getAllValues;

            internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
                Func<TKey, TValue4> f4)
            {
                _fun1 = f1 ?? throw new ArgumentNullException(nameof(f1));
                _fun2 = f2 ?? throw new ArgumentNullException(nameof(f2));
                _fun3 = f3 ?? throw new ArgumentNullException(nameof(f3));
                _fun4 = f4 ?? throw new ArgumentNullException(nameof(f4));
                if (ExecuteParallelly)
                    getAllValues = runParallel;
                else
                    getAllValues = runSequential;
            }

            internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValues)
            {
                _fun1 = null;
                _fun2 = null;
                _fun3 = null;
                _fun4 = null;
                getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
            }

            internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4)
            {
                try
                {
                    v1 = _fun1.Invoke(key);
                    v2 = _fun2.Invoke(key);
                    v3 = _fun3.Invoke(key);
                    v4 = _fun4.Invoke(key);
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return ResetAll(key, out v1, out v2, out v3, out v4);
                }
            }

            internal bool runParallel(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4)
            {
                TValue1 _v1 = default(TValue1);
                TValue2 _v2 = default(TValue2);
                TValue3 _v3 = default(TValue3);
                TValue4 _v4 = default(TValue4);

                try
                {
                    Parallel.Invoke(
                        () => _v1 = _fun1.Invoke(key),
                        () => _v2 = _fun2.Invoke(key),
                        () => _v3 = _fun3.Invoke(key),
                        () => _v4 = _fun4.Invoke(key)
                    );
                    return true;
                }
                catch (AggregateException ex)
                {
                    ex.Handle(exception =>
                    {
                        Trace.WriteLine(exception);
                        return true; //handled
                    });
                    return false;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    v1 = _v1;
                    v2 = _v2;
                    v3 = _v3;
                    v4 = _v4;
                }
            }

            internal bool runTogether(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4)
            {
                try
                {
                    return getAllValues.Invoke(key, out v1, out v2, out v3, out v4);
                }
                catch (Exception)
                {
                    return ResetAll(key, out v1, out v2, out v3, out v4);
                }
            }
        }

        #region Static methods

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4)
        {
            v1 = default(TValue1);
            v2 = default(TValue2);
            v3 = default(TValue3);
            v4 = default(TValue4);
            return false;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4)
        {
            FactoryWrapper factory = new FactoryWrapper(f1, f2, f3, f4);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValues)
        {
            FactoryWrapper factory = new FactoryWrapper(getValues);
            return factory.runTogether;
        }

        #endregion

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
            DefaultFactory = Wrap(f1, f2, f3, f4);
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4>>();
            DefaultFactory = Wrap(getValueFactory);
        }

        #endregion

        public bool Contains(TKey key)
        {
            return repository.ContainsKey(key);
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
                return ResetAll(key, out v1, out v2, out v3, out v4);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return ResetAll(key, out v1, out v2, out v3, out v4);
            }
        }

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4> this[TKey key] => Get(key, DefaultFactory);

        public virtual Tuple<TValue1, TValue2, TValue3, TValue4> Get(TKey key) => Get(key, DefaultFactory);

        public virtual bool Get(TKey key, out TValue1 v1) => Get(key, out v1, out TValue2 v2, out TValue3 v3, out TValue4 v4);

        public virtual bool Get(TKey key, out TValue2 v2) => Get(key, out TValue1 v1, out v2, out TValue3 v3, out TValue4 v4);

        public virtual bool Get(TKey key, out TValue3 v3) => Get(key, out TValue1 v1, out TValue2 v2, out v3, out TValue4 v4);

        public virtual bool Get(TKey key, out TValue4 v4) => Get(key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out v4);

        #endregion

        #region Remove Functions

        public bool Remove(TKey key)
        {
            return repository.Remove(key);
        }

        public int Remove(Predicate<TKey> predicate)
        {
            TKey[] matchedKeys = repository.Keys.Where(k => predicate(k)).ToArray();
            int count = 0;
            foreach (var key in matchedKeys)
            {
                if (Remove(key))
                    count++;
            }
            return count;
        }

        public void Clear()
        {
            repository.Clear();
        }

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
        : Repository, IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
    {
        protected class FactoryWrapper
        {
            private readonly Func<TKey, TValue1> _fun1;
            private readonly Func<TKey, TValue2> _fun2;
            private readonly Func<TKey, TValue3> _fun3;
            private readonly Func<TKey, TValue4> _fun4;
            private readonly Func<TKey, TValue5> _fun5;
            private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getAllValues;

            internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
                Func<TKey, TValue4> f4, Func<TKey, TValue5> f5)
            {
                _fun1 = f1 ?? throw new ArgumentNullException(nameof(f1));
                _fun2 = f2 ?? throw new ArgumentNullException(nameof(f2));
                _fun3 = f3 ?? throw new ArgumentNullException(nameof(f3));
                _fun4 = f4 ?? throw new ArgumentNullException(nameof(f4));
                _fun5 = f5 ?? throw new ArgumentNullException(nameof(f5));
                if (ExecuteParallelly)
                    getAllValues = runParallel;
                else
                    getAllValues = runSequential;
            }

            internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValues)
            {
                _fun1 = null;
                _fun2 = null;
                _fun3 = null;
                _fun4 = null;
                _fun5 = null;
                getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
            }

            internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4, out TValue5 v5)
            {
                try
                {
                    v1 = _fun1.Invoke(key);
                    v2 = _fun2.Invoke(key);
                    v3 = _fun3.Invoke(key);
                    v4 = _fun4.Invoke(key);
                    v5 = _fun5.Invoke(key);
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return ResetAll(key, out v1, out v2, out v3, out v4, out v5);
                }
            }

            internal bool runParallel(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4, out TValue5 v5)
            {
                TValue1 _v1 = default(TValue1);
                TValue2 _v2 = default(TValue2);
                TValue3 _v3 = default(TValue3);
                TValue4 _v4 = default(TValue4);
                TValue5 _v5 = default(TValue5);

                try
                {
                    Parallel.Invoke(
                        () => _v1 = _fun1.Invoke(key),
                        () => _v2 = _fun2.Invoke(key),
                        () => _v3 = _fun3.Invoke(key),
                        () => _v4 = _fun4.Invoke(key),
                        () => _v5 = _fun5.Invoke(key)
                    );
                    return true;
                }
                catch (AggregateException ex)
                {
                    ex.Handle(exception =>
                    {
                        Trace.WriteLine(exception);
                        return true; //handled
                    });
                    return false;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    v1 = _v1;
                    v2 = _v2;
                    v3 = _v3;
                    v4 = _v4;
                    v5 = _v5;
                }
            }

            internal bool runTogether(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5)
            {
                try
                {
                    return getAllValues.Invoke(key, out v1, out v2, out v3, out v4, out v5);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return ResetAll(key, out v1, out v2, out v3, out v4, out v5);
                }
            }
        }

        #region Static methods

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5)
        {
            v1 = default(TValue1);
            v2 = default(TValue2);
            v3 = default(TValue3);
            v4 = default(TValue4);
            v5 = default(TValue5);
            return false;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5)
        {
            FactoryWrapper factory = new FactoryWrapper(f1, f2, f3, f4, f5);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValues)
        {
            FactoryWrapper factory = new FactoryWrapper(getValues);
            return factory.runTogether;
        }

        #endregion

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
            DefaultFactory = Wrap(f1, f2, f3, f4, f5);
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5>>();
            DefaultFactory = Wrap(getValueFactory);
        }

        #endregion

        public bool Contains(TKey key)
        {
            return repository.ContainsKey(key);
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
                return ResetAll(key, out v1, out v2, out v3, out v4, out v5);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return ResetAll(key, out v1, out v2, out v3, out v4, out v5);
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

        #region Remove Functions

        public bool Remove(TKey key)
        {
            return repository.Remove(key);
        }

        public int Remove(Predicate<TKey> predicate)
        {
            TKey[] matchedKeys = repository.Keys.Where(k => predicate(k)).ToArray();
            int count = 0;
            foreach (var key in matchedKeys)
            {
                if (Remove(key))
                    count++;
            }
            return count;
        }

        public void Clear()
        {
            repository.Clear();
        }

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
        : Repository, IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
    {
        protected class FactoryWrapper
        {
            private readonly Func<TKey, TValue1> _fun1;
            private readonly Func<TKey, TValue2> _fun2;
            private readonly Func<TKey, TValue3> _fun3;
            private readonly Func<TKey, TValue4> _fun4;
            private readonly Func<TKey, TValue5> _fun5;
            private readonly Func<TKey, TValue6> _fun6;
            private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getAllValues;

            internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
                Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6)
            {
                _fun1 = f1 ?? throw new ArgumentNullException(nameof(f1));
                _fun2 = f2 ?? throw new ArgumentNullException(nameof(f2));
                _fun3 = f3 ?? throw new ArgumentNullException(nameof(f3));
                _fun4 = f4 ?? throw new ArgumentNullException(nameof(f4));
                _fun5 = f5 ?? throw new ArgumentNullException(nameof(f5));
                _fun6 = f6 ?? throw new ArgumentNullException(nameof(f6));
                if (ExecuteParallelly)
                    getAllValues = runParallel;
                else
                    getAllValues = runSequential;
            }

            internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValues)
            {
                _fun1 = null;
                _fun2 = null;
                _fun3 = null;
                _fun4 = null;
                _fun5 = null;
                _fun6 = null;
                getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
            }

            internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4, out TValue5 v5, out TValue6 v6)
            {
                try
                {
                    v1 = _fun1.Invoke(key);
                    v2 = _fun2.Invoke(key);
                    v3 = _fun3.Invoke(key);
                    v4 = _fun4.Invoke(key);
                    v5 = _fun5.Invoke(key);
                    v6 = _fun6.Invoke(key);
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6);
                }
            }

            internal bool runParallel(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4, out TValue5 v5, out TValue6 v6)
            {
                TValue1 _v1 = default(TValue1);
                TValue2 _v2 = default(TValue2);
                TValue3 _v3 = default(TValue3);
                TValue4 _v4 = default(TValue4);
                TValue5 _v5 = default(TValue5);
                TValue6 _v6 = default(TValue6);

                try
                {
                    Parallel.Invoke(
                        () => _v1 = _fun1.Invoke(key),
                        () => _v2 = _fun2.Invoke(key),
                        () => _v3 = _fun3.Invoke(key),
                        () => _v4 = _fun4.Invoke(key),
                        () => _v5 = _fun5.Invoke(key),
                        () => _v6 = _fun6.Invoke(key)
                    );
                    return true;
                }
                catch (AggregateException ex)
                {
                    ex.Handle(exception =>
                    {
                        Trace.WriteLine(exception);
                        return true; //handled
                    });
                    return false;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    v1 = _v1;
                    v2 = _v2;
                    v3 = _v3;
                    v4 = _v4;
                    v5 = _v5;
                    v6 = _v6;
                }
            }

            internal bool runTogether(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6)
            {
                try
                {
                    return getAllValues.Invoke(key, out v1, out v2, out v3, out v4, out v5, out v6);
                }
                catch (Exception)
                {
                    return ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6);
                }
            }
        }

        #region Static methods

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6)
        {
            v1 = default(TValue1);
            v2 = default(TValue2);
            v3 = default(TValue3);
            v4 = default(TValue4);
            v5 = default(TValue5);
            v6 = default(TValue6);
            return false;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6)
        {
            FactoryWrapper factory = new FactoryWrapper(f1, f2, f3, f4, f5, f6);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValues)
        {
            FactoryWrapper factory = new FactoryWrapper(getValues);
            return factory.runTogether;
        }

        #endregion

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
            DefaultFactory = Wrap(f1, f2, f3, f4, f5, f6);
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>>();
            DefaultFactory = Wrap(getValueFactory);
        }

        #endregion

        public bool Contains(TKey key)
        {
            return repository.ContainsKey(key);
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
                return ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6);
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

        #region Remove Functions

        public bool Remove(TKey key)
        {
            return repository.Remove(key);
        }

        public int Remove(Predicate<TKey> predicate)
        {
            TKey[] matchedKeys = repository.Keys.Where(k => predicate(k)).ToArray();
            int count = 0;
            foreach (var key in matchedKeys)
            {
                if (Remove(key))
                    count++;
            }
            return count;
        }

        public void Clear()
        {
            repository.Clear();
        }

        #endregion
    }

    public class Repository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>
        : Repository, IRepository<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>
    {
        protected class FactoryWrapper
        {
            private readonly Func<TKey, TValue1> _fun1;
            private readonly Func<TKey, TValue2> _fun2;
            private readonly Func<TKey, TValue3> _fun3;
            private readonly Func<TKey, TValue4> _fun4;
            private readonly Func<TKey, TValue5> _fun5;
            private readonly Func<TKey, TValue6> _fun6;
            private readonly Func<TKey, TValue7> _fun7;
            private readonly GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> getAllValues;

            internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
                Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6, Func<TKey, TValue7> f7)
            {
                _fun1 = f1 ?? throw new ArgumentNullException(nameof(f1));
                _fun2 = f2 ?? throw new ArgumentNullException(nameof(f2));
                _fun3 = f3 ?? throw new ArgumentNullException(nameof(f3));
                _fun4 = f4 ?? throw new ArgumentNullException(nameof(f4));
                _fun5 = f5 ?? throw new ArgumentNullException(nameof(f5));
                _fun6 = f6 ?? throw new ArgumentNullException(nameof(f6));
                _fun7 = f7 ?? throw new ArgumentNullException(nameof(f7));
                if (ExecuteParallelly)
                    getAllValues = runParallel;
                else
                    getAllValues = runSequential;
            }

            internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> getValues)
            {
                _fun1 = null;
                _fun2 = null;
                _fun3 = null;
                _fun4 = null;
                _fun5 = null;
                _fun6 = null;
                _fun7 = null;
                getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
            }

            internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7)
            {
                try
                {
                    v1 = _fun1.Invoke(key);
                    v2 = _fun2.Invoke(key);
                    v3 = _fun3.Invoke(key);
                    v4 = _fun4.Invoke(key);
                    v5 = _fun5.Invoke(key);
                    v6 = _fun6.Invoke(key);
                    v7 = _fun7.Invoke(key);
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
                }
            }

            internal bool runParallel(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
                out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7)
            {
                TValue1 _v1 = default(TValue1);
                TValue2 _v2 = default(TValue2);
                TValue3 _v3 = default(TValue3);
                TValue4 _v4 = default(TValue4);
                TValue5 _v5 = default(TValue5);
                TValue6 _v6 = default(TValue6);
                TValue7 _v7 = default(TValue7);

                try
                {
                    Parallel.Invoke(
                        () => _v1 = _fun1.Invoke(key),
                        () => _v2 = _fun2.Invoke(key),
                        () => _v3 = _fun3.Invoke(key),
                        () => _v4 = _fun4.Invoke(key),
                        () => _v5 = _fun5.Invoke(key),
                        () => _v6 = _fun6.Invoke(key),
                        () => _v7 = _fun7.Invoke(key)
                    );
                    return true;
                }
                catch (AggregateException ex)
                {
                    ex.Handle(exception =>
                    {
                        Trace.WriteLine(exception);
                        return true; //handled
                    });
                    return false;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    v1 = _v1;
                    v2 = _v2;
                    v3 = _v3;
                    v4 = _v4;
                    v5 = _v5;
                    v6 = _v6;
                    v7 = _v7;
                }
            }

            internal bool runTogether(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7)
            {
                try
                {
                    return getAllValues.Invoke(key, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
                }
                catch (Exception)
                {
                    return ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
                }
            }
        }

        #region Static methods

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7)
        {
            v1 = default(TValue1);
            v2 = default(TValue2);
            v3 = default(TValue3);
            v4 = default(TValue4);
            v5 = default(TValue5);
            v6 = default(TValue6);
            v7 = default(TValue7);
            return false;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6, Func<TKey, TValue7> f7)
        {
            FactoryWrapper factory = new FactoryWrapper(f1, f2, f3, f4, f5, f6, f7);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> getValues)
        {
            FactoryWrapper factory = new FactoryWrapper(getValues);
            return factory.runTogether;
        }

        #endregion

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
            DefaultFactory = Wrap(f1, f2, f3, f4, f5, f6, f7);
        }

        public Repository(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> getValueFactory,
            IDictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>> store = null)
        {
            repository = store ?? new Dictionary<TKey, Tuple<TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>>();
            DefaultFactory = Wrap(getValueFactory);
        }

        #endregion

        public bool Contains(TKey key)
        {
            return repository.ContainsKey(key);
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
                return ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
            }
            catch (Exception e)
            {
                logMessage?.Invoke($"{key}: {e.Message}");
                return ResetAll(key, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
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

        #region Remove Functions

        public bool Remove(TKey key)
        {
            return repository.Remove(key);
        }

        public int Remove(Predicate<TKey> predicate)
        {
            TKey[] matchedKeys = repository.Keys.Where(k => predicate(k)).ToArray();
            int count = 0;
            foreach (var key in matchedKeys)
            {
                if (Remove(key))
                    count++;
            }
            return count;
        }

        public void Clear()
        {
            repository.Clear();
        }

        #endregion
    }
}
