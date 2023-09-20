using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	// Token: 0x02000067 RID: 103
	public class OrderSiegeMachineItemButtonWidget : ButtonWidget
	{
		// Token: 0x06000573 RID: 1395 RVA: 0x0001057B File Offset: 0x0000E77B
		public OrderSiegeMachineItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x00010592 File Offset: 0x0000E792
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isVisualsDirty)
			{
				this.MachineIconWidgetChanged();
				this.UpdateRemainingCount();
				this.UpdateMachineIcon();
				this._isVisualsDirty = false;
			}
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x000105BC File Offset: 0x0000E7BC
		private void MachineIconWidgetChanged()
		{
			if (this.MachineIconWidget == null)
			{
				return;
			}
			this.MachineIconWidget.RegisterBrushStatesOfWidget();
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x000105D4 File Offset: 0x0000E7D4
		private void UpdateMachineIcon()
		{
			if (this.MachineIconWidget == null)
			{
				return;
			}
			this._isRemainingCountVisible = true;
			string machineClass = this.MachineClass;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(machineClass);
			if (num <= 1056090379U)
			{
				if (num <= 390431385U)
				{
					if (num <= 6339497U)
					{
						if (num != 0U)
						{
							if (num == 6339497U)
							{
								if (machineClass == "ladder")
								{
									this.MachineIconWidget.SetState("Ladder");
									return;
								}
							}
						}
						else if (machineClass != null)
						{
						}
					}
					else if (num != 354578048U)
					{
						if (num == 390431385U)
						{
							if (machineClass == "bricole")
							{
								this.MachineIconWidget.SetState("Bricole");
								return;
							}
						}
					}
					else if (machineClass == "Mangonel")
					{
						this.MachineIconWidget.SetState("Mangonel");
						return;
					}
				}
				else if (num <= 729368230U)
				{
					if (num != 616782878U)
					{
						if (num == 729368230U)
						{
							if (machineClass == "siege_tower_level1")
							{
								this.MachineIconWidget.SetState("SiegeTower");
								return;
							}
						}
					}
					else if (machineClass == "improved_ram")
					{
						this.MachineIconWidget.SetState("ImprovedRam");
						return;
					}
				}
				else if (num != 808481256U)
				{
					if (num == 1056090379U)
					{
						if (machineClass == "preparations")
						{
							this.MachineIconWidget.SetState("Preparations");
							return;
						}
					}
				}
				else if (machineClass == "fire_ballista")
				{
					this.MachineIconWidget.SetState("FireBallista");
					return;
				}
			}
			else if (num <= 1839032341U)
			{
				if (num <= 1748194790U)
				{
					if (num != 1241455715U)
					{
						if (num == 1748194790U)
						{
							if (machineClass == "fire_catapult")
							{
								this.MachineIconWidget.SetState("FireCatapult");
								return;
							}
						}
					}
					else if (machineClass == "ram")
					{
						this.MachineIconWidget.SetState("Ram");
						return;
					}
				}
				else if (num != 1820818168U)
				{
					if (num == 1839032341U)
					{
						if (machineClass == "trebuchet")
						{
							this.MachineIconWidget.SetState("Trebuchet");
							return;
						}
					}
				}
				else if (machineClass == "fire_onager")
				{
					this.MachineIconWidget.SetState("FireOnager");
					return;
				}
			}
			else if (num <= 1898442385U)
			{
				if (num != 1844264380U)
				{
					if (num == 1898442385U)
					{
						if (machineClass == "catapult")
						{
							this.MachineIconWidget.SetState("Catapult");
							return;
						}
					}
				}
				else if (machineClass == "FireMangonel")
				{
					this.MachineIconWidget.SetState("FireMangonel");
					return;
				}
			}
			else if (num != 2166136261U)
			{
				if (num != 2806198843U)
				{
					if (num == 4036530155U)
					{
						if (machineClass == "ballista")
						{
							this.MachineIconWidget.SetState("Ballista");
							return;
						}
					}
				}
				else if (machineClass == "onager")
				{
					this.MachineIconWidget.SetState("Onager");
					return;
				}
			}
			else if (machineClass != null && machineClass.Length != 0)
			{
			}
			this.MachineIconWidget.SetState("None");
			this._isRemainingCountVisible = false;
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x00010984 File Offset: 0x0000EB84
		private void UpdateRemainingCount()
		{
			if (this.RemainingCountWidget == null)
			{
				return;
			}
			base.IsDisabled = this.RemainingCount == 0;
			this.RemainingCountWidget.IsVisible = this._isRemainingCountVisible;
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x000109AF File Offset: 0x0000EBAF
		// (set) Token: 0x06000579 RID: 1401 RVA: 0x000109B7 File Offset: 0x0000EBB7
		[Editor(false)]
		public int RemainingCount
		{
			get
			{
				return this._remainingCount;
			}
			set
			{
				if (this._remainingCount != value)
				{
					this._remainingCount = value;
					base.OnPropertyChanged(value, "RemainingCount");
					this._isVisualsDirty = true;
				}
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x000109DC File Offset: 0x0000EBDC
		// (set) Token: 0x0600057B RID: 1403 RVA: 0x000109E4 File Offset: 0x0000EBE4
		[Editor(false)]
		public TextWidget RemainingCountWidget
		{
			get
			{
				return this._remainingCountWidget;
			}
			set
			{
				if (this._remainingCountWidget != value)
				{
					this._remainingCountWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "RemainingCountWidget");
					this._isVisualsDirty = true;
				}
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x00010A09 File Offset: 0x0000EC09
		// (set) Token: 0x0600057D RID: 1405 RVA: 0x00010A11 File Offset: 0x0000EC11
		[Editor(false)]
		public string MachineClass
		{
			get
			{
				return this._machineClass;
			}
			set
			{
				if (this._machineClass != value)
				{
					this._machineClass = value;
					base.OnPropertyChanged<string>(value, "MachineClass");
					this._isVisualsDirty = true;
				}
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x0600057E RID: 1406 RVA: 0x00010A3B File Offset: 0x0000EC3B
		// (set) Token: 0x0600057F RID: 1407 RVA: 0x00010A43 File Offset: 0x0000EC43
		[Editor(false)]
		public Widget MachineIconWidget
		{
			get
			{
				return this._machineIconWidget;
			}
			set
			{
				if (this._machineIconWidget != value)
				{
					this._machineIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "MachineIconWidget");
					this._isVisualsDirty = true;
				}
			}
		}

		// Token: 0x0400025B RID: 603
		private bool _isRemainingCountVisible = true;

		// Token: 0x0400025C RID: 604
		private bool _isVisualsDirty = true;

		// Token: 0x0400025D RID: 605
		private int _remainingCount;

		// Token: 0x0400025E RID: 606
		private TextWidget _remainingCountWidget;

		// Token: 0x0400025F RID: 607
		private string _machineClass;

		// Token: 0x04000260 RID: 608
		private Widget _machineIconWidget;
	}
}
