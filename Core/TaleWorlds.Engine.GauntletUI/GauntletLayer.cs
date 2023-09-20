using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.Engine.GauntletUI
{
	public class GauntletLayer : ScreenLayer
	{
		public GauntletLayer(int localOrder, string categoryId = "GauntletLayer", bool shouldClear = false)
			: base(localOrder, categoryId)
		{
			this._moviesAndDatasources = new List<Tuple<IGauntletMovie, ViewModel>>();
			this._widgetFactory = UIResourceManager.WidgetFactory;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._twoDimensionView = TwoDimensionView.CreateTwoDimension();
			if (shouldClear)
			{
				this._twoDimensionView.SetClearColor(255U);
				this._twoDimensionView.SetRenderOption(View.ViewRenderOptions.ClearColor, true);
			}
			this._twoDimensionEnginePlatform = new TwoDimensionEnginePlatform(this._twoDimensionView);
			TwoDimensionContext twoDimensionContext = new TwoDimensionContext(this._twoDimensionEnginePlatform, UIResourceManager.ResourceContext, uiresourceDepot);
			this._engineInputService = new EngineInputService(base.Input);
			this._gauntletUIContext = new UIContext(twoDimensionContext, base.Input, this._engineInputService, UIResourceManager.SpriteData, UIResourceManager.FontFactory, UIResourceManager.BrushFactory);
			this._gauntletUIContext.ScaleModifier = base.Scale;
			this._gauntletUIContext.Initialize();
			base.MouseEnabled = true;
			this._gauntletUIContext.EventManager.LoseFocus += this.EventManagerOnLoseFocus;
			this._gauntletUIContext.EventManager.GainFocus += this.EventManagerOnGainFocus;
			this._gauntletUIContext.EventManager.OnGetIsAvailableForGamepadNavigation = new Func<bool>(this.GetIsAvailableForGamepadNavigation);
			this._gauntletUIContext.EventManager.OnGetLastScreenOrder = new Func<int>(this.GetLastScreenOrder);
			this._gauntletUIContext.EventManager.OnGetIsHitThisFrame = new Func<bool>(this.GetIsHitThisFrame);
			this._gauntletUIContext.EventManager.OnGetIsBlockedAtPosition = new Func<Vector2, bool>(this.GetIsBlockedAtPosition);
			this._gauntletUIContext.EventManager.UsableArea = base.UsableArea;
		}

		private void EventManagerOnLoseFocus()
		{
			if (!base.IsFocusLayer)
			{
				ScreenManager.TryLoseFocus(this);
			}
		}

		private void EventManagerOnGainFocus()
		{
			ScreenManager.TrySetFocus(this);
		}

		public IGauntletMovie LoadMovie(string movieName, ViewModel dataSource)
		{
			bool flag = NativeConfig.GetUIDoNotUseGeneratedPrefabs || UIConfig.DoNotUseGeneratedPrefabs;
			bool flag2 = NativeConfig.GetUIDebugMode || UIConfig.DebugModeEnabled;
			IGauntletMovie gauntletMovie = GauntletMovie.Load(this._gauntletUIContext, this._widgetFactory, movieName, dataSource, flag, flag2);
			this._moviesAndDatasources.Add(new Tuple<IGauntletMovie, ViewModel>(gauntletMovie, dataSource));
			return gauntletMovie;
		}

		public void ReleaseMovie(IGauntletMovie movie)
		{
			Tuple<IGauntletMovie, ViewModel> tuple = this._moviesAndDatasources.SingleOrDefault((Tuple<IGauntletMovie, ViewModel> t) => t.Item1 == movie);
			this._moviesAndDatasources.Remove(tuple);
			movie.Release();
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this._twoDimensionView.SetEnable(true);
		}

		protected override void OnDeactivate()
		{
			this._twoDimensionView.Clear();
			this._twoDimensionView.SetEnable(false);
			base.OnDeactivate();
		}

		protected override void Tick(float dt)
		{
			base.Tick(dt);
			this._twoDimensionEnginePlatform.Reset();
			this._gauntletUIContext.Update(dt);
			foreach (Tuple<IGauntletMovie, ViewModel> tuple in this._moviesAndDatasources)
			{
				tuple.Item1.Update();
			}
			base.ActiveCursor = (CursorType)this._gauntletUIContext.ActiveCursorOfContext;
		}

		protected override void LateTick(float dt)
		{
			base.LateTick(dt);
			this._twoDimensionView.BeginFrame();
			this._gauntletUIContext.LateUpdate(dt);
			this._twoDimensionView.EndFrame();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this._engineInputService.UpdateInputDevices(base.KeyboardEnabled, base.MouseEnabled, base.GamepadEnabled);
		}

		protected override void Update(IReadOnlyList<int> lastKeysPressed)
		{
			Widget focusedWidget = this._gauntletUIContext.EventManager.FocusedWidget;
			if (focusedWidget == null)
			{
				return;
			}
			focusedWidget.HandleInput(lastKeysPressed);
		}

		protected override void OnFinalize()
		{
			foreach (Tuple<IGauntletMovie, ViewModel> tuple in this._moviesAndDatasources)
			{
				tuple.Item1.Release();
			}
			this._gauntletUIContext.EventManager.LoseFocus -= this.EventManagerOnLoseFocus;
			this._gauntletUIContext.EventManager.GainFocus -= this.EventManagerOnGainFocus;
			this._gauntletUIContext.EventManager.OnGetIsAvailableForGamepadNavigation = null;
			this._gauntletUIContext.EventManager.OnGetLastScreenOrder = null;
			this._gauntletUIContext.EventManager.OnGetIsHitThisFrame = null;
			this._gauntletUIContext.EventManager.OnGetIsBlockedAtPosition = null;
			this._gauntletUIContext.OnFinalize();
			base.OnFinalize();
		}

		protected override void RefreshGlobalOrder(ref int currentOrder)
		{
			this._twoDimensionView.SetRenderOrder(currentOrder);
			currentOrder++;
		}

		public override void ProcessEvents()
		{
			base.ProcessEvents();
			this._gauntletUIContext.UpdateInput(base._usedInputs);
		}

		public override bool HitTest(Vector2 position)
		{
			foreach (Tuple<IGauntletMovie, ViewModel> tuple in this._moviesAndDatasources)
			{
				if (this._gauntletUIContext.HitTest(tuple.Item1.RootWidget, position))
				{
					return true;
				}
			}
			return false;
		}

		private bool GetIsBlockedAtPosition(Vector2 position)
		{
			return ScreenManager.IsLayerBlockedAtPosition(this, position);
		}

		public override bool HitTest()
		{
			foreach (Tuple<IGauntletMovie, ViewModel> tuple in this._moviesAndDatasources)
			{
				if (this._gauntletUIContext.HitTest(tuple.Item1.RootWidget))
				{
					return true;
				}
			}
			this._gauntletUIContext.EventManager.SetHoveredView(null);
			return false;
		}

		public override bool FocusTest()
		{
			foreach (Tuple<IGauntletMovie, ViewModel> tuple in this._moviesAndDatasources)
			{
				if (this._gauntletUIContext.FocusTest(tuple.Item1.RootWidget))
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsFocusedOnInput()
		{
			return this._gauntletUIContext.EventManager.FocusedWidget is EditableTextWidget;
		}

		protected override void OnLoseFocus()
		{
			this._gauntletUIContext.EventManager.ClearFocus();
		}

		public override void OnOnScreenKeyboardDone(string inputText)
		{
			base.OnOnScreenKeyboardDone(inputText);
			this._gauntletUIContext.OnOnScreenkeyboardTextInputDone(inputText);
		}

		public override void OnOnScreenKeyboardCanceled()
		{
			base.OnOnScreenKeyboardCanceled();
			this._gauntletUIContext.OnOnScreenKeyboardCanceled();
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();
			this._gauntletUIContext.ScaleModifier = base.Scale;
			this._gauntletUIContext.EventManager.UsableArea = base.UsableArea;
			this._moviesAndDatasources.ForEach(delegate(Tuple<IGauntletMovie, ViewModel> m)
			{
				m.Item2.RefreshValues();
			});
			this._moviesAndDatasources.ForEach(delegate(Tuple<IGauntletMovie, ViewModel> m)
			{
				m.Item1.RefreshBindingWithChildren();
			});
			this._gauntletUIContext.EventManager.UpdateLayout();
		}

		public bool GetIsAvailableForGamepadNavigation()
		{
			return base.LastActiveState && base.IsActive && (base.MouseEnabled || base.GamepadEnabled) && (base.IsFocusLayer || (base.InputRestrictions.InputUsageMask & InputUsageMask.Mouse) > InputUsageMask.Invalid);
		}

		private bool GetIsHitThisFrame()
		{
			return base.IsHitThisFrame;
		}

		private int GetLastScreenOrder()
		{
			return base.ScreenOrderInLastFrame;
		}

		public override void DrawDebugInfo()
		{
			foreach (Tuple<IGauntletMovie, ViewModel> tuple in this._moviesAndDatasources)
			{
				IGauntletMovie item = tuple.Item1;
				ViewModel item2 = tuple.Item2;
				Imgui.Text("Movie: " + item.MovieName);
				Imgui.Text("Data Source: " + (((item2 != null) ? item2.GetType().Name : null) ?? "No Datasource"));
			}
			base.DrawDebugInfo();
			Imgui.Text("Press 'Shift+F' to take widget hierarchy snapshot.");
			this._gauntletUIContext.DrawWidgetDebugInfo();
		}

		public readonly TwoDimensionView _twoDimensionView;

		public readonly UIContext _gauntletUIContext;

		public readonly List<Tuple<IGauntletMovie, ViewModel>> _moviesAndDatasources;

		public readonly TwoDimensionEnginePlatform _twoDimensionEnginePlatform;

		public readonly EngineInputService _engineInputService;

		public readonly WidgetFactory _widgetFactory;
	}
}
