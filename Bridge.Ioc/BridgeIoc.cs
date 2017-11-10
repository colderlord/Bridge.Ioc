using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.Ioc
{
    /// <summary>
    /// Implementation of IIoc
    /// </summary>
    public class BridgeIoc : IIoc
    {
        private readonly Dictionary<Type, IResolver> _resolvers = new Dictionary<Type, IResolver>();

        #region REGISTRATION

        public void Register(Type type, IResolver resolver)
        {
            CheckAlreadyAdded(type);
            _resolvers.Add(type, resolver);
        }

        public void Register(Type type, Type impl)
        {
            CheckAlreadyAdded(type);

            var resolver = new TransientResolver(this, impl);
            _resolvers.Add(type, resolver);
        }

        public void Register<TType, TImplementation>() where TImplementation : class, TType
        {
            Register(typeof(TType), typeof(TImplementation));
        }

        public void Register(Type type)
        {
            Register(type, type);
        }

        public void Register<TType>() where TType : class
        {
            Register(typeof(TType));
        }

        public void RegisterSingleInstance(Type type, Type impl)
        {
            CheckAlreadyAdded(type);

            var resolver = new SingleInstanceResolver(this, impl);
            _resolvers.Add(type, resolver);
        }

        public void RegisterSingleInstance<TType, TImplementation>() where TImplementation : class, TType
        {
            RegisterSingleInstance(typeof(TType), typeof(TImplementation));
        }

        public void RegisterSingleInstance(Type type)
        {
            RegisterSingleInstance(type, type);
        }

        public void RegisterSingleInstance<TType>() where TType : class
        {
            RegisterSingleInstance(typeof(TType));
        }

        public void RegisterFunc<TType>(Func<TType> func)
        {
            CheckAlreadyAdded<TType>();

            var resolver = new FuncResolver<TType>(func);
            _resolvers.Add(typeof(TType), resolver);
        }

        public void RegisterInstance(Type type, object instance)
        {
            CheckAlreadyAdded(type);

            var resolver = new InstanceResolver(instance);
            _resolvers.Add(type, resolver);
        }

        public void RegisterInstance(object instance)
        {
            RegisterInstance(instance.GetType(), instance);
        }

        public void RegisterInstance<TType>(TType instance)
        {
            RegisterInstance(typeof(TType), instance);
        }
        #endregion


        #region RESOLVE
        public TType Resolve<TType>() where TType : class
        {
            return (TType)Resolve(typeof(TType));
        }

        public object Resolve(Type type)
        {
            var isLazyType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ILazy<>);
            CheckNotRegistered(!isLazyType ? type : type.GetGenericArguments()[0]);
            if (isLazyType)
            {
                var lazyType = typeof(Lazy<>).MakeGenericType(new[] { type.GetGenericArguments()[0] });
                return Activator.CreateInstance(lazyType, new[] { this });
            }
            else
            {
                var resolver = _resolvers[type];
                return resolver.Resolve();
            }
        }
        #endregion


        #region PRIVATE

        private void CheckAlreadyAdded(Type type)
        {
            if (_resolvers.ContainsKey(type))
                throw new Exception($"{type.FullName} is already registered!");
        }

        private void CheckAlreadyAdded<TType>()
        {
            CheckAlreadyAdded(typeof(TType));
        }

        private void CheckNotRegistered(Type type)
        {
            if (!_resolvers.ContainsKey(type))
                throw new Exception($"Cannot resolve {type.FullName}, it's not registered!");
        }

        private void CheckNotRegistered<TType>()
        {
            CheckNotRegistered(typeof(TType));
        }

        private class Lazy<T> : ILazy<T> where T : class
        {
            private IIoc _ioc;
            private T _resolved;

            public Lazy(IIoc ioc)
            {
                _ioc = ioc;
            }

            public T Value()
            {
                if (_resolved == null)
                {
                    _resolved = _ioc.Resolve<T>();
                }
                return _resolved;
            }
        }

        #endregion
    }

}