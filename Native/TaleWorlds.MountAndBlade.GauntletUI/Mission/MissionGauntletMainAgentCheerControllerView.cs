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
	// Token: 0x02000029 RID: 41
	[OverrideView(typeof(MissionMainAgentCheerBarkControllerView))]
	public class MissionGauntletMainAgentCheerControllerView : MissionView
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000A0A9 File Offset: 0x000082A9
		private bool IsDisplayingADialog
		{
			get
			{
				IMissionScreen missionScreenAsInterface = this._missionScreenAsInterface;
				return (missionScreenAsInterface != null && missionScreenAsInterface.GetDisplayDialog()) || base.MissionScreen.IsRadialMenuActive || base.Mission.IsOrderMenuOpen;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x0000A0D9 File Offset: 0x000082D9
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x0000A0E1 File Offset: 0x000082E1
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

		// Token: 0x060001D9 RID: 473 RVA: 0x0000A0FB File Offset: 0x000082FB
		public MissionGauntletMainAgentCheerControllerView()
		{
			this._missionScreenAsInterface = base.MissionScreen;
			this.HoldHandled = false;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000A128 File Offset: 0x00008328
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._gauntletLayer = new GauntletLayer(2, "GauntletLayer", false);
			this._missionMainAgentController = base.Mission.GetMissionBehavior<MissionMainAgentController>();
			this._dataSource = new MissionMainAgentCheerBarkControllerVM(new Action<int>(this.OnCheerSelect), new Action<int>(this.OnBarkSelect), Agent.TauntCheerActions, SkinVoiceManager.VoiceType.MpBarks);
			this._gauntletLayer.LoadMovie("MainAgentCheerBarkController", this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("CombatHotKeyCategory"));
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.Mission.OnMainAgentChanged += this.OnMainAgentChanged;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000A1E4 File Offset: 0x000083E4
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

		// Token: 0x060001DC RID: 476 RVA: 0x0000A23F File Offset: 0x0000843F
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this.IsMainAgentAvailable() && base.Mission.Mode != 6 && (!base.MissionScreen.IsRadialMenuActive || this._dataSource.IsActive))
			{
				this.TickControls(dt);
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000A27F File Offset: 0x0000847F
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

		// Token: 0x060001DE RID: 478 RVA: 0x0000A2B4 File Offset: 0x000084B4
		private void HandleNodeSelectionInput(CheerBarkNodeItemVM node, int nodeIndex, int parentNodeIndex = -1)
		{
			if (node.ShortcutKey != null)
			{
				if (base.MissionScreen.SceneLayer.Input.IsHotKeyPressed(node.ShortcutKey.HotKey.Id))
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
				else if (base.MissionScreen.SceneLayer.Input.IsHotKeyReleased(node.ShortcutKey.HotKey.Id))
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

		// Token: 0x060001DF RID: 479 RVA: 0x0000A3F8 File Offset: 0x000085F8
		private void TickControls(float dt)
		{
			if (GameNetwork.IsMultiplayer && this._cooldownTimeRemaining > 0f)
			{
				this._cooldownTimeRemaining -= dt;
				if (base.MissionScreen.SceneLayer.Input.IsGameKeyDown(31))
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
						else if (base.MissionScreen.SceneLayer.Input.IsHotKeyReleased("CheerBarkSelectFirstCategory"))
						{
							this._dataSource.SelectItem(0, -1);
						}
						else if (base.MissionScreen.SceneLayer.Input.IsHotKeyReleased("CheerBarkSelectSecondCategory"))
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
				if (base.MissionScreen.SceneLayer.Input.IsGameKeyDown(31) && !this.IsDisplayingADialog && this.IsMainAgentAvailable() && !base.MissionScreen.IsRadialMenuActive)
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
				if (this._prevCheerKeyDown && !base.MissionScreen.SceneLayer.Input.IsGameKeyDown(31))
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

		// Token: 0x060001E0 RID: 480 RVA: 0x0000A69C File Offset: 0x0000889C
		private void HandleOpenHold()
		{
			MissionMainAgentCheerBarkControllerVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnSelectControllerToggle(true);
			}
			base.MissionScreen.SetRadialMenuActiveState(true);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000A6BC File Offset: 0x000088BC
		private void HandleClosingHoldCheer()
		{
			MissionMainAgentCheerBarkControllerVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnSelectControllerToggle(false);
			}
			base.MissionScreen.SetRadialMenuActiveState(false);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000A6DC File Offset: 0x000088DC
		private void HandleQuickReleaseCheer()
		{
			this.OnCheerSelect(0);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000A6E5 File Offset: 0x000088E5
		private void OnCheerSelect(int indexOfCheer)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new CheerSelected(indexOfCheer));
				GameNetwork.EndModuleEventAsClient();
			}
			else
			{
				Agent.Main.HandleCheer(indexOfCheer);
			}
			this._cooldownTimeRemaining = 4f;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000A71B File Offset: 0x0000891B
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
				Agent.Main.HandleBark(indexOfBark);
			}
			this._cooldownTimeRemaining = 2f;
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000A751 File Offset: 0x00008951
		private bool IsMainAgentAvailable()
		{
			Agent main = Agent.Main;
			return main != null && main.IsActive();
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000A763 File Offset: 0x00008963
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000A780 File Offset: 0x00008980
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x040000DB RID: 219
		private const float CooldownPeriodDurationAfterCheer = 4f;

		// Token: 0x040000DC RID: 220
		private const float CooldownPeriodDurationAfterBark = 2f;

		// Token: 0x040000DD RID: 221
		private const float _minHoldTime = 0f;

		// Token: 0x040000DE RID: 222
		private readonly IMissionScreen _missionScreenAsInterface;

		// Token: 0x040000DF RID: 223
		private MissionMainAgentController _missionMainAgentController;

		// Token: 0x040000E0 RID: 224
		private readonly TextObject _cooldownInfoText = new TextObject("{=aogZyZlR}You need to wait {SECONDS} seconds until you can cheer/shout again.", null);

		// Token: 0x040000E1 RID: 225
		private bool _holdHandled;

		// Token: 0x040000E2 RID: 226
		private float _holdTime;

		// Token: 0x040000E3 RID: 227
		private bool _prevCheerKeyDown;

		// Token: 0x040000E4 RID: 228
		private GauntletLayer _gauntletLayer;

		// Token: 0x040000E5 RID: 229
		private MissionMainAgentCheerBarkControllerVM _dataSource;

		// Token: 0x040000E6 RID: 230
		private float _cooldownTimeRemaining;

		// Token: 0x040000E7 RID: 231
		private bool _isSelectingFromInput;

		// Token: 0x040000E8 RID: 232
		private bool _isReturningToCategories;
	}
}
