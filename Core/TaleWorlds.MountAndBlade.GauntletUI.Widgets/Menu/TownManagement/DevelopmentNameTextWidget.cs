using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000EC RID: 236
	public class DevelopmentNameTextWidget : TextWidget
	{
		// Token: 0x06000C4B RID: 3147 RVA: 0x000227FD File Offset: 0x000209FD
		public DevelopmentNameTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x00022818 File Offset: 0x00020A18
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

		// Token: 0x06000C4D RID: 3149 RVA: 0x00022850 File Offset: 0x00020A50
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

		// Token: 0x06000C4E RID: 3150 RVA: 0x000229E8 File Offset: 0x00020BE8
		public void StartMaxTextAnimation()
		{
			DevelopmentNameTextWidget.AnimState currentState = this._currentState;
			if (currentState > DevelopmentNameTextWidget.AnimState.StayMax)
			{
				int num = currentState - DevelopmentNameTextWidget.AnimState.DownMax;
				this._currentState = DevelopmentNameTextWidget.AnimState.Start;
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06000C4F RID: 3151 RVA: 0x00022A0D File Offset: 0x00020C0D
		// (set) Token: 0x06000C50 RID: 3152 RVA: 0x00022A15 File Offset: 0x00020C15
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

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06000C51 RID: 3153 RVA: 0x00022A38 File Offset: 0x00020C38
		// (set) Token: 0x06000C52 RID: 3154 RVA: 0x00022A40 File Offset: 0x00020C40
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

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06000C53 RID: 3155 RVA: 0x00022A5E File Offset: 0x00020C5E
		// (set) Token: 0x06000C54 RID: 3156 RVA: 0x00022A66 File Offset: 0x00020C66
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

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06000C55 RID: 3157 RVA: 0x00022A95 File Offset: 0x00020C95
		// (set) Token: 0x06000C56 RID: 3158 RVA: 0x00022A9D File Offset: 0x00020C9D
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

		// Token: 0x040005AB RID: 1451
		private float _currentAlphaTarget;

		// Token: 0x040005AC RID: 1452
		private float _stayMaxTotalTime;

		// Token: 0x040005AD RID: 1453
		private DevelopmentNameTextWidget.AnimState _currentState = DevelopmentNameTextWidget.AnimState.Idle;

		// Token: 0x040005AE RID: 1454
		private float _maxTextStayTime = 1f;

		// Token: 0x040005AF RID: 1455
		private bool _isInQueue;

		// Token: 0x040005B0 RID: 1456
		private string _maxText;

		// Token: 0x040005B1 RID: 1457
		private string _nameText;

		// Token: 0x02000193 RID: 403
		public enum AnimState
		{
			// Token: 0x04000902 RID: 2306
			Start,
			// Token: 0x04000903 RID: 2307
			DownName,
			// Token: 0x04000904 RID: 2308
			UpMax,
			// Token: 0x04000905 RID: 2309
			StayMax,
			// Token: 0x04000906 RID: 2310
			DownMax,
			// Token: 0x04000907 RID: 2311
			UpName,
			// Token: 0x04000908 RID: 2312
			Idle
		}
	}
}
