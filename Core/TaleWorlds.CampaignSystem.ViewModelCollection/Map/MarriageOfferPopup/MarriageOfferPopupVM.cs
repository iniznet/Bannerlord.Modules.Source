using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MarriageOfferPopup
{
	public class MarriageOfferPopupVM : ViewModel
	{
		public MarriageOfferPopupVM(Hero suitor, Hero maiden)
		{
			this._marriageBehavior = Campaign.Current.GetCampaignBehavior<IMarriageOfferCampaignBehavior>();
			if (suitor.Clan == Clan.PlayerClan)
			{
				this.OffereeClanMember = new MarriageOfferPopupHeroVM(suitor);
				this.OffererClanMember = new MarriageOfferPopupHeroVM(maiden);
			}
			else
			{
				this.OffereeClanMember = new MarriageOfferPopupHeroVM(maiden);
				this.OffererClanMember = new MarriageOfferPopupHeroVM(suitor);
			}
			this.ConsequencesList = new MBBindingList<BindingListStringItem>();
			this.RefreshValues();
		}

		public void Update()
		{
			MarriageOfferPopupHeroVM offereeClanMember = this.OffereeClanMember;
			if (offereeClanMember != null)
			{
				offereeClanMember.Update();
			}
			MarriageOfferPopupHeroVM offererClanMember = this.OffererClanMember;
			if (offererClanMember == null)
			{
				return;
			}
			offererClanMember.Update();
		}

		public void ExecuteAcceptOffer()
		{
			IMarriageOfferCampaignBehavior marriageBehavior = this._marriageBehavior;
			if (marriageBehavior == null)
			{
				return;
			}
			marriageBehavior.OnMarriageOfferAcceptedOnPopUp();
		}

		public void ExecuteDeclineOffer()
		{
			IMarriageOfferCampaignBehavior marriageBehavior = this._marriageBehavior;
			if (marriageBehavior == null)
			{
				return;
			}
			marriageBehavior.OnMarriageOfferDeclinedOnPopUp();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject textObject = GameTexts.FindText("str_marriage_offer_from_clan", null);
			textObject.SetTextVariable("CLAN_NAME", this.OffererClanMember.Hero.Clan.Name);
			this.TitleText = textObject.ToString();
			this.ClanText = GameTexts.FindText("str_clan", null).ToString();
			this.AgeText = new TextObject("{=jaaQijQs}Age", null).ToString();
			this.OccupationText = new TextObject("{=GZxFIeiJ}Occupation", null).ToString();
			this.RelationText = new TextObject("{=BlidMNGT}Relation", null).ToString();
			this.ConsequencesText = new TextObject("{=Lm6Mkhru}Consequences", null).ToString();
			this.ButtonOkLabel = new TextObject("{=Y94H6XnK}Accept", null).ToString();
			this.ButtonCancelLabel = new TextObject("{=cOgmdp9e}Decline", null).ToString();
			this.ConsequencesList.Clear();
			IMarriageOfferCampaignBehavior marriageBehavior = this._marriageBehavior;
			foreach (TextObject textObject2 in (((marriageBehavior != null) ? marriageBehavior.GetMarriageAcceptedConsequences() : null) ?? new MBBindingList<TextObject>()))
			{
				this.ConsequencesList.Add(new BindingListStringItem("- " + textObject2.ToString()));
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			MarriageOfferPopupHeroVM offereeClanMember = this.OffereeClanMember;
			if (offereeClanMember != null)
			{
				offereeClanMember.OnFinalize();
			}
			MarriageOfferPopupHeroVM offererClanMember = this.OffererClanMember;
			if (offererClanMember == null)
			{
				return;
			}
			offererClanMember.OnFinalize();
		}

		public void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
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
		public string ClanText
		{
			get
			{
				return this._clanText;
			}
			set
			{
				if (value != this._clanText)
				{
					this._clanText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanText");
				}
			}
		}

		[DataSourceProperty]
		public string AgeText
		{
			get
			{
				return this._ageText;
			}
			set
			{
				if (value != this._ageText)
				{
					this._ageText = value;
					base.OnPropertyChangedWithValue<string>(value, "AgeText");
				}
			}
		}

		[DataSourceProperty]
		public string OccupationText
		{
			get
			{
				return this._occupationText;
			}
			set
			{
				if (value != this._occupationText)
				{
					this._occupationText = value;
					base.OnPropertyChangedWithValue<string>(value, "OccupationText");
				}
			}
		}

		[DataSourceProperty]
		public string RelationText
		{
			get
			{
				return this._relationText;
			}
			set
			{
				if (value != this._relationText)
				{
					this._relationText = value;
					base.OnPropertyChangedWithValue<string>(value, "RelationText");
				}
			}
		}

		[DataSourceProperty]
		public string ConsequencesText
		{
			get
			{
				return this._consequencesText;
			}
			set
			{
				if (value != this._consequencesText)
				{
					this._consequencesText = value;
					base.OnPropertyChangedWithValue<string>(value, "ConsequencesText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<BindingListStringItem> ConsequencesList
		{
			get
			{
				return this._consequencesList;
			}
			set
			{
				if (value != this._consequencesList)
				{
					this._consequencesList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BindingListStringItem>>(value, "ConsequencesList");
				}
			}
		}

		[DataSourceProperty]
		public string ButtonOkLabel
		{
			get
			{
				return this._buttonOkLabel;
			}
			set
			{
				if (value != this._buttonOkLabel)
				{
					this._buttonOkLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "ButtonOkLabel");
				}
			}
		}

		[DataSourceProperty]
		public string ButtonCancelLabel
		{
			get
			{
				return this._buttonCancelLabel;
			}
			set
			{
				if (value != this._buttonCancelLabel)
				{
					this._buttonCancelLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "ButtonCancelLabel");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEncyclopediaOpen
		{
			get
			{
				return this._isEncyclopediaOpen;
			}
			set
			{
				if (value != this._isEncyclopediaOpen)
				{
					this._isEncyclopediaOpen = value;
					base.OnPropertyChangedWithValue(value, "IsEncyclopediaOpen");
				}
			}
		}

		[DataSourceProperty]
		public MarriageOfferPopupHeroVM OffereeClanMember
		{
			get
			{
				return this._offereeClanMember;
			}
			set
			{
				if (value != this._offereeClanMember)
				{
					this._offereeClanMember = value;
					base.OnPropertyChangedWithValue<MarriageOfferPopupHeroVM>(value, "OffereeClanMember");
				}
			}
		}

		[DataSourceProperty]
		public MarriageOfferPopupHeroVM OffererClanMember
		{
			get
			{
				return this._offererClanMember;
			}
			set
			{
				if (value != this._offererClanMember)
				{
					this._offererClanMember = value;
					base.OnPropertyChangedWithValue<MarriageOfferPopupHeroVM>(value, "OffererClanMember");
				}
			}
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		private readonly IMarriageOfferCampaignBehavior _marriageBehavior;

		private string _titleText;

		private string _clanText;

		private string _ageText;

		private string _occupationText;

		private string _relationText;

		private string _consequencesText;

		private MBBindingList<BindingListStringItem> _consequencesList;

		private string _buttonOkLabel;

		private string _buttonCancelLabel;

		private bool _isEncyclopediaOpen;

		private MarriageOfferPopupHeroVM _offereeClanMember;

		private MarriageOfferPopupHeroVM _offererClanMember;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;
	}
}
