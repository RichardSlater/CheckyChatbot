﻿using System;
using System.Collections.Generic;

namespace Checky.Common.Healthbot {
    public class Healthcheck {
        public Version Version { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public IEnumerable<Check> Checks { get; set; }
    }
}