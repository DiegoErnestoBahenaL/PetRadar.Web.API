using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Entities
{
    [Index(nameof(Key), IsUnique = true)]
    public class SystemConfigEntity : Entity, IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required, StringLength(maximumLength: 255)]
        public string Key { get; set; } = string.Empty;

        [StringLength(maximumLength: 4096)]
        public string? Value { get; set; } = null;

        public SystemConfigEntity() { }

        public SystemConfigEntity(string key, string? value)
        {
            Key = key;
            Value = value;
        }
    }
}
