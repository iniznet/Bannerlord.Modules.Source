using System;
using System.Runtime.Serialization;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[DataContract]
	[Serializable]
	public class SetChatFilterListsMessage : Message
	{
		public string[] ProfanityList { get; private set; }

		public string[] AllowList { get; private set; }

		public SetChatFilterListsMessage(string[] profanityList, string[] allowList)
		{
			this.ProfanityList = profanityList;
			this.AllowList = allowList;
		}
	}
}
