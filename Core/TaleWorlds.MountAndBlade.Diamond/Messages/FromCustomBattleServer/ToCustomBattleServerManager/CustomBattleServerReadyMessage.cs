﻿using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleServerReadyMessage : LoginMessage
	{
		[JsonProperty]
		public ApplicationVersion ApplicationVersion { get; private set; }

		[JsonProperty]
		public string AuthToken { get; private set; }

		[JsonProperty]
		public ModuleInfoModel[] LoadedModules { get; private set; }

		[JsonProperty]
		public bool AllowsOptionalModules { get; private set; }

		public CustomBattleServerReadyMessage()
		{
		}

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
