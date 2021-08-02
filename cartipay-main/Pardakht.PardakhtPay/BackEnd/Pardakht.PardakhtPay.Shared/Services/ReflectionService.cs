using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Services
{
    /// <summary>
    /// Represents caching reflection objects to improve performance.
    /// This class should be used as a singleton
    /// </summary>
    public class ReflectionService : IReflectionService
    {
        private static object _LockObject = new object();

        private Dictionary<Type, TypeInformation> Types { get; set; } = new Dictionary<Type, TypeInformation>();

        /// <summary>
        /// Gets attributes of given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<Attribute> GetAttributes<T>()
        {
            var type = typeof(T);

            if (Types.ContainsKey(type))
            {
                return Types[type].Attributes;
            }

            lock (_LockObject)
            {
                if (!Types.ContainsKey(type))
                {
                    ResolveTypeInformation(type);
                }

                return Types[type].Attributes;
            }
        }

        /// <summary>
        /// Gets custom attributes of given type
        /// </summary>
        /// <typeparam name="TType">Type will be resolved</typeparam>
        /// <typeparam name="TAttribute">Attribute will be searched</typeparam>
        /// <returns></returns>
        public List<TAttribute> GetCustomerAttributes<TType, TAttribute>() where TAttribute : Attribute
        {
            var type = typeof(TType);

            if (Types.ContainsKey(type))
            {
                return Types[type].Attributes.OfType<TAttribute>().ToList();
            }

            lock (_LockObject)
            {
                if (!Types.ContainsKey(type))
                {
                    ResolveTypeInformation(type);
                }

                return Types[type].Attributes.OfType<TAttribute>().ToList();
            }
        }

        /// <summary>
        /// Gets methods of given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<MethodInfo> GetMethods<T>()
        {
            var type = typeof(T);

            if (Types.ContainsKey(type))
            {
                return Types[type].Methods;
            }

            lock (_LockObject)
            {
                if (!Types.ContainsKey(type))
                {
                    ResolveTypeInformation(type);
                }

                return Types[type].Methods;
            }
        }

        /// <summary>
        /// Gets properties of given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<PropertyInformation> GetProperties<T>()
        {
            var type = typeof(T);

            if (Types.ContainsKey(type))
            {
                return Types[type].Properties;
            }

            lock (_LockObject)
            {
                if (!Types.ContainsKey(type))
                {
                    ResolveTypeInformation(type);
                }

                return Types[type].Properties;
            }
        }

        /// <summary>
        /// Resolves all type information which we need
        /// </summary>
        /// <param name="type"></param>
        private void ResolveTypeInformation(Type type)
        {
            if(type == null)
            {
                throw new ArgumentNullException("type");
            }
            TypeInformation t = new TypeInformation();

            foreach (var attribute in type.GetCustomAttributes())
            {
                t.Attributes.Add(attribute);
            }

            t.Methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).ToList();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).ToList();

            properties.ForEach(property =>
            {
                t.Properties.Add(new PropertyInformation()
                {
                    Info = property,
                    Attributes = property.GetCustomAttributes().ToList()
                });
            });

            Types.Add(type, t);
        }
    }
}
