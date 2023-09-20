﻿using System;

namespace TaleWorlds.Network
{
	public class ServiceExceptionModel
	{
		public string ExceptionMessage { get; set; }

		public string ExceptionType { get; set; }

		public ServiceException ToServiceException()
		{
			return new ServiceException(this.ExceptionType, this.ExceptionMessage);
		}
	}
}
