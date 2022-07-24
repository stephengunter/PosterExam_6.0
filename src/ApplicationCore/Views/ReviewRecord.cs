using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
    public class ReviewRecordViewModel : BaseReviewableView
    {
        public int PostId { get; set; }

        public string Type { get; set; }
    }
}
