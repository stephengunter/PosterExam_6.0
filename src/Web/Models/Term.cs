using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Views;

namespace Web.Models
{

	public class TermEditForm
	{
		public TermViewModel Term { get; set; } = new TermViewModel();

		public ICollection<TermViewModel> Parents { get; set; }
	}
}
