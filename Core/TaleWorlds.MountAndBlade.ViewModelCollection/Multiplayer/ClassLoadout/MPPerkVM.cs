using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	public class MPPerkVM : ViewModel
	{
		public int PerkIndex { get; private set; }

		public MPPerkVM(Action<MPPerkVM> onSelectPerk, IReadOnlyPerkObject perk, bool isSelectable, int perkIndex)
		{
			this.Perk = perk;
			this.PerkIndex = perkIndex;
			this._onSelectPerk = onSelectPerk;
			this.IconType = perk.IconId;
			this.IsSelectable = isSelectable;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.Perk.Name.ToString();
			this.Description = this.Perk.Description.ToString();
			GameTexts.SetVariable("newline", "\n");
			this.Hint = new HintViewModel(this.Perk.Description, null);
		}

		public void ExecuteSelectPerk()
		{
			this._onSelectPerk(this);
		}

		[DataSourceProperty]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconType");
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

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelectable
		{
			get
			{
				return this._isSelectable;
			}
			set
			{
				if (value != this._isSelectable)
				{
					this._isSelectable = value;
					base.OnPropertyChangedWithValue(value, "IsSelectable");
				}
			}
		}

		public readonly IReadOnlyPerkObject Perk;

		private readonly Action<MPPerkVM> _onSelectPerk;

		private string _iconType;

		private string _name;

		private string _description;

		private bool _isSelectable;

		private HintViewModel _hint;
	}
}
