using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D8 RID: 216
	public class OrderOfBattleScreenWidget : Widget
	{
		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06000AF0 RID: 2800 RVA: 0x0001E647 File Offset: 0x0001C847
		// (set) Token: 0x06000AF1 RID: 2801 RVA: 0x0001E64F File Offset: 0x0001C84F
		public float AlphaChangeDuration { get; set; } = 0.15f;

		// Token: 0x06000AF2 RID: 2802 RVA: 0x0001E658 File Offset: 0x0001C858
		public OrderOfBattleScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x0001E690 File Offset: 0x0001C890
		protected override void OnLateUpdate(float dt)
		{
			if (this._isTransitioning)
			{
				if (this._alphaChangeTimeElapsed < this.AlphaChangeDuration)
				{
					this._currentAlpha = MathF.Lerp(this._initialAlpha, this._targetAlpha, this._alphaChangeTimeElapsed / this.AlphaChangeDuration, 1E-05f);
					ListPanel leftSideFormations = this.LeftSideFormations;
					if (leftSideFormations != null)
					{
						leftSideFormations.SetGlobalAlphaRecursively(this._currentAlpha);
					}
					ListPanel rightSideFormations = this.RightSideFormations;
					if (rightSideFormations != null)
					{
						rightSideFormations.SetGlobalAlphaRecursively(this._currentAlpha);
					}
					ListPanel commanderPool = this.CommanderPool;
					if (commanderPool != null)
					{
						commanderPool.SetGlobalAlphaRecursively(this._currentAlpha);
					}
					Widget markers = this.Markers;
					if (markers != null)
					{
						markers.SetGlobalAlphaRecursively(this._currentAlpha);
					}
					this._alphaChangeTimeElapsed += dt;
					return;
				}
				this._isTransitioning = false;
			}
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x0001E754 File Offset: 0x0001C954
		protected void OnCameraControlsEnabledChanged()
		{
			this._alphaChangeTimeElapsed = 0f;
			this._targetAlpha = (this.AreCameraControlsEnabled ? this.CameraEnabledAlpha : 1f);
			this._initialAlpha = this._currentAlpha;
			this._isTransitioning = true;
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06000AF5 RID: 2805 RVA: 0x0001E78F File Offset: 0x0001C98F
		// (set) Token: 0x06000AF6 RID: 2806 RVA: 0x0001E797 File Offset: 0x0001C997
		[Editor(false)]
		public bool AreCameraControlsEnabled
		{
			get
			{
				return this._areCameraControlsEnabled;
			}
			set
			{
				if (value != this._areCameraControlsEnabled)
				{
					this._areCameraControlsEnabled = value;
					base.OnPropertyChanged(value, "AreCameraControlsEnabled");
					this.OnCameraControlsEnabledChanged();
				}
			}
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x0001E7BB File Offset: 0x0001C9BB
		// (set) Token: 0x06000AF8 RID: 2808 RVA: 0x0001E7C3 File Offset: 0x0001C9C3
		[Editor(false)]
		public float CameraEnabledAlpha
		{
			get
			{
				return this._cameraEnabledAlpha;
			}
			set
			{
				if (value != this._cameraEnabledAlpha)
				{
					this._cameraEnabledAlpha = value;
					base.OnPropertyChanged(value, "CameraEnabledAlpha");
				}
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06000AF9 RID: 2809 RVA: 0x0001E7E1 File Offset: 0x0001C9E1
		// (set) Token: 0x06000AFA RID: 2810 RVA: 0x0001E7E9 File Offset: 0x0001C9E9
		[Editor(false)]
		public ListPanel LeftSideFormations
		{
			get
			{
				return this._leftSideFormations;
			}
			set
			{
				if (value != this._leftSideFormations)
				{
					this._leftSideFormations = value;
					base.OnPropertyChanged<ListPanel>(value, "LeftSideFormations");
				}
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06000AFB RID: 2811 RVA: 0x0001E807 File Offset: 0x0001CA07
		// (set) Token: 0x06000AFC RID: 2812 RVA: 0x0001E80F File Offset: 0x0001CA0F
		[Editor(false)]
		public ListPanel RightSideFormations
		{
			get
			{
				return this._rightSideFormations;
			}
			set
			{
				if (value != this._rightSideFormations)
				{
					this._rightSideFormations = value;
					base.OnPropertyChanged<ListPanel>(value, "RightSideFormations");
				}
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06000AFD RID: 2813 RVA: 0x0001E82D File Offset: 0x0001CA2D
		// (set) Token: 0x06000AFE RID: 2814 RVA: 0x0001E835 File Offset: 0x0001CA35
		[Editor(false)]
		public ListPanel CommanderPool
		{
			get
			{
				return this._commanderPool;
			}
			set
			{
				if (value != this._commanderPool)
				{
					this._commanderPool = value;
					base.OnPropertyChanged<ListPanel>(value, "CommanderPool");
				}
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06000AFF RID: 2815 RVA: 0x0001E853 File Offset: 0x0001CA53
		// (set) Token: 0x06000B00 RID: 2816 RVA: 0x0001E85B File Offset: 0x0001CA5B
		[Editor(false)]
		public Widget Markers
		{
			get
			{
				return this._markers;
			}
			set
			{
				if (value != this._markers)
				{
					this._markers = value;
					base.OnPropertyChanged<Widget>(value, "Markers");
				}
			}
		}

		// Token: 0x040004FB RID: 1275
		private float _alphaChangeTimeElapsed;

		// Token: 0x040004FC RID: 1276
		private float _initialAlpha = 1f;

		// Token: 0x040004FD RID: 1277
		private float _targetAlpha;

		// Token: 0x040004FE RID: 1278
		private float _currentAlpha = 1f;

		// Token: 0x040004FF RID: 1279
		private bool _isTransitioning;

		// Token: 0x04000500 RID: 1280
		private bool _areCameraControlsEnabled;

		// Token: 0x04000501 RID: 1281
		private float _cameraEnabledAlpha = 0.2f;

		// Token: 0x04000502 RID: 1282
		private ListPanel _leftSideFormations;

		// Token: 0x04000503 RID: 1283
		private ListPanel _rightSideFormations;

		// Token: 0x04000504 RID: 1284
		private ListPanel _commanderPool;

		// Token: 0x04000505 RID: 1285
		private Widget _markers;
	}
}
