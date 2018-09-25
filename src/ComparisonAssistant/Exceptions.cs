using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    public class CheckConnectionException : Exception
    {
        public CheckConnectionException(string message) : base(message)
        {
        }
    }
}
