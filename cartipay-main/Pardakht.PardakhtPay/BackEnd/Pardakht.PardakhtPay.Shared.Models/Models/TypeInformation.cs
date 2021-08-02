using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    /// <summary>
    /// Represents a class which is used for reflection operations
    /// </summary>
    public class TypeInformation
    {
        public List<PropertyInformation> Properties { get; set; }

        public List<MethodInfo> Methods { get; set; }

        public List<Attribute> Attributes { get; set; }

        public TypeInformation()
        {
            Properties = new List<PropertyInformation>();
            Methods = new List<MethodInfo>();
            Attributes = new List<Attribute>();
        }

        public List<T> GetCustomAttributes<T>()
        {
            return Attributes.OfType<T>().ToList();
        }
    }
}
