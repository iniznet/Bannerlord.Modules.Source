using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000298 RID: 664
	public class MissionMultiplayerFFAClient : MissionMultiplayerGameModeBaseClient
	{
		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x060023E9 RID: 9193 RVA: 0x00085040 File Offset: 0x00083240
		public override bool IsGameModeUsingGold
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x060023EA RID: 9194 RVA: 0x00085043 File Offset: 0x00083243
		public override bool IsGameModeTactical
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x060023EB RID: 9195 RVA: 0x00085046 File Offset: 0x00083246
		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x060023EC RID: 9196 RVA: 0x00085049 File Offset: 0x00083249
		public override MissionLobbyComponent.MultiplayerGameType GameType
		{
			get
			{
				return MissionLobbyComponent.MultiplayerGameType.FreeForAll;
			}
		}

		// Token: 0x060023ED RID: 9197 RVA: 0x0008504C File Offset: 0x0008324C
		public override int GetGoldAmount()
		{
			return 0;
		}

		// Token: 0x060023EE RID: 9198 RVA: 0x0008504F File Offset: 0x0008324F
		public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
		{
		}

		// Token: 0x060023EF RID: 9199 RVA: 0x00085051 File Offset: 0x00083251
		public override void AfterStart()
		{
			base.Mission.SetMissionMode(MissionMode.Battle, true);
		}
	}
}
