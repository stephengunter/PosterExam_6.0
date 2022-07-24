using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using ApplicationCore.ViewServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using System;
using ApplicationCore.Logging;
using System.IO;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Web.Controllers.Tests
{
    public class ABackupTestController : BaseTestController
    {
		private readonly AdminSettings _adminSettings;
		private readonly DefaultContext _context;
		private readonly ICloudStorageService _cloudStorageService;

		public ABackupTestController(IOptions<AdminSettings> adminSettings, DefaultContext context,
			ICloudStorageService cloudStorageService)
        {
			_adminSettings = adminSettings.Value;
			_context = context;
			_cloudStorageService = cloudStorageService;
			
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

		[HttpPost]
		public async Task<ActionResult> Test([FromBody] AdminRequest model)
		{
			if (model.Key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
			if (string.IsNullOrEmpty(model.Cmd)) ModelState.AddModelError("cmd", "指令錯誤");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (model.Cmd.EqualTo("local"))
			{
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
			}
			else if (model.Cmd.EqualTo("cloud"))
			{
				string storageFolder = DateTime.Today.ToDateNumber().ToString();

				foreach (var filePath in Directory.GetFiles(BackupFolder))
				{
					var fileInfo = new FileInfo(filePath);
					await _cloudStorageService.UploadFileAsync(filePath, $"{storageFolder}/{fileInfo.Name}");
				}
			}
			else
			{
				ModelState.AddModelError("cmd", "指令錯誤");
				return BadRequest(ModelState);
			}

			return Ok($"{model.Cmd} - OK");
		}
	}
}
