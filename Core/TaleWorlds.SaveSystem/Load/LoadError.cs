using System;

namespace TaleWorlds.SaveSystem.Load
{
	public class LoadError
	{
		public string Message { get; private set; }

		internal LoadError(string message)
		{
			this.Message = message;
		}
	}
}
