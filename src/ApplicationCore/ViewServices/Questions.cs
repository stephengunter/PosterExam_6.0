using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Views;
using AutoMapper;

namespace ApplicationCore.ViewServices
{
	public static class QuestionsViewService
	{
		#region MapViewModel

		public static QuestionViewModel MapViewModel(this Question question, IMapper mapper)
			=> mapper.Map<QuestionViewModel>(question);

		static void LoadQuestionData(Question question, ICollection<Recruit> allRecruits, 
			ICollection<UploadFile> attachmentsList = null, ICollection<Term> allTerms = null)
		{

			if (question.Resolves.HasItems()) question.Resolves = question.Resolves.Where(item => !item.Removed).ToList();

			if (allRecruits.HasItems())
			{
				if (question.Recruits.HasItems())
				{
					foreach (var item in question.Recruits)
					{
						item.LoadParents(allRecruits);
					}
				}
			}

			if (attachmentsList.HasItems())
			{
				question.LoadAttachments(attachmentsList);

				foreach (var option in question.Options)
				{
					option.LoadAttachments(attachmentsList);
				}

				foreach (var resolve in question.Resolves)
				{
					resolve.LoadAttachments(attachmentsList);
				}
			}

			if (allTerms.HasItems()) question.LoadTerms(allTerms);

		}

		public static QuestionViewModel MapViewModel(this Question question, IMapper mapper, ICollection<Recruit> allRecruits = null, 
			ICollection<UploadFile> attachmentsList = null, ICollection<Term> allTerms = null)

		{
			LoadQuestionData(question, allRecruits, attachmentsList, allTerms);

			var model = mapper.Map<QuestionViewModel>(question);

			if (question.Recruits.HasItems()) model.Recruits = question.Recruits.MapViewModelList(mapper);

			if (question.Resolves.HasItems()) model.Resolves = question.Resolves.MapViewModelList(mapper, attachmentsList);

			return model;
		}


		public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper)
			=> questions.Select(item => MapViewModel(item, mapper)).ToList();

		public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper, ICollection<Recruit> rootRecruits = null,
			ICollection<UploadFile> attachments = null, ICollection<Term> allTerms = null)
			=> questions.Select(item => MapViewModel(item, mapper, rootRecruits, attachments, allTerms)).ToList();


		#endregion

		

		public static PagedList<Question, QuestionViewModel> GetPagedList(this IEnumerable<Question> questions, IMapper mapper, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Question, QuestionViewModel>(questions, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}


		public static PagedList<Question, QuestionViewModel> GetPagedList(this IEnumerable<Question> questions, IMapper mapper, ICollection<Recruit> allRecruits,
			 ICollection<UploadFile> attachments = null, ICollection<Term> allTerms = null, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Question, QuestionViewModel>(questions, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper, allRecruits, attachments, allTerms);

			pageList.List = null;

			return pageList;
		}



		#region MapEntity
		public static Question MapEntity(this QuestionViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<QuestionViewModel, Question>(model);

			if (model.Id == 0)
			{
				entity.SetCreated(currentUserId);
			}
			else
			{
				foreach (var option in entity.Options)
				{
					option.QuestionId = entity.Id;
				}
				entity.SetUpdated(currentUserId);
			}

			return entity;
		}

		#endregion


		public static IEnumerable<Question> FilterByKeyword(this IEnumerable<Question> questions, ICollection<string> keywords)
			=> questions.Where(item => keywords.Any(item.Title.CaseInsensitiveContains)).ToList();
		

	}
}
