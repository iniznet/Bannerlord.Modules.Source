using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MissionRepresentativeBase : PeerComponent
	{
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

		public Agent ControlledAgent { get; private set; }

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

		public void SetAgent(Agent agent)
		{
			this.ControlledAgent = agent;
			if (this.ControlledAgent != null)
			{
				this.ControlledAgent.MissionRepresentative = this;
				this.OnAgentSpawned();
			}
		}

		public virtual void OnAgentSpawned()
		{
		}

		public virtual void Tick(float dt)
		{
		}

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

		private int _gold;

		private MissionPeer _missionPeer;

		public Action OnGoldUpdated;

		protected enum PlayerTypes
		{
			Bot,
			Client,
			Server
		}
	}
}
