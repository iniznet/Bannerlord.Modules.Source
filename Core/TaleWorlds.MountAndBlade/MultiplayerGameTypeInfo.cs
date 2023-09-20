using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerGameTypeInfo
	{
		public string GameModule { get; private set; }

		public string GameType { get; private set; }

		public List<string> Scenes { get; private set; }

		public MultiplayerGameTypeInfo(string gameModule, string gameType)
		{
			this.GameModule = gameModule;
			this.GameType = gameType;
			this.Scenes = new List<string>();
		}
	}
}
