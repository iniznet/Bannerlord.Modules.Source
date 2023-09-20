using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	public class HandMorphTest : ScriptComponentBehavior
	{
		public uint ClothColor1 { get; private set; }

		public uint ClothColor2 { get; private set; }

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

		public void SpawnCharacter()
		{
			CharacterCode characterCode = CharacterCode.CreateFrom(MBObjectManager.Instance.GetObject<BasicCharacterObject>("facgen_template_test_char_0"));
			this.InitWithCharacter(characterCode);
		}

		public void Reset()
		{
			if (this._agentVisuals != null)
			{
				this._agentVisuals.Reset();
			}
		}

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

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this._agentVisuals.Reset();
		}

		private readonly ActionIndexCache act_defend_up_fist_active = ActionIndexCache.Create("act_defend_up_fist_active");

		private readonly ActionIndexCache act_visual_test_morph_animation = ActionIndexCache.Create("act_visual_test_morph_animation");

		private MBGameManager _editorGameManager;

		private bool isFinished;

		private bool characterSpawned;

		private bool CreateFaceImmediately = true;

		private AgentVisuals _agentVisuals;
	}
}
