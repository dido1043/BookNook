using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BookNook.Data.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(14)]
        public string RecordChannel { get; set; } = string.Empty;
    }
}
