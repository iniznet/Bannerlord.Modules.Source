using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.AnimationPoints
{
	public class ChairUsePoint : AnimationPoint
	{
		protected override void SetActionCodes()
		{
			base.SetActionCodes();
			this._loopAction = ActionIndexCache.Create(this.LoopStartAction);
			this._pairLoopAction = ActionIndexCache.Create(this.PairLoopStartAction);
			this._nearTableLoopAction = ActionIndexCache.Create(this.NearTableLoopAction);
			this._nearTablePairLoopAction = ActionIndexCache.Create(this.NearTablePairLoopAction);
			this._drinkLoopAction = ActionIndexCache.Create(this.DrinkLoopAction);
			this._drinkPairLoopAction = ActionIndexCache.Create(this.DrinkPairLoopAction);
			this._eatLoopAction = ActionIndexCache.Create(this.EatLoopAction);
			this._eatPairLoopAction = ActionIndexCache.Create(this.EatPairLoopAction);
			this.SetChairAction(this.GetRandomChairAction());
		}

		protected override bool ShouldUpdateOnEditorVariableChanged(string variableName)
		{
			return base.ShouldUpdateOnEditorVariableChanged(variableName) || variableName == "NearTable" || variableName == "Drink" || variableName == "Eat" || variableName == "NearTableLoopAction" || variableName == "DrinkLoopAction" || variableName == "EatLoopAction";
		}

		public override void OnUse(Agent userAgent)
		{
			ChairUsePoint.ChairAction chairAction = (base.CanAgentUseItem(userAgent) ? this.GetRandomChairAction() : ChairUsePoint.ChairAction.None);
			this.SetChairAction(chairAction);
			base.OnUse(userAgent);
		}

		private ChairUsePoint.ChairAction GetRandomChairAction()
		{
			List<ChairUsePoint.ChairAction> list = new List<ChairUsePoint.ChairAction> { ChairUsePoint.ChairAction.None };
			if (this.NearTable && this._nearTableLoopAction != ActionIndexCache.act_none)
			{
				list.Add(ChairUsePoint.ChairAction.LeanOnTable);
			}
			if (this.Drink && this._drinkLoopAction != ActionIndexCache.act_none)
			{
				list.Add(ChairUsePoint.ChairAction.Drink);
			}
			if (this.Eat && this._eatLoopAction != ActionIndexCache.act_none)
			{
				list.Add(ChairUsePoint.ChairAction.Eat);
			}
			return list[new Random().Next(list.Count)];
		}

		private void SetChairAction(ChairUsePoint.ChairAction chairAction)
		{
			switch (chairAction)
			{
			case ChairUsePoint.ChairAction.None:
				this.LoopStartActionCode = this._loopAction;
				this.PairLoopStartActionCode = this._pairLoopAction;
				base.SelectedRightHandItem = this.RightHandItem;
				base.SelectedLeftHandItem = this.LeftHandItem;
				return;
			case ChairUsePoint.ChairAction.LeanOnTable:
				this.LoopStartActionCode = this._nearTableLoopAction;
				this.PairLoopStartActionCode = this._nearTablePairLoopAction;
				base.SelectedRightHandItem = string.Empty;
				base.SelectedLeftHandItem = string.Empty;
				return;
			case ChairUsePoint.ChairAction.Drink:
				this.LoopStartActionCode = this._drinkLoopAction;
				this.PairLoopStartActionCode = this._drinkPairLoopAction;
				base.SelectedRightHandItem = this.DrinkRightHandItem;
				base.SelectedLeftHandItem = this.DrinkLeftHandItem;
				return;
			case ChairUsePoint.ChairAction.Eat:
				this.LoopStartActionCode = this._eatLoopAction;
				this.PairLoopStartActionCode = this._eatPairLoopAction;
				base.SelectedRightHandItem = this.EatRightHandItem;
				base.SelectedLeftHandItem = this.EatLeftHandItem;
				return;
			default:
				return;
			}
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (base.UserAgent != null && !base.UserAgent.IsAIControlled && Extensions.HasAnyFlag<Agent.EventControlFlag>(base.UserAgent.EventControlFlags, 24576))
			{
				base.UserAgent.StopUsingGameObject(true, 1);
			}
		}

		public bool NearTable;

		public string NearTableLoopAction = "";

		public string NearTablePairLoopAction = "";

		public bool Drink;

		public string DrinkLoopAction = "";

		public string DrinkPairLoopAction = "";

		public string DrinkRightHandItem = "";

		public string DrinkLeftHandItem = "";

		public bool Eat;

		public string EatLoopAction = "";

		public string EatPairLoopAction = "";

		public string EatRightHandItem = "";

		public string EatLeftHandItem = "";

		private ActionIndexCache _loopAction;

		private ActionIndexCache _pairLoopAction;

		private ActionIndexCache _nearTableLoopAction;

		private ActionIndexCache _nearTablePairLoopAction;

		private ActionIndexCache _drinkLoopAction;

		private ActionIndexCache _drinkPairLoopAction;

		private ActionIndexCache _eatLoopAction;

		private ActionIndexCache _eatPairLoopAction;

		private enum ChairAction
		{
			None,
			LeanOnTable,
			Drink,
			Eat
		}
	}
}
