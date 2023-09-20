using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	// Token: 0x02000072 RID: 114
	public class SettlementNameplateEventVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000650 RID: 1616 RVA: 0x00012DAA File Offset: 0x00010FAA
		public SettlementNameplateEventVisualBrushWidget(UIContext context)
			: base(context)
		{
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.LateUpdateAction), 1);
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00012DD3 File Offset: 0x00010FD3
		private void LateUpdateAction(float dt)
		{
			if (!this._determinedVisual)
			{
				this.RegisterBrushStatesOfWidget();
				this.UpdateVisual(this.Type);
				this._determinedVisual = true;
			}
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00012DF8 File Offset: 0x00010FF8
		private void UpdateVisual(int type)
		{
			switch (type)
			{
			case 0:
				this.SetState("Tournament");
				break;
			case 1:
				this.SetState("AvailableIssue");
				break;
			case 2:
				this.SetState("ActiveQuest");
				break;
			case 3:
				this.SetState("ActiveStoryQuest");
				break;
			case 4:
				this.SetState("TrackedIssue");
				break;
			case 5:
				this.SetState("TrackedStoryQuest");
				break;
			case 6:
				this.SetState(this.AdditionalParameters);
				base.MarginLeft = 2f;
				base.MarginRight = 2f;
				break;
			}
			Brush brush = base.Brush;
			Sprite sprite;
			if (brush == null)
			{
				sprite = null;
			}
			else
			{
				Style style = brush.GetStyle(base.CurrentState);
				if (style == null)
				{
					sprite = null;
				}
				else
				{
					StyleLayer layer = style.GetLayer(0);
					sprite = ((layer != null) ? layer.Sprite : null);
				}
			}
			Sprite sprite2 = sprite;
			if (sprite2 != null)
			{
				base.SuggestedWidth = base.SuggestedHeight / (float)sprite2.Height * (float)sprite2.Width;
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000653 RID: 1619 RVA: 0x00012EEA File Offset: 0x000110EA
		// (set) Token: 0x06000654 RID: 1620 RVA: 0x00012EF2 File Offset: 0x000110F2
		[Editor(false)]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (this._type != value)
				{
					this._type = value;
					base.OnPropertyChanged(value, "Type");
				}
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000655 RID: 1621 RVA: 0x00012F10 File Offset: 0x00011110
		// (set) Token: 0x06000656 RID: 1622 RVA: 0x00012F18 File Offset: 0x00011118
		[Editor(false)]
		public string AdditionalParameters
		{
			get
			{
				return this._additionalParameters;
			}
			set
			{
				if (this._additionalParameters != value)
				{
					this._additionalParameters = value;
					base.OnPropertyChanged<string>(value, "AdditionalParameters");
				}
			}
		}

		// Token: 0x040002C4 RID: 708
		private bool _determinedVisual;

		// Token: 0x040002C5 RID: 709
		private int _type = -1;

		// Token: 0x040002C6 RID: 710
		private string _additionalParameters;
	}
}
