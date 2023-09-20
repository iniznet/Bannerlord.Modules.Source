using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BasicLeaveMissionLogic : MissionLogic
	{
		public BasicLeaveMissionLogic()
			: this(false)
		{
		}

		public BasicLeaveMissionLogic(bool askBeforeLeave)
			: this(askBeforeLeave, 5)
		{
		}

		public BasicLeaveMissionLogic(bool askBeforeLeave, int minRetreatDistance)
		{
			this._askBeforeLeave = askBeforeLeave;
			this._minRetreatDistance = minRetreatDistance;
		}

		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return base.Mission.MainAgent != null && !base.Mission.MainAgent.IsActive();
		}

		public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
		{
			canPlayerLeave = true;
			if (base.Mission.MainAgent != null && base.Mission.MainAgent.IsActive() && (float)this._minRetreatDistance > 0f && base.Mission.IsPlayerCloseToAnEnemy((float)this._minRetreatDistance))
			{
				canPlayerLeave = false;
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_can_not_retreat", null), 0, null, "");
			}
			else if (this._askBeforeLeave)
			{
				return new InquiryData("", GameTexts.FindText("str_give_up_fight", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(base.Mission.OnEndMissionResult), null, "", 0f, null, null, null);
			}
			return null;
		}

		private readonly bool _askBeforeLeave;

		private readonly int _minRetreatDistance;
	}
}
