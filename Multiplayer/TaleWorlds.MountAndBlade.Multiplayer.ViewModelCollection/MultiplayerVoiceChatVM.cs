using System;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MultiplayerVoiceChatVM : ViewModel
	{
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

		private void OnVoiceRecordStarted()
		{
			this.ActiveVoicePlayers.Add(new MPVoicePlayerVM(PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer)));
		}

		private void OnVoiceRecordStopped()
		{
			MPVoicePlayerVM mpvoicePlayerVM = this.ActiveVoicePlayers.FirstOrDefault((MPVoicePlayerVM vp) => vp.Peer == PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer));
			this.ActiveVoicePlayers.Remove(mpvoicePlayerVM);
		}

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

		private readonly Mission _mission;

		private readonly VoiceChatHandler _voiceChatHandler;

		private MBBindingList<MPVoicePlayerVM> _activeVoicePlayers;
	}
}
