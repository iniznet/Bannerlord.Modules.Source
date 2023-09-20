using System;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions
{
	// Token: 0x0200007A RID: 122
	public class MultipleSelectionHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		// Token: 0x06000ADD RID: 2781 RVA: 0x00026B74 File Offset: 0x00024D74
		public MultipleSelectionHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(OptionsVM.OptionsDataType.MultipleSelectionOption, optionType, preferredIndex)
		{
			List<string> multiplayerOptionsList = MultiplayerOptions.Instance.GetMultiplayerOptionsList(base.OptionType);
			List<string> multiplayerOptionsTextList = MultiplayerOptions.Instance.GetMultiplayerOptionsTextList(base.OptionType);
			List<string> list = new List<string>();
			foreach (string text in multiplayerOptionsTextList)
			{
				list.Add(text);
			}
			this.Selector = new SelectorVM<SelectorItemVM>(list, multiplayerOptionsList.IndexOf(MultiplayerOptions.Instance.GetValueTextForOptionWithMultipleSelection(base.OptionType)), null);
			this.Selector.SetOnChangeAction(new Action<SelectorVM<SelectorItemVM>>(this.OnChangeSelected));
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x00026C2C File Offset: 0x00024E2C
		public override void RefreshData()
		{
			this.Selector.SetOnChangeAction(null);
			List<string> multiplayerOptionsList = MultiplayerOptions.Instance.GetMultiplayerOptionsList(base.OptionType);
			List<string> multiplayerOptionsTextList = MultiplayerOptions.Instance.GetMultiplayerOptionsTextList(base.OptionType);
			List<string> list = new List<string>();
			foreach (string text in multiplayerOptionsTextList)
			{
				list.Add(text);
			}
			int num = multiplayerOptionsList.IndexOf(MultiplayerOptions.Instance.GetValueTextForOptionWithMultipleSelection(base.OptionType));
			if (num != this.Selector.SelectedIndex)
			{
				this.Selector.SelectedIndex = num;
			}
			this.Selector.SetOnChangeAction(new Action<SelectorVM<SelectorItemVM>>(this.OnChangeSelected));
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x00026CF8 File Offset: 0x00024EF8
		public void RefreshList()
		{
			List<string> multiplayerOptionsList = MultiplayerOptions.Instance.GetMultiplayerOptionsList(base.OptionType);
			List<string> multiplayerOptionsTextList = MultiplayerOptions.Instance.GetMultiplayerOptionsTextList(base.OptionType);
			List<string> list = new List<string>();
			foreach (string text in multiplayerOptionsTextList)
			{
				list.Add(text);
			}
			this.Selector.Refresh(list, multiplayerOptionsList.IndexOf(MultiplayerOptions.Instance.GetValueTextForOptionWithMultipleSelection(base.OptionType)), new Action<SelectorVM<SelectorItemVM>>(this.OnChangeSelected));
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x00026D9C File Offset: 0x00024F9C
		private void OnChangeSelected(SelectorVM<SelectorItemVM> selector)
		{
			if (selector.SelectedIndex < 0 || selector.SelectedIndex >= selector.ItemList.Count)
			{
				return;
			}
			string text = MultiplayerOptions.Instance.GetMultiplayerOptionsList(base.OptionType)[selector.SelectedIndex];
			MultiplayerOptions.Instance.SetValueForOptionWithMultipleSelectionFromText(base.OptionType, text);
			Action<MultipleSelectionHostGameOptionDataVM> onChangedSelection = this.OnChangedSelection;
			if (onChangedSelection == null)
			{
				return;
			}
			onChangedSelection(this);
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x00026E04 File Offset: 0x00025004
		// (set) Token: 0x06000AE2 RID: 2786 RVA: 0x00026E0C File Offset: 0x0002500C
		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> Selector
		{
			get
			{
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

		// Token: 0x04000543 RID: 1347
		public Action<MultipleSelectionHostGameOptionDataVM> OnChangedSelection;

		// Token: 0x04000544 RID: 1348
		private SelectorVM<SelectorItemVM> _selector;
	}
}
