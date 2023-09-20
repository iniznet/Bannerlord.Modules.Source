using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Intermission;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Multiplayer
{
	[GameStateScreen(typeof(LobbyGameStateCustomGameClient))]
	public class MultiplayerIntermissionScreen : ScreenBase, IGameStateListener
	{
		public GauntletLayer Layer { get; private set; }

		public MultiplayerIntermissionScreen(LobbyGameStateCustomGameClient gameState)
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

		private MPIntermissionVM _dataSource;

		private SpriteCategory _customGameClientCategory;
	}
}
