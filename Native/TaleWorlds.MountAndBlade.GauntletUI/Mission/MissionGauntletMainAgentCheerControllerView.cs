using System;
using System.ComponentModel;
using NetworkMessages.FromClient;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	[OverrideView(typeof(MissionMainAgentCheerBarkControllerView))]
	public class MissionGauntletMainAgentCheerControllerView : MissionView
	{
		private bool IsDisplayingADialog
		{
			get
			{
				IMissionScreen missionScreenAsInterface = this._missionScreenAsInterface;
				return (missionScreenAsInterface != null && missionScreenAsInterface.GetDisplayDialog()) || base.MissionScreen.IsRadialMenuActive || base.Mission.IsOrderMenuOpen;
			}
		}

		private bool HoldHandled
		{
			get
			{
				return this._holdHandled;
			}
			set
			{
				this._holdHandled = value;
				MissionScreen missionScreen = base.MissionScreen;
				if (missionScreen == null)
				{
					return;
				}
				missionScreen.SetRadialMenuActiveState(value);
			}
		}

		public MissionGauntletMainAgentCheerControllerView()
		{
			this._missionScreenAsInterface = base.MissionScreen;
			this.HoldHandled = false;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._gauntletLayer = new GauntletLayer(2, "GauntletLayer", false);
			this._missionMainAgentController = base.Mission.GetMissionBehavior<MissionMainAgentController>();
			this._dataSource = new MissionMainAgentCheerBarkControllerVM(new Action<int>(this.OnCheerSelect), new Action<int>(this.OnBarkSelect));
			this._gauntletLayer.LoadMovie("MainAgentCheerBarkController", this._dataSource);
			GameKeyContext category = HotKeyManager.GetCategory("CombatHotKeyCategory");
			if (this._missionMainAgentController != null)
			{
				InputContext inputContext = this._missionMainAgentController.Input as InputContext;
				if (inputContext != null && !inputContext.IsCategoryRegistered(category))
				{
					inputContext.RegisterHotKeyCategory(category);
				}
			}
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.Mission.OnMainAgentChanged += this.OnMainAgentChanged;
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.Mission.OnMainAgentChanged -= this.OnMainAgentChanged;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._missionMainAgentController = null;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this.IsMainAgentAvailable() && base.Mission.Mode != 6 && (!base.MissionScreen.IsRadialMenuActive || this._dataSource.IsActive))
			{
				this.TickControls(dt);
			}
		}

		private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (base.Mission.MainAgent == null)
			{
				if (this.HoldHandled)
				{
					this.HoldHandled = false;
				}
				this._holdTime = 0f;
				this._dataSource.OnCancelHoldController();
			}
		}

		private void HandleNodeSelectionInput(CheerBarkNodeItemVM node, int nodeIndex, int parentNodeIndex = -1)
		{
			if (this._missionMainAgentController == null)
			{
				return;
			}
			IInputContext input = this._missionMainAgentController.Input;
			if (node.ShortcutKey != null)
			{
				if (input.IsHotKeyPressed(node.ShortcutKey.HotKey.Id))
				{
					if (parentNodeIndex != -1)
					{
						this._dataSource.SelectItem(parentNodeIndex, nodeIndex);
						this._isReturningToCategories = nodeIndex == 0;
						return;
					}
					this._dataSource.SelectItem(nodeIndex, -1);
					this._isSelectingFromInput = node.HasSubNodes;
					return;
				}
				else if (input.IsHotKeyReleased(node.ShortcutKey.HotKey.Id))
				{
					if (!this._isSelectingFromInput)
					{
						if (this._isReturningToCategories && nodeIndex >= 0 && nodeIndex < this._dataSource.Nodes.Count)
						{
							this._dataSource.Nodes[nodeIndex].SubNodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM x)
							{
								x.IsSelected = false;
							});
							this._dataSource.Nodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM x)
							{
								x.IsSelected = false;
							});
						}
						else
						{
							this.HandleClosingHoldCheer();
						}
					}
					this._isSelectingFromInput = false;
				}
			}
		}

		private void TickControls(float dt)
		{
			if (this._missionMainAgentController == null)
			{
				return;
			}
			IInputContext input = this._missionMainAgentController.Input;
			if (GameNetwork.IsMultiplayer && this._cooldownTimeRemaining > 0f)
			{
				this._cooldownTimeRemaining -= dt;
				if (input.IsGameKeyDown(31))
				{
					if (!this._prevCheerKeyDown && (double)this._cooldownTimeRemaining >= 0.1)
					{
						this._cooldownInfoText.SetTextVariable("SECONDS", this._cooldownTimeRemaining.ToString("0.0"));
						InformationManager.DisplayMessage(new InformationMessage(this._cooldownInfoText.ToString()));
					}
					this._prevCheerKeyDown = true;
					return;
				}
				this._prevCheerKeyDown = false;
				return;
			}
			else
			{
				if (this.HoldHandled)
				{
					int num = -1;
					for (int i = 0; i < this._dataSource.Nodes.Count; i++)
					{
						if (this._dataSource.Nodes[i].IsSelected)
						{
							num = i;
							break;
						}
					}
					if (this._dataSource.IsNodesCategories)
					{
						if (num != -1)
						{
							for (int j = 0; j < this._dataSource.Nodes[num].SubNodes.Count; j++)
							{
								this.HandleNodeSelectionInput(this._dataSource.Nodes[num].SubNodes[j], j, num);
							}
						}
						else if (input.IsHotKeyReleased("CheerBarkSelectFirstCategory"))
						{
							this._dataSource.SelectItem(0, -1);
						}
						else if (input.IsHotKeyReleased("CheerBarkSelectSecondCategory"))
						{
							this._dataSource.SelectItem(1, -1);
						}
					}
					else
					{
						for (int k = 0; k < this._dataSource.Nodes.Count; k++)
						{
							this.HandleNodeSelectionInput(this._dataSource.Nodes[k], k, -1);
						}
					}
				}
				if (input.IsGameKeyDown(31) && !this.IsDisplayingADialog && !base.MissionScreen.IsRadialMenuActive)
				{
					if (this._holdTime > 0f && !this.HoldHandled)
					{
						this.HandleOpenHold();
						this.HoldHandled = true;
					}
					this._holdTime += dt;
					this._prevCheerKeyDown = true;
					return;
				}
				if (this._prevCheerKeyDown && !input.IsGameKeyDown(31))
				{
					if (this._holdTime < 0f)
					{
						this.HandleQuickReleaseCheer();
					}
					else
					{
						this.HandleClosingHoldCheer();
					}
					this.HoldHandled = false;
					this._holdTime = 0f;
					this._prevCheerKeyDown = false;
				}
				return;
			}
		}

		private void HandleOpenHold()
		{
			MissionMainAgentCheerBarkControllerVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnSelectControllerToggle(true);
			}
			base.MissionScreen.SetRadialMenuActiveState(true);
		}

		private void HandleClosingHoldCheer()
		{
			MissionMainAgentCheerBarkControllerVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnSelectControllerToggle(false);
			}
			base.MissionScreen.SetRadialMenuActiveState(false);
		}

		private void HandleQuickReleaseCheer()
		{
			this.OnCheerSelect(-1);
		}

		private void OnCheerSelect(int tauntIndex)
		{
			if (tauntIndex < 0)
			{
				return;
			}
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new TauntSelected(tauntIndex));
				GameNetwork.EndModuleEventAsClient();
			}
			else
			{
				Agent main = Agent.Main;
				if (main != null)
				{
					main.HandleTaunt(tauntIndex, true);
				}
			}
			this._cooldownTimeRemaining = 4f;
		}

		private void OnBarkSelect(int indexOfBark)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new BarkSelected(indexOfBark));
				GameNetwork.EndModuleEventAsClient();
			}
			else
			{
				Agent main = Agent.Main;
				if (main != null)
				{
					main.HandleBark(indexOfBark);
				}
			}
			this._cooldownTimeRemaining = 2f;
		}

		private bool IsMainAgentAvailable()
		{
			Agent main = Agent.Main;
			return main != null && main.IsActive();
		}

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		private const float CooldownPeriodDurationAfterCheer = 4f;

		private const float CooldownPeriodDurationAfterBark = 2f;

		private const float _minHoldTime = 0f;

		private readonly IMissionScreen _missionScreenAsInterface;

		private MissionMainAgentController _missionMainAgentController;

		private readonly TextObject _cooldownInfoText = new TextObject("{=aogZyZlR}You need to wait {SECONDS} seconds until you can cheer/shout again.", null);

		private bool _holdHandled;

		private float _holdTime;

		private bool _prevCheerKeyDown;

		private GauntletLayer _gauntletLayer;

		private MissionMainAgentCheerBarkControllerVM _dataSource;

		private float _cooldownTimeRemaining;

		private bool _isSelectingFromInput;

		private bool _isReturningToCategories;
	}
}
