using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class AgentHealthWidget : Widget
	{
		public AgentHealthWidget(UIContext context)
			: base(context)
		{
			this._healtDrops = new List<AgentHealthWidget.HealthDropData>();
			this.CheckVisibility();
		}

		private void CreateHealthDrop(Widget container, int preHealth, int currentHealth)
		{
			float num = container.Size.X / base._scaleToUse;
			float num2 = Mathf.Ceil(num * ((float)(preHealth - currentHealth) / (float)this._maxHealth));
			float num3 = Mathf.Floor(num * ((float)currentHealth / (float)this._maxHealth));
			BrushWidget brushWidget = new BrushWidget(base.Context);
			brushWidget.WidthSizePolicy = SizePolicy.Fixed;
			brushWidget.HeightSizePolicy = SizePolicy.Fixed;
			brushWidget.Brush = this.HealthDropBrush;
			brushWidget.SuggestedWidth = num2;
			brushWidget.SuggestedHeight = (float)brushWidget.ReadOnlyBrush.Sprite.Height;
			brushWidget.HorizontalAlignment = HorizontalAlignment.Left;
			brushWidget.VerticalAlignment = VerticalAlignment.Center;
			brushWidget.PositionXOffset = num3;
			brushWidget.ParentWidget = container;
			this._healtDrops.Add(new AgentHealthWidget.HealthDropData(brushWidget, this.AnimationDelay + this.AnimationDuration));
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.HealthBar != null && this.HealthBar.IsVisible)
			{
				for (int i = this._healtDrops.Count - 1; i >= 0; i--)
				{
					AgentHealthWidget.HealthDropData healthDropData = this._healtDrops[i];
					healthDropData.LifeTime -= dt;
					if (healthDropData.LifeTime <= 0f)
					{
						this.HealthDropContainer.RemoveChild(healthDropData.Widget);
						this._healtDrops.RemoveAt(i);
					}
					else
					{
						float num = Mathf.Min(1f, healthDropData.LifeTime / this.AnimationDuration);
						healthDropData.Widget.Brush.AlphaFactor = num;
					}
				}
			}
			this.CheckVisibility();
		}

		private void HealthChanged(bool createDropVisual = true)
		{
			this.HealthBar.MaxAmount = this._maxHealth;
			this.HealthBar.InitialAmount = this.Health;
			if (this._prevHealth > this.Health)
			{
				this.CreateHealthDrop(this.HealthDropContainer, this._prevHealth, this.Health);
			}
		}

		private void CheckVisibility()
		{
			bool flag = this.ShowHealthBar;
			if (flag)
			{
				flag = (float)this._health > 0f || this._healtDrops.Count > 0;
			}
			base.IsVisible = flag;
		}

		[Editor(false)]
		public int Health
		{
			get
			{
				return this._health;
			}
			set
			{
				if (this._health != value)
				{
					this._prevHealth = this._health;
					this._health = value;
					this.HealthChanged(true);
					base.OnPropertyChanged(value, "Health");
				}
			}
		}

		[Editor(false)]
		public int MaxHealth
		{
			get
			{
				return this._maxHealth;
			}
			set
			{
				if (this._maxHealth != value)
				{
					this._maxHealth = value;
					this.HealthChanged(false);
					base.OnPropertyChanged(value, "MaxHealth");
				}
			}
		}

		[Editor(false)]
		public FillBarWidget HealthBar
		{
			get
			{
				return this._healthBar;
			}
			set
			{
				if (this._healthBar != value)
				{
					this._healthBar = value;
					base.OnPropertyChanged<FillBarWidget>(value, "HealthBar");
				}
			}
		}

		[Editor(false)]
		public Widget HealthDropContainer
		{
			get
			{
				return this._healthDropContainer;
			}
			set
			{
				if (this._healthDropContainer != value)
				{
					this._healthDropContainer = value;
					base.OnPropertyChanged<Widget>(value, "HealthDropContainer");
				}
			}
		}

		[Editor(false)]
		public Brush HealthDropBrush
		{
			get
			{
				return this._healthDropBrush;
			}
			set
			{
				if (this._healthDropBrush != value)
				{
					this._healthDropBrush = value;
					base.OnPropertyChanged<Brush>(value, "HealthDropBrush");
				}
			}
		}

		[Editor(false)]
		public bool ShowHealthBar
		{
			get
			{
				return this._showHealthBar;
			}
			set
			{
				if (this._showHealthBar != value)
				{
					this._showHealthBar = value;
					base.OnPropertyChanged(value, "ShowHealthBar");
				}
			}
		}

		private float AnimationDelay = 0.2f;

		private float AnimationDuration = 0.8f;

		private List<AgentHealthWidget.HealthDropData> _healtDrops;

		private int _health;

		private int _prevHealth = -1;

		private int _maxHealth;

		private bool _showHealthBar;

		private FillBarWidget _healthBar;

		private Widget _healthDropContainer;

		private Brush _healthDropBrush;

		public class HealthDropData
		{
			public HealthDropData(BrushWidget widget, float lifeTime)
			{
				this.Widget = widget;
				this.LifeTime = lifeTime;
			}

			public BrushWidget Widget;

			public float LifeTime;
		}
	}
}
