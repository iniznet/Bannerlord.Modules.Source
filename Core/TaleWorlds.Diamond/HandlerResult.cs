using System;

namespace TaleWorlds.Diamond
{
	public class HandlerResult
	{
		public bool IsSuccessful { get; }

		public string Error { get; }

		protected HandlerResult(bool isSuccessful, string error = null)
		{
			this.IsSuccessful = isSuccessful;
			this.Error = error;
		}

		public static HandlerResult CreateSuccessful()
		{
			return new HandlerResult(true, null);
		}

		public static HandlerResult CreateFailed(string error)
		{
			return new HandlerResult(false, error);
		}
	}
}
