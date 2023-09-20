using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000022 RID: 34
	[Serializable]
	public class GetAvailableScenesResult : FunctionResult
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00002550 File Offset: 0x00000750
		// (set) Token: 0x06000078 RID: 120 RVA: 0x00002558 File Offset: 0x00000758
		public AvailableScenes AvailableScenes { get; private set; }

		// Token: 0x06000079 RID: 121 RVA: 0x00002561 File Offset: 0x00000761
		public GetAvailableScenesResult(AvailableScenes scenes)
		{
			this.AvailableScenes = scenes;
		}
	}
}
