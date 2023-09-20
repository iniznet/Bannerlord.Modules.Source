using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Culture
{
	// Token: 0x0200016A RID: 362
	public class CharacterCreationCultureVisualBrushWidget : BrushWidget
	{
		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06001291 RID: 4753 RVA: 0x0003348A File Offset: 0x0003168A
		// (set) Token: 0x06001292 RID: 4754 RVA: 0x00033492 File Offset: 0x00031692
		public bool UseSmallVisuals { get; set; } = true;

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x0003349B File Offset: 0x0003169B
		// (set) Token: 0x06001294 RID: 4756 RVA: 0x000334A3 File Offset: 0x000316A3
		public ParallaxItemBrushWidget Layer1Widget { get; set; }

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x06001295 RID: 4757 RVA: 0x000334AC File Offset: 0x000316AC
		// (set) Token: 0x06001296 RID: 4758 RVA: 0x000334B4 File Offset: 0x000316B4
		public ParallaxItemBrushWidget Layer2Widget { get; set; }

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06001297 RID: 4759 RVA: 0x000334BD File Offset: 0x000316BD
		// (set) Token: 0x06001298 RID: 4760 RVA: 0x000334C5 File Offset: 0x000316C5
		public ParallaxItemBrushWidget Layer3Widget { get; set; }

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06001299 RID: 4761 RVA: 0x000334CE File Offset: 0x000316CE
		// (set) Token: 0x0600129A RID: 4762 RVA: 0x000334D6 File Offset: 0x000316D6
		public ParallaxItemBrushWidget Layer4Widget { get; set; }

		// Token: 0x0600129B RID: 4763 RVA: 0x000334DF File Offset: 0x000316DF
		public CharacterCreationCultureVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x000334F8 File Offset: 0x000316F8
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

		// Token: 0x0600129D RID: 4765 RVA: 0x000335A8 File Offset: 0x000317A8
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

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x0600129E RID: 4766 RVA: 0x000336C4 File Offset: 0x000318C4
		// (set) Token: 0x0600129F RID: 4767 RVA: 0x000336CC File Offset: 0x000318CC
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

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x060012A0 RID: 4768 RVA: 0x00033701 File Offset: 0x00031901
		// (set) Token: 0x060012A1 RID: 4769 RVA: 0x00033709 File Offset: 0x00031909
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

		// Token: 0x04000884 RID: 2180
		private float _alphaTarget;

		// Token: 0x04000885 RID: 2181
		private bool _isFirstFrame = true;

		// Token: 0x04000886 RID: 2182
		private string _currentCultureId;

		// Token: 0x04000887 RID: 2183
		private bool _isBig;
	}
}
