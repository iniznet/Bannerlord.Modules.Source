using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class GameLog
	{
		public int Id { get; set; }

		public GameLogType Type { get; set; }

		public PlayerId Player { get; set; }

		public float GameTime { get; set; }

		public Dictionary<string, string> Data { get; set; }

		public GameLog()
		{
		}

		public GameLog(GameLogType type, PlayerId player, float gameTime)
		{
			this.Type = type;
			this.Player = player;
			this.GameTime = gameTime;
			this.Data = new Dictionary<string, string>();
		}

		public string GetDataAsString()
		{
			string text = "{}";
			try
			{
				text = JsonConvert.SerializeObject(this.Data, Formatting.None);
			}
			catch (Exception)
			{
			}
			return text;
		}
	}
}
