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
	public class TrainingIcon : UsableMachine
	{
		public bool Focused { get; private set; }

		protected internal override void OnInit()
		{
			base.OnInit();
			this._markerBeam = base.GameEntity.GetFirstChildEntityWithTag(TrainingIcon.HighlightBeamTag);
			this._weaponIcons = (from x in base.GameEntity.GetChildren()
				where !x.GetScriptComponents().Any<ScriptComponentBehavior>() && x != this._markerBeam
				select x).ToList<GameEntity>();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

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

		public bool GetIsActivated()
		{
			bool activated = this._activated;
			this._activated = false;
			return activated;
		}

		public string GetTrainingSubTypeTag()
		{
			return this._trainingSubTypeTag;
		}

		public void DisableIcon()
		{
			foreach (GameEntity gameEntity in this._weaponIcons)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		public void EnableIcon()
		{
			foreach (GameEntity gameEntity in this._weaponIcons)
			{
				gameEntity.SetVisibilityExcludeParents(true);
			}
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			TextObject textObject = new TextObject("{=!}{TRAINING_TYPE}", null);
			textObject.SetTextVariable("TRAINING_TYPE", GameTexts.FindText("str_tutorial_" + this._descriptionTextOfIcon, null));
			return textObject.ToString();
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject = null)
		{
			TextObject textObject = new TextObject("{=wY1qP2qj}{KEY} Select", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override void OnFocusGain(Agent userAgent)
		{
			base.OnFocusGain(userAgent);
			this.Focused = true;
		}

		public override void OnFocusLose(Agent userAgent)
		{
			base.OnFocusLose(userAgent);
			this.Focused = false;
		}

		private static readonly ActionIndexCache act_pickup_middle_begin = ActionIndexCache.Create("act_pickup_middle_begin");

		private static readonly ActionIndexCache act_pickup_middle_begin_left_stance = ActionIndexCache.Create("act_pickup_middle_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_middle_end = ActionIndexCache.Create("act_pickup_middle_end");

		private static readonly ActionIndexCache act_pickup_middle_end_left_stance = ActionIndexCache.Create("act_pickup_middle_end_left_stance");

		private static readonly string HighlightBeamTag = "highlight_beam";

		private bool _activated;

		private float _markerAlpha;

		private float _targetMarkerAlpha;

		private float _markerAlphaChangeAmount = 110f;

		private List<GameEntity> _weaponIcons = new List<GameEntity>();

		private GameEntity _markerBeam;

		[EditableScriptComponentVariable(true)]
		private string _descriptionTextOfIcon = "";

		[EditableScriptComponentVariable(true)]
		private string _trainingSubTypeTag = "";
	}
}
