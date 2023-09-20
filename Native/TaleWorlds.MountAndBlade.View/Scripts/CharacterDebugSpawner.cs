using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x0200003B RID: 59
	public class CharacterDebugSpawner : ScriptComponentBehavior
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060002AD RID: 685 RVA: 0x00017E67 File Offset: 0x00016067
		// (set) Token: 0x060002AE RID: 686 RVA: 0x00017E6F File Offset: 0x0001606F
		public uint ClothColor1 { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060002AF RID: 687 RVA: 0x00017E78 File Offset: 0x00016078
		// (set) Token: 0x060002B0 RID: 688 RVA: 0x00017E80 File Offset: 0x00016080
		public uint ClothColor2 { get; private set; }

		// Token: 0x060002B1 RID: 689 RVA: 0x00017E89 File Offset: 0x00016089
		protected override void OnInit()
		{
			base.OnInit();
			this.ClothColor1 = uint.MaxValue;
			this.ClothColor2 = uint.MaxValue;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00017E9F File Offset: 0x0001609F
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			if (CharacterDebugSpawner._editorGameManager == null)
			{
				CharacterDebugSpawner._editorGameManager = new EditorGameManager();
			}
			CharacterDebugSpawner._editorGameManagerRefCount++;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00017EC4 File Offset: 0x000160C4
		protected override void OnEditorTick(float dt)
		{
			if (!CharacterDebugSpawner.isFinished && CharacterDebugSpawner.gameTickFrameNo != Utilities.EngineFrameNo)
			{
				CharacterDebugSpawner.gameTickFrameNo = Utilities.EngineFrameNo;
				CharacterDebugSpawner.isFinished = !CharacterDebugSpawner._editorGameManager.DoLoadingForGameManager();
			}
			if (Game.Current != null && this._agentVisuals == null)
			{
				this.MovementDirection.x = MBRandom.RandomFloatNormal;
				this.MovementDirection.y = MBRandom.RandomFloatNormal;
				this.MovementDirection.Normalize();
				this.MovementSpeed = MBRandom.RandomFloat * 9f + 1f;
				this.PhaseDiff = MBRandom.RandomFloat;
				this.MovementDirectionChange = MBRandom.RandomFloatNormal * 3.1415927f;
				this.Time = 0f;
				this.ActionSetTimer = 0f;
				this.ActionChangeInterval = MBRandom.RandomFloat * 0.5f + 0.5f;
				this.SpawnCharacter();
			}
			MatrixFrame globalFrame = this._agentVisuals.GetVisuals().GetGlobalFrame();
			this._agentVisuals.GetVisuals().SetFrame(ref globalFrame);
			Vec3 vec;
			vec..ctor(this.MovementDirection, 0f, -1f);
			vec.RotateAboutZ(this.MovementDirectionChange * dt);
			this.MovementDirection.x = vec.x;
			this.MovementDirection.y = vec.y;
			float num = this.MovementSpeed * (MathF.Sin(this.PhaseDiff + this.Time) * 0.5f) + 2f;
			Vec2 vec2 = this.MovementDirection * num;
			this._agentVisuals.SetAgentLocalSpeed(vec2);
			this.Time += dt;
			if (this.Time - this.ActionSetTimer > this.ActionChangeInterval)
			{
				this.ActionSetTimer = this.Time;
				this._agentVisuals.SetAction(this._actionIndices[MBRandom.RandomInt(this._actionIndices.Length)], 0f, true);
			}
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x000180A5 File Offset: 0x000162A5
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this.Reset();
			CharacterDebugSpawner._editorGameManagerRefCount--;
			if (CharacterDebugSpawner._editorGameManagerRefCount == 0)
			{
				CharacterDebugSpawner._editorGameManager = null;
				CharacterDebugSpawner.isFinished = false;
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x000180D4 File Offset: 0x000162D4
		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "LordName")
			{
				BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
				AgentVisualsData copyAgentVisualsData = this._agentVisuals.GetCopyAgentVisualsData();
				copyAgentVisualsData.BodyProperties(@object.GetBodyProperties(null, -1)).SkeletonType(@object.IsFemale ? 1 : 0).ActionSet(MBGlobals.GetActionSetWithSuffix(copyAgentVisualsData.MonsterData, @object.IsFemale, "_poses"))
					.Equipment(@object.Equipment)
					.UseMorphAnims(true);
				this._agentVisuals.Refresh(false, copyAgentVisualsData, false);
				return;
			}
			if (!(variableName == "PoseAction"))
			{
				if (variableName == "IsWeaponWielded")
				{
					BasicCharacterObject object2 = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
					this.WieldWeapon(CharacterCode.CreateFrom(object2));
				}
				return;
			}
			AgentVisuals agentVisuals = this._agentVisuals;
			if (agentVisuals == null)
			{
				return;
			}
			agentVisuals.SetAction(this.PoseAction, 0f, true);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x000181CB File Offset: 0x000163CB
		public void SetClothColors(uint color1, uint color2)
		{
			this.ClothColor1 = color1;
			this.ClothColor2 = color2;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x000181DC File Offset: 0x000163DC
		public void SpawnCharacter()
		{
			BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(this.LordName);
			if (@object != null)
			{
				CharacterCode characterCode = CharacterCode.CreateFrom(@object);
				this.InitWithCharacter(characterCode);
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00018210 File Offset: 0x00016410
		public void Reset()
		{
			AgentVisuals agentVisuals = this._agentVisuals;
			if (agentVisuals == null)
			{
				return;
			}
			agentVisuals.Reset();
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00018224 File Offset: 0x00016424
		public void InitWithCharacter(CharacterCode characterCode)
		{
			GameEntity gameEntity = GameEntity.CreateEmpty(base.GameEntity.Scene, false);
			gameEntity.Name = "TableauCharacterAgentVisualsEntity";
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(characterCode.Race);
			this._agentVisuals = AgentVisuals.Create(new AgentVisualsData().Equipment(characterCode.CalculateEquipment()).BodyProperties(characterCode.BodyProperties).Race(characterCode.Race)
				.Frame(gameEntity.GetGlobalFrame())
				.SkeletonType(characterCode.IsFemale ? 1 : 0)
				.Entity(gameEntity)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, characterCode.IsFemale, "_facegen"))
				.ActionCode(this.act_inventory_idle_start)
				.Scene(base.GameEntity.Scene)
				.Monster(baseMonsterFromRace)
				.PrepareImmediately(this.CreateFaceImmediately)
				.Banner(characterCode.Banner)
				.ClothColor1(this.ClothColor1)
				.ClothColor2(this.ClothColor2), "CharacterDebugSpawner", false, false, false);
			this._agentVisuals.SetAction(this.PoseAction, MBRandom.RandomFloat, true);
			base.GameEntity.AddChild(gameEntity, false);
			this.WieldWeapon(characterCode);
			this._agentVisuals.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.1f, this._agentVisuals.GetVisuals().GetGlobalFrame(), true);
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00018370 File Offset: 0x00016570
		public void WieldWeapon(CharacterCode characterCode)
		{
			if (this.IsWeaponWielded)
			{
				int num = -1;
				int num2 = -1;
				Equipment equipment = characterCode.CalculateEquipment();
				for (int i = 0; i < 4; i++)
				{
					ItemObject item = equipment[i].Item;
					if (((item != null) ? item.PrimaryWeapon : null) != null)
					{
						if (num2 == -1 && Extensions.HasAnyFlag<ItemFlags>(equipment[i].Item.ItemFlags, 524288))
						{
							num2 = i;
						}
						if (num == -1 && Extensions.HasAnyFlag<WeaponFlags>(equipment[i].Item.PrimaryWeapon.WeaponFlags, 3L))
						{
							num = i;
						}
					}
				}
				if (num != -1 || num2 != -1)
				{
					AgentVisualsData copyAgentVisualsData = this._agentVisuals.GetCopyAgentVisualsData();
					copyAgentVisualsData.RightWieldedItemIndex(num).LeftWieldedItemIndex(num2).ActionCode(this.PoseAction);
					this._agentVisuals.Refresh(false, copyAgentVisualsData, false);
				}
			}
		}

		// Token: 0x040001CA RID: 458
		private readonly ActionIndexCache[] _actionIndices = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_start_conversation"),
			ActionIndexCache.Create("act_stand_conversation"),
			ActionIndexCache.Create("act_start_angry_conversation"),
			ActionIndexCache.Create("act_stand_angry_conversation"),
			ActionIndexCache.Create("act_start_sad_conversation"),
			ActionIndexCache.Create("act_stand_sad_conversation"),
			ActionIndexCache.Create("act_start_happy_conversation"),
			ActionIndexCache.Create("act_stand_happy_conversation"),
			ActionIndexCache.Create("act_start_busy_conversation"),
			ActionIndexCache.Create("act_stand_busy_conversation"),
			ActionIndexCache.Create("act_explaining_conversation"),
			ActionIndexCache.Create("act_introduction_conversation"),
			ActionIndexCache.Create("act_wondering_conversation"),
			ActionIndexCache.Create("act_unknown_conversation"),
			ActionIndexCache.Create("act_friendly_conversation"),
			ActionIndexCache.Create("act_offer_conversation"),
			ActionIndexCache.Create("act_negative_conversation"),
			ActionIndexCache.Create("act_affermative_conversation"),
			ActionIndexCache.Create("act_secret_conversation"),
			ActionIndexCache.Create("act_remember_conversation"),
			ActionIndexCache.Create("act_laugh_conversation"),
			ActionIndexCache.Create("act_threat_conversation"),
			ActionIndexCache.Create("act_scared_conversation"),
			ActionIndexCache.Create("act_flirty_conversation"),
			ActionIndexCache.Create("act_thanks_conversation"),
			ActionIndexCache.Create("act_farewell_conversation"),
			ActionIndexCache.Create("act_troop_cavalry_sword"),
			ActionIndexCache.Create("act_inventory_idle_start"),
			ActionIndexCache.Create("act_inventory_idle"),
			ActionIndexCache.Create("act_character_developer_idle"),
			ActionIndexCache.Create("act_inventory_cloth_equip"),
			ActionIndexCache.Create("act_inventory_glove_equip"),
			ActionIndexCache.Create("act_jump"),
			ActionIndexCache.Create("act_jump_loop"),
			ActionIndexCache.Create("act_jump_end"),
			ActionIndexCache.Create("act_jump_end_hard"),
			ActionIndexCache.Create("act_jump_left_stance"),
			ActionIndexCache.Create("act_jump_loop_left_stance"),
			ActionIndexCache.Create("act_jump_end_left_stance"),
			ActionIndexCache.Create("act_jump_end_hard_left_stance"),
			ActionIndexCache.Create("act_jump_forward"),
			ActionIndexCache.Create("act_jump_forward_loop"),
			ActionIndexCache.Create("act_jump_forward_end"),
			ActionIndexCache.Create("act_jump_forward_end_hard"),
			ActionIndexCache.Create("act_jump_forward_left_stance"),
			ActionIndexCache.Create("act_jump_forward_loop_left_stance"),
			ActionIndexCache.Create("act_jump_forward_end_left_stance"),
			ActionIndexCache.Create("act_jump_forward_end_hard_left_stance"),
			ActionIndexCache.Create("act_jump_backward"),
			ActionIndexCache.Create("act_jump_backward_loop"),
			ActionIndexCache.Create("act_jump_backward_end"),
			ActionIndexCache.Create("act_jump_backward_end_hard"),
			ActionIndexCache.Create("act_jump_backward_left_stance"),
			ActionIndexCache.Create("act_jump_backward_loop_left_stance"),
			ActionIndexCache.Create("act_jump_backward_end_left_stance"),
			ActionIndexCache.Create("act_jump_backward_end_hard_left_stance"),
			ActionIndexCache.Create("act_jump_forward_right"),
			ActionIndexCache.Create("act_jump_forward_right_left_stance"),
			ActionIndexCache.Create("act_jump_forward_left"),
			ActionIndexCache.Create("act_jump_forward_left_left_stance"),
			ActionIndexCache.Create("act_jump_right"),
			ActionIndexCache.Create("act_jump_right_loop"),
			ActionIndexCache.Create("act_jump_right_end"),
			ActionIndexCache.Create("act_jump_right_end_hard"),
			ActionIndexCache.Create("act_jump_left"),
			ActionIndexCache.Create("act_jump_left_loop"),
			ActionIndexCache.Create("act_jump_left_end"),
			ActionIndexCache.Create("act_jump_left_end_hard"),
			ActionIndexCache.Create("act_jump_loop_long"),
			ActionIndexCache.Create("act_jump_loop_long_left_stance"),
			ActionIndexCache.Create("act_throne_sit_down_from_front"),
			ActionIndexCache.Create("act_throne_stand_up_to_front"),
			ActionIndexCache.Create("act_throne_sit_idle"),
			ActionIndexCache.Create("act_sit_down_from_front"),
			ActionIndexCache.Create("act_sit_down_from_right"),
			ActionIndexCache.Create("act_sit_down_from_left"),
			ActionIndexCache.Create("act_sit_down_on_floor_1"),
			ActionIndexCache.Create("act_sit_down_on_floor_2"),
			ActionIndexCache.Create("act_sit_down_on_floor_3"),
			ActionIndexCache.Create("act_stand_up_to_front"),
			ActionIndexCache.Create("act_stand_up_to_right"),
			ActionIndexCache.Create("act_stand_up_to_left"),
			ActionIndexCache.Create("act_stand_up_floor_1"),
			ActionIndexCache.Create("act_stand_up_floor_2"),
			ActionIndexCache.Create("act_stand_up_floor_3"),
			ActionIndexCache.Create("act_sit_1"),
			ActionIndexCache.Create("act_sit_2"),
			ActionIndexCache.Create("act_sit_3"),
			ActionIndexCache.Create("act_sit_4"),
			ActionIndexCache.Create("act_sit_5"),
			ActionIndexCache.Create("act_sit_6"),
			ActionIndexCache.Create("act_sit_7"),
			ActionIndexCache.Create("act_sit_8"),
			ActionIndexCache.Create("act_sit_idle_on_floor_1"),
			ActionIndexCache.Create("act_sit_idle_on_floor_2"),
			ActionIndexCache.Create("act_sit_idle_on_floor_3"),
			ActionIndexCache.Create("act_sit_conversation")
		};

		// Token: 0x040001CB RID: 459
		private readonly ActionIndexCache act_inventory_idle_start = ActionIndexCache.Create("act_inventory_idle_start");

		// Token: 0x040001CC RID: 460
		public readonly ActionIndexCache PoseAction = ActionIndexCache.Create("act_walk_idle_unarmed");

		// Token: 0x040001CD RID: 461
		public string LordName = "main_hero";

		// Token: 0x040001CE RID: 462
		public bool IsWeaponWielded;

		// Token: 0x040001D1 RID: 465
		private Vec2 MovementDirection;

		// Token: 0x040001D2 RID: 466
		private float MovementSpeed;

		// Token: 0x040001D3 RID: 467
		private float PhaseDiff;

		// Token: 0x040001D4 RID: 468
		private float Time;

		// Token: 0x040001D5 RID: 469
		private float ActionSetTimer;

		// Token: 0x040001D6 RID: 470
		private float ActionChangeInterval;

		// Token: 0x040001D7 RID: 471
		private float MovementDirectionChange;

		// Token: 0x040001D8 RID: 472
		private static MBGameManager _editorGameManager = null;

		// Token: 0x040001D9 RID: 473
		private static int _editorGameManagerRefCount = 0;

		// Token: 0x040001DA RID: 474
		private static bool isFinished = false;

		// Token: 0x040001DB RID: 475
		private static int gameTickFrameNo = -1;

		// Token: 0x040001DC RID: 476
		private bool CreateFaceImmediately = true;

		// Token: 0x040001DD RID: 477
		private AgentVisuals _agentVisuals;
	}
}
