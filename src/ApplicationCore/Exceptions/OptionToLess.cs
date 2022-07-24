using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Exceptions
{
	public class OptionToLessException : Exception
	{
		public OptionToLessException(string message) : base(message)
		{
			
		}
	}
}
