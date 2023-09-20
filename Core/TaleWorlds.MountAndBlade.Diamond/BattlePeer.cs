using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000E5 RID: 229
	public class BattlePeer
	{
		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000371 RID: 881 RVA: 0x00004780 File Offset: 0x00002980
		// (set) Token: 0x06000372 RID: 882 RVA: 0x00004788 File Offset: 0x00002988
		public int Index { get; private set; }

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000373 RID: 883 RVA: 0x00004791 File Offset: 0x00002991
		// (set) Token: 0x06000374 RID: 884 RVA: 0x00004799 File Offset: 0x00002999
		public string Name { get; private set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000375 RID: 885 RVA: 0x000047A2 File Offset: 0x000029A2
		public PlayerId PlayerId
		{
			get
			{
				return this.PlayerData.PlayerId;
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000376 RID: 886 RVA: 0x000047AF File Offset: 0x000029AF
		// (set) Token: 0x06000377 RID: 887 RVA: 0x000047B7 File Offset: 0x000029B7
		public int TeamNo { get; private set; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000378 RID: 888 RVA: 0x000047C0 File Offset: 0x000029C0
		// (set) Token: 0x06000379 RID: 889 RVA: 0x000047C8 File Offset: 0x000029C8
		public BattleJoinType BattleJoinType { get; private set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600037A RID: 890 RVA: 0x000047D1 File Offset: 0x000029D1
		public bool Quit
		{
			get
			{
				return this.QuitType > BattlePeerQuitType.None;
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600037B RID: 891 RVA: 0x000047DC File Offset: 0x000029DC
		// (set) Token: 0x0600037C RID: 892 RVA: 0x000047E4 File Offset: 0x000029E4
		public PlayerData PlayerData { get; private set; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600037D RID: 893 RVA: 0x000047ED File Offset: 0x000029ED
		// (set) Token: 0x0600037E RID: 894 RVA: 0x000047F5 File Offset: 0x000029F5
		public Dictionary<string, List<string>> UsedCosmetics { get; private set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600037F RID: 895 RVA: 0x000047FE File Offset: 0x000029FE
		// (set) Token: 0x06000380 RID: 896 RVA: 0x00004806 File Offset: 0x00002A06
		public int SessionKey { get; private set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000381 RID: 897 RVA: 0x0000480F File Offset: 0x00002A0F
		// (set) Token: 0x06000382 RID: 898 RVA: 0x00004817 File Offset: 0x00002A17
		public BattlePeerQuitType QuitType { get; private set; }

		// Token: 0x06000383 RID: 899 RVA: 0x00004820 File Offset: 0x00002A20
		public BattlePeer(string name, PlayerData playerData, Dictionary<string, List<string>> usedCosmetics, int teamNo, BattleJoinType battleJoinType)
		{
			this.Index = -1;
			this.Name = name;
			this.PlayerData = playerData;
			this.UsedCosmetics = usedCosmetics;
			this.TeamNo = teamNo;
			this.BattleJoinType = battleJoinType;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00004854 File Offset: 0x00002A54
		internal void Flee()
		{
			this.QuitType = BattlePeerQuitType.Fled;
			this.Index = -1;
			this.SessionKey = 0;
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0000486B File Offset: 0x00002A6B
		internal void SetPlayerDisconnectdFromLobby()
		{
			this.QuitType = BattlePeerQuitType.DisconnectedFromLobby;
			this.Index = -1;
			this.SessionKey = 0;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00004882 File Offset: 0x00002A82
		internal void SetPlayerDisconnectdFromGameSession()
		{
			this.QuitType = BattlePeerQuitType.DisconnectedFromGameSession;
			this.Index = -1;
			this.SessionKey = 0;
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00004899 File Offset: 0x00002A99
		public void Rejoin(int teamNo)
		{
			this.QuitType = BattlePeerQuitType.None;
			this.TeamNo = teamNo;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x000048A9 File Offset: 0x00002AA9
		public void InitializeSession(int index, int sessionKey)
		{
			this.Index = index;
			this.SessionKey = sessionKey;
		}

		// Token: 0x06000389 RID: 905 RVA: 0x000048B9 File Offset: 0x00002AB9
		internal void SetPlayerKickedDueToFriendlyDamage()
		{
			this.QuitType = BattlePeerQuitType.KickedDueToFriendlyDamage;
			this.Index = -1;
			this.SessionKey = 0;
		}
	}
}
