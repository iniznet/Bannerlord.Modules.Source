using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class SettlementGovernorSelectionItemVM : ViewModel
	{
		public Hero Governor { get; }

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

		private readonly Action<SettlementGovernorSelectionItemVM> _onSelection;

		private ImageIdentifierVM _visual;

		private string _name;

		private BasicTooltipViewModel _governorHint;
	}
}
