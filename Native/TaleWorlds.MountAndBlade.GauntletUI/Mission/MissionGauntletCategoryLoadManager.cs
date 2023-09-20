using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	[DefaultView]
	public class MissionGauntletCategoryLoadManager : MissionView, IMissionListener
	{
		private ITwoDimensionResourceContext _resourceContext
		{
			get
			{
				return UIResourceManager.ResourceContext;
			}
		}

		private ResourceDepot _resourceDepot
		{
			get
			{
				return UIResourceManager.UIResourceDepot;
			}
		}

		private SpriteData _spriteData
		{
			get
			{
				return UIResourceManager.SpriteData;
			}
		}

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

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._optionsView = null;
			base.Mission.RemoveListener(this);
			this.LoadUnloadAllCategories(true);
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.HandleCategoryLoadingUnloading();
		}

		private void HandleCategoryLoadingUnloading()
		{
			bool flag = true;
			if (base.Mission != null)
			{
				flag = this.IsBackgroundsUsedInMission(base.Mission);
			}
			this.LoadUnloadAllCategories(flag);
		}

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

		private bool IsBackgroundsUsedInMission(Mission mission)
		{
			return mission.IsInventoryAccessAllowed || mission.IsCharacterWindowAccessAllowed || mission.IsClanWindowAccessAllowed || mission.IsKingdomWindowAccessAllowed || mission.IsQuestScreenAccessAllowed || mission.IsPartyWindowAccessAllowed || mission.IsEncyclopediaWindowAccessAllowed;
		}

		void IMissionListener.OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
		{
		}

		void IMissionListener.OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
		{
		}

		void IMissionListener.OnEndMission()
		{
		}

		void IMissionListener.OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			this.HandleCategoryLoadingUnloading();
		}

		void IMissionListener.OnConversationCharacterChanged()
		{
		}

		void IMissionListener.OnResetMission()
		{
		}

		void IMissionListener.OnInitialDeploymentPlanMade(BattleSideEnum battleSide, bool isFirstPlan)
		{
		}

		private SpriteCategory _fullBackgroundCategory;

		private SpriteCategory _backgroundCategory;

		private SpriteCategory _fullscreensCategory;

		private SpriteCategory _mapBarCategory;

		private SpriteCategory _encyclopediaCategory;

		private MissionGauntletOptionsUIHandler _optionsView;
	}
}
