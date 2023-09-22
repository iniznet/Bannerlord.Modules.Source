using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Helpers;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.View.Map
{
	public class PartyVisual
	{
		public MapScreen MapScreen
		{
			get
			{
				return MapScreen.Instance;
			}
		}

		public GameEntity StrategicEntity { get; private set; }

		public List<GameEntity> TownPhysicalEntities { get; private set; }

		public MatrixFrame CircleLocalFrame { get; private set; }

		public Vec2 Position
		{
			get
			{
				return this.PartyBase.Position2D;
			}
		}

		public IMapEntity GetMapEntity()
		{
			return this.PartyBase.MapEntity;
		}

		public bool TargetVisibility
		{
			get
			{
				return this.PartyBase.IsVisible;
			}
		}

		private Scene MapScene
		{
			get
			{
				if (this._mapScene == null && Campaign.Current != null && Campaign.Current.MapSceneWrapper != null)
				{
					this._mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
				}
				return this._mapScene;
			}
		}

		public AgentVisuals HumanAgentVisuals { get; private set; }

		public AgentVisuals MountAgentVisuals { get; private set; }

		public AgentVisuals CaravanMountAgentVisuals { get; private set; }

		public bool IsEnemy { get; private set; }

		public bool IsFriendly { get; private set; }

		public bool IsEntityMovingVisually()
		{
			if (!this.PartyBase.IsMobile)
			{
				return false;
			}
			if (!this.PartyBase.MobileParty.VisualPosition2DWithoutError.NearlyEquals(this._lastFrameVisualPositionWithoutError, 1E-05f))
			{
				if (Campaign.Current.TimeControlMode != null)
				{
					this._lastFrameVisualPositionWithoutError = this.PartyBase.MobileParty.VisualPosition2DWithoutError;
				}
				return true;
			}
			return false;
		}

		public PartyVisual(PartyBase partyBase)
		{
			this.PartyBase = partyBase;
			this._siegeRangedMachineEntities = new List<ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity>>();
			this._siegeMeleeMachineEntities = new List<ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity>>();
			this._siegeMissileEntities = new List<ValueTuple<GameEntity, BattleSideEnum, int>>();
			this.CircleLocalFrame = MatrixFrame.Identity;
		}

		private void AddMountToPartyIcon(Vec3 positionOffset, string mountItemId, string harnessItemId, uint contourColor, CharacterObject character)
		{
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(mountItemId);
			Monster monster = @object.HorseComponent.Monster;
			ItemObject itemObject = null;
			if (!string.IsNullOrEmpty(harnessItemId))
			{
				itemObject = Game.Current.ObjectManager.GetObject<ItemObject>(harnessItemId);
			}
			Equipment equipment = new Equipment();
			equipment[10] = new EquipmentElement(@object, null, null, false);
			equipment[11] = new EquipmentElement(itemObject, null, null, false);
			AgentVisualsData agentVisualsData = new AgentVisualsData().Equipment(equipment).Scale(@object.ScaleFactor * 0.3f).Frame(new MatrixFrame(Mat3.Identity, positionOffset))
				.ActionSet(MBGlobals.GetActionSet(monster.ActionSetCode + "_map"))
				.Scene(this.MapScene)
				.Monster(monster)
				.PrepareImmediately(false)
				.UseScaledWeapons(true)
				.HasClippingPlane(true)
				.MountCreationKey(MountCreationKey.GetRandomMountKeyString(@object, character.GetMountKeySeed()));
			this.CaravanMountAgentVisuals = AgentVisuals.Create(agentVisualsData, "PartyIcon " + mountItemId, false, false, false);
			this.CaravanMountAgentVisuals.GetEntity().SetContourColor(new uint?(contourColor), false);
			MatrixFrame matrixFrame = this.CaravanMountAgentVisuals.GetFrame();
			matrixFrame.rotation.ApplyScaleLocal(this.CaravanMountAgentVisuals.GetScale());
			matrixFrame = this.StrategicEntity.GetFrame().TransformToParent(matrixFrame);
			this.CaravanMountAgentVisuals.GetEntity().SetFrame(ref matrixFrame);
			float num = MathF.Min(0.325f * this._speed / 0.3f, 20f);
			this.CaravanMountAgentVisuals.Tick(null, 0.0001f, this.IsEntityMovingVisually(), num);
			this.CaravanMountAgentVisuals.GetEntity().Skeleton.ForceUpdateBoneFrames();
		}

		private void AddCharacterToPartyIcon(CharacterObject characterObject, uint contourColor, string bannerKey, int wieldedItemIndex, uint teamColor1, uint teamColor2, ActionIndexCache leaderAction, ActionIndexCache mountAction, float animationStartDuration, ref bool clearBannerEntityCache)
		{
			Equipment equipment = characterObject.Equipment.Clone(false);
			bool flag = !string.IsNullOrEmpty(bannerKey) && (((characterObject.IsPlayerCharacter || characterObject.HeroObject.Clan == Clan.PlayerClan) && Clan.PlayerClan.Tier >= Campaign.Current.Models.ClanTierModel.BannerEligibleTier) || (!characterObject.IsPlayerCharacter && (!characterObject.IsHero || (characterObject.IsHero && characterObject.HeroObject.Clan != Clan.PlayerClan))));
			int num = 4;
			if (flag)
			{
				ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>("campaign_banner_small");
				equipment[4] = new EquipmentElement(@object, null, null, false);
			}
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(characterObject.Race);
			MBActionSet actionSetWithSuffix = MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, characterObject.IsFemale, flag ? "_map_with_banner" : "_map");
			AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(true).Equipment(equipment).BodyProperties(characterObject.GetBodyProperties(characterObject.Equipment, -1))
				.SkeletonType(characterObject.IsFemale ? 1 : 0)
				.Scale(0.3f)
				.Frame(this.StrategicEntity.GetFrame())
				.ActionSet(actionSetWithSuffix)
				.Scene(this.MapScene)
				.Monster(baseMonsterFromRace)
				.PrepareImmediately(false)
				.RightWieldedItemIndex(wieldedItemIndex)
				.HasClippingPlane(true)
				.UseScaledWeapons(true)
				.ClothColor1(teamColor1)
				.ClothColor2(teamColor2)
				.CharacterObjectStringId(characterObject.StringId)
				.AddColorRandomness(!characterObject.IsHero)
				.Race(characterObject.Race);
			if (flag)
			{
				Banner banner = new Banner(bannerKey);
				agentVisualsData.Banner(banner).LeftWieldedItemIndex(num);
				if (this._cachedBannerEntity.Item1 == bannerKey + "campaign_banner_small")
				{
					agentVisualsData.CachedWeaponEntity(4, this._cachedBannerEntity.Item2);
				}
			}
			this.HumanAgentVisuals = AgentVisuals.Create(agentVisualsData, "PartyIcon " + characterObject.Name, false, false, false);
			if (flag)
			{
				GameEntity entity = this.HumanAgentVisuals.GetEntity();
				GameEntity child = entity.GetChild(entity.ChildCount - 1);
				if (child.GetComponentCount(3) > 0)
				{
					clearBannerEntityCache = false;
					this._cachedBannerEntity = new ValueTuple<string, GameEntity>(bannerKey + "campaign_banner_small", child);
				}
			}
			if (leaderAction != ActionIndexCache.act_none)
			{
				float actionAnimationDuration = MBActionSet.GetActionAnimationDuration(actionSetWithSuffix, leaderAction);
				if (actionAnimationDuration < 1f)
				{
					MBSkeletonExtensions.SetAgentActionChannel(this.HumanAgentVisuals.GetVisuals().GetSkeleton(), 0, leaderAction, animationStartDuration, -0.2f, true);
				}
				else
				{
					MBSkeletonExtensions.SetAgentActionChannel(this.HumanAgentVisuals.GetVisuals().GetSkeleton(), 0, leaderAction, animationStartDuration / actionAnimationDuration, -0.2f, true);
				}
			}
			if (characterObject.HasMount())
			{
				Monster monster = characterObject.Equipment[10].Item.HorseComponent.Monster;
				MBActionSet actionSet = MBGlobals.GetActionSet(monster.ActionSetCode + "_map");
				AgentVisualsData agentVisualsData2 = new AgentVisualsData().Equipment(characterObject.Equipment).Scale(characterObject.Equipment[10].Item.ScaleFactor * 0.3f).Frame(MatrixFrame.Identity)
					.ActionSet(actionSet)
					.Scene(this.MapScene)
					.Monster(monster)
					.PrepareImmediately(false)
					.UseScaledWeapons(true)
					.HasClippingPlane(true)
					.MountCreationKey(MountCreationKey.GetRandomMountKeyString(characterObject.Equipment[10].Item, characterObject.GetMountKeySeed()));
				this.MountAgentVisuals = AgentVisuals.Create(agentVisualsData2, "PartyIcon " + characterObject.Name + " mount", false, false, false);
				if (mountAction != ActionIndexCache.act_none)
				{
					float actionAnimationDuration2 = MBActionSet.GetActionAnimationDuration(actionSet, mountAction);
					if (actionAnimationDuration2 < 1f)
					{
						MBSkeletonExtensions.SetAgentActionChannel(this.MountAgentVisuals.GetEntity().Skeleton, 0, mountAction, animationStartDuration, -0.2f, true);
					}
					else
					{
						MBSkeletonExtensions.SetAgentActionChannel(this.MountAgentVisuals.GetEntity().Skeleton, 0, mountAction, animationStartDuration / actionAnimationDuration2, -0.2f, true);
					}
				}
				this.MountAgentVisuals.GetEntity().SetContourColor(new uint?(contourColor), false);
				MatrixFrame frame = this.StrategicEntity.GetFrame();
				frame.rotation.ApplyScaleLocal(agentVisualsData2.ScaleData);
				this.MountAgentVisuals.GetEntity().SetFrame(ref frame);
			}
			this.HumanAgentVisuals.GetEntity().SetContourColor(new uint?(contourColor), false);
			MatrixFrame frame2 = this.StrategicEntity.GetFrame();
			frame2.rotation.ApplyScaleLocal(agentVisualsData.ScaleData);
			this.HumanAgentVisuals.GetEntity().SetFrame(ref frame2);
			float num2 = ((this.MountAgentVisuals != null) ? 1.3f : 1f);
			float num3 = MathF.Min(0.25f * num2 * this._speed / 0.3f, 20f);
			if (this.MountAgentVisuals != null)
			{
				this.MountAgentVisuals.Tick(null, 0.0001f, this.IsEntityMovingVisually(), num3);
				this.MountAgentVisuals.GetEntity().Skeleton.ForceUpdateBoneFrames();
			}
			this.HumanAgentVisuals.Tick(this.MountAgentVisuals, 0.0001f, this.IsEntityMovingVisually(), num3);
			this.HumanAgentVisuals.GetEntity().Skeleton.ForceUpdateBoneFrames();
		}

		private static MetaMesh GetBannerOfCharacter(Banner banner, string bannerMeshName)
		{
			MetaMesh copy = MetaMesh.GetCopy(bannerMeshName, true, false);
			for (int i = 0; i < copy.MeshCount; i++)
			{
				Mesh meshAtIndex = copy.GetMeshAtIndex(i);
				if (!meshAtIndex.HasTag("dont_use_tableau"))
				{
					Material material = meshAtIndex.GetMaterial();
					Material tableauMaterial = null;
					Tuple<Material, BannerCode> tuple = new Tuple<Material, BannerCode>(material, BannerCode.CreateFrom(banner));
					if (MapScreen.Instance._characterBannerMaterialCache.ContainsKey(tuple))
					{
						tableauMaterial = MapScreen.Instance._characterBannerMaterialCache[tuple];
					}
					else
					{
						tableauMaterial = material.CreateCopy();
						Action<Texture> action = delegate(Texture tex)
						{
							tableauMaterial.SetTexture(1, tex);
							uint num = (uint)tableauMaterial.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
							ulong shaderFlags = tableauMaterial.GetShaderFlags();
							tableauMaterial.SetShaderFlags(shaderFlags | (ulong)num);
						};
						BannerVisualExtensions.GetTableauTextureLarge(banner, action);
						MapScreen.Instance._characterBannerMaterialCache[tuple] = tableauMaterial;
					}
					meshAtIndex.SetMaterial(tableauMaterial);
				}
			}
			return copy;
		}

		public void Tick(float dt, ref int dirtyPartiesCount, ref PartyVisual[] dirtyPartiesList)
		{
			if (this.PartyBase.IsSettlement)
			{
				this.TickSettlementVisual(dt, ref dirtyPartiesCount, ref dirtyPartiesList);
			}
			else
			{
				this.TickMobilePartyVisual(dt, ref dirtyPartiesCount, ref dirtyPartiesList);
			}
			if (this.PartyBase.LevelMaskIsDirty)
			{
				this.RefreshLevelMask();
			}
		}

		private void TickSettlementVisual(float dt, ref int dirtyPartiesCount, ref PartyVisual[] dirtyPartiesList)
		{
			if (this.StrategicEntity == null)
			{
				return;
			}
			if (this.PartyBase.IsVisualDirty)
			{
				int num = Interlocked.Increment(ref dirtyPartiesCount);
				dirtyPartiesList[num] = this;
				return;
			}
			double toHours = CampaignTime.Now.ToHours;
			foreach (ValueTuple<GameEntity, BattleSideEnum, int> valueTuple in this._siegeMissileEntities)
			{
				GameEntity item = valueTuple.Item1;
				ISiegeEventSide siegeEventSide = this.PartyBase.Settlement.SiegeEvent.GetSiegeEventSide(valueTuple.Item2);
				int item2 = valueTuple.Item3;
				bool flag = false;
				if (siegeEventSide.SiegeEngineMissiles.Count > item2)
				{
					SiegeEvent.SiegeEngineMissile siegeEngineMissile = siegeEventSide.SiegeEngineMissiles[item2];
					CampaignTime collisionTime = siegeEngineMissile.CollisionTime;
					double toHours2 = collisionTime.ToHours;
					PartyVisual.SiegeBombardmentData siegeBombardmentData;
					this.CalculateDataAndDurationsForSiegeMachine(siegeEngineMissile.ShooterSlotIndex, siegeEngineMissile.ShooterSiegeEngineType, siegeEventSide.BattleSide, siegeEngineMissile.TargetType, siegeEngineMissile.TargetSlotIndex, out siegeBombardmentData);
					float num2 = siegeBombardmentData.MissileSpeed * MathF.Cos(siegeBombardmentData.LaunchAngle);
					if (toHours > toHours2 - (double)siegeBombardmentData.TotalDuration)
					{
						bool flag2 = toHours - (double)dt > toHours2 - (double)siegeBombardmentData.FlightDuration && toHours - (double)dt < toHours2;
						bool flag3 = toHours > toHours2 - (double)siegeBombardmentData.FlightDuration && toHours < toHours2;
						if (flag3)
						{
							flag = true;
							float num3 = (float)(toHours - (toHours2 - (double)siegeBombardmentData.FlightDuration));
							float num4 = siegeBombardmentData.MissileSpeed * MathF.Sin(siegeBombardmentData.LaunchAngle);
							Vec2 vec;
							vec..ctor(num2 * num3, num4 * num3 - siegeBombardmentData.Gravity * 0.5f * num3 * num3);
							Vec3 vec2 = siegeBombardmentData.LaunchGlobalPosition + siegeBombardmentData.TargetAlignedShooterGlobalFrame.rotation.f.NormalizedCopy() * vec.x + siegeBombardmentData.TargetAlignedShooterGlobalFrame.rotation.u.NormalizedCopy() * vec.y;
							float num5 = num3 + 0.1f;
							Vec2 vec3;
							vec3..ctor(num2 * num5, num4 * num5 - siegeBombardmentData.Gravity * 0.5f * num5 * num5);
							Vec3 vec4 = siegeBombardmentData.LaunchGlobalPosition + siegeBombardmentData.TargetAlignedShooterGlobalFrame.rotation.f.NormalizedCopy() * vec3.x + siegeBombardmentData.TargetAlignedShooterGlobalFrame.rotation.u.NormalizedCopy() * vec3.y;
							Mat3 rotation = item.GetGlobalFrame().rotation;
							rotation.f = vec4 - vec2;
							rotation.Orthonormalize();
							rotation.ApplyScaleLocal(this.MapScreen.PrefabEntityCache.GetScaleForSiegeEngine(siegeEngineMissile.ShooterSiegeEngineType, siegeEventSide.BattleSide));
							MatrixFrame matrixFrame;
							matrixFrame..ctor(rotation, vec2);
							item.SetGlobalFrame(ref matrixFrame);
						}
						item.GetChild(0).SetVisibilityExcludeParents(flag3);
						int num6 = -1;
						if (!flag2 && flag3)
						{
							if (siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.Ballista || siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.FireBallista)
							{
								num6 = MiscSoundContainer.SoundCodeAmbientNodeSiegeBallistaFire;
							}
							else if (siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.Catapult || siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.FireCatapult || siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.Onager || siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.FireOnager)
							{
								num6 = MiscSoundContainer.SoundCodeAmbientNodeSiegeMangonelFire;
							}
							else
							{
								num6 = MiscSoundContainer.SoundCodeAmbientNodeSiegeTrebuchetFire;
							}
						}
						else if (flag2 && !flag3)
						{
							this.StrategicEntity.Scene.CreateBurstParticle(ParticleSystemManager.GetRuntimeIdByName((siegeEngineMissile.TargetType == 2) ? "psys_game_ballista_destruction" : "psys_campaign_boulder_stone_coll"), item.GetGlobalFrame());
							num6 = ((siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.Ballista || siegeEngineMissile.ShooterSiegeEngineType == DefaultSiegeEngineTypes.FireBallista) ? MiscSoundContainer.SoundCodeAmbientNodeSiegeBallistaHit : MiscSoundContainer.SoundCodeAmbientNodeSiegeBoulderHit);
						}
						MBSoundEvent.PlaySound(num6, item.GlobalPosition);
						if (toHours >= toHours2 - (double)(siegeBombardmentData.TotalDuration - siegeBombardmentData.RotationDuration - siegeBombardmentData.ReloadDuration))
						{
							if (toHours < toHours2 - (double)(siegeBombardmentData.TotalDuration - siegeBombardmentData.RotationDuration - siegeBombardmentData.ReloadDuration - siegeBombardmentData.AimingDuration))
							{
								if (siegeEventSide.SiegeEngines.DeployedRangedSiegeEngines[siegeEngineMissile.ShooterSlotIndex] == null || siegeEventSide.SiegeEngines.DeployedRangedSiegeEngines[siegeEngineMissile.ShooterSlotIndex].SiegeEngine != siegeEngineMissile.ShooterSiegeEngineType)
								{
									goto IL_64E;
								}
								using (List<ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity>>.Enumerator enumerator2 = this._siegeRangedMachineEntities.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity> valueTuple2 = enumerator2.Current;
										if (!flag && valueTuple2.Item2 == siegeEventSide.BattleSide && valueTuple2.Item3 == siegeEngineMissile.ShooterSlotIndex)
										{
											GameEntity item3 = valueTuple2.Item5;
											if (item3 != null)
											{
												flag = true;
												GameEntity gameEntity = item;
												MatrixFrame matrixFrame2 = item3.GetGlobalFrame();
												matrixFrame2 = matrixFrame2.TransformToParent(MBSkeletonExtensions.GetBoneEntitialFrame(item3.Skeleton, Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapProjectileBoneIndex(siegeEngineMissile.ShooterSiegeEngineType, siegeEventSide.BattleSide), false));
												gameEntity.SetGlobalFrame(ref matrixFrame2);
											}
										}
									}
									goto IL_64E;
								}
							}
							if (toHours < toHours2 - (double)(siegeBombardmentData.TotalDuration - siegeBombardmentData.RotationDuration - siegeBombardmentData.ReloadDuration - siegeBombardmentData.AimingDuration - siegeBombardmentData.FireDuration) && !flag3 && siegeEventSide.SiegeEngines.DeployedRangedSiegeEngines[siegeEngineMissile.ShooterSlotIndex] != null && siegeEventSide.SiegeEngines.DeployedRangedSiegeEngines[siegeEngineMissile.ShooterSlotIndex].SiegeEngine == siegeEngineMissile.ShooterSiegeEngineType)
							{
								foreach (ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity> valueTuple3 in this._siegeRangedMachineEntities)
								{
									if (!flag && valueTuple3.Item2 == siegeEventSide.BattleSide && valueTuple3.Item3 == siegeEngineMissile.ShooterSlotIndex)
									{
										GameEntity item4 = valueTuple3.Item5;
										if (item4 != null)
										{
											flag = true;
											GameEntity gameEntity2 = item;
											MatrixFrame matrixFrame2 = item4.GetGlobalFrame();
											matrixFrame2 = matrixFrame2.TransformToParent(MBSkeletonExtensions.GetBoneEntitialFrame(item4.Skeleton, Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapProjectileBoneIndex(siegeEngineMissile.ShooterSiegeEngineType, siegeEventSide.BattleSide), false));
											gameEntity2.SetGlobalFrame(ref matrixFrame2);
										}
									}
								}
							}
						}
					}
				}
				IL_64E:
				item.SetVisibilityExcludeParents(flag);
			}
			foreach (ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity> valueTuple4 in this._siegeRangedMachineEntities)
			{
				GameEntity item5 = valueTuple4.Item1;
				BattleSideEnum item6 = valueTuple4.Item2;
				int item7 = valueTuple4.Item3;
				GameEntity item8 = valueTuple4.Item5;
				SiegeEngineType siegeEngine = this.PartyBase.Settlement.SiegeEvent.GetSiegeEventSide(item6).SiegeEngines.DeployedRangedSiegeEngines[item7].SiegeEngine;
				if (item8 != null)
				{
					Skeleton skeleton = item8.Skeleton;
					string siegeEngineMapFireAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapFireAnimationName(siegeEngine, item6);
					string siegeEngineMapReloadAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapReloadAnimationName(siegeEngine, item6);
					SiegeEvent.RangedSiegeEngine rangedSiegeEngine = this.PartyBase.Settlement.SiegeEvent.GetSiegeEventSide(item6).SiegeEngines.DeployedRangedSiegeEngines[item7].RangedSiegeEngine;
					PartyVisual.SiegeBombardmentData siegeBombardmentData2;
					this.CalculateDataAndDurationsForSiegeMachine(item7, siegeEngine, item6, rangedSiegeEngine.CurrentTargetType, rangedSiegeEngine.CurrentTargetIndex, out siegeBombardmentData2);
					MatrixFrame shooterGlobalFrame = siegeBombardmentData2.ShooterGlobalFrame;
					if (rangedSiegeEngine.PreviousTargetIndex >= 0)
					{
						Vec3 vec5;
						if (rangedSiegeEngine.PreviousDamagedTargetType == 1)
						{
							vec5 = this._defenderBreachableWallEntitiesCacheForCurrentLevel[rangedSiegeEngine.PreviousTargetIndex].GlobalPosition;
						}
						else
						{
							vec5 = ((item6 == 1) ? this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[rangedSiegeEngine.PreviousTargetIndex].GetGlobalFrame().origin : this._attackerRangedEngineSpawnEntities[rangedSiegeEngine.PreviousTargetIndex].GetGlobalFrame().origin);
						}
						shooterGlobalFrame.rotation.f.AsVec2 = (vec5 - shooterGlobalFrame.origin).AsVec2;
						shooterGlobalFrame.rotation.f.NormalizeWithoutChangingZ();
						shooterGlobalFrame.rotation.Orthonormalize();
					}
					item5.SetGlobalFrame(ref shooterGlobalFrame);
					skeleton.TickAnimations(dt, MatrixFrame.Identity, false);
					double toHours3 = rangedSiegeEngine.NextProjectileCollisionTime.ToHours;
					if (toHours > toHours3 - (double)siegeBombardmentData2.TotalDuration)
					{
						if (toHours < toHours3 - (double)(siegeBombardmentData2.TotalDuration - siegeBombardmentData2.RotationDuration))
						{
							float rotationInRadians = (siegeBombardmentData2.TargetPosition - shooterGlobalFrame.origin).AsVec2.RotationInRadians;
							float rotationInRadians2 = shooterGlobalFrame.rotation.f.AsVec2.RotationInRadians;
							float num7 = rotationInRadians - rotationInRadians2;
							float num8 = MathF.Abs(num7);
							float num9 = (float)(toHours3 - (double)(siegeBombardmentData2.TotalDuration - siegeBombardmentData2.RotationDuration) - toHours);
							if (num8 > num9 * 2f)
							{
								shooterGlobalFrame.rotation.f.AsVec2 = Vec2.FromRotation(rotationInRadians2 + (float)MathF.Sign(num7) * (num8 - num9 * 2f));
								shooterGlobalFrame.rotation.f.NormalizeWithoutChangingZ();
								shooterGlobalFrame.rotation.Orthonormalize();
								item5.SetGlobalFrame(ref shooterGlobalFrame);
							}
						}
						else if (toHours < toHours3 - (double)(siegeBombardmentData2.TotalDuration - siegeBombardmentData2.RotationDuration - siegeBombardmentData2.ReloadDuration))
						{
							item5.SetGlobalFrame(ref siegeBombardmentData2.TargetAlignedShooterGlobalFrame);
							MBSkeletonExtensions.SetAnimationAtChannel(skeleton, siegeEngineMapReloadAnimationName, 0, 1f, 0f, (float)((toHours - (toHours3 - (double)(siegeBombardmentData2.TotalDuration - siegeBombardmentData2.RotationDuration))) / (double)siegeBombardmentData2.ReloadDuration));
						}
						else if (toHours < toHours3 - (double)(siegeBombardmentData2.TotalDuration - siegeBombardmentData2.RotationDuration - siegeBombardmentData2.ReloadDuration - siegeBombardmentData2.AimingDuration))
						{
							item5.SetGlobalFrame(ref siegeBombardmentData2.TargetAlignedShooterGlobalFrame);
							MBSkeletonExtensions.SetAnimationAtChannel(skeleton, siegeEngineMapReloadAnimationName, 0, 1f, 0f, 1f);
						}
						else if (toHours < toHours3 - (double)(siegeBombardmentData2.TotalDuration - siegeBombardmentData2.RotationDuration - siegeBombardmentData2.ReloadDuration - siegeBombardmentData2.AimingDuration - siegeBombardmentData2.FireDuration))
						{
							item5.SetGlobalFrame(ref siegeBombardmentData2.TargetAlignedShooterGlobalFrame);
							MBSkeletonExtensions.SetAnimationAtChannel(skeleton, siegeEngineMapFireAnimationName, 0, 1f, 0f, (float)((toHours - (toHours3 - (double)(siegeBombardmentData2.TotalDuration - siegeBombardmentData2.RotationDuration - siegeBombardmentData2.ReloadDuration - siegeBombardmentData2.AimingDuration))) / (double)siegeBombardmentData2.FireDuration));
						}
						else
						{
							item5.SetGlobalFrame(ref siegeBombardmentData2.TargetAlignedShooterGlobalFrame);
							MBSkeletonExtensions.SetAnimationAtChannel(skeleton, siegeEngineMapFireAnimationName, 0, 1f, 0f, 1f);
						}
					}
				}
			}
		}

		private void TickMobilePartyVisual(float dt, ref int dirtyPartiesCount, ref PartyVisual[] dirtyPartiesList)
		{
			if (this.StrategicEntity == null)
			{
				return;
			}
			if (this.PartyBase.IsVisualDirty && (this._entityAlpha > 0f || this.TargetVisibility))
			{
				int num = Interlocked.Increment(ref dirtyPartiesCount);
				dirtyPartiesList[num] = this;
			}
			this._speed = this.PartyBase.MobileParty.Speed;
			if (this._entityAlpha > 0f && this.HumanAgentVisuals != null && !this.HumanAgentVisuals.GetEquipment()[4].IsEmpty)
			{
				this.HumanAgentVisuals.SetClothWindToWeaponAtIndex(-this.StrategicEntity.GetGlobalFrame().rotation.f, false, 4);
			}
			float num2 = ((this.MountAgentVisuals != null) ? 1.3f : 1f);
			float num3 = MathF.Min(0.25f * num2 * this._speed / 0.3f, 20f);
			bool flag = this.IsEntityMovingVisually();
			AgentVisuals humanAgentVisuals = this.HumanAgentVisuals;
			if (humanAgentVisuals != null)
			{
				humanAgentVisuals.Tick(this.MountAgentVisuals, dt, flag, num3);
			}
			AgentVisuals mountAgentVisuals = this.MountAgentVisuals;
			if (mountAgentVisuals != null)
			{
				mountAgentVisuals.Tick(null, dt, flag, num3);
			}
			AgentVisuals caravanMountAgentVisuals = this.CaravanMountAgentVisuals;
			if (caravanMountAgentVisuals != null)
			{
				caravanMountAgentVisuals.Tick(null, dt, flag, num3);
			}
			if (this.IsVisibleOrFadingOut())
			{
				MobileParty mobileParty = this.PartyBase.MobileParty;
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin = this.GetVisualPosition();
				if (mobileParty.Army != null && mobileParty.Army.LeaderParty.AttachedParties.Contains(mobileParty))
				{
					MatrixFrame frame = this.GetFrame();
					Vec2 vec = identity.origin.AsVec2 - frame.origin.AsVec2;
					if (vec.Length / dt > 20f)
					{
						identity.rotation.RotateAboutUp(this.PartyBase.AverageBearingRotation);
					}
					else if (mobileParty.CurrentSettlement == null)
					{
						float num4 = MBMath.LerpRadians(frame.rotation.f.AsVec2.RotationInRadians, (vec + Vec2.FromRotation(this.PartyBase.AverageBearingRotation) * 0.01f).RotationInRadians, 6f * dt, 0.03f * dt, 10f * dt);
						identity.rotation.RotateAboutUp(num4);
					}
					else
					{
						float rotationInRadians = frame.rotation.f.AsVec2.RotationInRadians;
						identity.rotation.RotateAboutUp(rotationInRadians);
					}
				}
				else if (mobileParty.CurrentSettlement == null)
				{
					identity.rotation.RotateAboutUp(this.PartyBase.AverageBearingRotation);
				}
				this.SetFrame(ref identity);
			}
		}

		public Vec3 GetVisualPosition()
		{
			float num = 0f;
			Vec2 zero = Vec2.Zero;
			if (this.PartyBase.IsMobile)
			{
				MobileParty mobileParty = this.PartyBase.MobileParty;
				zero..ctor(mobileParty.EventPositionAdder.x + mobileParty.ArmyPositionAdder.x + mobileParty.ErrorPosition.x, mobileParty.EventPositionAdder.y + mobileParty.ArmyPositionAdder.y + mobileParty.ErrorPosition.y);
			}
			Vec2 vec;
			vec..ctor(this.PartyBase.Position2D.x + zero.x, this.PartyBase.Position2D.y + zero.y);
			Campaign.Current.MapSceneWrapper.GetHeightAtPoint(vec, ref num);
			return new Vec3(vec, num, -1f);
		}

		private void CalculateDataAndDurationsForSiegeMachine(int machineSlotIndex, SiegeEngineType machineType, BattleSideEnum side, SiegeBombardTargets targetType, int targetSlotIndex, out PartyVisual.SiegeBombardmentData bombardmentData)
		{
			bombardmentData = default(PartyVisual.SiegeBombardmentData);
			MatrixFrame matrixFrame = ((side == null) ? this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[machineSlotIndex].GetGlobalFrame() : this._attackerRangedEngineSpawnEntities[machineSlotIndex].GetGlobalFrame());
			matrixFrame.rotation.MakeUnit();
			bombardmentData.ShooterGlobalFrame = matrixFrame;
			string siegeEngineMapFireAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapFireAnimationName(machineType, side);
			string siegeEngineMapReloadAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapReloadAnimationName(machineType, side);
			bombardmentData.ReloadDuration = MBAnimation.GetAnimationDuration(siegeEngineMapReloadAnimationName) * 0.25f;
			bombardmentData.AimingDuration = 0.25f;
			bombardmentData.RotationDuration = 0.4f;
			bombardmentData.FireDuration = MBAnimation.GetAnimationDuration(siegeEngineMapFireAnimationName) * 0.25f;
			float animationParameter = MBAnimation.GetAnimationParameter1(siegeEngineMapFireAnimationName);
			bombardmentData.MissileLaunchDuration = bombardmentData.FireDuration * animationParameter;
			bombardmentData.MissileSpeed = 14f;
			bombardmentData.Gravity = ((machineType == DefaultSiegeEngineTypes.Ballista || machineType == DefaultSiegeEngineTypes.FireBallista) ? 10f : 40f);
			if (targetType == 2)
			{
				bombardmentData.TargetPosition = ((side == 1) ? this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[targetSlotIndex].GetGlobalFrame().origin : this._attackerRangedEngineSpawnEntities[targetSlotIndex].GetGlobalFrame().origin);
			}
			else if (targetType == 1)
			{
				bombardmentData.TargetPosition = this._defenderBreachableWallEntitiesCacheForCurrentLevel[targetSlotIndex].GlobalPosition;
			}
			else if (targetSlotIndex == -1)
			{
				bombardmentData.TargetPosition = Vec3.Zero;
			}
			else
			{
				bombardmentData.TargetPosition = ((side == 1) ? this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[targetSlotIndex].GetGlobalFrame().origin : this._attackerRangedEngineSpawnEntities[targetSlotIndex].GetGlobalFrame().origin);
				bombardmentData.TargetPosition += (bombardmentData.TargetPosition - bombardmentData.ShooterGlobalFrame.origin).NormalizedCopy() * 2f;
				Campaign.Current.MapSceneWrapper.GetHeightAtPoint(bombardmentData.TargetPosition.AsVec2, ref bombardmentData.TargetPosition.z);
			}
			bombardmentData.TargetAlignedShooterGlobalFrame = bombardmentData.ShooterGlobalFrame;
			bombardmentData.TargetAlignedShooterGlobalFrame.rotation.f.AsVec2 = (bombardmentData.TargetPosition - bombardmentData.ShooterGlobalFrame.origin).AsVec2;
			bombardmentData.TargetAlignedShooterGlobalFrame.rotation.f.NormalizeWithoutChangingZ();
			bombardmentData.TargetAlignedShooterGlobalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			bombardmentData.LaunchGlobalPosition = bombardmentData.TargetAlignedShooterGlobalFrame.TransformToParent(this.MapScreen.PrefabEntityCache.GetLaunchEntitialFrameForSiegeEngine(machineType, side).origin);
			float lengthSquared = (bombardmentData.LaunchGlobalPosition.AsVec2 - bombardmentData.TargetPosition.AsVec2).LengthSquared;
			float num = MathF.Sqrt(lengthSquared);
			float num2 = bombardmentData.LaunchGlobalPosition.z - bombardmentData.TargetPosition.z;
			float num3 = bombardmentData.MissileSpeed * bombardmentData.MissileSpeed;
			float num4 = num3 * num3;
			float num5 = num4 - bombardmentData.Gravity * (bombardmentData.Gravity * lengthSquared - 2f * num2 * num3);
			if (num5 >= 0f)
			{
				bombardmentData.LaunchAngle = MathF.Atan((num3 - MathF.Sqrt(num5)) / (bombardmentData.Gravity * num));
			}
			else
			{
				bombardmentData.Gravity = 1f;
				num5 = num4 - bombardmentData.Gravity * (bombardmentData.Gravity * lengthSquared - 2f * num2 * num3);
				bombardmentData.LaunchAngle = MathF.Atan((num3 - MathF.Sqrt(num5)) / (bombardmentData.Gravity * num));
			}
			float num6 = bombardmentData.MissileSpeed * MathF.Cos(bombardmentData.LaunchAngle);
			bombardmentData.FlightDuration = num / num6;
			bombardmentData.TotalDuration = bombardmentData.RotationDuration + bombardmentData.ReloadDuration + bombardmentData.AimingDuration + bombardmentData.MissileLaunchDuration + bombardmentData.FlightDuration;
		}

		private void RemoveContourMesh()
		{
			if (this._contourMaskMesh != null)
			{
				this.MapScreen.ContourMaskEntity.RemoveComponentWithMesh(this._contourMaskMesh);
				this._contourMaskMesh = null;
			}
		}

		public void ReleaseResources()
		{
			this.RemoveSiege();
			this.ResetPartyIcon();
		}

		public void ValidateIsDirty(float realDt, float dt)
		{
			if (this.PartyBase.IsSettlement)
			{
				this.RefreshPartyIcon();
				PartyVisualManager.Current.RegisterFadingVisual(this);
				return;
			}
			if (this.PartyBase.MemberRoster.TotalManCount != 0)
			{
				this.RefreshPartyIcon();
				if ((this._entityAlpha < 1f && this.TargetVisibility) || (this._entityAlpha > 0f && !this.TargetVisibility))
				{
					PartyVisualManager.Current.RegisterFadingVisual(this);
					return;
				}
			}
			else
			{
				this.ResetPartyIcon();
			}
		}

		public void TickFadingState(float realDt, float dt)
		{
			if ((this._entityAlpha < 1f && this.TargetVisibility) || (this._entityAlpha > 0f && !this.TargetVisibility))
			{
				if (this.TargetVisibility)
				{
					if (this._entityAlpha <= 0f)
					{
						this.StrategicEntity.SetVisibilityExcludeParents(true);
						AgentVisuals humanAgentVisuals = this.HumanAgentVisuals;
						if (humanAgentVisuals != null)
						{
							GameEntity entity = humanAgentVisuals.GetEntity();
							if (entity != null)
							{
								entity.SetVisibilityExcludeParents(true);
							}
						}
						AgentVisuals mountAgentVisuals = this.MountAgentVisuals;
						if (mountAgentVisuals != null)
						{
							GameEntity entity2 = mountAgentVisuals.GetEntity();
							if (entity2 != null)
							{
								entity2.SetVisibilityExcludeParents(true);
							}
						}
						AgentVisuals caravanMountAgentVisuals = this.CaravanMountAgentVisuals;
						if (caravanMountAgentVisuals != null)
						{
							GameEntity entity3 = caravanMountAgentVisuals.GetEntity();
							if (entity3 != null)
							{
								entity3.SetVisibilityExcludeParents(true);
							}
						}
					}
					this._entityAlpha = MathF.Min(this._entityAlpha + realDt * 2f, 1f);
					this.StrategicEntity.SetAlpha(this._entityAlpha);
					AgentVisuals humanAgentVisuals2 = this.HumanAgentVisuals;
					if (humanAgentVisuals2 != null)
					{
						GameEntity entity4 = humanAgentVisuals2.GetEntity();
						if (entity4 != null)
						{
							entity4.SetAlpha(this._entityAlpha);
						}
					}
					AgentVisuals mountAgentVisuals2 = this.MountAgentVisuals;
					if (mountAgentVisuals2 != null)
					{
						GameEntity entity5 = mountAgentVisuals2.GetEntity();
						if (entity5 != null)
						{
							entity5.SetAlpha(this._entityAlpha);
						}
					}
					AgentVisuals caravanMountAgentVisuals2 = this.CaravanMountAgentVisuals;
					if (caravanMountAgentVisuals2 != null)
					{
						GameEntity entity6 = caravanMountAgentVisuals2.GetEntity();
						if (entity6 != null)
						{
							entity6.SetAlpha(this._entityAlpha);
						}
					}
					this.StrategicEntity.EntityFlags &= -536870913;
					return;
				}
				this._entityAlpha = MathF.Max(this._entityAlpha - realDt * 2f, 0f);
				this.StrategicEntity.SetAlpha(this._entityAlpha);
				AgentVisuals humanAgentVisuals3 = this.HumanAgentVisuals;
				if (humanAgentVisuals3 != null)
				{
					GameEntity entity7 = humanAgentVisuals3.GetEntity();
					if (entity7 != null)
					{
						entity7.SetAlpha(this._entityAlpha);
					}
				}
				AgentVisuals mountAgentVisuals3 = this.MountAgentVisuals;
				if (mountAgentVisuals3 != null)
				{
					GameEntity entity8 = mountAgentVisuals3.GetEntity();
					if (entity8 != null)
					{
						entity8.SetAlpha(this._entityAlpha);
					}
				}
				AgentVisuals caravanMountAgentVisuals3 = this.CaravanMountAgentVisuals;
				if (caravanMountAgentVisuals3 != null)
				{
					GameEntity entity9 = caravanMountAgentVisuals3.GetEntity();
					if (entity9 != null)
					{
						entity9.SetAlpha(this._entityAlpha);
					}
				}
				if (this._entityAlpha <= 0f)
				{
					this.StrategicEntity.SetVisibilityExcludeParents(false);
					AgentVisuals humanAgentVisuals4 = this.HumanAgentVisuals;
					if (humanAgentVisuals4 != null)
					{
						GameEntity entity10 = humanAgentVisuals4.GetEntity();
						if (entity10 != null)
						{
							entity10.SetVisibilityExcludeParents(false);
						}
					}
					AgentVisuals mountAgentVisuals4 = this.MountAgentVisuals;
					if (mountAgentVisuals4 != null)
					{
						GameEntity entity11 = mountAgentVisuals4.GetEntity();
						if (entity11 != null)
						{
							entity11.SetVisibilityExcludeParents(false);
						}
					}
					AgentVisuals caravanMountAgentVisuals4 = this.CaravanMountAgentVisuals;
					if (caravanMountAgentVisuals4 != null)
					{
						GameEntity entity12 = caravanMountAgentVisuals4.GetEntity();
						if (entity12 != null)
						{
							entity12.SetVisibilityExcludeParents(false);
						}
					}
					this.StrategicEntity.EntityFlags |= 536870912;
					return;
				}
			}
			else
			{
				PartyVisualManager.Current.UnRegisterFadingVisual(this);
			}
		}

		public void ResetPartyIcon()
		{
			if (this.StrategicEntity != null)
			{
				this.RemoveContourMesh();
			}
			if (this.HumanAgentVisuals != null)
			{
				this.HumanAgentVisuals.Reset();
				this.HumanAgentVisuals = null;
			}
			if (this.MountAgentVisuals != null)
			{
				this.MountAgentVisuals.Reset();
				this.MountAgentVisuals = null;
			}
			if (this.CaravanMountAgentVisuals != null)
			{
				this.CaravanMountAgentVisuals.Reset();
				this.CaravanMountAgentVisuals = null;
			}
			if (this.StrategicEntity != null)
			{
				if ((this.StrategicEntity.EntityFlags & 268435456) != null)
				{
					this.StrategicEntity.RemoveFromPredisplayEntity();
				}
				this.StrategicEntity.ClearComponents();
			}
			PartyVisualManager.Current.UnRegisterFadingVisual(this);
		}

		private void RefreshPartyIcon()
		{
			if (this.PartyBase.IsVisualDirty)
			{
				this.PartyBase.OnVisualsUpdated();
				bool flag = true;
				bool flag2 = true;
				if (!this.PartyBase.IsSettlement)
				{
					this.ResetPartyIcon();
					MatrixFrame circleLocalFrame = this.CircleLocalFrame;
					circleLocalFrame.origin = Vec3.Zero;
					this.CircleLocalFrame = circleLocalFrame;
				}
				else
				{
					this.RemoveSiege();
					this.StrategicEntity.RemoveAllParticleSystems();
					this.StrategicEntity.EntityFlags |= 536870912;
				}
				MobileParty mobileParty = this.PartyBase.MobileParty;
				if (((mobileParty != null) ? mobileParty.CurrentSettlement : null) != null)
				{
					Dictionary<int, List<GameEntity>> gateBannerEntitiesWithLevels = PartyVisualManager.Current.GetVisualOfParty(this.PartyBase.MobileParty.CurrentSettlement.Party)._gateBannerEntitiesWithLevels;
					if (!this.PartyBase.MobileParty.MapFaction.IsAtWarWith(this.PartyBase.MobileParty.CurrentSettlement.MapFaction) && gateBannerEntitiesWithLevels != null && !Extensions.IsEmpty<KeyValuePair<int, List<GameEntity>>>(gateBannerEntitiesWithLevels))
					{
						Hero leaderHero = this.PartyBase.LeaderHero;
						if (((leaderHero != null) ? leaderHero.ClanBanner : null) != null)
						{
							string text = this.PartyBase.LeaderHero.ClanBanner.Serialize();
							if (string.IsNullOrEmpty(text))
							{
								goto IL_68F;
							}
							int num = 0;
							foreach (MobileParty mobileParty2 in this.PartyBase.MobileParty.CurrentSettlement.Parties)
							{
								if (mobileParty2 == this.PartyBase.MobileParty)
								{
									break;
								}
								Hero leaderHero2 = mobileParty2.LeaderHero;
								if (((leaderHero2 != null) ? leaderHero2.ClanBanner : null) != null)
								{
									num++;
								}
							}
							MatrixFrame matrixFrame = MatrixFrame.Identity;
							int wallLevel = this.PartyBase.MobileParty.CurrentSettlement.Town.GetWallLevel();
							int count = gateBannerEntitiesWithLevels[wallLevel].Count;
							if (count == 0)
							{
								Debug.FailedAssert(string.Format("{0} - has no Banner Entities at level {1}.", this.PartyBase.MobileParty.CurrentSettlement.Name, wallLevel), "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\PartyVisual.cs", "RefreshPartyIcon", 1060);
							}
							GameEntity gameEntity = gateBannerEntitiesWithLevels[wallLevel][num % count];
							GameEntity child = gameEntity.GetChild(0);
							MatrixFrame matrixFrame2 = ((child != null) ? child.GetGlobalFrame() : gameEntity.GetGlobalFrame());
							num /= count;
							int num2 = this.PartyBase.MobileParty.CurrentSettlement.Parties.Count(delegate(MobileParty p)
							{
								Hero leaderHero3 = p.LeaderHero;
								return ((leaderHero3 != null) ? leaderHero3.ClanBanner : null) != null;
							});
							float num3 = 0.75f / (float)MathF.Max(1, num2 / (count * 2));
							int num4 = ((num % 2 == 0) ? (-1) : 1);
							Vec3 vec = matrixFrame2.rotation.f / 2f * (float)num4;
							if (vec.Length < matrixFrame2.rotation.s.Length)
							{
								vec = matrixFrame2.rotation.s / 2f * (float)num4;
							}
							matrixFrame.origin = matrixFrame2.origin + vec * (float)((num + 1) / 2) * (float)(num % 2 * 2 - 1) * num3 * (float)num4;
							MatrixFrame matrixFrame3 = this.StrategicEntity.GetGlobalFrame();
							matrixFrame.origin = matrixFrame3.TransformToLocal(matrixFrame.origin);
							float num5 = MBMath.Map((float)this.PartyBase.NumberOfAllMembers / 400f * ((this.PartyBase.MobileParty.Army != null && this.PartyBase.MobileParty.Army.LeaderParty == this.PartyBase.MobileParty) ? 1.25f : 1f), 0f, 1f, 0.2f, 0.5f);
							matrixFrame = matrixFrame.Elevate(-num5);
							matrixFrame.rotation.ApplyScaleLocal(num5);
							matrixFrame3 = this.StrategicEntity.GetGlobalFrame();
							matrixFrame.rotation = matrixFrame3.rotation.TransformToLocal(matrixFrame.rotation);
							GameEntityPhysicsExtensions.AddSphereAsBody(this.StrategicEntity, matrixFrame.origin + Vec3.Up * 0.3f, 0.15f, 0);
							flag = false;
							string text2 = "campaign_flag";
							if (this._cachedBannerComponent.Item1 == text + text2)
							{
								this._cachedBannerComponent.Item2.GetFirstMetaMesh().Frame = matrixFrame;
								this.StrategicEntity.AddComponent(this._cachedBannerComponent.Item2);
								goto IL_68F;
							}
							MetaMesh bannerOfCharacter = PartyVisual.GetBannerOfCharacter(new Banner(text), text2);
							bannerOfCharacter.Frame = matrixFrame;
							int componentCount = this.StrategicEntity.GetComponentCount(3);
							this.StrategicEntity.AddMultiMesh(bannerOfCharacter, true);
							if (this.StrategicEntity.GetComponentCount(3) > componentCount)
							{
								this._cachedBannerComponent.Item1 = text + text2;
								this._cachedBannerComponent.Item2 = this.StrategicEntity.GetComponentAtIndex(componentCount, 3);
								goto IL_68F;
							}
							goto IL_68F;
						}
					}
					GameEntityPhysicsExtensions.RemovePhysics(this.StrategicEntity, false);
				}
				else
				{
					this.IsEnemy = this.PartyBase.MapFaction != null && FactionManager.IsAtWarAgainstFaction(this.PartyBase.MapFaction, Hero.MainHero.MapFaction);
					this.IsFriendly = this.PartyBase.MapFaction != null && FactionManager.IsAlliedWithFaction(this.PartyBase.MapFaction, Hero.MainHero.MapFaction);
					this.InitializePartyCollider(this.PartyBase);
					if (this.PartyBase.IsSettlement)
					{
						if (this.PartyBase.Settlement.IsFortification)
						{
							this.UpdateDefenderSiegeEntitiesCache();
						}
						this.AddSiegeIconComponents(this.PartyBase);
						this.SetSettlementLevelVisibility();
						this.RefreshWallState();
						this.RefreshTownPhysicalEntitiesState(this.PartyBase);
						this.RefreshSiegePreparations(this.PartyBase);
						if (this.PartyBase.Settlement.IsVillage)
						{
							MapEvent mapEvent = this.PartyBase.MapEvent;
							if (mapEvent != null && mapEvent.IsRaid)
							{
								this.StrategicEntity.EntityFlags &= -536870913;
								this.StrategicEntity.AddParticleSystemComponent("psys_fire_smoke_env_point");
							}
							else if (this.PartyBase.Settlement.IsRaided)
							{
								this.StrategicEntity.EntityFlags &= -536870913;
								this.StrategicEntity.AddParticleSystemComponent("map_icon_village_plunder_fx");
							}
						}
					}
					else
					{
						this.AddMobileIconComponents(this.PartyBase, ref flag2, ref flag2);
					}
				}
				IL_68F:
				if (flag)
				{
					this._cachedBannerComponent = new ValueTuple<string, GameEntityComponent>(null, null);
				}
				if (flag2)
				{
					this._cachedBannerEntity = new ValueTuple<string, GameEntity>(null, null);
				}
				this.StrategicEntity.CheckResources(true, false);
			}
		}

		private void RemoveSiege()
		{
			foreach (ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity> valueTuple in this._siegeRangedMachineEntities)
			{
				this.StrategicEntity.RemoveChild(valueTuple.Item1, false, false, true, 36);
			}
			foreach (ValueTuple<GameEntity, BattleSideEnum, int> valueTuple2 in this._siegeMissileEntities)
			{
				this.StrategicEntity.RemoveChild(valueTuple2.Item1, false, false, true, 37);
			}
			foreach (ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity> valueTuple3 in this._siegeMeleeMachineEntities)
			{
				this.StrategicEntity.RemoveChild(valueTuple3.Item1, false, false, true, 38);
			}
			this._siegeRangedMachineEntities.Clear();
			this._siegeMeleeMachineEntities.Clear();
			this._siegeMissileEntities.Clear();
		}

		private void RefreshSiegePreparations(PartyBase party)
		{
			List<GameEntity> list = new List<GameEntity>();
			this.StrategicEntity.GetChildrenRecursive(ref list);
			List<GameEntity> list2 = list.FindAll((GameEntity x) => x.HasTag("siege_preparation"));
			bool flag = false;
			if (party.Settlement != null && party.Settlement.IsUnderSiege)
			{
				SiegeEvent.SiegeEngineConstructionProgress siegePreparations = party.Settlement.SiegeEvent.GetSiegeEventSide(1).SiegeEngines.SiegePreparations;
				if (siegePreparations != null && siegePreparations.Progress >= 1f)
				{
					flag = true;
					foreach (GameEntity gameEntity in list2)
					{
						gameEntity.SetVisibilityExcludeParents(true);
					}
				}
			}
			if (!flag)
			{
				foreach (GameEntity gameEntity2 in list2)
				{
					gameEntity2.SetVisibilityExcludeParents(false);
				}
			}
		}

		private void AddSiegeIconComponents(PartyBase party)
		{
			if (party.Settlement.IsUnderSiege)
			{
				int num = -1;
				if (party.Settlement.SiegeEvent.BesiegedSettlement.IsTown || party.Settlement.SiegeEvent.BesiegedSettlement.IsCastle)
				{
					num = party.Settlement.SiegeEvent.BesiegedSettlement.Town.GetWallLevel();
				}
				SiegeEvent.SiegeEngineConstructionProgress[] deployedRangedSiegeEngines = party.Settlement.SiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedRangedSiegeEngines;
				for (int i = 0; i < deployedRangedSiegeEngines.Length; i++)
				{
					SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress = deployedRangedSiegeEngines[i];
					if (siegeEngineConstructionProgress != null && siegeEngineConstructionProgress.IsConstructed && i < this._attackerRangedEngineSpawnEntities.Length)
					{
						MatrixFrame globalFrame = this._attackerRangedEngineSpawnEntities[i].GetGlobalFrame();
						globalFrame.rotation.MakeUnit();
						this.AddSiegeMachine(deployedRangedSiegeEngines[i].SiegeEngine, globalFrame, 1, num, i);
					}
				}
				SiegeEvent.SiegeEngineConstructionProgress[] deployedMeleeSiegeEngines = party.Settlement.SiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedMeleeSiegeEngines;
				for (int j = 0; j < deployedMeleeSiegeEngines.Length; j++)
				{
					SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress2 = deployedMeleeSiegeEngines[j];
					if (siegeEngineConstructionProgress2 != null && siegeEngineConstructionProgress2.IsConstructed)
					{
						if (deployedMeleeSiegeEngines[j].SiegeEngine == DefaultSiegeEngineTypes.SiegeTower)
						{
							int num2 = j - this._attackerBatteringRamSpawnEntities.Length;
							if (num2 >= 0)
							{
								MatrixFrame globalFrame2 = this._attackerSiegeTowerSpawnEntities[num2].GetGlobalFrame();
								globalFrame2.rotation.MakeUnit();
								this.AddSiegeMachine(deployedMeleeSiegeEngines[j].SiegeEngine, globalFrame2, 1, num, j);
							}
						}
						else if (deployedMeleeSiegeEngines[j].SiegeEngine == DefaultSiegeEngineTypes.Ram || deployedMeleeSiegeEngines[j].SiegeEngine == DefaultSiegeEngineTypes.ImprovedRam)
						{
							int num3 = j;
							if (num3 >= 0)
							{
								MatrixFrame globalFrame3 = this._attackerBatteringRamSpawnEntities[num3].GetGlobalFrame();
								globalFrame3.rotation.MakeUnit();
								this.AddSiegeMachine(deployedMeleeSiegeEngines[j].SiegeEngine, globalFrame3, 1, num, j);
							}
						}
					}
				}
				SiegeEvent.SiegeEngineConstructionProgress[] deployedRangedSiegeEngines2 = party.Settlement.SiegeEvent.GetSiegeEventSide(0).SiegeEngines.DeployedRangedSiegeEngines;
				for (int k = 0; k < deployedRangedSiegeEngines2.Length; k++)
				{
					SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress3 = deployedRangedSiegeEngines2[k];
					if (siegeEngineConstructionProgress3 != null && siegeEngineConstructionProgress3.IsConstructed && k < this._defenderBreachableWallEntitiesCacheForCurrentLevel.Length)
					{
						MatrixFrame globalFrame4 = this._defenderBreachableWallEntitiesCacheForCurrentLevel[k].GetGlobalFrame();
						globalFrame4.rotation.MakeUnit();
						this.AddSiegeMachine(deployedRangedSiegeEngines2[k].SiegeEngine, globalFrame4, 0, num, k);
					}
				}
				for (int l = 0; l < 2; l++)
				{
					BattleSideEnum battleSideEnum = ((l == 0) ? 1 : 0);
					MBReadOnlyList<SiegeEvent.SiegeEngineMissile> siegeEngineMissiles = party.Settlement.SiegeEvent.GetSiegeEventSide(battleSideEnum).SiegeEngineMissiles;
					for (int m = 0; m < siegeEngineMissiles.Count; m++)
					{
						this.AddSiegeMissile(siegeEngineMissiles[m].ShooterSiegeEngineType, this.StrategicEntity.GetGlobalFrame(), battleSideEnum, m);
					}
				}
			}
		}

		private void AddSiegeMachine(SiegeEngineType type, MatrixFrame globalFrame, BattleSideEnum side, int wallLevel, int slotIndex)
		{
			string siegeEngineMapPrefabName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapPrefabName(type, wallLevel, side);
			GameEntity gameEntity = GameEntity.Instantiate(this.MapScene, siegeEngineMapPrefabName, true);
			if (gameEntity != null)
			{
				this.StrategicEntity.AddChild(gameEntity, false);
				MatrixFrame matrixFrame;
				gameEntity.GetFrame(ref matrixFrame);
				GameEntity gameEntity2 = gameEntity;
				MatrixFrame matrixFrame2 = globalFrame.TransformToParent(matrixFrame);
				gameEntity2.SetGlobalFrame(ref matrixFrame2);
				List<GameEntity> list = new List<GameEntity>();
				gameEntity.GetChildrenRecursive(ref list);
				GameEntity gameEntity3 = null;
				if (list.Any((GameEntity entity) => entity.HasTag("siege_machine_mapicon_skeleton")))
				{
					GameEntity gameEntity4 = list.Find((GameEntity entity) => entity.HasTag("siege_machine_mapicon_skeleton"));
					if (gameEntity4.Skeleton != null)
					{
						gameEntity3 = gameEntity4;
						string siegeEngineMapFireAnimationName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapFireAnimationName(type, side);
						MBSkeletonExtensions.SetAnimationAtChannel(gameEntity3.Skeleton, siegeEngineMapFireAnimationName, 0, 1f, 0f, 1f);
					}
				}
				if (type.IsRanged)
				{
					this._siegeRangedMachineEntities.Add(ValueTuple.Create<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity>(gameEntity, side, slotIndex, globalFrame, gameEntity3));
					return;
				}
				this._siegeMeleeMachineEntities.Add(ValueTuple.Create<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity>(gameEntity, side, slotIndex, globalFrame, gameEntity3));
			}
		}

		private void AddSiegeMissile(SiegeEngineType type, MatrixFrame globalFrame, BattleSideEnum side, int missileIndex)
		{
			string siegeEngineMapProjectilePrefabName = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineMapProjectilePrefabName(type);
			GameEntity gameEntity = GameEntity.Instantiate(this.MapScene, siegeEngineMapProjectilePrefabName, true);
			if (gameEntity != null)
			{
				this._siegeMissileEntities.Add(ValueTuple.Create<GameEntity, BattleSideEnum, int>(gameEntity, side, missileIndex));
				this.StrategicEntity.AddChild(gameEntity, false);
				this.StrategicEntity.EntityFlags &= -536870913;
				MatrixFrame matrixFrame;
				gameEntity.GetFrame(ref matrixFrame);
				GameEntity gameEntity2 = gameEntity;
				MatrixFrame matrixFrame2 = globalFrame.TransformToParent(matrixFrame);
				gameEntity2.SetGlobalFrame(ref matrixFrame2);
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		private void AddMobileIconComponents(PartyBase party, ref bool clearBannerComponentCache, ref bool clearBannerEntityCache)
		{
			uint num = (FactionManager.IsAtWarAgainstFaction(party.MapFaction, Hero.MainHero.MapFaction) ? 4294905856U : 4278206719U);
			Settlement besiegedSettlement = party.MobileParty.BesiegedSettlement;
			if (((besiegedSettlement != null) ? besiegedSettlement.SiegeEvent : null) != null && party.MobileParty.BesiegedSettlement.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(party, 5))
			{
				GameEntity gameEntity = GameEntity.CreateEmpty(this.StrategicEntity.Scene, true);
				gameEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_siege_camp_tent", true, false), true);
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.ApplyScaleLocal(1.2f);
				gameEntity.SetFrame(ref identity);
				string text = null;
				Hero leaderHero = party.LeaderHero;
				if (((leaderHero != null) ? leaderHero.ClanBanner : null) != null)
				{
					text = party.LeaderHero.ClanBanner.Serialize();
				}
				bool flag = party.MobileParty.Army != null && party.MobileParty.Army.LeaderParty == party.MobileParty;
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.origin.z = identity2.origin.z + (flag ? 0.2f : 0.15f);
				identity2.rotation.RotateAboutUp(1.5707964f);
				float num2 = MBMath.Map(party.TotalStrength / 500f * ((party.MobileParty.Army != null && flag) ? 1f : 0.8f), 0f, 1f, 0.15f, 0.5f);
				identity2.rotation.ApplyScaleLocal(num2);
				if (!string.IsNullOrEmpty(text))
				{
					clearBannerComponentCache = false;
					string text2 = "campaign_flag";
					if (this._cachedBannerComponent.Item1 == text + text2)
					{
						this._cachedBannerComponent.Item2.GetFirstMetaMesh().Frame = identity2;
						this.StrategicEntity.AddComponent(this._cachedBannerComponent.Item2);
					}
					else
					{
						MetaMesh bannerOfCharacter = PartyVisual.GetBannerOfCharacter(new Banner(text), text2);
						bannerOfCharacter.Frame = identity2;
						int componentCount = gameEntity.GetComponentCount(3);
						gameEntity.AddMultiMesh(bannerOfCharacter, true);
						if (gameEntity.GetComponentCount(3) > componentCount)
						{
							this._cachedBannerComponent.Item1 = text + text2;
							this._cachedBannerComponent.Item2 = gameEntity.GetComponentAtIndex(componentCount, 3);
						}
					}
				}
				this.StrategicEntity.AddChild(gameEntity, false);
				return;
			}
			if (PartyBaseHelper.GetVisualPartyLeader(party) != null)
			{
				string text3 = null;
				Hero leaderHero2 = party.LeaderHero;
				if (((leaderHero2 != null) ? leaderHero2.ClanBanner : null) != null)
				{
					text3 = party.LeaderHero.ClanBanner.Serialize();
				}
				ActionIndexCache act_none = ActionIndexCache.act_none;
				ActionIndexCache act_none2 = ActionIndexCache.act_none;
				MapEvent mapEvent = ((party.MobileParty.Army != null && party.MobileParty.Army.DoesLeaderPartyAndAttachedPartiesContain(party.MobileParty)) ? party.MobileParty.Army.LeaderParty.MapEvent : party.MapEvent);
				int num3;
				this.GetMeleeWeaponToWield(party, out num3);
				if (mapEvent != null && (mapEvent.EventType == 1 || (mapEvent.EventType == 2 && party.MapEventSide == mapEvent.AttackerSide) || mapEvent.EventType == 8 || mapEvent.EventType == 7))
				{
					PartyVisual.GetPartyBattleAnimation(party, num3, out act_none, out act_none2);
				}
				IFaction mapFaction = party.MapFaction;
				uint num4 = ((mapFaction != null) ? mapFaction.Color : 4291609515U);
				IFaction mapFaction2 = party.MapFaction;
				uint num5 = ((mapFaction2 != null) ? mapFaction2.Color2 : 4291609515U);
				this.AddCharacterToPartyIcon(PartyBaseHelper.GetVisualPartyLeader(party), num, text3, num3, num4, num5, act_none, act_none2, MBRandom.NondeterministicRandomFloat * 0.7f, ref clearBannerEntityCache);
				if (party.IsMobile)
				{
					string text4;
					string text5;
					party.MobileParty.GetMountAndHarnessVisualIdsForPartyIcon(ref text4, ref text5);
					if (!string.IsNullOrEmpty(text4))
					{
						this.AddMountToPartyIcon(new Vec3(0.3f, -0.25f, 0f, -1f), text4, text5, num, PartyBaseHelper.GetVisualPartyLeader(party));
					}
				}
			}
		}

		private void GetMeleeWeaponToWield(PartyBase party, out int wieldedItemIndex)
		{
			wieldedItemIndex = -1;
			CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(party);
			if (visualPartyLeader != null)
			{
				for (int i = 0; i < 5; i++)
				{
					if (visualPartyLeader.Equipment[i].Item != null && visualPartyLeader.Equipment[i].Item.PrimaryWeapon.IsMeleeWeapon)
					{
						wieldedItemIndex = i;
						return;
					}
				}
			}
		}

		private static void GetPartyBattleAnimation(PartyBase party, int wieldedItemIndex, out ActionIndexCache leaderAction, out ActionIndexCache mountAction)
		{
			leaderAction = ActionIndexCache.act_none;
			mountAction = ActionIndexCache.act_none;
			if (party.MobileParty.Army == null || !party.MobileParty.Army.DoesLeaderPartyAndAttachedPartiesContain(party.MobileParty))
			{
				MapEvent mapEvent = party.MapEvent;
			}
			else
			{
				MapEvent mapEvent2 = party.MobileParty.Army.LeaderParty.MapEvent;
			}
			CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(party);
			MapEvent mapEvent3 = party.MapEvent;
			if (((mapEvent3 != null) ? mapEvent3.MapEventSettlement : null) != null && visualPartyLeader != null && !visualPartyLeader.HasMount())
			{
				leaderAction = PartyVisual._raidOnFoot;
				return;
			}
			if (wieldedItemIndex > -1 && ((visualPartyLeader != null) ? visualPartyLeader.Equipment[wieldedItemIndex].Item : null) != null)
			{
				WeaponComponent weaponComponent = visualPartyLeader.Equipment[wieldedItemIndex].Item.WeaponComponent;
				if (weaponComponent != null && weaponComponent.PrimaryWeapon.IsMeleeWeapon)
				{
					if (visualPartyLeader.HasMount())
					{
						if (visualPartyLeader.Equipment[10].Item.HorseComponent.Monster.MonsterUsage == "camel")
						{
							if (weaponComponent.GetItemType() == 2 || weaponComponent.GetItemType() == 3)
							{
								leaderAction = PartyVisual._camelSwordAttack;
								mountAction = PartyVisual._swordAttackMount;
							}
							else if (weaponComponent.GetItemType() == 4)
							{
								if (weaponComponent.PrimaryWeapon.SwingDamageType == -1)
								{
									leaderAction = PartyVisual._camelSpearAttack;
									mountAction = PartyVisual._spearAttackMount;
								}
								else if (weaponComponent.PrimaryWeapon.WeaponClass == 10)
								{
									leaderAction = PartyVisual._camel1HandedSwingAttack;
									mountAction = PartyVisual._swingAttackMount;
								}
								else
								{
									leaderAction = PartyVisual._camel2HandedSwingAttack;
									mountAction = PartyVisual._swingAttackMount;
								}
							}
						}
						else if (weaponComponent.GetItemType() == 2 || weaponComponent.GetItemType() == 3)
						{
							leaderAction = PartyVisual._horseSwordAttack;
							mountAction = PartyVisual._swordAttackMount;
						}
						else if (weaponComponent.GetItemType() == 4)
						{
							if (weaponComponent.PrimaryWeapon.SwingDamageType == -1)
							{
								leaderAction = PartyVisual._horseSpearAttack;
								mountAction = PartyVisual._spearAttackMount;
							}
							else if (weaponComponent.PrimaryWeapon.WeaponClass == 10)
							{
								leaderAction = PartyVisual._horse1HandedSwingAttack;
								mountAction = PartyVisual._swingAttackMount;
							}
							else
							{
								leaderAction = PartyVisual._horse2HandedSwingAttack;
								mountAction = PartyVisual._swingAttackMount;
							}
						}
					}
					else if (weaponComponent.PrimaryWeapon.WeaponClass == 4 || weaponComponent.PrimaryWeapon.WeaponClass == 6 || weaponComponent.PrimaryWeapon.WeaponClass == 2)
					{
						leaderAction = PartyVisual._attack1H;
					}
					else if (weaponComponent.PrimaryWeapon.WeaponClass == 5 || weaponComponent.PrimaryWeapon.WeaponClass == 8 || weaponComponent.PrimaryWeapon.WeaponClass == 3)
					{
						leaderAction = PartyVisual._attack2H;
					}
					else if (weaponComponent.PrimaryWeapon.WeaponClass == 9 || weaponComponent.PrimaryWeapon.WeaponClass == 10)
					{
						leaderAction = PartyVisual._attackSpear1HOr2H;
					}
				}
			}
			if (leaderAction == ActionIndexCache.act_none)
			{
				if (visualPartyLeader.HasMount())
				{
					if (visualPartyLeader.Equipment[10].Item.HorseComponent.Monster.MonsterUsage == "camel")
					{
						leaderAction = PartyVisual._camelUnarmedAttack;
					}
					else
					{
						leaderAction = PartyVisual._horseUnarmedAttack;
					}
					mountAction = PartyVisual._unarmedAttackMount;
					return;
				}
				leaderAction = PartyVisual._attackUnarmed;
			}
		}

		public void RefreshWallState()
		{
			if (this._defenderBreachableWallEntitiesForAllLevels != null)
			{
				PartyBase partyBase = this.PartyBase;
				MBReadOnlyList<float> mbreadOnlyList;
				if (((partyBase != null) ? partyBase.Settlement : null) == null || (this.PartyBase.Settlement != null && !this.PartyBase.Settlement.IsFortification))
				{
					mbreadOnlyList = null;
				}
				else
				{
					mbreadOnlyList = this.PartyBase.Settlement.SettlementWallSectionHitPointsRatioList;
				}
				if (mbreadOnlyList != null)
				{
					if (mbreadOnlyList.Count == 0)
					{
						Debug.FailedAssert(string.Concat(new object[]
						{
							"Town (",
							this.PartyBase.Settlement.Name.ToString(),
							") doesn't have wall entities defined for it's current level(",
							this.PartyBase.Settlement.Town.GetWallLevel(),
							")"
						}), "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\PartyVisual.cs", "RefreshWallState", 1656);
						return;
					}
					for (int i = 0; i < this._defenderBreachableWallEntitiesForAllLevels.Length; i++)
					{
						bool flag = mbreadOnlyList[i % mbreadOnlyList.Count] <= 0f;
						foreach (GameEntity gameEntity in this._defenderBreachableWallEntitiesForAllLevels[i].GetChildren())
						{
							if (gameEntity.HasTag("map_solid_wall"))
							{
								gameEntity.SetVisibilityExcludeParents(!flag);
							}
							else if (gameEntity.HasTag("map_broken_wall"))
							{
								gameEntity.SetVisibilityExcludeParents(flag);
							}
						}
					}
				}
			}
		}

		public void RefreshTownPhysicalEntitiesState(PartyBase party)
		{
			if (((party != null) ? party.Settlement : null) != null && party.Settlement.IsFortification && this.TownPhysicalEntities != null)
			{
				if (PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSiegeEvent.BesiegedSettlement == party.Settlement)
				{
					this.TownPhysicalEntities.ForEach(delegate(GameEntity p)
					{
						p.AddBodyFlags(1, true);
					});
					return;
				}
				this.TownPhysicalEntities.ForEach(delegate(GameEntity p)
				{
					p.RemoveBodyFlags(1, true);
				});
			}
		}

		public void SetLevelMask(uint newMask)
		{
			this._currentLevelMask = newMask;
			this.PartyBase.SetVisualAsDirty();
		}

		public void RefreshLevelMask()
		{
			if (this.PartyBase.IsSettlement)
			{
				uint num = 0U;
				if (this.PartyBase.Settlement.IsVillage)
				{
					if (this.PartyBase.Settlement.Village.VillageState == 4)
					{
						num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("looted");
					}
					else
					{
						num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("civilian");
					}
					num |= PartyVisual.GetLevelOfProduction(this.PartyBase.Settlement);
				}
				else if (this.PartyBase.Settlement.IsTown || this.PartyBase.Settlement.IsCastle)
				{
					if (this.PartyBase.Settlement.Town.GetWallLevel() == 1)
					{
						num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_1");
					}
					else if (this.PartyBase.Settlement.Town.GetWallLevel() == 2)
					{
						num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_2");
					}
					else if (this.PartyBase.Settlement.Town.GetWallLevel() == 3)
					{
						num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_3");
					}
					if (this.PartyBase.Settlement.SiegeEvent != null)
					{
						num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("siege");
					}
					else
					{
						num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("civilian");
					}
				}
				else if (this.PartyBase.Settlement.IsHideout)
				{
					num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_1");
				}
				if (this._currentLevelMask != num)
				{
					this.SetLevelMask(num);
				}
			}
			this.PartyBase.OnLevelMaskUpdated();
		}

		private static uint GetLevelOfProduction(Settlement settlement)
		{
			uint num = 0U;
			if (settlement.IsVillage)
			{
				if (settlement.Village.Hearth < 200f)
				{
					num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_1");
				}
				else if (settlement.Village.Hearth < 600f)
				{
					num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_2");
				}
				else
				{
					num |= Campaign.Current.MapSceneWrapper.GetSceneLevel("level_3");
				}
			}
			return num;
		}

		private void SetSettlementLevelVisibility()
		{
			List<GameEntity> list = new List<GameEntity>();
			this.StrategicEntity.GetChildrenRecursive(ref list);
			foreach (GameEntity gameEntity in list)
			{
				if ((gameEntity.GetUpgradeLevelMask() & this._currentLevelMask) == this._currentLevelMask)
				{
					gameEntity.SetVisibilityExcludeParents(true);
					GameEntityPhysicsExtensions.SetPhysicsState(gameEntity, true, true);
				}
				else
				{
					gameEntity.SetVisibilityExcludeParents(false);
					GameEntityPhysicsExtensions.SetPhysicsState(gameEntity, false, true);
				}
			}
		}

		private void InitializePartyCollider(PartyBase party)
		{
			if (this.StrategicEntity != null && party.IsMobile)
			{
				GameEntityPhysicsExtensions.AddSphereAsBody(this.StrategicEntity, new Vec3(0f, 0f, 0f, -1f), 0.5f, 144);
			}
		}

		public void OnPartyRemoved()
		{
			if (this.StrategicEntity != null)
			{
				MapScreen.VisualsOfEntities.Remove(this.StrategicEntity.Pointer);
				foreach (GameEntity gameEntity in this.StrategicEntity.GetChildren())
				{
					MapScreen.VisualsOfEntities.Remove(gameEntity.Pointer);
				}
				this.ReleaseResources();
				this.StrategicEntity.Remove(111);
			}
		}

		internal void OnMapHoverSiegeEngine(MatrixFrame engineFrame)
		{
			if (PlayerSiege.PlayerSiegeEvent == null)
			{
				return;
			}
			for (int i = 0; i < this._attackerBatteringRamSpawnEntities.Length; i++)
			{
				MatrixFrame globalFrame = this._attackerBatteringRamSpawnEntities[i].GetGlobalFrame();
				if (globalFrame.NearlyEquals(engineFrame, 1E-05f))
				{
					if (this._hoveredSiegeEntityFrame != globalFrame)
					{
						SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedMeleeSiegeEngines[i];
						InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(siegeEngineConstructionProgress) });
					}
					return;
				}
			}
			for (int j = 0; j < this._attackerSiegeTowerSpawnEntities.Length; j++)
			{
				MatrixFrame globalFrame2 = this._attackerSiegeTowerSpawnEntities[j].GetGlobalFrame();
				if (globalFrame2.NearlyEquals(engineFrame, 1E-05f))
				{
					if (this._hoveredSiegeEntityFrame != globalFrame2)
					{
						SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress2 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedMeleeSiegeEngines[this._attackerBatteringRamSpawnEntities.Length + j];
						InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(siegeEngineConstructionProgress2) });
					}
					return;
				}
			}
			for (int k = 0; k < this._attackerRangedEngineSpawnEntities.Length; k++)
			{
				MatrixFrame globalFrame3 = this._attackerRangedEngineSpawnEntities[k].GetGlobalFrame();
				if (globalFrame3.NearlyEquals(engineFrame, 1E-05f))
				{
					if (this._hoveredSiegeEntityFrame != globalFrame3)
					{
						SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress3 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(1).SiegeEngines.DeployedRangedSiegeEngines[k];
						InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(siegeEngineConstructionProgress3) });
					}
					return;
				}
			}
			for (int l = 0; l < this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel.Length; l++)
			{
				MatrixFrame globalFrame4 = this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[l].GetGlobalFrame();
				if (globalFrame4.NearlyEquals(engineFrame, 1E-05f))
				{
					if (this._hoveredSiegeEntityFrame != globalFrame4)
					{
						SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress4 = PlayerSiege.PlayerSiegeEvent.GetSiegeEventSide(0).SiegeEngines.DeployedRangedSiegeEngines[l];
						InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(siegeEngineConstructionProgress4) });
					}
					return;
				}
			}
			for (int m = 0; m < this._defenderBreachableWallEntitiesCacheForCurrentLevel.Length; m++)
			{
				MatrixFrame globalFrame5 = this._defenderBreachableWallEntitiesCacheForCurrentLevel[m].GetGlobalFrame();
				if (globalFrame5.NearlyEquals(engineFrame, 1E-05f))
				{
					if (this._hoveredSiegeEntityFrame != globalFrame5 && this.PartyBase.IsSettlement)
					{
						InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { SandBoxUIHelper.GetWallSectionTooltip(this.PartyBase.Settlement, m) });
					}
					return;
				}
			}
			this._hoveredSiegeEntityFrame = MatrixFrame.Identity;
		}

		internal void OnMapHoverSiegeEngineEnd()
		{
			this._hoveredSiegeEntityFrame = MatrixFrame.Identity;
			MBInformationManager.HideInformations();
		}

		public void OnStartup()
		{
			bool flag = false;
			if (this.PartyBase.IsMobile)
			{
				this.StrategicEntity = GameEntity.CreateEmpty(this.MapScene, true);
				if (!this.PartyBase.IsVisible)
				{
					this.StrategicEntity.EntityFlags |= 536870912;
				}
			}
			else if (this.PartyBase.IsSettlement)
			{
				this.StrategicEntity = this.MapScene.GetCampaignEntityWithName(this.PartyBase.Id);
				if (this.StrategicEntity == null)
				{
					Campaign.Current.MapSceneWrapper.AddNewEntityToMapScene(this.PartyBase.Settlement.StringId, this.PartyBase.Settlement.Position2D);
					this.StrategicEntity = this.MapScene.GetCampaignEntityWithName(this.PartyBase.Id);
				}
				bool flag2 = false;
				if (this.PartyBase.Settlement.IsFortification)
				{
					List<GameEntity> list = new List<GameEntity>();
					this.StrategicEntity.GetChildrenRecursive(ref list);
					this.PopulateSiegeEngineFrameListsFromChildren(list);
					this.UpdateDefenderSiegeEntitiesCache();
					this.TownPhysicalEntities = list.FindAll((GameEntity x) => x.HasTag("bo_town"));
					List<GameEntity> list2 = new List<GameEntity>();
					Dictionary<int, List<GameEntity>> dictionary = new Dictionary<int, List<GameEntity>>();
					dictionary.Add(1, new List<GameEntity>());
					dictionary.Add(2, new List<GameEntity>());
					dictionary.Add(3, new List<GameEntity>());
					List<MatrixFrame> list3 = new List<MatrixFrame>();
					List<MatrixFrame> list4 = new List<MatrixFrame>();
					foreach (GameEntity gameEntity in list)
					{
						if (gameEntity.HasTag("main_map_city_gate"))
						{
							PartyBase.IsPositionOkForTraveling(gameEntity.GetGlobalFrame().origin.AsVec2);
							flag2 = true;
							list2.Add(gameEntity);
						}
						if (gameEntity.HasTag("map_settlement_circle"))
						{
							this.CircleLocalFrame = gameEntity.GetGlobalFrame();
							flag = true;
							gameEntity.SetVisibilityExcludeParents(false);
							list2.Add(gameEntity);
						}
						if (gameEntity.HasTag("map_banner_placeholder"))
						{
							int upgradeLevelOfEntity = gameEntity.Parent.GetUpgradeLevelOfEntity();
							if (upgradeLevelOfEntity == 0)
							{
								dictionary[1].Add(gameEntity);
								dictionary[2].Add(gameEntity);
								dictionary[3].Add(gameEntity);
							}
							else
							{
								dictionary[upgradeLevelOfEntity].Add(gameEntity);
							}
							list2.Add(gameEntity);
						}
						if (gameEntity.HasTag("map_camp_area_1"))
						{
							list3.Add(gameEntity.GetGlobalFrame());
							list2.Add(gameEntity);
						}
						else if (gameEntity.HasTag("map_camp_area_2"))
						{
							list4.Add(gameEntity.GetGlobalFrame());
							list2.Add(gameEntity);
						}
					}
					this._gateBannerEntitiesWithLevels = dictionary;
					if (this.PartyBase.Settlement.IsFortification)
					{
						this.PartyBase.Settlement.Town.BesiegerCampPositions1 = list3.ToArray();
						this.PartyBase.Settlement.Town.BesiegerCampPositions2 = list4.ToArray();
					}
					foreach (GameEntity gameEntity2 in list2)
					{
						gameEntity2.Remove(112);
					}
				}
				if (!flag2)
				{
					if (!this.PartyBase.Settlement.IsTown)
					{
						bool isCastle = this.PartyBase.Settlement.IsCastle;
					}
					if (!PartyBase.IsPositionOkForTraveling(this.PartyBase.Settlement.GatePosition))
					{
						Vec2 gatePosition = this.PartyBase.Settlement.GatePosition;
					}
				}
			}
			CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(this.PartyBase);
			if (!flag)
			{
				this.CircleLocalFrame = MatrixFrame.Identity;
				if (this.PartyBase.IsSettlement)
				{
					MatrixFrame circleLocalFrame = this.CircleLocalFrame;
					Mat3 rotation = circleLocalFrame.rotation;
					if (this.PartyBase.Settlement.IsVillage)
					{
						rotation.ApplyScaleLocal(1.75f);
					}
					else if (this.PartyBase.Settlement.IsTown)
					{
						rotation.ApplyScaleLocal(5.75f);
					}
					else if (this.PartyBase.Settlement.IsCastle)
					{
						rotation.ApplyScaleLocal(2.75f);
					}
					else
					{
						rotation.ApplyScaleLocal(1.75f);
					}
					circleLocalFrame.rotation = rotation;
					this.CircleLocalFrame = circleLocalFrame;
				}
				else if ((visualPartyLeader != null && visualPartyLeader.HasMount()) || this.PartyBase.MobileParty.IsCaravan)
				{
					MatrixFrame circleLocalFrame2 = this.CircleLocalFrame;
					Mat3 rotation2 = circleLocalFrame2.rotation;
					rotation2.ApplyScaleLocal(0.4625f);
					circleLocalFrame2.rotation = rotation2;
					this.CircleLocalFrame = circleLocalFrame2;
				}
				else
				{
					MatrixFrame circleLocalFrame3 = this.CircleLocalFrame;
					Mat3 rotation3 = circleLocalFrame3.rotation;
					rotation3.ApplyScaleLocal(0.3725f);
					circleLocalFrame3.rotation = rotation3;
					this.CircleLocalFrame = circleLocalFrame3;
				}
			}
			this.StrategicEntity.SetVisibilityExcludeParents(this.PartyBase.IsVisible);
			AgentVisuals humanAgentVisuals = this.HumanAgentVisuals;
			if (humanAgentVisuals != null)
			{
				GameEntity entity = humanAgentVisuals.GetEntity();
				if (entity != null)
				{
					entity.SetVisibilityExcludeParents(this.PartyBase.IsVisible);
				}
			}
			AgentVisuals mountAgentVisuals = this.MountAgentVisuals;
			if (mountAgentVisuals != null)
			{
				GameEntity entity2 = mountAgentVisuals.GetEntity();
				if (entity2 != null)
				{
					entity2.SetVisibilityExcludeParents(this.PartyBase.IsVisible);
				}
			}
			AgentVisuals caravanMountAgentVisuals = this.CaravanMountAgentVisuals;
			if (caravanMountAgentVisuals != null)
			{
				GameEntity entity3 = caravanMountAgentVisuals.GetEntity();
				if (entity3 != null)
				{
					entity3.SetVisibilityExcludeParents(this.PartyBase.IsVisible);
				}
			}
			this.StrategicEntity.SetReadyToRender(true);
			this.StrategicEntity.SetEntityEnvMapVisibility(false);
			this._entityAlpha = (this.PartyBase.IsVisible ? 1f : 0f);
			this.InitializePartyCollider(this.PartyBase);
			List<GameEntity> list5 = new List<GameEntity>();
			this.StrategicEntity.GetChildrenRecursive(ref list5);
			if (!MapScreen.VisualsOfEntities.ContainsKey(this.StrategicEntity.Pointer))
			{
				MapScreen.VisualsOfEntities.Add(this.StrategicEntity.Pointer, this);
			}
			foreach (GameEntity gameEntity3 in list5)
			{
				if (!MapScreen.VisualsOfEntities.ContainsKey(gameEntity3.Pointer) && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity3.Pointer))
				{
					MapScreen.VisualsOfEntities.Add(gameEntity3.Pointer, this);
				}
			}
			if (this.PartyBase.IsSettlement)
			{
				this.StrategicEntity.SetAsPredisplayEntity();
			}
		}

		private void PopulateSiegeEngineFrameListsFromChildren(List<GameEntity> children)
		{
			this._attackerRangedEngineSpawnEntities = (from e in children.FindAll((GameEntity x) => x.Tags.Any((string t) => t.Contains("map_siege_engine")))
				orderby e.Tags.First((string s) => s.Contains("map_siege_engine"))
				select e).ToArray<GameEntity>();
			foreach (GameEntity gameEntity in this._attackerRangedEngineSpawnEntities)
			{
				if (gameEntity.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity.GetChild(0).Pointer))
				{
					MapScreen.FrameAndVisualOfEngines.Add(gameEntity.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity.GetGlobalFrame(), this));
				}
			}
			this._defenderRangedEngineSpawnEntitiesForAllLevels = (from e in children.FindAll((GameEntity x) => x.Tags.Any((string t) => t.Contains("map_defensive_engine")))
				orderby e.Tags.First((string s) => s.Contains("map_defensive_engine"))
				select e).ToArray<GameEntity>();
			foreach (GameEntity gameEntity2 in this._defenderRangedEngineSpawnEntitiesForAllLevels)
			{
				if (gameEntity2.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity2.GetChild(0).Pointer))
				{
					MapScreen.FrameAndVisualOfEngines.Add(gameEntity2.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity2.GetGlobalFrame(), this));
				}
			}
			this._attackerBatteringRamSpawnEntities = children.FindAll((GameEntity x) => x.HasTag("map_siege_ram")).ToArray();
			foreach (GameEntity gameEntity3 in this._attackerBatteringRamSpawnEntities)
			{
				if (gameEntity3.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity3.GetChild(0).Pointer))
				{
					MapScreen.FrameAndVisualOfEngines.Add(gameEntity3.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity3.GetGlobalFrame(), this));
				}
			}
			this._attackerSiegeTowerSpawnEntities = children.FindAll((GameEntity x) => x.HasTag("map_siege_tower")).ToArray();
			foreach (GameEntity gameEntity4 in this._attackerSiegeTowerSpawnEntities)
			{
				if (gameEntity4.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity4.GetChild(0).Pointer))
				{
					MapScreen.FrameAndVisualOfEngines.Add(gameEntity4.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity4.GetGlobalFrame(), this));
				}
			}
			this._defenderBreachableWallEntitiesForAllLevels = children.FindAll((GameEntity x) => x.HasTag("map_breachable_wall")).ToArray();
			foreach (GameEntity gameEntity5 in this._defenderBreachableWallEntitiesForAllLevels)
			{
				if (gameEntity5.ChildCount > 0 && !MapScreen.FrameAndVisualOfEngines.ContainsKey(gameEntity5.GetChild(0).Pointer))
				{
					MapScreen.FrameAndVisualOfEngines.Add(gameEntity5.GetChild(0).Pointer, new Tuple<MatrixFrame, PartyVisual>(gameEntity5.GetGlobalFrame(), this));
				}
			}
		}

		private MatrixFrame GetFrame()
		{
			return this.StrategicEntity.GetFrame();
		}

		private void SetFrame(ref MatrixFrame frame)
		{
			if (this.StrategicEntity != null && !this.StrategicEntity.GetFrame().NearlyEquals(frame, 1E-05f))
			{
				this.StrategicEntity.SetFrame(ref frame);
				if (this.HumanAgentVisuals != null)
				{
					MatrixFrame matrixFrame = frame;
					matrixFrame.rotation.ApplyScaleLocal(this.HumanAgentVisuals.GetScale());
					this.HumanAgentVisuals.GetEntity().SetFrame(ref matrixFrame);
				}
				if (this.MountAgentVisuals != null)
				{
					MatrixFrame matrixFrame2 = frame;
					matrixFrame2.rotation.ApplyScaleLocal(this.MountAgentVisuals.GetScale());
					this.MountAgentVisuals.GetEntity().SetFrame(ref matrixFrame2);
				}
				if (this.CaravanMountAgentVisuals != null)
				{
					MatrixFrame matrixFrame3 = frame.TransformToParent(this.CaravanMountAgentVisuals.GetFrame());
					matrixFrame3.rotation.ApplyScaleLocal(this.CaravanMountAgentVisuals.GetScale());
					this.CaravanMountAgentVisuals.GetEntity().SetFrame(ref matrixFrame3);
				}
			}
		}

		private void UpdateDefenderSiegeEntitiesCache()
		{
			GameEntity.UpgradeLevelMask upgradeLevelMask = 0;
			if (this.PartyBase.IsSettlement && this.PartyBase.Settlement.IsFortification)
			{
				if (this.PartyBase.Settlement.Town.GetWallLevel() == 1)
				{
					upgradeLevelMask = 2;
				}
				else if (this.PartyBase.Settlement.Town.GetWallLevel() == 2)
				{
					upgradeLevelMask = 4;
				}
				else if (this.PartyBase.Settlement.Town.GetWallLevel() == 3)
				{
					upgradeLevelMask = 8;
				}
			}
			this._currentSettlementUpgradeLevelMask = upgradeLevelMask;
			this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel = this._defenderRangedEngineSpawnEntitiesForAllLevels.Where((GameEntity e) => (e.GetUpgradeLevelMask() & this._currentSettlementUpgradeLevelMask) == this._currentSettlementUpgradeLevelMask).ToArray<GameEntity>();
			this._defenderBreachableWallEntitiesCacheForCurrentLevel = this._defenderBreachableWallEntitiesForAllLevels.Where((GameEntity e) => (e.GetUpgradeLevelMask() & this._currentSettlementUpgradeLevelMask) == this._currentSettlementUpgradeLevelMask).ToArray<GameEntity>();
		}

		public MatrixFrame[] GetAttackerTowerSiegeEngineFrames()
		{
			MatrixFrame[] array = new MatrixFrame[this._attackerSiegeTowerSpawnEntities.Length];
			for (int i = 0; i < this._attackerSiegeTowerSpawnEntities.Length; i++)
			{
				array[i] = this._attackerSiegeTowerSpawnEntities[i].GetGlobalFrame();
			}
			return array;
		}

		public MatrixFrame[] GetAttackerBatteringRamSiegeEngineFrames()
		{
			MatrixFrame[] array = new MatrixFrame[this._attackerBatteringRamSpawnEntities.Length];
			for (int i = 0; i < this._attackerBatteringRamSpawnEntities.Length; i++)
			{
				array[i] = this._attackerBatteringRamSpawnEntities[i].GetGlobalFrame();
			}
			return array;
		}

		public MatrixFrame[] GetAttackerRangedSiegeEngineFrames()
		{
			MatrixFrame[] array = new MatrixFrame[this._attackerRangedEngineSpawnEntities.Length];
			for (int i = 0; i < this._attackerRangedEngineSpawnEntities.Length; i++)
			{
				array[i] = this._attackerRangedEngineSpawnEntities[i].GetGlobalFrame();
			}
			return array;
		}

		public MatrixFrame[] GetDefenderRangedSiegeEngineFrames()
		{
			MatrixFrame[] array = new MatrixFrame[this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel.Length];
			for (int i = 0; i < this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel.Length; i++)
			{
				array[i] = this._defenderRangedEngineSpawnEntitiesCacheForCurrentLevel[i].GetGlobalFrame();
			}
			return array;
		}

		public MatrixFrame[] GetBreachableWallFrames()
		{
			MatrixFrame[] array = new MatrixFrame[this._defenderBreachableWallEntitiesCacheForCurrentLevel.Length];
			for (int i = 0; i < this._defenderBreachableWallEntitiesCacheForCurrentLevel.Length; i++)
			{
				array[i] = this._defenderBreachableWallEntitiesCacheForCurrentLevel[i].GetGlobalFrame();
			}
			return array;
		}

		public bool IsVisibleOrFadingOut()
		{
			return this._entityAlpha > 0f;
		}

		private const string MapSiegeEngineTag = "map_siege_engine";

		private const string MapBreachableWallTag = "map_breachable_wall";

		private const string MapDefenderEngineTag = "map_defensive_engine";

		private const string CircleTag = "map_settlement_circle";

		private const string BannerPlaceHolderTag = "map_banner_placeholder";

		private const string MapCampArea1Tag = "map_camp_area_1";

		private const string MapCampArea2Tag = "map_camp_area_2";

		private const string MapSiegeEngineRamTag = "map_siege_ram";

		private const string TownPhysicalTag = "bo_town";

		private const string MapSiegeEngineTowerTag = "map_siege_tower";

		private const string MapPreparationTag = "siege_preparation";

		private const string BurnedTag = "looted";

		private const float PartyScale = 0.3f;

		private const float HorseAnimationSpeedFactor = 1.3f;

		private static readonly ActionIndexCache _raidOnFoot = ActionIndexCache.Create("act_map_raid");

		private static readonly ActionIndexCache _camelSwordAttack = ActionIndexCache.Create("act_map_rider_camel_attack_1h");

		private static readonly ActionIndexCache _camelSpearAttack = ActionIndexCache.Create("act_map_rider_camel_attack_1h_spear");

		private static readonly ActionIndexCache _camel1HandedSwingAttack = ActionIndexCache.Create("act_map_rider_camel_attack_1h_swing");

		private static readonly ActionIndexCache _camel2HandedSwingAttack = ActionIndexCache.Create("act_map_rider_camel_attack_2h_swing");

		private static readonly ActionIndexCache _camelUnarmedAttack = ActionIndexCache.Create("act_map_rider_camel_attack_unarmed");

		private static readonly ActionIndexCache _horseSwordAttack = ActionIndexCache.Create("act_map_rider_horse_attack_1h");

		private static readonly ActionIndexCache _horseSpearAttack = ActionIndexCache.Create("act_map_rider_horse_attack_1h_spear");

		private static readonly ActionIndexCache _horse1HandedSwingAttack = ActionIndexCache.Create("act_map_rider_horse_attack_1h_swing");

		private static readonly ActionIndexCache _horse2HandedSwingAttack = ActionIndexCache.Create("act_map_rider_horse_attack_2h_swing");

		private static readonly ActionIndexCache _horseUnarmedAttack = ActionIndexCache.Create("act_map_rider_horse_attack_unarmed");

		private static readonly ActionIndexCache _swordAttackMount = ActionIndexCache.Create("act_map_mount_attack_1h");

		private static readonly ActionIndexCache _spearAttackMount = ActionIndexCache.Create("act_map_mount_attack_spear");

		private static readonly ActionIndexCache _swingAttackMount = ActionIndexCache.Create("act_map_mount_attack_swing");

		private static readonly ActionIndexCache _unarmedAttackMount = ActionIndexCache.Create("act_map_mount_attack_unarmed");

		private static readonly ActionIndexCache _attack1H = ActionIndexCache.Create("act_map_attack_1h");

		private static readonly ActionIndexCache _attack2H = ActionIndexCache.Create("act_map_attack_2h");

		private static readonly ActionIndexCache _attackSpear1HOr2H = ActionIndexCache.Create("act_map_attack_spear_1h_or_2h");

		private static readonly ActionIndexCache _attackUnarmed = ActionIndexCache.Create("act_map_attack_unarmed");

		private readonly List<ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity>> _siegeRangedMachineEntities;

		private readonly List<ValueTuple<GameEntity, BattleSideEnum, int, MatrixFrame, GameEntity>> _siegeMeleeMachineEntities;

		private readonly List<ValueTuple<GameEntity, BattleSideEnum, int>> _siegeMissileEntities;

		private Dictionary<int, List<GameEntity>> _gateBannerEntitiesWithLevels;

		private GameEntity[] _attackerRangedEngineSpawnEntities;

		private GameEntity[] _attackerBatteringRamSpawnEntities;

		private GameEntity[] _defenderBreachableWallEntitiesCacheForCurrentLevel;

		private GameEntity[] _attackerSiegeTowerSpawnEntities;

		private GameEntity[] _defenderRangedEngineSpawnEntitiesForAllLevels;

		private GameEntity[] _defenderRangedEngineSpawnEntitiesCacheForCurrentLevel;

		private GameEntity[] _defenderBreachableWallEntitiesForAllLevels;

		private ValueTuple<string, GameEntityComponent> _cachedBannerComponent;

		private ValueTuple<string, GameEntity> _cachedBannerEntity;

		private MatrixFrame _hoveredSiegeEntityFrame = MatrixFrame.Identity;

		private GameEntity.UpgradeLevelMask _currentSettlementUpgradeLevelMask;

		private float _speed;

		private float _entityAlpha;

		private Scene _mapScene;

		private Mesh _contourMaskMesh;

		private uint _currentLevelMask;

		public readonly PartyBase PartyBase;

		private Vec2 _lastFrameVisualPositionWithoutError;

		private struct SiegeBombardmentData
		{
			public Vec3 LaunchGlobalPosition;

			public Vec3 TargetPosition;

			public MatrixFrame ShooterGlobalFrame;

			public MatrixFrame TargetAlignedShooterGlobalFrame;

			public float MissileSpeed;

			public float Gravity;

			public float LaunchAngle;

			public float RotationDuration;

			public float ReloadDuration;

			public float AimingDuration;

			public float MissileLaunchDuration;

			public float FireDuration;

			public float FlightDuration;

			public float TotalDuration;
		}
	}
}
