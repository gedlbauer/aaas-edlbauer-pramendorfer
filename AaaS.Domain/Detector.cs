﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Domain
{
    public abstract class Detector
    {
        public int Id { get; set; }
        public Client Client { get; set; }
        public IAction Action { get; set; }
        public string TelemetryName { get; set; }
        public TimeSpan CheckInterval { get; set; }
    }
}