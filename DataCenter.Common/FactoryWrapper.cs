using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DataCenter.Common
{
    public class FactoryWrapper<TKey, TValue1, TValue2>
    {
        public static GetValueDelegate<TKey, TValue1, TValue2> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, bool ExecuteParallelly = false)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2>(f1, f2);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2> getValues)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2>(getValues);
            return factory.runTogether;
        }

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2)
        {
            v1 = default(TValue1);
            v2 = default(TValue2);
            return false;
        }

        protected Func<TKey, TValue1> _fun1;
        protected Func<TKey, TValue2> _fun2;
        private GetValueDelegate<TKey, TValue1, TValue2> getAllValues;

        internal FactoryWrapper()
        {
            _fun1 = null;
            _fun2 = null;
            getAllValues = null;
        }
        internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2)
        {
            _fun1 = f1 ?? throw new ArgumentNullException(nameof(f1));
            _fun2 = f2 ?? throw new ArgumentNullException(nameof(f2));
            if (Repository.ExecuteParallelly)
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

        internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2)
        {
            try
            {
                v1 = _fun1.Invoke(key);
                v2 = _fun2.Invoke(key);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return ResetAll(key, out v1, out v2);
            }
        }

        internal bool runParallel(TKey key, out TValue1 v1, out TValue2 v2)
        {
            TValue1 _v1 = default(TValue1);
            TValue2 _v2 = default(TValue2);

            try
            {
                Parallel.Invoke(
                    () => _v1 = _fun1.Invoke(key),
                    () => _v2 = _fun2.Invoke(key)
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
            }
        }

        internal bool runTogether(TKey key, out TValue1 v1, out TValue2 v2)
        {
            try
            {
                return getAllValues.Invoke(key, out v1, out v2);
            }
            catch (Exception)
            {
                return ResetAll(key, out v1, out v2);
            }
        }
    }
    public class FactoryWrapper<TKey, TValue1, TValue2, TValue3> : FactoryWrapper<TKey, TValue1, TValue2>
    {
        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3, bool ExecuteParallelly = false)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3>(f1, f2, f3);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValues)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3>(getValues);
            return factory.runTogether;
        }

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3)
        {
            v1 = default(TValue1);
            v2 = default(TValue2);
            v3 = default(TValue3);
            return false;
        }

        protected Func<TKey, TValue3> _fun3;
        private GetValueDelegate<TKey, TValue1, TValue2, TValue3> getAllValues;

        internal FactoryWrapper() : base()
        {
            _fun3 = null;
            getAllValues = null;
        }

        internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3)
            : base(f1, f2)
        {
            _fun3 = f3 ?? throw new ArgumentNullException(nameof(f3));
            if (Repository.ExecuteParallelly)
                getAllValues = runParallel;
            else
                getAllValues = runSequential;
        }

        internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3> getValues)
        {
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3)
        {
            try
            {
                runSequential(key, out v1, out v2);
                v3 = _fun3.Invoke(key);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return ResetAll(key, out v1, out v2, out v3);
            }
        }

        internal bool runParallel(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3)
        {
            TValue1 _v1 = default(TValue1);
            TValue2 _v2 = default(TValue2);
            TValue3 _v3 = default(TValue3);

            try
            {
                Parallel.Invoke(
                    () => _v1 = _fun1.Invoke(key),
                    () => _v2 = _fun2.Invoke(key),
                    () => _v3 = _fun3.Invoke(key)
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
            }
        }

        internal bool runTogether(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3)
        {
            try
            {
                return getAllValues.Invoke(key, out v1, out v2, out v3);
            }
            catch (Exception)
            {
                return ResetAll(key, out v1, out v2, out v3);
            }
        }
    }
    public class FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4> : FactoryWrapper<TKey, TValue1, TValue2, TValue3>
    {
        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3, Func<TKey, TValue4> f4, bool ExecuteParallelly = false)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>(f1, f2, f3, f4);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValues)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>(getValues);
            return factory.runTogether;
        }

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4)
        {
            v1 = default(TValue1);
            v2 = default(TValue2);
            v3 = default(TValue3);
            v4 = default(TValue4);
            return false;
        }

        protected Func<TKey, TValue4> _fun4;
        private GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getAllValues;

        internal FactoryWrapper() : base()
        {
            _fun4 = null;
            getAllValues = null;
        }

        internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3, Func<TKey, TValue4> f4)
            : base(f1, f2, f3)
        {
            _fun4 = f4 ?? throw new ArgumentNullException(nameof(f4));
            if (Repository.ExecuteParallelly)
                getAllValues = runParallel;
            else
                getAllValues = runSequential;
        }

        internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4> getValues)
        {
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4)
        {
            try
            {
                runSequential(key, out v1, out v2, out v3);
                v4 = _fun4.Invoke(key);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return ResetAll(key, out v1, out v2, out v3, out v4);
            }
        }

        internal bool runParallel(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4)
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
    public class FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
        : FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4>
    {
        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3, Func<TKey, TValue4> f4,
            Func<TKey, TValue5> f5, bool ExecuteParallelly = false)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>(f1, f2, f3, f4, f5);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValues)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>(getValues);
            return factory.runTogether;
        }

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3, out TValue4 v4, out TValue5 v5)
        {
            v1 = default(TValue1);
            v2 = default(TValue2);
            v3 = default(TValue3);
            v4 = default(TValue4);
            v5 = default(TValue5);
            return false;
        }

        protected Func<TKey, TValue5> _fun5;
        private GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getAllValues;

        internal FactoryWrapper() : base()
        {
            _fun5 = null;
            getAllValues = null;
        }

        internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5)
            : base(f1, f2, f3, f4)
        {
            _fun5 = f5 ?? throw new ArgumentNullException(nameof(f5));
            if (Repository.ExecuteParallelly)
                getAllValues = runParallel;
            else
                getAllValues = runSequential;
        }

        internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5> getValues)
        {
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
            out TValue4 v4, out TValue5 v5)
        {
            try
            {
                runSequential(key, out v1, out v2, out v3, out v4);
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
            catch (Exception)
            {
                return ResetAll(key, out v1, out v2, out v3, out v4, out v5);
            }
        }
    }

    public class FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
        : FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5>
    {
        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3, Func<TKey, TValue4> f4,
            Func<TKey, TValue5> f5, Func<TKey, TValue6> f6, bool ExecuteParallelly = false)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>(f1, f2, f3, f4, f5, f6);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValues)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>(getValues);
            return factory.runTogether;
        }

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

        protected Func<TKey, TValue6> _fun6;
        private GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getAllValues;

        internal FactoryWrapper() : base()
        {
            _fun5 = null;
            getAllValues = null;
        }

        internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6)
            : base(f1, f2, f3, f4, f5)
        {
            _fun6 = f6 ?? throw new ArgumentNullException(nameof(f6));
            if (Repository.ExecuteParallelly)
                getAllValues = runParallel;
            else
                getAllValues = runSequential;
        }

        internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6> getValues)
        {
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
            out TValue4 v4, out TValue5 v5, out TValue6 v6)
        {
            try
            {
                runSequential(key, out v1, out v2, out v3, out v4, out v5);
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

        internal bool runTogether(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
            out TValue4 v4, out TValue5 v5, out TValue6 v6)
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

    public class FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>
        : FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>
    {
        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> Wrap(
            Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3, Func<TKey, TValue4> f4,
            Func<TKey, TValue5> f5, Func<TKey, TValue6> f6, Func<TKey, TValue7> f7, bool ExecuteParallelly = false)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>(f1, f2, f3, f4, f5, f6, f7);
            if (ExecuteParallelly)
                return factory.runParallel;
            else
                return factory.runSequential;
        }

        public static GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> Wrap(
            GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> getValues)
        {
            var factory = new FactoryWrapper<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>(getValues);
            return factory.runTogether;
        }

        public static bool ResetAll(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
            out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7)
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

        protected Func<TKey, TValue7> _fun7;
        private GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> getAllValues;

        internal FactoryWrapper() : base()
        {
            _fun7 = null;
            getAllValues = null;
        }

        internal FactoryWrapper(Func<TKey, TValue1> f1, Func<TKey, TValue2> f2, Func<TKey, TValue3> f3,
            Func<TKey, TValue4> f4, Func<TKey, TValue5> f5, Func<TKey, TValue6> f6, Func<TKey, TValue7> f7)
            : base(f1, f2, f3, f4, f5, f6)
        {
            _fun7 = f7 ?? throw new ArgumentNullException(nameof(f7));
            if (Repository.ExecuteParallelly)
                getAllValues = runParallel;
            else
                getAllValues = runSequential;
        }

        internal FactoryWrapper(GetValueDelegate<TKey, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7> getValues)
        {
            getAllValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
        }

        internal bool runSequential(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
            out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7)
        {
            try
            {
                runSequential(key, out v1, out v2, out v3, out v4, out v5, out v6);
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

        internal bool runTogether(TKey key, out TValue1 v1, out TValue2 v2, out TValue3 v3,
            out TValue4 v4, out TValue5 v5, out TValue6 v6, out TValue7 v7)
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
}