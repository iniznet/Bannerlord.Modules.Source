using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200002B RID: 43
	public class OrderHideoutTutorial : TutorialItemBase
	{
		// Token: 0x060000D3 RID: 211 RVA: 0x00003A61 File Offset: 0x00001C61
		public OrderHideoutTutorial()
		{
			base.Type = "OrderTutorial2Tutorial";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00003A90 File Offset: 0x00001C90
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
			return this._hasPlayerOrderedFollowme;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00003B08 File Offset: 0x00001D08
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

		// Token: 0x060000D6 RID: 214 RVA: 0x00003B69 File Offset: 0x00001D69
		private void OnPlayerOrdered(OrderType orderType, IEnumerable<Formation> appliedFormations, params object[] delegateParams)
		{
			this._hasPlayerOrderedFollowme = this._hasPlayerOrderedFollowme || (orderType == 7 && appliedFormations.Any<Formation>());
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00003B89 File Offset: 0x00001D89
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00003B8C File Offset: 0x00001D8C
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 8 && TutorialHelper.IsPlayerInAHideoutBattleMission && TutorialHelper.IsOrderingAvailable;
		}

		// Token: 0x0400003A RID: 58
		private bool _hasPlayerOrderedFollowme;

		// Token: 0x0400003B RID: 59
		private bool _registeredToOrderEvent;
	}
}
