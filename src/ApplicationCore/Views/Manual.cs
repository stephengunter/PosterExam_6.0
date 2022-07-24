﻿using ApplicationCore.Helpers;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Views
{
    public class ManualViewModel : BaseCategoryView
    {
        public int Id { get; set; }

        public bool Free { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public IList<int> SubIds => SubItems.IsNullOrEmpty() ? new List<int>() : SubItems.Select(item => item.Id).ToList();

        public ICollection<FeatureViewModel> Features { get; set; } = new List<FeatureViewModel>();

        public ICollection<ManualViewModel> SubItems { get; set; }

        public ICollection<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();
    }

    public class FeatureViewModel : BaseRecordView
    {
        public int Id { get; set; }

        public int ManualId { get; set; }

        public string Key { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }


        public ICollection<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();
    }
}
