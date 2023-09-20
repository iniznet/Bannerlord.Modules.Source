using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GauntletUI.Tutorial
{
	public class OrderTutorialStep1 : TutorialItemBase
	{
		public OrderTutorialStep1()
		{
			base.Type = "OrderTutorial1TutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
		}

		public override bool IsConditionsMetForCompletion()
		{
			if (!this._registeredToOrderEvent)
			{
				Mission mission = Mission.Current;
				bool flag;
				if (mission == null)
				{
					flag = null != null;
				}
				else
				{
					Team playerTeam = mission.PlayerTeam;
					flag = ((playerTeam != null) ? playerTeam.PlayerOrderController : null) != null;
				}
				if (flag)
				{
					Mission mission2 = Mission.Current;
					if (mission2 != null && mission2.Mode == 2)
					{
						Mission.Current.PlayerTeam.PlayerOrderController.OnOrderIssued += new OnOrderIssuedDelegate(this.OnPlayerOrdered);
						this._registeredToOrderEvent = true;
					}
				}
			}
			return this._hasPlayerOrderedFollowMe;
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			if (this._registeredToOrderEvent)
			{
				Mission mission = Mission.Current;
				bool flag;
				if (mission == null)
				{
					flag = null != null;
				}
				else
				{
					Team playerTeam = mission.PlayerTeam;
					flag = ((playerTeam != null) ? playerTeam.PlayerOrderController : null) != null;
				}
				if (flag)
				{
					Mission.Current.PlayerTeam.PlayerOrderController.OnOrderIssued -= new OnOrderIssuedDelegate(this.OnPlayerOrdered);
				}
			}
			this._registeredToOrderEvent = false;
		}

		private void OnPlayerOrdered(OrderType orderType, IEnumerable<Formation> appliedFormations, params object[] delegateParams)
		{
			this._hasPlayerOrderedFollowMe = this._hasPlayerOrderedFollowMe || (orderType == 7 && appliedFormations.Any<Formation>());
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 8 && TutorialHelper.IsPlayerInABattleMission && Mission.Current.Mode != 6 && TutorialHelper.IsOrderingAvailable;
		}

		private bool _hasPlayerOrderedFollowMe;

		private bool _registeredToOrderEvent;
	}
}
