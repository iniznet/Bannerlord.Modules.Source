using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class HintViewModel : ViewModel
	{
		public HintViewModel()
		{
			this.HintText = TextObject.Empty;
		}

		public HintViewModel(TextObject hintText, string uniqueName = null)
		{
			this.HintText = hintText;
			this._uniqueName = uniqueName;
		}

		public void ExecuteBeginHint()
		{
			if (!TextObject.IsNullOrEmpty(this.HintText))
			{
				MBInformationManager.ShowHint(this.HintText.ToString());
			}
		}

		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		public TextObject HintText;

		private readonly string _uniqueName;
	}
}
