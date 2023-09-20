using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000F2 RID: 242
	public class ActionOptionDataVM : GenericOptionDataVM
	{
		// Token: 0x06001567 RID: 5479 RVA: 0x000457F8 File Offset: 0x000439F8
		public ActionOptionDataVM(Action onAction, OptionsVM optionsVM, IOptionData option, TextObject name, TextObject optionActionName, TextObject description)
			: base(optionsVM, option, name, description, OptionsVM.OptionsDataType.ActionOption)
		{
			this._onAction = onAction;
			this._optionActionName = optionActionName;
			this.RefreshValues();
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x0004581C File Offset: 0x00043A1C
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._optionActionName != null)
			{
				this.ActionName = this._optionActionName.ToString();
			}
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x0004583D File Offset: 0x00043A3D
		private void ExecuteAction()
		{
			Action onAction = this._onAction;
			if (onAction == null)
			{
				return;
			}
			onAction.DynamicInvokeWithLog(Array.Empty<object>());
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x00045855 File Offset: 0x00043A55
		public override void Cancel()
		{
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x00045857 File Offset: 0x00043A57
		public override bool IsChanged()
		{
			return false;
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x0004585A File Offset: 0x00043A5A
		public override void ResetData()
		{
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x0004585C File Offset: 0x00043A5C
		public override void SetValue(float value)
		{
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x0004585E File Offset: 0x00043A5E
		public override void UpdateValue()
		{
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x00045860 File Offset: 0x00043A60
		public override void ApplyValue()
		{
		}

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x06001570 RID: 5488 RVA: 0x00045862 File Offset: 0x00043A62
		// (set) Token: 0x06001571 RID: 5489 RVA: 0x0004586A File Offset: 0x00043A6A
		[DataSourceProperty]
		public string ActionName
		{
			get
			{
				return this._actionName;
			}
			set
			{
				if (value != this._actionName)
				{
					this._actionName = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionName");
				}
			}
		}

		// Token: 0x04000A3D RID: 2621
		private readonly Action _onAction;

		// Token: 0x04000A3E RID: 2622
		private readonly TextObject _optionActionName;

		// Token: 0x04000A3F RID: 2623
		private string _actionName;
	}
}
