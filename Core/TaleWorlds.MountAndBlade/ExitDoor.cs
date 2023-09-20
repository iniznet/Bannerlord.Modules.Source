using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class ExitDoor : UsableMachine
	{
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=gqQPSAQZ}{KEY} Leave Area", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return string.Empty;
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				if (standingPoint.HasUser)
				{
					Agent userAgent = standingPoint.UserAgent;
					ActionIndexValueCache currentActionValue = userAgent.GetCurrentActionValue(0);
					ActionIndexValueCache currentActionValue2 = userAgent.GetCurrentActionValue(1);
					if (!(currentActionValue2 == ActionIndexValueCache.act_none) || (!(currentActionValue == ExitDoor.act_pickup_middle_begin) && !(currentActionValue == ExitDoor.act_pickup_middle_begin_left_stance)))
					{
						if (currentActionValue2 == ActionIndexValueCache.act_none && (currentActionValue == ExitDoor.act_pickup_middle_end || currentActionValue == ExitDoor.act_pickup_middle_end_left_stance))
						{
							Mission.Current.EndMission();
							userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
						else if (currentActionValue2 != ActionIndexValueCache.act_none || !userAgent.SetActionChannel(0, userAgent.GetIsLeftStance() ? ExitDoor.act_pickup_middle_begin_left_stance : ExitDoor.act_pickup_middle_begin, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
						{
							userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
					}
				}
			}
		}

		private static readonly ActionIndexCache act_pickup_middle_begin = ActionIndexCache.Create("act_pickup_middle_begin");

		private static readonly ActionIndexCache act_pickup_middle_begin_left_stance = ActionIndexCache.Create("act_pickup_middle_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_middle_end = ActionIndexCache.Create("act_pickup_middle_end");

		private static readonly ActionIndexCache act_pickup_middle_end_left_stance = ActionIndexCache.Create("act_pickup_middle_end_left_stance");
	}
}
