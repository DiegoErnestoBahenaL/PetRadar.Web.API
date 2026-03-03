using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Common
{
    public class PetRadarException : Exception
    {
        public PetRadarException() { }
        public PetRadarException(string message) : base(message) { }
        public PetRadarException(string message, Exception innerException) : base(message, innerException) { }


        public static void ThrowImageSizeTooLargeException()
        {
            throw new PetRadarException(Constants.ImageSizeTooLarge);
        }

        public static void ThrowNoImageProvidedException()
        {
            throw new PetRadarException(Constants.NoImageProvided);
        }

        public static void ThrowInvalidExtensionProvidedException() 
        {
            throw new PetRadarException(Constants.InvalidExtensionProvided);
        }

        public static void ThrowImageNotFoundException()
        {
            throw new PetRadarException(Constants.ImageNotFound);
        }
    }
}
