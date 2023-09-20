using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class GauntletInformationView : GlobalLayer
	{
		private GauntletInformationView()
		{
			GauntletLayer gauntletLayer = new GauntletLayer(100000, "GauntletLayer", false);
			InformationManager.OnShowTooltip += this.OnShowTooltip;
			InformationManager.OnHideTooltip += this.OnHideTooltip;
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
			if (this._dataSource != null && (Input.IsKeyDown(56) || Input.IsKeyDown(184) || Input.IsKeyDown(248)))
			{
				this._gamepadTooltipExtendTimer += dt;
			}
			else
			{
				this._gamepadTooltipExtendTimer = 0f;
			}
			if (this._dataSource != null)
			{
				this._dataSource.Tick(dt);
				this._dataSource.IsExtended = (Input.IsGamepadActive ? (this._gamepadTooltipExtendTimer > 0.18f) : (this._gamepadTooltipExtendTimer > 0f));
			}
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

		private void OnShowTooltip(Type type, object[] args)
		{
			this.OnHideTooltip();
			ValueTuple<Type, object, string> valueTuple;
			if (InformationManager.RegisteredTypes.TryGetValue(type, out valueTuple))
			{
				this._dataSource = Activator.CreateInstance(valueTuple.Item1, new object[] { type, args }) as TooltipBaseVM;
				this._movie = this._layerAsGauntletLayer.LoadMovie(valueTuple.Item3, this._dataSource);
			}
		}

		private void OnHideTooltip()
		{
			if (this._dataSource != null)
			{
				this._dataSource.OnFinalize();
				this._layerAsGauntletLayer.ReleaseMovie(this._movie);
				this._dataSource = null;
				this._movie = null;
			}
		}

		private TooltipBaseVM _dataSource;

		private IGauntletMovie _movie;

		private GauntletLayer _layerAsGauntletLayer;

		private static GauntletInformationView _current;

		private const float _tooltipExtendTreshold = 0.18f;

		private float _gamepadTooltipExtendTimer;
	}
}
