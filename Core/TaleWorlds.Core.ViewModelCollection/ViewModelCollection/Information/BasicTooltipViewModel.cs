using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x02000013 RID: 19
	public class BasicTooltipViewModel : ViewModel
	{
		// Token: 0x060000DC RID: 220 RVA: 0x00003948 File Offset: 0x00001B48
		public BasicTooltipViewModel(Func<string> hintTextDelegate)
		{
			this._hintProperty = hintTextDelegate;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00003957 File Offset: 0x00001B57
		public BasicTooltipViewModel(Func<List<TooltipProperty>> tooltipPropertiesDelegate)
		{
			this._tooltipProperties = tooltipPropertiesDelegate;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00003966 File Offset: 0x00001B66
		public BasicTooltipViewModel(Action preBuiltTooltipCallback)
		{
			this._preBuiltTooltipCallback = preBuiltTooltipCallback;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00003975 File Offset: 0x00001B75
		public BasicTooltipViewModel()
		{
			this._hintProperty = null;
			this._tooltipProperties = null;
			this._preBuiltTooltipCallback = null;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00003992 File Offset: 0x00001B92
		public void SetToolipCallback(Func<List<TooltipProperty>> tooltipPropertiesDelegate)
		{
			this._tooltipProperties = tooltipPropertiesDelegate;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000399B File Offset: 0x00001B9B
		public void SetGenericTooltipCallback(Action preBuiltTooltipCallback)
		{
			this._preBuiltTooltipCallback = preBuiltTooltipCallback;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000039A4 File Offset: 0x00001BA4
		public void SetHintCallback(Func<string> hintProperty)
		{
			this._hintProperty = hintProperty;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000039B0 File Offset: 0x00001BB0
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

		// Token: 0x060000E4 RID: 228 RVA: 0x00003A47 File Offset: 0x00001C47
		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x04000054 RID: 84
		private Func<string> _hintProperty;

		// Token: 0x04000055 RID: 85
		private Func<List<TooltipProperty>> _tooltipProperties;

		// Token: 0x04000056 RID: 86
		private Action _preBuiltTooltipCallback;
	}
}
