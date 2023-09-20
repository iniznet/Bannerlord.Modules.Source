using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.AdminMessage
{
	// Token: 0x020000C0 RID: 192
	public class MultiplayerAdminMessageWidget : Widget
	{
		// Token: 0x17000368 RID: 872
		// (get) Token: 0x060009BF RID: 2495 RVA: 0x0001BD4D File Offset: 0x00019F4D
		// (set) Token: 0x060009C0 RID: 2496 RVA: 0x0001BD55 File Offset: 0x00019F55
		public TextWidget MessageTextWidget { get; set; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x060009C1 RID: 2497 RVA: 0x0001BD5E File Offset: 0x00019F5E
		public float MessageOnScreenStayTime
		{
			get
			{
				return 5f;
			}
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x060009C2 RID: 2498 RVA: 0x0001BD65 File Offset: 0x00019F65
		public float MessageFadeInTime
		{
			get
			{
				return 0.4f;
			}
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x060009C3 RID: 2499 RVA: 0x0001BD6C File Offset: 0x00019F6C
		public float MessageFadeOutTime
		{
			get
			{
				return 0.2f;
			}
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x0001BD73 File Offset: 0x00019F73
		public MultiplayerAdminMessageWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x0001BD7C File Offset: 0x00019F7C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.ChildCount <= 0)
			{
				this._currentTextOnScreenTime = 0f;
				return;
			}
			this._currentTextOnScreenTime += dt;
			if (this._currentTextOnScreenTime < this.MessageFadeInTime)
			{
				float num = MathF.Lerp(0f, 1f, this._currentTextOnScreenTime / this.MessageFadeInTime, 1E-05f);
				base.Children[0].SetGlobalAlphaRecursively(num);
				base.Children[0].IsVisible = true;
				return;
			}
			if (this._currentTextOnScreenTime > this.MessageFadeInTime && this._currentTextOnScreenTime < this.MessageOnScreenStayTime + this.MessageFadeInTime)
			{
				base.Children[0].SetGlobalAlphaRecursively(1f);
				return;
			}
			if (this._currentTextOnScreenTime < this.MessageFadeInTime + this.MessageOnScreenStayTime + this.MessageFadeOutTime)
			{
				float num2 = MathF.Lerp(1f, 0f, (this._currentTextOnScreenTime - (this.MessageFadeInTime + this.MessageOnScreenStayTime)) / this.MessageFadeOutTime, 1E-05f);
				base.Children[0].SetGlobalAlphaRecursively(num2);
				return;
			}
			MultiplayerAdminMessageItemWidget multiplayerAdminMessageItemWidget = base.Children[0] as MultiplayerAdminMessageItemWidget;
			if (multiplayerAdminMessageItemWidget != null)
			{
				multiplayerAdminMessageItemWidget.Remove();
			}
			this._currentTextOnScreenTime = 0f;
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x0001BECB File Offset: 0x0001A0CB
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
		}

		// Token: 0x04000476 RID: 1142
		private float _currentTextOnScreenTime;
	}
}
