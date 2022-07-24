using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models
{
	public class Subject : BaseCategory
	{
		public virtual ICollection<Term> Terms { get; set; }

		public virtual ICollection<Question> Questions { get; set; }
		

		[NotMapped]
		public ICollection<Subject> SubItems { get; private set; }

		[NotMapped]
		public ICollection<int> SubIds { get; private set; }


		public void LoadSubItems(IEnumerable<Subject> subItems, bool activeOnly = false)
		{
			var children = subItems.Where(item => item.ParentId == this.Id);
			if (activeOnly) children = children.Where(item => item.Active);

			SubItems = children.OrderBy(item => item.Order).ToList();

			foreach (var item in SubItems)
			{
				item.LoadSubItems(subItems);
			}
		}

		public ICollection<int> GetSubIds()
		{
			var subIds = new List<int>();
			if (SubItems.HasItems())
			{
				foreach (var item in SubItems)
				{
					subIds.Add(item.Id);

					subIds.AddRange(item.GetSubIds());
				}
			}
			

			this.SubIds = subIds;
			return subIds;
		}

		public List<int> GetQuestionIds()
		{
			if (Terms.IsNullOrEmpty()) return new List<int>();
			return Terms.SelectMany(item => item.GetQuestionIds()).Distinct().ToList();
		}
	}
}
