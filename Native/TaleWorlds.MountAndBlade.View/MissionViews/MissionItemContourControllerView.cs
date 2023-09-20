using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class MissionItemContourControllerView : MissionView
	{
		private bool _isAllowedByOption
		{
			get
			{
				return !BannerlordConfig.HideBattleUI || GameNetwork.IsMultiplayer;
			}
		}

		public MissionItemContourControllerView()
		{
			this._contourItems = new List<GameEntity>();
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._isAllowedByOption)
			{
				if (Agent.Main != null && base.MissionScreen.InputManager.IsGameKeyDown(5))
				{
					this.RemoveContourFromAllItems();
					this.PopulateContourListWithNearbyItems();
					this.ApplyContourToAllItems();
					this._lastItemQueryTime = base.Mission.CurrentTime;
				}
				else
				{
					this.RemoveContourFromAllItems();
					this._contourItems.Clear();
				}
				if (this._isContourAppliedToAllItems)
				{
					float currentTime = base.Mission.CurrentTime;
					if (currentTime - this._lastItemQueryTime > this._sceneItemQueryFreq)
					{
						this.RemoveContourFromAllItems();
						this.PopulateContourListWithNearbyItems();
						this._lastItemQueryTime = currentTime;
					}
				}
			}
		}

		public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
			base.OnFocusGained(agent, focusableObject, isInteractable);
			if (this._isAllowedByOption && focusableObject != this._currentFocusedObject && isInteractable)
			{
				this._currentFocusedObject = focusableObject;
				UsableMissionObject usableMissionObject;
				if ((usableMissionObject = focusableObject as UsableMissionObject) != null)
				{
					SpawnedItemEntity spawnedItemEntity;
					if ((spawnedItemEntity = usableMissionObject as SpawnedItemEntity) != null)
					{
						this._focusedGameEntity = spawnedItemEntity.GameEntity;
					}
					else if (!string.IsNullOrEmpty(usableMissionObject.ActionMessage.ToString()) && !string.IsNullOrEmpty(usableMissionObject.DescriptionMessage.ToString()))
					{
						this._focusedGameEntity = usableMissionObject.GameEntity;
					}
					else
					{
						UsableMachine usableMachineFromPoint = this.GetUsableMachineFromPoint(usableMissionObject);
						if (usableMachineFromPoint != null)
						{
							this._focusedGameEntity = usableMachineFromPoint.GameEntity;
						}
					}
				}
				this.AddContourToFocusedItem();
			}
		}

		public override void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			base.OnFocusLost(agent, focusableObject);
			if (this._isAllowedByOption)
			{
				this.RemoveContourFromFocusedItem();
				this._currentFocusedObject = null;
				this._focusedGameEntity = null;
			}
		}

		private void PopulateContourListWithNearbyItems()
		{
			this._contourItems.Clear();
			float num = (GameNetwork.IsSessionActive ? 1f : 3f);
			float num2 = Agent.Main.MaximumForwardUnlimitedSpeed * num;
			Vec3 vec = Agent.Main.Position - new Vec3(num2, num2, 1f, -1f);
			Vec3 vec2 = Agent.Main.Position + new Vec3(num2, num2, 1.8f, -1f);
			int num3 = base.Mission.Scene.SelectEntitiesInBoxWithScriptComponent<SpawnedItemEntity>(ref vec, ref vec2, this._tempPickableEntities, this._pickableItemsId);
			for (int i = 0; i < num3; i++)
			{
				SpawnedItemEntity firstScriptOfType = this._tempPickableEntities[i].GetFirstScriptOfType<SpawnedItemEntity>();
				if (firstScriptOfType != null)
				{
					if (firstScriptOfType.IsBanner())
					{
						if (MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(firstScriptOfType, Agent.Main))
						{
							this._contourItems.Add(firstScriptOfType.GameEntity);
						}
					}
					else
					{
						this._contourItems.Add(firstScriptOfType.GameEntity);
					}
				}
			}
			int num4 = base.Mission.Scene.SelectEntitiesInBoxWithScriptComponent<UsableMachine>(ref vec, ref vec2, this._tempPickableEntities, this._pickableItemsId);
			for (int j = 0; j < num4; j++)
			{
				UsableMachine firstScriptOfType2 = this._tempPickableEntities[j].GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType2 != null && !firstScriptOfType2.IsDisabled)
				{
					GameEntity validStandingPointForAgentWithoutDistanceCheck = firstScriptOfType2.GetValidStandingPointForAgentWithoutDistanceCheck(Agent.Main);
					if (validStandingPointForAgentWithoutDistanceCheck != null && !(validStandingPointForAgentWithoutDistanceCheck.GetFirstScriptOfType<UsableMissionObject>() is SpawnedItemEntity))
					{
						IFocusable focusable;
						if ((focusable = validStandingPointForAgentWithoutDistanceCheck.GetScriptComponents().FirstOrDefault((ScriptComponentBehavior sc) => sc is IFocusable) as IFocusable) != null && focusable is UsableMissionObject)
						{
							this._contourItems.Add(firstScriptOfType2.GameEntity);
						}
					}
				}
			}
		}

		private void ApplyContourToAllItems()
		{
			if (!this._isContourAppliedToAllItems)
			{
				foreach (GameEntity gameEntity in this._contourItems)
				{
					uint nonFocusedColor = this.GetNonFocusedColor(gameEntity);
					uint num = ((gameEntity == this._focusedGameEntity) ? this._focusedContourColor : nonFocusedColor);
					gameEntity.SetContourColor(new uint?(num), true);
				}
				this._isContourAppliedToAllItems = true;
			}
		}

		private uint GetNonFocusedColor(GameEntity entity)
		{
			SpawnedItemEntity firstScriptOfType = entity.GetFirstScriptOfType<SpawnedItemEntity>();
			ItemObject itemObject = ((firstScriptOfType != null) ? firstScriptOfType.WeaponCopy.Item : null);
			WeaponComponentData weaponComponentData = ((itemObject != null) ? itemObject.PrimaryWeapon : null);
			ItemObject.ItemTypeEnum? itemTypeEnum = ((itemObject != null) ? new ItemObject.ItemTypeEnum?(itemObject.ItemType) : null);
			if (itemObject != null && itemObject.HasBannerComponent)
			{
				return this._nonFocusedBannerContourColor;
			}
			if (weaponComponentData == null || !weaponComponentData.IsAmmo)
			{
				ItemObject.ItemTypeEnum? itemTypeEnum2 = itemTypeEnum;
				ItemObject.ItemTypeEnum itemTypeEnum3 = 5;
				if (!((itemTypeEnum2.GetValueOrDefault() == itemTypeEnum3) & (itemTypeEnum2 != null)))
				{
					itemTypeEnum2 = itemTypeEnum;
					itemTypeEnum3 = 6;
					if (!((itemTypeEnum2.GetValueOrDefault() == itemTypeEnum3) & (itemTypeEnum2 != null)))
					{
						itemTypeEnum2 = itemTypeEnum;
						itemTypeEnum3 = 18;
						if (!((itemTypeEnum2.GetValueOrDefault() == itemTypeEnum3) & (itemTypeEnum2 != null)))
						{
							itemTypeEnum2 = itemTypeEnum;
							itemTypeEnum3 = 10;
							if ((itemTypeEnum2.GetValueOrDefault() == itemTypeEnum3) & (itemTypeEnum2 != null))
							{
								return this._nonFocusedThrowableContourColor;
							}
							return this._nonFocusedDefaultContourColor;
						}
					}
				}
			}
			return this._nonFocusedAmmoContourColor;
		}

		private void RemoveContourFromAllItems()
		{
			if (this._isContourAppliedToAllItems)
			{
				foreach (GameEntity gameEntity in this._contourItems)
				{
					if (this._focusedGameEntity == null || gameEntity != this._focusedGameEntity)
					{
						gameEntity.SetContourColor(null, true);
					}
				}
				this._isContourAppliedToAllItems = false;
			}
		}

		private void AddContourToFocusedItem()
		{
			if (this._focusedGameEntity != null && !this._isContourAppliedToFocusedItem)
			{
				this._focusedGameEntity.SetContourColor(new uint?(this._focusedContourColor), true);
				this._isContourAppliedToFocusedItem = true;
			}
		}

		private void RemoveContourFromFocusedItem()
		{
			if (this._focusedGameEntity != null && this._isContourAppliedToFocusedItem)
			{
				if (this._contourItems.Contains(this._focusedGameEntity))
				{
					this._focusedGameEntity.SetContourColor(new uint?(this._nonFocusedDefaultContourColor), true);
				}
				else
				{
					this._focusedGameEntity.SetContourColor(null, true);
				}
				this._isContourAppliedToFocusedItem = false;
			}
		}

		private UsableMachine GetUsableMachineFromPoint(UsableMissionObject standingPoint)
		{
			GameEntity gameEntity = standingPoint.GameEntity;
			while (gameEntity != null && !gameEntity.HasScriptOfType<UsableMachine>())
			{
				gameEntity = gameEntity.Parent;
			}
			if (gameEntity != null)
			{
				UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType != null)
				{
					return firstScriptOfType;
				}
			}
			return null;
		}

		private GameEntity[] _tempPickableEntities = new GameEntity[128];

		private UIntPtr[] _pickableItemsId = new UIntPtr[128];

		private List<GameEntity> _contourItems;

		private GameEntity _focusedGameEntity;

		private IFocusable _currentFocusedObject;

		private bool _isContourAppliedToAllItems;

		private bool _isContourAppliedToFocusedItem;

		private uint _nonFocusedDefaultContourColor = new Color(0.85f, 0.85f, 0.85f, 1f).ToUnsignedInteger();

		private uint _nonFocusedAmmoContourColor = new Color(0f, 0.73f, 1f, 1f).ToUnsignedInteger();

		private uint _nonFocusedThrowableContourColor = new Color(0.051f, 0.988f, 0.18f, 1f).ToUnsignedInteger();

		private uint _nonFocusedBannerContourColor = new Color(0.521f, 0.988f, 0.521f, 1f).ToUnsignedInteger();

		private uint _focusedContourColor = new Color(1f, 0.84f, 0.35f, 1f).ToUnsignedInteger();

		private float _lastItemQueryTime;

		private float _sceneItemQueryFreq = 1f;
	}
}
