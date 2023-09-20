using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public interface IRoundComponent : IMissionBehavior
	{
		event Action OnRoundStarted;

		event Action OnPreparationEnded;

		event Action OnPreRoundEnding;

		event Action OnRoundEnding;

		event Action OnPostRoundEnded;

		event Action OnCurrentRoundStateChanged;

		float LastRoundEndRemainingTime { get; }

		float RemainingRoundTime { get; }

		MultiplayerRoundState CurrentRoundState { get; }

		int RoundCount { get; }

		BattleSideEnum RoundWinner { get; }

		RoundEndReason RoundEndReason { get; }
	}
}
