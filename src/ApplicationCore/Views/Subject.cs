using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Paging;

namespace ApplicationCore.Views
{
	public class SubjectViewModel : BaseCategoryView
	{
		public int Id { get; set; }

		public ICollection<SubjectViewModel> SubItems { get; set; }

		public ICollection<int> SubIds { get; set; }

	}
}
