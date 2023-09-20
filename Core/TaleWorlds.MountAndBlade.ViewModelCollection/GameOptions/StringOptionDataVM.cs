using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public class StringOptionDataVM : GenericOptionDataVM
	{
		public StringOptionDataVM(OptionsVM optionsVM, ISelectionOptionData option, TextObject name, TextObject description)
			: base(optionsVM, option, name, description, OptionsVM.OptionsDataType.MultipleSelectionOption)
		{
			this.Selector = new SelectorVM<SelectorItemVM>(0, null);
			this._selectionOptionData = option;
			this.UpdateData(true);
			this._initialValue = (int)this.Option.GetValue(false);
			this.Selector.SelectedIndex = this._initialValue;
		}

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

		public void UpdateValue(SelectorVM<SelectorItemVM> selector)
		{
			if (selector.SelectedIndex >= 0)
			{
				this.Option.SetValue((float)selector.SelectedIndex);
				this.Option.Commit();
				this._optionsVM.SetConfig(this.Option, (float)selector.SelectedIndex);
			}
		}

		public override void UpdateValue()
		{
			if (this.Selector.SelectedIndex >= 0 && (float)this.Selector.SelectedIndex != this.Option.GetValue(false))
			{
				this.Option.Commit();
				this._optionsVM.SetConfig(this.Option, (float)this.Selector.SelectedIndex);
			}
		}

		public override void Cancel()
		{
			this.Selector.SelectedIndex = this._initialValue;
			this.UpdateValue();
		}

		public override void SetValue(float value)
		{
			this.Selector.SelectedIndex = (int)value;
		}

		public override void ResetData()
		{
			this.Selector.SelectedIndex = (int)this.Option.GetDefaultValue();
		}

		public override bool IsChanged()
		{
			return this._initialValue != this.Selector.SelectedIndex;
		}

		public override void ApplyValue()
		{
			if (this._initialValue != this.Selector.SelectedIndex)
			{
				this._initialValue = this.Selector.SelectedIndex;
			}
		}

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

		private int _initialValue;

		private ISelectionOptionData _selectionOptionData;

		public SelectorVM<SelectorItemVM> _selector;
	}
}
