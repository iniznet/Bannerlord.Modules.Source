using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Objects.AreaMarkers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x02000075 RID: 117
	public class NotableSpawnPointHandler : MissionLogic
	{
		// Token: 0x06000514 RID: 1300 RVA: 0x00024928 File Offset: 0x00022B28
		public override void OnBehaviorInitialize()
		{
			List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag("sp_notables_parent").ToList<GameEntity>();
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			this._workshopAssignedHeroes = new List<Hero>();
			foreach (Hero hero in settlement.Notables)
			{
				if (hero.IsGangLeader)
				{
					this._gangLeaderNotableCount++;
				}
				else if (hero.IsPreacher)
				{
					this._preacherNotableCount++;
				}
				else if (hero.IsArtisan)
				{
					this._artisanNotableCount++;
				}
				else if (hero.IsRuralNotable || hero.IsHeadman)
				{
					this._ruralNotableCount++;
				}
				else if (hero.IsMerchant)
				{
					this._merchantNotableCount++;
				}
			}
			foreach (GameEntity gameEntity in list.ToList<GameEntity>())
			{
				foreach (GameEntity gameEntity2 in gameEntity.GetChildren())
				{
					this.FindAndSetChild(gameEntity2);
				}
				foreach (WorkshopAreaMarker workshopAreaMarker in (from x in MBExtensions.FindAllWithType<WorkshopAreaMarker>(base.Mission.ActiveMissionObjects).ToList<WorkshopAreaMarker>()
					orderby x.AreaIndex
					select x).ToList<WorkshopAreaMarker>())
				{
					if (workshopAreaMarker.IsPositionInRange(gameEntity.GlobalPosition))
					{
						if (workshopAreaMarker.GetWorkshop().Owner.OwnedWorkshops.First((Workshop x) => !x.WorkshopType.IsHidden).Tag == workshopAreaMarker.Tag)
						{
							this.ActivateParentSetInsideWorkshop(workshopAreaMarker);
							list.Remove(gameEntity);
							break;
						}
					}
				}
			}
			foreach (GameEntity gameEntity3 in list)
			{
				foreach (GameEntity gameEntity4 in gameEntity3.GetChildren())
				{
					this.FindAndSetChild(gameEntity4);
				}
				this.ActivateParentSetOutsideWorkshop();
			}
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x00024C54 File Offset: 0x00022E54
		private void FindAndSetChild(GameEntity childGameEntity)
		{
			if (childGameEntity.HasTag("merchant_notary_talking_set"))
			{
				this._currentMerchantSetGameEntity = childGameEntity;
				return;
			}
			if (childGameEntity.HasTag("preacher_notary_talking_set"))
			{
				this._currentPreacherSetGameEntity = childGameEntity;
				return;
			}
			if (childGameEntity.HasTag("gangleader_sitting_and_talking_with_guards_set"))
			{
				this._currentGangLeaderSetGameEntity = childGameEntity;
				return;
			}
			if (childGameEntity.HasTag("sp_artisan_notary_talking_set"))
			{
				this._currentArtisanSetGameEntity = childGameEntity;
				return;
			}
			if (childGameEntity.HasTag("sp_ruralnotable_notary_talking_set"))
			{
				this._currentRuralNotableSetGameEntity = childGameEntity;
			}
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x00024CCC File Offset: 0x00022ECC
		private void ActivateParentSetInsideWorkshop(WorkshopAreaMarker areaMarker)
		{
			Hero owner = areaMarker.GetWorkshop().Owner;
			if (!this._workshopAssignedHeroes.Contains(owner))
			{
				this._workshopAssignedHeroes.Add(owner);
				if (owner.IsMerchant)
				{
					this.DeactivateAllExcept(this._currentMerchantSetGameEntity);
					this._merchantNotableCount--;
					return;
				}
				if (owner.IsArtisan)
				{
					this.DeactivateAllExcept(this._currentArtisanSetGameEntity);
					this._artisanNotableCount--;
					return;
				}
				if (owner.IsGangLeader)
				{
					this.DeactivateAllExcept(this._currentGangLeaderSetGameEntity);
					this._gangLeaderNotableCount--;
					return;
				}
				if (owner.IsPreacher)
				{
					this.DeactivateAllExcept(this._currentPreacherSetGameEntity);
					this._preacherNotableCount--;
					return;
				}
				if (owner.IsRuralNotable)
				{
					this.DeactivateAllExcept(this._currentRuralNotableSetGameEntity);
					this._ruralNotableCount--;
					return;
				}
			}
			else
			{
				this.DeactivateAll();
			}
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x00024DB8 File Offset: 0x00022FB8
		private void ActivateParentSetOutsideWorkshop()
		{
			if (this._gangLeaderNotableCount > 0)
			{
				this.DeactivateAllExcept(this._currentGangLeaderSetGameEntity);
				this._gangLeaderNotableCount--;
				return;
			}
			if (this._merchantNotableCount > 0)
			{
				this.DeactivateAllExcept(this._currentMerchantSetGameEntity);
				this._merchantNotableCount--;
				return;
			}
			if (this._preacherNotableCount > 0)
			{
				this.DeactivateAllExcept(this._currentPreacherSetGameEntity);
				this._preacherNotableCount--;
				return;
			}
			if (this._artisanNotableCount > 0)
			{
				this.DeactivateAllExcept(this._currentArtisanSetGameEntity);
				this._artisanNotableCount--;
				return;
			}
			if (this._ruralNotableCount > 0)
			{
				this.DeactivateAllExcept(this._currentRuralNotableSetGameEntity);
				this._ruralNotableCount--;
				return;
			}
			this.DeactivateAll();
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x00024E7F File Offset: 0x0002307F
		private void DeactivateAll()
		{
			this.MakeInvisibleAndDeactivate(this._currentGangLeaderSetGameEntity);
			this.MakeInvisibleAndDeactivate(this._currentMerchantSetGameEntity);
			this.MakeInvisibleAndDeactivate(this._currentPreacherSetGameEntity);
			this.MakeInvisibleAndDeactivate(this._currentArtisanSetGameEntity);
			this.MakeInvisibleAndDeactivate(this._currentRuralNotableSetGameEntity);
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00024EC0 File Offset: 0x000230C0
		private void DeactivateAllExcept(GameEntity gameEntity)
		{
			if (gameEntity != this._currentMerchantSetGameEntity)
			{
				this.MakeInvisibleAndDeactivate(this._currentMerchantSetGameEntity);
			}
			if (gameEntity != this._currentGangLeaderSetGameEntity)
			{
				this.MakeInvisibleAndDeactivate(this._currentGangLeaderSetGameEntity);
			}
			if (gameEntity != this._currentPreacherSetGameEntity)
			{
				this.MakeInvisibleAndDeactivate(this._currentPreacherSetGameEntity);
			}
			if (gameEntity != this._currentArtisanSetGameEntity)
			{
				this.MakeInvisibleAndDeactivate(this._currentArtisanSetGameEntity);
			}
			if (gameEntity != this._currentRuralNotableSetGameEntity)
			{
				this.MakeInvisibleAndDeactivate(this._currentRuralNotableSetGameEntity);
			}
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00024F50 File Offset: 0x00023150
		private void MakeInvisibleAndDeactivate(GameEntity gameEntity)
		{
			gameEntity.SetVisibilityExcludeParents(false);
			UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
			if (firstScriptOfType != null)
			{
				firstScriptOfType.Deactivate();
			}
			foreach (GameEntity gameEntity2 in gameEntity.GetChildren())
			{
				this.MakeInvisibleAndDeactivate(gameEntity2);
			}
		}

		// Token: 0x0400027C RID: 636
		private int _merchantNotableCount;

		// Token: 0x0400027D RID: 637
		private int _gangLeaderNotableCount;

		// Token: 0x0400027E RID: 638
		private int _preacherNotableCount;

		// Token: 0x0400027F RID: 639
		private int _artisanNotableCount;

		// Token: 0x04000280 RID: 640
		private int _ruralNotableCount;

		// Token: 0x04000281 RID: 641
		private GameEntity _currentMerchantSetGameEntity;

		// Token: 0x04000282 RID: 642
		private GameEntity _currentPreacherSetGameEntity;

		// Token: 0x04000283 RID: 643
		private GameEntity _currentGangLeaderSetGameEntity;

		// Token: 0x04000284 RID: 644
		private GameEntity _currentArtisanSetGameEntity;

		// Token: 0x04000285 RID: 645
		private GameEntity _currentRuralNotableSetGameEntity;

		// Token: 0x04000286 RID: 646
		private List<Hero> _workshopAssignedHeroes;
	}
}
