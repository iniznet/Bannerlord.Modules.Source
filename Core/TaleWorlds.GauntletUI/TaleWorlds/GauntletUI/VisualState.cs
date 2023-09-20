using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	public class VisualState
	{
		public string State { get; private set; }

		public float TransitionDuration
		{
			get
			{
				return this._transitionDuration;
			}
			set
			{
				this._transitionDuration = value;
				this.GotTransitionDuration = true;
			}
		}

		public float PositionXOffset
		{
			get
			{
				return this._positionXOffset;
			}
			set
			{
				this._positionXOffset = value;
				this.GotPositionXOffset = true;
			}
		}

		public float PositionYOffset
		{
			get
			{
				return this._positionYOffset;
			}
			set
			{
				this._positionYOffset = value;
				this.GotPositionYOffset = true;
			}
		}

		public float SuggestedWidth
		{
			get
			{
				return this._suggestedWidth;
			}
			set
			{
				this._suggestedWidth = value;
				this.GotSuggestedWidth = true;
			}
		}

		public float SuggestedHeight
		{
			get
			{
				return this._suggestedHeight;
			}
			set
			{
				this._suggestedHeight = value;
				this.GotSuggestedHeight = true;
			}
		}

		public float MarginTop
		{
			get
			{
				return this._marginTop;
			}
			set
			{
				this._marginTop = value;
				this.GotMarginTop = true;
			}
		}

		public float MarginBottom
		{
			get
			{
				return this._marginBottom;
			}
			set
			{
				this._marginBottom = value;
				this.GotMarginBottom = true;
			}
		}

		public float MarginLeft
		{
			get
			{
				return this._marginLeft;
			}
			set
			{
				this._marginLeft = value;
				this.GotMarginLeft = true;
			}
		}

		public float MarginRight
		{
			get
			{
				return this._marginRight;
			}
			set
			{
				this._marginRight = value;
				this.GotMarginRight = true;
			}
		}

		public bool GotTransitionDuration { get; private set; }

		public bool GotPositionXOffset { get; private set; }

		public bool GotPositionYOffset { get; private set; }

		public bool GotSuggestedWidth { get; private set; }

		public bool GotSuggestedHeight { get; private set; }

		public bool GotMarginTop { get; private set; }

		public bool GotMarginBottom { get; private set; }

		public bool GotMarginLeft { get; private set; }

		public bool GotMarginRight { get; private set; }

		public VisualState(string state)
		{
			this.State = state;
		}

		public void FillFromWidget(Widget widget)
		{
			this.PositionXOffset = widget.PositionXOffset;
			this.PositionYOffset = widget.PositionYOffset;
			this.SuggestedWidth = widget.SuggestedWidth;
			this.SuggestedHeight = widget.SuggestedHeight;
			this.MarginTop = widget.MarginTop;
			this.MarginBottom = widget.MarginBottom;
			this.MarginLeft = widget.MarginLeft;
			this.MarginRight = widget.MarginRight;
		}

		private float _transitionDuration;

		private float _positionXOffset;

		private float _positionYOffset;

		private float _suggestedWidth;

		private float _suggestedHeight;

		private float _marginTop;

		private float _marginBottom;

		private float _marginLeft;

		private float _marginRight;
	}
}
