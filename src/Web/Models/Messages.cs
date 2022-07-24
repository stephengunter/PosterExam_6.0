using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;


namespace Web.Models
{
    public class MessageEditForm : AanonymousRequest
    {
        public MessageViewModel Message  { get; set; }
    }
}
