using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000370 RID: 880
	public class TutorialArea : MissionObject
	{
		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x06003012 RID: 12306 RVA: 0x000C5770 File Offset: 0x000C3970
		public MBReadOnlyList<TrainingIcon> TrainingIconsReadOnly
		{
			get
			{
				return this._trainingIcons;
			}
		}

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x06003013 RID: 12307 RVA: 0x000C5778 File Offset: 0x000C3978
		// (set) Token: 0x06003014 RID: 12308 RVA: 0x000C5780 File Offset: 0x000C3980
		public TutorialArea.TrainingType TypeOfTraining
		{
			get
			{
				return this._typeOfTraining;
			}
			private set
			{
				this._typeOfTraining = value;
			}
		}

		// Token: 0x06003015 RID: 12309 RVA: 0x000C5789 File Offset: 0x000C3989
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.GatherWeapons();
		}

		// Token: 0x06003016 RID: 12310 RVA: 0x000C5798 File Offset: 0x000C3998
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (MBEditor.IsEntitySelected(base.GameEntity))
			{
				uint num = 4294901760U;
				using (List<TutorialArea.TutorialEntity>.Enumerator enumerator = this._tagWeapon.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TutorialArea.TutorialEntity tutorialEntity = enumerator.Current;
						foreach (Tuple<GameEntity, MatrixFrame> tuple in tutorialEntity.EntityList)
						{
							tuple.Item1.SetContourColor(new uint?(num), true);
							this._highlightedEntities.Add(tuple.Item1);
						}
					}
					return;
				}
			}
			foreach (GameEntity gameEntity in this._highlightedEntities)
			{
				gameEntity.SetContourColor(null, true);
			}
			this._highlightedEntities.Clear();
		}

		// Token: 0x06003017 RID: 12311 RVA: 0x000C58B4 File Offset: 0x000C3AB4
		protected internal override void OnInit()
		{
			base.OnInit();
			List<GameEntity> list = new List<GameEntity>();
			base.GameEntity.Scene.GetEntities(ref list);
			foreach (GameEntity gameEntity in list)
			{
				string[] tags = gameEntity.Tags;
				for (int i = 0; i < tags.Length; i++)
				{
					if (tags[i].StartsWith(this._tagPrefix) && gameEntity.HasScriptOfType<WeaponSpawner>())
					{
						gameEntity.GetFirstScriptOfType<WeaponSpawner>().SpawnWeapon();
						break;
					}
				}
			}
			this.GatherWeapons();
		}

		// Token: 0x06003018 RID: 12312 RVA: 0x000C5960 File Offset: 0x000C3B60
		public override void AfterMissionStart()
		{
			this.DeactivateAllWeapons(true);
			this.MarkTrainingIcons(false);
		}

		// Token: 0x06003019 RID: 12313 RVA: 0x000C5970 File Offset: 0x000C3B70
		private void GatherWeapons()
		{
			List<GameEntity> list = new List<GameEntity>();
			base.GameEntity.Scene.GetEntities(ref list);
			foreach (GameEntity gameEntity in list)
			{
				foreach (string text in gameEntity.Tags)
				{
					TrainingIcon firstScriptOfType = gameEntity.GetFirstScriptOfType<TrainingIcon>();
					if (firstScriptOfType != null)
					{
						if (firstScriptOfType.GetTrainingSubTypeTag().StartsWith(this._tagPrefix))
						{
							this._trainingIcons.Add(firstScriptOfType);
						}
					}
					else if (text == this._tagPrefix + "boundary")
					{
						this.AddBoundary(gameEntity);
					}
					else if (text.StartsWith(this._tagPrefix))
					{
						this.AddTaggedWeapon(gameEntity, text);
					}
				}
			}
		}

		// Token: 0x0600301A RID: 12314 RVA: 0x000C5A60 File Offset: 0x000C3C60
		public void MarkTrainingIcons(bool mark)
		{
			foreach (TrainingIcon trainingIcon in this._trainingIcons)
			{
				trainingIcon.SetMarked(mark);
			}
		}

		// Token: 0x0600301B RID: 12315 RVA: 0x000C5AB4 File Offset: 0x000C3CB4
		public TrainingIcon GetActiveTrainingIcon()
		{
			foreach (TrainingIcon trainingIcon in this._trainingIcons)
			{
				if (trainingIcon.GetIsActivated())
				{
					return trainingIcon;
				}
			}
			return null;
		}

		// Token: 0x0600301C RID: 12316 RVA: 0x000C5B10 File Offset: 0x000C3D10
		private void AddBoundary(GameEntity boundary)
		{
			this._boundaries.Add(boundary);
		}

		// Token: 0x0600301D RID: 12317 RVA: 0x000C5B20 File Offset: 0x000C3D20
		private void AddTaggedWeapon(GameEntity weapon, string tag)
		{
			if (weapon.HasScriptOfType<VolumeBox>())
			{
				this._volumeBoxes.Add(weapon.GetFirstScriptOfType<VolumeBox>());
				return;
			}
			bool flag = false;
			foreach (TutorialArea.TutorialEntity tutorialEntity in this._tagWeapon)
			{
				if (tutorialEntity.Tag == tag)
				{
					tutorialEntity.EntityList.Add(Tuple.Create<GameEntity, MatrixFrame>(weapon, weapon.GetGlobalFrame()));
					if (weapon.HasScriptOfType<DestructableComponent>())
					{
						tutorialEntity.DestructableComponents.Add(weapon.GetFirstScriptOfType<DestructableComponent>());
					}
					else if (weapon.HasScriptOfType<SpawnedItemEntity>())
					{
						tutorialEntity.WeaponList.Add(weapon);
						tutorialEntity.WeaponNames.Add(MBObjectManager.Instance.GetObject<ItemObject>(weapon.GetFirstScriptOfType<SpawnedItemEntity>().WeaponCopy.Item.StringId));
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this._tagWeapon.Add(new TutorialArea.TutorialEntity(tag, new List<Tuple<GameEntity, MatrixFrame>> { Tuple.Create<GameEntity, MatrixFrame>(weapon, weapon.GetGlobalFrame()) }, new List<DestructableComponent>(), new List<GameEntity>(), new List<ItemObject>()));
				if (weapon.HasScriptOfType<DestructableComponent>())
				{
					this._tagWeapon[this._tagWeapon.Count - 1].DestructableComponents.Add(weapon.GetFirstScriptOfType<DestructableComponent>());
					return;
				}
				if (weapon.HasScriptOfType<SpawnedItemEntity>())
				{
					this._tagWeapon[this._tagWeapon.Count - 1].WeaponList.Add(weapon);
					this._tagWeapon[this._tagWeapon.Count - 1].WeaponNames.Add(MBObjectManager.Instance.GetObject<ItemObject>(weapon.GetFirstScriptOfType<SpawnedItemEntity>().WeaponCopy.Item.StringId));
				}
			}
		}

		// Token: 0x0600301E RID: 12318 RVA: 0x000C5CF0 File Offset: 0x000C3EF0
		public int GetIndexFromTag(string tag)
		{
			for (int i = 0; i < this._tagWeapon.Count; i++)
			{
				if (this._tagWeapon[i].Tag == tag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600301F RID: 12319 RVA: 0x000C5D30 File Offset: 0x000C3F30
		public List<string> GetSubTrainingTags()
		{
			List<string> list = new List<string>();
			foreach (TutorialArea.TutorialEntity tutorialEntity in this._tagWeapon)
			{
				list.Add(tutorialEntity.Tag);
			}
			return list;
		}

		// Token: 0x06003020 RID: 12320 RVA: 0x000C5D90 File Offset: 0x000C3F90
		public void ActivateTaggedWeapons(int index)
		{
			if (index >= this._tagWeapon.Count)
			{
				return;
			}
			this.DeactivateAllWeapons(false);
			foreach (Tuple<GameEntity, MatrixFrame> tuple in this._tagWeapon[index].EntityList)
			{
				tuple.Item1.SetVisibilityExcludeParents(true);
			}
		}

		// Token: 0x06003021 RID: 12321 RVA: 0x000C5E08 File Offset: 0x000C4008
		public void EquipWeaponsToPlayer(int index)
		{
			foreach (GameEntity gameEntity in this._tagWeapon[index].WeaponList)
			{
				bool flag;
				Agent.Main.OnItemPickup(gameEntity.GetFirstScriptOfType<SpawnedItemEntity>(), EquipmentIndex.None, out flag);
			}
		}

		// Token: 0x06003022 RID: 12322 RVA: 0x000C5E74 File Offset: 0x000C4074
		public void DeactivateAllWeapons(bool resetDestructibles)
		{
			foreach (TutorialArea.TutorialEntity tutorialEntity in this._tagWeapon)
			{
				if (resetDestructibles)
				{
					foreach (DestructableComponent destructableComponent in tutorialEntity.DestructableComponents)
					{
						destructableComponent.Reset();
						destructableComponent.HitPoint = 1000000f;
						Markable firstScriptOfType = destructableComponent.GameEntity.GetFirstScriptOfType<Markable>();
						if (firstScriptOfType != null)
						{
							firstScriptOfType.DisableMarkerActivation();
						}
					}
				}
				foreach (Tuple<GameEntity, MatrixFrame> tuple in tutorialEntity.EntityList)
				{
					if (!tuple.Item1.HasScriptOfType<DestructableComponent>())
					{
						if (tuple.Item1.HasScriptOfType<SpawnedItemEntity>())
						{
							tuple.Item1.GetFirstScriptOfType<SpawnedItemEntity>().StopPhysicsAndSetFrameForClient(tuple.Item2, null);
							tuple.Item1.GetFirstScriptOfType<SpawnedItemEntity>().HasLifeTime = false;
						}
						GameEntity item = tuple.Item1;
						MatrixFrame item2 = tuple.Item2;
						item.SetGlobalFrame(item2);
					}
					tuple.Item1.SetVisibilityExcludeParents(false);
				}
			}
			this.HideBoundaries();
		}

		// Token: 0x06003023 RID: 12323 RVA: 0x000C5FFC File Offset: 0x000C41FC
		public void ActivateBoundaries()
		{
			if (this._boundariesHidden)
			{
				foreach (GameEntity gameEntity in this._boundaries)
				{
					gameEntity.SetVisibilityExcludeParents(true);
				}
				this._boundariesHidden = false;
			}
		}

		// Token: 0x06003024 RID: 12324 RVA: 0x000C605C File Offset: 0x000C425C
		public void HideBoundaries()
		{
			if (!this._boundariesHidden)
			{
				foreach (GameEntity gameEntity in this._boundaries)
				{
					gameEntity.SetVisibilityExcludeParents(false);
				}
				this._boundariesHidden = true;
			}
		}

		// Token: 0x06003025 RID: 12325 RVA: 0x000C60BC File Offset: 0x000C42BC
		public int GetBreakablesCount(int index)
		{
			return this._tagWeapon[index].DestructableComponents.Count;
		}

		// Token: 0x06003026 RID: 12326 RVA: 0x000C60D4 File Offset: 0x000C42D4
		public void MakeDestructible(int index)
		{
			for (int i = 0; i < this._tagWeapon[index].DestructableComponents.Count; i++)
			{
				this._tagWeapon[index].DestructableComponents[i].HitPoint = this._tagWeapon[index].DestructableComponents[i].MaxHitPoint;
			}
		}

		// Token: 0x06003027 RID: 12327 RVA: 0x000C613C File Offset: 0x000C433C
		public void MarkAllTargets(int index, bool mark)
		{
			foreach (DestructableComponent destructableComponent in this._tagWeapon[index].DestructableComponents)
			{
				if (mark)
				{
					Markable firstScriptOfType = destructableComponent.GameEntity.GetFirstScriptOfType<Markable>();
					if (firstScriptOfType != null)
					{
						firstScriptOfType.ActivateMarkerFor(3f, 10f);
					}
				}
				else
				{
					Markable firstScriptOfType2 = destructableComponent.GameEntity.GetFirstScriptOfType<Markable>();
					if (firstScriptOfType2 != null)
					{
						firstScriptOfType2.DisableMarkerActivation();
					}
				}
			}
		}

		// Token: 0x06003028 RID: 12328 RVA: 0x000C61D0 File Offset: 0x000C43D0
		public void ResetMarkingTargetTimers(int index)
		{
			foreach (DestructableComponent destructableComponent in this._tagWeapon[index].DestructableComponents)
			{
				Markable firstScriptOfType = destructableComponent.GameEntity.GetFirstScriptOfType<Markable>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.ResetPassiveDurationTimer();
				}
			}
		}

		// Token: 0x06003029 RID: 12329 RVA: 0x000C623C File Offset: 0x000C443C
		public void MakeInDestructible(int index)
		{
			for (int i = 0; i < this._tagWeapon[index].DestructableComponents.Count; i++)
			{
				this._tagWeapon[index].DestructableComponents[i].HitPoint = 1000000f;
			}
		}

		// Token: 0x0600302A RID: 12330 RVA: 0x000C628C File Offset: 0x000C448C
		public bool AllBreakablesAreBroken(int index)
		{
			for (int i = 0; i < this._tagWeapon[index].DestructableComponents.Count; i++)
			{
				if (!this._tagWeapon[index].DestructableComponents[i].IsDestroyed)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600302B RID: 12331 RVA: 0x000C62DC File Offset: 0x000C44DC
		public int GetBrokenBreakableCount(int index)
		{
			int num = 0;
			for (int i = 0; i < this._tagWeapon[index].DestructableComponents.Count; i++)
			{
				if (this._tagWeapon[index].DestructableComponents[i].IsDestroyed)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600302C RID: 12332 RVA: 0x000C6330 File Offset: 0x000C4530
		public int GetUnbrokenBreakableCount(int index)
		{
			int num = 0;
			for (int i = 0; i < this._tagWeapon[index].DestructableComponents.Count; i++)
			{
				if (!this._tagWeapon[index].DestructableComponents[i].IsDestroyed)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600302D RID: 12333 RVA: 0x000C6384 File Offset: 0x000C4584
		public void ResetBreakables(int index, bool makeIndestructible = true)
		{
			for (int i = 0; i < this._tagWeapon[index].DestructableComponents.Count; i++)
			{
				if (makeIndestructible)
				{
					this._tagWeapon[index].DestructableComponents[i].HitPoint = 1000000f;
				}
				this._tagWeapon[index].DestructableComponents[i].Reset();
			}
		}

		// Token: 0x0600302E RID: 12334 RVA: 0x000C63F4 File Offset: 0x000C45F4
		public bool HasMainAgentPickedAll(int index)
		{
			using (List<GameEntity>.Enumerator enumerator = this._tagWeapon[index].WeaponList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HasScriptOfType<SpawnedItemEntity>())
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600302F RID: 12335 RVA: 0x000C6458 File Offset: 0x000C4658
		public void CheckMainAgentEquipment(int index)
		{
			this._allowedWeaponsHelper.Clear();
			this._allowedWeaponsHelper.AddRange(this._tagWeapon[index].WeaponNames);
			EquipmentIndex i;
			EquipmentIndex j;
			for (i = EquipmentIndex.WeaponItemBeginSlot; i <= EquipmentIndex.Weapon3; i = j + 1)
			{
				if (!Mission.Current.MainAgent.Equipment[i].IsEmpty)
				{
					if (this._allowedWeaponsHelper.Exists((ItemObject x) => x == Mission.Current.MainAgent.Equipment[i].Item))
					{
						this._allowedWeaponsHelper.Remove(Mission.Current.MainAgent.Equipment[i].Item);
					}
					else
					{
						Mission.Current.MainAgent.DropItem(i, WeaponClass.Undefined);
						MBInformationManager.AddQuickInformation(new TextObject("{=3PP01vFv}Keep away from that weapon.", null), 0, null, "");
					}
				}
				j = i;
			}
		}

		// Token: 0x06003030 RID: 12336 RVA: 0x000C6554 File Offset: 0x000C4754
		public void CheckWeapons(int index)
		{
			foreach (GameEntity gameEntity in this._tagWeapon[index].WeaponList)
			{
				if (gameEntity.HasScriptOfType<SpawnedItemEntity>())
				{
					gameEntity.GetFirstScriptOfType<SpawnedItemEntity>().HasLifeTime = false;
				}
			}
		}

		// Token: 0x06003031 RID: 12337 RVA: 0x000C65C0 File Offset: 0x000C47C0
		public bool IsPositionInsideTutorialArea(Vec3 position, out string[] volumeBoxTags)
		{
			foreach (VolumeBox volumeBox in this._volumeBoxes)
			{
				if (volumeBox.IsPointIn(position))
				{
					volumeBoxTags = volumeBox.GameEntity.Tags;
					return true;
				}
			}
			volumeBoxTags = null;
			return false;
		}

		// Token: 0x04001403 RID: 5123
		[EditableScriptComponentVariable(true)]
		private TutorialArea.TrainingType _typeOfTraining;

		// Token: 0x04001404 RID: 5124
		[EditableScriptComponentVariable(true)]
		private string _tagPrefix = "A_";

		// Token: 0x04001405 RID: 5125
		private readonly List<TutorialArea.TutorialEntity> _tagWeapon = new List<TutorialArea.TutorialEntity>();

		// Token: 0x04001406 RID: 5126
		private readonly List<VolumeBox> _volumeBoxes = new List<VolumeBox>();

		// Token: 0x04001407 RID: 5127
		private readonly List<GameEntity> _boundaries = new List<GameEntity>();

		// Token: 0x04001408 RID: 5128
		private bool _boundariesHidden;

		// Token: 0x04001409 RID: 5129
		private readonly List<GameEntity> _highlightedEntities = new List<GameEntity>();

		// Token: 0x0400140A RID: 5130
		private readonly List<ItemObject> _allowedWeaponsHelper = new List<ItemObject>();

		// Token: 0x0400140B RID: 5131
		private readonly MBList<TrainingIcon> _trainingIcons = new MBList<TrainingIcon>();

		// Token: 0x02000682 RID: 1666
		public enum TrainingType
		{
			// Token: 0x04002132 RID: 8498
			Bow,
			// Token: 0x04002133 RID: 8499
			Melee,
			// Token: 0x04002134 RID: 8500
			Mounted,
			// Token: 0x04002135 RID: 8501
			AdvancedMelee
		}

		// Token: 0x02000683 RID: 1667
		private struct TutorialEntity
		{
			// Token: 0x06003EBB RID: 16059 RVA: 0x000F5DEA File Offset: 0x000F3FEA
			public TutorialEntity(string tag, List<Tuple<GameEntity, MatrixFrame>> entityList, List<DestructableComponent> destructableComponents, List<GameEntity> weapon, List<ItemObject> weaponNames)
			{
				this.Tag = tag;
				this.EntityList = entityList;
				this.DestructableComponents = destructableComponents;
				this.WeaponList = weapon;
				this.WeaponNames = weaponNames;
			}

			// Token: 0x04002136 RID: 8502
			public string Tag;

			// Token: 0x04002137 RID: 8503
			public List<Tuple<GameEntity, MatrixFrame>> EntityList;

			// Token: 0x04002138 RID: 8504
			public List<DestructableComponent> DestructableComponents;

			// Token: 0x04002139 RID: 8505
			public List<GameEntity> WeaponList;

			// Token: 0x0400213A RID: 8506
			public List<ItemObject> WeaponNames;
		}
	}
}
