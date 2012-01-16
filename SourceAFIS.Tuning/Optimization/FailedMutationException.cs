using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class FailedMutationException : Exception
    {
        public FailedMutationException(string message) : base(message) { }
        public FailedMutationException(string message, params object[] parameters) : base(String.Format(message, parameters)) { }
    }
}
