using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000FD RID: 253
	public class StringOptionDataVM : GenericOptionDataVM
	{
		// Token: 0x06001679 RID: 5753 RVA: 0x00048928 File Offset: 0x00046B28
		public StringOptionDataVM(OptionsVM optionsVM, ISelectionOptionData option, TextObject name, TextObject description)
			: base(optionsVM, option, name, description, OptionsVM.OptionsDataType.MultipleSelectionOption)
		{
			this.Selector = new SelectorVM<SelectorItemVM>(0, null);
			this._selectionOptionData = option;
			this.UpdateData(true);
			this._initialValue = (int)this.Option.GetValue(false);
			this.Selector.SelectedIndex = this._initialValue;
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x00048980 File Offset: 0x00046B80
		public override void UpdateData(bool initalUpdate)
		{
			base.UpdateData(initalUpdate);
			IEnumerable<SelectionData> selectableOptionNames = this._selectionOptionData.GetSelectableOptionNames();
			this.Selector.SetOnChangeAction(null);
			bool flag = (int)this.Option.GetValue(true) != this.Selector.SelectedIndex;
			Action<SelectorVM<SelectorItemVM>> action = null;
			if (flag)
			{
				action = new Action<SelectorVM<SelectorItemVM>>(this.UpdateValue);
			}
			if (selectableOptionNames.Any<SelectionData>())
			{
				if (selectableOptionNames.All((SelectionData n) => n.IsLocalizationId))
				{
					List<TextObject> list = new List<TextObject>();
					foreach (SelectionData selectionData in selectableOptionNames)
					{
						TextObject textObject = Module.CurrentModule.GlobalTextManager.FindText(selectionData.Data, null);
						list.Add(textObject);
					}
					this.Selector.Refresh(list, (int)this.Option.GetValue(!initalUpdate), action);
					goto IL_183;
				}
			}
			List<string> list2 = new List<string>();
			foreach (SelectionData selectionData2 in selectableOptionNames)
			{
				if (selectionData2.IsLocalizationId)
				{
					TextObject textObject2 = Module.CurrentModule.GlobalTextManager.FindText(selectionData2.Data, null);
					list2.Add(textObject2.ToString());
				}
				else
				{
					list2.Add(selectionData2.Data);
				}
			}
			this.Selector.Refresh(list2, (int)this.Option.GetValue(!initalUpdate), action);
			IL_183:
			if (!flag)
			{
				this.Selector.SetOnChangeAction(new Action<SelectorVM<SelectorItemVM>>(this.UpdateValue));
			}
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x00048B48 File Offset: 0x00046D48
		public override void RefreshValues()
		{
			base.RefreshValues();
			SelectorVM<SelectorItemVM> selector = this.Selector;
			if (selector == null)
			{
				return;
			}
			selector.RefreshValues();
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x00048B60 File Offset: 0x00046D60
		public void UpdateValue(SelectorVM<SelectorItemVM> selector)
		{
			if (selector.SelectedIndex >= 0)
			{
				this.Option.SetValue((float)selector.SelectedIndex);
				this.Option.Commit();
				this._optionsVM.SetConfig(this.Option, (float)selector.SelectedIndex);
			}
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x00048BA0 File Offset: 0x00046DA0
		public override void UpdateValue()
		{
			if (this.Selector.SelectedIndex >= 0 && (float)this.Selector.SelectedIndex != this.Option.GetValue(false))
			{
				this.Option.Commit();
				this._optionsVM.SetConfig(this.Option, (float)this.Selector.SelectedIndex);
			}
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x00048BFD File Offset: 0x00046DFD
		public override void Cancel()
		{
			this.Selector.SelectedIndex = this._initialValue;
			this.UpdateValue();
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x00048C16 File Offset: 0x00046E16
		public override void SetValue(float value)
		{
			this.Selector.SelectedIndex = (int)value;
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x00048C25 File Offset: 0x00046E25
		public override void ResetData()
		{
			this.Selector.SelectedIndex = (int)this.Option.GetDefaultValue();
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00048C3E File Offset: 0x00046E3E
		public override bool IsChanged()
		{
			return this._initialValue != this.Selector.SelectedIndex;
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x00048C56 File Offset: 0x00046E56
		public override void ApplyValue()
		{
			if (this._initialValue != this.Selector.SelectedIndex)
			{
				this._initialValue = this.Selector.SelectedIndex;
			}
		}

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06001683 RID: 5763 RVA: 0x00048C7C File Offset: 0x00046E7C
		// (set) Token: 0x06001684 RID: 5764 RVA: 0x00048C8B File Offset: 0x00046E8B
		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> Selector
		{
			get
			{
				SelectorVM<SelectorItemVM> selector = this._selector;
				return this._selector;
			}
			set
			{
				if (value != this._selector)
				{
					this._selector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "Selector");
				}
			}
		}

		// Token: 0x04000AB0 RID: 2736
		private int _initialValue;

		// Token: 0x04000AB1 RID: 2737
		private ISelectionOptionData _selectionOptionData;

		// Token: 0x04000AB2 RID: 2738
		public SelectorVM<SelectorItemVM> _selector;
	}
}
