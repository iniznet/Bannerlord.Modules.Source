using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000021 RID: 33
	[Serializable]
	public class GetAnotherPlayerStateMessageResult : FunctionResult
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000074 RID: 116 RVA: 0x0000252A File Offset: 0x0000072A
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00002532 File Offset: 0x00000732
		public AnotherPlayerData AnotherPlayerData { get; private set; }

		// Token: 0x06000076 RID: 118 RVA: 0x0000253B File Offset: 0x0000073B
		public GetAnotherPlayerStateMessageResult(AnotherPlayerState anotherPlayerState, int anotherPlayerExperience)
		{
			this.AnotherPlayerData = new AnotherPlayerData(anotherPlayerState, anotherPlayerExperience);
		}
	}
}
