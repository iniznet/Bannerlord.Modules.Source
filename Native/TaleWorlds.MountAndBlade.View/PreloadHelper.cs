using System;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000013 RID: 19
	public class PreloadHelper
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00005AF8 File Offset: 0x00003CF8
		public void PreloadCharacters(List<BasicCharacterObject> characters)
		{
			Utilities.EnableGlobalEditDataCacher();
			foreach (BasicCharacterObject basicCharacterObject in characters)
			{
				foreach (Equipment equipment in basicCharacterObject.AllEquipments)
				{
					if (Mission.Current == null || Mission.Current.DoesMissionRequireCivilianEquipment || !equipment.IsCivilian)
					{
						this.AddEquipment(equipment);
					}
				}
				if (Mission.Current != null)
				{
					List<EquipmentElement> extraEquipmentElementsForCharacter = Mission.Current.GetExtraEquipmentElementsForCharacter(basicCharacterObject, true);
					if (extraEquipmentElementsForCharacter != null)
					{
						int count = extraEquipmentElementsForCharacter.Count;
						for (int i = 0; i < count; i++)
						{
							ItemObject item = extraEquipmentElementsForCharacter[i].Item;
							this.AddItemObject(item);
						}
					}
				}
			}
			Utilities.DisableGlobalEditDataCacher();
			this.PreloadMeshesAndPhysics();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005C04 File Offset: 0x00003E04
		public void WaitForMeshesToBeLoaded()
		{
			int num;
			do
			{
				num = 0;
				foreach (ValueTuple<MetaMesh, bool, bool> valueTuple in this._uniqueMetaMeshes)
				{
					num += valueTuple.Item1.CheckResources();
				}
				foreach (string text in this._uniqueDynamicPhysicsShapeName)
				{
					num += ((PhysicsShape.GetFromResource(text, true) == null) ? 1 : 0);
				}
				Thread.Sleep(1);
			}
			while (num != 0);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00005CC0 File Offset: 0x00003EC0
		public void PreloadEquipments(List<Equipment> equipments)
		{
			foreach (Equipment equipment in equipments)
			{
				this.AddEquipment(equipment);
			}
			this.PreloadMeshesAndPhysics();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005D14 File Offset: 0x00003F14
		public void PreloadItems(List<ItemObject> items)
		{
			Utilities.EnableGlobalEditDataCacher();
			for (int i = 0; i < items.Count; i++)
			{
				this.AddItemObject(items[i]);
			}
			Utilities.DisableGlobalEditDataCacher();
			this.PreloadMeshesAndPhysics();
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00005D50 File Offset: 0x00003F50
		private void AddEquipment(Equipment equipment)
		{
			for (int i = 0; i < 12; i++)
			{
				ItemObject item = equipment.GetEquipmentFromSlot(i).Item;
				if (item != null)
				{
					this.AddItemObject(item);
				}
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005D84 File Offset: 0x00003F84
		private void AddItemObject(ItemObject item)
		{
			if (item == null)
			{
				return;
			}
			bool isUsingTableau = item.IsUsingTableau;
			bool isUsingTeamColor = item.IsUsingTeamColor;
			this.RegisterMetaMeshUsageIfValid(item.MultiMeshName, isUsingTableau, isUsingTeamColor);
			this.RegisterMetaMeshUsageIfValid(item.HolsterMeshName, isUsingTableau, isUsingTeamColor);
			if (item.WeaponComponent != null)
			{
				if (item.IsCraftedWeapon)
				{
					this.RegisterMetaMeshUsageIfValid(item.GetHolsterWithWeaponMeshIfExists(), isUsingTableau, isUsingTeamColor);
					this.RegisterMetaMeshUsageIfValid(item.GetHolsterMeshIfExists(), isUsingTableau, isUsingTeamColor);
					this.RegisterMetaMeshUsageIfValid(item.GetFlyingMeshIfExists(), isUsingTableau, isUsingTeamColor);
				}
				else
				{
					this.RegisterMetaMeshUsageIfValid(item.HolsterWithWeaponMeshName, isUsingTableau, isUsingTeamColor);
					this.RegisterMetaMeshUsageIfValid(item.HolsterMeshName, isUsingTableau, isUsingTeamColor);
					this.RegisterMetaMeshUsageIfValid(item.FlyingMeshName, isUsingTableau, isUsingTeamColor);
				}
			}
			else
			{
				if (item.HasHorseComponent)
				{
					using (List<KeyValuePair<string, bool>>.Enumerator enumerator = item.HorseComponent.AdditionalMeshesNameList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, bool> keyValuePair = enumerator.Current;
							this.RegisterMetaMeshUsageIfValid(keyValuePair.Key, isUsingTableau, isUsingTeamColor);
						}
						goto IL_127;
					}
				}
				if (item.HasArmorComponent && !string.IsNullOrEmpty(item.ArmorComponent.ReinsMesh))
				{
					this.RegisterMetaMeshUsageIfValid(item.ArmorComponent.ReinsMesh, isUsingTableau, isUsingTeamColor);
					this.RegisterMetaMeshUsageIfValid(item.ArmorComponent.ReinsRopeMesh, isUsingTableau, isUsingTeamColor);
				}
			}
			IL_127:
			if (!this._loadedItems.Contains(item))
			{
				this.RegisterMetaMeshUsageIfValid(item.GetMultiMesh(false, false, true), isUsingTableau, isUsingTeamColor);
				if (item.HasArmorComponent)
				{
					this.RegisterMetaMeshUsageIfValid(item.GetMultiMesh(false, true, true), isUsingTableau, isUsingTeamColor);
					this.RegisterMetaMeshUsageIfValid(item.GetMultiMesh(true, false, true), isUsingTableau, isUsingTeamColor);
					this.RegisterMetaMeshUsageIfValid(item.GetMultiMesh(true, true, true), isUsingTableau, isUsingTeamColor);
				}
				this._loadedItems.Add(item);
			}
			this.RegisterPhysicsBodyUsageIfValid(this._uniqueDynamicPhysicsShapeName, item.CollisionBodyName);
			this.RegisterPhysicsBodyUsageIfValid(this._uniqueDynamicPhysicsShapeName, item.BodyName);
			this.RegisterPhysicsBodyUsageIfValid(this._uniqueDynamicPhysicsShapeName, item.HolsterBodyName);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005F68 File Offset: 0x00004168
		private void PreloadMeshesAndPhysics()
		{
			foreach (ValueTuple<MetaMesh, bool, bool> valueTuple in this._uniqueMetaMeshes)
			{
				MetaMesh item = valueTuple.Item1;
				item.PreloadForRendering();
				item.PreloadShaders(valueTuple.Item2, valueTuple.Item3);
			}
			foreach (string text in this._uniqueDynamicPhysicsShapeName)
			{
				PhysicsShape.AddPreloadQueueWithName(text, new Vec3(1f, 1f, 1f, -1f));
			}
			PhysicsShape.ProcessPreloadQueue();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00006030 File Offset: 0x00004230
		public void Clear()
		{
			this._uniqueMetaMeshNames.Clear();
			this._uniqueDynamicPhysicsShapeName.Clear();
			this._uniqueMetaMeshes.Clear();
			PhysicsShape.UnloadDynamicBodies();
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00006058 File Offset: 0x00004258
		private void RegisterMetaMeshUsageIfValid(string metaMeshName, bool useTableau, bool useTeamColor)
		{
			ValueTuple<string, bool, bool> valueTuple = new ValueTuple<string, bool, bool>(metaMeshName, useTableau, useTeamColor);
			if (!string.IsNullOrEmpty(metaMeshName) && !this._uniqueMetaMeshNames.Contains(valueTuple))
			{
				this.RegisterMetaMeshUsageIfValid(MetaMesh.GetCopy(metaMeshName, false, true), useTableau, useTeamColor);
				this._uniqueMetaMeshNames.Add(valueTuple);
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000060A4 File Offset: 0x000042A4
		private void RegisterMetaMeshUsageIfValid(MetaMesh metaMesh, bool useTableau, bool useTeamColor)
		{
			if (metaMesh != null)
			{
				ValueTuple<MetaMesh, bool, bool> valueTuple = new ValueTuple<MetaMesh, bool, bool>(metaMesh, useTableau, useTeamColor);
				this._uniqueMetaMeshes.Add(valueTuple);
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000060D1 File Offset: 0x000042D1
		private void RegisterPhysicsBodyUsageIfValid(HashSet<string> uniquePhysicsShapeName, string physicsShape)
		{
			if (!string.IsNullOrWhiteSpace(physicsShape))
			{
				uniquePhysicsShapeName.Add(physicsShape);
			}
		}

		// Token: 0x04000017 RID: 23
		private readonly HashSet<ValueTuple<string, bool, bool>> _uniqueMetaMeshNames = new HashSet<ValueTuple<string, bool, bool>>();

		// Token: 0x04000018 RID: 24
		private readonly HashSet<string> _uniqueDynamicPhysicsShapeName = new HashSet<string>();

		// Token: 0x04000019 RID: 25
		private readonly HashSet<ValueTuple<MetaMesh, bool, bool>> _uniqueMetaMeshes = new HashSet<ValueTuple<MetaMesh, bool, bool>>();

		// Token: 0x0400001A RID: 26
		private readonly HashSet<ItemObject> _loadedItems = new HashSet<ItemObject>();
	}
}
