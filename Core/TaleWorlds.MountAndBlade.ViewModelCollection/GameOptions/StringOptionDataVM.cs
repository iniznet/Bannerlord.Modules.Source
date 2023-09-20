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

		public override void UpdateData(bool initialUpdate)
		{
			base.UpdateData(initialUpdate);
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
					int num = (int)this.Option.GetValue(!initialUpdate);
					if (list.Count > 0 && num == -1)
					{
						num = 0;
					}
					this.Selector.Refresh(list, num, action);
					goto IL_1B1;
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
			int num2 = (int)this.Option.GetValue(!initialUpdate);
			if (list2.Count > 0 && num2 == -1)
			{
				num2 = 0;
			}
			this.Selector.Refresh(list2, num2, action);
			IL_1B1:
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
