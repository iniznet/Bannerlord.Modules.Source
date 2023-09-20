using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI.Mission
{
	[OverrideView(typeof(MultiplayerPollProgressUIHandler))]
	public class MissionGauntletPollingProgress : MissionView
	{
		private InputContext _input
		{
			get
			{
				return base.MissionScreen.SceneLayer.Input;
			}
		}

		public MissionGauntletPollingProgress()
		{
			this.ViewOrderPriority = 24;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._multiplayerPollComponent = base.Mission.GetMissionBehavior<MultiplayerPollComponent>();
			MultiplayerPollComponent multiplayerPollComponent = this._multiplayerPollComponent;
			multiplayerPollComponent.OnKickPollOpened = (Action<MissionPeer, MissionPeer, bool>)Delegate.Combine(multiplayerPollComponent.OnKickPollOpened, new Action<MissionPeer, MissionPeer, bool>(this.OnKickPollOpened));
			MultiplayerPollComponent multiplayerPollComponent2 = this._multiplayerPollComponent;
			multiplayerPollComponent2.OnPollUpdated = (Action<int, int>)Delegate.Combine(multiplayerPollComponent2.OnPollUpdated, new Action<int, int>(this.OnPollUpdated));
			MultiplayerPollComponent multiplayerPollComponent3 = this._multiplayerPollComponent;
			multiplayerPollComponent3.OnPollClosed = (Action)Delegate.Combine(multiplayerPollComponent3.OnPollClosed, new Action(this.OnPollClosed));
			MultiplayerPollComponent multiplayerPollComponent4 = this._multiplayerPollComponent;
			multiplayerPollComponent4.OnPollCancelled = (Action)Delegate.Combine(multiplayerPollComponent4.OnPollCancelled, new Action(this.OnPollClosed));
			this._dataSource = new MultiplayerPollProgressVM();
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerPollingProgress", this._dataSource);
			this._input.RegisterHotKeyCategory(HotKeyManager.GetCategory("PollHotkeyCategory"));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PollHotkeyCategory").GetGameKey(106));
			this._dataSource.AddKey(HotKeyManager.GetCategory("PollHotkeyCategory").GetGameKey(107));
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		public override void OnMissionScreenFinalize()
		{
			MultiplayerPollComponent multiplayerPollComponent = this._multiplayerPollComponent;
			multiplayerPollComponent.OnKickPollOpened = (Action<MissionPeer, MissionPeer, bool>)Delegate.Remove(multiplayerPollComponent.OnKickPollOpened, new Action<MissionPeer, MissionPeer, bool>(this.OnKickPollOpened));
			MultiplayerPollComponent multiplayerPollComponent2 = this._multiplayerPollComponent;
			multiplayerPollComponent2.OnPollUpdated = (Action<int, int>)Delegate.Remove(multiplayerPollComponent2.OnPollUpdated, new Action<int, int>(this.OnPollUpdated));
			MultiplayerPollComponent multiplayerPollComponent3 = this._multiplayerPollComponent;
			multiplayerPollComponent3.OnPollClosed = (Action)Delegate.Remove(multiplayerPollComponent3.OnPollClosed, new Action(this.OnPollClosed));
			MultiplayerPollComponent multiplayerPollComponent4 = this._multiplayerPollComponent;
			multiplayerPollComponent4.OnPollCancelled = (Action)Delegate.Remove(multiplayerPollComponent4.OnPollCancelled, new Action(this.OnPollClosed));
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			MultiplayerPollProgressVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			base.MissionScreen.SetDisplayDialog(false);
			base.OnMissionScreenFinalize();
		}

		private void OnKickPollOpened(MissionPeer initiatorPeer, MissionPeer targetPeer, bool isBanRequested)
		{
			this._isActive = true;
			this._isVoteOpenForMyPeer = NetworkMain.GameClient.PlayerID == targetPeer.Peer.Id;
			this._dataSource.OnKickPollOpened(initiatorPeer, targetPeer, isBanRequested);
		}

		private void OnPollUpdated(int votesAccepted, int votesRejected)
		{
			this._dataSource.OnPollUpdated(votesAccepted, votesRejected);
		}

		private void OnPollClosed()
		{
			this._isActive = false;
			this._dataSource.OnPollClosed();
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._isActive && !this._isVoteOpenForMyPeer)
			{
				if (this._input.IsGameKeyPressed(106))
				{
					this._isActive = false;
					this._multiplayerPollComponent.Vote(true);
					this._dataSource.OnPollOptionPicked();
					return;
				}
				if (this._input.IsGameKeyPressed(107))
				{
					this._isActive = false;
					this._multiplayerPollComponent.Vote(false);
					this._dataSource.OnPollOptionPicked();
				}
			}
		}

		private MultiplayerPollComponent _multiplayerPollComponent;

		private MultiplayerPollProgressVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private bool _isActive;

		private bool _isVoteOpenForMyPeer;
	}
}
