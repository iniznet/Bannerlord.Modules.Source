using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InitializeSessionResponse : LoginResultObject
	{
		public PlayerData PlayerData { get; private set; }

		public ServerStatus ServerStatus { get; private set; }

		public AvailableScenes AvailableScenes { get; private set; }

		public SupportedFeatures SupportedFeatures { get; private set; }

		public bool HasPendingRejoin { get; private set; }

		public InitializeSessionResponse(PlayerData playerData, ServerStatus serverStatus, AvailableScenes availableScenes, SupportedFeatures supportedFeatures, bool hasPendingRejoin)
		{
			this.PlayerData = playerData;
			this.ServerStatus = serverStatus;
			this.AvailableScenes = availableScenes;
			this.SupportedFeatures = supportedFeatures;
			this.HasPendingRejoin = hasPendingRejoin;
		}
	}
}
