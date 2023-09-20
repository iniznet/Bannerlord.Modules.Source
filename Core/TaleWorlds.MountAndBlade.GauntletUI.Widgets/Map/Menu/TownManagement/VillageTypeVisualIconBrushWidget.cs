using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Menu.TownManagement
{
	public class VillageTypeVisualIconBrushWidget : BrushWidget
	{
		public VillageTypeVisualIconBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				switch (this.VillageType)
				{
				case 0:
					this.SetState("Default");
					break;
				case 1:
					this.SetState("EuropeHorseRanch");
					break;
				case 2:
					this.SetState("BattanianHorseRanch");
					break;
				case 3:
					this.SetState("SteppeHorseRanch");
					break;
				case 4:
					this.SetState("DesertHorseRanch");
					break;
				case 5:
					this.SetState("WheatFarm");
					break;
				case 6:
					this.SetState("Lumberjack");
					break;
				case 7:
					this.SetState("ClayMine");
					break;
				case 8:
					this.SetState("SaltMine");
					break;
				case 9:
					this.SetState("IronMine");
					break;
				case 10:
					this.SetState("Fisherman");
					break;
				case 11:
					this.SetState("CattleRange");
					break;
				case 12:
					this.SetState("SheepFarm");
					break;
				case 13:
					this.SetState("VineYard");
					break;
				case 14:
					this.SetState("FlaxPlant");
					break;
				case 15:
					this.SetState("DateFarm");
					break;
				case 16:
					this.SetState("OliveTrees");
					break;
				case 17:
					this.SetState("SilkPlant");
					break;
				case 18:
					this.SetState("SilverMine");
					break;
				default:
					Debug.FailedAssert("No workshop visual with this type: " + this.VillageType, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Map\\Menu\\TownManagement\\VillageTypeVisualIconBrushWidget.cs", "OnLateUpdate", 103);
					this.SetState("Default");
					break;
				}
				this._initialized = true;
			}
		}

		[Editor(false)]
		public int VillageType
		{
			get
			{
				return this._villageType;
			}
			set
			{
				if (this._villageType != value)
				{
					this._villageType = value;
					base.OnPropertyChanged(value, "VillageType");
				}
			}
		}

		private bool _initialized;

		private int _villageType;
	}
}
