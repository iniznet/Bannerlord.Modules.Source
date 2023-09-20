using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	// Token: 0x020000D5 RID: 213
	public class EducationOptionVM : StringItemWithActionVM
	{
		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x060013B9 RID: 5049 RVA: 0x0004B8FB File Offset: 0x00049AFB
		// (set) Token: 0x060013BA RID: 5050 RVA: 0x0004B903 File Offset: 0x00049B03
		public string OptionEffect { get; private set; }

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x060013BB RID: 5051 RVA: 0x0004B90C File Offset: 0x00049B0C
		// (set) Token: 0x060013BC RID: 5052 RVA: 0x0004B914 File Offset: 0x00049B14
		public string OptionDescription { get; private set; }

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x060013BD RID: 5053 RVA: 0x0004B91D File Offset: 0x00049B1D
		// (set) Token: 0x060013BE RID: 5054 RVA: 0x0004B925 File Offset: 0x00049B25
		public EducationCampaignBehavior.EducationCharacterProperties[] CharacterProperties { get; private set; }

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x060013BF RID: 5055 RVA: 0x0004B92E File Offset: 0x00049B2E
		// (set) Token: 0x060013C0 RID: 5056 RVA: 0x0004B936 File Offset: 0x00049B36
		public string ActionID { get; private set; }

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x060013C1 RID: 5057 RVA: 0x0004B93F File Offset: 0x00049B3F
		// (set) Token: 0x060013C2 RID: 5058 RVA: 0x0004B947 File Offset: 0x00049B47
		public ValueTuple<CharacterAttribute, int>[] OptionAttributes { get; private set; }

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x060013C3 RID: 5059 RVA: 0x0004B950 File Offset: 0x00049B50
		// (set) Token: 0x060013C4 RID: 5060 RVA: 0x0004B958 File Offset: 0x00049B58
		public ValueTuple<SkillObject, int>[] OptionSkills { get; private set; }

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x060013C5 RID: 5061 RVA: 0x0004B961 File Offset: 0x00049B61
		// (set) Token: 0x060013C6 RID: 5062 RVA: 0x0004B969 File Offset: 0x00049B69
		public ValueTuple<SkillObject, int>[] OptionFocusPoints { get; private set; }

		// Token: 0x060013C7 RID: 5063 RVA: 0x0004B974 File Offset: 0x00049B74
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

		// Token: 0x060013C8 RID: 5064 RVA: 0x0004B9D4 File Offset: 0x00049BD4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OptionEffect = this._optionEffectObject.ToString();
			this.OptionDescription = this._optionDescriptionObject.ToString();
			base.ActionText = this._optionTextObject.ToString();
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x060013C9 RID: 5065 RVA: 0x0004BA0F File Offset: 0x00049C0F
		// (set) Token: 0x060013CA RID: 5066 RVA: 0x0004BA17 File Offset: 0x00049C17
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

		// Token: 0x04000934 RID: 2356
		private readonly TextObject _optionTextObject;

		// Token: 0x04000935 RID: 2357
		private readonly TextObject _optionDescriptionObject;

		// Token: 0x04000936 RID: 2358
		private readonly TextObject _optionEffectObject;

		// Token: 0x04000937 RID: 2359
		private bool _isSelected;
	}
}
