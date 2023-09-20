using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class ProfitItemPropertyVM : ViewModel
	{
		public ProfitItemPropertyVM(string name, int value, ProfitItemPropertyVM.PropertyType type = ProfitItemPropertyVM.PropertyType.None, ImageIdentifierVM governorVisual = null, BasicTooltipViewModel hint = null)
		{
			this.Name = name;
			this.Value = value;
			this.Type = (int)type;
			this.GovernorVisual = governorVisual;
			this.Hint = hint;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ColonText = GameTexts.FindText("str_colon", null).ToString();
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
					this.ShowGovernorPortrait = this._type == 5;
					base.OnPropertyChangedWithValue(value, "Type");
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
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					this.ValueString = this._value.ToString("+0;-#");
					base.OnPropertyChangedWithValue(value, "Value");
				}
			}
		}

		[DataSourceProperty]
		public string ValueString
		{
			get
			{
				return this._valueString;
			}
			private set
			{
				if (value != this._valueString)
				{
					this._valueString = value;
					base.OnPropertyChangedWithValue<string>(value, "ValueString");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Hint
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
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public string ColonText
		{
			get
			{
				return this._colonText;
			}
			set
			{
				if (value != this._colonText)
				{
					this._colonText = value;
					base.OnPropertyChangedWithValue<string>(value, "ColonText");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM GovernorVisual
		{
			get
			{
				return this._governorVisual;
			}
			set
			{
				if (value != this._governorVisual)
				{
					this._governorVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "GovernorVisual");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowGovernorPortrait
		{
			get
			{
				return this._showGovernorPortrait;
			}
			private set
			{
				if (value != this._showGovernorPortrait)
				{
					this._showGovernorPortrait = value;
					base.OnPropertyChangedWithValue(value, "ShowGovernorPortrait");
				}
			}
		}

		private int _type;

		private string _name;

		private int _value;

		private string _valueString;

		private BasicTooltipViewModel _hint;

		private string _colonText;

		private ImageIdentifierVM _governorVisual;

		private bool _showGovernorPortrait;

		public enum PropertyType
		{
			None,
			Tax,
			Tariff,
			Garrison,
			Village,
			Governor
		}
	}
}
