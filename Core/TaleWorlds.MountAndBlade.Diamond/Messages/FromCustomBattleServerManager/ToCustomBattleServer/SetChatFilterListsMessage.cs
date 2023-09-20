using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[DataContract]
	[Serializable]
	public class SetChatFilterListsMessage : Message
	{
		[JsonProperty]
		public string[] ProfanityList { get; private set; }

		[JsonProperty]
		public string[] AllowList { get; private set; }

		public SetChatFilterListsMessage()
		{
		}

		public SetChatFilterListsMessage(string[] profanityList, string[] allowList)
		{
			this.ProfanityList = profanityList;
			this.AllowList = allowList;
		}
	}
}
