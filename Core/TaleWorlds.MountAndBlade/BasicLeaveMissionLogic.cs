using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200025D RID: 605
	public class BasicLeaveMissionLogic : MissionLogic
	{
		// Token: 0x060020A6 RID: 8358 RVA: 0x0007490D File Offset: 0x00072B0D
		public BasicLeaveMissionLogic()
			: this(false)
		{
		}

		// Token: 0x060020A7 RID: 8359 RVA: 0x00074916 File Offset: 0x00072B16
		public BasicLeaveMissionLogic(bool askBeforeLeave)
			: this(askBeforeLeave, 5)
		{
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x00074920 File Offset: 0x00072B20
		public BasicLeaveMissionLogic(bool askBeforeLeave, int minRetreatDistance)
		{
			this._askBeforeLeave = askBeforeLeave;
			this._minRetreatDistance = minRetreatDistance;
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x00074936 File Offset: 0x00072B36
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return base.Mission.MainAgent != null && !base.Mission.MainAgent.IsActive();
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x0007495C File Offset: 0x00072B5C
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

		// Token: 0x04000BFE RID: 3070
		private readonly bool _askBeforeLeave;

		// Token: 0x04000BFF RID: 3071
		private readonly int _minRetreatDistance;
	}
}
