using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class TermFilterSpecification : BaseSpecification<Term>
	{
		public TermFilterSpecification() : base(item => !item.Removed) 
		{
			AddInclude(item => item.Subject);
		}

		public TermFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
		{
			AddInclude(item => item.Subject);
		}

		public TermFilterSpecification(Subject subject) : base(item => !item.Removed && item.SubjectId == subject.Id)
		{
			AddInclude(item => item.Subject);
		}

		public TermFilterSpecification(Subject subject, int parentId)
			: base(item => !item.Removed && item.SubjectId == subject.Id && item.ParentId == parentId)
		{
			AddInclude(item => item.Subject);
		}
	}

	public class TermFilterBySubjectsSpecification : BaseSpecification<Term>
	{
		public TermFilterBySubjectsSpecification(IList<int> subjectIds) : base(item => !item.Removed && subjectIds.Contains(item.SubjectId))
		{
			AddInclude(item => item.Subject);
		}

		public TermFilterBySubjectsSpecification(IList<int> subjectIds, int parentId) : base(item => !item.Removed && subjectIds.Contains(item.SubjectId) && item.ParentId == parentId )
		{
			AddInclude(item => item.Subject);
		}
	}
}
