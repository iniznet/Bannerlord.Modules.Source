using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	public class CheckClanTagValidResult : FunctionResult
	{
		public bool TagExists { get; private set; }

		public CheckClanTagValidResult(bool tagExists)
		{
			this.TagExists = tagExists;
		}
	}
}
