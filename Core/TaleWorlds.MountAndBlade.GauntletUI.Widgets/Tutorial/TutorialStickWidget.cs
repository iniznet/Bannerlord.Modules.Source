using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	public class TutorialStickWidget : Widget
	{
		public TutorialStickWidget(UIContext context)
			: base(context)
		{
		}

		public Widget GhostMouseVisualWidget { get; set; }

		public Widget LeftMouseClickVisualWidget { get; set; }

		public Widget RightMouseClickVisualWidget { get; set; }

		public Widget HorizontalArrowWidget { get; set; }

		public Widget VerticalArrowWidget { get; set; }

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._animQueue.Count > 0)
			{
				base.IsVisible = true;
				base.ParentWidget.ParentWidget.AlphaFactor = 0.5f;
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

		private void ResetAll()
		{
			this._animQueue.Clear();
			this.ResetAnim();
			this.GhostMouseVisualWidget.SetGlobalAlphaRecursively(0f);
			this.HorizontalArrowWidget.SetGlobalAlphaRecursively(0f);
			this.VerticalArrowWidget.SetGlobalAlphaRecursively(0f);
			base.ParentWidget.ParentWidget.AlphaFactor = 0f;
		}

		private void ResetAnim()
		{
			base.PositionXOffset = 0f;
			base.PositionYOffset = 0f;
			this.SetGlobalAlphaRecursively(0f);
			this.RightMouseClickVisualWidget.SetGlobalAlphaRecursively(0f);
			this.LeftMouseClickVisualWidget.SetGlobalAlphaRecursively(0f);
		}

		private void UpdateAnimQueue()
		{
			this.ResetAnim();
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
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(2f) });
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
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(2f) });
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
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(2f) });
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
				this._animQueue.Enqueue(new List<MouseAnimStage> { MouseAnimStage.CreateStayStage(2f) });
				return;
			default:
				return;
			}
		}

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
					this.ResetAll();
					this.UpdateAnimQueue();
				}
			}
		}

		private const float LongStayTime = 1f;

		private const float ShortStayTime = 0.1f;

		private const float FadeInTime = 0.15f;

		private const float FadeOutTime = 0.15f;

		private const float SingleMovementDirection = 20f;

		private const float MovementTime = 0.15f;

		private const float ParentActiveAlpha = 0.5f;

		private Queue<List<MouseAnimStage>> _animQueue = new Queue<List<MouseAnimStage>>();

		private int _currentObjectiveType;
	}
}
