using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Culture
{
	public class CharacterCreationCultureVisualBrushWidget : BrushWidget
	{
		public bool UseSmallVisuals { get; set; } = true;

		public ParallaxItemBrushWidget Layer1Widget { get; set; }

		public ParallaxItemBrushWidget Layer2Widget { get; set; }

		public ParallaxItemBrushWidget Layer3Widget { get; set; }

		public ParallaxItemBrushWidget Layer4Widget { get; set; }

		public CharacterCreationCultureVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isFirstFrame)
			{
				this._alphaTarget = (float)(string.IsNullOrEmpty(this.CurrentCultureId) ? 0 : 1);
				this.SetGlobalAlphaRecursively(this._alphaTarget);
				ParallaxItemBrushWidget layer1Widget = this.Layer1Widget;
				if (layer1Widget != null)
				{
					layer1Widget.RegisterBrushStatesOfWidget();
				}
				ParallaxItemBrushWidget layer2Widget = this.Layer2Widget;
				if (layer2Widget != null)
				{
					layer2Widget.RegisterBrushStatesOfWidget();
				}
				ParallaxItemBrushWidget layer3Widget = this.Layer3Widget;
				if (layer3Widget != null)
				{
					layer3Widget.RegisterBrushStatesOfWidget();
				}
				ParallaxItemBrushWidget layer4Widget = this.Layer4Widget;
				if (layer4Widget != null)
				{
					layer4Widget.RegisterBrushStatesOfWidget();
				}
				this._isFirstFrame = false;
			}
			this.SetGlobalAlphaRecursively(Mathf.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, this._alphaTarget, dt * 10f));
		}

		private void SetCultureVisual(string newCultureId)
		{
			if (string.IsNullOrEmpty(newCultureId))
			{
				this._alphaTarget = 0f;
				return;
			}
			if (this.UseSmallVisuals)
			{
				using (Dictionary<string, Style>.ValueCollection.Enumerator enumerator = base.Brush.Styles.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Style style = enumerator.Current;
						foreach (StyleLayer styleLayer in style.Layers)
						{
							styleLayer.Sprite = base.Context.SpriteData.GetSprite("CharacterCreation\\Culture\\" + newCultureId);
						}
					}
					goto IL_E7;
				}
			}
			ParallaxItemBrushWidget layer1Widget = this.Layer1Widget;
			if (layer1Widget != null)
			{
				layer1Widget.SetState(newCultureId);
			}
			ParallaxItemBrushWidget layer2Widget = this.Layer2Widget;
			if (layer2Widget != null)
			{
				layer2Widget.SetState(newCultureId);
			}
			ParallaxItemBrushWidget layer3Widget = this.Layer3Widget;
			if (layer3Widget != null)
			{
				layer3Widget.SetState(newCultureId);
			}
			ParallaxItemBrushWidget layer4Widget = this.Layer4Widget;
			if (layer4Widget != null)
			{
				layer4Widget.SetState(newCultureId);
			}
			IL_E7:
			this._alphaTarget = 1f;
		}

		[Editor(false)]
		public string CurrentCultureId
		{
			get
			{
				return this._currentCultureId;
			}
			set
			{
				if (this._currentCultureId != value)
				{
					this._currentCultureId = value;
					base.OnPropertyChanged<string>(value, "CurrentCultureId");
					this.SetCultureVisual(value);
					this.SetGlobalAlphaRecursively(1f);
				}
			}
		}

		[Editor(false)]
		public bool IsBig
		{
			get
			{
				return this._isBig;
			}
			set
			{
				if (this._isBig != value)
				{
					this._isBig = value;
					base.OnPropertyChanged(value, "IsBig");
				}
			}
		}

		private float _alphaTarget;

		private bool _isFirstFrame = true;

		private string _currentCultureId;

		private bool _isBig;
	}
}
