using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Views;
using ApplicationCore.Paging;
using Infrastructure.Views;
using ApplicationCore.Models;

namespace Web.Models
{
    public class ArticlesAdminModel
    {
        public ICollection<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        public PagedList<Article, ArticleViewModel> PagedList { get; set; }
    }
}
