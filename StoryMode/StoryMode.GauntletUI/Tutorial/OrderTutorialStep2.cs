using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200002A RID: 42
	public class OrderTutorialStep2 : TutorialItemBase
	{
		// Token: 0x060000CD RID: 205 RVA: 0x00003911 File Offset: 0x00001B11
		public OrderTutorialStep2()
		{
			base.Type = "OrderTutorial1TutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "";
			base.MouseRequired = false;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00003940 File Offset: 0x00001B40
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
			return this._hasPlayerOrderedCharge;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000039B8 File Offset: 0x00001BB8
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

		// Token: 0x060000D0 RID: 208 RVA: 0x00003A19 File Offset: 0x00001C19
		private void OnPlayerOrdered(OrderType orderType, IEnumerable<Formation> appliedFormations, params object[] delegateParams)
		{
			this._hasPlayerOrderedCharge = this._hasPlayerOrderedCharge || (orderType == 4 && appliedFormations.Any<Formation>());
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00003A39 File Offset: 0x00001C39
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00003A3C File Offset: 0x00001C3C
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 8 && TutorialHelper.IsPlayerInABattleMission && Mission.Current.Mode != 6 && TutorialHelper.IsOrderingAvailable;
		}

		// Token: 0x04000038 RID: 56
		private bool _hasPlayerOrderedCharge;

		// Token: 0x04000039 RID: 57
		private bool _registeredToOrderEvent;
	}
}
