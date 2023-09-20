using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class BrushListPanel : ListPanel
	{
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

		public void ForceUseBrush(Brush brush)
		{
			this._clonedBrush = brush;
		}

		public BrushRenderer BrushRenderer { get; private set; }

		public BrushListPanel(UIContext context)
			: base(context)
		{
			this.BrushRenderer = new BrushRenderer();
			base.EventFire += this.BrushWidget_EventFire;
		}

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

		protected override void RefreshState()
		{
			base.RefreshState();
			this.RegisterUpdateBrushes();
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (!this._isInsideCache || this.BrushRenderer.IsUpdateNeeded())
			{
				this.HandleUpdateNeededOnRender();
			}
			this.BrushRenderer.Render(drawContext, this._cachedGlobalPosition, base.Size, base._scaleToUse, base.Context.ContextAlpha, default(Vector2));
		}

		protected void HandleUpdateNeededOnRender()
		{
			this.UpdateBrushRendererInternal(base.EventManager.CachedDt);
			if (this.BrushRenderer.IsUpdateNeeded())
			{
				this.RegisterUpdateBrushes();
			}
			this._isInsideCache = true;
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.BrushRenderer.SetSeed(this._seed);
		}

		public override void UpdateAnimationPropertiesSubTask(float alphaFactor)
		{
			this.Brush.GlobalAlphaFactor = alphaFactor;
			foreach (Widget widget in base.Children)
			{
				widget.UpdateAnimationPropertiesSubTask(alphaFactor);
			}
		}

		public virtual void OnBrushChanged()
		{
			this.RegisterUpdateBrushes();
		}

		private void RegisterUpdateBrushes()
		{
			base.EventManager.RegisterWidgetForEvent(WidgetContainer.ContainerType.UpdateBrushes, this);
		}

		private void UnRegisterUpdateBrushes()
		{
			base.EventManager.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.UpdateBrushes, this);
		}

		private Brush _originalBrush;

		private Brush _clonedBrush;

		private bool _animRestarted;

		private bool _isInsideCache;
	}
}
