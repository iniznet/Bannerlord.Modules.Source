using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000CD RID: 205
	public class EncyclopediaUnitVM : ViewModel
	{
		// Token: 0x06001347 RID: 4935 RVA: 0x00049FE8 File Offset: 0x000481E8
		public EncyclopediaUnitVM(CharacterObject character, bool isActive)
		{
			if (character != null)
			{
				CharacterCode characterCode = CharacterCode.CreateFrom(character);
				this.ImageIdentifier = new ImageIdentifierVM(characterCode);
				this._character = character;
				this.IsActiveUnit = isActive;
				this.TierIconData = CampaignUIHelper.GetCharacterTierData(character, true);
				this.TypeIconData = CampaignUIHelper.GetCharacterTypeData(character, true);
			}
			else
			{
				this.IsActiveUnit = false;
			}
			this.RefreshValues();
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x0004A048 File Offset: 0x00048248
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._character != null)
			{
				this.NameText = this._character.Name.ToString();
			}
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x0004A06E File Offset: 0x0004826E
		public void ExecuteLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this._character.EncyclopediaLink);
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x0004A08A File Offset: 0x0004828A
		public virtual void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(CharacterObject), new object[] { this._character });
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x0004A0AA File Offset: 0x000482AA
		public virtual void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x0600134C RID: 4940 RVA: 0x0004A0B1 File Offset: 0x000482B1
		// (set) Token: 0x0600134D RID: 4941 RVA: 0x0004A0B9 File Offset: 0x000482B9
		[DataSourceProperty]
		public bool IsActiveUnit
		{
			get
			{
				return this._isActiveUnit;
			}
			set
			{
				if (value != this._isActiveUnit)
				{
					this._isActiveUnit = value;
					base.OnPropertyChangedWithValue(value, "IsActiveUnit");
				}
			}
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x0600134E RID: 4942 RVA: 0x0004A0D7 File Offset: 0x000482D7
		// (set) Token: 0x0600134F RID: 4943 RVA: 0x0004A0DF File Offset: 0x000482DF
		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06001350 RID: 4944 RVA: 0x0004A0FD File Offset: 0x000482FD
		// (set) Token: 0x06001351 RID: 4945 RVA: 0x0004A105 File Offset: 0x00048305
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

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06001352 RID: 4946 RVA: 0x0004A128 File Offset: 0x00048328
		// (set) Token: 0x06001353 RID: 4947 RVA: 0x0004A130 File Offset: 0x00048330
		[DataSourceProperty]
		public StringItemWithHintVM TierIconData
		{
			get
			{
				return this._tierIconData;
			}
			set
			{
				if (value != this._tierIconData)
				{
					this._tierIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TierIconData");
				}
			}
		}

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06001354 RID: 4948 RVA: 0x0004A14E File Offset: 0x0004834E
		// (set) Token: 0x06001355 RID: 4949 RVA: 0x0004A156 File Offset: 0x00048356
		[DataSourceProperty]
		public StringItemWithHintVM TypeIconData
		{
			get
			{
				return this._typeIconData;
			}
			set
			{
				if (value != this._typeIconData)
				{
					this._typeIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TypeIconData");
				}
			}
		}

		// Token: 0x040008ED RID: 2285
		private CharacterObject _character;

		// Token: 0x040008EE RID: 2286
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x040008EF RID: 2287
		private string _nameText;

		// Token: 0x040008F0 RID: 2288
		private bool _isActiveUnit;

		// Token: 0x040008F1 RID: 2289
		private StringItemWithHintVM _tierIconData;

		// Token: 0x040008F2 RID: 2290
		private StringItemWithHintVM _typeIconData;
	}
}
