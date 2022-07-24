using ApplicationCore.Settings;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using ApplicationCore.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Newtonsoft.Json;
using System.Linq;
using ApplicationCore.DataAccess;

namespace ApplicationCore.Services
{
    public interface IDataService
    {
        NoteParamsViewModel FindNoteParams(string userId);
        void SaveNoteParams(string userId, NoteParamsViewModel model);

        ExamSettingsViewModel FindExamSettings(int subjectId);
        void SaveExamSettings(int subjectId, ExamSettingsViewModel model);

        IEnumerable<SubjectQuestionsViewModel> FindSubjectQuestions(int subjectId);
        void SaveSubjectQuestions(int subjectId, IEnumerable<SubjectQuestionsViewModel> models);

        IEnumerable<RecruitViewModel> FetchYearRecruits();
        void SaveYearRecruits(IEnumerable<RecruitViewModel> models);

        IEnumerable<NoteCategoryViewModel> FetchNoteCategories();
        void SaveNoteCategories(IEnumerable<NoteCategoryViewModel> models);

        IEnumerable<TermViewModel> FetchTermNotesBySubject(int subjectId);
        TermViewModel FindTermNotesByTerm(int termId);
        TermNotes FindTermNotesViewByTerm(int termId);
        IEnumerable<TermNotes> FetchTermNotesViewBySubject(int subjectId);
        void CleanTermNotes();
        void SaveTermNotes(TermViewModel model, List<NoteViewModel> noteViewList, List<int> RQIds, List<int> qIds);

    }

    public class DataService : IDataService
    {
        private readonly IDataRepository<NoteParams> _noteParamsRepository;
        private readonly IDataRepository<ExamSettings> _examSettingsRepository;
        private readonly IDataRepository<SubjectQuestions> _subjectQuestionsRepository;
        private readonly IDataRepository<YearRecruit> _yearRecruitsRepository;
        private readonly IDataRepository<NoteCategories> _noteCategoriesRepository;
        private readonly IDataRepository<TermNotes> _termNotesRepository;

        public DataService(IDataRepository<NoteParams> noteParamsRepository,
            IDataRepository<ExamSettings> examSettingsRepository, IDataRepository<SubjectQuestions> subjectQuestionsRepository,
            IDataRepository<YearRecruit> yearRecruitsRepository, IDataRepository<NoteCategories> noteCategoriesRepository,
            IDataRepository<TermNotes> termNotesRepository)
        {
            _noteParamsRepository = noteParamsRepository;
            _examSettingsRepository = examSettingsRepository;
            _subjectQuestionsRepository = subjectQuestionsRepository;
            _yearRecruitsRepository = yearRecruitsRepository;
            _noteCategoriesRepository = noteCategoriesRepository;
            _termNotesRepository = termNotesRepository;
        }

        public void SaveNoteParams(string userId, NoteParamsViewModel model)
        {
            var existingDoc = _noteParamsRepository.Get(item => item.UserId == userId);
            string content = JsonConvert.SerializeObject(model);

            if (existingDoc == null) _noteParamsRepository.Add(new NoteParams { UserId = userId, Content = content });
            else
            {

                existingDoc.Content = content;
                existingDoc.LastUpdated = DateTime.Now;
                _noteParamsRepository.Update(existingDoc);
            }
        }

        public NoteParamsViewModel FindNoteParams(string userId)
        {
            var doc = _noteParamsRepository.Get(item => item.UserId == userId);
            if (doc == null) return null;

            return JsonConvert.DeserializeObject<NoteParamsViewModel>(doc.Content);
        }


        public void SaveExamSettings(int subjectId, ExamSettingsViewModel model)
        {
            var existingDoc = _examSettingsRepository.Get(item => item.SubjectId == subjectId);
            string content = JsonConvert.SerializeObject(model);

            if (existingDoc == null) _examSettingsRepository.Add(new ExamSettings { SubjectId = subjectId, Content = content });
            else
            {

                existingDoc.Content = content;
                existingDoc.LastUpdated = DateTime.Now;
                _examSettingsRepository.Update(existingDoc);
            }
        }

        public ExamSettingsViewModel FindExamSettings(int subjectId)
        {
            var doc = _examSettingsRepository.Get(item => item.SubjectId == subjectId);
            if (doc == null) return null;

            return JsonConvert.DeserializeObject<ExamSettingsViewModel>(doc.Content);

        }

        public IEnumerable<SubjectQuestionsViewModel> FindSubjectQuestions(int subjectId)
        {
            var doc = _subjectQuestionsRepository.Get(item => item.SubjectId == subjectId);
            if (doc == null) return null;

            return JsonConvert.DeserializeObject<IEnumerable<SubjectQuestionsViewModel>>(doc.Content);
        }

        public void SaveSubjectQuestions(int subjectId, IEnumerable<SubjectQuestionsViewModel> models)
        {
            var existingDoc = _subjectQuestionsRepository.Get(item => item.SubjectId == subjectId);
            string content = JsonConvert.SerializeObject(models);

            if (existingDoc == null) _subjectQuestionsRepository.Add(new SubjectQuestions { SubjectId = subjectId, Content = content });
            else
            {

                existingDoc.Content = content;
                existingDoc.LastUpdated = DateTime.Now;
                _subjectQuestionsRepository.Update(existingDoc);
            }
        }

        public IEnumerable<RecruitViewModel> FetchYearRecruits()
        {
            var docs = _yearRecruitsRepository.ListAll();
            if (docs.IsNullOrEmpty()) return null;

            return docs.Select(doc => JsonConvert.DeserializeObject<RecruitViewModel>(doc.Content));
        }

        public void SaveYearRecruits(IEnumerable<RecruitViewModel> models)
        {
            var exitingItems = _yearRecruitsRepository.ListAll();
            if(exitingItems.HasItems()) _yearRecruitsRepository.DeleteRange(exitingItems);


            var docs = models.Select(model => new YearRecruit { Content = JsonConvert.SerializeObject(model) });

            _yearRecruitsRepository.AddRange(docs.ToList());
        }

        public IEnumerable<NoteCategoryViewModel> FetchNoteCategories()
        {
            var docs = _noteCategoriesRepository.ListAll();
            if (docs.IsNullOrEmpty()) return null;

            return docs.Select(doc => JsonConvert.DeserializeObject<NoteCategoryViewModel>(doc.Content));
        }

        public void SaveNoteCategories(IEnumerable<NoteCategoryViewModel> models)
        {
            var exitingItems = _noteCategoriesRepository.ListAll();
            if (exitingItems.HasItems()) _noteCategoriesRepository.DeleteRange(exitingItems);
           

            var docs = models.Select(model => new NoteCategories { Content = JsonConvert.SerializeObject(model) });

            _noteCategoriesRepository.AddRange(docs.ToList());
        }

        public IEnumerable<TermViewModel> FetchTermNotesBySubject(int subjectId)
        {
            var docs = _termNotesRepository.GetMany(x => x.SubjectId == subjectId);
            if (docs.IsNullOrEmpty()) return new List<TermViewModel>();

            return docs.Select(doc => JsonConvert.DeserializeObject<TermViewModel>(doc.Content));
        }

        public TermViewModel FindTermNotesByTerm(int termId)
        {
            var doc = _termNotesRepository.Get(item => item.TermId == termId);
            if (doc == null) return null;

            return JsonConvert.DeserializeObject<TermViewModel>(doc.Content);
        }

        public TermNotes FindTermNotesViewByTerm(int termId) => _termNotesRepository.Get(item => item.TermId == termId);
        public IEnumerable<TermNotes> FetchTermNotesViewBySubject(int subjectId)
            => _termNotesRepository.GetMany(x => x.SubjectId == subjectId);

        public void CleanTermNotes()
        {
            var exitingItems = _termNotesRepository.ListAll();
            if (exitingItems.HasItems()) _termNotesRepository.DeleteRange(exitingItems);
        }

        public void SaveTermNotes(TermViewModel model, List<NoteViewModel> noteViewList, List<int> RQIds, List<int> qIds)
        {
            int termId = model.Id;
            int subjectId = model.SubjectId;

            model.Subject = null;
            if (model.SubItems.HasItems()) foreach (var item in model.SubItems) item.Subject = null;

            model.LoadNotes(noteViewList);

            var termNote = new TermNotes
            {
                SubjectId = subjectId,
                TermId = termId,
                Content = JsonConvert.SerializeObject(model),
                RQIds = RQIds.JoinToStringIntegers(),
                QIds = qIds.JoinToStringIntegers()
            };


            _termNotesRepository.Add(termNote);


        }

    }
}
