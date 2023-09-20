using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public abstract class CampaignOptionData : ICampaignOptionData
	{
		public CampaignOptionData(string identifier, int priorityIndex, CampaignOptionEnableState enableState, Func<float> getValue, Action<float> setValue, Func<CampaignOptionDisableStatus> getIsDisabledWithReason = null, bool isRelatedToDifficultyPreset = false, Func<float, CampaignOptionsDifficultyPresets> onGetDifficultyPresetFromValue = null, Func<CampaignOptionsDifficultyPresets, float> onGetValueFromDifficultyPreset = null)
		{
			this._priorityIndex = priorityIndex;
			this._identifier = identifier;
			this._isRelatedToDifficultyPreset = isRelatedToDifficultyPreset;
			this._getIsDisabledWithReason = getIsDisabledWithReason;
			this._onGetDifficultyPresetFromValue = onGetDifficultyPresetFromValue;
			this._onGetValueFromDifficultyPreset = onGetValueFromDifficultyPreset;
			this._enableState = enableState;
			this._name = CampaignOptionData.GetNameOfOption(identifier);
			this._description = CampaignOptionData.GetDescriptionOfOption(identifier);
			this._getValue = getValue;
			this._setValue = setValue;
		}

		public static TextObject GetNameOfOption(string optionIdentifier)
		{
			TextObject textObject;
			if (Input.IsGamepadActive && CampaignOptionData.CheckIsPlayStation() && GameTexts.TryGetText("str_campaign_options_type", out textObject, optionIdentifier + "_ps"))
			{
				return textObject;
			}
			return GameTexts.FindText("str_campaign_options_type", optionIdentifier);
		}

		public static TextObject GetDescriptionOfOption(string optionIdentifier)
		{
			TextObject textObject;
			if (Input.IsGamepadActive && CampaignOptionData.CheckIsPlayStation() && GameTexts.TryGetText("str_campaign_options_description", out textObject, optionIdentifier + "_ps"))
			{
				return textObject;
			}
			return GameTexts.FindText("str_campaign_options_description", optionIdentifier);
		}

		private static bool CheckIsPlayStation()
		{
			return Input.ControllerType == Input.ControllerTypes.PlayStationDualSense || Input.ControllerType == Input.ControllerTypes.PlayStationDualShock;
		}

		public int GetPriorityIndex()
		{
			return this._priorityIndex;
		}

		public abstract CampaignOptionDataType GetDataType();

		public bool IsRelatedToDifficultyPreset()
		{
			return this._isRelatedToDifficultyPreset;
		}

		public float GetValueFromDifficultyPreset(CampaignOptionsDifficultyPresets preset)
		{
			if (this._onGetValueFromDifficultyPreset != null)
			{
				return this._onGetValueFromDifficultyPreset(preset);
			}
			switch (preset)
			{
			case CampaignOptionsDifficultyPresets.Freebooter:
				return 0f;
			case CampaignOptionsDifficultyPresets.Warrior:
				return 1f;
			case CampaignOptionsDifficultyPresets.Bannerlord:
				return 2f;
			default:
				return 0f;
			}
		}

		public CampaignOptionDisableStatus GetIsDisabledWithReason()
		{
			Func<CampaignOptionDisableStatus> getIsDisabledWithReason = this._getIsDisabledWithReason;
			CampaignOptionDisableStatus? campaignOptionDisableStatus = ((getIsDisabledWithReason != null) ? new CampaignOptionDisableStatus?(getIsDisabledWithReason()) : null);
			bool flag = false;
			string text = string.Empty;
			float num = -1f;
			if (this._enableState == CampaignOptionEnableState.Disabled)
			{
				flag = true;
				text = GameTexts.FindText("str_campaign_options_disabled_reason", this._identifier).ToString();
			}
			else if (this._enableState == CampaignOptionEnableState.DisabledLater)
			{
				text = GameTexts.FindText("str_campaign_options_persistency_warning", null).ToString();
			}
			if (campaignOptionDisableStatus != null && campaignOptionDisableStatus.Value.IsDisabled)
			{
				flag = true;
				if (!string.IsNullOrEmpty(campaignOptionDisableStatus.Value.DisabledReason))
				{
					if (!string.IsNullOrEmpty(text))
					{
						TextObject textObject = GameTexts.FindText("str_string_newline_string", null).CopyTextObject();
						textObject.SetTextVariable("STR1", text);
						textObject.SetTextVariable("STR2", campaignOptionDisableStatus.Value.DisabledReason);
						text = textObject.ToString();
					}
					else
					{
						text = campaignOptionDisableStatus.Value.DisabledReason;
					}
				}
				num = campaignOptionDisableStatus.Value.ValueIfDisabled;
			}
			return new CampaignOptionDisableStatus(flag, text, num);
		}

		public string GetIdentifier()
		{
			return this._identifier;
		}

		public CampaignOptionEnableState GetEnableState()
		{
			return this._enableState;
		}

		public string GetName()
		{
			return this._name.ToString();
		}

		public string GetDescription()
		{
			return this._description.ToString();
		}

		public float GetValue()
		{
			Func<float> getValue = this._getValue;
			if (getValue == null)
			{
				return 0f;
			}
			return getValue();
		}

		public void SetValue(float value)
		{
			Action<float> setValue = this._setValue;
			if (setValue == null)
			{
				return;
			}
			setValue(value);
		}

		private int _priorityIndex;

		private string _identifier;

		private bool _isRelatedToDifficultyPreset;

		private CampaignOptionEnableState _enableState;

		private TextObject _name;

		private TextObject _description;

		private Func<CampaignOptionDisableStatus> _getIsDisabledWithReason;

		protected Func<float> _getValue;

		protected Action<float> _setValue;

		protected Func<float, CampaignOptionsDifficultyPresets> _onGetDifficultyPresetFromValue;

		protected Func<CampaignOptionsDifficultyPresets, float> _onGetValueFromDifficultyPreset;
	}
}
