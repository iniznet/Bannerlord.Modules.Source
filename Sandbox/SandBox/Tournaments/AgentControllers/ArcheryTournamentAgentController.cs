using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.AgentControllers
{
	public class ArcheryTournamentAgentController : AgentController
	{
		public override void OnInitialize()
		{
			this._missionController = Mission.Current.GetMissionBehavior<TournamentArcheryMissionController>();
		}

		public void OnTick()
		{
			if (!base.Owner.IsAIControlled)
			{
				return;
			}
			this.UpdateTarget();
		}

		public void SetTargets(List<DestructableComponent> targetList)
		{
			this._targetList = targetList;
			this._target = null;
		}

		private void UpdateTarget()
		{
			if (this._target == null || this._target.IsDestroyed)
			{
				this.SelectNewTarget();
			}
		}

		private void SelectNewTarget()
		{
			List<KeyValuePair<float, DestructableComponent>> list = new List<KeyValuePair<float, DestructableComponent>>();
			foreach (DestructableComponent destructableComponent in this._targetList)
			{
				float score = this.GetScore(destructableComponent);
				if (score > 0f)
				{
					list.Add(new KeyValuePair<float, DestructableComponent>(score, destructableComponent));
				}
			}
			if (list.Count == 0)
			{
				this._target = null;
				base.Owner.DisableScriptedCombatMovement();
				WorldPosition worldPosition = base.Owner.GetWorldPosition();
				base.Owner.SetScriptedPosition(ref worldPosition, false, 0);
			}
			else
			{
				List<KeyValuePair<float, DestructableComponent>> list2 = list.OrderByDescending((KeyValuePair<float, DestructableComponent> x) => x.Key).ToList<KeyValuePair<float, DestructableComponent>>();
				int num = MathF.Min(list2.Count, 5);
				this._target = list2[MBRandom.RandomInt(num)].Value;
			}
			if (this._target != null)
			{
				base.Owner.SetScriptedTargetEntityAndPosition(this._target.GameEntity, base.Owner.GetWorldPosition(), 0, false);
			}
		}

		private float GetScore(DestructableComponent target)
		{
			if (!target.IsDestroyed)
			{
				return 1f / base.Owner.Position.DistanceSquared(target.GameEntity.GlobalPosition);
			}
			return 0f;
		}

		public void OnTargetHit(Agent agent, DestructableComponent target)
		{
			if (agent == base.Owner || target == this._target)
			{
				this.SelectNewTarget();
			}
		}

		private List<DestructableComponent> _targetList;

		private DestructableComponent _target;

		private TournamentArcheryMissionController _missionController;
	}
}
