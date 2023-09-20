using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Chat
{
	// Token: 0x02000159 RID: 345
	public class ChatCollapsableListPanel : ListPanel
	{
		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x060011B4 RID: 4532 RVA: 0x00030D5C File Offset: 0x0002EF5C
		// (set) Token: 0x060011B5 RID: 4533 RVA: 0x00030D64 File Offset: 0x0002EF64
		public bool IsLinesVisible { get; private set; }

		// Token: 0x060011B6 RID: 4534 RVA: 0x00030D6D File Offset: 0x0002EF6D
		public ChatCollapsableListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00030D78 File Offset: 0x0002EF78
		private void ToggleLines(bool isVisible)
		{
			for (int i = 0; i < base.ChildCount; i++)
			{
				base.GetChild(i).IsVisible = i == 0 || isVisible;
			}
			this.IsLinesVisible = isVisible;
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00030DAF File Offset: 0x0002EFAF
		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			this.ToggleLines(!this.IsLinesVisible);
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x00030DC6 File Offset: 0x0002EFC6
		protected override bool OnPreviewMousePressed()
		{
			return base.OnPreviewMousePressed();
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x00030DCE File Offset: 0x0002EFCE
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this.ToggleLines(true);
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x00030DDE File Offset: 0x0002EFDE
		private void RefreshAlphaValues(float newAlpha)
		{
			this.SetGlobalAlphaRecursively(newAlpha);
			if (newAlpha > 0f)
			{
				ChatLogWidget parentChatLogWidget = this.ParentChatLogWidget;
				if (parentChatLogWidget == null)
				{
					return;
				}
				parentChatLogWidget.RegisterMultiLineElement(this);
				return;
			}
			else
			{
				ChatLogWidget parentChatLogWidget2 = this.ParentChatLogWidget;
				if (parentChatLogWidget2 == null)
				{
					return;
				}
				parentChatLogWidget2.RemoveMultiLineElement(this);
				return;
			}
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x00030E14 File Offset: 0x0002F014
		private void UpdateColorValuesOfChildren(Widget widget, Color newColor)
		{
			foreach (Widget widget2 in widget.Children)
			{
				BrushWidget brushWidget;
				if ((brushWidget = widget2 as BrushWidget) != null)
				{
					brushWidget.Brush.FontColor = newColor;
				}
				else
				{
					widget2.Color = newColor;
				}
				this.UpdateColorValuesOfChildren(widget2, newColor);
			}
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x00030E88 File Offset: 0x0002F088
		private void RefreshColorValues(Color newColor)
		{
			this.UpdateColorValuesOfChildren(this, newColor);
		}

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x060011BE RID: 4542 RVA: 0x00030E92 File Offset: 0x0002F092
		// (set) Token: 0x060011BF RID: 4543 RVA: 0x00030E9A File Offset: 0x0002F09A
		[DataSourceProperty]
		public float Alpha
		{
			get
			{
				return this._alpha;
			}
			set
			{
				if (value != this._alpha)
				{
					this._alpha = value;
					base.OnPropertyChanged(value, "Alpha");
					this.RefreshAlphaValues(value);
				}
			}
		}

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x00030EBF File Offset: 0x0002F0BF
		// (set) Token: 0x060011C1 RID: 4545 RVA: 0x00030EC7 File Offset: 0x0002F0C7
		[DataSourceProperty]
		public Color LineColor
		{
			get
			{
				return this._lineColor;
			}
			set
			{
				if (value != this._lineColor)
				{
					this._lineColor = value;
					base.OnPropertyChanged(value, "LineColor");
					this.RefreshColorValues(value);
				}
			}
		}

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x00030EF1 File Offset: 0x0002F0F1
		// (set) Token: 0x060011C3 RID: 4547 RVA: 0x00030EF9 File Offset: 0x0002F0F9
		[DataSourceProperty]
		public ChatLogWidget ParentChatLogWidget
		{
			get
			{
				return this._parentChatLogWidget;
			}
			set
			{
				if (value != this._parentChatLogWidget)
				{
					this._parentChatLogWidget = value;
					base.OnPropertyChanged<ChatLogWidget>(value, "ParentChatLogWidget");
				}
			}
		}

		// Token: 0x04000817 RID: 2071
		private float _alpha;

		// Token: 0x04000818 RID: 2072
		private Color _lineColor;

		// Token: 0x04000819 RID: 2073
		private ChatLogWidget _parentChatLogWidget;
	}
}
