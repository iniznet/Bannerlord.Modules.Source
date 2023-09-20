using System;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.HostGame.HostGameOptions
{
	public class MultipleSelectionHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		public MultipleSelectionHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(3, optionType, preferredIndex)
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

		public Action<MultipleSelectionHostGameOptionDataVM> OnChangedSelection;

		private SelectorVM<SelectorItemVM> _selector;
	}
}
