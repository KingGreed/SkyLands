using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;

namespace LibNoise
{
    public class NoiseException : Exception {
        public NoiseException(string message) : base(message) {}
    }
}