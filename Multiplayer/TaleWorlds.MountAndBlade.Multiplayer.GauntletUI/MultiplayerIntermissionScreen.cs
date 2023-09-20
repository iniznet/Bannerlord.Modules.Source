﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Intermission;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI
{
	[GameStateScreen(typeof(LobbyGameStateCustomGameClient))]
	[GameStateScreen(typeof(LobbyGameStateCommunityClient))]
	public class MultiplayerIntermissionScreen : ScreenBase, IGameStateListener, IGauntletChatLogHandlerScreen
	{
		public GauntletLayer Layer { get; private set; }

		public MultiplayerIntermissionScreen(LobbyGameStateCustomGameClient gameState)
		{
			this.Construct();
		}

		public MultiplayerIntermissionScreen(LobbyGameStateCommunityClient gameState)
		{
			this.Construct();
		}

		private void Construct()
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._customGameClientCategory = spriteData.SpriteCategories["ui_mpintermission"];
			this._customGameClientCategory.Load(resourceContext, uiresourceDepot);
			this._dataSource = new MPIntermissionVM();
			this.Layer = new GauntletLayer(100, "GauntletLayer", false);
			this.Layer.IsFocusLayer = true;
			base.AddLayer(this.Layer);
			this.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this.Layer.LoadMovie("MultiplayerIntermission", this._dataSource);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._dataSource.Tick();
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._customGameClientCategory.Unload();
			this.Layer.InputRestrictions.ResetInputRestrictions();
			this.Layer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		void IGameStateListener.OnActivate()
		{
			this.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			ScreenManager.TrySetFocus(this.Layer);
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
		}

		void IGauntletChatLogHandlerScreen.TryUpdateChatLogLayerParameters(ref bool isTeamChatAvailable, ref bool inputEnabled, ref InputContext inputContext)
		{
			if (this.Layer != null)
			{
				isTeamChatAvailable = false;
				inputEnabled = true;
				inputContext = this.Layer.Input;
			}
		}

		private MPIntermissionVM _dataSource;

		private SpriteCategory _customGameClientCategory;
	}
}
