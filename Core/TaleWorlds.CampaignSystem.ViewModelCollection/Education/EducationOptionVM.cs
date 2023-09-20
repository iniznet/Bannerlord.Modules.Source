using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	public class EducationOptionVM : StringItemWithActionVM
	{
		public string OptionEffect { get; private set; }

		public string OptionDescription { get; private set; }

		public EducationCampaignBehavior.EducationCharacterProperties[] CharacterProperties { get; private set; }

		public string ActionID { get; private set; }

		public ValueTuple<CharacterAttribute, int>[] OptionAttributes { get; private set; }

		public ValueTuple<SkillObject, int>[] OptionSkills { get; private set; }

		public ValueTuple<SkillObject, int>[] OptionFocusPoints { get; private set; }

		public EducationOptionVM(Action<object> onExecute, string optionId, TextObject optionText, TextObject optionDescription, TextObject optionEffect, bool isSelected, ValueTuple<CharacterAttribute, int>[] optionAttributes, ValueTuple<SkillObject, int>[] optionSkills, ValueTuple<SkillObject, int>[] optionFocusPoints, EducationCampaignBehavior.EducationCharacterProperties[] characterProperties)
			: base(onExecute, optionText.ToString(), optionId)
		{
			this.IsSelected = isSelected;
			this.CharacterProperties = characterProperties;
			this._optionTextObject = optionText;
			this._optionDescriptionObject = optionDescription;
			this._optionEffectObject = optionEffect;
			this.OptionAttributes = optionAttributes;
			this.OptionSkills = optionSkills;
			this.OptionFocusPoints = optionFocusPoints;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OptionEffect = this._optionEffectObject.ToString();
			this.OptionDescription = this._optionDescriptionObject.ToString();
			base.ActionText = this._optionTextObject.ToString();
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

		private readonly TextObject _optionTextObject;

		private readonly TextObject _optionDescriptionObject;

		private readonly TextObject _optionEffectObject;

		private bool _isSelected;
	}
}
