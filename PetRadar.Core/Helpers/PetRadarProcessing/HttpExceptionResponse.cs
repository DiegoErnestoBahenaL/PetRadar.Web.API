using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    public class HttpExceptionResponse
    {
        [JsonProperty("detail")]
        public string Detail { get; set; }
    }
}
