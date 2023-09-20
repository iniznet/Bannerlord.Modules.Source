using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class BattlePlayerStatsBaseJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(BattlePlayerStatsBase).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jobject = JObject.Load(reader);
			string text = (string)jobject["GameType"];
			BattlePlayerStatsBase battlePlayerStatsBase;
			if (text == "Skirmish")
			{
				battlePlayerStatsBase = new BattlePlayerStatsSkirmish();
			}
			else if (text == "Captain")
			{
				battlePlayerStatsBase = new BattlePlayerStatsCaptain();
			}
			else if (text == "Siege")
			{
				battlePlayerStatsBase = new BattlePlayerStatsSiege();
			}
			else if (text == "TeamDeathmatch")
			{
				battlePlayerStatsBase = new BattlePlayerStatsTeamDeathmatch();
			}
			else if (text == "Duel")
			{
				battlePlayerStatsBase = new BattlePlayerStatsDuel();
			}
			else
			{
				if (!(text == "Battle"))
				{
					return null;
				}
				battlePlayerStatsBase = new BattlePlayerStatsBattle();
			}
			serializer.Populate(jobject.CreateReader(), battlePlayerStatsBase);
			return battlePlayerStatsBase;
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
