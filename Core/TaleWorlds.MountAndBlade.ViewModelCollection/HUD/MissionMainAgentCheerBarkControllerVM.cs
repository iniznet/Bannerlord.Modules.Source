using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionMainAgentCheerBarkControllerVM : ViewModel
	{
		public MissionMainAgentCheerBarkControllerVM(Action<int> onSelectCheer, Action<int> onSelectBark)
		{
			this._onSelectCheer = onSelectCheer;
			this._onSelectBark = onSelectBark;
			this.Nodes = new MBBindingList<CheerBarkNodeItemVM>();
			if (GameNetwork.IsMultiplayer)
			{
				this._ownedTauntCosmetics = NetworkMain.GameClient.OwnedCosmetics.ToList<string>();
				this.UpdatePlayerTauntIndices();
			}
			CheerBarkNodeItemVM.OnSelection += this.OnNodeFocused;
			CheerBarkNodeItemVM.OnNodeFocused += this.OnNodeTooltipToggled;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Nodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM n)
			{
				n.OnFinalize();
			});
			CheerBarkNodeItemVM.OnSelection -= this.OnNodeFocused;
			CheerBarkNodeItemVM.OnNodeFocused -= this.OnNodeTooltipToggled;
		}

		private void PopulateList()
		{
			bool isClient = GameNetwork.IsClient;
			this.IsNodesCategories = isClient;
			this.Nodes.Clear();
			GameKeyContext category = HotKeyManager.GetCategory("CombatHotKeyCategory");
			HotKey hotKey = category.GetHotKey("CheerBarkCloseMenu");
			SkinVoiceManager.SkinVoiceType[] mpBarks = SkinVoiceManager.VoiceType.MpBarks;
			if (isClient)
			{
				HotKey hotKey2 = category.GetHotKey("CheerBarkSelectFirstCategory");
				CheerBarkNodeItemVM cheerBarkNodeItemVM = new CheerBarkNodeItemVM(new TextObject("{=*}Taunt", null), "cheer", hotKey2, false, TauntUsageManager.TauntUsage.TauntUsageFlag.None);
				this.Nodes.Add(cheerBarkNodeItemVM);
				TauntCosmeticElement[] array = new TauntCosmeticElement[TauntCosmeticElement.MaxNumberOfTaunts];
				foreach (ValueTuple<string, int> valueTuple in this._playerTauntsWithIndices)
				{
					string item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					TauntCosmeticElement tauntCosmeticElement = CosmeticsManager.GetCosmeticElement(item) as TauntCosmeticElement;
					if (!tauntCosmeticElement.IsFree && !this._ownedTauntCosmetics.Contains(item))
					{
						Debug.FailedAssert("Taunt list have invalid taunt: " + item, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\HUD\\MissionMainAgentCheerBarkControllerVM.cs", "PopulateList", 82);
					}
					else if (item2 >= 0 && item2 < TauntCosmeticElement.MaxNumberOfTaunts)
					{
						array[item2] = tauntCosmeticElement;
					}
				}
				for (int i = 0; i < array.Length; i++)
				{
					TauntCosmeticElement tauntCosmeticElement2 = array[i];
					if (tauntCosmeticElement2 != null)
					{
						int indexOfAction = TauntUsageManager.GetIndexOfAction(tauntCosmeticElement2.Id);
						TauntUsageManager.TauntUsage.TauntUsageFlag actionNotUsableReason = CosmeticsManagerHelper.GetActionNotUsableReason(Agent.Main, indexOfAction);
						cheerBarkNodeItemVM.AddSubNode(new CheerBarkNodeItemVM(tauntCosmeticElement2.Id, new TextObject("{=!}" + tauntCosmeticElement2.Name, null), tauntCosmeticElement2.Id, this.GetCheerShortcut(i), true, actionNotUsableReason));
					}
					else
					{
						cheerBarkNodeItemVM.AddSubNode(new CheerBarkNodeItemVM(string.Empty, TextObject.Empty, string.Empty, null, true, TauntUsageManager.TauntUsage.TauntUsageFlag.None));
					}
				}
				HotKey hotKey3 = category.GetHotKey("CheerBarkSelectSecondCategory");
				CheerBarkNodeItemVM cheerBarkNodeItemVM2 = new CheerBarkNodeItemVM(new TextObject("{=5Xoilj6r}Shout", null), "bark", hotKey3, false, TauntUsageManager.TauntUsage.TauntUsageFlag.None);
				this.Nodes.Add(cheerBarkNodeItemVM2);
				cheerBarkNodeItemVM2.AddSubNode(new CheerBarkNodeItemVM(new TextObject("{=koX9okuG}None", null), "none", hotKey, true, TauntUsageManager.TauntUsage.TauntUsageFlag.None));
				for (int j = 0; j < mpBarks.Length; j++)
				{
					cheerBarkNodeItemVM2.AddSubNode(new CheerBarkNodeItemVM(mpBarks[j].GetName(), "bark" + j, this.GetCheerShortcut(j), true, TauntUsageManager.TauntUsage.TauntUsageFlag.None));
				}
			}
			else
			{
				ActionIndexCache[] array2 = Agent.DefaultTauntActions.ToArray<ActionIndexCache>();
				this.Nodes.Add(new CheerBarkNodeItemVM(new TextObject("{=koX9okuG}None", null), "none", hotKey, true, TauntUsageManager.TauntUsage.TauntUsageFlag.None));
				for (int k = 0; k < array2.Length; k++)
				{
					this.Nodes.Add(new CheerBarkNodeItemVM(new TextObject("{=!}" + (k + 1).ToString(), null), array2[k].Name, this.GetCheerShortcut(k), true, TauntUsageManager.TauntUsage.TauntUsageFlag.None));
				}
			}
			this.DisabledReasonText = string.Empty;
		}

		private void UpdatePlayerTauntIndices()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (((gameClient != null) ? gameClient.PlayerData : null) != null)
			{
				string text = NetworkMain.GameClient.PlayerData.UserId.ToString();
				this._playerTauntsWithIndices = TauntCosmeticElement.GetTauntIndicesForPlayer(text);
			}
			if (this._playerTauntsWithIndices == null)
			{
				this._playerTauntsWithIndices = new List<ValueTuple<string, int>>();
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
							if (flag)
							{
								Action<int> onSelectBark = this._onSelectBark;
								if (onSelectBark != null)
								{
									onSelectBark(cheerBarkNodeItemVM.SubNodes.IndexOf(cheerBarkNodeItemVM3) - 1);
								}
							}
							else
							{
								int indexOfAction = TauntUsageManager.GetIndexOfAction(cheerBarkNodeItemVM3.TypeAsString);
								Action<int> onSelectCheer = this._onSelectCheer;
								if (onSelectCheer != null)
								{
									onSelectCheer(indexOfAction);
								}
							}
						}
					}
					else if (cheerBarkNodeItemVM.TypeAsString != "none")
					{
						int num = TauntUsageManager.GetIndexOfAction(cheerBarkNodeItemVM.TypeAsString);
						if (num == -1)
						{
							ActionIndexCache[] defaultTauntActions = Agent.DefaultTauntActions;
							for (int i = 0; i < defaultTauntActions.Length; i++)
							{
								string name = defaultTauntActions[i].Name;
								if (cheerBarkNodeItemVM.TypeAsString == name)
								{
									num = i;
									break;
								}
							}
						}
						Action<int> onSelectCheer2 = this._onSelectCheer;
						if (onSelectCheer2 != null)
						{
							onSelectCheer2(num);
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

		public void OnNodeTooltipToggled(CheerBarkNodeItemVM node)
		{
			if (node != null && node.TauntUsageDisabledReason != TauntUsageManager.TauntUsage.TauntUsageFlag.None)
			{
				this.DisabledReasonText = TauntUsageManager.GetActionDisabledReasonText(node.TauntUsageDisabledReason);
				return;
			}
			this.DisabledReasonText = string.Empty;
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
					if (this._isActive)
					{
						this.PopulateList();
					}
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
		public string DisabledReasonText
		{
			get
			{
				return this._disabledReasonText;
			}
			set
			{
				if (value != this._disabledReasonText)
				{
					this._disabledReasonText = value;
					base.OnPropertyChangedWithValue<string>(value, "DisabledReasonText");
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

		private List<string> _ownedTauntCosmetics;

		private List<ValueTuple<string, int>> _playerTauntsWithIndices;

		private bool _isActive;

		private bool _isNodesCategories;

		private string _disabledReasonText;

		private string _selectedNodeText;

		private string _selectText;

		private string _focusedCheerText;

		private MBBindingList<CheerBarkNodeItemVM> _nodes;
	}
}
