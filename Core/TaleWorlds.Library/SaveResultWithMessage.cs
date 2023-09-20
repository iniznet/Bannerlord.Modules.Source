using System;

namespace TaleWorlds.Library
{
	public struct SaveResultWithMessage
	{
		public static SaveResultWithMessage Default
		{
			get
			{
				return new SaveResultWithMessage(SaveResult.Success, string.Empty);
			}
		}

		public SaveResultWithMessage(SaveResult result, string message)
		{
			this.SaveResult = result;
			this.Message = message;
		}

		public readonly SaveResult SaveResult;

		public readonly string Message;
	}
}
