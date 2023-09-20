using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace SandBox.Objects.AreaMarkers
{
	// Token: 0x0200002F RID: 47
	public class WorkshopAreaMarker : AreaMarker
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000E2F2 File Offset: 0x0000C4F2
		public override string Tag
		{
			get
			{
				Workshop workshop = this.GetWorkshop();
				if (workshop == null)
				{
					return null;
				}
				return workshop.Tag;
			}
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000E308 File Offset: 0x0000C508
		public Workshop GetWorkshop()
		{
			Workshop workshop = null;
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			if (settlement != null && settlement.IsTown && settlement.Town.Workshops.Length > this.AreaIndex && this.AreaIndex > 0)
			{
				workshop = settlement.Town.Workshops[this.AreaIndex];
			}
			return workshop;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000E360 File Offset: 0x0000C560
		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (MBEditor.HelpersEnabled() && this.CheckToggle)
			{
				float distanceSquared = this.AreaRadius * this.AreaRadius;
				List<GameEntity> list = new List<GameEntity>();
				base.Scene.GetEntities(ref list);
				foreach (GameEntity gameEntity in list)
				{
					gameEntity.HasTag("shop_prop");
				}
				foreach (UsableMachine usableMachine in (from x in MBExtensions.FindAllWithType<UsableMachine>(list)
					where x.GameEntity.GlobalPosition.DistanceSquared(this.GameEntity.GlobalPosition) <= distanceSquared
					select x).ToList<UsableMachine>())
				{
				}
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000E454 File Offset: 0x0000C654
		public WorkshopType GetWorkshopType()
		{
			Workshop workshop = this.GetWorkshop();
			if (workshop == null)
			{
				return null;
			}
			return workshop.WorkshopType;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000E467 File Offset: 0x0000C667
		public override TextObject GetName()
		{
			Workshop workshop = this.GetWorkshop();
			if (workshop == null)
			{
				return null;
			}
			return workshop.Name;
		}
	}
}
