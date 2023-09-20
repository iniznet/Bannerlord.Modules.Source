using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200031D RID: 797
	public static class RoundStateExtensions
	{
		// Token: 0x06002B03 RID: 11011 RVA: 0x000A8AF8 File Offset: 0x000A6CF8
		public static bool StateHasVisualTimer(this MultiplayerRoundState roundState)
		{
			return roundState - MultiplayerRoundState.Preparation <= 1;
		}
	}
}
