using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000029 RID: 41
	public class OrderTutorialStep1 : TutorialItemBase
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x000037C3 File Offset: 0x000019C3
		public OrderTutorialStep1()
		{
			base.Type = "OrderTutorial1TutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000037F0 File Offset: 0x000019F0
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

		// Token: 0x060000C9 RID: 201 RVA: 0x00003868 File Offset: 0x00001A68
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

		// Token: 0x060000CA RID: 202 RVA: 0x000038C9 File Offset: 0x00001AC9
		private void OnPlayerOrdered(OrderType orderType, IEnumerable<Formation> appliedFormations, params object[] delegateParams)
		{
			this._hasPlayerOrderedFollowMe = this._hasPlayerOrderedFollowMe || (orderType == 7 && appliedFormations.Any<Formation>());
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000038E9 File Offset: 0x00001AE9
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000038EC File Offset: 0x00001AEC
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 8 && TutorialHelper.IsPlayerInABattleMission && Mission.Current.Mode != 6 && TutorialHelper.IsOrderingAvailable;
		}

		// Token: 0x04000036 RID: 54
		private bool _hasPlayerOrderedFollowMe;

		// Token: 0x04000037 RID: 55
		private bool _registeredToOrderEvent;
	}
}
