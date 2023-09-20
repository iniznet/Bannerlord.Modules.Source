using System;

namespace TaleWorlds.Network
{
	public class ServiceException : Exception
	{
		public ServiceException(string type, string message)
			: base("ServiceException")
		{
			this.ExceptionType = type;
			this.ExceptionMessage = message;
		}

		public string ExceptionMessage { get; set; }

		public string ExceptionType { get; set; }
	}
}
