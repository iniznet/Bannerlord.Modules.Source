using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	public class OrderSiegeMachineItemButtonWidget : ButtonWidget
	{
		public OrderSiegeMachineItemButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		private void MachineIconWidgetChanged()
		{
			if (this.MachineIconWidget == null)
			{
				return;
			}
			this.MachineIconWidget.RegisterBrushStatesOfWidget();
		}

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

		private void UpdateRemainingCount()
		{
			if (this.RemainingCountWidget == null)
			{
				return;
			}
			base.IsDisabled = this.RemainingCount == 0;
			this.RemainingCountWidget.IsVisible = this._isRemainingCountVisible;
		}

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

		private bool _isRemainingCountVisible = true;

		private bool _isVisualsDirty = true;

		private int _remainingCount;

		private TextWidget _remainingCountWidget;

		private string _machineClass;

		private Widget _machineIconWidget;
	}
}
