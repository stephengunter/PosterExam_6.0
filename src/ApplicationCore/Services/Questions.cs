using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;
using AutoMapper;

namespace ApplicationCore.Services
{
	public interface IQuestionsService
	{
		Task<IEnumerable<Question>> FetchAsync(Subject subject, ICollection<int> termIds = null, ICollection<int> recruitIds = null);
		Task<IEnumerable<Question>> FetchByRecruitAsync(Recruit recruit, Subject subject);
		Task<IEnumerable<Question>> FetchByIdsAsync(IEnumerable<int> ids);
		IEnumerable<int> FetchQuestionIdsByRecruit(Recruit recruit);
		IEnumerable<int> FetchQuestionIdsByRecruits(ICollection<int> recruitIds);
		IEnumerable<int> FetchAllRecruitQuestionIds();

		Task<Question> GetByIdAsync(int id);
		Task<Question> CreateAsync(Question question);
		Task UpdateAsync(Question question);
		Task RemoveAsync(Question question);

		Task UpdateAsync(Question existingEntity, Question model);
		Question GetById(int id);

		Task<Option> AddOptionAsync(Option option);
		Task<IEnumerable<Option>> FetchAllOptionsAsync();
	}

	public class QuestionsService : IQuestionsService
	{
		private readonly IDefaultRepository<Question> _questionRepository;
		private readonly IDefaultRepository<Option> _optionRepository;
		private readonly DefaultContext _context;

		public QuestionsService(IDefaultRepository<Question> questionRepository, IDefaultRepository<Option> optionRepository,
			DefaultContext context)
		{
			_questionRepository = questionRepository;
			_optionRepository = optionRepository;
			_context = context;
		}

		public async Task<IEnumerable<Question>> FetchAsync(Subject subject, ICollection<int> termIds = null, ICollection<int> recruitIds = null)
		{
			var subjectIds = subject.GetSubIds();
			subjectIds.Add(subject.Id);

			var spec = new QuestionSubjectIdsFilterSpecification(subjectIds);
			var list = await _questionRepository.ListAsync(spec);

			if (termIds.HasItems())
			{
				var questionIds = FetchQuestionIdsByTerms(termIds);
				list = list.Where(item => questionIds.Contains(item.Id)).ToList();
			}

			if (recruitIds.HasItems())
			{
				var questionIds = FetchQuestionIdsByRecruits(recruitIds);
				list = list.Where(item => questionIds.Contains(item.Id)).ToList();
			}

			

			return list;
		}

		public async Task<IEnumerable<Question>> FetchByRecruitAsync(Recruit recruit, Subject subject)
		{
			var subjectIds = subject.GetSubIds();
			subjectIds.Add(subject.Id);

			var spec = new QuestionSubjectIdsFilterSpecification(subjectIds);
			var list = await _questionRepository.ListAsync(spec);

			var recruitIds = recruit.GetSubIds();
			recruitIds.Add(recruit.Id);

			var questionIds = FetchQuestionIdsByRecruits(recruitIds);

			return list.Where(item => questionIds.Contains(item.Id)).ToList();
		}

		public async Task<IEnumerable<Question>> FetchByIdsAsync(IEnumerable<int> ids)
		{
			var spec = new QuestionFilterSpecification(ids);
			return await _questionRepository.ListAsync(spec);
		}

		public async Task<Question> GetByIdAsync(int id) => await _questionRepository.GetByIdAsync(id);

		public async Task<Question> CreateAsync(Question question)
		{
			question = await _questionRepository.AddAsync(question);

			if (!String.IsNullOrEmpty(question.TermIds))
			{
				await AddTermQuestionsAsync(question);
			}

			return question;
		}
		

		public async Task UpdateAsync(Question existingEntity, Question question)
		{
			existingEntity.RecruitQuestions = question.RecruitQuestions;

			await _questionRepository.UpdateAsync(existingEntity, question);
			
			_optionRepository.SyncList(existingEntity.Options.ToList(), question.Options.ToList());

			await SyncTermQuestions(question);

		}

		public async Task UpdateAsync(Question question) => await _questionRepository.UpdateAsync(question);

		public Question GetById(int id)
		{
			var spec = new QuestionFilterSpecification(id);
			return _questionRepository.GetSingleBySpec(spec);
		}

		public async Task RemoveAsync(Question question)
		{
			question.Removed = true;
			await _questionRepository.UpdateAsync(question);
		}

		public async Task<Option> AddOptionAsync(Option option) => await _optionRepository.AddAsync(option);

		public async Task<IEnumerable<Option>> FetchAllOptionsAsync() => await _optionRepository.ListAllAsync();

		
		public IEnumerable<int> FetchQuestionIdsByRecruit(Recruit recruit)
		{
			var recruitQuestions = _context.RecruitQuestions.Where(x => x.RecruitId == recruit.Id);
			if (recruitQuestions.IsNullOrEmpty()) return new List<int>();

			return recruitQuestions.Select(x => x.QuestionId).ToList();
		}

		public IEnumerable<int> FetchAllRecruitQuestionIds() => _context.RecruitQuestions.Select(item => item.QuestionId);

		public IEnumerable<int> FetchQuestionIdsByRecruits(ICollection<int> recruitIds)
		{
			var recruitQuestions = _context.RecruitQuestions.Where(x => recruitIds.Contains(x.RecruitId));
			if (recruitQuestions.IsNullOrEmpty()) return new List<int>();

			return recruitQuestions.Select(x => x.QuestionId).ToList();
		}

		IEnumerable<int> FetchQuestionIdsByTerms(ICollection<int> termIds)
		{
			var termQuestions = _context.TermQuestions.Where(x => termIds.Contains(x.TermId));
			if (termQuestions.IsNullOrEmpty()) return new List<int>();

			return termQuestions.Select(x => x.QuestionId).ToList();
		}



		async Task SyncTermQuestions(Question entity)
		{
			await RemoveTermQuestionsAsync(entity);
			if(!String.IsNullOrEmpty(entity.TermIds)) await AddTermQuestionsAsync(entity);
		}

		async Task RemoveTermQuestionsAsync(Question entity)
		{
			_context.TermQuestions.RemoveRange(_context.TermQuestions.Where(x => x.QuestionId == entity.Id).ToList());
			await _context.SaveChangesAsync();
		}

		async Task AddTermQuestionsAsync(Question entity)
		{
			var termIds = entity.TermIds.SplitToIds();
			var termQuestions = termIds.Select(termId => new TermQuestion { QuestionId = entity.Id, TermId = termId });

			_context.TermQuestions.AddRange(termQuestions);
			await _context.SaveChangesAsync();
		}

	}
}
