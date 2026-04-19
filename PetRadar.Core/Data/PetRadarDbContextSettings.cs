using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data
{
    public class PetRadarDbContextSettings
    {
        [Required, MinLength(2), EmailAddress]
        public string SuperAdminEmail { get; set; }
        [Required, MinLength(2)]
        public string SuperAdminPassword { get; set; }
    }
}
