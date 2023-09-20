using System;
using System.Numerics;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000053 RID: 83
	public class BrushWidget : Widget
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x0600053F RID: 1343 RVA: 0x00016E34 File Offset: 0x00015034
		// (set) Token: 0x06000540 RID: 1344 RVA: 0x00016EAE File Offset: 0x000150AE
		[Editor(false)]
		public Brush Brush
		{
			get
			{
				if (this._originalBrush == null)
				{
					this._originalBrush = base.Context.DefaultBrush;
					this._clonedBrush = this._originalBrush.Clone();
					this.BrushRenderer.Brush = this.ReadOnlyBrush;
				}
				else if (this._clonedBrush == null)
				{
					this._clonedBrush = this._originalBrush.Clone();
					this.BrushRenderer.Brush = this.ReadOnlyBrush;
				}
				return this._clonedBrush;
			}
			set
			{
				if (this._originalBrush != value)
				{
					this._originalBrush = value;
					this._clonedBrush = null;
					this.OnBrushChanged();
					base.OnPropertyChanged<Brush>(value, "Brush");
				}
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x00016ED9 File Offset: 0x000150D9
		public Brush ReadOnlyBrush
		{
			get
			{
				if (this._clonedBrush != null)
				{
					return this._clonedBrush;
				}
				if (this._originalBrush == null)
				{
					this._originalBrush = base.Context.DefaultBrush;
				}
				return this._originalBrush;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000542 RID: 1346 RVA: 0x00016F09 File Offset: 0x00015109
		// (set) Token: 0x06000543 RID: 1347 RVA: 0x00016F25 File Offset: 0x00015125
		[Editor(false)]
		public new Sprite Sprite
		{
			get
			{
				return this.ReadOnlyBrush.DefaultStyle.GetLayer("Default").Sprite;
			}
			set
			{
				this.Brush.DefaultStyle.GetLayer("Default").Sprite = value;
			}
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x00016F42 File Offset: 0x00015142
		public void ForceUseBrush(Brush brush)
		{
			this._clonedBrush = brush;
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000545 RID: 1349 RVA: 0x00016F4B File Offset: 0x0001514B
		// (set) Token: 0x06000546 RID: 1350 RVA: 0x00016F53 File Offset: 0x00015153
		public BrushRenderer BrushRenderer { get; private set; }

		// Token: 0x06000547 RID: 1351 RVA: 0x00016F5C File Offset: 0x0001515C
		public BrushWidget(UIContext context)
			: base(context)
		{
			this.BrushRenderer = new BrushRenderer();
			base.EventFire += this.BrushWidget_EventFire;
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00016F84 File Offset: 0x00015184
		private void BrushWidget_EventFire(Widget arg1, string eventName, object[] arg3)
		{
			if (this.ReadOnlyBrush != null)
			{
				AudioProperty eventAudioProperty = this.ReadOnlyBrush.SoundProperties.GetEventAudioProperty(eventName);
				if (eventAudioProperty != null && eventAudioProperty.AudioName != null && !eventAudioProperty.AudioName.Equals(""))
				{
					base.EventManager.Context.TwoDimensionContext.PlaySound(eventAudioProperty.AudioName);
				}
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x00016FE4 File Offset: 0x000151E4
		public override void UpdateBrushes(float dt)
		{
			if (base.IsVisible)
			{
				Rectangle rectangle = new Rectangle(this._cachedGlobalPosition.X, this._cachedGlobalPosition.Y, base.MeasuredSize.X, base.MeasuredSize.Y);
				Rectangle rectangle2 = new Rectangle(base.EventManager.LeftUsableAreaStart, base.EventManager.TopUsableAreaStart, base.EventManager.PageSize.X, base.EventManager.PageSize.Y);
				this._isInsideCache = rectangle.IsCollide(rectangle2);
				if (this._isInsideCache)
				{
					this.UpdateBrushRendererInternal(dt);
				}
			}
			if (!base.IsVisible || !this._isInsideCache || !this.BrushRenderer.IsUpdateNeeded())
			{
				this.UnRegisterUpdateBrushes();
			}
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x000170AC File Offset: 0x000152AC
		protected void UpdateBrushRendererInternal(float dt)
		{
			this.BrushRenderer.ForcePixelPerfectPlacement = base.ForcePixelPerfectRenderPlacement;
			this.BrushRenderer.UseLocalTimer = !base.UseGlobalTimeForAnimation;
			this.BrushRenderer.Brush = this.ReadOnlyBrush;
			this.BrushRenderer.CurrentState = base.CurrentState;
			this.BrushRenderer.Update(base.Context.TwoDimensionContext.Platform.ApplicationTime, dt);
			if (base.RestartAnimationFirstFrame && !this._animRestarted)
			{
				base.EventManager.AddLateUpdateAction(this, delegate(float _dt)
				{
					if (base.RestartAnimationFirstFrame)
					{
						this.BrushRenderer.RestartAnimation();
					}
				}, 5);
				this._animRestarted = true;
			}
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x00017154 File Offset: 0x00015354
		public override void SetState(string stateName)
		{
			if (base.CurrentState != stateName)
			{
				if (base.EventManager != null && this.ReadOnlyBrush != null)
				{
					AudioProperty stateAudioProperty = this.ReadOnlyBrush.SoundProperties.GetStateAudioProperty(stateName);
					if (stateAudioProperty != null)
					{
						if (stateAudioProperty.AudioName != null && !stateAudioProperty.AudioName.Equals(""))
						{
							base.EventManager.Context.TwoDimensionContext.PlaySound(stateAudioProperty.AudioName);
						}
						else
						{
							Debug.FailedAssert(string.Concat(new string[] { "Widget with id \"", base.Id, "\" has a sound having no audioName for event \"", stateName, "\"!" }), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\BaseTypes\\BrushWidget.cs", "SetState", 174);
						}
					}
				}
				this.RegisterUpdateBrushes();
			}
			base.SetState(stateName);
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x00017225 File Offset: 0x00015425
		protected override void RefreshState()
		{
			base.RefreshState();
			this.RegisterUpdateBrushes();
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x00017234 File Offset: 0x00015434
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (!this._isInsideCache || this.BrushRenderer.IsUpdateNeeded())
			{
				this.HandleUpdateNeededOnRender();
			}
			this.BrushRenderer.Render(drawContext, this._cachedGlobalPosition, base.Size, base._scaleToUse, base.Context.ContextAlpha, default(Vector2));
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001728E File Offset: 0x0001548E
		protected void HandleUpdateNeededOnRender()
		{
			this.UpdateBrushRendererInternal(base.EventManager.CachedDt);
			if (this.BrushRenderer.IsUpdateNeeded())
			{
				this.RegisterUpdateBrushes();
			}
			this._isInsideCache = true;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x000172BB File Offset: 0x000154BB
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.BrushRenderer.SetSeed(this._seed);
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x000172D4 File Offset: 0x000154D4
		public override void UpdateAnimationPropertiesSubTask(float alphaFactor)
		{
			this.Brush.GlobalAlphaFactor = alphaFactor;
			foreach (Widget widget in base.Children)
			{
				widget.UpdateAnimationPropertiesSubTask(alphaFactor);
			}
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00017334 File Offset: 0x00015534
		public virtual void OnBrushChanged()
		{
			this.RegisterUpdateBrushes();
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0001733C File Offset: 0x0001553C
		protected void RegisterUpdateBrushes()
		{
			base.EventManager.RegisterWidgetForEvent(WidgetContainer.ContainerType.UpdateBrushes, this);
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0001734B File Offset: 0x0001554B
		protected void UnRegisterUpdateBrushes()
		{
			base.EventManager.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.UpdateBrushes, this);
		}

		// Token: 0x0400028E RID: 654
		private Brush _originalBrush;

		// Token: 0x0400028F RID: 655
		private Brush _clonedBrush;

		// Token: 0x04000291 RID: 657
		private bool _animRestarted;

		// Token: 0x04000292 RID: 658
		protected bool _isInsideCache;
	}
}
