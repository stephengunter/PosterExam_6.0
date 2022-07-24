using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Specifications
{

	public class MessageFilterSpecification : BaseSpecification<Message>
	{
		public MessageFilterSpecification() : base(item => !item.Removed) 
		{
			
		}

		public MessageFilterSpecification(bool returned) : base(item => !item.Removed && item.Returned == returned)
		{

		}

	}
}
