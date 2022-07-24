using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Specifications
{

	public class QuestionFilterSpecification : BaseSpecification<Question>
	{
		public QuestionFilterSpecification() : base(item => !item.Removed)
		{
			AddInclude(item => item.Options);
			AddInclude(item => item.Resolves);
			AddInclude("RecruitQuestions.Recruit");
		}

		public QuestionFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
		{
			AddInclude(item => item.Options);
			AddInclude(item => item.Resolves);
			AddInclude("RecruitQuestions.Recruit");
		}

		public QuestionFilterSpecification(IEnumerable<int> ids) : base(item => !item.Removed && ids.Contains(item.Id))
		{
			AddInclude(item => item.Options);
			AddInclude(item => item.Resolves);
			AddInclude("RecruitQuestions.Recruit");
		}

		public QuestionFilterSpecification(Subject subject) : base(item => !item.Removed && item.SubjectId == subject.Id)
		{
			AddInclude(item => item.Options);
			AddInclude(item => item.Resolves);
			AddInclude("RecruitQuestions.Recruit");
		}
	}

	public class QuestionSubjectIdsFilterSpecification : BaseSpecification<Question>
	{
		public QuestionSubjectIdsFilterSpecification(ICollection<int> subjectIds) 
			: base(item => !item.Removed && subjectIds.Contains(item.SubjectId))
		{
			AddInclude(item => item.Options);
			AddInclude(item => item.Resolves);
			AddInclude("RecruitQuestions.Recruit");
		}
	}
}
