using System;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x0200003F RID: 63
	public class MPVoicePlayerVM : MPPlayerVM
	{
		// Token: 0x06000570 RID: 1392 RVA: 0x0001757B File Offset: 0x0001577B
		public MPVoicePlayerVM(MissionPeer peer)
			: base(peer)
		{
			this.UpdatesSinceSilence = 0;
			this.IsMyPeer = peer.IsMine;
		}

		// Token: 0x040002C5 RID: 709
		public const int UpdatesRequiredToRemoveForSilence = 30;

		// Token: 0x040002C6 RID: 710
		public readonly bool IsMyPeer;

		// Token: 0x040002C7 RID: 711
		public int UpdatesSinceSilence;
	}
}
