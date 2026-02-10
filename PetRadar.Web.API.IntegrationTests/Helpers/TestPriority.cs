using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Web.API.IntegrationTests.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriority : Attribute
    {
       public int Priority { get; private set; }
        
       public TestPriority(int priority)
       {
            Priority = priority;
       }
    }
}
