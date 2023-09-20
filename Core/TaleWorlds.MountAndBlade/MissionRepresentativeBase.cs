using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B9 RID: 697
	public abstract class MissionRepresentativeBase : PeerComponent
	{
		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x060026D8 RID: 9944 RVA: 0x00091E60 File Offset: 0x00090060
		protected MissionRepresentativeBase.PlayerTypes PlayerType
		{
			get
			{
				if (!base.Peer.Communicator.IsNetworkActive)
				{
					return MissionRepresentativeBase.PlayerTypes.Bot;
				}
				if (!base.Peer.Communicator.IsServerPeer)
				{
					return MissionRepresentativeBase.PlayerTypes.Client;
				}
				return MissionRepresentativeBase.PlayerTypes.Server;
			}
		}

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x060026D9 RID: 9945 RVA: 0x00091E8B File Offset: 0x0009008B
		// (set) Token: 0x060026DA RID: 9946 RVA: 0x00091E93 File Offset: 0x00090093
		public Agent ControlledAgent { get; private set; }

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x060026DB RID: 9947 RVA: 0x00091E9C File Offset: 0x0009009C
		// (set) Token: 0x060026DC RID: 9948 RVA: 0x00091EDC File Offset: 0x000900DC
		public int Gold
		{
			get
			{
				if (this._gold < 0)
				{
					return this._gold;
				}
				bool flag;
				MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.UnlimitedGold, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out flag);
				if (!flag)
				{
					return this._gold;
				}
				return 2000;
			}
			private set
			{
				if (value < 0)
				{
					this._gold = value;
					return;
				}
				bool flag;
				MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.UnlimitedGold, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out flag);
				this._gold = ((!flag) ? value : 2000);
			}
		}

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x060026DD RID: 9949 RVA: 0x00091F1A File Offset: 0x0009011A
		public MissionPeer MissionPeer
		{
			get
			{
				if (this._missionPeer == null)
				{
					this._missionPeer = base.GetComponent<MissionPeer>();
				}
				return this._missionPeer;
			}
		}

		// Token: 0x060026DF RID: 9951 RVA: 0x00091F3E File Offset: 0x0009013E
		public void SetAgent(Agent agent)
		{
			this.ControlledAgent = agent;
			if (this.ControlledAgent != null)
			{
				this.ControlledAgent.MissionRepresentative = this;
				this.OnAgentSpawned();
			}
		}

		// Token: 0x060026E0 RID: 9952 RVA: 0x00091F61 File Offset: 0x00090161
		public virtual void OnAgentSpawned()
		{
		}

		// Token: 0x060026E1 RID: 9953 RVA: 0x00091F63 File Offset: 0x00090163
		public virtual void Tick(float dt)
		{
		}

		// Token: 0x060026E2 RID: 9954 RVA: 0x00091F65 File Offset: 0x00090165
		public void UpdateGold(int gold)
		{
			this.Gold = gold;
			Action onGoldUpdated = this.OnGoldUpdated;
			if (onGoldUpdated == null)
			{
				return;
			}
			onGoldUpdated();
		}

		// Token: 0x04000E6A RID: 3690
		private int _gold;

		// Token: 0x04000E6B RID: 3691
		private MissionPeer _missionPeer;

		// Token: 0x04000E6C RID: 3692
		public Action OnGoldUpdated;

		// Token: 0x020005D7 RID: 1495
		protected enum PlayerTypes
		{
			// Token: 0x04001E5C RID: 7772
			Bot,
			// Token: 0x04001E5D RID: 7773
			Client,
			// Token: 0x04001E5E RID: 7774
			Server
		}
	}
}
