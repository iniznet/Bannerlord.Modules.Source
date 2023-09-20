using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	public class CharacterAttributeItemVM : ViewModel
	{
		public CharacterAttribute AttributeType { get; private set; }

		public CharacterAttributeItemVM(Hero hero, CharacterAttribute currAtt, CharacterVM developerVM, Action<CharacterAttributeItemVM> onInpectAttribute, Action<CharacterAttributeItemVM> onAddAttributePoint)
		{
			this._hero = hero;
			this._developer = this._hero.HeroDeveloper;
			this._characterVM = developerVM;
			this.AttributeType = currAtt;
			this._onInpectAttribute = onInpectAttribute;
			this._onAddAttributePoint = onAddAttributePoint;
			this._initialAttValue = hero.GetAttributeValue(currAtt);
			this.AttributeValue = this._initialAttValue;
			this.BoundSkills = new MBBindingList<AttributeBoundSkillItemVM>();
			this.RefreshWithCurrentValues();
			this.RefreshValues();
			this.UnspentAttributePoints = this._characterVM.UnspentAttributePoints;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.AttributeType.Abbreviation.ToString();
			string text = this.AttributeType.Description.ToString();
			GameTexts.SetVariable("STR1", text);
			GameTexts.SetVariable("ATTRIBUTE_NAME", this.AttributeType.Name);
			TextObject textObject = GameTexts.FindText("str_skill_attribute_bound_skills", null);
			textObject.SetTextVariable("IS_SOCIAL", (this.AttributeType == DefaultCharacterAttributes.Social) ? 1 : 0);
			GameTexts.SetVariable("STR2", textObject);
			this.Description = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			TextObject textObject2 = GameTexts.FindText("str_skill_attribute_increase_description", null);
			textObject2.SetTextVariable("IS_SOCIAL", (this.AttributeType == DefaultCharacterAttributes.Social) ? 1 : 0);
			this.IncreaseHelpText = textObject2.ToString();
			this.BoundSkills.Clear();
			foreach (SkillObject skillObject in this.AttributeType.Skills)
			{
				this.BoundSkills.Add(new AttributeBoundSkillItemVM(skillObject));
			}
		}

		public void ExecuteInspectAttribute()
		{
			Action<CharacterAttributeItemVM> onInpectAttribute = this._onInpectAttribute;
			if (onInpectAttribute == null)
			{
				return;
			}
			onInpectAttribute(this);
		}

		public void ExecuteAddAttributePoint()
		{
			int attributeValue = this.AttributeValue;
			this.AttributeValue = attributeValue + 1;
			Action<CharacterAttributeItemVM> onAddAttributePoint = this._onAddAttributePoint;
			if (onAddAttributePoint != null)
			{
				onAddAttributePoint(this);
			}
			this.UnspentAttributePoints = this._characterVM.UnspentAttributePoints;
			this.RefreshWithCurrentValues();
		}

		public void Reset()
		{
			this.AttributeValue = this._initialAttValue;
			this.RefreshWithCurrentValues();
		}

		public void RefreshWithCurrentValues()
		{
			this.UnspentAttributePoints = this._characterVM.UnspentAttributePoints;
			this.CanAddPoint = this.AttributeValue < Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute && this._characterVM.UnspentAttributePoints > 0;
			this.IsAttributeAtMax = this.AttributeValue >= Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute;
		}

		public void Commit()
		{
			for (int i = 0; i < this.AttributeValue - this._initialAttValue; i++)
			{
				this._developer.AddAttribute(this.AttributeType, 1, true);
			}
		}

		[DataSourceProperty]
		public MBBindingList<AttributeBoundSkillItemVM> BoundSkills
		{
			get
			{
				return this._boundSkills;
			}
			set
			{
				if (value != this._boundSkills)
				{
					this._boundSkills = value;
					base.OnPropertyChangedWithValue<MBBindingList<AttributeBoundSkillItemVM>>(value, "BoundSkills");
				}
			}
		}

		[DataSourceProperty]
		public int AttributeValue
		{
			get
			{
				return this._atttributeValue;
			}
			set
			{
				if (value != this._atttributeValue)
				{
					this._atttributeValue = value;
					base.OnPropertyChangedWithValue(value, "AttributeValue");
				}
			}
		}

		[DataSourceProperty]
		public int UnspentAttributePoints
		{
			get
			{
				return this._unspentAttributePoints;
			}
			set
			{
				if (value != this._unspentAttributePoints)
				{
					this._unspentAttributePoints = value;
					base.OnPropertyChangedWithValue(value, "UnspentAttributePoints");
					GameTexts.SetVariable("NUMBER", value);
					this.UnspentAttributePointsText = GameTexts.FindText("str_free_attribute_points", null).ToString();
				}
			}
		}

		[DataSourceProperty]
		public string UnspentAttributePointsText
		{
			get
			{
				return this._unspentAttributePointsText;
			}
			set
			{
				if (value != this._unspentAttributePointsText)
				{
					this._unspentAttributePointsText = value;
					base.OnPropertyChangedWithValue<string>(value, "UnspentAttributePointsText");
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
		public string NameExtended
		{
			get
			{
				return this._nameExtended;
			}
			set
			{
				if (value != this._nameExtended)
				{
					this._nameExtended = value;
					base.OnPropertyChangedWithValue<string>(value, "NameExtended");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		[DataSourceProperty]
		public string IncreaseHelpText
		{
			get
			{
				return this._increaseHelpText;
			}
			set
			{
				if (value != this._increaseHelpText)
				{
					this._increaseHelpText = value;
					base.OnPropertyChangedWithValue<string>(value, "IncreaseHelpText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInspecting
		{
			get
			{
				return this._isInspecting;
			}
			set
			{
				if (value != this._isInspecting)
				{
					this._isInspecting = value;
					base.OnPropertyChangedWithValue(value, "IsInspecting");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAttributeAtMax
		{
			get
			{
				return this._isAttributeAtMax;
			}
			set
			{
				if (value != this._isAttributeAtMax)
				{
					this._isAttributeAtMax = value;
					base.OnPropertyChangedWithValue(value, "IsAttributeAtMax");
				}
			}
		}

		[DataSourceProperty]
		public bool CanAddPoint
		{
			get
			{
				return this._canAddPoint;
			}
			set
			{
				if (value != this._canAddPoint)
				{
					this._canAddPoint = value;
					base.OnPropertyChangedWithValue(value, "CanAddPoint");
				}
			}
		}

		private readonly Hero _hero;

		private readonly IHeroDeveloper _developer;

		private readonly int _initialAttValue;

		private readonly Action<CharacterAttributeItemVM> _onInpectAttribute;

		private readonly Action<CharacterAttributeItemVM> _onAddAttributePoint;

		private readonly CharacterVM _characterVM;

		private int _atttributeValue;

		private int _unspentAttributePoints;

		private string _unspentAttributePointsText;

		private bool _canAddPoint;

		private bool _isInspecting;

		private bool _isAttributeAtMax;

		private string _name;

		private string _nameExtended;

		private string _description;

		private string _increaseHelpText;

		private MBBindingList<AttributeBoundSkillItemVM> _boundSkills;
	}
}
