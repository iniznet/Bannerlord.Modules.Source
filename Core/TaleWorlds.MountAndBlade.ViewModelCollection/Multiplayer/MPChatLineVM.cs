using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	public class MPChatLineVM : ViewModel
	{
		public MPChatLineVM(string chatLine, Color color, string category)
		{
			this.ChatLine = chatLine;
			this.Color = color;
			this.Alpha = 1f;
			this.Category = category;
		}

		public void HandleFading(float dt)
		{
			this._timeSinceCreation += dt;
			this.RefreshAlpha();
		}

		private void RefreshAlpha()
		{
			if (this._forcedVisible)
			{
				this.Alpha = 1f;
				return;
			}
			this.Alpha = this.GetActualAlpha();
		}

		public void ForceInvisible()
		{
			this._timeSinceCreation = 10.5f;
			this.Alpha = 0f;
		}

		private float GetActualAlpha()
		{
			if (this._timeSinceCreation >= 10f)
			{
				return MBMath.ClampFloat(1f - (this._timeSinceCreation - 10f) / 0.5f, 0f, 1f);
			}
			return 1f;
		}

		public void ToggleForceVisible(bool visible)
		{
			this._forcedVisible = visible;
			this.RefreshAlpha();
		}

		[DataSourceProperty]
		public string ChatLine
		{
			get
			{
				return this._chatLine;
			}
			set
			{
				if (this._chatLine != value)
				{
					this._chatLine = value;
					base.OnPropertyChangedWithValue<string>(value, "ChatLine");
				}
			}
		}

		[DataSourceProperty]
		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (this._color != value)
				{
					this._color = value;
					base.OnPropertyChangedWithValue(value, "Color");
				}
			}
		}

		[DataSourceProperty]
		public float Alpha
		{
			get
			{
				return this._alpha;
			}
			set
			{
				if (this._alpha != value)
				{
					this._alpha = value;
					base.OnPropertyChangedWithValue(value, "Alpha");
				}
			}
		}

		[DataSourceProperty]
		public string Category
		{
			get
			{
				return this._category;
			}
			set
			{
				if (this._category != value)
				{
					this._category = value;
					base.OnPropertyChangedWithValue<string>(value, "Category");
				}
			}
		}

		private bool _forcedVisible;

		private string _category;

		private const float ChatVisibilityDuration = 10f;

		private const float ChatFadeOutDuration = 0.5f;

		private float _timeSinceCreation;

		private string _chatLine;

		private Color _color;

		private float _alpha;
	}
}
