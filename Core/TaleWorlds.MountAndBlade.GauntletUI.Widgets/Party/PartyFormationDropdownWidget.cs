using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x02000057 RID: 87
	public class PartyFormationDropdownWidget : DropdownWidget
	{
		// Token: 0x06000484 RID: 1156 RVA: 0x0000E058 File Offset: 0x0000C258
		public PartyFormationDropdownWidget(UIContext context)
			: base(context)
		{
			base.DoNotHandleDropdownListPanel = true;
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x0000E068 File Offset: 0x0000C268
		private void ListStateChangerUpdated()
		{
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0000E06A File Offset: 0x0000C26A
		private void SeperatorStateChangerUpdated()
		{
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0000E06C File Offset: 0x0000C26C
		protected override void OpenPanel()
		{
			base.ListPanel.IsVisible = true;
			this.SeperatorStateChanger.IsVisible = true;
			this.ListStateChanger.Delay = this.SeperatorStateChanger.VisualDefinition.TransitionDuration;
			this.ListStateChanger.State = "Opened";
			this.ListStateChanger.Start();
			this.SeperatorStateChanger.Delay = 0f;
			this.SeperatorStateChanger.State = "Opened";
			this.SeperatorStateChanger.Start();
			base.Context.TwoDimensionContext.PlaySound("dropdown");
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0000E108 File Offset: 0x0000C308
		protected override void ClosePanel()
		{
			this.ListStateChanger.Delay = 0f;
			this.ListStateChanger.State = "Closed";
			this.ListStateChanger.Start();
			this.SeperatorStateChanger.Delay = this.ListStateChanger.TargetWidget.VisualDefinition.TransitionDuration;
			this.SeperatorStateChanger.State = "Closed";
			this.SeperatorStateChanger.Start();
			base.Context.TwoDimensionContext.PlaySound("dropdown");
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x0000E190 File Offset: 0x0000C390
		// (set) Token: 0x0600048A RID: 1162 RVA: 0x0000E198 File Offset: 0x0000C398
		[Editor(false)]
		public DelayedStateChanger SeperatorStateChanger
		{
			get
			{
				return this._seperatorStateChanger;
			}
			set
			{
				if (this._seperatorStateChanger != value)
				{
					this._seperatorStateChanger = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "SeperatorStateChanger");
					this.SeperatorStateChangerUpdated();
				}
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x0000E1BC File Offset: 0x0000C3BC
		// (set) Token: 0x0600048C RID: 1164 RVA: 0x0000E1C4 File Offset: 0x0000C3C4
		[Editor(false)]
		public DelayedStateChanger ListStateChanger
		{
			get
			{
				return this._listStateChanger;
			}
			set
			{
				if (this._listStateChanger != value)
				{
					this._listStateChanger = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "ListStateChanger");
					this.ListStateChangerUpdated();
				}
			}
		}

		// Token: 0x040001F8 RID: 504
		private DelayedStateChanger _seperatorStateChanger;

		// Token: 0x040001F9 RID: 505
		private DelayedStateChanger _listStateChanger;
	}
}
