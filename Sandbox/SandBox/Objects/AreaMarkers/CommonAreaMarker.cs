using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace SandBox.Objects.AreaMarkers
{
	public class CommonAreaMarker : AreaMarker
	{
		public List<MatrixFrame> HiddenSpawnFrames { get; private set; }

		public override string Tag
		{
			get
			{
				Alley alley = this.GetAlley();
				if (alley == null)
				{
					return null;
				}
				return alley.Tag;
			}
		}

		protected override void OnInit()
		{
			this.HiddenSpawnFrames = new List<MatrixFrame>();
		}

		public override List<UsableMachine> GetUsableMachinesInRange(string excludeTag = null)
		{
			List<UsableMachine> usableMachinesInRange = base.GetUsableMachinesInRange(null);
			for (int i = usableMachinesInRange.Count - 1; i >= 0; i--)
			{
				UsableMachine usableMachine = usableMachinesInRange[i];
				string[] tags = usableMachine.GameEntity.Tags;
				if (usableMachine.GameEntity.HasScriptOfType<Passage>() || (!tags.Contains("npc_common") && !tags.Contains("npc_common_limited") && !tags.Contains("sp_guard") && !tags.Contains("sp_guard_unarmed") && !tags.Contains("sp_notable")))
				{
					usableMachinesInRange.RemoveAt(i);
				}
			}
			List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag("sp_common_hidden").ToList<GameEntity>();
			GameEntity gameEntity = null;
			float num = float.MaxValue;
			float num2 = this.AreaRadius * this.AreaRadius;
			for (int j = list.Count - 1; j >= 0; j--)
			{
				float num3 = list[j].GlobalPosition.DistanceSquared(base.GameEntity.GlobalPosition);
				if (num3 < num)
				{
					gameEntity = list[j];
					num = num3;
				}
				if (num3 < num2)
				{
					this.HiddenSpawnFrames.Add(list[j].GetGlobalFrame());
				}
			}
			if (Extensions.IsEmpty<MatrixFrame>(this.HiddenSpawnFrames) && gameEntity != null)
			{
				this.HiddenSpawnFrames.Add(gameEntity.GetGlobalFrame());
			}
			return usableMachinesInRange;
		}

		public Alley GetAlley()
		{
			Alley alley = null;
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			if (settlement != null && ((settlement != null) ? settlement.Alleys : null) != null && this.AreaIndex > 0 && this.AreaIndex <= settlement.Alleys.Count)
			{
				alley = settlement.Alleys[this.AreaIndex - 1];
			}
			return alley;
		}

		public override TextObject GetName()
		{
			Alley alley = this.GetAlley();
			if (alley == null)
			{
				return null;
			}
			return alley.Name;
		}

		public string Type = "";
	}
}
