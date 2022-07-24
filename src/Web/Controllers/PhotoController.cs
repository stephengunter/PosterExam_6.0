using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ApplicationCore.Settings;
using ApplicationCore.Services;
using Web.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Api
{
	public class PhotoController : BaseController
	{
		private readonly IWebHostEnvironment _environment;
		private readonly AppSettings _appSettings;
		private readonly IAttachmentsService _attachmentsService;

		public PhotoController(IWebHostEnvironment environment, IOptions<AppSettings> appSettings, IAttachmentsService attachmentsService)
		{
			_environment = environment;
			_appSettings = appSettings.Value;
			_attachmentsService = attachmentsService;
		}

		string UploadFilesPath => Path.Combine(_environment.WebRootPath, _appSettings.UploadPath);

		string GetImgSourcePath(string filename) => Path.Combine(UploadFilesPath, filename);

		[HttpGet("")]
		public IActionResult Index(string name, string? type = "", int width = 0, int height = 0)
		{
			string imgSourcePath = ValidateRequest(name);
			if (String.IsNullOrEmpty(imgSourcePath)) return BadRequest(ModelState);

			// 長寬數值不正確時, 回傳原圖
			if (width < 0 || height < 0) return SendOriginalImage(imgSourcePath);
			if (width == 0 && height == 0) return SendOriginalImage(imgSourcePath);

			if (width == 0) width = height;
			else if (height == 0) height = width;


			var resizeType = type.ToImageResizeType();
			if (resizeType == ImageResizeType.Unknown) resizeType = ImageResizeType.Scale;


			var imgSource = Image.FromStream(new MemoryStream(System.IO.File.ReadAllBytes(imgSourcePath)));

			Image imgResized = imgSource.Resize(resizeType, width, height);

			if (imgResized == null) return SendOriginalImage(imgSourcePath);
			else
			{
				var outputStream = new MemoryStream();

				imgResized.Save(outputStream, ImageFormat.Jpeg);
				outputStream.Seek(0, SeekOrigin.Begin);

				return this.File(outputStream, "image/jpeg");
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Index(int id, int width = 0, int height = 0, string type = "")
		{
			var entity = await _attachmentsService.GetByIdAsync(id);
			if (entity == null) return NotFound();

			
			return Index(entity.PreviewPath, type, width, height);

		}


		// 傳回原始圖片
		private IActionResult SendOriginalImage(string imgSourcePath)
		{
			string type = "image/jpeg";

			string ext = Path.GetExtension(imgSourcePath).ToLower();
			if (ext == "png") type = "image/png";
			else if (ext == "gif") type = "image/gif";

			var image = System.IO.File.OpenRead(imgSourcePath);
			return File(image, type);

		}

		string ValidateRequest(string name)
		{
			string imgSourcePath = GetImgSourcePath(name);
			if (!System.IO.File.Exists(imgSourcePath))
			{
				ModelState.AddModelError("path", "圖片路徑無效");
				return "";
			}

			string extension = (Path.HasExtension(imgSourcePath)) ?
										  System.IO.Path.GetExtension(imgSourcePath).Substring(1).ToLower() :
										  string.Empty;


			if (!("jpg".Equals(extension) || "gif".Equals(extension) || "png".Equals(extension)))
			{
				ModelState.AddModelError("path", "圖片格式錯誤");
				return "";
			}

			return imgSourcePath;

		}

	}
}