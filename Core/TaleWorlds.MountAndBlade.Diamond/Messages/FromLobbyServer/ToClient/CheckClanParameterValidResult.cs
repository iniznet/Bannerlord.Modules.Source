using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class CheckClanParameterValidResult : FunctionResult
	{
		public bool IsValid { get; private set; }

		public StringValidationError Error { get; private set; }

		public CheckClanParameterValidResult(bool isValid, StringValidationError error)
		{
			this.IsValid = isValid;
			this.Error = error;
		}
	}
}
