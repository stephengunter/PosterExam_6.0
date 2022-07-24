using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;

namespace ApplicationCore.Services
{
	public abstract class BaseCategoriesService<T> where T : BaseCategory
	{
		protected IEnumerable<T> AllRootItems(DbSet<T> categoryDbSet) => categoryDbSet.Where(item => !item.Removed && item.ParentId == 0);

		protected IEnumerable<T> AllSubItems(DbSet<T> categoryDbSet) => categoryDbSet.Where(item => !item.Removed && item.ParentId > 0);

		protected List<int> ResolveSelectedIds(int[] selectedIds, DbSet<T> categoryDbSet)
		{
			if (selectedIds.IsNullOrEmpty()) return null;

			int lastId = selectedIds[selectedIds.Length - 1];
			int parentId = 0;
			if (selectedIds.Length == 1)
			{

			}
			else
			{
				parentId = selectedIds[selectedIds.Length - 2];
			}

			if (lastId > 0) return new List<int> { lastId };

			if (parentId == 0) return null;

			return categoryDbSet.Where(item => !item.Removed && item.ParentId == parentId).Select(item => item.Id).ToList();

		}

	}
}
