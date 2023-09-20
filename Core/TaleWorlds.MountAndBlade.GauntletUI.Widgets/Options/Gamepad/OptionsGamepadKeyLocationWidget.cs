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
	public class OptionsGamepadKeyLocationWidget : Widget
	{
		public bool ForceVisible { get; set; }

		public int KeyID { get; set; }

		public int NormalPositionXOffset { get; set; }

		public int NormalPositionYOffset { get; set; }

		public int NormalSizeXOfImage { get; private set; } = -1;

		public int NormalSizeYOfImage { get; private set; } = -1;

		public int CurrentSizeXOfImage { get; private set; } = -1;

		public int CurrentSizeYOfImage { get; private set; } = -1;

		public bool IsKeyToTheLeftOfTheGamepad { get; private set; }

		public OptionsGamepadKeyLocationWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _valuesInitialized;

		private string _actionText;

		private Widget _parentAreaWidget;

		private List<TextWidget> _keyNameTextWidgets = new List<TextWidget>();

		private InputKeyVisualWidget _keyVisualWidget;
	}
}
