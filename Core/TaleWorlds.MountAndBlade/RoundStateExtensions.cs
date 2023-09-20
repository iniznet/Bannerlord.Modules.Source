using System;

namespace TaleWorlds.MountAndBlade
{
	public static class RoundStateExtensions
	{
		public static bool StateHasVisualTimer(this MultiplayerRoundState roundState)
		{
			return roundState - MultiplayerRoundState.Preparation <= 1;
		}
	}
}
