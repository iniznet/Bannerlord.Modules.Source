using System;

namespace TaleWorlds.Library.Information
{
	public class TooltipTriggerVM : ViewModel
	{
		public TooltipTriggerVM(Type linkedTooltipType, params object[] args)
		{
			this._linkedTooltipType = linkedTooltipType;
			this._args = args;
		}

		public void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(this._linkedTooltipType, this._args);
		}

		public void ExecuteEndHint()
		{
			InformationManager.HideTooltip();
		}

		private Type _linkedTooltipType;

		private object[] _args;
	}
}
