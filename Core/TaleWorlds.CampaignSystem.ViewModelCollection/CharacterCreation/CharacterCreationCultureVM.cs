using System;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x02000124 RID: 292
	public class CharacterCreationCultureVM : ViewModel
	{
		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x06001C2E RID: 7214 RVA: 0x000653DE File Offset: 0x000635DE
		public CultureObject Culture { get; }

		// Token: 0x06001C2F RID: 7215 RVA: 0x000653E8 File Offset: 0x000635E8
		public CharacterCreationCultureVM(CultureObject culture, Action<CharacterCreationCultureVM> onSelection)
		{
			this._onSelection = onSelection;
			this.Culture = culture;
			MBTextManager.SetTextVariable("FOCUS_VALUE", CharacterCreationContentBase.Instance.FocusToAddByCulture);
			MBTextManager.SetTextVariable("EXP_VALUE", CharacterCreationContentBase.Instance.SkillLevelToAddByCulture);
			this.DescriptionText = GameTexts.FindText("str_culture_description", this.Culture.StringId).ToString();
			this.ShortenedNameText = GameTexts.FindText("str_culture_rich_name", this.Culture.StringId).ToString();
			this.NameText = GameTexts.FindText("str_culture_rich_name", this.Culture.StringId).ToString();
			this.CultureID = ((culture == null) ? "" : culture.StringId);
			this.Feats = new MBBindingList<CharacterCreationCultureFeatVM>();
			foreach (FeatObject featObject in this.Culture.GetCulturalFeats((FeatObject x) => x.IsPositive))
			{
				this.Feats.Add(new CharacterCreationCultureFeatVM(true, featObject.Description.ToString()));
			}
			foreach (FeatObject featObject2 in this.Culture.GetCulturalFeats((FeatObject x) => !x.IsPositive))
			{
				this.Feats.Add(new CharacterCreationCultureFeatVM(false, featObject2.Description.ToString()));
			}
		}

		// Token: 0x06001C30 RID: 7216 RVA: 0x000655AC File Offset: 0x000637AC
		public void ExecuteSelectCulture()
		{
			this._onSelection(this);
		}

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x06001C31 RID: 7217 RVA: 0x000655BA File Offset: 0x000637BA
		// (set) Token: 0x06001C32 RID: 7218 RVA: 0x000655C2 File Offset: 0x000637C2
		[DataSourceProperty]
		public string CultureID
		{
			get
			{
				return this._cultureID;
			}
			set
			{
				if (value != this._cultureID)
				{
					this._cultureID = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureID");
				}
			}
		}

		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x06001C33 RID: 7219 RVA: 0x000655E5 File Offset: 0x000637E5
		// (set) Token: 0x06001C34 RID: 7220 RVA: 0x000655ED File Offset: 0x000637ED
		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x06001C35 RID: 7221 RVA: 0x00065610 File Offset: 0x00063810
		// (set) Token: 0x06001C36 RID: 7222 RVA: 0x00065618 File Offset: 0x00063818
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x06001C37 RID: 7223 RVA: 0x0006563B File Offset: 0x0006383B
		// (set) Token: 0x06001C38 RID: 7224 RVA: 0x00065643 File Offset: 0x00063843
		[DataSourceProperty]
		public string ShortenedNameText
		{
			get
			{
				return this._shortenedNameText;
			}
			set
			{
				if (value != this._shortenedNameText)
				{
					this._shortenedNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShortenedNameText");
				}
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x06001C39 RID: 7225 RVA: 0x00065666 File Offset: 0x00063866
		// (set) Token: 0x06001C3A RID: 7226 RVA: 0x0006566E File Offset: 0x0006386E
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

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x06001C3B RID: 7227 RVA: 0x0006568C File Offset: 0x0006388C
		// (set) Token: 0x06001C3C RID: 7228 RVA: 0x00065694 File Offset: 0x00063894
		[DataSourceProperty]
		public MBBindingList<CharacterCreationCultureFeatVM> Feats
		{
			get
			{
				return this._feats;
			}
			set
			{
				if (value != this._feats)
				{
					this._feats = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterCreationCultureFeatVM>>(value, "Feats");
				}
			}
		}

		// Token: 0x04000D49 RID: 3401
		private readonly Action<CharacterCreationCultureVM> _onSelection;

		// Token: 0x04000D4A RID: 3402
		private string _descriptionText = "";

		// Token: 0x04000D4B RID: 3403
		private string _nameText;

		// Token: 0x04000D4C RID: 3404
		private string _shortenedNameText;

		// Token: 0x04000D4D RID: 3405
		private bool _isSelected;

		// Token: 0x04000D4E RID: 3406
		private string _cultureID;

		// Token: 0x04000D4F RID: 3407
		private MBBindingList<CharacterCreationCultureFeatVM> _feats;
	}
}
