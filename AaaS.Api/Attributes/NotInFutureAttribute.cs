using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Attributes
{
    public class NotInFutureAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => $"DateTime must not be in future.";
        public override bool IsValid(object value)
        {
            var date = (DateTime)value;

            return date <= DateTime.Now;
        }

    }
}
