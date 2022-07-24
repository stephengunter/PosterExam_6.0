﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ApplicationCore.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views
{
    public class MessageViewModel : BaseRecordView
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "必須填寫您的Email")]
        [EmailAddress(ErrorMessage = "錯誤的Email格式")]
        public string Email { get; set; }


        [Required(ErrorMessage = "必須填寫主旨")]
        [StringLength(200, ErrorMessage = "主旨長度超出限制")]
        public string Subject { get; set; }


        [Required(ErrorMessage = "必須填寫內容")]
        [StringLength(3600, ErrorMessage = "內容長度超出限制")]
        public string Content { get; set; }


        public string ReturnContent { get; set; } //json string  => BaseMessageViewModel
        
        public bool Returned { get; set; }

        public BaseMessageViewModel ReturnContentView { get; set; }
    }

    public class BaseMessageViewModel : BaseRecordView
    {
        public string Subject { get; set; }

        public string Content { get; set; }

        public string Text { get; set; }

        public string Template { get; set; }

        public bool Draft { get; set; } = true; //草稿
    }
}
