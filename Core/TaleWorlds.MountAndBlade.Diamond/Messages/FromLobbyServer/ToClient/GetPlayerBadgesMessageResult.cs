using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetPlayerBadgesMessageResult : FunctionResult
	{
		public string[] Badges { get; private set; }

		public GetPlayerBadgesMessageResult(string[] badges)
		{
			this.Badges = badges;
		}
	}
}
