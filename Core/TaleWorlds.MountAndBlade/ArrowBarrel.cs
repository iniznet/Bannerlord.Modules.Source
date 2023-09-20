using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class ArrowBarrel : UsableMachine
	{
		private static int _pickupArrowSoundFromBarrelCache
		{
			get
			{
				if (ArrowBarrel._pickupArrowSoundFromBarrel == -1)
				{
					ArrowBarrel._pickupArrowSoundFromBarrel = SoundEvent.GetEventIdFromString("event:/mission/combat/pickup_arrows");
				}
				return ArrowBarrel._pickupArrowSoundFromBarrel;
			}
		}

		protected ArrowBarrel()
		{
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				(standingPoint as StandingPointWithWeaponRequirement).InitRequiredWeaponClasses(WeaponClass.Arrow, WeaponClass.Bolt);
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
			this.MakeVisibilityCheck = false;
			this._isVisible = base.GameEntity.IsVisibleIncludeParents();
		}

		public override void AfterMissionStart()
		{
			if (base.StandingPoints != null)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					standingPoint.LockUserFrames = true;
				}
			}
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=bNYm3K6b}{KEY} Pick Up", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return new TextObject("{=bWi4aMO9}Arrow Barrel", null).ToString();
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (GameNetwork.IsClientOrReplay)
			{
				return base.GetTickRequirement();
			}
			return ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel | base.GetTickRequirement();
		}

		protected internal override void OnTickParallel(float dt)
		{
			this.TickAux(true);
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._needsSingleThreadTickOnce)
			{
				this._needsSingleThreadTickOnce = false;
				this.TickAux(false);
			}
		}

		private void TickAux(bool isParallel)
		{
			if (this._isVisible && !GameNetwork.IsClientOrReplay)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					if (standingPoint.HasUser)
					{
						Agent userAgent = standingPoint.UserAgent;
						ActionIndexValueCache currentActionValue = userAgent.GetCurrentActionValue(0);
						ActionIndexValueCache currentActionValue2 = userAgent.GetCurrentActionValue(1);
						if (!(currentActionValue2 == ActionIndexValueCache.act_none) || (!(currentActionValue == ArrowBarrel.act_pickup_down_begin) && !(currentActionValue == ArrowBarrel.act_pickup_down_begin_left_stance)))
						{
							if (currentActionValue2 == ActionIndexValueCache.act_none && (currentActionValue == ArrowBarrel.act_pickup_down_end || currentActionValue == ArrowBarrel.act_pickup_down_end_left_stance))
							{
								if (isParallel)
								{
									this._needsSingleThreadTickOnce = true;
								}
								else
								{
									for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
									{
										if (!userAgent.Equipment[equipmentIndex].IsEmpty && (userAgent.Equipment[equipmentIndex].CurrentUsageItem.WeaponClass == WeaponClass.Arrow || userAgent.Equipment[equipmentIndex].CurrentUsageItem.WeaponClass == WeaponClass.Bolt) && userAgent.Equipment[equipmentIndex].Amount < userAgent.Equipment[equipmentIndex].ModifiedMaxAmount)
										{
											userAgent.SetWeaponAmountInSlot(equipmentIndex, userAgent.Equipment[equipmentIndex].ModifiedMaxAmount, true);
											Mission.Current.MakeSoundOnlyOnRelatedPeer(ArrowBarrel._pickupArrowSoundFromBarrelCache, userAgent.Position, userAgent.Index);
										}
									}
									userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
								}
							}
							else if (currentActionValue2 != ActionIndexValueCache.act_none || !userAgent.SetActionChannel(0, userAgent.GetIsLeftStance() ? ArrowBarrel.act_pickup_down_begin_left_stance : ArrowBarrel.act_pickup_down_begin, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
							{
								if (isParallel)
								{
									this._needsSingleThreadTickOnce = true;
								}
								else
								{
									userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
								}
							}
						}
					}
				}
			}
		}

		public override OrderType GetOrder(BattleSideEnum side)
		{
			return OrderType.None;
		}

		private static readonly ActionIndexCache act_pickup_down_begin = ActionIndexCache.Create("act_pickup_down_begin");

		private static readonly ActionIndexCache act_pickup_down_begin_left_stance = ActionIndexCache.Create("act_pickup_down_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_down_end = ActionIndexCache.Create("act_pickup_down_end");

		private static readonly ActionIndexCache act_pickup_down_end_left_stance = ActionIndexCache.Create("act_pickup_down_end_left_stance");

		private static int _pickupArrowSoundFromBarrel = -1;

		private bool _isVisible = true;

		private bool _needsSingleThreadTickOnce;
	}
}
