using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class BasicTooltipViewModel : ViewModel
	{
		public BasicTooltipViewModel(Func<string> hintTextDelegate)
		{
			this._hintProperty = hintTextDelegate;
		}

		public BasicTooltipViewModel(Func<List<TooltipProperty>> tooltipPropertiesDelegate)
		{
			this._tooltipProperties = tooltipPropertiesDelegate;
		}

		public BasicTooltipViewModel(Action preBuiltTooltipCallback)
		{
			this._preBuiltTooltipCallback = preBuiltTooltipCallback;
		}

		public BasicTooltipViewModel()
		{
			this._hintProperty = null;
			this._tooltipProperties = null;
			this._preBuiltTooltipCallback = null;
		}

		public void SetToolipCallback(Func<List<TooltipProperty>> tooltipPropertiesDelegate)
		{
			this._tooltipProperties = tooltipPropertiesDelegate;
		}

		public void SetGenericTooltipCallback(Action preBuiltTooltipCallback)
		{
			this._preBuiltTooltipCallback = preBuiltTooltipCallback;
		}

		public void SetHintCallback(Func<string> hintProperty)
		{
			this._hintProperty = hintProperty;
		}

		public void ExecuteBeginHint()
		{
			if (this._hintProperty == null && this._tooltipProperties == null && this._preBuiltTooltipCallback == null)
			{
				return;
			}
			if (this._hintProperty != null)
			{
				Func<List<TooltipProperty>> tooltipProperties = this._tooltipProperties;
			}
			if (this._tooltipProperties != null)
			{
				InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { this._tooltipProperties() });
				return;
			}
			if (this._hintProperty != null)
			{
				string text = this._hintProperty();
				if (!string.IsNullOrEmpty(text))
				{
					MBInformationManager.ShowHint(text);
					return;
				}
			}
			else if (this._preBuiltTooltipCallback != null)
			{
				this._preBuiltTooltipCallback();
			}
		}

		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		private Func<string> _hintProperty;

		private Func<List<TooltipProperty>> _tooltipProperties;

		private Action _preBuiltTooltipCallback;
	}
}
