using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Diamond
{
	public class AccessObjectResult
	{
		public object AccessObject { get; private set; }

		public bool Success { get; private set; }

		public TextObject FailReason { get; private set; }

		public static AccessObjectResult CreateSuccess(object accessObject)
		{
			return new AccessObjectResult
			{
				Success = true,
				AccessObject = accessObject
			};
		}

		public static AccessObjectResult CreateFailed(TextObject failReason)
		{
			return new AccessObjectResult
			{
				Success = false,
				FailReason = failReason
			};
		}
	}
}
