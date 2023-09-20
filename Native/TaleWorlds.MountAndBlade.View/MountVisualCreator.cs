using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000012 RID: 18
	public static class MountVisualCreator
	{
		// Token: 0x06000074 RID: 116 RVA: 0x000055A4 File Offset: 0x000037A4
		public static void SetMaterialProperties(ItemObject mountItem, MetaMesh mountMesh, MountCreationKey key, ref uint maneMeshMultiplier)
		{
			HorseComponent horseComponent = mountItem.HorseComponent;
			int num = MathF.Min((int)key.MaterialIndex, horseComponent.HorseMaterialNames.Count - 1);
			HorseComponent.MaterialProperty materialProperty = horseComponent.HorseMaterialNames[num];
			Material fromResource = Material.GetFromResource(materialProperty.Name);
			if (mountItem.ItemType == 1)
			{
				int num2 = MathF.Min((int)key.MeshMultiplierIndex, materialProperty.MeshMultiplier.Count - 1);
				if (num2 != -1)
				{
					maneMeshMultiplier = materialProperty.MeshMultiplier[num2].Item1;
				}
				mountMesh.SetMaterialToSubMeshesWithTag(fromResource, "horse_body");
				mountMesh.SetFactorColorToSubMeshesWithTag(maneMeshMultiplier, "horse_tail");
				return;
			}
			mountMesh.SetMaterial(fromResource);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005648 File Offset: 0x00003848
		public static List<MetaMesh> AddMountMesh(MBAgentVisuals agentVisual, ItemObject mountItem, ItemObject harnessItem, string mountCreationKeyStr, Agent agent = null)
		{
			List<MetaMesh> list = new List<MetaMesh>();
			HorseComponent horseComponent = mountItem.HorseComponent;
			uint maxValue = uint.MaxValue;
			MetaMesh multiMesh = mountItem.GetMultiMesh(false, false, true);
			if (string.IsNullOrEmpty(mountCreationKeyStr))
			{
				mountCreationKeyStr = MountCreationKey.GetRandomMountKeyString(mountItem, MBRandom.RandomInt());
			}
			MountCreationKey mountCreationKey = MountCreationKey.FromString(mountCreationKeyStr);
			if (mountItem.ItemType == 1)
			{
				MountVisualCreator.SetHorseColors(multiMesh, mountCreationKey);
			}
			if (horseComponent.HorseMaterialNames != null && horseComponent.HorseMaterialNames.Count > 0)
			{
				MountVisualCreator.SetMaterialProperties(mountItem, multiMesh, mountCreationKey, ref maxValue);
			}
			int nondeterministicRandomInt = MBRandom.NondeterministicRandomInt;
			MountVisualCreator.SetVoiceDefinition(agent, nondeterministicRandomInt);
			MetaMesh metaMesh = null;
			if (harnessItem != null)
			{
				metaMesh = harnessItem.GetMultiMesh(false, false, true);
			}
			foreach (KeyValuePair<string, bool> keyValuePair in horseComponent.AdditionalMeshesNameList)
			{
				if (keyValuePair.Key.Length > 0)
				{
					string text = keyValuePair.Key;
					if (harnessItem == null || !keyValuePair.Value)
					{
						MetaMesh copy = MetaMesh.GetCopy(text, true, false);
						if (maxValue != 4294967295U)
						{
							copy.SetFactor1Linear(maxValue);
						}
						list.Add(copy);
					}
					else
					{
						ArmorComponent armorComponent = harnessItem.ArmorComponent;
						if (armorComponent == null || armorComponent.ManeCoverType != 3)
						{
							ArmorComponent armorComponent2 = harnessItem.ArmorComponent;
							if (armorComponent2 != null && armorComponent2.ManeCoverType > 0)
							{
								object obj = text;
								object obj2 = "_";
								ArmorComponent.HorseHarnessCoverTypes? horseHarnessCoverTypes;
								if (harnessItem == null)
								{
									horseHarnessCoverTypes = null;
								}
								else
								{
									ArmorComponent armorComponent3 = harnessItem.ArmorComponent;
									horseHarnessCoverTypes = ((armorComponent3 != null) ? new ArmorComponent.HorseHarnessCoverTypes?(armorComponent3.ManeCoverType) : null);
								}
								text = obj + obj2 + horseHarnessCoverTypes;
							}
							MetaMesh copy2 = MetaMesh.GetCopy(text, true, false);
							if (maxValue != 4294967295U)
							{
								copy2.SetFactor1Linear(maxValue);
							}
							list.Add(copy2);
						}
					}
				}
			}
			if (multiMesh != null)
			{
				if (harnessItem != null)
				{
					ArmorComponent armorComponent4 = harnessItem.ArmorComponent;
					ArmorComponent.HorseTailCoverTypes? horseTailCoverTypes = ((armorComponent4 != null) ? new ArmorComponent.HorseTailCoverTypes?(armorComponent4.TailCoverType) : null);
					ArmorComponent.HorseTailCoverTypes horseTailCoverTypes2 = 1;
					if ((horseTailCoverTypes.GetValueOrDefault() == horseTailCoverTypes2) & (horseTailCoverTypes != null))
					{
						multiMesh.RemoveMeshesWithTag("horse_tail");
					}
				}
				list.Add(multiMesh);
			}
			if (metaMesh != null)
			{
				if (agentVisual != null)
				{
					MetaMesh metaMesh2 = null;
					if (NativeConfig.CharacterDetail > 2 && harnessItem.ArmorComponent != null)
					{
						metaMesh2 = MetaMesh.GetCopy(harnessItem.ArmorComponent.ReinsRopeMesh, false, true);
					}
					ArmorComponent armorComponent5 = harnessItem.ArmorComponent;
					MetaMesh copy3 = MetaMesh.GetCopy((armorComponent5 != null) ? armorComponent5.ReinsMesh : null, false, true);
					if (metaMesh2 != null && copy3 != null)
					{
						agentVisual.AddHorseReinsClothMesh(copy3, metaMesh2);
						metaMesh2.ManualInvalidate();
					}
					if (copy3 != null)
					{
						list.Add(copy3);
					}
				}
				else if (harnessItem.ArmorComponent != null)
				{
					MetaMesh copy4 = MetaMesh.GetCopy(harnessItem.ArmorComponent.ReinsMesh, true, true);
					if (copy4 != null)
					{
						list.Add(copy4);
					}
				}
				list.Add(metaMesh);
			}
			return list;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00005928 File Offset: 0x00003B28
		public static void SetHorseColors(MetaMesh horseMesh, MountCreationKey mountCreationKey)
		{
			horseMesh.SetVectorArgument((float)mountCreationKey._leftFrontLegColorIndex, (float)mountCreationKey._rightFrontLegColorIndex, (float)mountCreationKey._leftBackLegColorIndex, (float)mountCreationKey._rightBackLegColorIndex);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000594C File Offset: 0x00003B4C
		public static void ClearMountMesh(GameEntity gameEntity)
		{
			gameEntity.RemoveAllChildren();
			gameEntity.Remove(106);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000595C File Offset: 0x00003B5C
		private static void SetVoiceDefinition(Agent agent, int seedForRandomVoiceTypeAndPitch)
		{
			MBAgentVisuals mbagentVisuals = ((agent != null) ? agent.AgentVisuals : null);
			if (mbagentVisuals != null)
			{
				string soundAndCollisionInfoClassName = agent.GetSoundAndCollisionInfoClassName();
				int num;
				if (string.IsNullOrEmpty(soundAndCollisionInfoClassName))
				{
					num = 0;
				}
				else
				{
					num = SkinVoiceManager.GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(soundAndCollisionInfoClassName);
				}
				if (num == 0)
				{
					mbagentVisuals.SetVoiceDefinitionIndex(-1, 0f);
					return;
				}
				int num2 = MathF.Abs(seedForRandomVoiceTypeAndPitch);
				float num3 = (float)num2 * 4.656613E-10f;
				int[] array = new int[num];
				SkinVoiceManager.GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(soundAndCollisionInfoClassName, array);
				int num4 = array[num2 % num];
				mbagentVisuals.SetVoiceDefinitionIndex(num4, num3);
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000059DC File Offset: 0x00003BDC
		public static void AddMountMeshToEntity(GameEntity gameEntity, ItemObject mountItem, ItemObject harnessItem, string mountCreationKeyStr, Agent agent = null)
		{
			foreach (MetaMesh metaMesh in MountVisualCreator.AddMountMesh(null, mountItem, harnessItem, mountCreationKeyStr, agent))
			{
				gameEntity.AddMultiMeshToSkeleton(metaMesh);
				metaMesh.ManualInvalidate();
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005A3C File Offset: 0x00003C3C
		public static void AddMountMeshToAgentVisual(MBAgentVisuals agentVisual, ItemObject mountItem, ItemObject harnessItem, string mountCreationKeyStr, Agent agent = null)
		{
			foreach (MetaMesh metaMesh in MountVisualCreator.AddMountMesh(agentVisual, mountItem, harnessItem, mountCreationKeyStr, agent))
			{
				agentVisual.AddMultiMesh(metaMesh, -1);
				metaMesh.ManualInvalidate();
			}
			HorseComponent horseComponent = mountItem.HorseComponent;
			if (((horseComponent != null) ? horseComponent.SkeletonScale : null) != null)
			{
				agentVisual.ApplySkeletonScale(mountItem.HorseComponent.SkeletonScale.MountSitBoneScale, mountItem.HorseComponent.SkeletonScale.MountRadiusAdder, mountItem.HorseComponent.SkeletonScale.BoneIndices, mountItem.HorseComponent.SkeletonScale.Scales);
			}
		}
	}
}
