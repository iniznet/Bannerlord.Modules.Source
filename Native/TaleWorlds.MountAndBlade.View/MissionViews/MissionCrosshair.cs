using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	[DefaultView]
	public class MissionCrosshair : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._crosshairEntities = new GameEntity[3];
			this._arrowEntities = new GameEntity[4];
			this._gadgetOpacities = new float[7];
			for (int i = 0; i < 3; i++)
			{
				this._crosshairEntities[i] = GameEntity.CreateEmpty(base.Mission.Scene, true);
				string text = ((i == 0) ? "crosshair_top" : ((i == 1) ? "crosshair_left" : "crosshair_right"));
				MetaMesh copy = MetaMesh.GetCopy(text, true, false);
				int meshCount = copy.MeshCount;
				for (int j = 0; j < meshCount; j++)
				{
					Mesh meshAtIndex = copy.GetMeshAtIndex(j);
					meshAtIndex.SetMeshRenderOrder(200);
					meshAtIndex.VisibilityMask = 1;
				}
				this._crosshairEntities[i].AddComponent(copy);
				MatrixFrame identity = MatrixFrame.Identity;
				this._crosshairEntities[i].Name = text;
				this._crosshairEntities[i].SetFrame(ref identity);
				this._crosshairEntities[i].SetVisibilityExcludeParents(false);
			}
			for (int k = 0; k < 4; k++)
			{
				this._arrowEntities[k] = GameEntity.CreateEmpty(base.Mission.Scene, true);
				string text2 = ((k == 0) ? "arrow_up" : ((k == 1) ? "arrow_right" : ((k == 2) ? "arrow_down" : "arrow_left")));
				MetaMesh copy2 = MetaMesh.GetCopy(text2, true, false);
				int meshCount2 = copy2.MeshCount;
				for (int l = 0; l < meshCount2; l++)
				{
					Mesh meshAtIndex2 = copy2.GetMeshAtIndex(l);
					meshAtIndex2.SetMeshRenderOrder(200);
					meshAtIndex2.VisibilityMask = 1;
				}
				this._arrowEntities[k].AddComponent(copy2);
				MatrixFrame identity2 = MatrixFrame.Identity;
				this._arrowEntities[k].Name = text2;
				this._arrowEntities[k].SetFrame(ref identity2);
				this._arrowEntities[k].SetVisibilityExcludeParents(false);
			}
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			bool flag = false;
			float[] array = new float[8];
			for (int i = 0; i < 7; i++)
			{
				array[i] = 0f;
			}
			if (MBEditor.EditModeEnabled && (this._crosshairEntities[0] == null || this._arrowEntities[0] == null))
			{
				return;
			}
			this._crosshairEntities[0].SetVisibilityExcludeParents(false);
			this._crosshairEntities[1].SetVisibilityExcludeParents(false);
			this._crosshairEntities[2].SetVisibilityExcludeParents(false);
			this._arrowEntities[0].SetVisibilityExcludeParents(false);
			this._arrowEntities[1].SetVisibilityExcludeParents(false);
			this._arrowEntities[2].SetVisibilityExcludeParents(false);
			this._arrowEntities[3].SetVisibilityExcludeParents(false);
			if (base.Mission.Mode != 1 && base.Mission.Mode != 9)
			{
				float near = base.MissionScreen.CombatCamera.Near;
				float num = 4.7f + (base.MissionScreen.CombatCamera.HorizontalFov - 0.64f) * 7.14f;
				if (base.Mission.MainAgent != null)
				{
					Agent mainAgent = base.Mission.MainAgent;
					float num2 = base.MissionScreen.CameraViewAngle * 0.017453292f;
					float num3 = MathF.Tan((mainAgent.CurrentAimingError + mainAgent.CurrentAimingTurbulance) * (0.5f / MathF.Tan(num2 * 0.5f)));
					new Vec2(0.5f, 0.375f);
					Vec2 vec;
					vec..ctor(0f, num3);
					MatrixFrame frame = base.MissionScreen.CombatCamera.Frame;
					frame.Elevate(-5f);
					frame.rotation.ApplyScaleLocal(new Vec3(num, num, num, -1f));
					frame.Strafe(vec.x);
					frame.Advance(vec.y);
					this._crosshairEntities[0].SetFrame(ref frame);
					Vec2 vec2 = vec;
					vec2.RotateCCW(2.268928f);
					MatrixFrame frame2 = base.MissionScreen.CombatCamera.Frame;
					frame2.Elevate(-5f);
					frame2.rotation.ApplyScaleLocal(new Vec3(num, num, num, -1f));
					frame2.Strafe(vec2.x);
					frame2.Advance(vec2.y);
					this._crosshairEntities[1].SetFrame(ref frame2);
					Vec2 vec3 = vec;
					vec3.RotateCCW(-2.268928f);
					MatrixFrame frame3 = base.MissionScreen.CombatCamera.Frame;
					frame3.Elevate(-5f);
					frame3.rotation.ApplyScaleLocal(new Vec3(num, num, num, -1f));
					frame3.Strafe(vec3.x);
					frame3.Advance(vec3.y);
					this._crosshairEntities[2].SetFrame(ref frame3);
					MatrixFrame frame4 = base.MissionScreen.CombatCamera.Frame;
					frame4.Elevate(-5f);
					frame4.rotation.ApplyScaleLocal(new Vec3(num, num, num, -1f));
					frame4.Strafe(0f);
					frame4.Advance(0.07499999f);
					this._arrowEntities[0].SetFrame(ref frame4);
					MatrixFrame frame5 = base.MissionScreen.CombatCamera.Frame;
					frame5.Elevate(-5f);
					frame5.rotation.ApplyScaleLocal(new Vec3(num, num, num, -1f));
					frame5.Strafe(0.14999998f);
					frame5.Advance(-0.025000006f);
					this._arrowEntities[1].SetFrame(ref frame5);
					MatrixFrame frame6 = base.MissionScreen.CombatCamera.Frame;
					frame6.Elevate(-5f);
					frame6.rotation.ApplyScaleLocal(new Vec3(num, num, num, -1f));
					frame6.Strafe(0f);
					frame6.Advance(-0.07499999f);
					this._arrowEntities[2].SetFrame(ref frame6);
					MatrixFrame frame7 = base.MissionScreen.CombatCamera.Frame;
					frame7.Elevate(-5f);
					frame7.rotation.ApplyScaleLocal(new Vec3(num, num, num, -1f));
					frame7.Strafe(-0.15f);
					frame7.Advance(-0.025000006f);
					this._arrowEntities[3].SetFrame(ref frame7);
					WeaponInfo wieldedWeaponInfo = mainAgent.GetWieldedWeaponInfo(0);
					float num4 = MBMath.WrapAngle(mainAgent.LookDirection.AsVec2.RotationInRadians - mainAgent.GetMovementDirection().RotationInRadians);
					if (wieldedWeaponInfo.IsValid && !base.MissionScreen.IsViewingCharacter())
					{
						if (wieldedWeaponInfo.IsRangedWeapon && BannerlordConfig.DisplayTargetingReticule)
						{
							Vec2 bodyRotationConstraint = mainAgent.GetBodyRotationConstraint(1);
							if (base.Mission.MainAgent.MountAgent == null || MBMath.IsBetween(num4, bodyRotationConstraint.x, bodyRotationConstraint.y))
							{
								array[0] = 0.9f;
								array[1] = 0.9f;
								array[2] = 0.9f;
							}
							else if (base.Mission.MainAgent.MountAgent != null && !MBMath.IsBetween(num4, bodyRotationConstraint.x, bodyRotationConstraint.y) && (bodyRotationConstraint.x < -0.1f || bodyRotationConstraint.y > 0.1f))
							{
								flag = true;
							}
						}
						else if (wieldedWeaponInfo.IsMeleeWeapon)
						{
							Agent.ActionCodeType currentActionType = mainAgent.GetCurrentActionType(1);
							Agent.UsageDirection currentActionDirection = mainAgent.GetCurrentActionDirection(1);
							if (BannerlordConfig.DisplayAttackDirection && (currentActionType == 19 || currentActionDirection != -1))
							{
								if (currentActionType == 19)
								{
									switch (mainAgent.AttackDirection)
									{
									case 0:
										array[3] = 0.7f;
										break;
									case 1:
										array[5] = 0.7f;
										break;
									case 2:
										array[6] = 0.7f;
										break;
									case 3:
										array[4] = 0.7f;
										break;
									}
								}
								else
								{
									flag = true;
									switch (currentActionDirection)
									{
									case 4:
										array[3] = 0.7f;
										break;
									case 5:
										array[5] = 0.7f;
										break;
									case 6:
										array[6] = 0.7f;
										break;
									case 7:
										array[4] = 0.7f;
										break;
									}
								}
							}
							else if (BannerlordConfig.DisplayAttackDirection || BannerlordConfig.AttackDirectionControl == 0)
							{
								Agent.UsageDirection usageDirection = mainAgent.PlayerAttackDirection();
								if (usageDirection >= 0 && usageDirection < 4)
								{
									if (usageDirection == null)
									{
										array[3] = 0.7f;
									}
									else if (usageDirection == 3)
									{
										array[4] = 0.7f;
									}
									else if (usageDirection == 1)
									{
										array[5] = 0.7f;
									}
									else if (usageDirection == 2)
									{
										array[6] = 0.7f;
									}
								}
							}
						}
					}
				}
				for (int j = 0; j < 7; j++)
				{
					float num5;
					if (j < 3)
					{
						num5 = dt * 5f;
					}
					else
					{
						num5 = dt * 3f;
					}
					if (array[j] > this._gadgetOpacities[j])
					{
						this._gadgetOpacities[j] += 1.2f * num5;
						if (this._gadgetOpacities[j] > array[j])
						{
							this._gadgetOpacities[j] = array[j];
						}
					}
					else if (array[j] < this._gadgetOpacities[j])
					{
						this._gadgetOpacities[j] -= num5;
						if (this._gadgetOpacities[j] < array[j])
						{
							this._gadgetOpacities[j] = array[j];
						}
					}
					int num6 = (int)(255f * this._gadgetOpacities[j]);
					if (num6 > 0)
					{
						if (j < 3)
						{
							this._crosshairEntities[j].SetVisibilityExcludeParents(true);
						}
						else
						{
							this._arrowEntities[j - 3].SetVisibilityExcludeParents(true);
						}
					}
					num6 <<= 24;
					if (j < 3)
					{
						Mesh firstMesh = this._crosshairEntities[j].GetFirstMesh();
						if (flag)
						{
							firstMesh.Color = (uint)(16711680 | num6);
						}
						else
						{
							firstMesh.Color = (uint)(16777215 | num6);
						}
					}
					else
					{
						Mesh firstMesh2 = this._arrowEntities[j - 3].GetFirstMesh();
						if (flag)
						{
							firstMesh2.Color = (uint)(3386111 | num6);
						}
						else
						{
							firstMesh2.Color = (uint)(16759569 | num6);
						}
					}
				}
			}
		}

		private GameEntity[] _crosshairEntities;

		private GameEntity[] _arrowEntities;

		private float[] _gadgetOpacities;

		private const int GadgetCount = 7;
	}
}
