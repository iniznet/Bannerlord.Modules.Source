using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x0200008E RID: 142
	public class SettlementGovernorSelectionItemVM : ViewModel
	{
		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x00037BBF File Offset: 0x00035DBF
		public Hero Governor { get; }

		// Token: 0x06000DE1 RID: 3553 RVA: 0x00037BC8 File Offset: 0x00035DC8
		public SettlementGovernorSelectionItemVM(Hero governor, Action<SettlementGovernorSelectionItemVM> onSelection)
		{
			this.Governor = governor;
			this._onSelection = onSelection;
			if (governor != null)
			{
				this.Visual = new ImageIdentifierVM(CampaignUIHelper.GetCharacterCode(this.Governor.CharacterObject, true));
				this.GovernorHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetHeroGovernorEffectsTooltip(this.Governor, Settlement.CurrentSettlement));
			}
			else
			{
				this.Visual = new ImageIdentifierVM(ImageIdentifierType.Null);
				this.GovernorHint = new BasicTooltipViewModel();
			}
			this.RefreshValues();
		}

		// Token: 0x06000DE2 RID: 3554 RVA: 0x00037C40 File Offset: 0x00035E40
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Governor != null)
			{
				this.Name = this.Governor.Name.ToString();
				return;
			}
			this.Visual = new ImageIdentifierVM(ImageIdentifierType.Null);
			this.Name = new TextObject("{=koX9okuG}None", null).ToString();
		}

		// Token: 0x06000DE3 RID: 3555 RVA: 0x00037C94 File Offset: 0x00035E94
		public void OnSelection()
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			Hero hero;
			if (currentSettlement == null)
			{
				hero = null;
			}
			else
			{
				Town town = currentSettlement.Town;
				hero = ((town != null) ? town.Governor : null);
			}
			Hero hero2 = hero;
			bool flag = this.Governor == null;
			if (hero2 != this.Governor && (!flag || hero2 != null))
			{
				ValueTuple<TextObject, TextObject> governorSelectionConfirmationPopupTexts = CampaignUIHelper.GetGovernorSelectionConfirmationPopupTexts(hero2, this.Governor, currentSettlement);
				InformationManager.ShowInquiry(new InquiryData(governorSelectionConfirmationPopupTexts.Item1.ToString(), governorSelectionConfirmationPopupTexts.Item2.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					this._onSelection(this);
				}, null, "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x00037D45 File Offset: 0x00035F45
		// (set) Token: 0x06000DE5 RID: 3557 RVA: 0x00037D4D File Offset: 0x00035F4D
		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06000DE6 RID: 3558 RVA: 0x00037D6B File Offset: 0x00035F6B
		// (set) Token: 0x06000DE7 RID: 3559 RVA: 0x00037D73 File Offset: 0x00035F73
		[DataSourceProperty]
		public BasicTooltipViewModel GovernorHint
		{
			get
			{
				return this._governorHint;
			}
			set
			{
				if (value != this._governorHint)
				{
					this._governorHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "GovernorHint");
				}
			}
		}

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x00037D91 File Offset: 0x00035F91
		// (set) Token: 0x06000DE9 RID: 3561 RVA: 0x00037D99 File Offset: 0x00035F99
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

		// Token: 0x0400066F RID: 1647
		private readonly Action<SettlementGovernorSelectionItemVM> _onSelection;

		// Token: 0x04000671 RID: 1649
		private ImageIdentifierVM _visual;

		// Token: 0x04000672 RID: 1650
		private string _name;

		// Token: 0x04000673 RID: 1651
		private BasicTooltipViewModel _governorHint;
	}
}
