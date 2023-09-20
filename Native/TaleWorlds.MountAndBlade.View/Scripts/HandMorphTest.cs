using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x0200003D RID: 61
	public class HandMorphTest : ScriptComponentBehavior
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x00019764 File Offset: 0x00017964
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x0001976C File Offset: 0x0001796C
		public uint ClothColor1 { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x00019775 File Offset: 0x00017975
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x0001977D File Offset: 0x0001797D
		public uint ClothColor2 { get; private set; }

		// Token: 0x060002D4 RID: 724 RVA: 0x00019788 File Offset: 0x00017988
		protected override void OnInit()
		{
			base.OnInit();
			this.ClothColor1 = uint.MaxValue;
			this.ClothColor2 = uint.MaxValue;
			if (this._agentVisuals == null && !this.characterSpawned)
			{
				this.SpawnCharacter();
				this.characterSpawned = true;
			}
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			this._agentVisuals.GetVisuals().SetFrame(ref globalFrame);
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000197E4 File Offset: 0x000179E4
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			if (Game.Current == null)
			{
				this._editorGameManager = new EditorGameManager();
			}
			this.ClothColor1 = uint.MaxValue;
			this.ClothColor2 = uint.MaxValue;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0001980C File Offset: 0x00017A0C
		protected override void OnEditorTick(float dt)
		{
			if (!this.isFinished && this._editorGameManager != null)
			{
				this.isFinished = !this._editorGameManager.DoLoadingForGameManager();
			}
			if (Game.Current != null && this._agentVisuals == null && !this.characterSpawned)
			{
				this.SpawnCharacter();
				this.characterSpawned = true;
			}
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			this._agentVisuals.GetVisuals().SetFrame(ref globalFrame);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00019880 File Offset: 0x00017A80
		public void SpawnCharacter()
		{
			CharacterCode characterCode = CharacterCode.CreateFrom(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_0"));
			this.InitWithCharacter(characterCode);
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x000198A9 File Offset: 0x00017AA9
		public void Reset()
		{
			if (this._agentVisuals != null)
			{
				this._agentVisuals.Reset();
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x000198C0 File Offset: 0x00017AC0
		public void InitWithCharacter(CharacterCode characterCode)
		{
			this.Reset();
			MatrixFrame frame = base.GameEntity.GetFrame();
			frame.rotation.s.z = 0f;
			frame.rotation.f.z = 0f;
			frame.rotation.s.Normalize();
			frame.rotation.f.Normalize();
			frame.rotation.u = Vec3.CrossProduct(frame.rotation.s, frame.rotation.f);
			characterCode.BodyProperties = new BodyProperties(new DynamicBodyProperties(20f, 0f, 0f), characterCode.BodyProperties.StaticProperties);
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(characterCode.Race);
			this._agentVisuals = AgentVisuals.Create(new AgentVisualsData().Equipment(characterCode.CalculateEquipment()).BodyProperties(characterCode.BodyProperties).Race(characterCode.Race)
				.SkeletonType(characterCode.IsFemale ? 1 : 0)
				.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, characterCode.IsFemale, "_facegen"))
				.ActionCode(this.act_visual_test_morph_animation)
				.Scene(base.GameEntity.Scene)
				.Monster(baseMonsterFromRace)
				.PrepareImmediately(this.CreateFaceImmediately)
				.UseMorphAnims(true)
				.ClothColor1(this.ClothColor1)
				.ClothColor2(this.ClothColor2)
				.Frame(frame), "HandMorphTest", false, false, false);
			this._agentVisuals.SetAction(this.act_defend_up_fist_active, 1f, true);
			MatrixFrame matrixFrame = frame;
			this._agentVisuals.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(1f, matrixFrame, true);
			this._agentVisuals.GetVisuals().SetFrame(ref matrixFrame);
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00019A81 File Offset: 0x00017C81
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this._agentVisuals.Reset();
		}

		// Token: 0x040001F3 RID: 499
		private readonly ActionIndexCache act_defend_up_fist_active = ActionIndexCache.Create("act_defend_up_fist_active");

		// Token: 0x040001F4 RID: 500
		private readonly ActionIndexCache act_visual_test_morph_animation = ActionIndexCache.Create("act_visual_test_morph_animation");

		// Token: 0x040001F7 RID: 503
		private MBGameManager _editorGameManager;

		// Token: 0x040001F8 RID: 504
		private bool isFinished;

		// Token: 0x040001F9 RID: 505
		private bool characterSpawned;

		// Token: 0x040001FA RID: 506
		private bool CreateFaceImmediately = true;

		// Token: 0x040001FB RID: 507
		private AgentVisuals _agentVisuals;
	}
}
