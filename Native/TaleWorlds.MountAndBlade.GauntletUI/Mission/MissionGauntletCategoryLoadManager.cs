using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x02000026 RID: 38
	[DefaultView]
	public class MissionGauntletCategoryLoadManager : MissionView, IMissionListener
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000929B File Offset: 0x0000749B
		private ITwoDimensionResourceContext _resourceContext
		{
			get
			{
				return UIResourceManager.ResourceContext;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x000092A2 File Offset: 0x000074A2
		private ResourceDepot _resourceDepot
		{
			get
			{
				return UIResourceManager.UIResourceDepot;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x000092A9 File Offset: 0x000074A9
		private SpriteData _spriteData
		{
			get
			{
				return UIResourceManager.SpriteData;
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x000092B0 File Offset: 0x000074B0
		public override void AfterStart()
		{
			base.AfterStart();
			if (this._fullBackgroundCategory == null)
			{
				this._fullBackgroundCategory = this._spriteData.SpriteCategories["ui_fullbackgrounds"];
			}
			if (this._backgroundCategory == null)
			{
				this._backgroundCategory = this._spriteData.SpriteCategories["ui_backgrounds"];
			}
			if (this._fullscreensCategory == null)
			{
				this._fullscreensCategory = this._spriteData.SpriteCategories["ui_fullscreens"];
			}
			if (this._encyclopediaCategory == null)
			{
				this._encyclopediaCategory = this._spriteData.SpriteCategories["ui_encyclopedia"];
			}
			if (this._mapBarCategory == null && this._spriteData.SpriteCategories.ContainsKey("ui_mapbar") && this._spriteData.SpriteCategories["ui_mapbar"].IsLoaded)
			{
				this._mapBarCategory = this._spriteData.SpriteCategories["ui_mapbar"];
			}
			if (this._optionsView == null)
			{
				this._optionsView = base.Mission.GetMissionBehavior<MissionGauntletOptionsUIHandler>();
				base.Mission.AddListener(this);
			}
			this.HandleCategoryLoadingUnloading();
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x000093D0 File Offset: 0x000075D0
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._optionsView = null;
			base.Mission.RemoveListener(this);
			this.LoadUnloadAllCategories(true);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x000093F2 File Offset: 0x000075F2
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.HandleCategoryLoadingUnloading();
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00009404 File Offset: 0x00007604
		private void HandleCategoryLoadingUnloading()
		{
			bool flag = true;
			if (base.Mission != null)
			{
				flag = this.IsBackgroundsUsedInMission(base.Mission);
			}
			this.LoadUnloadAllCategories(flag);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00009430 File Offset: 0x00007630
		private void LoadUnloadAllCategories(bool load)
		{
			if (load)
			{
				if (!this._fullBackgroundCategory.IsLoaded)
				{
					this._fullBackgroundCategory.Load(this._resourceContext, this._resourceDepot);
				}
				if (!this._backgroundCategory.IsLoaded)
				{
					this._backgroundCategory.Load(this._resourceContext, this._resourceDepot);
				}
				if (!this._fullscreensCategory.IsLoaded)
				{
					this._fullscreensCategory.Load(this._resourceContext, this._resourceDepot);
				}
				if (!this._encyclopediaCategory.IsLoaded)
				{
					this._encyclopediaCategory.Load(this._resourceContext, this._resourceDepot);
				}
				SpriteCategory mapBarCategory = this._mapBarCategory;
				if (mapBarCategory != null && !mapBarCategory.IsLoaded)
				{
					this._mapBarCategory.Load(this._resourceContext, this._resourceDepot);
					return;
				}
			}
			else
			{
				if (this._fullBackgroundCategory.IsLoaded)
				{
					this._fullBackgroundCategory.Unload();
				}
				if (this._backgroundCategory.IsLoaded)
				{
					this._backgroundCategory.Unload();
				}
				if (this._fullscreensCategory.IsLoaded && !this._optionsView.IsEnabled)
				{
					this._fullscreensCategory.Unload();
				}
				if (this._encyclopediaCategory.IsLoaded)
				{
					Mission mission = base.Mission;
					if (mission == null || mission.Mode != 1)
					{
						this._encyclopediaCategory.Unload();
					}
				}
				SpriteCategory mapBarCategory2 = this._mapBarCategory;
				if (mapBarCategory2 != null && mapBarCategory2.IsLoaded)
				{
					this._mapBarCategory.Unload();
				}
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x000095AB File Offset: 0x000077AB
		private bool IsBackgroundsUsedInMission(Mission mission)
		{
			return mission.IsInventoryAccessAllowed || mission.IsCharacterWindowAccessAllowed || mission.IsClanWindowAccessAllowed || mission.IsKingdomWindowAccessAllowed || mission.IsQuestScreenAccessAllowed || mission.IsPartyWindowAccessAllowed || mission.IsEncyclopediaWindowAccessAllowed;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x000095E5 File Offset: 0x000077E5
		void IMissionListener.OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
		{
		}

		// Token: 0x060001BE RID: 446 RVA: 0x000095E7 File Offset: 0x000077E7
		void IMissionListener.OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
		{
		}

		// Token: 0x060001BF RID: 447 RVA: 0x000095E9 File Offset: 0x000077E9
		void IMissionListener.OnEndMission()
		{
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000095EB File Offset: 0x000077EB
		void IMissionListener.OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			this.HandleCategoryLoadingUnloading();
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x000095F3 File Offset: 0x000077F3
		void IMissionListener.OnConversationCharacterChanged()
		{
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x000095F5 File Offset: 0x000077F5
		void IMissionListener.OnResetMission()
		{
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x000095F7 File Offset: 0x000077F7
		void IMissionListener.OnInitialDeploymentPlanMade(BattleSideEnum battleSide, bool isFirstPlan)
		{
		}

		// Token: 0x040000CC RID: 204
		private SpriteCategory _fullBackgroundCategory;

		// Token: 0x040000CD RID: 205
		private SpriteCategory _backgroundCategory;

		// Token: 0x040000CE RID: 206
		private SpriteCategory _fullscreensCategory;

		// Token: 0x040000CF RID: 207
		private SpriteCategory _mapBarCategory;

		// Token: 0x040000D0 RID: 208
		private SpriteCategory _encyclopediaCategory;

		// Token: 0x040000D1 RID: 209
		private MissionGauntletOptionsUIHandler _optionsView;
	}
}
