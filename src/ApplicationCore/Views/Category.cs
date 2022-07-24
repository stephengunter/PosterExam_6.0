using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ApplicationCore.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views
{
    public class CategoryViewModel : BaseRecordView
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }


        public int ParentId { get; set; }
        public bool IsRootItem { get; set; }
        
        
    }
   
}
