﻿using System;
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
	public static class ExamsViewService
	{
		#region  MapViewModel
		public static ExamViewModel MapExamViewModel(this Exam exam, IMapper mapper) => mapper.Map<ExamViewModel>(exam);

		public static ExamViewModel MapExamViewModel(this Exam exam, IMapper mapper, ICollection<UploadFile> attachments, ICollection<Resolve> resolves = null)
		{
			bool hasResolves = resolves.HasItems();

			var examQuestions = exam.Parts.SelectMany(p => p.Questions);
			foreach (var examQuestion in examQuestions)
			{
				examQuestion.Question.LoadAttachments(attachments);
				examQuestion.LoadOptions();
				foreach (var option in examQuestion.Question.Options)
                {
					option.LoadAttachments(attachments);
				}
				if (hasResolves) examQuestion.LoadResolves(resolves);
			}
			
			var model = mapper.Map<ExamViewModel>(exam);

			if (hasResolves)
			{
				//載入解析與附圖
				var examQuestionModels = model.Parts.SelectMany(p => p.Questions);
				foreach (var item in examQuestionModels)
				{
					var examQuestion = examQuestions.FirstOrDefault(x => x.Id == item.Id);
					item.Resolves = examQuestion.Resolves.MapViewModelList(mapper, attachments);
				}
			}
			else
			{
				//測驗模式, 不要顯示選項是否正確
				var optionViews = model.Parts.SelectMany(c => c.Questions).SelectMany(t => t.Options);
				foreach (var optionView in optionViews) optionView.Correct = false;
			}

			

			return model;
		}

		public static List<ExamViewModel> MapViewModelList(this IEnumerable<Exam> exams, IMapper mapper)
			=> exams.Select(item => MapExamViewModel(item, mapper)).ToList();

		public static List<ExamViewModel> MapViewModelList(this IEnumerable<Exam> exams, IMapper mapper, ICollection<UploadFile> attachments, ICollection<Resolve> resolves = null)
			=> exams.Select(item => MapExamViewModel(item, mapper, attachments, resolves)).ToList();


		public static PagedList<Exam, ExamViewModel> GetPagedList(this IEnumerable<Exam> exams, IMapper mapper, 
			int page = 1, int pageSize = 999, string sortBy = "lastUpdated", bool desc = true)
		{
			var pageList = new PagedList<Exam, ExamViewModel>(exams, page, pageSize, sortBy, desc);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}

		public static PagedList<Exam, ExamViewModel> GetPagedList(this IEnumerable<Exam> exams, IMapper mapper, ICollection<UploadFile> attachments, ICollection<Resolve> resolves = null,
			int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Exam, ExamViewModel>(exams, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper, attachments, resolves);

			pageList.List = null;

			return pageList;
		}


		#endregion


		#region  MapEntity
		public static Exam MapEntity(this ExamViewModel model, IMapper mapper, string currentUserId)
		{
			var examQuestionModels = model.Parts.SelectMany(p => p.Questions);
			foreach (var examQuestionModel in examQuestionModels)
			{
				examQuestionModel.Question = null;
				examQuestionModel.Options = null;
			}

			var entity = mapper.Map<ExamViewModel, Exam>(model);

			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}
		#endregion

		public static IEnumerable<Exam> GetOrdered(this IEnumerable<Exam> exams, string sortBy = "lastUpdated", bool desc = true)
		{
			if (String.IsNullOrEmpty(sortBy)) sortBy = "LastUpdated".ToLower();

			if (sortBy.EqualTo("score"))
			{ 
				return desc ? exams.OrderByDescending(item => item.Score) : exams.OrderBy(item => item.Score);
			}

			if (sortBy.EqualTo("lastupdated"))
			{
				return desc ? exams.OrderByDescending(item => item.LastUpdated) : exams.OrderBy(item => item.LastUpdated);
			}

			return exams.OrderByDescending(item => item.LastUpdated);

		}


		public static OptionType ToOptionType(this string val)
		{
			try
			{
				var type = val.ToEnum<OptionType>();
				return type;
			}
			catch (Exception ex)
			{
				return OptionType.Number;
			}
		}


	}
}
