using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Specifications
{

	public class NoticeFilterSpecification : BaseSpecification<Notice>
	{
		public NoticeFilterSpecification() : base(item => !item.Removed) 
		{
			AddInclude(item => item.Receivers);
		}

		public NoticeFilterSpecification(bool isPublic) : base(item => !item.Removed && item.Public == isPublic)
		{
			AddInclude(item => item.Receivers);
		}

	}
}
