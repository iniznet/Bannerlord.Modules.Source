using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.View
{
	public class AgentVisuals : IAgentVisual
	{
		public bool IsFemale
		{
			get
			{
				return this._data.SkeletonTypeData == 1 || this._data.SkeletonTypeData == 5 || this._data.SkeletonTypeData == 6 || this._data.SkeletonTypeData == 7;
			}
		}

		public MBAgentVisuals GetVisuals()
		{
			return this._data.AgentVisuals;
		}

		public void Reset()
		{
			this._data.AgentVisuals.Reset();
		}

		public void ResetNextFrame()
		{
			this._data.AgentVisuals.ResetNextFrame();
		}

		public MatrixFrame GetFrame()
		{
			return this._data.FrameData;
		}

		public BodyProperties GetBodyProperties()
		{
			return this._data.BodyPropertiesData;
		}

		public void SetBodyProperties(BodyProperties bodyProperties)
		{
			this._data.BodyProperties(bodyProperties);
		}

		public bool GetIsFemale()
		{
			return this.IsFemale;
		}

		public string GetCharacterObjectID()
		{
			return this._data.CharacterObjectStringIdData;
		}

		public void SetCharacterObjectID(string id)
		{
			this._data.CharacterObjectStringId(id);
		}

		public Equipment GetEquipment()
		{
			return this._data.EquipmentData;
		}

		private AgentVisuals(AgentVisualsData data, string name, bool isRandomProgress, bool needBatchedVersionForWeaponMeshes, bool forceUseFaceCache)
		{
			this._data = data;
			this._data.AgentVisuals = MBAgentVisuals.CreateAgentVisuals(this._data.SceneData, name, data.MonsterData.EyeOffsetWrtHead);
			if (data.EntityData != null)
			{
				this._data.AgentVisuals.SetEntity(data.EntityData);
			}
			this._scale = ((this._data.ScaleData <= 1E-05f) ? 1f : this._data.ScaleData);
			this.Refresh(needBatchedVersionForWeaponMeshes, false, null, isRandomProgress, forceUseFaceCache);
		}

		public AgentVisualsData GetCopyAgentVisualsData()
		{
			return new AgentVisualsData(this._data);
		}

		public GameEntity GetEntity()
		{
			return this._data.AgentVisuals.GetEntity();
		}

		public void SetVisible(bool value)
		{
			this._data.AgentVisuals.SetVisible(value);
		}

		public Vec3 GetGlobalStableEyePoint(bool isHumanoid)
		{
			return this._data.AgentVisuals.GetGlobalStableEyePoint(isHumanoid);
		}

		public Vec3 GetGlobalStableNeckPoint(bool isHumanoid)
		{
			return this._data.AgentVisuals.GetGlobalStableNeckPoint(isHumanoid);
		}

		public CompositeComponent AddPrefabToAgentVisualBoneByBoneType(string prefabName, HumanBone boneType)
		{
			return this._data.AgentVisuals.AddPrefabToAgentVisualBoneByBoneType(prefabName, boneType);
		}

		public CompositeComponent AddPrefabToAgentVisualBoneByRealBoneIndex(string prefabName, sbyte realBoneIndex)
		{
			return this._data.AgentVisuals.AddPrefabToAgentVisualBoneByRealBoneIndex(prefabName, realBoneIndex);
		}

		public void SetAgentLodZeroOrMax(bool value)
		{
			this._data.AgentVisuals.SetAgentLodZeroOrMax(value);
		}

		public float GetScale()
		{
			return this._scale;
		}

		public void SetAction(ActionIndexCache actionIndex, float startProgress = 0f, bool forceFaceMorphRestart = true)
		{
			if (this._data.AgentVisuals != null)
			{
				Skeleton skeleton = this._data.AgentVisuals.GetSkeleton();
				if (skeleton != null)
				{
					MBSkeletonExtensions.SetAgentActionChannel(skeleton, 0, actionIndex, startProgress, -0.2f, forceFaceMorphRestart);
					skeleton.ManualInvalidate();
				}
			}
		}

		public bool DoesActionContinueWithCurrentAction(ActionIndexCache actionIndex)
		{
			bool flag = false;
			if (this._data.AgentVisuals != null)
			{
				Skeleton skeleton = this._data.AgentVisuals.GetSkeleton();
				if (skeleton != null)
				{
					flag = MBSkeletonExtensions.DoesActionContinueWithCurrentActionAtChannel(skeleton, 0, actionIndex);
				}
			}
			return flag;
		}

		public float GetAnimationParameterAtChannel(int channelIndex)
		{
			float num = 0f;
			if (this._data.AgentVisuals != null && this._data.AgentVisuals.GetSkeleton() != null)
			{
				num = this._data.AgentVisuals.GetSkeleton().GetAnimationParameterAtChannel(channelIndex);
			}
			return num;
		}

		public void Refresh(bool needBatchedVersionForWeaponMeshes, AgentVisualsData data, bool forceUseFaceCache = false)
		{
			AgentVisualsData data2 = this._data;
			this._data = data;
			bool flag = data2.SkeletonTypeData != this._data.SkeletonTypeData;
			Equipment equipmentData = this._data.EquipmentData;
			this.Refresh(needBatchedVersionForWeaponMeshes, flag, equipmentData, false, forceUseFaceCache);
		}

		public void SetClothWindToWeaponAtIndex(Vec3 localWindDirection, bool isLocal, EquipmentIndex weaponIndex)
		{
			this._data.AgentVisuals.SetClothWindToWeaponAtIndex(localWindDirection, isLocal, weaponIndex);
		}

		private void Refresh(bool needBatchedVersionForWeaponMeshes, bool removeSkeleton = false, Equipment oldEquipment = null, bool isRandomProgress = false, bool forceUseFaceCache = false)
		{
			float num = 0f;
			float num2 = 0f;
			string text = "";
			bool flag = Extensions.HasAnyFlag<AgentFlag>(this._data.MonsterData.Flags, 2048);
			Skeleton skeleton = this._data.AgentVisuals.GetSkeleton();
			float num3 = -0.2f;
			ActionIndexCache actionIndexCache;
			if (skeleton != null && this._data.ActionSetData.IsValid)
			{
				num = skeleton.GetAnimationParameterAtChannel(0);
				actionIndexCache = MBSkeletonExtensions.GetActionAtChannel(skeleton, 0);
				num3 = 0f;
				if (flag)
				{
					num2 = MBSkeletonExtensions.GetSkeletonFaceAnimationTime(skeleton);
					text = MBSkeletonExtensions.GetSkeletonFaceAnimationName(skeleton);
				}
			}
			else
			{
				actionIndexCache = this._data.ActionCodeData;
			}
			if (skeleton != null)
			{
				skeleton.ManualInvalidate();
			}
			this._data.AgentVisuals.SetSetupMorphNode(this._data.UseMorphAnimsData);
			this._data.AgentVisuals.UseScaledWeapons(this._data.UseScaledWeaponsData);
			MatrixFrame frameData = this._data.FrameData;
			this._scale = ((this._data.ScaleData == 0f) ? MBBodyProperties.GetScaleFromKey(this._data.RaceData, this.IsFemale ? 1 : 0, this._data.BodyPropertiesData) : this._data.ScaleData);
			frameData.rotation.ApplyScaleLocal(this._scale);
			this._data.AgentVisuals.SetFrame(ref frameData);
			object obj = !removeSkeleton && skeleton != null && oldEquipment != null;
			bool flag2 = false;
			object obj2 = obj;
			if (obj2 != null)
			{
				flag2 = this.ClearAndAddChangedVisualComponentsOfWeapons(oldEquipment, needBatchedVersionForWeaponMeshes);
			}
			if (obj2 == null || !flag2)
			{
				this._data.AgentVisuals.ClearVisualComponents(false);
				if (this._data.ActionSetData.IsValid && text != "facegen_teeth")
				{
					AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(this._data.MonsterData, this._data.ActionSetData, 1f, this._data.HasClippingPlaneData);
					Skeleton skeleton2 = MBSkeletonExtensions.CreateWithActionSet(ref animationSystemData);
					this._data.AgentVisuals.SetSkeleton(skeleton2);
					skeleton2.ManualInvalidate();
				}
				if (this._data.EquipmentData == null)
				{
					int num4 = 481;
					this.AddSkinMeshesToEntity(num4, !needBatchedVersionForWeaponMeshes, forceUseFaceCache);
				}
				else if (!string.IsNullOrEmpty(this._data.MountCreationKeyData) || !flag)
				{
					MountVisualCreator.AddMountMeshToEntity(this.GetEntity(), this._data.EquipmentData[10].Item, this._data.EquipmentData[11].Item, this._data.MountCreationKeyData, null);
				}
				else
				{
					this.AddSkinArmorWeaponMultiMeshesToEntity(this._data.ClothColor1Data, this._data.ClothColor2Data, needBatchedVersionForWeaponMeshes, forceUseFaceCache);
				}
			}
			if (this._data.ActionSetData.IsValid && actionIndexCache != ActionIndexCache.act_none)
			{
				if (isRandomProgress)
				{
					num = MBRandom.RandomFloat;
				}
				skeleton = this._data.AgentVisuals.GetSkeleton();
				if (skeleton != null)
				{
					MBSkeletonExtensions.SetAgentActionChannel(skeleton, 0, actionIndexCache, num, num3, true);
					if (num2 > 0f)
					{
						MBSkeletonExtensions.SetSkeletonFaceAnimationTime(skeleton, num2);
					}
					skeleton.ManualInvalidate();
				}
			}
		}

		public void TickVisuals()
		{
			if (this._data.ActionSetData.IsValid)
			{
				MBSkeletonExtensions.TickActionChannels(this._data.AgentVisuals.GetSkeleton());
			}
		}

		public void Tick(AgentVisuals parentAgentVisuals, float dt, bool isEntityMoving = false, float speed = 0f)
		{
			this._data.AgentVisuals.Tick((parentAgentVisuals != null) ? parentAgentVisuals._data.AgentVisuals : null, dt, isEntityMoving, speed);
		}

		public static AgentVisuals Create(AgentVisualsData data, string name, bool isRandomProgress, bool needBatchedVersionForWeaponMeshes, bool forceUseFaceCache)
		{
			return new AgentVisuals(data, name, isRandomProgress, needBatchedVersionForWeaponMeshes, forceUseFaceCache);
		}

		public static float GetRandomGlossFactor(Random randomGenerator)
		{
			return 1f + (Extensions.NextFloat(randomGenerator) * 2f - 1f) * 0.05f;
		}

		public static void GetRandomClothingColors(int seed, Color inputColor1, Color inputColor2, out Color color1, out Color color2)
		{
			MBFastRandom mbfastRandom = new MBFastRandom((uint)seed);
			color1 = ColorExtensions.AddFactorInHSB(inputColor1, (2f * mbfastRandom.NextFloat() - 1f) * 4f, (2f * mbfastRandom.NextFloat() - 1f) * 0.2f, (2f * mbfastRandom.NextFloat() - 1f) * 0.2f);
			color2 = ColorExtensions.AddFactorInHSB(inputColor2, (2f * mbfastRandom.NextFloat() - 1f) * 8f, (2f * mbfastRandom.NextFloat() - 1f) * 0.5f, (2f * mbfastRandom.NextFloat() - 1f) * 0.3f);
		}

		private void AddSkinArmorWeaponMultiMeshesToEntity(uint teamColor1, uint teamColor2, bool needBatchedVersion, bool forceUseFaceCache = false)
		{
			this.AddSkinMeshesToEntity(MBEquipmentMissionExtensions.GetSkinMeshesMask(this._data.EquipmentData), !needBatchedVersion, forceUseFaceCache);
			this.AddArmorMultiMeshesToAgentEntity(teamColor1, teamColor2);
			int hashCode = this._data.BodyPropertiesData.GetHashCode();
			for (int i = 0; i < 5; i++)
			{
				if (!this._data.EquipmentData[i].IsEmpty)
				{
					MissionWeapon missionWeapon;
					missionWeapon..ctor(this._data.EquipmentData[i].Item, this._data.EquipmentData[i].ItemModifier, this._data.BannerData);
					if (this._data.AddColorRandomnessData)
					{
						missionWeapon.SetRandomGlossMultiplier(hashCode);
					}
					WeaponData weaponData = missionWeapon.GetWeaponData(needBatchedVersion);
					WeaponData ammoWeaponData = missionWeapon.GetAmmoWeaponData(needBatchedVersion);
					this._data.AgentVisuals.AddWeaponToAgentEntity(i, ref weaponData, missionWeapon.GetWeaponStatsData(), ref ammoWeaponData, missionWeapon.GetAmmoWeaponStatsData(), this._data.GetCachedWeaponEntity(i));
					weaponData.DeinitializeManagedPointers();
					ammoWeaponData.DeinitializeManagedPointers();
				}
			}
			this._data.AgentVisuals.SetWieldedWeaponIndices(this._data.RightWieldedItemIndexData, this._data.LeftWieldedItemIndexData);
			for (int j = 0; j < 5; j++)
			{
				if (!this._data.EquipmentData[j].IsEmpty && this._data.EquipmentData[j].Item.PrimaryWeapon.IsConsumable)
				{
					short num = this._data.EquipmentData[j].Item.PrimaryWeapon.MaxDataValue;
					if (j == this._data.RightWieldedItemIndexData)
					{
						num -= 1;
					}
					this._data.AgentVisuals.UpdateQuiverMeshesWithoutAgent(j, (int)num);
				}
			}
		}

		private void AddSkinMeshesToEntity(int mask, bool useGPUMorph, bool forceUseFaceCache = false)
		{
			SkinGenerationParams skinGenerationParams;
			if (this._data.EquipmentData != null)
			{
				bool flag = this._data.BodyPropertiesData.Age >= 14f && this._data.SkeletonTypeData == 1;
				skinGenerationParams..ctor(mask, this._data.EquipmentData.GetUnderwearType(flag), this._data.EquipmentData.BodyMeshType, this._data.EquipmentData.HairCoverType, this._data.EquipmentData.BeardCoverType, this._data.EquipmentData.BodyDeformType, this._data.PrepareImmediatelyData, 0f, this._data.SkeletonTypeData, this._data.RaceData, this._data.UseTranslucencyData, this._data.UseTesselationData);
			}
			else
			{
				skinGenerationParams..ctor(mask, 1, 0, 4, 0, 0, this._data.PrepareImmediatelyData, 0f, this._data.SkeletonTypeData, this._data.RaceData, this._data.UseTranslucencyData, this._data.UseTesselationData);
			}
			BasicCharacterObject basicCharacterObject = null;
			if (this._data.CharacterObjectStringIdData != null)
			{
				basicCharacterObject = MBObjectManager.Instance.GetObject<BasicCharacterObject>(this._data.CharacterObjectStringIdData);
			}
			bool flag2 = forceUseFaceCache || (basicCharacterObject != null && basicCharacterObject.FaceMeshCache);
			this._data.AgentVisuals.AddSkinMeshes(skinGenerationParams, this._data.BodyPropertiesData, useGPUMorph, flag2);
		}

		public void SetFaceGenerationParams(FaceGenerationParams faceGenerationParams)
		{
			this._data.AgentVisuals.SetFaceGenerationParams(faceGenerationParams);
		}

		public void SetVoiceDefinitionIndex(int voiceDefinitionIndex, float voicePitch)
		{
			this._data.AgentVisuals.SetVoiceDefinitionIndex(voiceDefinitionIndex, voicePitch);
		}

		public void StartRhubarbRecord(string path, int soundId)
		{
			this._data.AgentVisuals.StartRhubarbRecord(path, soundId);
		}

		public void SetAgentLodZeroOrMaxExternal(bool makeZero)
		{
			this._data.AgentVisuals.SetAgentLodZeroOrMax(makeZero);
		}

		public void SetAgentLocalSpeed(Vec2 speed)
		{
			this._data.AgentVisuals.SetAgentLocalSpeed(speed);
		}

		public void SetLookDirection(Vec3 direction)
		{
			this._data.AgentVisuals.SetLookDirection(direction);
		}

		public void AddArmorMultiMeshesToAgentEntity(uint teamColor1, uint teamColor2)
		{
			Random random = null;
			uint num;
			uint num2;
			if (this._data.AddColorRandomnessData)
			{
				int hashCode = this._data.BodyPropertiesData.GetHashCode();
				random = new Random(hashCode);
				Color color;
				Color color2;
				AgentVisuals.GetRandomClothingColors(hashCode, Color.FromUint(teamColor1), Color.FromUint(teamColor2), out color, out color2);
				num = color.ToUnsignedInteger();
				num2 = color2.ToUnsignedInteger();
			}
			else
			{
				num = teamColor1;
				num2 = teamColor2;
			}
			for (EquipmentIndex equipmentIndex = 11; equipmentIndex >= 0; equipmentIndex--)
			{
				if (equipmentIndex == 5 || equipmentIndex == 6 || equipmentIndex == 7 || equipmentIndex == 8 || equipmentIndex == 9)
				{
					ItemObject item = this._data.EquipmentData[equipmentIndex].Item;
					ItemObject itemObject = this._data.EquipmentData[equipmentIndex].CosmeticItem ?? item;
					if (itemObject != null)
					{
						bool flag = this._data.BodyPropertiesData.Age >= 14f && this._data.SkeletonTypeData == 1;
						bool flag2 = equipmentIndex == 6 && this._data.EquipmentData[8].Item != null;
						MetaMesh multiMesh = this._data.EquipmentData[equipmentIndex].GetMultiMesh(flag, flag2, true);
						if (multiMesh != null)
						{
							if (this._data.AddColorRandomnessData)
							{
								multiMesh.SetGlossMultiplier(AgentVisuals.GetRandomGlossFactor(random));
							}
							if (itemObject.IsUsingTableau && this._data.BannerData != null)
							{
								for (int i = 0; i < multiMesh.MeshCount; i++)
								{
									Mesh currentMesh = multiMesh.GetMeshAtIndex(i);
									Mesh currentMesh3 = currentMesh;
									if (currentMesh3 != null && !currentMesh3.HasTag("dont_use_tableau"))
									{
										Mesh currentMesh2 = currentMesh;
										if (currentMesh2 != null && currentMesh2.HasTag("banner_replacement_mesh"))
										{
											((BannerVisual)this._data.BannerData.BannerVisual).GetTableauTextureLarge(delegate(Texture t)
											{
												this.ApplyBannerTextureToMesh(currentMesh, t);
											}, true);
											currentMesh.ManualInvalidate();
											break;
										}
									}
									currentMesh.ManualInvalidate();
								}
							}
							else if (itemObject.IsUsingTeamColor)
							{
								for (int j = 0; j < multiMesh.MeshCount; j++)
								{
									Mesh meshAtIndex = multiMesh.GetMeshAtIndex(j);
									if (!meshAtIndex.HasTag("no_team_color"))
									{
										meshAtIndex.Color = num;
										meshAtIndex.Color2 = num2;
										Material material = meshAtIndex.GetMaterial().CreateCopy();
										material.AddMaterialShaderFlag("use_double_colormap_with_mask_texture", false);
										meshAtIndex.SetMaterial(material);
									}
									meshAtIndex.ManualInvalidate();
								}
							}
							if (itemObject.UsingFacegenScaling)
							{
								Skeleton skeleton = this._data.AgentVisuals.GetSkeleton();
								multiMesh.UseHeadBoneFaceGenScaling(skeleton, this._data.MonsterData.HeadLookDirectionBoneIndex, this._data.AgentVisuals.GetFacegenScalingMatrix());
								skeleton.ManualInvalidate();
							}
							this._data.AgentVisuals.AddMultiMesh(multiMesh, MBAgentVisuals.GetBodyMeshIndex(equipmentIndex));
							multiMesh.ManualInvalidate();
						}
					}
				}
			}
		}

		private void ApplyBannerTextureToMesh(Mesh armorMesh, Texture bannerTexture)
		{
			if (armorMesh != null)
			{
				Material material = armorMesh.GetMaterial().CreateCopy();
				material.SetTexture(1, bannerTexture);
				uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
				ulong shaderFlags = material.GetShaderFlags();
				material.SetShaderFlags(shaderFlags | (ulong)num);
				armorMesh.SetMaterial(material);
			}
		}

		public void MakeRandomVoiceForFacegen()
		{
			GameEntity entity = this._data.AgentVisuals.GetEntity();
			Vec3 origin = entity.Skeleton.GetBoneEntitialFrame(this._data.MonsterData.HeadLookDirectionBoneIndex).origin;
			Vec3 vec = entity.GetFrame().TransformToParent(origin);
			MBSkeletonExtensions.SetAgentActionChannel(entity.Skeleton, 1, AgentVisuals.act_command_leftstance_cached, 0f, -0.2f, true);
			SkinVoiceManager.SkinVoiceType[] array = new SkinVoiceManager.SkinVoiceType[]
			{
				SkinVoiceManager.VoiceType.Yell,
				SkinVoiceManager.VoiceType.Victory,
				SkinVoiceManager.VoiceType.Charge,
				SkinVoiceManager.VoiceType.Advance,
				SkinVoiceManager.VoiceType.Stop,
				SkinVoiceManager.VoiceType.FallBack,
				SkinVoiceManager.VoiceType.UseLadders,
				SkinVoiceManager.VoiceType.Infantry,
				SkinVoiceManager.VoiceType.FireAtWill,
				SkinVoiceManager.VoiceType.FormLine,
				SkinVoiceManager.VoiceType.FormShieldWall,
				SkinVoiceManager.VoiceType.FormCircle
			};
			int index = array[MBRandom.RandomInt(array.Length)].Index;
			this._data.AgentVisuals.MakeVoice(index, vec);
		}

		private bool ClearAndAddChangedVisualComponentsOfWeapons(Equipment oldEquipment, bool needBatchedVersionForMeshes)
		{
			int num = 0;
			for (int i = 0; i <= 3; i++)
			{
				if (!oldEquipment[i].IsEqualTo(this._data.EquipmentData[i]))
				{
					num++;
				}
			}
			if (num > 1)
			{
				return false;
			}
			bool flag = false;
			for (int j = 0; j <= 3; j++)
			{
				if (!oldEquipment[j].IsEqualTo(this._data.EquipmentData[j]))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this._data.AgentVisuals.ClearAllWeaponMeshes();
				int k = 0;
				int num2 = 0;
				while (k < 5)
				{
					if (!this._data.EquipmentData[k].IsEmpty)
					{
						MissionWeapon missionWeapon;
						missionWeapon..ctor(this._data.EquipmentData[k].Item, this._data.EquipmentData[k].ItemModifier, this._data.BannerData);
						if (this._data.AddColorRandomnessData)
						{
							missionWeapon.SetRandomGlossMultiplier(this._data.BodyPropertiesData.GetHashCode());
						}
						ItemObject.ItemTypeEnum ammoTypeForItemType = ItemObject.GetAmmoTypeForItemType(WeaponComponentData.GetItemTypeFromWeaponClass(this._data.EquipmentData[k].Item.PrimaryWeapon.WeaponClass));
						bool flag2 = false;
						MissionWeapon missionWeapon2 = default(MissionWeapon);
						for (int l = 0; l < 5; l++)
						{
							if (!this._data.EquipmentData[l].IsEmpty && WeaponComponentData.GetItemTypeFromWeaponClass(this._data.EquipmentData[l].Item.PrimaryWeapon.WeaponClass) == ammoTypeForItemType)
							{
								flag2 = true;
								missionWeapon2..ctor(this._data.EquipmentData[l].Item, this._data.EquipmentData[l].ItemModifier, this._data.BannerData);
								if (this._data.AddColorRandomnessData)
								{
									missionWeapon2.SetRandomGlossMultiplier(this._data.BodyPropertiesData.GetHashCode());
								}
							}
						}
						WeaponData weaponData = missionWeapon.GetWeaponData(needBatchedVersionForMeshes);
						WeaponData weaponData2 = (flag2 ? missionWeapon2.GetWeaponData(needBatchedVersionForMeshes) : WeaponData.InvalidWeaponData);
						WeaponStatsData[] array = (flag2 ? missionWeapon2.GetWeaponStatsData() : null);
						this._data.AgentVisuals.AddWeaponToAgentEntity(k, ref weaponData, missionWeapon.GetWeaponStatsData(), ref weaponData2, array, null);
					}
					k++;
					num2++;
				}
				this._data.AgentVisuals.SetWieldedWeaponIndices(this._data.RightWieldedItemIndexData, this._data.LeftWieldedItemIndexData);
			}
			return flag;
		}

		public void SetClothingColors(uint color1, uint color2)
		{
			this._data.ClothColor1(color1);
			this._data.ClothColor2(color2);
		}

		public void GetClothingColors(out uint color1, out uint color2)
		{
			color1 = this._data.ClothColor1Data;
			color2 = this._data.ClothColor2Data;
		}

		public void SetEntity(GameEntity entity)
		{
			this._data.AgentVisuals.SetEntity(entity);
		}

		public const float RandomGlossinessRange = 0.05f;

		public const float RandomClothingColor1HueRange = 4f;

		public const float RandomClothingColor1SaturationRange = 0.2f;

		public const float RandomClothingColor1BrightnessRange = 0.2f;

		public const float RandomClothingColor2HueRange = 8f;

		public const float RandomClothingColor2SaturationRange = 0.5f;

		public const float RandomClothingColor2BrightnessRange = 0.3f;

		private static readonly ActionIndexCache act_command_leftstance_cached = ActionIndexCache.Create("act_command_leftstance");

		private AgentVisualsData _data;

		private float _scale;
	}
}
