using System;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class InitializeSession : LoginMessage
	{
		public PlayerId PlayerId { get; private set; }

		public string PlayerName { get; private set; }

		public object AccessObject { get; private set; }

		public ApplicationVersion ApplicationVersion { get; private set; }

		public string ConnectionPassword { get; private set; }

		public ModuleInfoModel[] LoadedModules { get; private set; }

		public InitializeSession(PlayerId playerId, string playerName, object accessObject, ApplicationVersion applicationVersion, string connectionPassword, ModuleInfoModel[] loadedModules)
			: base(playerId.ConvertToPeerId())
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
			this.AccessObject = accessObject;
			this.ApplicationVersion = applicationVersion;
			this.ConnectionPassword = connectionPassword;
			this.LoadedModules = loadedModules;
		}
	}
}
