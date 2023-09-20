using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Quest
{
	public class QuestMarkerBrushWidget : BrushWidget
	{
		public QuestMarkerBrushWidget(UIContext context)
			: base(context)
		{
		}

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

		private int _questMarkerType;
	}
}
