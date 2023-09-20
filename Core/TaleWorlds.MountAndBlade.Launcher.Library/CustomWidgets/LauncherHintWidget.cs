using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	public class LauncherHintWidget : Widget
	{
		private float TooltipOffset
		{
			get
			{
				return 30f;
			}
		}

		public LauncherHintWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateAlpha(dt);
			if (this.IsActive)
			{
				if (!this._prevIsActive)
				{
					this._frame = 0;
				}
				this.UpdatePosition();
			}
			this._prevIsActive = this.IsActive;
		}

		private void UpdatePosition()
		{
			Vector2 vector;
			if (this._frame < 3)
			{
				this._tooltipPosition = base.EventManager.MousePosition;
				vector = new Vector2(-2000f, -2000f);
			}
			else
			{
				vector = this._tooltipPosition;
			}
			this._frame++;
			float num = 0f;
			HorizontalAlignment horizontalAlignment;
			if ((double)vector.X < (double)base.EventManager.PageSize.X * 0.5)
			{
				horizontalAlignment = HorizontalAlignment.Left;
				num = this.TooltipOffset;
			}
			else
			{
				horizontalAlignment = HorizontalAlignment.Right;
				num -= 0f;
				vector = new Vector2(-(base.EventManager.PageSize.X - vector.X), vector.Y);
			}
			VerticalAlignment verticalAlignment;
			float num2;
			if ((double)vector.Y < (double)base.EventManager.PageSize.Y * 0.5)
			{
				verticalAlignment = VerticalAlignment.Top;
				num2 = this.TooltipOffset;
			}
			else
			{
				verticalAlignment = VerticalAlignment.Bottom;
				num2 = 0f;
				vector = new Vector2(vector.X, -(base.EventManager.PageSize.Y - vector.Y));
			}
			vector += new Vector2(num, num2);
			if (this._frame > 3)
			{
				if (base.Size.Y > base.EventManager.PageSize.Y)
				{
					verticalAlignment = VerticalAlignment.Center;
					vector = new Vector2(vector.X, 0f);
				}
				else
				{
					if (verticalAlignment == VerticalAlignment.Top && vector.Y + base.Size.Y > base.EventManager.PageSize.Y)
					{
						vector += new Vector2(0f, -(vector.Y + base.Size.Y - base.EventManager.PageSize.Y));
					}
					if (verticalAlignment == VerticalAlignment.Bottom && vector.Y - base.Size.Y + base.EventManager.PageSize.Y < 0f)
					{
						vector += new Vector2(0f, -(vector.Y - base.Size.Y + base.EventManager.PageSize.Y));
					}
				}
			}
			base.HorizontalAlignment = horizontalAlignment;
			base.VerticalAlignment = verticalAlignment;
			base.ScaledPositionXOffset = vector.X;
			base.ScaledPositionYOffset = vector.Y;
		}

		private void UpdateAlpha(float dt)
		{
			float num;
			if (this.IsActive)
			{
				num = 1f;
			}
			else
			{
				num = 0f;
			}
			float num2 = MathF.Lerp(base.AlphaFactor, num, dt * 20f, 1E-05f);
			this.SetGlobalAlphaRecursively(num2);
		}

		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
				}
			}
		}

		private int _frame;

		private bool _prevIsActive;

		private Vector2 _tooltipPosition;

		private bool _isActive;
	}
}
