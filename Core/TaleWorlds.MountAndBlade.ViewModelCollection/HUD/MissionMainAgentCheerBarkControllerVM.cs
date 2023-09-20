using System;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionMainAgentCheerBarkControllerVM : ViewModel
	{
		public MissionMainAgentCheerBarkControllerVM(Action<int> onSelectCheer, Action<int> onSelectBark, ActionIndexCache[] cheerActionArr, SkinVoiceManager.SkinVoiceType[] barkActionArr)
		{
			this._onSelectCheer = onSelectCheer;
			this._onSelectBark = onSelectBark;
			this._cheerActionArr = cheerActionArr;
			this._barkActionArr = barkActionArr;
			this.Nodes = new MBBindingList<CheerBarkNodeItemVM>();
			this.PopulateList();
		}

		private void PopulateList()
		{
			bool isClient = GameNetwork.IsClient;
			this.IsNodesCategories = isClient;
			this.Nodes.Clear();
			GameKeyContext category = HotKeyManager.GetCategory("CombatHotKeyCategory");
			HotKey hotKey = category.GetHotKey("CheerBarkCloseMenu");
			if (isClient)
			{
				HotKey hotKey2 = category.GetHotKey("CheerBarkSelectFirstCategory");
				CheerBarkNodeItemVM cheerBarkNodeItemVM = new CheerBarkNodeItemVM(new TextObject("{=PmbKYDON}Cheer", null), "cheer", hotKey2, null, false);
				this.Nodes.Add(cheerBarkNodeItemVM);
				cheerBarkNodeItemVM.AddSubNode(new CheerBarkNodeItemVM(new TextObject("{=koX9okuG}None", null), "none", hotKey, new Action<CheerBarkNodeItemVM>(this.OnNodeFocused), true));
				for (int i = 0; i < this._cheerActionArr.Length; i++)
				{
					cheerBarkNodeItemVM.AddSubNode(new CheerBarkNodeItemVM(new TextObject("{=!}" + (i + 1).ToString(), null), "cheer" + i, this.GetCheerShortcut(i), new Action<CheerBarkNodeItemVM>(this.OnNodeFocused), true));
				}
				HotKey hotKey3 = category.GetHotKey("CheerBarkSelectSecondCategory");
				CheerBarkNodeItemVM cheerBarkNodeItemVM2 = new CheerBarkNodeItemVM(new TextObject("{=5Xoilj6r}Shout", null), "bark", hotKey3, null, false);
				this.Nodes.Add(cheerBarkNodeItemVM2);
				cheerBarkNodeItemVM2.AddSubNode(new CheerBarkNodeItemVM(new TextObject("{=koX9okuG}None", null), "none", hotKey, new Action<CheerBarkNodeItemVM>(this.OnNodeFocused), true));
				for (int j = 0; j < this._barkActionArr.Length; j++)
				{
					cheerBarkNodeItemVM2.AddSubNode(new CheerBarkNodeItemVM(this._barkActionArr[j].GetName(), "bark" + j, this.GetCheerShortcut(j), new Action<CheerBarkNodeItemVM>(this.OnNodeFocused), true));
				}
				return;
			}
			this.Nodes.Add(new CheerBarkNodeItemVM(new TextObject("{=koX9okuG}None", null), "none", hotKey, new Action<CheerBarkNodeItemVM>(this.OnNodeFocused), true));
			for (int k = 0; k < this._cheerActionArr.Length; k++)
			{
				this.Nodes.Add(new CheerBarkNodeItemVM(new TextObject("{=!}" + (k + 1).ToString(), null), "cheer" + k, this.GetCheerShortcut(k), new Action<CheerBarkNodeItemVM>(this.OnNodeFocused), true));
			}
		}

		private HotKey GetCheerShortcut(int cheerIndex)
		{
			GameKeyContext category = HotKeyManager.GetCategory("CombatHotKeyCategory");
			switch (cheerIndex)
			{
			case 0:
				return category.GetHotKey("CheerBarkItem1");
			case 1:
				return category.GetHotKey("CheerBarkItem2");
			case 2:
				return category.GetHotKey("CheerBarkItem3");
			case 3:
				return category.GetHotKey("CheerBarkItem4");
			default:
				return null;
			}
		}

		public void SelectItem(int itemIndex, int subNodeIndex = -1)
		{
			if (subNodeIndex == -1)
			{
				for (int i = 0; i < this.Nodes.Count; i++)
				{
					this.Nodes[i].IsSelected = itemIndex == i;
				}
				return;
			}
			if (itemIndex >= 0 && itemIndex < this.Nodes.Count)
			{
				for (int j = 0; j < this.Nodes[itemIndex].SubNodes.Count; j++)
				{
					this.Nodes[itemIndex].SubNodes[j].IsSelected = subNodeIndex == j;
				}
			}
		}

		public void OnCancelHoldController()
		{
			this.IsActive = false;
			this.Nodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM c)
			{
				c.IsSelected = false;
			});
		}

		public void OnSelectControllerToggle(bool isActive)
		{
			this.FocusedCheerText = "";
			this.SelectedNodeText = "";
			if (!isActive)
			{
				CheerBarkNodeItemVM cheerBarkNodeItemVM = this.Nodes.FirstOrDefault((CheerBarkNodeItemVM c) => c.IsSelected);
				if (cheerBarkNodeItemVM != null)
				{
					if (this.IsNodesCategories)
					{
						bool flag = cheerBarkNodeItemVM.TypeAsString == "bark";
						CheerBarkNodeItemVM cheerBarkNodeItemVM2;
						if (cheerBarkNodeItemVM == null)
						{
							cheerBarkNodeItemVM2 = null;
						}
						else
						{
							cheerBarkNodeItemVM2 = cheerBarkNodeItemVM.SubNodes.FirstOrDefault((CheerBarkNodeItemVM c) => c.IsSelected);
						}
						CheerBarkNodeItemVM cheerBarkNodeItemVM3 = cheerBarkNodeItemVM2;
						if (cheerBarkNodeItemVM3 != null && cheerBarkNodeItemVM3.TypeAsString != "none")
						{
							Action<int> action = (flag ? this._onSelectBark : this._onSelectCheer);
							if (action != null)
							{
								action(cheerBarkNodeItemVM.SubNodes.IndexOf(cheerBarkNodeItemVM3) - 1);
							}
						}
					}
					else if (cheerBarkNodeItemVM.TypeAsString != "none")
					{
						Action<int> onSelectCheer = this._onSelectCheer;
						if (onSelectCheer != null)
						{
							onSelectCheer(this.Nodes.IndexOf(cheerBarkNodeItemVM) - 1);
						}
					}
				}
				this.Nodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM n)
				{
					n.IsSelected = false;
				});
			}
			this.IsActive = isActive;
		}

		public void OnNodeFocused(CheerBarkNodeItemVM focusedNode)
		{
			string text = ((focusedNode != null) ? focusedNode.CheerNameText : null) ?? string.Empty;
			if (this.IsNodesCategories)
			{
				bool flag = focusedNode != null && focusedNode.TypeAsString.Contains("bark");
				string typeId = (flag ? "bark" : "cheer");
				this.Nodes.First((CheerBarkNodeItemVM c) => c.TypeAsString == typeId).SelectedNodeText = text;
				return;
			}
			this.SelectedNodeText = text;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Nodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM n)
			{
				n.OnFinalize();
			});
		}

		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		[DataSourceProperty]
		public string SelectText
		{
			get
			{
				return this._selectText;
			}
			set
			{
				if (value != this._selectText)
				{
					this._selectText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectText");
				}
			}
		}

		[DataSourceProperty]
		public string FocusedCheerText
		{
			get
			{
				return this._focusedCheerText;
			}
			set
			{
				if (value != this._focusedCheerText)
				{
					this._focusedCheerText = value;
					base.OnPropertyChangedWithValue<string>(value, "FocusedCheerText");
				}
			}
		}

		[DataSourceProperty]
		public string SelectedNodeText
		{
			get
			{
				return this._selectedNodeText;
			}
			set
			{
				if (value != this._selectedNodeText)
				{
					this._selectedNodeText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectedNodeText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNodesCategories
		{
			get
			{
				return this._isNodesCategories;
			}
			set
			{
				if (value != this._isNodesCategories)
				{
					this._isNodesCategories = value;
					base.OnPropertyChangedWithValue(value, "IsNodesCategories");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CheerBarkNodeItemVM> Nodes
		{
			get
			{
				return this._nodes;
			}
			set
			{
				if (value != this._nodes)
				{
					this._nodes = value;
					base.OnPropertyChangedWithValue<MBBindingList<CheerBarkNodeItemVM>>(value, "Nodes");
				}
			}
		}

		private const string CheerId = "cheer";

		private const string BarkId = "bark";

		private const string NoneId = "none";

		private readonly Action<int> _onSelectCheer;

		private readonly Action<int> _onSelectBark;

		private readonly ActionIndexCache[] _cheerActionArr;

		private readonly SkinVoiceManager.SkinVoiceType[] _barkActionArr;

		private bool _isActive;

		private bool _isNodesCategories;

		private string _selectedNodeText;

		private string _selectText;

		private string _focusedCheerText;

		private MBBindingList<CheerBarkNodeItemVM> _nodes;
	}
}
