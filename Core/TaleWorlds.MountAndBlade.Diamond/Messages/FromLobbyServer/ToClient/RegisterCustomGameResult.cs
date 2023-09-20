using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class RegisterCustomGameResult : FunctionResult
	{
		public bool Success { get; private set; }

		public RegisterCustomGameResult(bool success)
		{
			this.Success = success;
		}
	}
}
