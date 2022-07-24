using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using ApplicationCore.Helpers;
using System.Linq;
using ApplicationCore.Exceptions;

namespace ApplicationCore.Models
{
	public class ExamQuestion : BaseEntity
	{
		public int ExamPartId { get; set; }
		public int Order { get; set; }
		public int QuestionId { get; set; }
		public string? AnswerIndexes { get; set; } = String.Empty;
		public string? OptionIds { get; set; } = String.Empty; // 12,4,34,15

		public string? UserAnswerIndexes { get; set; } = String.Empty;

		public bool Correct { get; private set; }
		
		


		public virtual ExamPart ExamPart { get; set; }
		public virtual Question Question { get; set; }
		
		
		[NotMapped]
		public ICollection<Resolve> Resolves { get; set; }

		[NotMapped]
		public ICollection<Option> Options { get; set; }


		#region Helpers

		public void SetCorrect()
		{
			if (AnswerIndexList.IsNullOrEmpty()) throw new NoAnswerToFinishException(ExamPart.ExamId, this.Id);
			Correct = UserAnswerIndexList.AllTheSame(AnswerIndexList);
		}


		public void LoadOptions()
		{
			if (Question.Options.IsNullOrEmpty()) return;

			this.Options = new List<Option>();
			var ids = OptionIds.SplitToIds();
			for (int i = 0; i < ids.Count; i++)
			{
				var item = Question.Options.FirstOrDefault(x => x.Id == ids[i]);
				this.Options.Add(item);
			}

		}

		public void SetAnswerIndexes()
		{
			var answerIndexList = new List<int>();
			var correctOptionIds = Question.Options.Where(o => o.Correct).Select(o => o.Id);

			for (int i = 0; i < OptionIdsList.Count; i++)
			{
				if (correctOptionIds.Contains(OptionIdsList[i])) answerIndexList.Add(i);
			}

			AnswerIndexes = answerIndexList.JoinToStringIntegers();

		}

		public void LoadResolves(IEnumerable<Resolve> resolves)
		{
			Resolves = resolves.Where(x => x.QuestionId == QuestionId).HasItems() 
								? resolves.Where(x => x.QuestionId == QuestionId).ToList() : new List<Resolve>();

		}


		public List<int> OptionIdsList => OptionIds.SplitToIntList();

		public List<int> AnswerIndexList => AnswerIndexes.SplitToIntList();

		public List<int> UserAnswerIndexList => UserAnswerIndexes.SplitToIntList();

		#endregion


	}
}
