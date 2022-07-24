using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System.Linq;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models
{
	public class Option : BaseEntity
	{
		public string? Title { get; set; } = String.Empty;
		public bool Correct { get; set; }
		public int QuestionId { get; set; }

		public virtual Question Question { get; set; }

		[NotMapped]
		public ICollection<UploadFile> Attachments { get; set; }


		public void LoadAttachments(IEnumerable<UploadFile> uploadFiles)
		{
			var attachments = uploadFiles.Where(x => x.PostType == PostType.Option && x.PostId == Id);
			this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<UploadFile>();
		}

	}
}
