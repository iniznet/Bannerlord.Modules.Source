using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View
{
	public static class WeaponComponentViewExtensions
	{
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
