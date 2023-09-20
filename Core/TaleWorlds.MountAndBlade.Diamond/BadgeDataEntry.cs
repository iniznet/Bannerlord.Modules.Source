using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class BadgeDataEntry
	{
		[JsonProperty]
		public PlayerId PlayerId { get; set; }

		[JsonProperty]
		public string BadgeId { get; set; }

		[JsonProperty]
		public string ConditionId { get; set; }

		[JsonProperty]
		public int Count { get; set; }

		public static Dictionary<ValueTuple<PlayerId, string, string>, int> ToDictionary(List<BadgeDataEntry> entries)
		{
			Dictionary<ValueTuple<PlayerId, string, string>, int> dictionary = new Dictionary<ValueTuple<PlayerId, string, string>, int>();
			if (entries != null)
			{
				foreach (BadgeDataEntry badgeDataEntry in entries)
				{
					dictionary.Add(new ValueTuple<PlayerId, string, string>(badgeDataEntry.PlayerId, badgeDataEntry.BadgeId, badgeDataEntry.ConditionId), badgeDataEntry.Count);
				}
			}
			return dictionary;
		}

		public static List<BadgeDataEntry> ToList(Dictionary<ValueTuple<PlayerId, string, string>, int> dictionary)
		{
			List<BadgeDataEntry> list = new List<BadgeDataEntry>();
			if (dictionary != null)
			{
				foreach (KeyValuePair<ValueTuple<PlayerId, string, string>, int> keyValuePair in dictionary)
				{
					list.Add(new BadgeDataEntry
					{
						PlayerId = keyValuePair.Key.Item1,
						BadgeId = keyValuePair.Key.Item2,
						ConditionId = keyValuePair.Key.Item3,
						Count = keyValuePair.Value
					});
				}
			}
			return list;
		}
	}
}
