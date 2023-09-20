using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
	public class BasicMissionHandler : MissionLogic
	{
		public bool IsWarningWidgetOpened { get; private set; }

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.IsWarningWidgetOpened = false;
		}

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

		private void OnEventCancelSelectionWidget()
		{
			this.CloseSelectionWidget();
		}

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

		private InquiryData GetRetreatPopUpData()
		{
			return new InquiryData("", GameTexts.FindText("str_retreat_question", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(this.OnEventAcceptSelectionWidget), new Action(this.OnEventCancelSelectionWidget), "", 0f, null, null, null);
		}

		private InquiryData GetSurrenderPopupData()
		{
			return new InquiryData(GameTexts.FindText("str_surrender", null).ToString(), GameTexts.FindText("str_surrender_question", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(this.OnEventAcceptSelectionWidget), new Action(this.OnEventCancelSelectionWidget), "", 0f, null, null, null);
		}

		private bool _isSurrender;
	}
}
