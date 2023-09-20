using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x0200003C RID: 60
	public class CharacterSpawner : ScriptComponentBehavior
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060002BD RID: 701 RVA: 0x000189FD File Offset: 0x00016BFD
		// (set) Token: 0x060002BE RID: 702 RVA: 0x00018A05 File Offset: 0x00016C05
		public uint ClothColor1 { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060002BF RID: 703 RVA: 0x00018A0E File Offset: 0x00016C0E
		// (set) Token: 0x060002C0 RID: 704 RVA: 0x00018A16 File Offset: 0x00016C16
		public uint ClothColor2 { get; private set; }

		// Token: 0x060002C1 RID: 705 RVA: 0x00018A20 File Offset: 0x00016C20
		~CharacterSpawner()
		{
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00018A48 File Offset: 0x00016C48
		protected override void OnInit()
		{
			base.OnInit();
			this.ClothColor1 = uint.MaxValue;
			this.ClothColor2 = uint.MaxValue;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00018A5E File Offset: 0x00016C5E
		protected void Init()
		{
			this.Active = false;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00018A67 File Offset: 0x00016C67
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			if (Game.Current == null)
			{
				this._editorGameManager = new EditorGameManager();
				this.isFinished = !this._editorGameManager.DoLoadingForGameManager();
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00018A98 File Offset: 0x00016C98
		protected override void OnEditorTick(float dt)
		{
			if (!this.Enabled)
			{
				return;
			}
			if (!this.isFinished && this._editorGameManager != null)
			{
				this.isFinished = !this._editorGameManager.DoLoadingForGameManager();
			}
			if (Game.Current != null && this._agentVisuals == null)
			{
				this.SpawnCharacter();
				base.GameEntity.SetVisibilityExcludeParents(false);
				if (this._agentEntity != null)
				{
					this._agentEntity.SetVisibilityExcludeParents(false);
				}
				if (this._horseEntity != null)
				{
					this._horseEntity.SetVisibilityExcludeParents(false);
				}
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00018B27 File Offset: 0x00016D27
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			if (this._agentVisuals != null)
			{
				this._agentVisuals.Reset();
				this._agentVisuals.GetVisuals().ManualInvalidate();
				this._agentVisuals = null;
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00018B5A File Offset: 0x00016D5A
		public void SetCreateFaceImmediately(bool value)
		{
			this.CreateFaceImmediately = value;
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00018B64 File Offset: 0x00016D64
		private void Disable()
		{
			if (this._agentEntity != null && this._agentEntity.Parent == base.GameEntity)
			{
				base.GameEntity.RemoveChild(this._agentEntity, false, false, true, 34);
			}
			if (this._agentVisuals != null)
			{
				this._agentVisuals.Reset();
				this._agentVisuals.GetVisuals().ManualInvalidate();
				this._agentVisuals = null;
			}
			if (this._horseEntity != null && this._horseEntity.Parent == base.GameEntity)
			{
				this._horseEntity.Scene.RemoveEntity(this._horseEntity, 96);
			}
			this.Active = false;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x00018C1C File Offset: 0x00016E1C
		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "Enabled")
			{
				if (this.Enabled)
				{
					this.Init();
				}
				else
				{
					this.Disable();
				}
			}
			if (!this.Enabled)
			{
				return;
			}
			if (variableName == "LordName" || variableName == "ActionSetSuffix")
			{
				if (this._agentVisuals != null)
				{
					BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
					if (@object != null)
					{
						this.InitWithCharacter(CharacterCode.CreateFrom(@object), true);
						return;
					}
				}
			}
			else if (variableName == "PoseActionForHorse")
			{
				BasicCharacterObject object2 = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
				if (object2 != null)
				{
					this.InitWithCharacter(CharacterCode.CreateFrom(object2), true);
					return;
				}
			}
			else if (variableName == "PoseAction")
			{
				if (this._agentVisuals != null)
				{
					BasicCharacterObject object3 = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
					if (object3 != null)
					{
						this.InitWithCharacter(CharacterCode.CreateFrom(object3), true);
						return;
					}
				}
			}
			else
			{
				if (variableName == "IsWeaponWielded")
				{
					BasicCharacterObject object4 = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
					this.WieldWeapon(CharacterCode.CreateFrom(object4));
					return;
				}
				if (variableName == "AnimationProgress")
				{
					Skeleton skeleton = this._agentVisuals.GetVisuals().GetSkeleton();
					skeleton.Freeze(false);
					skeleton.TickAnimationsAndForceUpdate(0.001f, this._agentVisuals.GetVisuals().GetGlobalFrame(), false);
					skeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat(this.AnimationProgress, 0f, 1f));
					skeleton.SetUptoDate(false);
					skeleton.Freeze(true);
					return;
				}
				if (variableName == "HorseAnimationProgress")
				{
					if (this._horseEntity != null)
					{
						this._horseEntity.Skeleton.Freeze(false);
						this._horseEntity.Skeleton.TickAnimationsAndForceUpdate(0.001f, this._horseEntity.GetGlobalFrame(), false);
						this._horseEntity.Skeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat(this.HorseAnimationProgress, 0f, 1f));
						this._horseEntity.Skeleton.SetUptoDate(false);
						this._horseEntity.Skeleton.Freeze(true);
						return;
					}
				}
				else if (variableName == "HasMount")
				{
					if (this.HasMount)
					{
						BasicCharacterObject object5 = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
						this.SpawnMount(CharacterCode.CreateFrom(object5));
						return;
					}
					if (this._horseEntity != null)
					{
						this._horseEntity.Scene.RemoveEntity(this._horseEntity, 97);
						return;
					}
				}
				else if (variableName == "Active")
				{
					base.GameEntity.SetVisibilityExcludeParents(this.Active);
					if (this._agentEntity != null)
					{
						this._agentEntity.SetVisibilityExcludeParents(this.Active);
					}
					if (this._horseEntity != null)
					{
						this._horseEntity.SetVisibilityExcludeParents(this.Active);
						return;
					}
				}
				else if (variableName == "FaceKeyString")
				{
					BasicCharacterObject object6 = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
					if (object6 != null)
					{
						this.InitWithCharacter(CharacterCode.CreateFrom(object6), true);
						return;
					}
				}
				else if (variableName == "WieldOffHand")
				{
					BasicCharacterObject object7 = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
					if (object7 != null)
					{
						this.InitWithCharacter(CharacterCode.CreateFrom(object7), true);
					}
				}
			}
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00018F88 File Offset: 0x00017188
		public void SetClothColors(uint color1, uint color2)
		{
			this.ClothColor1 = color1;
			this.ClothColor2 = color2;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00018F98 File Offset: 0x00017198
		public void SpawnCharacter()
		{
			BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
			if (@object != null)
			{
				CharacterCode characterCode = CharacterCode.CreateFrom(@object);
				this.InitWithCharacter(characterCode, true);
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00018FD0 File Offset: 0x000171D0
		public void InitWithCharacter(CharacterCode characterCode, bool useBodyProperties = false)
		{
			base.GameEntity.BreakPrefab();
			if (this._agentEntity != null && this._agentEntity.Parent == base.GameEntity)
			{
				base.GameEntity.RemoveChild(this._agentEntity, false, false, true, 35);
			}
			AgentVisuals agentVisuals = this._agentVisuals;
			if (agentVisuals != null)
			{
				agentVisuals.Reset();
			}
			AgentVisuals agentVisuals2 = this._agentVisuals;
			if (agentVisuals2 != null)
			{
				MBAgentVisuals visuals = agentVisuals2.GetVisuals();
				if (visuals != null)
				{
					visuals.ManualInvalidate();
				}
			}
			if (this._horseEntity != null && this._horseEntity.Parent == base.GameEntity)
			{
				this._horseEntity.Scene.RemoveEntity(this._horseEntity, 98);
			}
			this._agentEntity = GameEntity.CreateEmpty(base.GameEntity.Scene, false);
			this._agentEntity.Name = "TableauCharacterAgentVisualsEntity";
			this._spawnFrame = this._agentEntity.GetFrame();
			BodyProperties bodyProperties = characterCode.BodyProperties;
			if (useBodyProperties)
			{
				BodyProperties.FromString(this.BodyPropertiesString, ref bodyProperties);
			}
			if (characterCode.Color1 != 4294967295U)
			{
				this.ClothColor1 = characterCode.Color1;
			}
			if (characterCode.Color2 != 4294967295U)
			{
				this.ClothColor2 = characterCode.Color2;
			}
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(characterCode.Race);
			this._agentVisuals = AgentVisuals.Create(new AgentVisualsData().Equipment(characterCode.CalculateEquipment()).BodyProperties(bodyProperties).Race(characterCode.Race)
				.Frame(this._spawnFrame)
				.Scale(1f)
				.SkeletonType(characterCode.IsFemale ? 1 : 0)
				.Entity(this._agentEntity)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, characterCode.IsFemale, this.ActionSetSuffix))
				.ActionCode(ActionIndexCache.Create("act_inventory_idle_start"))
				.Scene(base.GameEntity.Scene)
				.Monster(baseMonsterFromRace)
				.PrepareImmediately(this.CreateFaceImmediately)
				.Banner(characterCode.Banner)
				.ClothColor1(this.ClothColor1)
				.ClothColor2(this.ClothColor2)
				.UseMorphAnims(true), "TableauCharacterAgentVisuals", false, false, false);
			this._agentVisuals.SetAction(ActionIndexCache.Create(this.PoseAction), MBMath.ClampFloat(this.AnimationProgress, 0f, 1f), true);
			base.GameEntity.AddChild(this._agentEntity, false);
			this.WieldWeapon(characterCode);
			MatrixFrame identity = MatrixFrame.Identity;
			this._agentVisuals.GetVisuals().SetFrame(ref identity);
			if (this.HasMount)
			{
				this.SpawnMount(characterCode);
			}
			base.GameEntity.SetVisibilityExcludeParents(true);
			this._agentEntity.SetVisibilityExcludeParents(true);
			if (this._horseEntity != null)
			{
				this._horseEntity.SetVisibilityExcludeParents(true);
			}
			Skeleton skeleton = this._agentVisuals.GetVisuals().GetSkeleton();
			skeleton.Freeze(false);
			skeleton.TickAnimationsAndForceUpdate(0.001f, this._agentVisuals.GetVisuals().GetGlobalFrame(), false);
			skeleton.SetUptoDate(false);
			skeleton.Freeze(true);
			this._agentEntity.SetBoundingboxDirty();
			skeleton.Freeze(false);
			skeleton.TickAnimationsAndForceUpdate(0.001f, this._agentVisuals.GetVisuals().GetGlobalFrame(), false);
			skeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat(this.AnimationProgress, 0f, 1f));
			skeleton.SetUptoDate(false);
			skeleton.Freeze(true);
			skeleton.ManualInvalidate();
			if (this._horseEntity != null)
			{
				this._horseEntity.Skeleton.Freeze(false);
				this._horseEntity.Skeleton.TickAnimationsAndForceUpdate(0.001f, this._horseEntity.GetGlobalFrame(), false);
				this._horseEntity.Skeleton.SetUptoDate(false);
				this._horseEntity.Skeleton.Freeze(true);
				this._horseEntity.SetBoundingboxDirty();
			}
			if (this._horseEntity != null)
			{
				this._horseEntity.Skeleton.Freeze(false);
				this._horseEntity.Skeleton.TickAnimationsAndForceUpdate(0.001f, this._horseEntity.GetGlobalFrame(), false);
				this._horseEntity.Skeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat(this.HorseAnimationProgress, 0f, 1f));
				this._horseEntity.Skeleton.SetUptoDate(false);
				this._horseEntity.Skeleton.Freeze(true);
			}
			base.GameEntity.SetBoundingboxDirty();
			if (!base.GameEntity.Scene.IsEditorScene())
			{
				if (this._agentEntity != null)
				{
					this._agentEntity.ManualInvalidate();
				}
				if (this._horseEntity != null)
				{
					this._horseEntity.ManualInvalidate();
				}
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00019468 File Offset: 0x00017668
		private void WieldWeapon(CharacterCode characterCode)
		{
			if (this.IsWeaponWielded)
			{
				WeaponFlags weaponFlags = 0L;
				switch (characterCode.FormationClass)
				{
				case 0:
				case 2:
				case 4:
				case 5:
				case 6:
				case 8:
				case 9:
					weaponFlags = 1L;
					break;
				case 1:
				case 3:
					weaponFlags = 2L;
					break;
				}
				int num = -1;
				int num2 = -1;
				Equipment equipment = characterCode.CalculateEquipment();
				for (int i = 0; i < 4; i++)
				{
					ItemObject item = equipment[i].Item;
					if (((item != null) ? item.PrimaryWeapon : null) != null)
					{
						if (num2 == -1 && Extensions.HasAnyFlag<ItemFlags>(equipment[i].Item.ItemFlags, 524288) && this.WieldOffHand)
						{
							num2 = i;
						}
						if (num == -1 && Extensions.HasAnyFlag<WeaponFlags>(equipment[i].Item.PrimaryWeapon.WeaponFlags, weaponFlags))
						{
							num = i;
						}
					}
				}
				if (num != -1 || num2 != -1)
				{
					AgentVisualsData copyAgentVisualsData = this._agentVisuals.GetCopyAgentVisualsData();
					copyAgentVisualsData.RightWieldedItemIndex(num).LeftWieldedItemIndex(num2).ActionCode(ActionIndexCache.Create(this.PoseAction))
						.Frame(this._spawnFrame);
					this._agentVisuals.Refresh(false, copyAgentVisualsData, false);
				}
			}
		}

		// Token: 0x060002CE RID: 718 RVA: 0x000195B0 File Offset: 0x000177B0
		private void SpawnMount(CharacterCode characterCode)
		{
			Equipment equipment = characterCode.CalculateEquipment();
			ItemObject item = equipment[10].Item;
			if (item == null)
			{
				this.HasMount = false;
				return;
			}
			this._horseEntity = GameEntity.CreateEmpty(base.GameEntity.Scene, false);
			this._horseEntity.Name = "MountEntity";
			Monster monster = item.HorseComponent.Monster;
			MBActionSet actionSet = MBActionSet.GetActionSet(monster.ActionSetCode);
			GameEntityExtensions.CreateAgentSkeleton(this._horseEntity, actionSet.GetSkeletonName(), false, actionSet, monster.MonsterUsage, monster);
			this._horseEntity.CopyComponentsToSkeleton();
			MBSkeletonExtensions.SetAgentActionChannel(this._horseEntity.Skeleton, 0, ActionIndexCache.Create(this.PoseActionForHorse), MBMath.ClampFloat(this.HorseAnimationProgress, 0f, 1f), -0.2f, true);
			base.GameEntity.AddChild(this._horseEntity, false);
			MountVisualCreator.AddMountMeshToEntity(this._horseEntity, equipment[10].Item, equipment[11].Item, MountCreationKey.GetRandomMountKeyString(equipment[10].Item, MBRandom.RandomInt()), null);
			this._horseEntity.SetVisibilityExcludeParents(true);
			this._horseEntity.Skeleton.TickAnimations(0.01f, this._agentVisuals.GetVisuals().GetGlobalFrame(), true);
		}

		// Token: 0x040001DE RID: 478
		public bool Enabled;

		// Token: 0x040001DF RID: 479
		public string PoseAction = "act_walk_idle_unarmed";

		// Token: 0x040001E0 RID: 480
		public string LordName = "main_hero_for_perf";

		// Token: 0x040001E1 RID: 481
		public string ActionSetSuffix = "_facegen";

		// Token: 0x040001E2 RID: 482
		public string PoseActionForHorse = "horse_stand_3";

		// Token: 0x040001E3 RID: 483
		public string BodyPropertiesString = "<BodyProperties version=\"4\" age=\"23.16\" weight=\"0.3333\" build=\"0\" key=\"00000C07000000010011111211151111000701000010000000111011000101000000500202111110000000000000000000000000000000000000000000A00000\" />";

		// Token: 0x040001E4 RID: 484
		public bool IsWeaponWielded;

		// Token: 0x040001E5 RID: 485
		public bool HasMount;

		// Token: 0x040001E6 RID: 486
		public bool WieldOffHand = true;

		// Token: 0x040001E7 RID: 487
		public float AnimationProgress;

		// Token: 0x040001E8 RID: 488
		public float HorseAnimationProgress;

		// Token: 0x040001EB RID: 491
		private MBGameManager _editorGameManager;

		// Token: 0x040001EC RID: 492
		private bool isFinished;

		// Token: 0x040001ED RID: 493
		private bool CreateFaceImmediately = true;

		// Token: 0x040001EE RID: 494
		private AgentVisuals _agentVisuals;

		// Token: 0x040001EF RID: 495
		private GameEntity _agentEntity;

		// Token: 0x040001F0 RID: 496
		private GameEntity _horseEntity;

		// Token: 0x040001F1 RID: 497
		public bool Active;

		// Token: 0x040001F2 RID: 498
		private MatrixFrame _spawnFrame;
	}
}
