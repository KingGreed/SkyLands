using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;

namespace LibNoise
{
    public class NoModuleException : NoiseException {
        public NoModuleException() : base("No module") {}
    }
}
