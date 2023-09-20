using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Chat
{
	public class ChatCollapsableListPanel : ListPanel
	{
		public bool IsLinesVisible { get; private set; }

		public ChatCollapsableListPanel(UIContext context)
			: base(context)
		{
		}

		private void ToggleLines(bool isVisible)
		{
			for (int i = 0; i < base.ChildCount; i++)
			{
				base.GetChild(i).IsVisible = i == 0 || isVisible;
			}
			this.IsLinesVisible = isVisible;
		}

		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			this.ToggleLines(!this.IsLinesVisible);
		}

		protected override bool OnPreviewMousePressed()
		{
			return base.OnPreviewMousePressed();
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this.ToggleLines(true);
		}

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

		private void RefreshColorValues(Color newColor)
		{
			this.UpdateColorValuesOfChildren(this, newColor);
		}

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

		private float _alpha;

		private Color _lineColor;

		private ChatLogWidget _parentChatLogWidget;
	}
}
