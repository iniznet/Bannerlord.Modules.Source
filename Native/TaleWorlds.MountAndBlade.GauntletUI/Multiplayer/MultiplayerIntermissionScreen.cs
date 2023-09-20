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
	// Token: 0x0200001F RID: 31
	[GameStateScreen(typeof(LobbyGameStateCustomGameClient))]
	public class MultiplayerIntermissionScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00006FCA File Offset: 0x000051CA
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00006FD2 File Offset: 0x000051D2
		public GauntletLayer Layer { get; private set; }

		// Token: 0x06000121 RID: 289 RVA: 0x00006FDC File Offset: 0x000051DC
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

		// Token: 0x06000122 RID: 290 RVA: 0x0000708B File Offset: 0x0000528B
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._dataSource.Tick();
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000709F File Offset: 0x0000529F
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._customGameClientCategory.Unload();
			this.Layer.InputRestrictions.ResetInputRestrictions();
			this.Layer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000070DB File Offset: 0x000052DB
		void IGameStateListener.OnActivate()
		{
			this.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			ScreenManager.TrySetFocus(this.Layer);
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000070FF File Offset: 0x000052FF
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00007101 File Offset: 0x00005301
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00007103 File Offset: 0x00005303
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x040000A5 RID: 165
		private MPIntermissionVM _dataSource;

		// Token: 0x040000A6 RID: 166
		private SpriteCategory _customGameClientCategory;
	}
}
