using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class MissionSiegeEngineMarkerWidget : Widget
	{
		public SliderWidget Slider { get; set; }

		public BrushWidget MachineIconParent { get; set; }

		public Brush EnemyBrush { get; set; }

		public Brush AllyBrush { get; set; }

		public Vec2 ScreenPosition { get; set; }

		public MissionSiegeEngineMarkerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			base.ScaledPositionXOffset = this.ScreenPosition.x - base.Size.X / 2f;
			base.ScaledPositionYOffset = this.ScreenPosition.y;
			float num = (this.IsActive ? 0.65f : 0f);
			float num2 = MathF.Lerp(base.AlphaFactor, num, dt * 10f, 1E-05f);
			this.SetGlobalAlphaRecursively(num2);
			if (!this._isBrushChanged)
			{
				this.MachineIconParent.Brush = (this.IsEnemy ? this.EnemyBrush : this.AllyBrush);
				this._isBrushChanged = true;
			}
			this.UpdateColorOfSlider();
		}

		private void SetMachineTypeIcon(string machineType)
		{
			string text = "SPGeneral\\MapSiege\\" + machineType;
			this.MachineTypeIconWidget.Sprite = base.Context.SpriteData.GetSprite(text);
		}

		private void UpdateColorOfSlider()
		{
			(this.Slider.Filler as BrushWidget).Brush.Color = Color.Lerp(this._emptyColor, this._fullColor, this.Slider.ValueFloat / this.Slider.MaxValueFloat);
		}

		public bool IsEnemy
		{
			get
			{
				return this._isEnemy;
			}
			set
			{
				if (this._isEnemy != value)
				{
					this._isEnemy = value;
				}
			}
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (this._isActive != value)
				{
					this._isActive = value;
				}
			}
		}

		public string EngineType
		{
			get
			{
				return this._engineType;
			}
			set
			{
				if (this._engineType != value)
				{
					this._engineType = value;
					this.SetMachineTypeIcon(value);
				}
			}
		}

		public Widget MachineTypeIconWidget
		{
			get
			{
				return this._machineTypeIconWidget;
			}
			set
			{
				if (this._machineTypeIconWidget != value)
				{
					this._machineTypeIconWidget = value;
				}
			}
		}

		private Color _fullColor = new Color(0.2784314f, 0.9882353f, 0.44313726f, 1f);

		private Color _emptyColor = new Color(0.9882353f, 0.2784314f, 0.2784314f, 1f);

		private bool _isBrushChanged;

		private bool _isEnemy;

		private bool _isActive;

		private Widget _machineTypeIconWidget;

		private string _engineType;
	}
}
