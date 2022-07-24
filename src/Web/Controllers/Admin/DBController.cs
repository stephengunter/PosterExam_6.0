using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using Web.Controllers;

namespace Web.Controllers.Admin
{
	public class DBController : BaseAdminController
	{
		private readonly AdminSettings _adminSettings;
		private readonly DefaultContext _context;
		private readonly IDBImportService _dBImportService;

		public DBController(IOptions<AdminSettings> adminSettings, DefaultContext context, IDBImportService dBImportService)
		{
			_adminSettings = adminSettings.Value;
			_context = context;
			_dBImportService = dBImportService;
		}

		#region Properties

		string _connectionString;
		string ConnectionString
		{
			get
			{
				if (String.IsNullOrEmpty(_connectionString))
				{
					_connectionString = _context.Database.GetDbConnection().ConnectionString;
				}
				return _connectionString;
			}
		}

		string _dbName;
		string DbName
		{
			get
			{
				if (String.IsNullOrEmpty(_dbName))
				{
					_dbName = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
				}
				return _dbName;
			}
		}



		string _backupFolder;
		string BackupFolder
		{
			get
			{
				if (String.IsNullOrEmpty(_backupFolder))
				{
					var path = Path.Combine(_adminSettings.BackupPath, DateTime.Today.ToDateNumber().ToString());
					if (!Directory.Exists(path)) Directory.CreateDirectory(path);

					_backupFolder = path;
				}
				return _backupFolder;
			}
		}
		#endregion

		async Task<string> ReadFileTextAsync(IFormFile file)
		{
			var result = new StringBuilder();
			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				while (reader.Peek() >= 0) result.AppendLine(await reader.ReadLineAsync());
			}
			return result.ToString();

		}

		[HttpGet("dbname")]
		public ActionResult DBName() => Ok(DbName);

		[HttpPost("migrate")]
		public ActionResult Migrate([FromBody] AdminRequest model)
		{
			
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			_context.Database.Migrate();

			return Ok();
		}

		[HttpPost("backup")]
		public ActionResult Backup([FromBody] AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var fileName = Path.Combine(BackupFolder, $"{DbName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.bak");

			string cmdText = $"BACKUP DATABASE [{DbName}] TO DISK = '{fileName}'";
			using (var conn = new SqlConnection(ConnectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(cmdText, conn))
				{
					int result = cmd.ExecuteNonQuery();

				}
				conn.Close();
			}

			return Ok();
		}

		[HttpPost("export")]
		public ActionResult Export([FromBody] AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);
			
			var folderPath = BackupFolder;
		
			_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
			
			var subjects = _context.Subjects.ToList();
			SaveJson(folderPath, new Subject().GetType().Name, JsonConvert.SerializeObject(subjects));
			
			var terms = _context.Terms.ToList();
			SaveJson(folderPath, new Term().GetType().Name, JsonConvert.SerializeObject(terms));

			var questions = _context.Questions.ToList();
			SaveJson(folderPath, new Question().GetType().Name, JsonConvert.SerializeObject(questions));

			var options = _context.Options.ToList();
			SaveJson(folderPath, new Option().GetType().Name, JsonConvert.SerializeObject(options));

			var termQuestions = _context.TermQuestions.ToList();
			SaveJson(folderPath, new TermQuestion().GetType().Name, JsonConvert.SerializeObject(termQuestions));


			var resolves = _context.Resolves.ToList();
			SaveJson(folderPath, new Resolve().GetType().Name, JsonConvert.SerializeObject(resolves));

			var recruits = _context.Recruits.ToList();
			SaveJson(folderPath, new Recruit().GetType().Name, JsonConvert.SerializeObject(recruits));

			var recruitQuestions = _context.RecruitQuestions.ToList();
			SaveJson(folderPath, new RecruitQuestion().GetType().Name, JsonConvert.SerializeObject(recruitQuestions));

			var notes = _context.Notes.ToList();
			SaveJson(folderPath, new Note().GetType().Name, JsonConvert.SerializeObject(notes));

			var articles = _context.Articles.ToList();
			SaveJson(folderPath, new Article().GetType().Name, JsonConvert.SerializeObject(articles));

			var manuals = _context.Manuals.ToList();
			SaveJson(folderPath, new Manual().GetType().Name, JsonConvert.SerializeObject(manuals));

			var features = _context.Features.ToList();
			SaveJson(folderPath, new Feature().GetType().Name, JsonConvert.SerializeObject(features));

			var uploads = _context.UploadFiles.ToList();
			SaveJson(folderPath, new UploadFile().GetType().Name, JsonConvert.SerializeObject(uploads));

			var reviewRecords = _context.ReviewRecords.ToList();
			SaveJson(folderPath, new ReviewRecord().GetType().Name, JsonConvert.SerializeObject(reviewRecords));

			return Ok();
		}

		[HttpPost("import")]
		public async Task<IActionResult> Import([FromForm] AdminFileRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var fileNames = new List<string>();

			if (model.Files.Count < 1)
			{
				ModelState.AddModelError("files", "必須上傳檔案");
				return BadRequest(ModelState);
			}

			var extensions = model.Files.Select(item => Path.GetExtension(item.FileName).ToLower());
			if (extensions.Any(x => x != ".json"))
			{
				ModelState.AddModelError("files", "檔案格式錯誤");
				return BadRequest(ModelState);
			}

			string content = "";
			string fileName = new Subject().GetType().Name;
			var file = model.GetFile(fileName);
			if (file != null)
			{
				fileNames.Add(fileName);
				content = await ReadFileTextAsync(file);
				var subjectModels = JsonConvert.DeserializeObject<List<Subject>>(content);
				_dBImportService.ImportSubjects(_context, subjectModels);

				_dBImportService.SyncSubjects(_context, subjectModels);

			}

			fileName = new Term().GetType().Name;
			file = model.GetFile(fileName);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var termModels = JsonConvert.DeserializeObject<List<Term>>(content);
				_dBImportService.ImportTerms(_context, termModels);

				_dBImportService.SyncTerms(_context, termModels);
			}



			file = model.GetFile(new Question().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var questionModels = JsonConvert.DeserializeObject<List<Question>>(content);
				_dBImportService.ImportQuestions(_context, questionModels);

				_dBImportService.SyncQuestions(_context, questionModels);
			}

			file = model.GetFile(new Option().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var optionModels = JsonConvert.DeserializeObject<List<Option>>(content);
				_dBImportService.ImportOptions(_context, optionModels);

				_dBImportService.SyncOptions(_context, optionModels);
			}

			file = model.GetFile(new TermQuestion().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var termQuestionModels = JsonConvert.DeserializeObject<List<TermQuestion>>(content);
				_dBImportService.ImportTermQuestions(_context, termQuestionModels);

				_dBImportService.SyncTermQuestions(_context, termQuestionModels);
			}

			file = model.GetFile(new Resolve().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var resolveModels = JsonConvert.DeserializeObject<List<Resolve>>(content);
				_dBImportService.ImportResolves(_context, resolveModels);

				_dBImportService.SyncResolves(_context, resolveModels);
			}

			file = model.GetFile(new Recruit().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var recruitModels = JsonConvert.DeserializeObject<List<Recruit>>(content);
				_dBImportService.ImportRecruits(_context, recruitModels);

				_dBImportService.SyncRecruits(_context, recruitModels);
			}

			file = model.GetFile(new RecruitQuestion().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var recruitQuestionModels = JsonConvert.DeserializeObject<List<RecruitQuestion>>(content);
				_dBImportService.ImportRecruitQuestions(_context, recruitQuestionModels);

				_dBImportService.SyncRecruitQuestions(_context, recruitQuestionModels);
			}

			file = model.GetFile(new Note().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var noteModels = JsonConvert.DeserializeObject<List<Note>>(content);
				_dBImportService.ImportNotes(_context, noteModels);

				_dBImportService.SyncNotes(_context, noteModels);
			}

			file = model.GetFile(new Article().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var articleModels = JsonConvert.DeserializeObject<List<Article>>(content);
				_dBImportService.ImportArticles(_context, articleModels);

				_dBImportService.SyncArticles(_context, articleModels);
			}

			file = model.GetFile(new Manual().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var manualModels = JsonConvert.DeserializeObject<List<Manual>>(content);
				_dBImportService.ImportManuals(_context, manualModels);

				_dBImportService.SyncManuals(_context, manualModels);
			}

			file = model.GetFile(new Feature().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var featureModels = JsonConvert.DeserializeObject<List<Feature>>(content);
				_dBImportService.ImportFeatures(_context, featureModels);

				_dBImportService.SyncFeatures(_context, featureModels);
			}


			file = model.GetFile(new UploadFile().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var uploadFileModels = JsonConvert.DeserializeObject<List<UploadFile>>(content);
				_dBImportService.ImportUploadFiles(_context, uploadFileModels);

				_dBImportService.SyncUploadFiles(_context, uploadFileModels);
			}

			file = model.GetFile(new ReviewRecord().GetType().Name);
			if (file != null)
			{
				content = await ReadFileTextAsync(file);
				var reviewRecordModels = JsonConvert.DeserializeObject<List<ReviewRecord>>(content);
				_dBImportService.ImportReviewRecords(_context, reviewRecordModels);

				_dBImportService.SyncReviewRecords(_context, reviewRecordModels);
			}


			//end of import

			return Ok();
		}

		void SaveJson(string folderPath, string name, string content)
		{
			var filePath = Path.Combine(folderPath, $"{name}.json");
			System.IO.File.WriteAllText(filePath, content);
		}

	}
}
