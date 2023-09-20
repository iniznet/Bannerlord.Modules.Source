using System;
using System.Runtime.Serialization;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[DataContract]
	[Serializable]
	public class CustomBattleServerReadyResponseMessage : LoginResultObject
	{
	}
}
