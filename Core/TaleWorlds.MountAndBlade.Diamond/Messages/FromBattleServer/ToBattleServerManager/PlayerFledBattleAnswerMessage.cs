using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000D3 RID: 211
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class PlayerFledBattleAnswerMessage : Message
	{
		// Token: 0x17000131 RID: 305
		// (get) Token: 0x0600030B RID: 779 RVA: 0x00004278 File Offset: 0x00002478
		// (set) Token: 0x0600030C RID: 780 RVA: 0x00004280 File Offset: 0x00002480
		public BattleResult BattleResult { get; private set; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600030D RID: 781 RVA: 0x00004289 File Offset: 0x00002489
		// (set) Token: 0x0600030E RID: 782 RVA: 0x00004291 File Offset: 0x00002491
		public PlayerId PlayerId { get; private set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0000429A File Offset: 0x0000249A
		// (set) Token: 0x06000310 RID: 784 RVA: 0x000042A2 File Offset: 0x000024A2
		public bool IsAllowedLeave { get; private set; }

		// Token: 0x06000311 RID: 785 RVA: 0x000042AB File Offset: 0x000024AB
		public PlayerFledBattleAnswerMessage(PlayerId playerId, BattleResult battleResult, bool isAllowedLeave)
		{
			this.PlayerId = playerId;
			this.BattleResult = battleResult;
			this.IsAllowedLeave = isAllowedLeave;
		}
	}
}
