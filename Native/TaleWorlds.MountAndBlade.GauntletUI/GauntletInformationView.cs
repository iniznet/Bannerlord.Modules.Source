using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class GauntletInformationView : GlobalLayer
	{
		private GauntletInformationView()
		{
			this._dataSource = new InformationVM();
			GauntletLayer gauntletLayer = new GauntletLayer(100000, "GauntletLayer", false);
			this._movie = gauntletLayer.LoadMovie("InformationUI", this._dataSource);
			base.Layer = gauntletLayer;
			this._layerAsGauntletLayer = gauntletLayer;
		}

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

		private string GetExtendTooltipKeyText()
		{
			if (Input.IsControllerConnected && !Input.IsMouseActive)
			{
				return this.GetKey("MapHotKeyCategory", "MapFollowModifier");
			}
			return Game.Current.GameTextManager.FindText("str_game_key_text", "anyalt").ToString();
		}

		private string GetKey(string categoryId, string keyId)
		{
			return GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, categoryId, keyId).ToString();
		}

		private string GetKey(string categoryId, int keyId)
		{
			return GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, categoryId, keyId).ToString();
		}

		private GauntletLayer _layerAsGauntletLayer;

		private InformationVM _dataSource;

		private IGauntletMovie _movie;

		private static GauntletInformationView _current;

		private const float _tooltipExtendTreshold = 0.18f;

		private float _gamepadTooltipExtendTimer;
	}
}
