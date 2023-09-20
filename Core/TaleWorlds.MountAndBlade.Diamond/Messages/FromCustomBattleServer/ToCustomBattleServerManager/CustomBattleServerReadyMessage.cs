using System;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleServerReadyMessage : LoginMessage
	{
		public ApplicationVersion ApplicationVersion { get; private set; }

		public string AuthToken { get; private set; }

		public ModuleInfoModel[] LoadedModules { get; private set; }

		public bool AllowsOptionalModules { get; private set; }

		public CustomBattleServerReadyMessage(PeerId peerId, ApplicationVersion applicationVersion, string authToken, ModuleInfoModel[] loadedModules, bool allowsOptionalModules)
			: base(peerId)
		{
			this.ApplicationVersion = applicationVersion;
			this.AuthToken = authToken;
			this.LoadedModules = loadedModules;
			this.AllowsOptionalModules = allowsOptionalModules;
		}
	}
}
