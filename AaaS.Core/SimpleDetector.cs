using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core
{
    public class SimpleDetector : Detector
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return $"Hello I am a SimpleDetector! (${base.ToString()})";
        }

        protected override void Detect()
        {
            throw new NotImplementedException();
        }
    }
}
