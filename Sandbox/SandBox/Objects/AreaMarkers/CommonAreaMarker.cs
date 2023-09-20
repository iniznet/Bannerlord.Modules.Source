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
	// Token: 0x0200002E RID: 46
	public class CommonAreaMarker : AreaMarker
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600020F RID: 527 RVA: 0x0000E0DD File Offset: 0x0000C2DD
		// (set) Token: 0x06000210 RID: 528 RVA: 0x0000E0E5 File Offset: 0x0000C2E5
		public List<MatrixFrame> HiddenSpawnFrames { get; private set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000211 RID: 529 RVA: 0x0000E0EE File Offset: 0x0000C2EE
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

		// Token: 0x06000212 RID: 530 RVA: 0x0000E101 File Offset: 0x0000C301
		protected override void OnInit()
		{
			this.HiddenSpawnFrames = new List<MatrixFrame>();
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000E110 File Offset: 0x0000C310
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

		// Token: 0x06000214 RID: 532 RVA: 0x0000E270 File Offset: 0x0000C470
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

		// Token: 0x06000215 RID: 533 RVA: 0x0000E2CC File Offset: 0x0000C4CC
		public override TextObject GetName()
		{
			Alley alley = this.GetAlley();
			if (alley == null)
			{
				return null;
			}
			return alley.Name;
		}

		// Token: 0x040000F1 RID: 241
		public string Type = "";
	}
}
