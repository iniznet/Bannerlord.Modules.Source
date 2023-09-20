using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200036F RID: 879
	public class TrainingIcon : UsableMachine
	{
		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x06003001 RID: 12289 RVA: 0x000C52C5 File Offset: 0x000C34C5
		// (set) Token: 0x06003002 RID: 12290 RVA: 0x000C52CD File Offset: 0x000C34CD
		public bool Focused { get; private set; }

		// Token: 0x06003003 RID: 12291 RVA: 0x000C52D8 File Offset: 0x000C34D8
		protected internal override void OnInit()
		{
			base.OnInit();
			this._markerBeam = base.GameEntity.GetFirstChildEntityWithTag(TrainingIcon.HighlightBeamTag);
			this._weaponIcons = (from x in base.GameEntity.GetChildren()
				where !x.GetScriptComponents().Any<ScriptComponentBehavior>() && x != this._markerBeam
				select x).ToList<GameEntity>();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06003004 RID: 12292 RVA: 0x000C5334 File Offset: 0x000C3534
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		// Token: 0x06003005 RID: 12293 RVA: 0x000C5340 File Offset: 0x000C3540
		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._markerBeam != null)
			{
				if (MathF.Abs(this._markerAlpha - this._targetMarkerAlpha) > dt * this._markerAlphaChangeAmount)
				{
					this._markerAlpha += dt * this._markerAlphaChangeAmount * (float)MathF.Sign(this._targetMarkerAlpha - this._markerAlpha);
					this._markerBeam.GetChild(0).GetFirstMesh().SetVectorArgument(this._markerAlpha, 1f, 0.49f, 11.65f);
				}
				else
				{
					this._markerAlpha = this._targetMarkerAlpha;
					if (this._targetMarkerAlpha == 0f)
					{
						GameEntity markerBeam = this._markerBeam;
						if (markerBeam != null)
						{
							markerBeam.SetVisibilityExcludeParents(false);
						}
					}
				}
			}
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				if (standingPoint.HasUser)
				{
					Agent userAgent = standingPoint.UserAgent;
					ActionIndexValueCache currentActionValue = userAgent.GetCurrentActionValue(0);
					ActionIndexValueCache currentActionValue2 = userAgent.GetCurrentActionValue(1);
					if (!(currentActionValue2 == ActionIndexValueCache.act_none) || (!(currentActionValue == TrainingIcon.act_pickup_middle_begin) && !(currentActionValue == TrainingIcon.act_pickup_middle_begin_left_stance)))
					{
						if (currentActionValue2 == ActionIndexValueCache.act_none && (currentActionValue == TrainingIcon.act_pickup_middle_end || currentActionValue == TrainingIcon.act_pickup_middle_end_left_stance))
						{
							this._activated = true;
							userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
						else if (currentActionValue2 != ActionIndexValueCache.act_none || !userAgent.SetActionChannel(0, userAgent.GetIsLeftStance() ? TrainingIcon.act_pickup_middle_begin_left_stance : TrainingIcon.act_pickup_middle_begin, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
						{
							userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
					}
				}
			}
		}

		// Token: 0x06003006 RID: 12294 RVA: 0x000C5528 File Offset: 0x000C3728
		public void SetMarked(bool highlight)
		{
			if (!highlight)
			{
				this._targetMarkerAlpha = 0f;
				return;
			}
			this._targetMarkerAlpha = 75f;
			this._markerBeam.GetChild(0).GetFirstMesh().SetVectorArgument(this._markerAlpha, 1f, 0.49f, 11.65f);
			GameEntity markerBeam = this._markerBeam;
			if (markerBeam == null)
			{
				return;
			}
			markerBeam.SetVisibilityExcludeParents(true);
		}

		// Token: 0x06003007 RID: 12295 RVA: 0x000C558B File Offset: 0x000C378B
		public bool GetIsActivated()
		{
			bool activated = this._activated;
			this._activated = false;
			return activated;
		}

		// Token: 0x06003008 RID: 12296 RVA: 0x000C559A File Offset: 0x000C379A
		public string GetTrainingSubTypeTag()
		{
			return this._trainingSubTypeTag;
		}

		// Token: 0x06003009 RID: 12297 RVA: 0x000C55A4 File Offset: 0x000C37A4
		public void DisableIcon()
		{
			foreach (GameEntity gameEntity in this._weaponIcons)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		// Token: 0x0600300A RID: 12298 RVA: 0x000C55F8 File Offset: 0x000C37F8
		public void EnableIcon()
		{
			foreach (GameEntity gameEntity in this._weaponIcons)
			{
				gameEntity.SetVisibilityExcludeParents(true);
			}
		}

		// Token: 0x0600300B RID: 12299 RVA: 0x000C564C File Offset: 0x000C384C
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			TextObject textObject = new TextObject("{=!}{TRAINING_TYPE}", null);
			textObject.SetTextVariable("TRAINING_TYPE", GameTexts.FindText("str_tutorial_" + this._descriptionTextOfIcon, null));
			return textObject.ToString();
		}

		// Token: 0x0600300C RID: 12300 RVA: 0x000C5680 File Offset: 0x000C3880
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject = null)
		{
			TextObject textObject = new TextObject("{=wY1qP2qj}{KEY} Select", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		// Token: 0x0600300D RID: 12301 RVA: 0x000C56AA File Offset: 0x000C38AA
		public override void OnFocusGain(Agent userAgent)
		{
			base.OnFocusGain(userAgent);
			this.Focused = true;
		}

		// Token: 0x0600300E RID: 12302 RVA: 0x000C56BA File Offset: 0x000C38BA
		public override void OnFocusLose(Agent userAgent)
		{
			base.OnFocusLose(userAgent);
			this.Focused = false;
		}

		// Token: 0x040013F5 RID: 5109
		private static readonly ActionIndexCache act_pickup_middle_begin = ActionIndexCache.Create("act_pickup_middle_begin");

		// Token: 0x040013F6 RID: 5110
		private static readonly ActionIndexCache act_pickup_middle_begin_left_stance = ActionIndexCache.Create("act_pickup_middle_begin_left_stance");

		// Token: 0x040013F7 RID: 5111
		private static readonly ActionIndexCache act_pickup_middle_end = ActionIndexCache.Create("act_pickup_middle_end");

		// Token: 0x040013F8 RID: 5112
		private static readonly ActionIndexCache act_pickup_middle_end_left_stance = ActionIndexCache.Create("act_pickup_middle_end_left_stance");

		// Token: 0x040013F9 RID: 5113
		private static readonly string HighlightBeamTag = "highlight_beam";

		// Token: 0x040013FB RID: 5115
		private bool _activated;

		// Token: 0x040013FC RID: 5116
		private float _markerAlpha;

		// Token: 0x040013FD RID: 5117
		private float _targetMarkerAlpha;

		// Token: 0x040013FE RID: 5118
		private float _markerAlphaChangeAmount = 110f;

		// Token: 0x040013FF RID: 5119
		private List<GameEntity> _weaponIcons = new List<GameEntity>();

		// Token: 0x04001400 RID: 5120
		private GameEntity _markerBeam;

		// Token: 0x04001401 RID: 5121
		[EditableScriptComponentVariable(true)]
		private string _descriptionTextOfIcon = "";

		// Token: 0x04001402 RID: 5122
		[EditableScriptComponentVariable(true)]
		private string _trainingSubTypeTag = "";
	}
}
