using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x0200000F RID: 15
	public static class ItemCollectionElementViewExtensions
	{
		// Token: 0x0600005D RID: 93 RVA: 0x000046B8 File Offset: 0x000028B8
		public static string GetMaterialCacheID(object o)
		{
			ItemRosterElement itemRosterElement = (ItemRosterElement)o;
			if (!itemRosterElement.EquipmentElement.Item.IsCraftedWeapon)
			{
				return "InventorySlot_" + itemRosterElement.EquipmentElement.Item.MultiMeshName;
			}
			return "";
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004708 File Offset: 0x00002908
		public static MetaMesh GetMultiMesh(this ItemObject item, bool isFemale, bool hasGloves, bool needBatchedVersion)
		{
			MetaMesh metaMesh = null;
			if (item != null)
			{
				bool flag = false;
				if (item.HasArmorComponent)
				{
					flag = item.ArmorComponent.MultiMeshHasGenderVariations;
				}
				metaMesh = item.GetMultiMeshCopyWithGenderData(flag && isFemale, hasGloves, needBatchedVersion);
				if (metaMesh == null || metaMesh.MeshCount == 0)
				{
					metaMesh = item.GetMultiMeshCopy();
				}
			}
			return metaMesh;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004755 File Offset: 0x00002955
		public static MetaMesh GetMultiMesh(this EquipmentElement equipmentElement, bool isFemale, bool hasGloves, bool needBatchedVersion)
		{
			if (equipmentElement.CosmeticItem == null)
			{
				return equipmentElement.Item.GetMultiMesh(isFemale, hasGloves, needBatchedVersion);
			}
			return equipmentElement.CosmeticItem.GetMultiMesh(isFemale, hasGloves, needBatchedVersion);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000477D File Offset: 0x0000297D
		public static MetaMesh GetMultiMesh(this MissionWeapon weapon, bool isFemale, bool hasGloves, bool needBatchedVersion)
		{
			return weapon.Item.GetMultiMesh(isFemale, hasGloves, needBatchedVersion);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004790 File Offset: 0x00002990
		public static MetaMesh GetItemMeshForInventory(this ItemRosterElement rosterElement, bool isFemale = false)
		{
			if (rosterElement.EquipmentElement.Item.ItemType != 5 && rosterElement.EquipmentElement.Item.ItemType != 6)
			{
				return rosterElement.EquipmentElement.GetMultiMesh(isFemale, false, false);
			}
			return rosterElement.EquipmentElement.Item.GetHolsterMeshCopy();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000047F0 File Offset: 0x000029F0
		public static MetaMesh GetHolsterMeshCopy(this ItemObject item)
		{
			if (item.WeaponDesign != null)
			{
				MetaMesh holsterMesh = CraftedDataViewManager.GetCraftedDataView(item.WeaponDesign).HolsterMesh;
				if (!(holsterMesh != null))
				{
					return null;
				}
				return holsterMesh.CreateCopy();
			}
			else
			{
				if (!string.IsNullOrEmpty(item.HolsterMeshName))
				{
					return MetaMesh.GetCopy(item.HolsterMeshName, true, false);
				}
				return null;
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000484A File Offset: 0x00002A4A
		public static MetaMesh GetHolsterMeshIfExists(this ItemObject item)
		{
			if (!(item.WeaponDesign != null))
			{
				return null;
			}
			return CraftedDataViewManager.GetCraftedDataView(item.WeaponDesign).HolsterMesh;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000486C File Offset: 0x00002A6C
		public static MetaMesh GetHolsterWithWeaponMeshCopy(this ItemObject item, bool needBatchedVersion)
		{
			if (item.WeaponDesign != null)
			{
				CraftedDataView craftedDataView = CraftedDataViewManager.GetCraftedDataView(item.WeaponDesign);
				MetaMesh metaMesh = (needBatchedVersion ? craftedDataView.HolsterMeshWithWeapon : craftedDataView.NonBatchedHolsterMeshWithWeapon);
				if (!(metaMesh != null))
				{
					return null;
				}
				return metaMesh.CreateCopy();
			}
			else
			{
				if (!string.IsNullOrEmpty(item.HolsterWithWeaponMeshName))
				{
					return MetaMesh.GetCopy(item.HolsterWithWeaponMeshName, true, false);
				}
				return null;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000048D3 File Offset: 0x00002AD3
		public static MetaMesh GetHolsterWithWeaponMeshIfExists(this ItemObject item)
		{
			if (!(item.WeaponDesign != null))
			{
				return null;
			}
			return CraftedDataViewManager.GetCraftedDataView(item.WeaponDesign).HolsterMeshWithWeapon;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000048F8 File Offset: 0x00002AF8
		public static MetaMesh GetFlyingMeshCopy(this ItemObject item, bool needBatchedVersion)
		{
			WeaponComponent weaponComponent = item.WeaponComponent;
			WeaponComponentData weaponComponentData = ((weaponComponent != null) ? weaponComponent.PrimaryWeapon : null);
			if (item.WeaponDesign != null && weaponComponentData != null)
			{
				if (!weaponComponentData.IsRangedWeapon || !weaponComponentData.IsConsumable)
				{
					return null;
				}
				CraftedDataView craftedDataView = CraftedDataViewManager.GetCraftedDataView(item.WeaponDesign);
				MetaMesh metaMesh = (needBatchedVersion ? craftedDataView.WeaponMesh : craftedDataView.NonBatchedWeaponMesh);
				if (!(metaMesh != null))
				{
					return null;
				}
				return metaMesh.CreateCopy();
			}
			else
			{
				if (!string.IsNullOrEmpty(item.FlyingMeshName))
				{
					return MetaMesh.GetCopy(item.FlyingMeshName, true, false);
				}
				return null;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004987 File Offset: 0x00002B87
		public static MetaMesh GetFlyingMeshIfExists(this ItemObject item)
		{
			if (item == null)
			{
				return null;
			}
			WeaponComponent weaponComponent = item.WeaponComponent;
			if (weaponComponent == null)
			{
				return null;
			}
			return weaponComponent.PrimaryWeapon.GetFlyingMeshIfExists(item);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000049A8 File Offset: 0x00002BA8
		internal static Material GetTableauMaterial(this ItemObject item, Banner banner)
		{
			Material tableauMaterial = null;
			if (item != null && item.IsUsingTableau)
			{
				Material material = null;
				MetaMesh metaMesh = item.GetMultiMeshCopy();
				int meshCount = metaMesh.MeshCount;
				for (int i = 0; i < meshCount; i++)
				{
					Mesh meshAtIndex = metaMesh.GetMeshAtIndex(i);
					if (!meshAtIndex.HasTag("dont_use_tableau"))
					{
						material = meshAtIndex.GetMaterial();
						meshAtIndex.ManualInvalidate();
						break;
					}
					meshAtIndex.ManualInvalidate();
				}
				metaMesh.ManualInvalidate();
				if (meshCount == 0 || material == null)
				{
					metaMesh = item.GetMultiMeshCopy();
					Mesh meshAtIndex2 = metaMesh.GetMeshAtIndex(0);
					material = meshAtIndex2.GetMaterial();
					meshAtIndex2.ManualInvalidate();
					metaMesh.ManualInvalidate();
				}
				if (banner != null)
				{
					if (material == null)
					{
						material = Material.GetDefaultTableauSampleMaterial(true);
					}
					uint flagMask = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
					Dictionary<Tuple<Material, BannerCode>, Material> dictionary = null;
					if (ViewSubModule.BannerTexturedMaterialCache != null)
					{
						dictionary = ViewSubModule.BannerTexturedMaterialCache;
					}
					BannerCode bannerCode = BannerCode.CreateFrom(banner);
					if (dictionary != null)
					{
						if (dictionary.ContainsKey(new Tuple<Material, BannerCode>(material, bannerCode)))
						{
							tableauMaterial = dictionary[new Tuple<Material, BannerCode>(material, bannerCode)];
						}
						else
						{
							tableauMaterial = material.CreateCopy();
							Action<Texture> action = delegate(Texture tex)
							{
								ulong shaderFlags = tableauMaterial.GetShaderFlags();
								tableauMaterial.SetShaderFlags(shaderFlags | (ulong)flagMask);
								tableauMaterial.SetTexture(1, tex);
							};
							banner.GetTableauTextureSmall(action);
							dictionary.Add(new Tuple<Material, BannerCode>(material, bannerCode), tableauMaterial);
						}
					}
					else
					{
						tableauMaterial = material.CreateCopy();
						Action<Texture> action2 = delegate(Texture tex)
						{
							ulong shaderFlags2 = tableauMaterial.GetShaderFlags();
							tableauMaterial.SetShaderFlags(shaderFlags2 | (ulong)flagMask);
							tableauMaterial.SetTexture(1, tex);
						};
						banner.GetTableauTextureSmall(action2);
					}
				}
			}
			return tableauMaterial;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004B55 File Offset: 0x00002D55
		public static MatrixFrame GetCameraFrameForInventory(this ItemRosterElement itemRosterElement)
		{
			return MatrixFrame.Identity;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004B5C File Offset: 0x00002D5C
		public static MatrixFrame GetItemFrameForInventory(this ItemRosterElement itemRosterElement)
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			Mat3 identity = Mat3.Identity;
			float num = 0.95f;
			Vec3 vec;
			vec..ctor(0f, 0f, 0f, -1f);
			MetaMesh itemMeshForInventory = itemRosterElement.GetItemMeshForInventory(false);
			if (itemMeshForInventory != null)
			{
				switch (itemRosterElement.EquipmentElement.Item.ItemType)
				{
				case 1:
				case 19:
				case 23:
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutUp(1.5707964f);
					break;
				case 2:
				case 3:
				case 4:
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutForward(-0.7853982f);
					break;
				case 5:
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutForward(-0.7853982f);
					break;
				case 6:
				case 10:
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutForward(-0.7853982f);
					break;
				case 7:
					identity.RotateAboutUp(3.1415927f);
					break;
				case 8:
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutForward(-0.7853982f);
					break;
				case 9:
					identity.RotateAboutForward(2.3561945f);
					identity.RotateAboutSide(-2.3561945f);
					identity.RotateAboutUp(-1.5707964f);
					break;
				case 11:
					identity.RotateAboutSide(-1.0995574f);
					identity.RotateAboutUp(0.7853982f);
					break;
				case 12:
				case 13:
				case 22:
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutUp(3.1415927f);
					identity.RotateAboutSide(-0.18849556f);
					break;
				case 14:
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutUp(3.1415927f);
					identity.RotateAboutSide(-0.31415927f);
					identity.RotateAboutUp(0.47123894f);
					break;
				case 15:
					identity.RotateAboutSide(1.7278761f);
					num = 2.1f;
					vec..ctor(0f, -0.4f, 0f, -1f);
					break;
				case 20:
					identity.RotateAboutSide(-0.62831855f);
					identity.RotateAboutUp(-0.47123894f);
					break;
				}
				if (itemRosterElement.EquipmentElement.Item.IsCraftedWeapon)
				{
					num *= 0.55f;
				}
				matrixFrame = itemRosterElement.EquipmentElement.Item.GetScaledFrame(identity, itemMeshForInventory, num, vec);
				if (itemRosterElement.EquipmentElement.Item.IsCraftedWeapon)
				{
					matrixFrame.Elevate(-0.01f * ((float)itemRosterElement.EquipmentElement.Item.WeaponComponent.PrimaryWeapon.WeaponLength / 2f));
				}
			}
			return matrixFrame;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004E38 File Offset: 0x00003038
		public static MatrixFrame GetItemFrameForItemTooltip(this ItemRosterElement itemRosterElement)
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			Mat3 identity = Mat3.Identity;
			float num = 0.85f;
			Vec3 vec;
			vec..ctor(0f, 0f, 0f, -1f);
			MetaMesh itemMeshForInventory = itemRosterElement.GetItemMeshForInventory(false);
			if (itemMeshForInventory != null)
			{
				ItemObject.ItemTypeEnum itemType = itemRosterElement.EquipmentElement.Item.ItemType;
				if (itemType == 13 || itemType == 22 || itemType == 12)
				{
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutUp(3.1415927f);
				}
				else if (itemType == 5)
				{
					identity.RotateAboutSide(-1.5707964f);
				}
				else if (itemType == 1 || itemType == 23 || itemType == 19)
				{
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutUp(1.5707964f);
					num = 0.65f;
				}
				else if (itemType == 2 || itemType == 3 || itemType == 8 || itemType == 10 || itemType == 6 || itemType == 4 || itemType == 9)
				{
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutForward(-0.7853982f);
				}
				else if (itemType == 14)
				{
					identity.RotateAboutSide(-1.5707964f);
					identity.RotateAboutUp(3.1415927f);
				}
				else if (itemType == 15)
				{
					identity.RotateAboutSide(1.5707964f);
					identity.RotateAboutSide(-0.25132743f);
					num = 2.1f;
					vec..ctor(0f, -0.4f, 0f, -1f);
				}
				else if (itemType == 7)
				{
					identity.RotateAboutUp(2.261947f);
				}
				else if (itemType != 17)
				{
					if (itemType == 11)
					{
						identity.RotateAboutSide(-1.0995574f);
						identity.RotateAboutUp(0.7853982f);
					}
					else if (itemType == 20)
					{
						identity.RotateAboutSide(-0.62831855f);
						identity.RotateAboutUp(-0.47123894f);
					}
				}
				if (itemRosterElement.EquipmentElement.Item.IsCraftedWeapon)
				{
					num *= 0.55f;
				}
				matrixFrame = itemRosterElement.EquipmentElement.Item.GetScaledFrame(identity, itemMeshForInventory, num, vec);
				matrixFrame.origin.z = matrixFrame.origin.z - 5f;
			}
			if (itemRosterElement.EquipmentElement.Item.IsCraftedWeapon)
			{
				matrixFrame.Elevate(-0.01f * ((float)itemRosterElement.EquipmentElement.Item.WeaponComponent.PrimaryWeapon.WeaponLength / 2f));
			}
			return matrixFrame;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000050B0 File Offset: 0x000032B0
		public static void OnGetWeaponData(ref WeaponData weaponData, MissionWeapon weapon, bool isFemale, Banner banner, bool needBatchedVersion)
		{
			MetaMesh multiMesh = weapon.GetMultiMesh(isFemale, false, needBatchedVersion);
			weaponData.WeaponMesh = multiMesh;
			MetaMesh holsterMeshCopy = weapon.Item.GetHolsterMeshCopy();
			weaponData.HolsterMesh = holsterMeshCopy;
			MetaMesh holsterWithWeaponMeshCopy = weapon.Item.GetHolsterWithWeaponMeshCopy(needBatchedVersion);
			weaponData.Prefab = weapon.Item.PrefabName;
			weaponData.HolsterMeshWithWeapon = holsterWithWeaponMeshCopy;
			MetaMesh flyingMeshCopy = weapon.Item.GetFlyingMeshCopy(needBatchedVersion);
			weaponData.FlyingMesh = flyingMeshCopy;
			Material tableauMaterial = weapon.Item.GetTableauMaterial(banner);
			weaponData.TableauMaterial = tableauMaterial;
		}
	}
}
