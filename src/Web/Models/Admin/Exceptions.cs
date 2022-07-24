using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Views;
using ApplicationCore.Paging;

namespace Web.Models
{
    public class ExceptionsAdminModel
    {
        public List<string> Period { get; set; } = new List<string>();

        public PagedList<ExceptionViewModel> PagedList { get; set; }

        public List<string> Types { get; set; } = new List<string>();
    }
}
