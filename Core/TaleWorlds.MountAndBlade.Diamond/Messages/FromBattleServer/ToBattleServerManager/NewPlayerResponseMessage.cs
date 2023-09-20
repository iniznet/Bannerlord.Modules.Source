using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000D1 RID: 209
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class NewPlayerResponseMessage : Message
	{
		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060002FD RID: 765 RVA: 0x000041D7 File Offset: 0x000023D7
		// (set) Token: 0x060002FE RID: 766 RVA: 0x000041DF File Offset: 0x000023DF
		public PlayerId PlayerId { get; private set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060002FF RID: 767 RVA: 0x000041E8 File Offset: 0x000023E8
		// (set) Token: 0x06000300 RID: 768 RVA: 0x000041F0 File Offset: 0x000023F0
		public PlayerBattleServerInformation PlayerBattleInformation { get; private set; }

		// Token: 0x06000301 RID: 769 RVA: 0x000041F9 File Offset: 0x000023F9
		public NewPlayerResponseMessage(PlayerId playerId, PlayerBattleServerInformation playerBattleInformation)
		{
			this.PlayerId = playerId;
			this.PlayerBattleInformation = playerBattleInformation;
		}
	}
}
