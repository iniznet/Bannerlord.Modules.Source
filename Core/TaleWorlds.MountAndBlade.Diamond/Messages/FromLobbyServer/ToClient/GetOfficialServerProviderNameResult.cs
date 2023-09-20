using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetOfficialServerProviderNameResult : FunctionResult
	{
		public string Name { get; }

		public GetOfficialServerProviderNameResult(string name)
		{
			this.Name = name;
		}
	}
}
