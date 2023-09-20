using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class CampaignOptionsControllerVM : ViewModel
	{
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

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignOptionsManager.ClearCachedOptions();
		}

		private void OnOptionChanged(CampaignOptionItemVM optionVM)
		{
			this.UpdatePresetData(optionVM);
			this.Options.ApplyActionOnAllItems(delegate(CampaignOptionItemVM x)
			{
				x.RefreshDisabledStatus();
			});
		}

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

		private const string _difficultyPresetsId = "DifficultyPresets";

		internal const int AutosaveDisableValue = -1;

		private SelectionCampaignOptionData _difficultyPreset;

		private Dictionary<string, CampaignOptionItemVM> _optionItems;

		private bool _isUpdatingPresetData;

		private List<CampaignOptionItemVM> _difficultyPresetRelatedOptions;

		private MBBindingList<CampaignOptionItemVM> _options;

		private class CampaignOptionComparer : IComparer<CampaignOptionItemVM>
		{
			public int Compare(CampaignOptionItemVM x, CampaignOptionItemVM y)
			{
				int priorityIndex = x.OptionData.GetPriorityIndex();
				int priorityIndex2 = y.OptionData.GetPriorityIndex();
				return priorityIndex.CompareTo(priorityIndex2);
			}
		}
	}
}
