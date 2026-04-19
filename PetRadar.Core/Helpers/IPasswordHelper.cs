using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers
{
    public interface IPasswordHelper
    {
        byte[] GenerateHash(string password, byte[] salt);
        byte[] GenerateSalt();
    }
}
