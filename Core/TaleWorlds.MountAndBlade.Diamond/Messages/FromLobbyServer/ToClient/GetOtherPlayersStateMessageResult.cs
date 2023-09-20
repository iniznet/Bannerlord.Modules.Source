using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000029 RID: 41
	[Serializable]
	public class GetOtherPlayersStateMessageResult : FunctionResult
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600008A RID: 138 RVA: 0x0000261E File Offset: 0x0000081E
		public List<ValueTuple<PlayerId, AnotherPlayerData>> States { get; }

		// Token: 0x0600008B RID: 139 RVA: 0x00002626 File Offset: 0x00000826
		public GetOtherPlayersStateMessageResult(List<ValueTuple<PlayerId, AnotherPlayerData>> states)
		{
			this.States = states;
		}
	}
}
