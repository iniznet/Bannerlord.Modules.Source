using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Menu.TownManagement
{
	// Token: 0x02000103 RID: 259
	public class VillageTypeVisualIconBrushWidget : BrushWidget
	{
		// Token: 0x06000D77 RID: 3447 RVA: 0x00025BDB File Offset: 0x00023DDB
		public VillageTypeVisualIconBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x00025BE4 File Offset: 0x00023DE4
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

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06000D79 RID: 3449 RVA: 0x00025DB4 File Offset: 0x00023FB4
		// (set) Token: 0x06000D7A RID: 3450 RVA: 0x00025DBC File Offset: 0x00023FBC
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

		// Token: 0x04000638 RID: 1592
		private bool _initialized;

		// Token: 0x04000639 RID: 1593
		private int _villageType;
	}
}
