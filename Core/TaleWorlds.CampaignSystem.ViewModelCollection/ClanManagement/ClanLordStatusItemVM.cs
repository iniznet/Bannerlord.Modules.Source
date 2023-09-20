using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanLordStatusItemVM : ViewModel
	{
		public ClanLordStatusItemVM(ClanLordStatusItemVM.LordStatus status, TextObject hintText)
		{
			this.Type = (int)status;
			this.Hint = new HintViewModel(hintText, null);
		}

		[DataSourceProperty]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		private int _type = -1;

		private HintViewModel _hint;

		public enum LordStatus
		{
			Dead,
			Married,
			Pregnant,
			InBattle,
			InSiege,
			Child,
			Prisoner,
			Sick
		}
	}
}
