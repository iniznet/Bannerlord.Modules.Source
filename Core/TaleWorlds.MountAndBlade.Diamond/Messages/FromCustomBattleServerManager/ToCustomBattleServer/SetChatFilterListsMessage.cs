using System;
using System.Runtime.Serialization;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	// Token: 0x02000066 RID: 102
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[DataContract]
	[Serializable]
	public class SetChatFilterListsMessage : Message
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00003290 File Offset: 0x00001490
		// (set) Token: 0x0600019E RID: 414 RVA: 0x00003298 File Offset: 0x00001498
		public string[] ProfanityList { get; private set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600019F RID: 415 RVA: 0x000032A1 File Offset: 0x000014A1
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x000032A9 File Offset: 0x000014A9
		public string[] AllowList { get; private set; }

		// Token: 0x060001A1 RID: 417 RVA: 0x000032B2 File Offset: 0x000014B2
		public SetChatFilterListsMessage(string[] profanityList, string[] allowList)
		{
			this.ProfanityList = profanityList;
			this.AllowList = allowList;
		}
	}
}
