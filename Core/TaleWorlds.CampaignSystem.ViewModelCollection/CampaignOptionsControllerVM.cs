using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x02000008 RID: 8
	public class CampaignOptionsControllerVM : ViewModel
	{
		// Token: 0x0600007A RID: 122 RVA: 0x000034D0 File Offset: 0x000016D0
		public CampaignOptionsControllerVM(MBBindingList<CampaignOptionItemVM> options)
		{
			this._optionItems = new Dictionary<string, CampaignOptionItemVM>();
			this.Options = options;
			CampaignOptionItemVM campaignOptionItemVM = this.Options.FirstOrDefault((CampaignOptionItemVM x) => x.OptionData.GetIdentifier() == "DifficultyPresets");
			this._difficultyPreset = ((campaignOptionItemVM != null) ? campaignOptionItemVM.OptionData : null) as SelectionCampaignOptionData;
			this.Options.Sort(new CampaignOptionsControllerVM.CampaignOptionComparer());
			for (int i = 0; i < this.Options.Count; i++)
			{
				this._optionItems.Add(this.Options[i].OptionData.GetIdentifier(), this.Options[i]);
			}
			this.Options.ApplyActionOnAllItems(delegate(CampaignOptionItemVM x)
			{
				x.RefreshDisabledStatus();
			});
			this.Options.ApplyActionOnAllItems(delegate(CampaignOptionItemVM x)
			{
				x.SetOnValueChangedCallback(new Action<CampaignOptionItemVM>(this.OnOptionChanged));
			});
			this._difficultyPresetRelatedOptions = this.Options.Where((CampaignOptionItemVM x) => x.OptionData.IsRelatedToDifficultyPreset()).ToList<CampaignOptionItemVM>();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000035FF File Offset: 0x000017FF
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignOptionsManager.ClearCachedOptions();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000360C File Offset: 0x0000180C
		private void OnOptionChanged(CampaignOptionItemVM optionVM)
		{
			this.UpdatePresetData(optionVM);
			this.Options.ApplyActionOnAllItems(delegate(CampaignOptionItemVM x)
			{
				x.RefreshDisabledStatus();
			});
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003640 File Offset: 0x00001840
		private void UpdatePresetData(CampaignOptionItemVM changedOption)
		{
			if (this._isUpdatingPresetData)
			{
				return;
			}
			this._isUpdatingPresetData = true;
			if (changedOption.OptionData == this._difficultyPreset)
			{
				using (List<CampaignOptionItemVM>.Enumerator enumerator = this._difficultyPresetRelatedOptions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CampaignOptionItemVM campaignOptionItemVM = enumerator.Current;
						string identifier = campaignOptionItemVM.OptionData.GetIdentifier();
						CampaignOptionsDifficultyPresets campaignOptionsDifficultyPresets = (CampaignOptionsDifficultyPresets)this._difficultyPreset.GetValue();
						float valueFromDifficultyPreset = campaignOptionItemVM.OptionData.GetValueFromDifficultyPreset(campaignOptionsDifficultyPresets);
						CampaignOptionItemVM campaignOptionItemVM2;
						if (this._optionItems.TryGetValue(identifier, out campaignOptionItemVM2) && !campaignOptionItemVM2.IsDisabled)
						{
							campaignOptionItemVM2.SetValue(valueFromDifficultyPreset);
						}
					}
					goto IL_173;
				}
			}
			CampaignOptionItemVM campaignOptionItemVM3;
			if (this._difficultyPresetRelatedOptions.Any((CampaignOptionItemVM x) => x.OptionData.GetIdentifier() == changedOption.OptionData.GetIdentifier()) && this._optionItems.TryGetValue(this._difficultyPreset.GetIdentifier(), out campaignOptionItemVM3))
			{
				CampaignOptionsDifficultyPresets campaignOptionsDifficultyPresets2 = this.FindOptionPresetForValue(changedOption.OptionData);
				bool flag = true;
				if (campaignOptionsDifficultyPresets2 != CampaignOptionsDifficultyPresets.Custom)
				{
					for (int i = 0; i < this._difficultyPresetRelatedOptions.Count; i++)
					{
						if (!this._difficultyPresetRelatedOptions[i].IsDisabled)
						{
							CampaignOptionsDifficultyPresets campaignOptionsDifficultyPresets3 = this.FindOptionPresetForValue(this._difficultyPresetRelatedOptions[i].OptionData);
							if (campaignOptionsDifficultyPresets2 != campaignOptionsDifficultyPresets3)
							{
								campaignOptionItemVM3.SetValue(3f);
								flag = false;
								break;
							}
						}
					}
				}
				if (campaignOptionsDifficultyPresets2 != CampaignOptionsDifficultyPresets.Custom && flag)
				{
					campaignOptionItemVM3.SetValue((float)campaignOptionsDifficultyPresets2);
				}
			}
			IL_173:
			this._isUpdatingPresetData = false;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000037D8 File Offset: 0x000019D8
		private CampaignOptionsDifficultyPresets FindOptionPresetForValue(ICampaignOptionData option)
		{
			float value = option.GetValue();
			if (option.GetValueFromDifficultyPreset(CampaignOptionsDifficultyPresets.Freebooter) == value)
			{
				return CampaignOptionsDifficultyPresets.Freebooter;
			}
			if (option.GetValueFromDifficultyPreset(CampaignOptionsDifficultyPresets.Warrior) == value)
			{
				return CampaignOptionsDifficultyPresets.Warrior;
			}
			if (option.GetValueFromDifficultyPreset(CampaignOptionsDifficultyPresets.Bannerlord) == value)
			{
				return CampaignOptionsDifficultyPresets.Bannerlord;
			}
			return CampaignOptionsDifficultyPresets.Custom;
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00003811 File Offset: 0x00001A11
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00003819 File Offset: 0x00001A19
		[DataSourceProperty]
		public MBBindingList<CampaignOptionItemVM> Options
		{
			get
			{
				return this._options;
			}
			set
			{
				if (value != this._options)
				{
					this._options = value;
					base.OnPropertyChangedWithValue<MBBindingList<CampaignOptionItemVM>>(value, "Options");
				}
			}
		}

		// Token: 0x04000048 RID: 72
		private const string _difficultyPresetsId = "DifficultyPresets";

		// Token: 0x04000049 RID: 73
		internal const int AutosaveDisableValue = -1;

		// Token: 0x0400004A RID: 74
		private SelectionCampaignOptionData _difficultyPreset;

		// Token: 0x0400004B RID: 75
		private Dictionary<string, CampaignOptionItemVM> _optionItems;

		// Token: 0x0400004C RID: 76
		private bool _isUpdatingPresetData;

		// Token: 0x0400004D RID: 77
		private List<CampaignOptionItemVM> _difficultyPresetRelatedOptions;

		// Token: 0x0400004E RID: 78
		private MBBindingList<CampaignOptionItemVM> _options;

		// Token: 0x02000138 RID: 312
		private class CampaignOptionComparer : IComparer<CampaignOptionItemVM>
		{
			// Token: 0x06001E2E RID: 7726 RVA: 0x0006B888 File Offset: 0x00069A88
			public int Compare(CampaignOptionItemVM x, CampaignOptionItemVM y)
			{
				int priorityIndex = x.OptionData.GetPriorityIndex();
				int priorityIndex2 = y.OptionData.GetPriorityIndex();
				return priorityIndex.CompareTo(priorityIndex2);
			}
		}
	}
}
