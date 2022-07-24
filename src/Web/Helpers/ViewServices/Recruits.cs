using ApplicationCore.Views;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Helpers;
using System.Linq;
using ApplicationCore.Models;
using ApplicationCore.ViewServices;
using Web.Models;

namespace Web.Helpers.ViewServices
{
    public static class RecruitsViewService
    {
        public static BaseOption<int> ToYearOption(this Recruit recruit)
            => new BaseOption<int>(recruit.Id, recruit.Title);

        public static ICollection<BaseOption<int>> ToYearOptions(this IEnumerable<Recruit> recruits)
            => recruits.Select(item => item.ToYearOption()).ToList();


    }
}
