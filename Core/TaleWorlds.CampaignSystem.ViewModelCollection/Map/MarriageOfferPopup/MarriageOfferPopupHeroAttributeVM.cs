using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MarriageOfferPopup
{
	public class MarriageOfferPopupHeroAttributeVM : ViewModel
	{
		public MarriageOfferPopupHeroAttributeVM(Hero hero, CharacterAttribute attribute)
		{
			this._hero = hero;
			this._attribute = attribute;
			this.FillSkillsList();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject textObject = GameTexts.FindText("str_STR1_space_STR2", null);
			textObject.SetTextVariable("STR1", this._attribute.Name);
			TextObject textObject2 = GameTexts.FindText("str_STR_in_parentheses", null);
			textObject2.SetTextVariable("STR", this._hero.GetAttributeValue(this._attribute));
			textObject.SetTextVariable("STR2", textObject2);
			this._attributeText = textObject.ToString();
		}

		private void FillSkillsList()
		{
			this._attributeSkills = new MBBindingList<EncyclopediaSkillVM>();
			foreach (SkillObject skillObject in this._attribute.Skills)
			{
				this._attributeSkills.Add(new EncyclopediaSkillVM(skillObject, this._hero.GetSkillValue(skillObject)));
			}
		}

		[DataSourceProperty]
		public string AttributeText
		{
			get
			{
				return this._attributeText;
			}
			set
			{
				if (value != this._attributeText)
				{
					this._attributeText = value;
					base.OnPropertyChangedWithValue<string>(value, "AttributeText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EncyclopediaSkillVM> AttributeSkills
		{
			get
			{
				return this._attributeSkills;
			}
			set
			{
				if (value != this._attributeSkills)
				{
					this._attributeSkills = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSkillVM>>(value, "AttributeSkills");
				}
			}
		}

		private readonly Hero _hero;

		private readonly CharacterAttribute _attribute;

		private string _attributeText;

		private MBBindingList<EncyclopediaSkillVM> _attributeSkills;
	}
}
