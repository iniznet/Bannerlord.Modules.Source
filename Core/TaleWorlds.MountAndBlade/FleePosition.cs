using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class FleePosition : ScriptComponentBehavior
	{
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

		public BattleSideEnum GetSide()
		{
			return this._side;
		}

		protected internal override void OnEditorInit()
		{
			this.CollectNodes();
		}

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

		protected internal override bool MovesEntity()
		{
			return true;
		}

		private List<Vec3> _nodes = new List<Vec3>();

		private BattleSideEnum _side;

		public string Side = "both";
	}
}
