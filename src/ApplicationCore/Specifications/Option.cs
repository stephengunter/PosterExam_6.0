using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class OptionFilterSpecification : BaseSpecification<Option>
	{
		public OptionFilterSpecification(int questionId) : base(item => item.QuestionId == questionId)
		{

		}
	}
}
