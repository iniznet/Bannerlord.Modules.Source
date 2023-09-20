using System;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class UpdateGamePropertiesMessage : Message
	{
		public string GameType { get; private set; }

		public string Scene { get; private set; }

		public string UniqueSceneId { get; private set; }

		public UpdateGamePropertiesMessage(string gameType, string scene, string uniqueSceneId)
		{
			this.GameType = gameType;
			this.Scene = scene;
			this.UniqueSceneId = uniqueSceneId;
		}
	}
}
