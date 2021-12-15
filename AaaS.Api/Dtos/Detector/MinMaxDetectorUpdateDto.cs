using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Detector
{
    public class MinMaxDetectorUpdateDto : MinMaxDetectorInsertDto
    {
        public int Id { get; set; }
    }
}
