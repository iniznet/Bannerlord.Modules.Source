using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CustomBattleOverMessage : Message
	{
		[JsonProperty]
		public int OldExperience { get; set; }

		[JsonProperty]
		public int NewExperience { get; set; }

		[JsonProperty]
		public int GoldGain { get; set; }

		public CustomBattleOverMessage()
		{
		}

		public CustomBattleOverMessage(int oldExperience, int newExperience, int goldGain)
		{
			this.OldExperience = oldExperience;
			this.NewExperience = newExperience;
			this.GoldGain = goldGain;
		}
	}
}
