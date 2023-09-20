using System;

namespace TaleWorlds.SaveSystem.Save
{
	public class SaveError
	{
		public string Message { get; private set; }

		internal SaveError(string message)
		{
			this.Message = message;
		}
	}
}
