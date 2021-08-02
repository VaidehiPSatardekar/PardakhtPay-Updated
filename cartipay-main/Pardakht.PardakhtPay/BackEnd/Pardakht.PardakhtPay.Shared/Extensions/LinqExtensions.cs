using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Reflection;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models;

namespace Pardakht.PardakhtPay.Shared.Extensions
{
    public static class LinqExtensions
    {
        static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });


        public static IQueryable<T> ApplyParameters<T>(this IQueryable<T> query, Dictionary<string, dynamic> filterModel, IAesEncryptionService encryptionService)
        {
            if (filterModel == null || filterModel.Keys.Count == 0)
            {
                return query;
            }

            foreach (var item in filterModel)
            {
                if (string.IsNullOrEmpty(item.Key))
                {
                    continue;
                }

                var type = item.Value.filterType.ToString();

                AgGridFilterBase filter = null;

                switch (type)
                {
                    case "number":
                        filter = JsonConvert.DeserializeObject<AgGridNumberFilter>(item.Value.ToString());
                        break;
                    case "text":
                        filter = JsonConvert.DeserializeObject<AgGridTextFilter>(item.Value.ToString());
                        break;
                    case "date":
                        filter = JsonConvert.DeserializeObject<AgGridDateFilter>(item.Value.ToString());
                        break;
                    default:
                        break;
                }

                if (filter != null)
                {
                    query = query.ApplyParameters(item.Key, filter, encryptionService);
                }
            }

            return query;
        }

        public static IQueryable<T> ApplyParameters<T, TType>(this IQueryable<T> query, Expression<Func<T, TType>> expression, AgGridFilterBase filterBase)
        {
            var filters = filterBase.GetFilters();

            if (filters == null)
            {
                return query;
            }

            filters.ForEach(filter =>
            {
                query = ApplyParameter(query, expression, filter);
            });

            return query;
        }

        public static IQueryable<T> ApplyParameter<T, TType>(this IQueryable<T> query, Expression<Func<T, TType>> expression, AgGridFilter filter)
        {
            var parameterExpression = Expression.Parameter(typeof(T));

            var propertyInfo = ((MemberExpression)expression.Body).Member as PropertyInfo;

            var left = Expression.Property(parameterExpression, propertyInfo);

            var right = Expression.Convert(Expression.Constant(filter.Value), propertyInfo.PropertyType);

            query = ApplyFilter(query, filter, parameterExpression, left, right);

            return query;
        }

        public static IQueryable<T> ApplyParameters<T>(this IQueryable<T> query, string propertyName, AgGridFilterBase filterBase, IAesEncryptionService encryptionService)
        {
            var filters = filterBase.GetFilters();

            if (filters == null)
            {
                return query;
            }

            filters.ForEach(filter =>
            {
                query = ApplyParameter(query, propertyName, filter, encryptionService);
            });

            return query;
        }

        public static IQueryable<T> ApplyParameter<T>(this IQueryable<T> query, string propertyName, AgGridFilter filter, IAesEncryptionService encryptionService)
        {
            propertyName = propertyName.ToCamelCase();

            var parameterExpression = Expression.Parameter(typeof(T), propertyName);

            var left = Expression.Property(parameterExpression, propertyName);

            var propertyInfo = (PropertyInfo)left.Member;

            if (propertyInfo.PropertyType == typeof(string))
            {
                if (propertyInfo.GetCustomAttribute<EncryptAttribute>() != null)
                {
                    filter.Value = encryptionService.EncryptToBase64(filter.Value.ToString());
                }
            }

            var right = Expression.Convert(Expression.Constant(filter.Value), propertyInfo.PropertyType);

            query = ApplyFilter(query, filter, parameterExpression, left, right);

            return query;
        }

        private static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, AgGridFilter filter, ParameterExpression parameterExpression, MemberExpression left, UnaryExpression right)
        {
            if (filter.Type == Helper.AgGridTypes.Equal)
            {
                query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), new[] { parameterExpression }));
            }
            else if (filter.Type == Helper.AgGridTypes.NotEquals)
            {
                query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.NotEqual(left, right), new[] { parameterExpression }));
            }
            else if (filter.Type == Helper.AgGridTypes.LessThan)
            {
                query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.LessThan(left, right), new[] { parameterExpression }));
            }
            else if (filter.Type == Helper.AgGridTypes.LessThanOrEqual)
            {
                query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(left, right), new[] { parameterExpression }));
            }
            else if (filter.Type == Helper.AgGridTypes.GreaterThan)
            {
                query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(left, right), new[] { parameterExpression }));
            }
            else if (filter.Type == Helper.AgGridTypes.GreaterThanOrEqual)
            {
                query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(left, right), new[] { parameterExpression }));
            }
            else if (filter.Type == Helper.AgGridTypes.Contains)
            {
                var expression = Expression.Call(left, ContainsMethod, right);

                query = query.Where(Expression.Lambda<Func<T, bool>>(expression, parameterExpression));
            }
            else if (filter.Type == Helper.AgGridTypes.StartsWith)
            {
                var expression = Expression.Call(left, StartsWithMethod, right);

                query = query.Where(Expression.Lambda<Func<T, bool>>(expression, parameterExpression));
            }
            else if (filter.Type == Helper.AgGridTypes.EndsWith)
            {
                var expression = Expression.Call(left, EndsWithMethod, right);

                query = query.Where(Expression.Lambda<Func<T, bool>>(expression, parameterExpression));
            }
            else if (filter.Type == Helper.AgGridTypes.NotContains)
            {
                var expression = Expression.Not(Expression.Call(left, ContainsMethod, right));

                query = query.Where(Expression.Lambda<Func<T, bool>>(expression, parameterExpression));
            }

            return query;
        }
    }
}
