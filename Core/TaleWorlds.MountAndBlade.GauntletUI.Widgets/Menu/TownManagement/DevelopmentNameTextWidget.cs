using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	public class DevelopmentNameTextWidget : TextWidget
	{
		public DevelopmentNameTextWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this.IsInQueue)
			{
				this.SetState(base.ParentWidget.CurrentState);
			}
			else
			{
				this.SetState("Selected");
			}
			this.HandleAnim(dt);
		}

		private void HandleAnim(float dt)
		{
			switch (this._currentState)
			{
			case DevelopmentNameTextWidget.AnimState.Start:
				this._currentAlphaTarget = 0f;
				this._currentState = DevelopmentNameTextWidget.AnimState.DownName;
				break;
			case DevelopmentNameTextWidget.AnimState.DownName:
				if ((double)base.ReadOnlyBrush.TextAlphaFactor < 0.01)
				{
					this._currentAlphaTarget = 1f;
					base.Text = this.MaxText;
					this._currentState = DevelopmentNameTextWidget.AnimState.UpMax;
				}
				break;
			case DevelopmentNameTextWidget.AnimState.UpMax:
				if ((double)base.ReadOnlyBrush.TextAlphaFactor > 0.99)
				{
					this._currentAlphaTarget = 0f;
					this._currentState = DevelopmentNameTextWidget.AnimState.StayMax;
					this._stayMaxTotalTime = 0f;
				}
				break;
			case DevelopmentNameTextWidget.AnimState.StayMax:
				this._stayMaxTotalTime += dt;
				if (this._stayMaxTotalTime >= this.MaxTextStayTime)
				{
					this._currentAlphaTarget = 0f;
					this._currentState = DevelopmentNameTextWidget.AnimState.DownMax;
				}
				break;
			case DevelopmentNameTextWidget.AnimState.DownMax:
				if ((double)base.ReadOnlyBrush.TextAlphaFactor < 0.01)
				{
					this._currentAlphaTarget = 1f;
					this._currentState = DevelopmentNameTextWidget.AnimState.UpName;
					base.Text = this.NameText;
				}
				break;
			case DevelopmentNameTextWidget.AnimState.UpName:
				if ((double)base.ReadOnlyBrush.TextAlphaFactor > 0.99)
				{
					this._currentState = DevelopmentNameTextWidget.AnimState.Idle;
					base.Text = this.NameText;
				}
				break;
			}
			if (this._currentState != DevelopmentNameTextWidget.AnimState.Idle && this._currentState != DevelopmentNameTextWidget.AnimState.StayMax)
			{
				base.Brush.TextAlphaFactor = Mathf.Lerp(base.ReadOnlyBrush.TextAlphaFactor, this._currentAlphaTarget, dt * 15f);
			}
		}

		public void StartMaxTextAnimation()
		{
			DevelopmentNameTextWidget.AnimState currentState = this._currentState;
			if (currentState > DevelopmentNameTextWidget.AnimState.StayMax)
			{
				int num = currentState - DevelopmentNameTextWidget.AnimState.DownMax;
				this._currentState = DevelopmentNameTextWidget.AnimState.Start;
			}
		}

		[Editor(false)]
		public string MaxText
		{
			get
			{
				return this._maxText;
			}
			set
			{
				if (this._maxText != value)
				{
					this._maxText = value;
					base.OnPropertyChanged<string>(value, "MaxText");
				}
			}
		}

		[Editor(false)]
		public float MaxTextStayTime
		{
			get
			{
				return this._maxTextStayTime;
			}
			set
			{
				if (this._maxTextStayTime != value)
				{
					this._maxTextStayTime = value;
					base.OnPropertyChanged(value, "MaxTextStayTime");
				}
			}
		}

		[Editor(false)]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (this._nameText != value)
				{
					this._nameText = value;
					base.OnPropertyChanged<string>(value, "NameText");
					base.Text = this.NameText;
				}
			}
		}

		[Editor(false)]
		public bool IsInQueue
		{
			get
			{
				return this._isInQueue;
			}
			set
			{
				if (this._isInQueue != value)
				{
					this._isInQueue = value;
					base.OnPropertyChanged(value, "IsInQueue");
				}
			}
		}

		private float _currentAlphaTarget;

		private float _stayMaxTotalTime;

		private DevelopmentNameTextWidget.AnimState _currentState = DevelopmentNameTextWidget.AnimState.Idle;

		private float _maxTextStayTime = 1f;

		private bool _isInQueue;

		private string _maxText;

		private string _nameText;

		public enum AnimState
		{
			Start,
			DownName,
			UpMax,
			StayMax,
			DownMax,
			UpName,
			Idle
		}
	}
}
