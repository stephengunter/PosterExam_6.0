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
    public class Note : BaseCategory
    {
        public int TermId { get; set; }

        public string? Text { get; set; } = String.Empty;

        public string? Highlight { get; set; } = String.Empty; //json string

        public string? Reference { get; set; } = String.Empty; //json string

        public virtual Term Term { get; set; }

        public bool Important { get; set; }

        [NotMapped]
        public ICollection<Note> SubItems { get; private set; }

        [NotMapped]
        public ICollection<int> ParentIds { get; private set; } = new List<int>();


        [NotMapped]
        public ICollection<UploadFile> Attachments { get; set; }


        #region Helpers

        public bool HasKeyword(string keyword)
        {
            
            if (String.IsNullOrEmpty(this.Text)) return false;
            return this.Text.Contains(keyword);
        }
       
        public void LoadAttachments(IEnumerable<UploadFile> uploadFiles)
        {
            var attachments = uploadFiles.Where(x => x.PostType == PostType.Note && x.PostId == Id);
            this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<UploadFile>();
        }

        public ICollection<int> GetSubIds()
        {
            var subIds = new List<int>();
            foreach (var item in SubItems)
            {
                subIds.Add(item.Id);

                subIds.AddRange(item.GetSubIds());
            }
            return subIds;
        }
        public void LoadSubItems(IEnumerable<Note> subItems)
        {
            SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

            foreach (var item in SubItems)
            {
                item.LoadSubItems(subItems);
            }
        }
        public void LoadParentIds(IEnumerable<Note> allNotes)
        {
            var parentIds = new List<int>();
            Note root = null;
            if (ParentId > 0)
            {
                int parentId = ParentId;

                do
                {
                    var parent = allNotes.Where(item => item.Id == parentId).FirstOrDefault();
                    if (parent == null) throw new Exception($"Note not found. id = {parentId}");

                    if (parent.IsRootItem) root = parent;
                    parentId = parent.Id;
                    parentIds.Insert(0, parentId);

                } while (root == null);
            }


            ParentIds = parentIds;
        }

        #endregion
    }
}
