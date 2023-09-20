using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C2 RID: 194
	public class AgentHealthWidget : Widget
	{
		// Token: 0x060009CB RID: 2507 RVA: 0x0001BF2B File Offset: 0x0001A12B
		public AgentHealthWidget(UIContext context)
			: base(context)
		{
			this._healtDrops = new List<AgentHealthWidget.HealthDropData>();
			this.CheckVisibility();
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x0001BF64 File Offset: 0x0001A164
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

		// Token: 0x060009CD RID: 2509 RVA: 0x0001C028 File Offset: 0x0001A228
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

		// Token: 0x060009CE RID: 2510 RVA: 0x0001C0E8 File Offset: 0x0001A2E8
		private void HealthChanged(bool createDropVisual = true)
		{
			this.HealthBar.MaxAmount = this._maxHealth;
			this.HealthBar.InitialAmount = this.Health;
			if (this._prevHealth > this.Health)
			{
				this.CreateHealthDrop(this.HealthDropContainer, this._prevHealth, this.Health);
			}
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x0001C140 File Offset: 0x0001A340
		private void CheckVisibility()
		{
			bool flag = this.ShowHealthBar;
			if (flag)
			{
				flag = (float)this._health > 0f || this._healtDrops.Count > 0;
			}
			base.IsVisible = flag;
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x060009D0 RID: 2512 RVA: 0x0001C17E File Offset: 0x0001A37E
		// (set) Token: 0x060009D1 RID: 2513 RVA: 0x0001C186 File Offset: 0x0001A386
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

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x060009D2 RID: 2514 RVA: 0x0001C1B7 File Offset: 0x0001A3B7
		// (set) Token: 0x060009D3 RID: 2515 RVA: 0x0001C1BF File Offset: 0x0001A3BF
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

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x060009D4 RID: 2516 RVA: 0x0001C1E4 File Offset: 0x0001A3E4
		// (set) Token: 0x060009D5 RID: 2517 RVA: 0x0001C1EC File Offset: 0x0001A3EC
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

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x060009D6 RID: 2518 RVA: 0x0001C20A File Offset: 0x0001A40A
		// (set) Token: 0x060009D7 RID: 2519 RVA: 0x0001C212 File Offset: 0x0001A412
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

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x060009D8 RID: 2520 RVA: 0x0001C230 File Offset: 0x0001A430
		// (set) Token: 0x060009D9 RID: 2521 RVA: 0x0001C238 File Offset: 0x0001A438
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

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x060009DA RID: 2522 RVA: 0x0001C256 File Offset: 0x0001A456
		// (set) Token: 0x060009DB RID: 2523 RVA: 0x0001C25E File Offset: 0x0001A45E
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

		// Token: 0x04000478 RID: 1144
		private float AnimationDelay = 0.2f;

		// Token: 0x04000479 RID: 1145
		private float AnimationDuration = 0.8f;

		// Token: 0x0400047A RID: 1146
		private List<AgentHealthWidget.HealthDropData> _healtDrops;

		// Token: 0x0400047B RID: 1147
		private int _health;

		// Token: 0x0400047C RID: 1148
		private int _prevHealth = -1;

		// Token: 0x0400047D RID: 1149
		private int _maxHealth;

		// Token: 0x0400047E RID: 1150
		private bool _showHealthBar;

		// Token: 0x0400047F RID: 1151
		private FillBarWidget _healthBar;

		// Token: 0x04000480 RID: 1152
		private Widget _healthDropContainer;

		// Token: 0x04000481 RID: 1153
		private Brush _healthDropBrush;

		// Token: 0x02000190 RID: 400
		public class HealthDropData
		{
			// Token: 0x06001311 RID: 4881 RVA: 0x000343E6 File Offset: 0x000325E6
			public HealthDropData(BrushWidget widget, float lifeTime)
			{
				this.Widget = widget;
				this.LifeTime = lifeTime;
			}

			// Token: 0x040008F9 RID: 2297
			public BrushWidget Widget;

			// Token: 0x040008FA RID: 2298
			public float LifeTime;
		}
	}
}
