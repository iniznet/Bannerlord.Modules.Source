using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Supporters
{
	public class ClanSupporterGroupVM : ViewModel
	{
		public ClanSupporterGroupVM(TextObject groupName, float influenceBonus, Action<ClanSupporterGroupVM> onSelection)
		{
			this._groupNameText = groupName;
			this._influenceBonus = influenceBonus;
			this._onSelection = onSelection;
			this.Supporters = new MBBindingList<ClanSupporterItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		public void AddSupporter(Hero hero)
		{
			if (!this.Supporters.Any((ClanSupporterItemVM x) => x.Hero.Hero == hero))
			{
				this.Supporters.Add(new ClanSupporterItemVM(hero));
			}
		}

		public void Refresh()
		{
			TextObject textObject = GameTexts.FindText("str_amount_with_influence_icon", null);
			this.TotalInfluenceBonus = (float)this.Supporters.Count * this._influenceBonus;
			TextObject textObject2 = GameTexts.FindText("str_plus_with_number", null);
			textObject2.SetTextVariable("NUMBER", this.TotalInfluenceBonus.ToString("F2"));
			textObject.SetTextVariable("AMOUNT", textObject2.ToString());
			textObject.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
			this.TotalInfluence = textObject.ToString();
			TextObject textObject3 = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null);
			textObject3.SetTextVariable("RANK", this._groupNameText.ToString());
			textObject3.SetTextVariable("NUMBER", this.Supporters.Count);
			this.Name = textObject3.ToString();
			TextObject textObject4 = new TextObject("{=cZCOa00c}{SUPPORTER_RANK} Supporters ({NUM})", null);
			textObject4.SetTextVariable("SUPPORTER_RANK", this._groupNameText.ToString());
			textObject4.SetTextVariable("NUM", this.Supporters.Count);
			this.TitleText = textObject4.ToString();
			TextObject textObject5 = new TextObject("{=jdbT6nc9}Each {SUPPORTER_RANK} supporter provides {INFLUENCE_BONUS} per day.", null);
			textObject5.SetTextVariable("SUPPORTER_RANK", this._groupNameText.ToString());
			textObject5.SetTextVariable("INFLUENCE_BONUS", this._influenceBonus.ToString("F2") + "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
			this.InfluenceBonusDescription = textObject5.ToString();
		}

		public void ExecuteSelect()
		{
			Action<ClanSupporterGroupVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public float TotalInfluenceBonus
		{
			get
			{
				return this._totalInfluenceBonus;
			}
			private set
			{
				if (value != this._totalInfluenceBonus)
				{
					this._totalInfluenceBonus = value;
					base.OnPropertyChangedWithValue(value, "TotalInfluenceBonus");
				}
			}
		}

		[DataSourceProperty]
		public string InfluenceBonusDescription
		{
			get
			{
				return this._influenceBonusDescription;
			}
			set
			{
				if (value != this._influenceBonusDescription)
				{
					this._influenceBonusDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "InfluenceBonusDescription");
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
		public string TotalInfluence
		{
			get
			{
				return this._totalInfluence;
			}
			set
			{
				if (value != this._totalInfluence)
				{
					this._totalInfluence = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalInfluence");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<ClanSupporterItemVM> Supporters
		{
			get
			{
				return this._supporters;
			}
			set
			{
				if (value != this._supporters)
				{
					this._supporters = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanSupporterItemVM>>(value, "Supporters");
				}
			}
		}

		private TextObject _groupNameText;

		private float _influenceBonus;

		private Action<ClanSupporterGroupVM> _onSelection;

		private string _titleText;

		private string _influenceBonusDescription;

		private string _name;

		private string _totalInfluence;

		private bool _isSelected;

		private MBBindingList<ClanSupporterItemVM> _supporters;

		private float _totalInfluenceBonus;
	}
}
