using System;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MPVoicePlayerVM : MPPlayerVM
	{
		public MPVoicePlayerVM(MissionPeer peer)
			: base(peer)
		{
			this.UpdatesSinceSilence = 0;
			this.IsMyPeer = peer.IsMine;
		}

		public const int UpdatesRequiredToRemoveForSilence = 30;

		public readonly bool IsMyPeer;

		public int UpdatesSinceSilence;
	}
}
