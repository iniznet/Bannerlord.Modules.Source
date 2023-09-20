using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000046 RID: 70
	[OverrideView(typeof(MultiplayerPollProgressUIHandler))]
	public class MissionGauntletPollingProgress : MissionView
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000341 RID: 833 RVA: 0x00012792 File Offset: 0x00010992
		private InputContext _input
		{
			get
			{
				return base.MissionScreen.SceneLayer.Input;
			}
		}

		// Token: 0x06000342 RID: 834 RVA: 0x000127A4 File Offset: 0x000109A4
		public MissionGauntletPollingProgress()
		{
			this.ViewOrderPriority = 24;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x000127B4 File Offset: 0x000109B4
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

		// Token: 0x06000344 RID: 836 RVA: 0x0001290C File Offset: 0x00010B0C
		public override void OnMissionScreenFinalize()
		{
			MultiplayerPollComponent multiplayerPollComponent = this._multiplayerPollComponent;
			multiplayerPollComponent.OnKickPollOpened = (Action<MissionPeer, MissionPeer, bool>)Delegate.Remove(multiplayerPollComponent.OnKickPollOpened, new Action<MissionPeer, MissionPeer, bool>(this.OnKickPollOpened));
			MultiplayerPollComponent multiplayerPollComponent2 = this._multiplayerPollComponent;
			multiplayerPollComponent2.OnPollUpdated = (Action<int, int>)Delegate.Remove(multiplayerPollComponent2.OnPollUpdated, new Action<int, int>(this.OnPollUpdated));
			MultiplayerPollComponent multiplayerPollComponent3 = this._multiplayerPollComponent;
			multiplayerPollComponent3.OnPollClosed = (Action)Delegate.Remove(multiplayerPollComponent3.OnPollClosed, new Action(this.OnPollClosed));
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

		// Token: 0x06000345 RID: 837 RVA: 0x000129D1 File Offset: 0x00010BD1
		private void OnKickPollOpened(MissionPeer initiatorPeer, MissionPeer targetPeer, bool isBanRequested)
		{
			this._isActive = true;
			this._isVoteOpenForMyPeer = NetworkMain.GameClient.PlayerID == targetPeer.Peer.Id;
			this._dataSource.OnKickPollOpened(initiatorPeer, targetPeer, isBanRequested);
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00012A08 File Offset: 0x00010C08
		private void OnPollUpdated(int votesAccepted, int votesRejected)
		{
			this._dataSource.OnPollUpdated(votesAccepted, votesRejected);
		}

		// Token: 0x06000347 RID: 839 RVA: 0x00012A17 File Offset: 0x00010C17
		private void OnPollClosed()
		{
			this._isActive = false;
			this._dataSource.OnPollClosed();
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00012A2C File Offset: 0x00010C2C
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

		// Token: 0x040001C1 RID: 449
		private MultiplayerPollComponent _multiplayerPollComponent;

		// Token: 0x040001C2 RID: 450
		private MultiplayerPollProgressVM _dataSource;

		// Token: 0x040001C3 RID: 451
		private GauntletLayer _gauntletLayer;

		// Token: 0x040001C4 RID: 452
		private bool _isActive;

		// Token: 0x040001C5 RID: 453
		private bool _isVoteOpenForMyPeer;
	}
}
