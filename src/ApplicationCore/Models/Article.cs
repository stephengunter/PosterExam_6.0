using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models
{
    public class Article : BaseRecord
    {
        public int CategoryId { get; set; }
        public string? Title { get; set; } = String.Empty;
        public string? Content { get; set; } = String.Empty;
        public string? Summary { get; set; } = String.Empty;
        public virtual Category Category { get; set; }


        [NotMapped]
        public virtual ICollection<UploadFile> Attachments { get; set; }

        public void LoadAttachments(IEnumerable<UploadFile> uploadFiles)
        {
            var attachments = uploadFiles.Where(x => x.PostType == PostType.Article && x.PostId == Id);
            this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<UploadFile>();
        }
    }
}
