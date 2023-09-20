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
	// Token: 0x0200001F RID: 31
	public class ArcheryTournamentAgentController : AgentController
	{
		// Token: 0x06000175 RID: 373 RVA: 0x0000AF36 File Offset: 0x00009136
		public override void OnInitialize()
		{
			this._missionController = Mission.Current.GetMissionBehavior<TournamentArcheryMissionController>();
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000AF48 File Offset: 0x00009148
		public void OnTick()
		{
			if (!base.Owner.IsAIControlled)
			{
				return;
			}
			this.UpdateTarget();
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000AF5E File Offset: 0x0000915E
		public void SetTargets(List<DestructableComponent> targetList)
		{
			this._targetList = targetList;
			this._target = null;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000AF6E File Offset: 0x0000916E
		private void UpdateTarget()
		{
			if (this._target == null || this._target.IsDestroyed)
			{
				this.SelectNewTarget();
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000AF8C File Offset: 0x0000918C
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

		// Token: 0x0600017A RID: 378 RVA: 0x0000B0B8 File Offset: 0x000092B8
		private float GetScore(DestructableComponent target)
		{
			if (!target.IsDestroyed)
			{
				return 1f / base.Owner.Position.DistanceSquared(target.GameEntity.GlobalPosition);
			}
			return 0f;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000B0F7 File Offset: 0x000092F7
		public void OnTargetHit(Agent agent, DestructableComponent target)
		{
			if (agent == base.Owner || target == this._target)
			{
				this.SelectNewTarget();
			}
		}

		// Token: 0x0400009C RID: 156
		private List<DestructableComponent> _targetList;

		// Token: 0x0400009D RID: 157
		private DestructableComponent _target;

		// Token: 0x0400009E RID: 158
		private TournamentArcheryMissionController _missionController;
	}
}
