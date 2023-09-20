using System;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000049 RID: 73
	public class MultiplayerVoiceChatVM : ViewModel
	{
		// Token: 0x0600060C RID: 1548 RVA: 0x00019220 File Offset: 0x00017420
		public MultiplayerVoiceChatVM(Mission mission)
		{
			this._mission = mission;
			this._voiceChatHandler = this._mission.GetMissionBehavior<VoiceChatHandler>();
			if (this._voiceChatHandler != null)
			{
				this._voiceChatHandler.OnPeerVoiceStatusUpdated += this.OnPeerVoiceStatusUpdated;
				this._voiceChatHandler.OnVoiceRecordStarted += this.OnVoiceRecordStarted;
				this._voiceChatHandler.OnVoiceRecordStopped += this.OnVoiceRecordStopped;
			}
			this.ActiveVoicePlayers = new MBBindingList<MPVoicePlayerVM>();
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x000192A4 File Offset: 0x000174A4
		public override void OnFinalize()
		{
			if (this._voiceChatHandler != null)
			{
				this._voiceChatHandler.OnPeerVoiceStatusUpdated -= this.OnPeerVoiceStatusUpdated;
				this._voiceChatHandler.OnVoiceRecordStarted -= this.OnVoiceRecordStarted;
				this._voiceChatHandler.OnVoiceRecordStopped -= this.OnVoiceRecordStopped;
			}
			base.OnFinalize();
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00019304 File Offset: 0x00017504
		public void OnTick(float dt)
		{
			for (int i = 0; i < this.ActiveVoicePlayers.Count; i++)
			{
				if (!this.ActiveVoicePlayers[i].IsMyPeer && this.ActiveVoicePlayers[i].UpdatesSinceSilence >= 30)
				{
					this.ActiveVoicePlayers.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00019360 File Offset: 0x00017560
		private void OnPeerVoiceStatusUpdated(MissionPeer peer, bool isTalking)
		{
			MPVoicePlayerVM mpvoicePlayerVM = this.ActiveVoicePlayers.FirstOrDefault((MPVoicePlayerVM vp) => vp.Peer == peer);
			if (!isTalking)
			{
				if (!isTalking && mpvoicePlayerVM != null)
				{
					mpvoicePlayerVM.UpdatesSinceSilence++;
				}
				return;
			}
			if (mpvoicePlayerVM == null)
			{
				this.ActiveVoicePlayers.Add(new MPVoicePlayerVM(peer));
				return;
			}
			mpvoicePlayerVM.UpdatesSinceSilence = 0;
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x000193CB File Offset: 0x000175CB
		private void OnVoiceRecordStarted()
		{
			this.ActiveVoicePlayers.Add(new MPVoicePlayerVM(GameNetwork.MyPeer.GetComponent<MissionPeer>()));
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x000193E8 File Offset: 0x000175E8
		private void OnVoiceRecordStopped()
		{
			MPVoicePlayerVM mpvoicePlayerVM = this.ActiveVoicePlayers.FirstOrDefault((MPVoicePlayerVM vp) => vp.Peer == GameNetwork.MyPeer.GetComponent<MissionPeer>());
			this.ActiveVoicePlayers.Remove(mpvoicePlayerVM);
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x0001942D File Offset: 0x0001762D
		// (set) Token: 0x06000613 RID: 1555 RVA: 0x00019435 File Offset: 0x00017635
		[DataSourceProperty]
		public MBBindingList<MPVoicePlayerVM> ActiveVoicePlayers
		{
			get
			{
				return this._activeVoicePlayers;
			}
			set
			{
				if (value != this._activeVoicePlayers)
				{
					this._activeVoicePlayers = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPVoicePlayerVM>>(value, "ActiveVoicePlayers");
				}
			}
		}

		// Token: 0x0400030E RID: 782
		private readonly Mission _mission;

		// Token: 0x0400030F RID: 783
		private readonly VoiceChatHandler _voiceChatHandler;

		// Token: 0x04000310 RID: 784
		private MBBindingList<MPVoicePlayerVM> _activeVoicePlayers;
	}
}
