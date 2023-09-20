using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x02000035 RID: 53
	[OverrideView(typeof(MissionSingleplayerEscapeMenu))]
	public class MissionGauntletSingleplayerEscapeMenu : MissionGauntletEscapeMenuBase
	{
		// Token: 0x06000280 RID: 640 RVA: 0x0000DEF5 File Offset: 0x0000C0F5
		public MissionGauntletSingleplayerEscapeMenu(bool isIronmanMode)
			: base("EscapeMenu")
		{
			this._isIronmanMode = isIronmanMode;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000DF0C File Offset: 0x0000C10C
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._missionOptionsComponent = base.Mission.GetMissionBehavior<MissionOptionsComponent>();
			this.DataSource = new EscapeMenuVM(null, null);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000DF5D File Offset: 0x0000C15D
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000DF85 File Offset: 0x0000C185
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 41)
			{
				EscapeMenuVM dataSource = this.DataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.RefreshItems(this.GetEscapeMenuItems());
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000DFA4 File Offset: 0x0000C1A4
		public override void OnFocusChangeOnGameWindow(bool focusGained)
		{
			base.OnFocusChangeOnGameWindow(focusGained);
			if (!focusGained && BannerlordConfig.StopGameOnFocusLost && base.MissionScreen.IsOpeningEscapeMenuOnFocusChangeAllowed() && !GameStateManager.Current.ActiveStateDisabledByUser && !LoadingWindow.IsLoadingWindowActive && !base.IsActive)
			{
				this.OnEscape();
			}
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000DFF1 File Offset: 0x0000C1F1
		public override void OnSceneRenderingStarted()
		{
			base.OnSceneRenderingStarted();
			if (base.MissionScreen.IsFocusLost)
			{
				EscapeMenuVM dataSource = this.DataSource;
				if (dataSource != null)
				{
					dataSource.RefreshItems(this.GetEscapeMenuItems());
				}
				base.OnEscapeMenuToggled(true);
			}
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000E028 File Offset: 0x0000C228
		protected override List<EscapeMenuItemVM> GetEscapeMenuItems()
		{
			TextObject ironmanDisabledReason = GameTexts.FindText("str_pause_menu_disabled_hint", "IronmanMode");
			List<EscapeMenuItemVM> list = new List<EscapeMenuItemVM>();
			list.Add(new EscapeMenuItemVM(new TextObject("{=e139gKZc}Return to the Game", null), delegate(object o)
			{
				this.OnEscapeMenuToggled(false);
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), true));
			list.Add(new EscapeMenuItemVM(new TextObject("{=NqarFr4P}Options", null), delegate(object o)
			{
				this.OnEscapeMenuToggled(false);
				MissionOptionsComponent missionOptionsComponent = this._missionOptionsComponent;
				if (missionOptionsComponent == null)
				{
					return;
				}
				missionOptionsComponent.OnAddOptionsUIHandler();
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			if (BannerlordConfig.HideBattleUI)
			{
				list.Add(new EscapeMenuItemVM(new TextObject("{=asCeKZXx}Re-enable Battle UI", null), delegate(object o)
				{
					ManagedOptions.SetConfig(41, 0f);
					ManagedOptions.SaveConfig();
					this.DataSource.RefreshItems(this.GetEscapeMenuItems());
				}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			}
			list.Add(new EscapeMenuItemVM(new TextObject("{=VklN5Wm6}Photo Mode", null), delegate(object o)
			{
				this.OnEscapeMenuToggled(false);
				this.MissionScreen.SetPhotoModeEnabled(true);
				this.Mission.IsInPhotoMode = true;
				InformationManager.ClearAllMessages();
			}, null, () => this.GetIsPhotoModeDisabled(), false));
			Action <>9__10;
			list.Add(new EscapeMenuItemVM(new TextObject("{=RamV6yLM}Exit to Main Menu", null), delegate(object o)
			{
				Game game = Game.Current;
				if (!(((game != null) ? game.GameType : null) is EditorGame))
				{
					Game game2 = Game.Current;
					if (!(((game2 != null) ? game2.GameType.GetType().Name : null) == "CustomGame"))
					{
						Game game3 = Game.Current;
						if (!(((game3 != null) ? game3.GameType : null) is MultiplayerGame))
						{
							string text = GameTexts.FindText("str_exit", null).ToString();
							string text2 = GameTexts.FindText("str_mission_exit_query", null).ToString();
							bool flag = true;
							bool flag2 = true;
							string text3 = GameTexts.FindText("str_yes", null).ToString();
							string text4 = GameTexts.FindText("str_no", null).ToString();
							Action action = new Action(this.OnExitToMainMenu);
							Action action2;
							if ((action2 = <>9__10) == null)
							{
								action2 = (<>9__10 = delegate
								{
									this.OnEscapeMenuToggled(false);
								});
							}
							InformationManager.ShowInquiry(new InquiryData(text, text2, flag, flag2, text3, text4, action, action2, "", 0f, null, null, null), false, false);
							return;
						}
					}
				}
				this.OnExitToMainMenu();
			}, null, () => new Tuple<bool, TextObject>(this._isIronmanMode, ironmanDisabledReason), false));
			return list;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000E190 File Offset: 0x0000C390
		private Tuple<bool, TextObject> GetIsPhotoModeDisabled()
		{
			if (base.MissionScreen.IsDeploymentActive)
			{
				return new Tuple<bool, TextObject>(true, new TextObject("{=rZSjkCpw}Cannot use photo mode during deployment.", null));
			}
			if (base.MissionScreen.IsConversationActive)
			{
				return new Tuple<bool, TextObject>(true, new TextObject("{=ImQnhIQ5}Cannot use photo mode during conversation.", null));
			}
			if (base.MissionScreen.IsPhotoModeEnabled)
			{
				return new Tuple<bool, TextObject>(true, new TextObject("{=79bODbwZ}Photo mode is already active.", null));
			}
			if (Module.CurrentModule.IsOnlyCoreContentEnabled)
			{
				return new Tuple<bool, TextObject>(true, new TextObject("{=V8BXjyYq}Disabled during installation.", null));
			}
			return new Tuple<bool, TextObject>(false, TextObject.Empty);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000E223 File Offset: 0x0000C423
		private void OnExitToMainMenu()
		{
			base.OnEscapeMenuToggled(false);
			InformationManager.HideInquiry();
			MBGameManager.EndGame();
		}

		// Token: 0x04000149 RID: 329
		private MissionOptionsComponent _missionOptionsComponent;

		// Token: 0x0400014A RID: 330
		private bool _isIronmanMode;
	}
}
