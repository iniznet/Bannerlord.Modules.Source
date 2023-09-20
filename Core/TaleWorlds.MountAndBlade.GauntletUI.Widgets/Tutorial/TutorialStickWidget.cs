using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x02000045 RID: 69
	public class TutorialStickWidget : Widget
	{
		// Token: 0x060003A5 RID: 933 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public TutorialStickWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x0000BDC2 File Offset: 0x00009FC2
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x0000BDCA File Offset: 0x00009FCA
		public Widget GhostMouseVisualWidget { get; set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x0000BDD3 File Offset: 0x00009FD3
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x0000BDDB File Offset: 0x00009FDB
		public Widget LeftMouseClickVisualWidget { get; set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060003AA RID: 938 RVA: 0x0000BDE4 File Offset: 0x00009FE4
		// (set) Token: 0x060003AB RID: 939 RVA: 0x0000BDEC File Offset: 0x00009FEC
		public Widget RightMouseClickVisualWidget { get; set; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060003AC RID: 940 RVA: 0x0000BDF5 File Offset: 0x00009FF5
		// (set) Token: 0x060003AD RID: 941 RVA: 0x0000BDFD File Offset: 0x00009FFD
		public Widget HorizontalArrowWidget { get; set; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060003AE RID: 942 RVA: 0x0000BE06 File Offset: 0x0000A006
		// (set) Token: 0x060003AF RID: 943 RVA: 0x0000BE0E File Offset: 0x0000A00E
		public Widget VerticalArrowWidget { get; set; }

		// Token: 0x060003B0 RID: 944 RVA: 0x0000BE18 File Offset: 0x0000A018
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

		// Token: 0x060003B1 RID: 945 RVA: 0x0000BEC8 File Offset: 0x0000A0C8
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

		// Token: 0x060003B2 RID: 946 RVA: 0x0000BF68 File Offset: 0x0000A168
		private void UpdateAnimQueue()
		{
			this.Reset();
			switch (this.CurrentObjectiveType)
			{
			case 1:
				this.HorizontalArrowWidget.HorizontalFlip = true;
				this._animQueue.Enqueue(new List<MouseAnimStage>
				{
					MouseAnimStage.CreateFadeInStage(0.15f, this, true),
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
					MouseAnimStage.CreateFadeInStage(0.15f, this, true),
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
					MouseAnimStage.CreateFadeInStage(0.15f, this, true),
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
					MouseAnimStage.CreateFadeInStage(0.15f, this, true),
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
					MouseAnimStage.CreateFadeInStage(0.15f, this, true),
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
					MouseAnimStage.CreateFadeInStage(0.15f, this, true),
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
					MouseAnimStage.CreateFadeInStage(0.15f, this, true),
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
					MouseAnimStage.CreateFadeInStage(0.15f, this, true),
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

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060003B3 RID: 947 RVA: 0x0000C5CA File Offset: 0x0000A7CA
		// (set) Token: 0x060003B4 RID: 948 RVA: 0x0000C5D2 File Offset: 0x0000A7D2
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

		// Token: 0x0400018D RID: 397
		private const float LongStayTime = 1f;

		// Token: 0x0400018E RID: 398
		private const float ShortStayTime = 0.1f;

		// Token: 0x0400018F RID: 399
		private const float FadeInTime = 0.15f;

		// Token: 0x04000190 RID: 400
		private const float FadeOutTime = 0.15f;

		// Token: 0x04000191 RID: 401
		private const float SingleMovementDirection = 20f;

		// Token: 0x04000192 RID: 402
		private const float MovementTime = 0.15f;

		// Token: 0x04000193 RID: 403
		private const float ParentActiveAlpha = 0.1f;

		// Token: 0x04000199 RID: 409
		private Queue<List<MouseAnimStage>> _animQueue = new Queue<List<MouseAnimStage>>();

		// Token: 0x0400019A RID: 410
		private int _currentObjectiveType;
	}
}
