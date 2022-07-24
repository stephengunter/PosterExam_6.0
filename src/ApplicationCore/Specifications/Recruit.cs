using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class RecruitFilterSpecification : BaseSpecification<Recruit>
	{
		public RecruitFilterSpecification() : base(item => !item.Removed)
		{

		}

		public RecruitFilterSpecification(int parentId) : base(item => !item.Removed && item.ParentId == parentId)
		{

		}

		public RecruitFilterSpecification(RecruitEntityType type) : base(item => !item.Removed && item.RecruitEntityType == type)
		{

		}


	}

	public class RecruitFilterBySubjectSpecification : BaseSpecification<Recruit>
	{
		public RecruitFilterBySubjectSpecification(int subjectId) : base(item => !item.Removed && item.SubjectId == subjectId)
		{

		}
	}
}
