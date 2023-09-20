using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class ActionCampaignOptionData : CampaignOptionData
	{
		public ActionCampaignOptionData(string identifier, int priorityIndex, CampaignOptionEnableState enableState, Action action, Func<CampaignOptionDisableStatus> getIsDisabledWithReason = null)
			: base(identifier, priorityIndex, enableState, null, null, getIsDisabledWithReason, false, null, null)
		{
			this._action = action;
		}

		public override CampaignOptionDataType GetDataType()
		{
			return CampaignOptionDataType.Action;
		}

		public void ExecuteAction()
		{
			Action action = this._action;
			if (action == null)
			{
				return;
			}
			action();
		}

		private Action _action;
	}
}
