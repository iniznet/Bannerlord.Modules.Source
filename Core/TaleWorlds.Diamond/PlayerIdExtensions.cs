using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200001A RID: 26
	public static class PlayerIdExtensions
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00002B01 File Offset: 0x00000D01
		public static PeerId ConvertToPeerId(this PlayerId playerId)
		{
			return new PeerId(playerId.ToByteArray());
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00002B0F File Offset: 0x00000D0F
		public static PlayerId ConvertToPlayerId(this PeerId peerId)
		{
			return new PlayerId(peerId.ToByteArray());
		}
	}
}
