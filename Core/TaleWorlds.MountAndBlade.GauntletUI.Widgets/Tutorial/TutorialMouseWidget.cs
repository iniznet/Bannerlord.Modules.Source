using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x02000041 RID: 65
	public class TutorialMouseWidget : Widget
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000360 RID: 864 RVA: 0x0000AD32 File Offset: 0x00008F32
		// (set) Token: 0x06000361 RID: 865 RVA: 0x0000AD3A File Offset: 0x00008F3A
		public Widget GhostMouseVisualWidget { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000362 RID: 866 RVA: 0x0000AD43 File Offset: 0x00008F43
		// (set) Token: 0x06000363 RID: 867 RVA: 0x0000AD4B File Offset: 0x00008F4B
		public Widget LeftMouseClickVisualWidget { get; set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000364 RID: 868 RVA: 0x0000AD54 File Offset: 0x00008F54
		// (set) Token: 0x06000365 RID: 869 RVA: 0x0000AD5C File Offset: 0x00008F5C
		public Widget RightMouseClickVisualWidget { get; set; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000366 RID: 870 RVA: 0x0000AD65 File Offset: 0x00008F65
		// (set) Token: 0x06000367 RID: 871 RVA: 0x0000AD6D File Offset: 0x00008F6D
		public Widget MiddleMouseClickVisualWidget { get; set; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000368 RID: 872 RVA: 0x0000AD76 File Offset: 0x00008F76
		// (set) Token: 0x06000369 RID: 873 RVA: 0x0000AD7E File Offset: 0x00008F7E
		public Widget HorizontalArrowWidget { get; set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600036A RID: 874 RVA: 0x0000AD87 File Offset: 0x00008F87
		// (set) Token: 0x0600036B RID: 875 RVA: 0x0000AD8F File Offset: 0x00008F8F
		public Widget VerticalArrowWidget { get; set; }

		// Token: 0x0600036C RID: 876 RVA: 0x0000AD98 File Offset: 0x00008F98
		public TutorialMouseWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0000ADAC File Offset: 0x00008FAC
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._animQueue.Count > 0)
			{
				base.IsVisible = true;
				base.ParentWidget.ParentWidget.AlphaFactor = 0.1f;
				this._animQueue.Peek().ForEach(delegate(MouseAnimStage a)
				{
					a.Tick(dt);
				});
				if (this._animQueue.Peek().All((MouseAnimStage a) => a.IsCompleted))
				{
					this._animQueue.Dequeue();
					return;
				}
			}
			else
			{
				this.UpdateAnimQueue();
			}
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0000AE5C File Offset: 0x0000905C
		private void Reset()
		{
			this._animQueue.Clear();
			base.PositionXOffset = 0f;
			base.PositionYOffset = 0f;
			this.SetGlobalAlphaRecursively(0f);
			this.GhostMouseVisualWidget.SetGlobalAlphaRecursively(0f);
			this.HorizontalArrowWidget.SetGlobalAlphaRecursively(0f);
			this.VerticalArrowWidget.SetGlobalAlphaRecursively(0f);
			this.RightMouseClickVisualWidget.SetGlobalAlphaRecursively(0f);
			this.LeftMouseClickVisualWidget.SetGlobalAlphaRecursively(0f);
			base.ParentWidget.ParentWidget.AlphaFactor = 0f;
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0000AEFC File Offset: 0x000090FC
		private void UpdateAnimQueue()
		{
			this.Reset();
			switch (this.CurrentObjectiveType)
			{
			case 1:
				this.HorizontalArrowWidget.HorizontalFlip = true;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.GhostMouseVisualWidget, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.HorizontalArrowWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateMovementStage(0.15f, new Vec2(-20f, 0f), this),
					MouseAnimStage.CreateFadeInStage(0.15f, this.LeftMouseClickVisualWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(1f) });
				return;
			case 2:
				this.HorizontalArrowWidget.HorizontalFlip = false;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.GhostMouseVisualWidget, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.HorizontalArrowWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateMovementStage(0.15f, new Vec2(20f, 0f), this),
					MouseAnimStage.CreateFadeInStage(0.15f, this.LeftMouseClickVisualWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(1f) });
				return;
			case 3:
				this.VerticalArrowWidget.VerticalFlip = true;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.GhostMouseVisualWidget, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.VerticalArrowWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateMovementStage(0.15f, new Vec2(0f, -20f), this),
					MouseAnimStage.CreateFadeInStage(0.15f, this.LeftMouseClickVisualWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(1f) });
				return;
			case 4:
				this.VerticalArrowWidget.VerticalFlip = false;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.GhostMouseVisualWidget, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.VerticalArrowWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateMovementStage(0.15f, new Vec2(0f, 20f), this),
					MouseAnimStage.CreateFadeInStage(0.15f, this.LeftMouseClickVisualWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(1f) });
				return;
			case 5:
				this.HorizontalArrowWidget.HorizontalFlip = true;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.GhostMouseVisualWidget, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.HorizontalArrowWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateMovementStage(0.15f, new Vec2(-20f, 0f), this),
					MouseAnimStage.CreateFadeInStage(0.15f, this.RightMouseClickVisualWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(1f) });
				return;
			case 6:
				this.HorizontalArrowWidget.HorizontalFlip = false;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.GhostMouseVisualWidget, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.HorizontalArrowWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateMovementStage(0.15f, new Vec2(20f, 0f), this),
					MouseAnimStage.CreateFadeInStage(0.15f, this.RightMouseClickVisualWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(1f) });
				return;
			case 7:
				this.VerticalArrowWidget.VerticalFlip = true;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.GhostMouseVisualWidget, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.VerticalArrowWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateMovementStage(0.15f, new Vec2(0f, -20f), this),
					MouseAnimStage.CreateFadeInStage(0.15f, this.RightMouseClickVisualWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(1f) });
				return;
			case 8:
				this.VerticalArrowWidget.VerticalFlip = false;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.GhostMouseVisualWidget, false),
					MouseAnimStage.CreateFadeInStage(0.15f, this.VerticalArrowWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateMovementStage(0.15f, new Vec2(0f, 20f), this),
					MouseAnimStage.CreateFadeInStage(0.15f, this.RightMouseClickVisualWidget, false)
				});
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(1f) });
				return;
			default:
				return;
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000370 RID: 880 RVA: 0x0000B55E File Offset: 0x0000975E
		// (set) Token: 0x06000371 RID: 881 RVA: 0x0000B566 File Offset: 0x00009766
		[Editor(false)]
		public int CurrentObjectiveType
		{
			get
			{
				return this._currentObjectiveType;
			}
			set
			{
				if (this._currentObjectiveType != value)
				{
					this._currentObjectiveType = value;
					base.OnPropertyChanged(value, "CurrentObjectiveType");
					this.UpdateAnimQueue();
				}
			}
		}

		// Token: 0x04000168 RID: 360
		private const float LongStayTime = 1f;

		// Token: 0x04000169 RID: 361
		private const float ShortStayTime = 0.1f;

		// Token: 0x0400016A RID: 362
		private const float FadeInTime = 0.15f;

		// Token: 0x0400016B RID: 363
		private const float FadeOutTime = 0.15f;

		// Token: 0x0400016C RID: 364
		private const float SingleMovementDirection = 20f;

		// Token: 0x0400016D RID: 365
		private const float MovementTime = 0.15f;

		// Token: 0x0400016E RID: 366
		private const float ParentActiveAlpha = 0.1f;

		// Token: 0x04000175 RID: 373
		private Queue<List<MouseAnimStage>> _animQueue = new Queue<List<MouseAnimStage>>();

		// Token: 0x04000176 RID: 374
		private int _currentObjectiveType;
	}
}
