using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleHeroDropWidget : ButtonWidget
	{
		public OrderOfBattleHeroDropWidget(UIContext context)
			: base(context)
		{
		}

		protected override bool OnPreviewDrop()
		{
			this.HandleSoundEvent();
			return true;
		}

		protected override void OnClick()
		{
			this.HandleSoundEvent();
			base.OnClick();
		}

		private void HandleSoundEvent()
		{
			switch (this.FormationClass)
			{
			case 0:
				break;
			case 1:
				base.EventFired("Infantry", Array.Empty<object>());
				return;
			case 2:
				base.EventFired("Archers", Array.Empty<object>());
				return;
			case 3:
				base.EventFired("Cavalry", Array.Empty<object>());
				return;
			case 4:
				base.EventFired("HorseArchers", Array.Empty<object>());
				return;
			case 5:
				base.EventFired("InfantryArchers", Array.Empty<object>());
				return;
			case 6:
				base.EventFired("CavalryHorseArchers", Array.Empty<object>());
				break;
			default:
				return;
			}
		}

		protected override bool OnPreviewDragHover()
		{
			return true;
		}

		[DataSourceProperty]
		public int FormationClass
		{
			get
			{
				return this._formationClass;
			}
			set
			{
				if (value != this._formationClass)
				{
					this._formationClass = value;
					base.OnPropertyChanged(value, "FormationClass");
				}
			}
		}

		private int _formationClass;
	}
}
