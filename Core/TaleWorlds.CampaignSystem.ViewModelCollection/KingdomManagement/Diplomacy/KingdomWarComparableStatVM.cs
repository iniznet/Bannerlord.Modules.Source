using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	public class KingdomWarComparableStatVM : ViewModel
	{
		public KingdomWarComparableStatVM(int faction1Stat, int faction2Stat, TextObject name, string faction1Color, string faction2Color, int defaultRange, BasicTooltipViewModel faction1Hint = null, BasicTooltipViewModel faction2Hint = null)
		{
			int num = MathF.Max(MathF.Max(faction1Stat, faction2Stat), defaultRange);
			if (num == 0)
			{
				num = 1;
			}
			this.Faction1Color = faction1Color;
			this.Faction2Color = faction2Color;
			this.Faction1Value = faction1Stat;
			this.Faction2Value = faction2Stat;
			this._defaultRange = defaultRange;
			this.Faction1Percentage = MathF.Round((float)faction1Stat / (float)num * 100f);
			this.Faction2Percentage = MathF.Round((float)faction2Stat / (float)num * 100f);
			this._nameObj = name;
			this.Faction1Hint = faction1Hint;
			this.Faction2Hint = faction2Hint;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameObj.ToString();
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Faction1Hint
		{
			get
			{
				return this._faction1Hint;
			}
			set
			{
				if (value != this._faction1Hint)
				{
					this._faction1Hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Faction1Hint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Faction2Hint
		{
			get
			{
				return this._faction2Hint;
			}
			set
			{
				if (value != this._faction2Hint)
				{
					this._faction2Hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Faction2Hint");
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
		public string Faction1Color
		{
			get
			{
				return this._faction1Color;
			}
			set
			{
				if (value != this._faction1Color)
				{
					this._faction1Color = value;
					base.OnPropertyChangedWithValue<string>(value, "Faction1Color");
				}
			}
		}

		[DataSourceProperty]
		public string Faction2Color
		{
			get
			{
				return this._faction2Color;
			}
			set
			{
				if (value != this._faction2Color)
				{
					this._faction2Color = value;
					base.OnPropertyChangedWithValue<string>(value, "Faction2Color");
				}
			}
		}

		[DataSourceProperty]
		public int Faction1Percentage
		{
			get
			{
				return this._faction1Percentage;
			}
			set
			{
				if (value != this._faction1Percentage)
				{
					this._faction1Percentage = value;
					base.OnPropertyChangedWithValue(value, "Faction1Percentage");
				}
			}
		}

		[DataSourceProperty]
		public int Faction1Value
		{
			get
			{
				return this._faction1Value;
			}
			set
			{
				if (value != this._faction1Value)
				{
					this._faction1Value = value;
					base.OnPropertyChangedWithValue(value, "Faction1Value");
				}
			}
		}

		[DataSourceProperty]
		public int Faction2Percentage
		{
			get
			{
				return this._faction2Percentage;
			}
			set
			{
				if (value != this._faction2Percentage)
				{
					this._faction2Percentage = value;
					base.OnPropertyChangedWithValue(value, "Faction2Percentage");
				}
			}
		}

		[DataSourceProperty]
		public int Faction2Value
		{
			get
			{
				return this._faction2Value;
			}
			set
			{
				if (value != this._faction2Value)
				{
					this._faction2Value = value;
					base.OnPropertyChangedWithValue(value, "Faction2Value");
				}
			}
		}

		private TextObject _nameObj;

		private int _defaultRange;

		private BasicTooltipViewModel _faction1Hint;

		private BasicTooltipViewModel _faction2Hint;

		private string _name;

		private string _faction1Color;

		private string _faction2Color;

		private int _faction1Percentage;

		private int _faction1Value;

		private int _faction2Percentage;

		private int _faction2Value;
	}
}
