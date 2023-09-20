using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000023 RID: 35
	public class InitialMenuAnimControllerWidget : Widget
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x00006BBA File Offset: 0x00004DBA
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x00006BC2 File Offset: 0x00004DC2
		public bool IsAnimEnabled { get; set; }

		// Token: 0x060001B8 RID: 440 RVA: 0x00006BCB File Offset: 0x00004DCB
		public InitialMenuAnimControllerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00006BD4 File Offset: 0x00004DD4
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.IsAnimEnabled)
			{
				if (!this._isInitialized)
				{
					Widget optionsList = this.OptionsList;
					bool flag;
					if (optionsList == null)
					{
						flag = false;
					}
					else
					{
						List<Widget> children = optionsList.Children;
						int? num = ((children != null) ? new int?(children.Count) : null);
						int num2 = 0;
						flag = (num.GetValueOrDefault() > num2) & (num != null);
					}
					if (flag)
					{
						this.OptionsList.Children.ForEach(delegate(Widget x)
						{
							x.SetGlobalAlphaRecursively(0f);
						});
						this._totalOptionCount = this.OptionsList.Children.Count;
						this._isInitialized = true;
					}
				}
				if (this._isInitialized && !this._isFinalized && this.OptionsList != null)
				{
					this._timer += dt;
					if (this._timer >= this.InitialWaitTime + (float)this._currentOptionIndex * this.WaitTimeBetweenOptions)
					{
						Widget child = this.OptionsList.GetChild(this._currentOptionIndex);
						if (child != null)
						{
							child.SetState("Activated");
						}
						this._currentOptionIndex++;
					}
					for (int i = 0; i < this._currentOptionIndex; i++)
					{
						float num3 = this.InitialWaitTime + this.WaitTimeBetweenOptions * (float)i;
						float num4 = num3 + this.OptionFadeInTime;
						if (this._timer < num4)
						{
							float num5 = MathF.Clamp((this._timer - num3) / (num4 - num3), 0f, 1f);
							Widget child2 = this.OptionsList.GetChild(i);
							if (child2 != null)
							{
								child2.SetGlobalAlphaRecursively(num5);
							}
						}
					}
					this._isFinalized = this._timer > this.InitialWaitTime + this.WaitTimeBetweenOptions * (float)(this._totalOptionCount - 1) + this.OptionFadeInTime;
				}
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00006DA4 File Offset: 0x00004FA4
		// (set) Token: 0x060001BB RID: 443 RVA: 0x00006DAC File Offset: 0x00004FAC
		[Editor(false)]
		public Widget OptionsList
		{
			get
			{
				return this._optionsList;
			}
			set
			{
				if (this._optionsList != value)
				{
					this._optionsList = value;
					base.OnPropertyChanged<Widget>(value, "OptionsList");
				}
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00006DCA File Offset: 0x00004FCA
		// (set) Token: 0x060001BD RID: 445 RVA: 0x00006DD2 File Offset: 0x00004FD2
		[Editor(false)]
		public float InitialWaitTime
		{
			get
			{
				return this._initialWaitTime;
			}
			set
			{
				if (this._initialWaitTime != value)
				{
					this._initialWaitTime = value;
					base.OnPropertyChanged(value, "InitialWaitTime");
				}
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00006DF0 File Offset: 0x00004FF0
		// (set) Token: 0x060001BF RID: 447 RVA: 0x00006DF8 File Offset: 0x00004FF8
		[Editor(false)]
		public float WaitTimeBetweenOptions
		{
			get
			{
				return this._waitTimeBetweenOptions;
			}
			set
			{
				if (this._waitTimeBetweenOptions != value)
				{
					this._waitTimeBetweenOptions = value;
					base.OnPropertyChanged(value, "WaitTimeBetweenOptions");
				}
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x00006E16 File Offset: 0x00005016
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x00006E1E File Offset: 0x0000501E
		[Editor(false)]
		public float OptionFadeInTime
		{
			get
			{
				return this._optionFadeInTime;
			}
			set
			{
				if (this._optionFadeInTime != value)
				{
					this._optionFadeInTime = value;
					base.OnPropertyChanged(value, "OptionFadeInTime");
				}
			}
		}

		// Token: 0x040000D4 RID: 212
		private bool _isInitialized;

		// Token: 0x040000D5 RID: 213
		private bool _isFinalized;

		// Token: 0x040000D6 RID: 214
		private int _currentOptionIndex;

		// Token: 0x040000D7 RID: 215
		private int _totalOptionCount;

		// Token: 0x040000D8 RID: 216
		private float _timer;

		// Token: 0x040000D9 RID: 217
		private Widget _optionsList;

		// Token: 0x040000DA RID: 218
		private float _initialWaitTime;

		// Token: 0x040000DB RID: 219
		private float _waitTimeBetweenOptions;

		// Token: 0x040000DC RID: 220
		private float _optionFadeInTime;
	}
}
