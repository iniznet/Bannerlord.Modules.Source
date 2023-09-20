using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	public interface IPrimarySiegeWeapon
	{
		bool HasCompletedAction();

		float SiegeWeaponPriority { get; }

		int OverTheWallNavMeshID { get; }

		bool HoldLadders { get; }

		bool SendLadders { get; }

		MissionObject TargetCastlePosition { get; }

		FormationAI.BehaviorSide WeaponSide { get; }

		bool GetNavmeshFaceIds(out List<int> navmeshFaceIds);
	}
}
