using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options.Gamepad
{
	// Token: 0x0200006E RID: 110
	public class OptionsGamepadKeyLocationWidget : Widget
	{
		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x0001194B File Offset: 0x0000FB4B
		// (set) Token: 0x060005F0 RID: 1520 RVA: 0x00011953 File Offset: 0x0000FB53
		public bool ForceVisible { get; set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x0001195C File Offset: 0x0000FB5C
		// (set) Token: 0x060005F2 RID: 1522 RVA: 0x00011964 File Offset: 0x0000FB64
		public int KeyID { get; set; }

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060005F3 RID: 1523 RVA: 0x0001196D File Offset: 0x0000FB6D
		// (set) Token: 0x060005F4 RID: 1524 RVA: 0x00011975 File Offset: 0x0000FB75
		public int NormalPositionXOffset { get; set; }

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060005F5 RID: 1525 RVA: 0x0001197E File Offset: 0x0000FB7E
		// (set) Token: 0x060005F6 RID: 1526 RVA: 0x00011986 File Offset: 0x0000FB86
		public int NormalPositionYOffset { get; set; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060005F7 RID: 1527 RVA: 0x0001198F File Offset: 0x0000FB8F
		// (set) Token: 0x060005F8 RID: 1528 RVA: 0x00011997 File Offset: 0x0000FB97
		public int NormalSizeXOfImage { get; private set; } = -1;

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060005F9 RID: 1529 RVA: 0x000119A0 File Offset: 0x0000FBA0
		// (set) Token: 0x060005FA RID: 1530 RVA: 0x000119A8 File Offset: 0x0000FBA8
		public int NormalSizeYOfImage { get; private set; } = -1;

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060005FB RID: 1531 RVA: 0x000119B1 File Offset: 0x0000FBB1
		// (set) Token: 0x060005FC RID: 1532 RVA: 0x000119B9 File Offset: 0x0000FBB9
		public int CurrentSizeXOfImage { get; private set; } = -1;

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060005FD RID: 1533 RVA: 0x000119C2 File Offset: 0x0000FBC2
		// (set) Token: 0x060005FE RID: 1534 RVA: 0x000119CA File Offset: 0x0000FBCA
		public int CurrentSizeYOfImage { get; private set; } = -1;

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060005FF RID: 1535 RVA: 0x000119D3 File Offset: 0x0000FBD3
		// (set) Token: 0x06000600 RID: 1536 RVA: 0x000119DB File Offset: 0x0000FBDB
		public bool IsKeyToTheLeftOfTheGamepad { get; private set; }

		// Token: 0x06000601 RID: 1537 RVA: 0x000119E4 File Offset: 0x0000FBE4
		public OptionsGamepadKeyLocationWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x00011A14 File Offset: 0x0000FC14
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._valuesInitialized)
			{
				this.NormalSizeXOfImage = base.ParentWidget.Sprite.Width;
				this.NormalSizeYOfImage = base.ParentWidget.Sprite.Height;
				this.CurrentSizeXOfImage = (int)(base.ParentWidget.SuggestedWidth * base._scaleToUse);
				this.CurrentSizeYOfImage = (int)(base.ParentWidget.SuggestedHeight * base._scaleToUse);
				this._keyNameTextWidgets.Clear();
				using (IEnumerator<Widget> enumerator = base.AllChildren.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TextWidget textWidget;
						if ((textWidget = enumerator.Current as TextWidget) != null)
						{
							this._keyNameTextWidgets.Add(textWidget);
						}
					}
				}
				this._keyVisualWidget = base.AllChildren.FirstOrDefault((Widget c) => c is InputKeyVisualWidget) as InputKeyVisualWidget;
				this._valuesInitialized = true;
				this.IsKeyToTheLeftOfTheGamepad = (float)this.NormalPositionXOffset < (float)this.NormalSizeXOfImage / 2f;
			}
			float num = base.ParentWidget.SuggestedWidth / (float)this.NormalSizeXOfImage;
			float num2 = base.ParentWidget.SuggestedHeight / (float)this.NormalSizeYOfImage;
			base.PositionXOffset = (float)this.NormalPositionXOffset * num;
			base.PositionYOffset = (float)this.NormalPositionYOffset * num2;
			List<TextWidget> keyNameTextWidgets = this._keyNameTextWidgets;
			if (keyNameTextWidgets != null && keyNameTextWidgets.Count == 1)
			{
				this._keyNameTextWidgets[0].Text = this._actionText;
			}
			base.IsVisible = !string.IsNullOrEmpty(this._actionText) || this.ForceVisible;
			if (this._valuesInitialized)
			{
				if (this.IsKeyToTheLeftOfTheGamepad)
				{
					this._keyNameTextWidgets.ForEach(delegate(TextWidget t)
					{
						t.ScaledSuggestedWidth = MathF.Abs(this._parentAreaWidget.GlobalPosition.X - this._keyVisualWidget.GlobalPosition.X);
						t.Brush.TextHorizontalAlignment = TextHorizontalAlignment.Right;
					});
					return;
				}
				this._keyNameTextWidgets.ForEach(delegate(TextWidget t)
				{
					t.ScaledSuggestedWidth = this._parentAreaWidget.GlobalPosition.X + this._parentAreaWidget.Size.X - (this._keyVisualWidget.GlobalPosition.X + this._keyVisualWidget.Size.X);
					t.Brush.TextHorizontalAlignment = TextHorizontalAlignment.Left;
				});
			}
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x00011C10 File Offset: 0x0000FE10
		internal void SetKeyProperties(string actionText, Widget parentAreaWidget)
		{
			this._actionText = actionText;
			List<TextWidget> keyNameTextWidgets = this._keyNameTextWidgets;
			if (keyNameTextWidgets != null && keyNameTextWidgets.Count == 1)
			{
				this._keyNameTextWidgets[0].Text = this._actionText;
			}
			this._parentAreaWidget = parentAreaWidget;
			this._valuesInitialized = false;
		}

		// Token: 0x04000298 RID: 664
		private bool _valuesInitialized;

		// Token: 0x04000299 RID: 665
		private string _actionText;

		// Token: 0x0400029A RID: 666
		private Widget _parentAreaWidget;

		// Token: 0x0400029B RID: 667
		private List<TextWidget> _keyNameTextWidgets = new List<TextWidget>();

		// Token: 0x0400029C RID: 668
		private InputKeyVisualWidget _keyVisualWidget;
	}
}
