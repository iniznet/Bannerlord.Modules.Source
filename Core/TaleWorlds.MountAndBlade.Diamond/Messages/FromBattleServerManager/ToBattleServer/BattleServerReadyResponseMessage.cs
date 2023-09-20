using System;
using System.Runtime.Serialization;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	// Token: 0x020000D8 RID: 216
	[MessageDescription("BattleServerManager", "BattleServer")]
	[DataContract]
	[Serializable]
	public class BattleServerReadyResponseMessage : LoginResultObject
	{
	}
}
