using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Entities;

namespace ApplicationCore.Models
{
    public class Category : BaseCategory
    {
        public string? Key { get; set; } = String.Empty;


        public virtual ICollection<Article> Articles { get; set; }
    }

    public class CategoryKeys
    {
        public static string Experience = "Experience";
    }
}
