using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate
{
	public class NameplateVM : ViewModel
	{
		public double Scale { get; set; }

		public int NameplateOrder { get; set; }

		public NameplateVM()
		{
			if (Game.Current != null)
			{
				Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementChanged));
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementChanged));
		}

		private void OnTutorialNotificationElementChanged(TutorialNotificationElementChangeEvent obj)
		{
			this.RefreshTutorialStatus(obj.NewNotificationElementID);
		}

		public virtual void Initialize(GameEntity strategicEntity)
		{
			this.SizeType = 1;
		}

		public virtual void RefreshDynamicProperties(bool forceUpdate)
		{
		}

		public virtual void RefreshPosition()
		{
		}

		public virtual void RefreshRelationStatus()
		{
		}

		public virtual void RefreshTutorialStatus(string newTutorialHighlightElementID)
		{
		}

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

		protected bool _bindIsTargetedByTutorial;

		private Vec2 _position;

		private bool _isVisibleOnMap;

		private string _factionColor;

		private int _sizeType;

		private bool _isTargetedByTutorial;

		private float _distanceToCamera;

		protected enum NameplateSize
		{
			Small,
			Normal,
			Big
		}
	}
}
