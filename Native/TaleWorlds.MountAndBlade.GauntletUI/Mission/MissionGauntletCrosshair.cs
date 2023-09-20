using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x02000027 RID: 39
	[OverrideView(typeof(MissionCrosshair))]
	public class MissionGauntletCrosshair : MissionGauntletBattleUIBase
	{
		// Token: 0x060001C5 RID: 453 RVA: 0x00009604 File Offset: 0x00007804
		protected override void OnCreateView()
		{
			CombatLogManager.OnGenerateCombatLog += new CombatLogManager.OnPrintCombatLogHandler(this.OnCombatLogGenerated);
			this._dataSource = new CrosshairVM();
			this._layer = new GauntletLayer(1, "GauntletLayer", false);
			this._movie = this._layer.LoadMovie("Crosshair", this._dataSource);
			if (base.Mission.Mode != 1 && base.Mission.Mode != 9)
			{
				base.MissionScreen.AddLayer(this._layer);
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000968C File Offset: 0x0000788C
		protected override void OnDestroyView()
		{
			CombatLogManager.OnGenerateCombatLog -= new CombatLogManager.OnPrintCombatLogHandler(this.OnCombatLogGenerated);
			if (base.Mission.Mode != 1 && base.Mission.Mode != 9)
			{
				base.MissionScreen.RemoveLayer(this._layer);
			}
			this._dataSource = null;
			this._movie = null;
			this._layer = null;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x000096F0 File Offset: 0x000078F0
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.DebugInput.IsKeyReleased(63) && base.IsViewActive)
			{
				this.OnDestroyView();
				this.OnCreateView();
			}
			if (!base.IsViewActive)
			{
				return;
			}
			this._dataSource.IsVisible = this.GetShouldCrosshairBeVisible();
			bool flag = true;
			bool flag2 = false;
			for (int i = 0; i < this._targetGadgetOpacities.Length; i++)
			{
				this._targetGadgetOpacities[i] = 0.0;
			}
			if (base.Mission.Mode != 1 && base.Mission.Mode != 9 && base.Mission.Mode != 6 && base.Mission.MainAgent != null && !base.MissionScreen.IsViewingCharacter() && !this.IsMissionScreenUsingCustomCamera())
			{
				this._dataSource.CrosshairType = BannerlordConfig.CrosshairType;
				Agent mainAgent = base.Mission.MainAgent;
				double num = (double)(base.MissionScreen.CameraViewAngle * 0.017453292f);
				double num2 = 2.0 * Math.Tan((double)(mainAgent.CurrentAimingError + mainAgent.CurrentAimingTurbulance) * (0.5 / Math.Tan(num * 0.5)));
				this._dataSource.SetProperties(num2, (double)(1f + (base.MissionScreen.CombatCamera.HorizontalFov - 1.5707964f) / 1.5707964f));
				WeaponInfo wieldedWeaponInfo = mainAgent.GetWieldedWeaponInfo(0);
				float num3 = MBMath.WrapAngle(mainAgent.LookDirection.AsVec2.RotationInRadians - mainAgent.GetMovementDirection().RotationInRadians);
				if (wieldedWeaponInfo.IsValid && wieldedWeaponInfo.IsRangedWeapon && BannerlordConfig.DisplayTargetingReticule)
				{
					Agent.ActionCodeType currentActionType = mainAgent.GetCurrentActionType(1);
					MissionWeapon wieldedWeapon = mainAgent.WieldedWeapon;
					if (wieldedWeapon.ReloadPhaseCount > 1 && wieldedWeapon.IsReloading && currentActionType == 18)
					{
						StackArray.StackArray10FloatFloatTuple stackArray10FloatFloatTuple = default(StackArray.StackArray10FloatFloatTuple);
						ActionIndexValueCache itemUsageReloadActionCode = MBItem.GetItemUsageReloadActionCode(wieldedWeapon.CurrentUsageItem.ItemUsage, 9, mainAgent.HasMount, -1, mainAgent.GetIsLeftStance());
						this.FillReloadDurationsFromActions(ref stackArray10FloatFloatTuple, (int)wieldedWeapon.ReloadPhaseCount, mainAgent, itemUsageReloadActionCode);
						float num4 = mainAgent.GetCurrentActionProgress(1);
						ActionIndexValueCache currentActionValue = mainAgent.GetCurrentActionValue(1);
						if (currentActionValue != ActionIndexValueCache.act_none)
						{
							float num5 = 1f - MBActionSet.GetActionBlendOutStartProgress(mainAgent.ActionSet, currentActionValue);
							num4 += num5;
						}
						float animationParameter = MBAnimation.GetAnimationParameter2(mainAgent.AgentVisuals.GetSkeleton().GetAnimationAtChannel(1));
						bool flag3 = num4 > animationParameter;
						float num6 = (flag3 ? 1f : (num4 / animationParameter));
						short reloadPhase = wieldedWeapon.ReloadPhase;
						for (int j = 0; j < (int)reloadPhase; j++)
						{
							stackArray10FloatFloatTuple[j] = new ValueTuple<float, float>(1f, stackArray10FloatFloatTuple[j].Item2);
						}
						if (!flag3)
						{
							stackArray10FloatFloatTuple[(int)reloadPhase] = new ValueTuple<float, float>(num6, stackArray10FloatFloatTuple[(int)reloadPhase].Item2);
							this._dataSource.SetReloadProperties(ref stackArray10FloatFloatTuple, (int)wieldedWeapon.ReloadPhaseCount);
						}
						flag = false;
					}
					if (currentActionType == 15)
					{
						Vec2 bodyRotationConstraint = mainAgent.GetBodyRotationConstraint(1);
						flag2 = base.Mission.MainAgent.MountAgent != null && !MBMath.IsBetween(num3, bodyRotationConstraint.x, bodyRotationConstraint.y) && (bodyRotationConstraint.x < -0.1f || bodyRotationConstraint.y > 0.1f);
					}
				}
				else if (!wieldedWeaponInfo.IsValid || wieldedWeaponInfo.IsMeleeWeapon)
				{
					Agent.ActionCodeType currentActionType2 = mainAgent.GetCurrentActionType(1);
					Agent.UsageDirection currentActionDirection = mainAgent.GetCurrentActionDirection(1);
					if (BannerlordConfig.DisplayAttackDirection && (currentActionType2 == 19 || MBMath.IsBetween(currentActionType2, 1, 15)))
					{
						if (currentActionType2 == 19)
						{
							switch (mainAgent.AttackDirection)
							{
							case 0:
								this._targetGadgetOpacities[0] = 0.7;
								break;
							case 1:
								this._targetGadgetOpacities[2] = 0.7;
								break;
							case 2:
								this._targetGadgetOpacities[3] = 0.7;
								break;
							case 3:
								this._targetGadgetOpacities[1] = 0.7;
								break;
							}
						}
						else
						{
							flag2 = true;
							switch (currentActionDirection)
							{
							case 4:
								this._targetGadgetOpacities[0] = 0.7;
								break;
							case 5:
								this._targetGadgetOpacities[2] = 0.7;
								break;
							case 6:
								this._targetGadgetOpacities[3] = 0.7;
								break;
							case 7:
								this._targetGadgetOpacities[1] = 0.7;
								break;
							}
						}
					}
					else if (BannerlordConfig.DisplayAttackDirection)
					{
						Agent.UsageDirection usageDirection = mainAgent.PlayerAttackDirection();
						if (usageDirection >= 0 && usageDirection < 4)
						{
							if (usageDirection == null)
							{
								this._targetGadgetOpacities[0] = 0.7;
							}
							else if (usageDirection == 3)
							{
								this._targetGadgetOpacities[1] = 0.7;
							}
							else if (usageDirection == 1)
							{
								this._targetGadgetOpacities[2] = 0.7;
							}
							else if (usageDirection == 2)
							{
								this._targetGadgetOpacities[3] = 0.7;
							}
						}
					}
				}
			}
			if (flag)
			{
				StackArray.StackArray10FloatFloatTuple stackArray10FloatFloatTuple2 = default(StackArray.StackArray10FloatFloatTuple);
				this._dataSource.SetReloadProperties(ref stackArray10FloatFloatTuple2, 0);
			}
			this._dataSource.SetArrowProperties(this._targetGadgetOpacities[0], this._targetGadgetOpacities[1], this._targetGadgetOpacities[2], this._targetGadgetOpacities[3]);
			this._dataSource.IsTargetInvalid = flag2;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00009C74 File Offset: 0x00007E74
		private bool GetShouldCrosshairBeVisible()
		{
			if (base.Mission.MainAgent != null)
			{
				MissionWeapon wieldedWeapon = base.Mission.MainAgent.WieldedWeapon;
				if (BannerlordConfig.DisplayTargetingReticule && base.Mission.Mode != 1 && base.Mission.Mode != 9 && !ScreenManager.GetMouseVisibility() && !wieldedWeapon.IsEmpty && wieldedWeapon.CurrentUsageItem.IsRangedWeapon && !base.MissionScreen.IsViewingCharacter() && !this.IsMissionScreenUsingCustomCamera())
				{
					return wieldedWeapon.CurrentUsageItem.WeaponClass != 16 || !wieldedWeapon.IsReloading;
				}
			}
			return false;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00009D17 File Offset: 0x00007F17
		private bool IsMissionScreenUsingCustomCamera()
		{
			return base.MissionScreen.CustomCamera != null;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00009D2C File Offset: 0x00007F2C
		private void OnCombatLogGenerated(CombatLogData logData)
		{
			bool isAttackerAgentMine = logData.IsAttackerAgentMine;
			bool flag = !logData.IsVictimAgentSameAsAttackerAgent && !logData.IsFriendlyFire;
			bool flag2 = logData.IsAttackerAgentHuman && logData.BodyPartHit == 0;
			if (isAttackerAgentMine && flag && logData.TotalDamage > 0)
			{
				CrosshairVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.ShowHitMarker(logData.IsFatalDamage, flag2);
			}
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00009D90 File Offset: 0x00007F90
		private void FillReloadDurationsFromActions(ref StackArray.StackArray10FloatFloatTuple reloadPhases, int reloadPhaseCount, Agent mainAgent, ActionIndexValueCache reloadAction)
		{
			float num = 0f;
			for (int i = 0; i < reloadPhaseCount; i++)
			{
				if (reloadAction != ActionIndexValueCache.act_none)
				{
					float num2 = MBAnimation.GetAnimationParameter2(MBActionSet.GetAnimationIndexOfAction(mainAgent.ActionSet, reloadAction)) * MBActionSet.GetActionAnimationDuration(mainAgent.ActionSet, reloadAction);
					reloadPhases[i] = new ValueTuple<float, float>(reloadPhases[i].Item1, num2);
					if (num2 > num)
					{
						num = num2;
					}
					reloadAction = MBActionSet.GetActionAnimationContinueToAction(mainAgent.ActionSet, reloadAction);
				}
			}
			if (num > 1E-05f)
			{
				for (int j = 0; j < reloadPhaseCount; j++)
				{
					reloadPhases[j] = new ValueTuple<float, float>(reloadPhases[j].Item1, reloadPhases[j].Item2 / num);
				}
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00009E48 File Offset: 0x00008048
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewActive)
			{
				this._layer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00009E6D File Offset: 0x0000806D
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewActive)
			{
				this._layer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x040000D2 RID: 210
		private GauntletLayer _layer;

		// Token: 0x040000D3 RID: 211
		private CrosshairVM _dataSource;

		// Token: 0x040000D4 RID: 212
		private IGauntletMovie _movie;

		// Token: 0x040000D5 RID: 213
		private double[] _targetGadgetOpacities = new double[4];
	}
}
