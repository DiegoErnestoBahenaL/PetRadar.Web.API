using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core
{
    public class PetRadarCoreOptions
    {
        [Required, MinLength(1)]
        public string MailGunAPIKey { get; set; }
        [Required, MinLength(1)]
        public string BaseURL { get; set; }
        [Required, MinLength(1)]
        public string PetRadarProcessingBaseURL { get; set; }
    }
}
