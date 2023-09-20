using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	public class DuelTargetMarkerListPanel : ListPanel
	{
		public DuelTargetMarkerListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			if (!this.IsAvailable)
			{
				base.IsVisible = false;
				return;
			}
			float x = base.Context.EventManager.PageSize.X;
			float y = base.Context.EventManager.PageSize.Y;
			Vec2 vec = this.Position;
			if (this.WSign > 0 && vec.x - base.Size.X / 2f > 0f && vec.x + base.Size.X / 2f < base.Context.EventManager.PageSize.X && vec.y > 0f && vec.y + base.Size.Y < base.Context.EventManager.PageSize.Y)
			{
				base.ScaledPositionXOffset = vec.x - base.Size.X / 2f;
				base.ScaledPositionYOffset = vec.y - base.Size.Y - 20f;
				this._actionText.ScaledPositionXOffset = base.ScaledPositionXOffset;
				this._actionText.ScaledPositionYOffset = base.ScaledPositionYOffset + base.Size.Y;
				base.IsVisible = true;
				return;
			}
			if (this.IsTracked)
			{
				Vec2 vec2 = new Vec2(base.Context.EventManager.PageSize.X / 2f, base.Context.EventManager.PageSize.Y / 2f);
				vec -= vec2;
				if (this.WSign < 0)
				{
					vec *= -1f;
				}
				float num = Mathf.Atan2(vec.y, vec.x) - 1.5707964f;
				float num2 = Mathf.Cos(num);
				float num3 = Mathf.Sin(num);
				vec = vec2 + new Vec2(num3 * 150f, num2 * 150f);
				float num4 = num2 / num3;
				Vec2 vec3 = vec2 * 1f;
				vec = ((num2 > 0f) ? new Vec2(-vec3.y / num4, vec2.y) : new Vec2(vec3.y / num4, -vec2.y));
				if (vec.x > vec3.x)
				{
					vec = new Vec2(vec3.x, -vec3.x * num4);
				}
				else if (vec.x < -vec3.x)
				{
					vec = new Vec2(-vec3.x, vec3.x * num4);
				}
				vec += vec2;
				base.ScaledPositionXOffset = Mathf.Clamp(vec.x - base.Size.X / 2f, 0f, x - base.Size.X);
				base.ScaledPositionYOffset = Mathf.Clamp(vec.y - base.Size.Y, 0f, y - base.Size.Y);
				base.IsVisible = true;
				return;
			}
			base.IsVisible = false;
		}

		private void UpdateChildrenFocusStates()
		{
			string text = (this.HasTargetSentDuelRequest ? "Tracked" : ((this.HasPlayerSentDuelRequest || this.IsAgentFocused) ? "Focused" : "Default"));
			this.Background.SetState(text);
			this.Border.SetState(text);
			BrushWidget troopClassBorder = this.TroopClassBorder;
			if (troopClassBorder == null)
			{
				return;
			}
			troopClassBorder.SetState(text);
		}

		[Editor(false)]
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (value != this._position)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		[Editor(false)]
		public bool IsAgentInScreenBoundaries
		{
			get
			{
				return this._isAgentInScreenBoundaries;
			}
			set
			{
				if (value != this._isAgentInScreenBoundaries)
				{
					this._isAgentInScreenBoundaries = value;
					base.OnPropertyChanged(value, "IsAgentInScreenBoundaries");
				}
			}
		}

		[Editor(false)]
		public bool IsAvailable
		{
			get
			{
				return this._isAvailable;
			}
			set
			{
				if (value != this._isAvailable)
				{
					this._isAvailable = value;
					base.OnPropertyChanged(value, "IsAvailable");
				}
			}
		}

		[Editor(false)]
		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (value != this._isTracked)
				{
					this._isTracked = value;
					base.OnPropertyChanged(value, "IsTracked");
				}
			}
		}

		[Editor(false)]
		public bool IsAgentFocused
		{
			get
			{
				return this._isAgentFocused;
			}
			set
			{
				if (value != this._isAgentFocused)
				{
					this._isAgentFocused = value;
					base.OnPropertyChanged(value, "IsAgentFocused");
					this.UpdateChildrenFocusStates();
				}
			}
		}

		[Editor(false)]
		public bool HasTargetSentDuelRequest
		{
			get
			{
				return this._hasTargetSentDuelRequest;
			}
			set
			{
				if (value != this._hasTargetSentDuelRequest)
				{
					this._hasTargetSentDuelRequest = value;
					base.OnPropertyChanged(value, "HasTargetSentDuelRequest");
					this.UpdateChildrenFocusStates();
				}
			}
		}

		[Editor(false)]
		public bool HasPlayerSentDuelRequest
		{
			get
			{
				return this._hasPlayerSentDuelRequest;
			}
			set
			{
				if (value != this._hasPlayerSentDuelRequest)
				{
					this._hasPlayerSentDuelRequest = value;
					base.OnPropertyChanged(value, "HasPlayerSentDuelRequest");
					this.UpdateChildrenFocusStates();
				}
			}
		}

		[Editor(false)]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (this._wSign != value)
				{
					this._wSign = value;
					base.OnPropertyChanged(value, "WSign");
				}
			}
		}

		[Editor(false)]
		public RichTextWidget ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (value != this._actionText)
				{
					this._actionText = value;
					base.OnPropertyChanged<RichTextWidget>(value, "ActionText");
				}
			}
		}

		[Editor(false)]
		public BrushWidget Background
		{
			get
			{
				return this._background;
			}
			set
			{
				if (value != this._background)
				{
					this._background = value;
					base.OnPropertyChanged<BrushWidget>(value, "Background");
				}
			}
		}

		[Editor(false)]
		public BrushWidget Border
		{
			get
			{
				return this._border;
			}
			set
			{
				if (value != this._border)
				{
					this._border = value;
					base.OnPropertyChanged<BrushWidget>(value, "Border");
				}
			}
		}

		[Editor(false)]
		public BrushWidget TroopClassBorder
		{
			get
			{
				return this._troopClassBorder;
			}
			set
			{
				if (value != this._troopClassBorder)
				{
					this._troopClassBorder = value;
					base.OnPropertyChanged<BrushWidget>(value, "TroopClassBorder");
				}
			}
		}

		private const string DefaultState = "Default";

		private const string FocusedState = "Focused";

		private const string TrackedState = "Tracked";

		private Vec2 _position;

		private bool _isAgentInScreenBoundaries;

		private bool _isAvailable;

		private bool _isTracked;

		private bool _isAgentFocused;

		private bool _hasTargetSentDuelRequest;

		private bool _hasPlayerSentDuelRequest;

		private int _wSign;

		private RichTextWidget _actionText;

		private BrushWidget _background;

		private BrushWidget _border;

		private BrushWidget _troopClassBorder;
	}
}
