using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Specifications
{

	public class ExamFilterSpecification : BaseSpecification<Exam>
	{
		public ExamFilterSpecification() : base(item => !item.Removed)
		{
			AddInclude("Parts.Questions");
		}

		public ExamFilterSpecification(int id, bool withOptions = false) : base(item => !item.Removed && item.Id == id)
		{
			if(withOptions) AddInclude("Parts.Questions.Question.Options");
			else AddInclude("Parts.Questions.Question");
		}

		public ExamFilterSpecification(User user) : base(item => !item.Removed && item.UserId == user.Id)
		{
			AddInclude("Parts.Questions");
		}
	}
}
