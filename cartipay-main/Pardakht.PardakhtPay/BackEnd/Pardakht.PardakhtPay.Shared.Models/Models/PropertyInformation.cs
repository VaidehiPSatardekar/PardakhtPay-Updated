using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    public class PropertyInformation
    {
        public PropertyInfo Info { get; set; }

        public List<Attribute> Attributes { get; set; }
    }
}
