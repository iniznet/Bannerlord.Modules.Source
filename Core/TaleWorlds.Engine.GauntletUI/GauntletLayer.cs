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
	// Token: 0x02000004 RID: 4
	public class GauntletLayer : ScreenLayer
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000020DC File Offset: 0x000002DC
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

		// Token: 0x0600000E RID: 14 RVA: 0x00002274 File Offset: 0x00000474
		private void EventManagerOnLoseFocus()
		{
			if (!base.IsFocusLayer)
			{
				ScreenManager.TryLoseFocus(this);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002284 File Offset: 0x00000484
		private void EventManagerOnGainFocus()
		{
			ScreenManager.TrySetFocus(this);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000228C File Offset: 0x0000048C
		public IGauntletMovie LoadMovie(string movieName, ViewModel dataSource)
		{
			bool flag = NativeConfig.GetUIDoNotUseGeneratedPrefabs || UIConfig.DoNotUseGeneratedPrefabs;
			bool flag2 = NativeConfig.GetUIDebugMode || UIConfig.DebugModeEnabled;
			IGauntletMovie gauntletMovie = GauntletMovie.Load(this._gauntletUIContext, this._widgetFactory, movieName, dataSource, flag, flag2);
			this._moviesAndDatasources.Add(new Tuple<IGauntletMovie, ViewModel>(gauntletMovie, dataSource));
			return gauntletMovie;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000022E4 File Offset: 0x000004E4
		public void ReleaseMovie(IGauntletMovie movie)
		{
			Tuple<IGauntletMovie, ViewModel> tuple = this._moviesAndDatasources.SingleOrDefault((Tuple<IGauntletMovie, ViewModel> t) => t.Item1 == movie);
			this._moviesAndDatasources.Remove(tuple);
			movie.Release();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000232E File Offset: 0x0000052E
		protected override void OnActivate()
		{
			base.OnActivate();
			this._twoDimensionView.SetEnable(true);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002342 File Offset: 0x00000542
		protected override void OnDeactivate()
		{
			this._twoDimensionView.Clear();
			this._twoDimensionView.SetEnable(false);
			base.OnDeactivate();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002364 File Offset: 0x00000564
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

		// Token: 0x06000015 RID: 21 RVA: 0x000023E8 File Offset: 0x000005E8
		protected override void LateTick(float dt)
		{
			base.LateTick(dt);
			this._twoDimensionView.BeginFrame();
			this._gauntletUIContext.LateUpdate(dt);
			this._twoDimensionView.EndFrame();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002413 File Offset: 0x00000613
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this._engineInputService.UpdateInputDevices(base.KeyboardEnabled, base.MouseEnabled, base.GamepadEnabled);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002439 File Offset: 0x00000639
		protected override void Update(IReadOnlyList<int> lastKeysPressed)
		{
			Widget focusedWidget = this._gauntletUIContext.EventManager.FocusedWidget;
			if (focusedWidget == null)
			{
				return;
			}
			focusedWidget.HandleInput(lastKeysPressed);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002458 File Offset: 0x00000658
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

		// Token: 0x06000019 RID: 25 RVA: 0x0000253C File Offset: 0x0000073C
		protected override void RefreshGlobalOrder(ref int currentOrder)
		{
			this._twoDimensionView.SetRenderOrder(currentOrder);
			currentOrder++;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002551 File Offset: 0x00000751
		public override void ProcessEvents()
		{
			base.ProcessEvents();
			this._gauntletUIContext.UpdateInput(base._usedInputs);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000256C File Offset: 0x0000076C
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

		// Token: 0x0600001C RID: 28 RVA: 0x000025D8 File Offset: 0x000007D8
		private bool GetIsBlockedAtPosition(Vector2 position)
		{
			return ScreenManager.IsLayerBlockedAtPosition(this, position);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000025E4 File Offset: 0x000007E4
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

		// Token: 0x0600001E RID: 30 RVA: 0x00002660 File Offset: 0x00000860
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

		// Token: 0x0600001F RID: 31 RVA: 0x000026CC File Offset: 0x000008CC
		public override bool IsFocusedOnInput()
		{
			return this._gauntletUIContext.EventManager.FocusedWidget is EditableTextWidget;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000026E6 File Offset: 0x000008E6
		protected override void OnLoseFocus()
		{
			this._gauntletUIContext.EventManager.ClearFocus();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026F8 File Offset: 0x000008F8
		public override void OnOnScreenKeyboardDone(string inputText)
		{
			base.OnOnScreenKeyboardDone(inputText);
			this._gauntletUIContext.OnOnScreenkeyboardTextInputDone(inputText);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000270D File Offset: 0x0000090D
		public override void OnOnScreenKeyboardCanceled()
		{
			base.OnOnScreenKeyboardCanceled();
			this._gauntletUIContext.OnOnScreenKeyboardCanceled();
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002720 File Offset: 0x00000920
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

		// Token: 0x06000024 RID: 36 RVA: 0x000027BE File Offset: 0x000009BE
		public bool GetIsAvailableForGamepadNavigation()
		{
			return base.LastActiveState && base.IsActive && (base.MouseEnabled || base.GamepadEnabled) && (base.IsFocusLayer || (base.InputRestrictions.InputUsageMask & InputUsageMask.Mouse) > InputUsageMask.Invalid);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000027FC File Offset: 0x000009FC
		private bool GetIsHitThisFrame()
		{
			return base.IsHitThisFrame;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002804 File Offset: 0x00000A04
		private int GetLastScreenOrder()
		{
			return base.ScreenOrderInLastFrame;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000280C File Offset: 0x00000A0C
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

		// Token: 0x04000006 RID: 6
		public readonly TwoDimensionView _twoDimensionView;

		// Token: 0x04000007 RID: 7
		public readonly UIContext _gauntletUIContext;

		// Token: 0x04000008 RID: 8
		public readonly List<Tuple<IGauntletMovie, ViewModel>> _moviesAndDatasources;

		// Token: 0x04000009 RID: 9
		public readonly TwoDimensionEnginePlatform _twoDimensionEnginePlatform;

		// Token: 0x0400000A RID: 10
		public readonly EngineInputService _engineInputService;

		// Token: 0x0400000B RID: 11
		public readonly WidgetFactory _widgetFactory;
	}
}
