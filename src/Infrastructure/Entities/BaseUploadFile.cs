using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Entities
{
	public abstract class BaseUploadFile : BaseRecord
	{
		public string? Path { get; set; } = string.Empty;

		public string? PreviewPath { get; set; } = string.Empty;

		public int Width { get; set; }

		public int Height { get; set; }

		public string? Type { get; set; } = string.Empty;

		public string? Name { get; set; } = string.Empty;

		public string? Title { get; set; } = string.Empty;

		public string? Description { get; set; } = string.Empty;

	}
}
