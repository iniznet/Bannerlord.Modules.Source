using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options.Gamepad
{
	// Token: 0x0200006D RID: 109
	public class OptionsGamepadCategoryWidget : Widget
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060005E4 RID: 1508 RVA: 0x00011844 File Offset: 0x0000FA44
		// (set) Token: 0x060005E5 RID: 1509 RVA: 0x0001184C File Offset: 0x0000FA4C
		public Widget Playstation4LayoutParentWidget { get; set; }

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00011855 File Offset: 0x0000FA55
		// (set) Token: 0x060005E7 RID: 1511 RVA: 0x0001185D File Offset: 0x0000FA5D
		public Widget Playstation5LayoutParentWidget { get; set; }

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00011866 File Offset: 0x0000FA66
		// (set) Token: 0x060005E9 RID: 1513 RVA: 0x0001186E File Offset: 0x0000FA6E
		public Widget XboxLayoutParentWidget { get; set; }

		// Token: 0x060005EA RID: 1514 RVA: 0x00011877 File Offset: 0x0000FA77
		public OptionsGamepadCategoryWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00011887 File Offset: 0x0000FA87
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initalized)
			{
				this.SetGamepadLayoutVisibility(this.CurrentGamepadType);
				this._initalized = true;
			}
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x000118AC File Offset: 0x0000FAAC
		private void SetGamepadLayoutVisibility(int gamepadType)
		{
			this.XboxLayoutParentWidget.IsVisible = false;
			this.Playstation4LayoutParentWidget.IsVisible = false;
			this.Playstation5LayoutParentWidget.IsVisible = false;
			if (gamepadType == 0)
			{
				this.XboxLayoutParentWidget.IsVisible = true;
				return;
			}
			if (gamepadType == 1)
			{
				this.Playstation4LayoutParentWidget.IsVisible = true;
				return;
			}
			if (gamepadType == 2)
			{
				this.Playstation5LayoutParentWidget.IsVisible = true;
				return;
			}
			this.XboxLayoutParentWidget.IsVisible = true;
			Debug.FailedAssert("This kind of gamepad is not visually supported", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Options\\Gamepad\\OptionsGamepadCategoryWidget.cs", "SetGamepadLayoutVisibility", 47);
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x00011931 File Offset: 0x0000FB31
		// (set) Token: 0x060005EE RID: 1518 RVA: 0x00011939 File Offset: 0x0000FB39
		public int CurrentGamepadType
		{
			get
			{
				return this._currentGamepadType;
			}
			set
			{
				if (this._currentGamepadType != value)
				{
					this._currentGamepadType = value;
				}
			}
		}

		// Token: 0x0400028D RID: 653
		private bool _initalized;

		// Token: 0x0400028E RID: 654
		private int _currentGamepadType = -1;
	}
}
