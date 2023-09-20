using System;
using System.Runtime.Serialization;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[DataContract]
	[Serializable]
	public class BattleServerReadyResponseMessage : LoginResultObject
	{
	}
}
