using System;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000DD RID: 221
	public class MissionMainAgentCheerBarkControllerVM : ViewModel
	{
		// Token: 0x06001442 RID: 5186 RVA: 0x00042161 File Offset: 0x00040361
		public MissionMainAgentCheerBarkControllerVM(Action<int> onSelectCheer, Action<int> onSelectBark, ActionIndexCache[] cheerActionArr, SkinVoiceManager.SkinVoiceType[] barkActionArr)
		{
			this._onSelectCheer = onSelectCheer;
			this._onSelectBark = onSelectBark;
			this._cheerActionArr = cheerActionArr;
			this._barkActionArr = barkActionArr;
			this.Nodes = new MBBindingList<CheerBarkNodeItemVM>();
			this.PopulateList();
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x00042198 File Offset: 0x00040398
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

		// Token: 0x06001444 RID: 5188 RVA: 0x000423F0 File Offset: 0x000405F0
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

		// Token: 0x06001445 RID: 5189 RVA: 0x00042454 File Offset: 0x00040654
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

		// Token: 0x06001446 RID: 5190 RVA: 0x000424E4 File Offset: 0x000406E4
		public void OnCancelHoldController()
		{
			this.IsActive = false;
			this.Nodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM c)
			{
				c.IsSelected = false;
			});
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x00042518 File Offset: 0x00040718
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

		// Token: 0x06001448 RID: 5192 RVA: 0x00042664 File Offset: 0x00040864
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

		// Token: 0x06001449 RID: 5193 RVA: 0x000426E6 File Offset: 0x000408E6
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Nodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM n)
			{
				n.OnFinalize();
			});
		}

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x0600144A RID: 5194 RVA: 0x00042718 File Offset: 0x00040918
		// (set) Token: 0x0600144B RID: 5195 RVA: 0x00042720 File Offset: 0x00040920
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

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x0600144C RID: 5196 RVA: 0x0004273E File Offset: 0x0004093E
		// (set) Token: 0x0600144D RID: 5197 RVA: 0x00042746 File Offset: 0x00040946
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

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x0600144E RID: 5198 RVA: 0x00042769 File Offset: 0x00040969
		// (set) Token: 0x0600144F RID: 5199 RVA: 0x00042771 File Offset: 0x00040971
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

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x06001450 RID: 5200 RVA: 0x00042794 File Offset: 0x00040994
		// (set) Token: 0x06001451 RID: 5201 RVA: 0x0004279C File Offset: 0x0004099C
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

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06001452 RID: 5202 RVA: 0x000427BF File Offset: 0x000409BF
		// (set) Token: 0x06001453 RID: 5203 RVA: 0x000427C7 File Offset: 0x000409C7
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

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06001454 RID: 5204 RVA: 0x000427E5 File Offset: 0x000409E5
		// (set) Token: 0x06001455 RID: 5205 RVA: 0x000427ED File Offset: 0x000409ED
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

		// Token: 0x040009B3 RID: 2483
		private const string CheerId = "cheer";

		// Token: 0x040009B4 RID: 2484
		private const string BarkId = "bark";

		// Token: 0x040009B5 RID: 2485
		private const string NoneId = "none";

		// Token: 0x040009B6 RID: 2486
		private readonly Action<int> _onSelectCheer;

		// Token: 0x040009B7 RID: 2487
		private readonly Action<int> _onSelectBark;

		// Token: 0x040009B8 RID: 2488
		private readonly ActionIndexCache[] _cheerActionArr;

		// Token: 0x040009B9 RID: 2489
		private readonly SkinVoiceManager.SkinVoiceType[] _barkActionArr;

		// Token: 0x040009BA RID: 2490
		private bool _isActive;

		// Token: 0x040009BB RID: 2491
		private bool _isNodesCategories;

		// Token: 0x040009BC RID: 2492
		private string _selectedNodeText;

		// Token: 0x040009BD RID: 2493
		private string _selectText;

		// Token: 0x040009BE RID: 2494
		private string _focusedCheerText;

		// Token: 0x040009BF RID: 2495
		private MBBindingList<CheerBarkNodeItemVM> _nodes;
	}
}
