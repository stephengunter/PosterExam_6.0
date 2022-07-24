using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class TermQuestion : IAggregateRoot
	{
		public int TermId { get; set; }
		public int QuestionId { get; set; }
	}
}
