using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	public class SettlementNameplateEventVisualBrushWidget : BrushWidget
	{
		public SettlementNameplateEventVisualBrushWidget(UIContext context)
			: base(context)
		{
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.LateUpdateAction), 1);
		}

		private void LateUpdateAction(float dt)
		{
			if (!this._determinedVisual)
			{
				this.RegisterBrushStatesOfWidget();
				this.UpdateVisual(this.Type);
				this._determinedVisual = true;
			}
		}

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

		private bool _determinedVisual;

		private int _type = -1;

		private string _additionalParameters;
	}
}
