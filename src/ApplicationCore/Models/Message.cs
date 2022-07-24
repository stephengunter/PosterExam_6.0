using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models
{
    public class Message : BaseRecord
    {
        public string? Subject { get; set; } = String.Empty;

        public string? Content { get; set; } = String.Empty;

        public string? Email { get; set; } = String.Empty;

        public string? ReturnContent { get; set; } = String.Empty; //json string  => BaseMessageViewModel

        public bool Returned { get; set; }
    }
}
