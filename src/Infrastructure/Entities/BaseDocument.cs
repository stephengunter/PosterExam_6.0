﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public abstract class BaseDocument : BaseEntity
    {
        public string? Type { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;//json string
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
