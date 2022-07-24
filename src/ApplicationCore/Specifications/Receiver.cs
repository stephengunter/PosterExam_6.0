using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Specifications
{

	public class ReceiverFilterSpecification : BaseSpecification<Receiver>
	{
		public ReceiverFilterSpecification(string userId) : base(item => item.UserId == userId) 
		{
			AddInclude(item => item.Notice);
		}

		public ReceiverFilterSpecification(int id) : base(item => item.Id == id)
		{
			AddInclude(item => item.Notice);
		}

	}
}
