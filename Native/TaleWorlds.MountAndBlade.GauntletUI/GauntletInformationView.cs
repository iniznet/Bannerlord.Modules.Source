using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x0200000A RID: 10
	public class GauntletInformationView : GlobalLayer
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003068 File Offset: 0x00001268
		private GauntletInformationView()
		{
			this._dataSource = new InformationVM();
			GauntletLayer gauntletLayer = new GauntletLayer(100000, "GauntletLayer", false);
			this._movie = gauntletLayer.LoadMovie("InformationUI", this._dataSource);
			base.Layer = gauntletLayer;
			this._layerAsGauntletLayer = gauntletLayer;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000030BC File Offset: 0x000012BC
		public static void Initialize()
		{
			if (GauntletInformationView._current == null)
			{
				GauntletInformationView._current = new GauntletInformationView();
				ScreenManager.AddGlobalLayer(GauntletInformationView._current, false);
				PropertyBasedTooltipVM.AddKeyType("MapClick", () => GauntletInformationView._current.GetKey("MapHotKeyCategory", "MapClick"));
				PropertyBasedTooltipVM.AddKeyType("FollowModifier", () => GauntletInformationView._current.GetKey("MapHotKeyCategory", "MapFollowModifier"));
				PropertyBasedTooltipVM.AddKeyType("ExtendModifier", () => GauntletInformationView._current.GetExtendTooltipKeyText());
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003164 File Offset: 0x00001364
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			this._dataSource.Tick(dt);
			if (this._dataSource.Tooltip != null && (Input.IsKeyDown(56) || Input.IsKeyDown(184) || Input.IsKeyDown(248)))
			{
				this._gamepadTooltipExtendTimer += dt;
			}
			else
			{
				this._gamepadTooltipExtendTimer = 0f;
			}
			this._dataSource.Tooltip.IsExtended = (Input.IsGamepadActive ? (this._gamepadTooltipExtendTimer > 0.18f) : (this._gamepadTooltipExtendTimer > 0f));
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003200 File Offset: 0x00001400
		private string GetExtendTooltipKeyText()
		{
			if (Input.IsControllerConnected && !Input.IsMouseActive)
			{
				return this.GetKey("MapHotKeyCategory", "MapFollowModifier");
			}
			return Game.Current.GameTextManager.FindText("str_game_key_text", "anyalt").ToString();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003250 File Offset: 0x00001450
		private string GetKey(string categoryId, string keyId)
		{
			return GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, categoryId, keyId).ToString();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003268 File Offset: 0x00001468
		private string GetKey(string categoryId, int keyId)
		{
			return GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, categoryId, keyId).ToString();
		}

		// Token: 0x04000020 RID: 32
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x04000021 RID: 33
		private InformationVM _dataSource;

		// Token: 0x04000022 RID: 34
		private IGauntletMovie _movie;

		// Token: 0x04000023 RID: 35
		private static GauntletInformationView _current;

		// Token: 0x04000024 RID: 36
		private const float _tooltipExtendTreshold = 0.18f;

		// Token: 0x04000025 RID: 37
		private float _gamepadTooltipExtendTimer;
	}
}
