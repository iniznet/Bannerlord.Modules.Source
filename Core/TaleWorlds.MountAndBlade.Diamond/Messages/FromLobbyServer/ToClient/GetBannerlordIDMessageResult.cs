using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetBannerlordIDMessageResult : FunctionResult
	{
		public string BannerlordID { get; }

		public GetBannerlordIDMessageResult(string bannerlordID)
		{
			this.BannerlordID = bannerlordID;
		}
	}
}
