using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class RecruitQuestion : IAggregateRoot
	{
		public int RecruitId { get; set; }
		public virtual Recruit Recruit { get; set; }

		public int QuestionId { get; set; }
		public virtual Question Question { get; set; }
	}
}
