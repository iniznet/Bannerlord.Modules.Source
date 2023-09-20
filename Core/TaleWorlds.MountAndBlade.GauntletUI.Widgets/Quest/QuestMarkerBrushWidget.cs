using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Quest
{
	// Token: 0x02000053 RID: 83
	public class QuestMarkerBrushWidget : BrushWidget
	{
		// Token: 0x06000461 RID: 1121 RVA: 0x0000DB91 File Offset: 0x0000BD91
		public QuestMarkerBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x0000DB9C File Offset: 0x0000BD9C
		private void UpdateMarkerState(int type)
		{
			string text;
			switch (type)
			{
			case 0:
				text = "None";
				goto IL_6D;
			case 1:
				text = "AvailableIssue";
				goto IL_6D;
			case 2:
				text = "ActiveIssue";
				goto IL_6D;
			case 3:
			case 5:
			case 6:
			case 7:
				break;
			case 4:
				text = "ActiveStoryQuest";
				goto IL_6D;
			case 8:
				text = "TrackedIssue";
				goto IL_6D;
			default:
				if (type == 16)
				{
					text = "TrackedStoryQuest";
					goto IL_6D;
				}
				break;
			}
			text = "None";
			IL_6D:
			if (text != null)
			{
				this.SetState(text);
				Sprite sprite = base.Brush.GetLayer(text).Sprite;
				if (sprite != null)
				{
					float num = base.SuggestedHeight / (float)sprite.Height;
					base.SuggestedWidth = (float)sprite.Width * num;
				}
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x0000DC53 File Offset: 0x0000BE53
		// (set) Token: 0x06000464 RID: 1124 RVA: 0x0000DC5B File Offset: 0x0000BE5B
		public int QuestMarkerType
		{
			get
			{
				return this._questMarkerType;
			}
			set
			{
				if (value != this._questMarkerType)
				{
					this._questMarkerType = value;
					this.UpdateMarkerState(this._questMarkerType);
				}
			}
		}

		// Token: 0x040001E8 RID: 488
		private int _questMarkerType;
	}
}
