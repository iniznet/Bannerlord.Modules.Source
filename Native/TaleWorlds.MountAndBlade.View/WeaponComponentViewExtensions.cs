using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x0200001E RID: 30
	public static class WeaponComponentViewExtensions
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x00007E78 File Offset: 0x00006078
		public static MetaMesh GetFlyingMeshCopy(this WeaponComponentData weaponComponentData, ItemObject item)
		{
			if (item.WeaponDesign != null)
			{
				if (!weaponComponentData.IsRangedWeapon || !weaponComponentData.IsConsumable)
				{
					return null;
				}
				MetaMesh weaponMesh = CraftedDataViewManager.GetCraftedDataView(item.WeaponDesign).WeaponMesh;
				if (!(weaponMesh != null))
				{
					return null;
				}
				return weaponMesh.CreateCopy();
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

		// Token: 0x060000F8 RID: 248 RVA: 0x00007EE4 File Offset: 0x000060E4
		public static MetaMesh GetFlyingMeshIfExists(this WeaponComponentData weaponComponentData, ItemObject item)
		{
			if (item.WeaponDesign != null && weaponComponentData.IsRangedWeapon && weaponComponentData.IsConsumable)
			{
				return CraftedDataViewManager.GetCraftedDataView(item.WeaponDesign).WeaponMesh;
			}
			return null;
		}
	}
}
