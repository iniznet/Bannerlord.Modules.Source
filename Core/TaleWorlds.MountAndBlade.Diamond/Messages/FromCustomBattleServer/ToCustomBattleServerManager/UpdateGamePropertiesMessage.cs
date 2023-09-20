using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class UpdateGamePropertiesMessage : Message
	{
		[JsonProperty]
		public string GameType { get; private set; }

		[JsonProperty]
		public string Scene { get; private set; }

		[JsonProperty]
		public string UniqueSceneId { get; private set; }

		public UpdateGamePropertiesMessage()
		{
		}

		public UpdateGamePropertiesMessage(string gameType, string scene, string uniqueSceneId)
		{
			this.GameType = gameType;
			this.Scene = scene;
			this.UniqueSceneId = uniqueSceneId;
		}
	}
}
