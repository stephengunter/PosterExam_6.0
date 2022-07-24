using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class ResolveFilterSpecification : BaseSpecification<Resolve>
	{
		public ResolveFilterSpecification() : base(item => !item.Removed)
		{

		}

		public ResolveFilterSpecification(int questionId) : base(item => !item.Removed && item.QuestionId == questionId)
		{

		}

		public ResolveFilterSpecification(Exam exam) : base(item => !item.Removed && exam.QuestionIds.Contains(item.QuestionId))
		{

		}

	}
}
