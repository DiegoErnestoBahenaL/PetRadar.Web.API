using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core
{
    public class PetRadarException : Exception
    {
        public PetRadarException() { }
        public PetRadarException(string message) : base(message) { }
        public PetRadarException(string message, Exception innerException) : base(message, innerException) { }
    }
}
