using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Supporters
{
	// Token: 0x0200010B RID: 267
	public class ClanSupporterGroupVM : ViewModel
	{
		// Token: 0x06001991 RID: 6545 RVA: 0x0005C7B7 File Offset: 0x0005A9B7
		public ClanSupporterGroupVM(TextObject groupName, float influenceBonus, Action<ClanSupporterGroupVM> onSelection)
		{
			this._groupNameText = groupName;
			this._influenceBonus = influenceBonus;
			this._onSelection = onSelection;
			this.Supporters = new MBBindingList<ClanSupporterItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0005C7E5 File Offset: 0x0005A9E5
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x0005C7F4 File Offset: 0x0005A9F4
		public void AddSupporter(Hero hero)
		{
			if (!this.Supporters.Any((ClanSupporterItemVM x) => x.Hero.Hero == hero))
			{
				this.Supporters.Add(new ClanSupporterItemVM(hero));
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0005C840 File Offset: 0x0005AA40
		public void Refresh()
		{
			TextObject textObject = GameTexts.FindText("str_amount_with_influence_icon", null);
			TextObject textObject2 = GameTexts.FindText("str_plus_with_number", null);
			textObject2.SetTextVariable("NUMBER", ((float)this.Supporters.Count * this._influenceBonus).ToString("F2"));
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

		// Token: 0x06001995 RID: 6549 RVA: 0x0005C9A6 File Offset: 0x0005ABA6
		public void ExecuteSelect()
		{
			Action<ClanSupporterGroupVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x06001996 RID: 6550 RVA: 0x0005C9B9 File Offset: 0x0005ABB9
		// (set) Token: 0x06001997 RID: 6551 RVA: 0x0005C9C1 File Offset: 0x0005ABC1
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

		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x06001998 RID: 6552 RVA: 0x0005C9E4 File Offset: 0x0005ABE4
		// (set) Token: 0x06001999 RID: 6553 RVA: 0x0005C9EC File Offset: 0x0005ABEC
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

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x0600199A RID: 6554 RVA: 0x0005CA0F File Offset: 0x0005AC0F
		// (set) Token: 0x0600199B RID: 6555 RVA: 0x0005CA17 File Offset: 0x0005AC17
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

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x0600199C RID: 6556 RVA: 0x0005CA3A File Offset: 0x0005AC3A
		// (set) Token: 0x0600199D RID: 6557 RVA: 0x0005CA42 File Offset: 0x0005AC42
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

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x0600199E RID: 6558 RVA: 0x0005CA65 File Offset: 0x0005AC65
		// (set) Token: 0x0600199F RID: 6559 RVA: 0x0005CA6D File Offset: 0x0005AC6D
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

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x060019A0 RID: 6560 RVA: 0x0005CA8B File Offset: 0x0005AC8B
		// (set) Token: 0x060019A1 RID: 6561 RVA: 0x0005CA93 File Offset: 0x0005AC93
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

		// Token: 0x04000C24 RID: 3108
		private TextObject _groupNameText;

		// Token: 0x04000C25 RID: 3109
		private float _influenceBonus;

		// Token: 0x04000C26 RID: 3110
		private Action<ClanSupporterGroupVM> _onSelection;

		// Token: 0x04000C27 RID: 3111
		private string _titleText;

		// Token: 0x04000C28 RID: 3112
		private string _influenceBonusDescription;

		// Token: 0x04000C29 RID: 3113
		private string _name;

		// Token: 0x04000C2A RID: 3114
		private string _totalInfluence;

		// Token: 0x04000C2B RID: 3115
		private bool _isSelected;

		// Token: 0x04000C2C RID: 3116
		private MBBindingList<ClanSupporterItemVM> _supporters;
	}
}
