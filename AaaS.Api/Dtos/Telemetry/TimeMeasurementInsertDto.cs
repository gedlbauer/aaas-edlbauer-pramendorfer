using AaaS.Api.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Telemetry
{
    public class TimeMeasurementInsertDto : IValidatableObject
    {
        [NotInFuture]
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public Guid CreatorId { get; set; }
        [NotInFuture]
        public DateTime StartTime { get; set; }
        [NotInFuture]
        public DateTime EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(EndTime < StartTime)
            {
                yield return new ValidationResult(
                    $"{nameof(EndTime)} must be after {nameof(StartTime)}.",
                    new[] { nameof(EndTime) });
            }
        }
    }
}
