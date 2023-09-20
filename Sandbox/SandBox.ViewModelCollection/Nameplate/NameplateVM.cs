using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate
{
	// Token: 0x02000013 RID: 19
	public class NameplateVM : ViewModel
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x000093E8 File Offset: 0x000075E8
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x000093F0 File Offset: 0x000075F0
		public double Scale { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x000093F9 File Offset: 0x000075F9
		// (set) Token: 0x060001C6 RID: 454 RVA: 0x00009401 File Offset: 0x00007601
		public int NameplateOrder { get; set; }

		// Token: 0x060001C7 RID: 455 RVA: 0x0000940A File Offset: 0x0000760A
		public NameplateVM()
		{
			if (Game.Current != null)
			{
				Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementChanged));
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00009434 File Offset: 0x00007634
		public override void OnFinalize()
		{
			base.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementChanged));
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00009457 File Offset: 0x00007657
		private void OnTutorialNotificationElementChanged(TutorialNotificationElementChangeEvent obj)
		{
			this.RefreshTutorialStatus(obj.NewNotificationElementID);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00009465 File Offset: 0x00007665
		public virtual void Initialize(GameEntity strategicEntity)
		{
			this.SizeType = 1;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000946E File Offset: 0x0000766E
		public virtual void RefreshDynamicProperties(bool forceUpdate)
		{
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00009470 File Offset: 0x00007670
		public virtual void RefreshPosition()
		{
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00009472 File Offset: 0x00007672
		public virtual void RefreshRelationStatus()
		{
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00009474 File Offset: 0x00007674
		public virtual void RefreshTutorialStatus(string newTutorialHighlightElementID)
		{
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001CF RID: 463 RVA: 0x00009476 File Offset: 0x00007676
		// (set) Token: 0x060001D0 RID: 464 RVA: 0x0000947E File Offset: 0x0000767E
		public int SizeType
		{
			get
			{
				return this._sizeType;
			}
			set
			{
				if (value != this._sizeType)
				{
					this._sizeType = value;
					base.OnPropertyChangedWithValue(value, "SizeType");
				}
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x0000949C File Offset: 0x0000769C
		// (set) Token: 0x060001D2 RID: 466 RVA: 0x000094A4 File Offset: 0x000076A4
		public string FactionColor
		{
			get
			{
				return this._factionColor;
			}
			set
			{
				if (value != this._factionColor)
				{
					this._factionColor = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionColor");
				}
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x000094C7 File Offset: 0x000076C7
		// (set) Token: 0x060001D4 RID: 468 RVA: 0x000094CF File Offset: 0x000076CF
		public float DistanceToCamera
		{
			get
			{
				return this._distanceToCamera;
			}
			set
			{
				if (value != this._distanceToCamera)
				{
					this._distanceToCamera = value;
					base.OnPropertyChangedWithValue(value, "DistanceToCamera");
				}
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x000094ED File Offset: 0x000076ED
		// (set) Token: 0x060001D6 RID: 470 RVA: 0x000094F5 File Offset: 0x000076F5
		public bool IsVisibleOnMap
		{
			get
			{
				return this._isVisibleOnMap;
			}
			set
			{
				if (value != this._isVisibleOnMap)
				{
					this._isVisibleOnMap = value;
					base.OnPropertyChangedWithValue(value, "IsVisibleOnMap");
				}
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x00009513 File Offset: 0x00007713
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x0000951B File Offset: 0x0000771B
		public bool IsTargetedByTutorial
		{
			get
			{
				return this._isTargetedByTutorial;
			}
			set
			{
				if (value != this._isTargetedByTutorial)
				{
					this._isTargetedByTutorial = value;
					base.OnPropertyChangedWithValue(value, "IsTargetedByTutorial");
					base.OnPropertyChanged("ShouldShowFullName");
					base.OnPropertyChanged("IsTracked");
				}
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x0000954F File Offset: 0x0000774F
		// (set) Token: 0x060001DA RID: 474 RVA: 0x00009557 File Offset: 0x00007757
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChangedWithValue(value, "Position");
				}
			}
		}

		// Token: 0x040000C1 RID: 193
		protected bool _bindIsTargetedByTutorial;

		// Token: 0x040000C2 RID: 194
		private Vec2 _position;

		// Token: 0x040000C3 RID: 195
		private bool _isVisibleOnMap;

		// Token: 0x040000C4 RID: 196
		private string _factionColor;

		// Token: 0x040000C5 RID: 197
		private int _sizeType;

		// Token: 0x040000C6 RID: 198
		private bool _isTargetedByTutorial;

		// Token: 0x040000C7 RID: 199
		private float _distanceToCamera;

		// Token: 0x0200005C RID: 92
		protected enum NameplateSize
		{
			// Token: 0x040002BD RID: 701
			Small,
			// Token: 0x040002BE RID: 702
			Normal,
			// Token: 0x040002BF RID: 703
			Big
		}
	}
}
