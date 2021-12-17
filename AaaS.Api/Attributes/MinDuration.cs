using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Attributes
{
    public class MinDuration : ValidationAttribute
    {
        private TimeSpan _minSpan;

        public MinDuration(int minMilliseconds)
        {
            _minSpan = TimeSpan.FromMilliseconds(minMilliseconds);
        }

        public string GetErrorMessage() => $"Minimum duration is {_minSpan.TotalMilliseconds} ms.";

        public override bool IsValid(object value)
        {
            var timeSpan = (TimeSpan)value;

            return timeSpan >= _minSpan;
        }
    }
}
