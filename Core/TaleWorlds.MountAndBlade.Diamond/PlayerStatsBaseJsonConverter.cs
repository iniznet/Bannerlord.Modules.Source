using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaleWorlds.Diamond;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class PlayerStatsBaseJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(AccessObject).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jobject = JObject.Load(reader);
			string text = (string)jobject["gameType"];
			if (text == null)
			{
				text = (string)jobject["GameType"];
			}
			PlayerStatsBase playerStatsBase;
			if (text == "Skirmish")
			{
				playerStatsBase = new PlayerStatsSkirmish();
			}
			else if (text == "Captain")
			{
				playerStatsBase = new PlayerStatsCaptain();
			}
			else if (text == "TeamDeathmatch")
			{
				playerStatsBase = new PlayerStatsTeamDeathmatch();
			}
			else if (text == "Siege")
			{
				playerStatsBase = new PlayerStatsSiege();
			}
			else if (text == "Duel")
			{
				playerStatsBase = new PlayerStatsDuel();
			}
			else
			{
				if (!(text == "Battle"))
				{
					return null;
				}
				playerStatsBase = new PlayerStatsBattle();
			}
			serializer.Populate(jobject.CreateReader(), playerStatsBase);
			return playerStatsBase;
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
		}
	}
}
