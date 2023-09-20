using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	// Token: 0x020000DA RID: 218
	public class DuelTargetMarkerListPanel : ListPanel
	{
		// Token: 0x06000B09 RID: 2825 RVA: 0x0001EA2A File Offset: 0x0001CC2A
		public DuelTargetMarkerListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x0001EA34 File Offset: 0x0001CC34
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

		// Token: 0x06000B0B RID: 2827 RVA: 0x0001ED50 File Offset: 0x0001CF50
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

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06000B0C RID: 2828 RVA: 0x0001EDB2 File Offset: 0x0001CFB2
		// (set) Token: 0x06000B0D RID: 2829 RVA: 0x0001EDBA File Offset: 0x0001CFBA
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

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06000B0E RID: 2830 RVA: 0x0001EDDD File Offset: 0x0001CFDD
		// (set) Token: 0x06000B0F RID: 2831 RVA: 0x0001EDE5 File Offset: 0x0001CFE5
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

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06000B10 RID: 2832 RVA: 0x0001EE03 File Offset: 0x0001D003
		// (set) Token: 0x06000B11 RID: 2833 RVA: 0x0001EE0B File Offset: 0x0001D00B
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

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06000B12 RID: 2834 RVA: 0x0001EE29 File Offset: 0x0001D029
		// (set) Token: 0x06000B13 RID: 2835 RVA: 0x0001EE31 File Offset: 0x0001D031
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

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06000B14 RID: 2836 RVA: 0x0001EE4F File Offset: 0x0001D04F
		// (set) Token: 0x06000B15 RID: 2837 RVA: 0x0001EE57 File Offset: 0x0001D057
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

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06000B16 RID: 2838 RVA: 0x0001EE7B File Offset: 0x0001D07B
		// (set) Token: 0x06000B17 RID: 2839 RVA: 0x0001EE83 File Offset: 0x0001D083
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

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0001EEA7 File Offset: 0x0001D0A7
		// (set) Token: 0x06000B19 RID: 2841 RVA: 0x0001EEAF File Offset: 0x0001D0AF
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

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06000B1A RID: 2842 RVA: 0x0001EED3 File Offset: 0x0001D0D3
		// (set) Token: 0x06000B1B RID: 2843 RVA: 0x0001EEDB File Offset: 0x0001D0DB
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

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06000B1C RID: 2844 RVA: 0x0001EEF9 File Offset: 0x0001D0F9
		// (set) Token: 0x06000B1D RID: 2845 RVA: 0x0001EF01 File Offset: 0x0001D101
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

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06000B1E RID: 2846 RVA: 0x0001EF1F File Offset: 0x0001D11F
		// (set) Token: 0x06000B1F RID: 2847 RVA: 0x0001EF27 File Offset: 0x0001D127
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

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06000B20 RID: 2848 RVA: 0x0001EF45 File Offset: 0x0001D145
		// (set) Token: 0x06000B21 RID: 2849 RVA: 0x0001EF4D File Offset: 0x0001D14D
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

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06000B22 RID: 2850 RVA: 0x0001EF6B File Offset: 0x0001D16B
		// (set) Token: 0x06000B23 RID: 2851 RVA: 0x0001EF73 File Offset: 0x0001D173
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

		// Token: 0x04000509 RID: 1289
		private const string DefaultState = "Default";

		// Token: 0x0400050A RID: 1290
		private const string FocusedState = "Focused";

		// Token: 0x0400050B RID: 1291
		private const string TrackedState = "Tracked";

		// Token: 0x0400050C RID: 1292
		private Vec2 _position;

		// Token: 0x0400050D RID: 1293
		private bool _isAgentInScreenBoundaries;

		// Token: 0x0400050E RID: 1294
		private bool _isAvailable;

		// Token: 0x0400050F RID: 1295
		private bool _isTracked;

		// Token: 0x04000510 RID: 1296
		private bool _isAgentFocused;

		// Token: 0x04000511 RID: 1297
		private bool _hasTargetSentDuelRequest;

		// Token: 0x04000512 RID: 1298
		private bool _hasPlayerSentDuelRequest;

		// Token: 0x04000513 RID: 1299
		private int _wSign;

		// Token: 0x04000514 RID: 1300
		private RichTextWidget _actionText;

		// Token: 0x04000515 RID: 1301
		private BrushWidget _background;

		// Token: 0x04000516 RID: 1302
		private BrushWidget _border;

		// Token: 0x04000517 RID: 1303
		private BrushWidget _troopClassBorder;
	}
}
