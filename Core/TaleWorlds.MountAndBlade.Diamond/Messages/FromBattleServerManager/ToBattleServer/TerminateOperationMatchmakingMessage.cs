using System;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class TerminateOperationMatchmakingMessage : Message
	{
	}
}
