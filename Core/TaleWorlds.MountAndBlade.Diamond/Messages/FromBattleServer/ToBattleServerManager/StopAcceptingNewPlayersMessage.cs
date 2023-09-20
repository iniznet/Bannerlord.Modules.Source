using System;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class StopAcceptingNewPlayersMessage : Message
	{
	}
}
