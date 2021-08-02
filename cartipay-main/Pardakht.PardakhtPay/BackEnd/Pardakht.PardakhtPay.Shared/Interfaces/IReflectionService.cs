using System;
using System.Collections.Generic;
using System.Reflection;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    /// <summary>
    /// Represents and interface to manage reflection objects
    /// </summary>
    public interface IReflectionService
    {
        List<PropertyInformation> GetProperties<T>();

        List<MethodInfo> GetMethods<T>();

        List<Attribute> GetAttributes<T>();

        List<TAttribute> GetCustomerAttributes<TType, TAttribute>() where TAttribute : Attribute;
    }
}
