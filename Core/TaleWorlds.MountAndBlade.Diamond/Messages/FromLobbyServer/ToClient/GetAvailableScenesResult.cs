using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetAvailableScenesResult : FunctionResult
	{
		public AvailableScenes AvailableScenes { get; private set; }

		public GetAvailableScenesResult(AvailableScenes scenes)
		{
			this.AvailableScenes = scenes;
		}
	}
}
