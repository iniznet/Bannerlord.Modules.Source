using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleScreenWidget : Widget
	{
		public float AlphaChangeDuration { get; set; } = 0.15f;

		public OrderOfBattleScreenWidget(UIContext context)
			: base(context)
		{
		}

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

		protected void OnCameraControlsEnabledChanged()
		{
			this._alphaChangeTimeElapsed = 0f;
			this._targetAlpha = (this.AreCameraControlsEnabled ? this.CameraEnabledAlpha : 1f);
			this._initialAlpha = this._currentAlpha;
			this._isTransitioning = true;
		}

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

		private float _alphaChangeTimeElapsed;

		private float _initialAlpha = 1f;

		private float _targetAlpha;

		private float _currentAlpha = 1f;

		private bool _isTransitioning;

		private bool _areCameraControlsEnabled;

		private float _cameraEnabledAlpha = 0.2f;

		private ListPanel _leftSideFormations;

		private ListPanel _rightSideFormations;

		private ListPanel _commanderPool;

		private Widget _markers;
	}
}
