using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
	// Token: 0x020003F8 RID: 1016
	public class BasicMissionHandler : MissionLogic
	{
		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x060034E4 RID: 13540 RVA: 0x000DC1EE File Offset: 0x000DA3EE
		// (set) Token: 0x060034E5 RID: 13541 RVA: 0x000DC1F6 File Offset: 0x000DA3F6
		public bool IsWarningWidgetOpened { get; private set; }

		// Token: 0x060034E6 RID: 13542 RVA: 0x000DC1FF File Offset: 0x000DA3FF
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.IsWarningWidgetOpened = false;
		}

		// Token: 0x060034E7 RID: 13543 RVA: 0x000DC20E File Offset: 0x000DA40E
		public void CreateWarningWidgetForResult(BattleEndLogic.ExitResult result)
		{
			if (!GameNetwork.IsClient)
			{
				MBCommon.PauseGameEngine();
			}
			this._isSurrender = result == BattleEndLogic.ExitResult.SurrenderSiege;
			InformationManager.ShowInquiry(this._isSurrender ? this.GetSurrenderPopupData() : this.GetRetreatPopUpData(), true, false);
			this.IsWarningWidgetOpened = true;
		}

		// Token: 0x060034E8 RID: 13544 RVA: 0x000DC24A File Offset: 0x000DA44A
		private void CloseSelectionWidget()
		{
			if (!this.IsWarningWidgetOpened)
			{
				return;
			}
			this.IsWarningWidgetOpened = false;
			if (!GameNetwork.IsClient)
			{
				MBCommon.UnPauseGameEngine();
			}
		}

		// Token: 0x060034E9 RID: 13545 RVA: 0x000DC268 File Offset: 0x000DA468
		private void OnEventCancelSelectionWidget()
		{
			this.CloseSelectionWidget();
		}

		// Token: 0x060034EA RID: 13546 RVA: 0x000DC270 File Offset: 0x000DA470
		private void OnEventAcceptSelectionWidget()
		{
			MissionLogic[] array = base.Mission.MissionLogics.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnBattleEnded();
			}
			this.CloseSelectionWidget();
			if (this._isSurrender)
			{
				base.Mission.SurrenderMission();
				return;
			}
			base.Mission.RetreatMission();
		}

		// Token: 0x060034EB RID: 13547 RVA: 0x000DC2CC File Offset: 0x000DA4CC
		private InquiryData GetRetreatPopUpData()
		{
			return new InquiryData("", GameTexts.FindText("str_retreat_question", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(this.OnEventAcceptSelectionWidget), new Action(this.OnEventCancelSelectionWidget), "", 0f, null, null, null);
		}

		// Token: 0x060034EC RID: 13548 RVA: 0x000DC33C File Offset: 0x000DA53C
		private InquiryData GetSurrenderPopupData()
		{
			return new InquiryData(GameTexts.FindText("str_surrender", null).ToString(), GameTexts.FindText("str_surrender_question", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(this.OnEventAcceptSelectionWidget), new Action(this.OnEventCancelSelectionWidget), "", 0f, null, null, null);
		}

		// Token: 0x040016A8 RID: 5800
		private bool _isSurrender;
	}
}
