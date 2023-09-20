using System;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	public class PreloadHelper
	{
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

		public void PreloadEquipments(List<Equipment> equipments)
		{
			foreach (Equipment equipment in equipments)
			{
				this.AddEquipment(equipment);
			}
			this.PreloadMeshesAndPhysics();
		}

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

		public void Clear()
		{
			this._uniqueMetaMeshNames.Clear();
			this._uniqueDynamicPhysicsShapeName.Clear();
			this._uniqueMetaMeshes.Clear();
			PhysicsShape.UnloadDynamicBodies();
		}

		private void RegisterMetaMeshUsageIfValid(string metaMeshName, bool useTableau, bool useTeamColor)
		{
			ValueTuple<string, bool, bool> valueTuple = new ValueTuple<string, bool, bool>(metaMeshName, useTableau, useTeamColor);
			if (!string.IsNullOrEmpty(metaMeshName) && !this._uniqueMetaMeshNames.Contains(valueTuple))
			{
				this.RegisterMetaMeshUsageIfValid(MetaMesh.GetCopy(metaMeshName, false, true), useTableau, useTeamColor);
				this._uniqueMetaMeshNames.Add(valueTuple);
			}
		}

		private void RegisterMetaMeshUsageIfValid(MetaMesh metaMesh, bool useTableau, bool useTeamColor)
		{
			if (metaMesh != null)
			{
				ValueTuple<MetaMesh, bool, bool> valueTuple = new ValueTuple<MetaMesh, bool, bool>(metaMesh, useTableau, useTeamColor);
				this._uniqueMetaMeshes.Add(valueTuple);
			}
		}

		private void RegisterPhysicsBodyUsageIfValid(HashSet<string> uniquePhysicsShapeName, string physicsShape)
		{
			if (!string.IsNullOrWhiteSpace(physicsShape))
			{
				uniquePhysicsShapeName.Add(physicsShape);
			}
		}

		private readonly HashSet<ValueTuple<string, bool, bool>> _uniqueMetaMeshNames = new HashSet<ValueTuple<string, bool, bool>>();

		private readonly HashSet<string> _uniqueDynamicPhysicsShapeName = new HashSet<string>();

		private readonly HashSet<ValueTuple<MetaMesh, bool, bool>> _uniqueMetaMeshes = new HashSet<ValueTuple<MetaMesh, bool, bool>>();

		private readonly HashSet<ItemObject> _loadedItems = new HashSet<ItemObject>();
	}
}
