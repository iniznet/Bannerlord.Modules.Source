using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	// Token: 0x02000116 RID: 278
	public class CharacterAttributeItemVM : ViewModel
	{
		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x06001AA6 RID: 6822 RVA: 0x00060CEC File Offset: 0x0005EEEC
		// (set) Token: 0x06001AA7 RID: 6823 RVA: 0x00060CF4 File Offset: 0x0005EEF4
		public CharacterAttribute AttributeType { get; private set; }

		// Token: 0x06001AA8 RID: 6824 RVA: 0x00060D00 File Offset: 0x0005EF00
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

		// Token: 0x06001AA9 RID: 6825 RVA: 0x00060D8C File Offset: 0x0005EF8C
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

		// Token: 0x06001AAA RID: 6826 RVA: 0x00060EC8 File Offset: 0x0005F0C8
		public void ExecuteInspectAttribute()
		{
			Action<CharacterAttributeItemVM> onInpectAttribute = this._onInpectAttribute;
			if (onInpectAttribute == null)
			{
				return;
			}
			onInpectAttribute(this);
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x00060EDC File Offset: 0x0005F0DC
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

		// Token: 0x06001AAC RID: 6828 RVA: 0x00060F22 File Offset: 0x0005F122
		public void Reset()
		{
			this.AttributeValue = this._initialAttValue;
			this.RefreshWithCurrentValues();
		}

		// Token: 0x06001AAD RID: 6829 RVA: 0x00060F38 File Offset: 0x0005F138
		public void RefreshWithCurrentValues()
		{
			this.UnspentAttributePoints = this._characterVM.UnspentAttributePoints;
			this.CanAddPoint = this.AttributeValue < Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute && this._characterVM.UnspentAttributePoints > 0;
			this.IsAttributeAtMax = this.AttributeValue >= Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute;
		}

		// Token: 0x06001AAE RID: 6830 RVA: 0x00060FB0 File Offset: 0x0005F1B0
		public void Commit()
		{
			for (int i = 0; i < this.AttributeValue - this._initialAttValue; i++)
			{
				this._developer.AddAttribute(this.AttributeType, 1, true);
			}
		}

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06001AAF RID: 6831 RVA: 0x00060FE8 File Offset: 0x0005F1E8
		// (set) Token: 0x06001AB0 RID: 6832 RVA: 0x00060FF0 File Offset: 0x0005F1F0
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

		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06001AB1 RID: 6833 RVA: 0x0006100E File Offset: 0x0005F20E
		// (set) Token: 0x06001AB2 RID: 6834 RVA: 0x00061016 File Offset: 0x0005F216
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

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x06001AB3 RID: 6835 RVA: 0x00061034 File Offset: 0x0005F234
		// (set) Token: 0x06001AB4 RID: 6836 RVA: 0x0006103C File Offset: 0x0005F23C
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

		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06001AB5 RID: 6837 RVA: 0x0006107B File Offset: 0x0005F27B
		// (set) Token: 0x06001AB6 RID: 6838 RVA: 0x00061083 File Offset: 0x0005F283
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

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06001AB7 RID: 6839 RVA: 0x000610A6 File Offset: 0x0005F2A6
		// (set) Token: 0x06001AB8 RID: 6840 RVA: 0x000610AE File Offset: 0x0005F2AE
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

		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06001AB9 RID: 6841 RVA: 0x000610D1 File Offset: 0x0005F2D1
		// (set) Token: 0x06001ABA RID: 6842 RVA: 0x000610D9 File Offset: 0x0005F2D9
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

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x06001ABB RID: 6843 RVA: 0x000610FC File Offset: 0x0005F2FC
		// (set) Token: 0x06001ABC RID: 6844 RVA: 0x00061104 File Offset: 0x0005F304
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

		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x06001ABD RID: 6845 RVA: 0x00061127 File Offset: 0x0005F327
		// (set) Token: 0x06001ABE RID: 6846 RVA: 0x0006112F File Offset: 0x0005F32F
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

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06001ABF RID: 6847 RVA: 0x00061152 File Offset: 0x0005F352
		// (set) Token: 0x06001AC0 RID: 6848 RVA: 0x0006115A File Offset: 0x0005F35A
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

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06001AC1 RID: 6849 RVA: 0x00061178 File Offset: 0x0005F378
		// (set) Token: 0x06001AC2 RID: 6850 RVA: 0x00061180 File Offset: 0x0005F380
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

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06001AC3 RID: 6851 RVA: 0x0006119E File Offset: 0x0005F39E
		// (set) Token: 0x06001AC4 RID: 6852 RVA: 0x000611A6 File Offset: 0x0005F3A6
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

		// Token: 0x04000C9D RID: 3229
		private readonly Hero _hero;

		// Token: 0x04000C9F RID: 3231
		private readonly IHeroDeveloper _developer;

		// Token: 0x04000CA0 RID: 3232
		private readonly int _initialAttValue;

		// Token: 0x04000CA1 RID: 3233
		private readonly Action<CharacterAttributeItemVM> _onInpectAttribute;

		// Token: 0x04000CA2 RID: 3234
		private readonly Action<CharacterAttributeItemVM> _onAddAttributePoint;

		// Token: 0x04000CA3 RID: 3235
		private readonly CharacterVM _characterVM;

		// Token: 0x04000CA4 RID: 3236
		private int _atttributeValue;

		// Token: 0x04000CA5 RID: 3237
		private int _unspentAttributePoints;

		// Token: 0x04000CA6 RID: 3238
		private string _unspentAttributePointsText;

		// Token: 0x04000CA7 RID: 3239
		private bool _canAddPoint;

		// Token: 0x04000CA8 RID: 3240
		private bool _isInspecting;

		// Token: 0x04000CA9 RID: 3241
		private bool _isAttributeAtMax;

		// Token: 0x04000CAA RID: 3242
		private string _name;

		// Token: 0x04000CAB RID: 3243
		private string _nameExtended;

		// Token: 0x04000CAC RID: 3244
		private string _description;

		// Token: 0x04000CAD RID: 3245
		private string _increaseHelpText;

		// Token: 0x04000CAE RID: 3246
		private MBBindingList<AttributeBoundSkillItemVM> _boundSkills;
	}
}
