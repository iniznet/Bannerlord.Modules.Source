using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200033A RID: 826
	public class FleePosition : ScriptComponentBehavior
	{
		// Token: 0x06002C58 RID: 11352 RVA: 0x000ABEE8 File Offset: 0x000AA0E8
		protected internal override void OnInit()
		{
			this.CollectNodes();
			bool flag = false;
			if (this.Side == "both")
			{
				this._side = BattleSideEnum.None;
			}
			else if (this.Side == "attacker")
			{
				this._side = (flag ? BattleSideEnum.Defender : BattleSideEnum.Attacker);
			}
			else if (this.Side == "defender")
			{
				this._side = (flag ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			}
			else
			{
				this._side = BattleSideEnum.None;
			}
			if (base.GameEntity.HasTag("sally_out"))
			{
				if (Mission.Current.IsSallyOutBattle)
				{
					Mission.Current.AddFleePosition(this);
					return;
				}
			}
			else
			{
				Mission.Current.AddFleePosition(this);
			}
		}

		// Token: 0x06002C59 RID: 11353 RVA: 0x000ABF96 File Offset: 0x000AA196
		public BattleSideEnum GetSide()
		{
			return this._side;
		}

		// Token: 0x06002C5A RID: 11354 RVA: 0x000ABF9E File Offset: 0x000AA19E
		protected internal override void OnEditorInit()
		{
			this.CollectNodes();
		}

		// Token: 0x06002C5B RID: 11355 RVA: 0x000ABFA8 File Offset: 0x000AA1A8
		private void CollectNodes()
		{
			this._nodes.Clear();
			int childCount = base.GameEntity.ChildCount;
			for (int i = 0; i < childCount; i++)
			{
				GameEntity child = base.GameEntity.GetChild(i);
				this._nodes.Add(child.GlobalPosition);
			}
			if (this._nodes.IsEmpty<Vec3>())
			{
				this._nodes.Add(base.GameEntity.GlobalPosition);
			}
		}

		// Token: 0x06002C5C RID: 11356 RVA: 0x000AC01C File Offset: 0x000AA21C
		protected internal override void OnEditorTick(float dt)
		{
			this.CollectNodes();
			bool flag = base.GameEntity.IsSelectedOnEditor();
			int childCount = base.GameEntity.ChildCount;
			int num = 0;
			while (!flag && num < childCount)
			{
				flag = base.GameEntity.GetChild(num).IsSelectedOnEditor();
				num++;
			}
			if (flag)
			{
				for (int i = 0; i < this._nodes.Count; i++)
				{
					int num2 = this._nodes.Count - 1;
				}
			}
		}

		// Token: 0x06002C5D RID: 11357 RVA: 0x000AC094 File Offset: 0x000AA294
		public Vec3 GetClosestPointToEscape(Vec2 position)
		{
			if (this._nodes.Count == 1)
			{
				return this._nodes[0];
			}
			float num = float.MaxValue;
			Vec3 vec = this._nodes[0];
			for (int i = 0; i < this._nodes.Count - 1; i++)
			{
				Vec3 vec2 = this._nodes[i];
				Vec3 vec3 = this._nodes[i + 1];
				float num2 = vec2.DistanceSquared(vec3);
				if (num2 > 0f)
				{
					float num3 = MathF.Max(0f, MathF.Min(1f, Vec2.DotProduct(position - vec2.AsVec2, vec3.AsVec2 - vec2.AsVec2) / num2));
					Vec3 vec4 = vec2 + num3 * (vec3 - vec2);
					float num4 = vec4.AsVec2.DistanceSquared(position);
					if (num > num4)
					{
						num = num4;
						vec = vec4;
					}
				}
				else
				{
					num = 0f;
					vec = vec2;
				}
			}
			return vec;
		}

		// Token: 0x06002C5E RID: 11358 RVA: 0x000AC19C File Offset: 0x000AA39C
		protected internal override bool MovesEntity()
		{
			return true;
		}

		// Token: 0x040010E0 RID: 4320
		private List<Vec3> _nodes = new List<Vec3>();

		// Token: 0x040010E1 RID: 4321
		private BattleSideEnum _side;

		// Token: 0x040010E2 RID: 4322
		public string Side = "both";
	}
}
