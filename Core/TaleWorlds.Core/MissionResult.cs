﻿using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public class MissionResult
	{
		internal static void AutoGeneratedStaticCollectObjectsMissionResult(object o, List<object> collectedObjects)
		{
			((MissionResult)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		public BattleState BattleState { get; private set; }

		public bool BattleResolved
		{
			get
			{
				return this.PlayerVictory || this.PlayerDefeated;
			}
		}

		public bool PlayerVictory { get; private set; }

		public bool PlayerDefeated { get; private set; }

		public bool EnemyRetreated { get; private set; }

		public MissionResult(BattleState battleState, bool playerVictory, bool playerDefeated, bool enemyRetreated)
		{
			this.BattleState = battleState;
			this.PlayerVictory = playerVictory;
			this.PlayerDefeated = playerDefeated;
			this.EnemyRetreated = enemyRetreated;
		}

		public MissionResult()
		{
			this.PlayerVictory = false;
			this.PlayerDefeated = false;
			this.EnemyRetreated = false;
		}

		public static MissionResult CreateSuccessful(IMission mission, bool enemyRetreated = false)
		{
			return new MissionResult((mission.PlayerTeam.Side == BattleSideEnum.Attacker) ? BattleState.AttackerVictory : BattleState.DefenderVictory, true, false, enemyRetreated);
		}

		public static MissionResult CreateDefeated(IMission mission)
		{
			return new MissionResult((mission.PlayerTeam.Side == BattleSideEnum.Attacker) ? BattleState.DefenderVictory : BattleState.AttackerVictory, false, true, false);
		}

		public static MissionResult CreateDefenderPushedBack()
		{
			return new MissionResult(BattleState.DefenderPullBack, false, false, false);
		}
	}
}
