using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000012 RID: 18
	public class BrushListPanel : ListPanel
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00006F0C File Offset: 0x0000510C
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00006F96 File Offset: 0x00005196
		[Editor(false)]
		public Brush Brush
		{
			get
			{
				if (this._originalBrush == null)
				{
					this._originalBrush = base.Context.DefaultBrush;
					this._clonedBrush = this._originalBrush.Clone();
					if (this.BrushRenderer != null)
					{
						this.BrushRenderer.Brush = this.ReadOnlyBrush;
					}
				}
				else if (this._clonedBrush == null)
				{
					this._clonedBrush = this._originalBrush.Clone();
					if (this.BrushRenderer != null)
					{
						this.BrushRenderer.Brush = this.ReadOnlyBrush;
					}
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

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00006FC1 File Offset: 0x000051C1
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

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00006FF1 File Offset: 0x000051F1
		// (set) Token: 0x06000118 RID: 280 RVA: 0x0000700D File Offset: 0x0000520D
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

		// Token: 0x06000119 RID: 281 RVA: 0x0000702A File Offset: 0x0000522A
		public void ForceUseBrush(Brush brush)
		{
			this._clonedBrush = brush;
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600011A RID: 282 RVA: 0x00007033 File Offset: 0x00005233
		// (set) Token: 0x0600011B RID: 283 RVA: 0x0000703B File Offset: 0x0000523B
		public BrushRenderer BrushRenderer { get; private set; }

		// Token: 0x0600011C RID: 284 RVA: 0x00007044 File Offset: 0x00005244
		public BrushListPanel(UIContext context)
			: base(context)
		{
			this.BrushRenderer = new BrushRenderer();
			base.EventFire += this.BrushWidget_EventFire;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000706C File Offset: 0x0000526C
		private void BrushWidget_EventFire(Widget arg1, string eventName, object[] arg3)
		{
			if (this.ReadOnlyBrush != null)
			{
				AudioProperty eventAudioProperty = this.Brush.SoundProperties.GetEventAudioProperty(eventName);
				if (eventAudioProperty != null && eventAudioProperty.AudioName != null && !eventAudioProperty.AudioName.Equals(""))
				{
					base.EventManager.Context.TwoDimensionContext.PlaySound(eventAudioProperty.AudioName);
				}
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000070CC File Offset: 0x000052CC
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

		// Token: 0x0600011F RID: 287 RVA: 0x00007194 File Offset: 0x00005394
		private void UpdateBrushRendererInternal(float dt)
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

		// Token: 0x06000120 RID: 288 RVA: 0x0000723C File Offset: 0x0000543C
		public override void SetState(string stateName)
		{
			if (base.CurrentState != stateName)
			{
				if (base.EventManager != null && this.ReadOnlyBrush != null)
				{
					AudioProperty stateAudioProperty = this.Brush.SoundProperties.GetStateAudioProperty(stateName);
					if (stateAudioProperty != null)
					{
						if (stateAudioProperty.AudioName != null && !stateAudioProperty.AudioName.Equals(""))
						{
							base.EventManager.Context.TwoDimensionContext.PlaySound(stateAudioProperty.AudioName);
						}
						else
						{
							Debug.FailedAssert(string.Concat(new string[] { "Widget with id \"", base.Id, "\" has a sound having no audioName for event \"", stateName, "\"!" }), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushListPanel.cs", "SetState", 180);
						}
					}
				}
				this.RegisterUpdateBrushes();
			}
			base.SetState(stateName);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000730D File Offset: 0x0000550D
		protected override void RefreshState()
		{
			base.RefreshState();
			this.RegisterUpdateBrushes();
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000731C File Offset: 0x0000551C
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (!this._isInsideCache || this.BrushRenderer.IsUpdateNeeded())
			{
				this.HandleUpdateNeededOnRender();
			}
			this.BrushRenderer.Render(drawContext, this._cachedGlobalPosition, base.Size, base._scaleToUse, base.Context.ContextAlpha, default(Vector2));
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00007376 File Offset: 0x00005576
		protected void HandleUpdateNeededOnRender()
		{
			this.UpdateBrushRendererInternal(base.EventManager.CachedDt);
			if (this.BrushRenderer.IsUpdateNeeded())
			{
				this.RegisterUpdateBrushes();
			}
			this._isInsideCache = true;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000073A3 File Offset: 0x000055A3
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.BrushRenderer.SetSeed(this._seed);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000073BC File Offset: 0x000055BC
		public override void UpdateAnimationPropertiesSubTask(float alphaFactor)
		{
			this.Brush.GlobalAlphaFactor = alphaFactor;
			foreach (Widget widget in base.Children)
			{
				widget.UpdateAnimationPropertiesSubTask(alphaFactor);
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000741C File Offset: 0x0000561C
		public virtual void OnBrushChanged()
		{
			this.RegisterUpdateBrushes();
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00007424 File Offset: 0x00005624
		private void RegisterUpdateBrushes()
		{
			base.EventManager.RegisterWidgetForEvent(WidgetContainer.ContainerType.UpdateBrushes, this);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00007433 File Offset: 0x00005633
		private void UnRegisterUpdateBrushes()
		{
			base.EventManager.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.UpdateBrushes, this);
		}

		// Token: 0x04000071 RID: 113
		private Brush _originalBrush;

		// Token: 0x04000072 RID: 114
		private Brush _clonedBrush;

		// Token: 0x04000074 RID: 116
		private bool _animRestarted;

		// Token: 0x04000075 RID: 117
		private bool _isInsideCache;
	}
}
