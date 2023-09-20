using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x02000042 RID: 66
	public class MouseAnimStage
	{
		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000372 RID: 882 RVA: 0x0000B58A File Offset: 0x0000978A
		// (set) Token: 0x06000373 RID: 883 RVA: 0x0000B592 File Offset: 0x00009792
		public bool IsCompleted { get; private set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000374 RID: 884 RVA: 0x0000B59B File Offset: 0x0000979B
		// (set) Token: 0x06000375 RID: 885 RVA: 0x0000B5A3 File Offset: 0x000097A3
		public float AnimTime { get; private set; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000376 RID: 886 RVA: 0x0000B5AC File Offset: 0x000097AC
		// (set) Token: 0x06000377 RID: 887 RVA: 0x0000B5B4 File Offset: 0x000097B4
		public Vec2 Direction { get; private set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000378 RID: 888 RVA: 0x0000B5BD File Offset: 0x000097BD
		// (set) Token: 0x06000379 RID: 889 RVA: 0x0000B5C5 File Offset: 0x000097C5
		public MouseAnimStage.AnimTypes AnimType { get; private set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x0600037A RID: 890 RVA: 0x0000B5CE File Offset: 0x000097CE
		// (set) Token: 0x0600037B RID: 891 RVA: 0x0000B5D6 File Offset: 0x000097D6
		public Widget WidgetToManipulate { get; private set; }

		// Token: 0x0600037C RID: 892 RVA: 0x0000B5DF File Offset: 0x000097DF
		private MouseAnimStage()
		{
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0000B5E7 File Offset: 0x000097E7
		internal static MouseAnimStage CreateMovementStage(float movementTime, Vec2 direction, Widget widgetToManipulate)
		{
			return new MouseAnimStage
			{
				AnimTime = movementTime,
				Direction = direction,
				AnimType = MouseAnimStage.AnimTypes.Movement,
				WidgetToManipulate = widgetToManipulate
			};
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000B60A File Offset: 0x0000980A
		internal static MouseAnimStage CreateFadeInStage(float fadeInTime, Widget widgetToManipulate, bool isGlobal)
		{
			return new MouseAnimStage
			{
				AnimTime = fadeInTime,
				AnimType = (isGlobal ? MouseAnimStage.AnimTypes.FadeInGlobal : MouseAnimStage.AnimTypes.FadeInLocal),
				WidgetToManipulate = widgetToManipulate
			};
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0000B62C File Offset: 0x0000982C
		internal static MouseAnimStage CreateFadeOutStage(float fadeOutTime, Widget widgetToManipulate, bool isGlobal)
		{
			return new MouseAnimStage
			{
				AnimTime = fadeOutTime,
				AnimType = (isGlobal ? MouseAnimStage.AnimTypes.FadeOutGlobal : MouseAnimStage.AnimTypes.FadeOutLocal),
				WidgetToManipulate = widgetToManipulate
			};
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000B64E File Offset: 0x0000984E
		internal static MouseAnimStage CreateStayStage(float stayTime)
		{
			return new MouseAnimStage
			{
				AnimTime = stayTime,
				AnimType = MouseAnimStage.AnimTypes.Stay,
				WidgetToManipulate = null
			};
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0000B66C File Offset: 0x0000986C
		public void Tick(float dt)
		{
			float num = MathF.Clamp(this._totalTime / this.AnimTime, 0f, 1f);
			switch (this.AnimType)
			{
			case MouseAnimStage.AnimTypes.Movement:
				this.WidgetToManipulate.PositionXOffset = ((this.Direction.X != 0f) ? MathF.Lerp(0f, this.Direction.X, num, 1E-05f) : 0f);
				this.WidgetToManipulate.PositionYOffset = ((this.Direction.Y != 0f) ? MathF.Lerp(0f, this.Direction.Y, num, 1E-05f) : 0f);
				this.IsCompleted = this._totalTime > this.AnimTime;
				break;
			case MouseAnimStage.AnimTypes.FadeInLocal:
				this.WidgetToManipulate.AlphaFactor = num;
				this.IsCompleted = this.WidgetToManipulate.AlphaFactor > 0.98f;
				break;
			case MouseAnimStage.AnimTypes.FadeOutLocal:
				this.WidgetToManipulate.AlphaFactor = 1f - num;
				this.IsCompleted = this.WidgetToManipulate.AlphaFactor < 0.02f;
				break;
			case MouseAnimStage.AnimTypes.FadeInGlobal:
				this.WidgetToManipulate.SetGlobalAlphaRecursively(num);
				this.IsCompleted = this.WidgetToManipulate.AlphaFactor > 0.98f;
				break;
			case MouseAnimStage.AnimTypes.FadeOutGlobal:
				this.WidgetToManipulate.SetGlobalAlphaRecursively(1f - num);
				this.IsCompleted = this.WidgetToManipulate.AlphaFactor < 0.02f;
				break;
			case MouseAnimStage.AnimTypes.Stay:
				this.IsCompleted = this._totalTime > this.AnimTime;
				break;
			}
			this._totalTime += dt;
		}

		// Token: 0x0400017C RID: 380
		private float _totalTime;

		// Token: 0x02000180 RID: 384
		public enum AnimTypes
		{
			// Token: 0x040008C9 RID: 2249
			Movement,
			// Token: 0x040008CA RID: 2250
			FadeInLocal,
			// Token: 0x040008CB RID: 2251
			FadeOutLocal,
			// Token: 0x040008CC RID: 2252
			FadeInGlobal,
			// Token: 0x040008CD RID: 2253
			FadeOutGlobal,
			// Token: 0x040008CE RID: 2254
			Stay
		}
	}
}
