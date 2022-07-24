using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;

namespace ApplicationCore.Services
{
	public interface IExamsService
	{
		Task<IEnumerable<Exam>> FetchAsync(User user);
		void SaveExam(Exam existingEntity, Exam exam);

		Exam GetById(int id, bool withOptions = false);
		Task<Exam> CreateAsync(Exam exam, string userId);
		Task<Exam> GetByIdAsync(int id);
		Task UpdateAsync(Exam exam);
		Task UpdateAsync(Exam existingEntity, Exam model);
		Task RemoveAsync(Exam exam);
		Task DeleteAsync(Exam exam);

	}

	public class ExamsService : IExamsService
	{
		private readonly IDefaultRepository<Exam> _examRepository;
		private readonly IDefaultRepository<ExamQuestion> _examQuestionRepository;

		public ExamsService(IDefaultRepository<Exam> examRepository, IDefaultRepository<ExamQuestion> examQuestionRepository)			
		{
			_examRepository = examRepository;
			_examQuestionRepository = examQuestionRepository;
		}

		public async Task<IEnumerable<Exam>> FetchAsync(User user)
		{
			var spec = new ExamFilterSpecification(user);
			var exams = await _examRepository.ListAsync(spec);
			return exams.Where(x => x.Reserved);
		}

		public void SaveExam(Exam existingEntity, Exam exam)
		{
			//指針對問題更新
			var examQuestions = exam.Parts.SelectMany(p => p.Questions);
			_examQuestionRepository.UpdateRange(examQuestions);

			exam.Reserved = true;
			_examRepository.Update(existingEntity, exam);
		}

		public Exam GetById(int id, bool withOptions = false)
		{
			var spec = new ExamFilterSpecification(id, withOptions);
			return _examRepository.GetSingleBySpec(spec);
		}

		public async Task<Exam> GetByIdAsync(int id) => await _examRepository.GetByIdAsync(id);

		public async Task<Exam> CreateAsync(Exam exam, string userId)
		{
			exam.UserId = userId;
			exam.SetCreated(userId);
			return await _examRepository.AddAsync(exam);
		}


		public async Task UpdateAsync(Exam exam) => await _examRepository.UpdateAsync(exam);

		public async Task UpdateAsync(Exam existingEntity, Exam exam) => await _examRepository.UpdateAsync(existingEntity, exam);

		public async Task RemoveAsync(Exam exam)
		{
			exam.Removed = true;
			await _examRepository.UpdateAsync(exam);
		}

		public async Task DeleteAsync(Exam exam) => await _examRepository.DeleteAsync(exam);

	}
}
