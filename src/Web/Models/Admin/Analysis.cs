using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Views;
using ApplicationCore.Settings;

namespace Web.Models
{
    public class AnalysisAdminModel
    {
        public ICollection<SubjectViewModel> Subjects { get; set; }

        public ICollection<RecruitViewModel> Recruits { get; set; }

        public RootSubjectSettings RootSubjectSettings { get; set; }




        public List<RecruitQuestionAnalysisView> ViewList { get; set; }

        public List<ExamSettingsViewModel> Results { get; set; }
    }
}
