using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	public interface ICommanderInfo : IMissionBehavior
	{
		event Action<BattleSideEnum, float> OnMoraleChangedEvent;

		event Action OnFlagNumberChangedEvent;

		event Action<FlagCapturePoint, Team> OnCapturePointOwnerChangedEvent;

		IEnumerable<FlagCapturePoint> AllCapturePoints { get; }

		Team GetFlagOwner(FlagCapturePoint flag);

		bool AreMoralesIndependent { get; }
	}
}
