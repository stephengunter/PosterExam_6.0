using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models
{
	public class OAuth : BaseEntity
	{
		public string UserId { get; set; }

		public string? Name { get; set; } = String.Empty;

		public string? FamilyName { get; set; } = String.Empty;

		public string? GivenName { get; set; } = String.Empty;

		public string? OAuthId { get; set; } = String.Empty;

		public OAuthProvider Provider { get; set; }

		public string? PictureUrl { get; set; } = String.Empty;
		

		public virtual User User { get; set; }
	}

	public enum OAuthProvider
	{
		Google = 0,
		Unknown = -1
	}
}
