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
	public class WorkshopAreaMarker : AreaMarker
	{
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

		public WorkshopType GetWorkshopType()
		{
			Workshop workshop = this.GetWorkshop();
			if (workshop == null)
			{
				return null;
			}
			return workshop.WorkshopType;
		}

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
