using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class MapNotificationItemBaseVM : ViewModel
	{
		public INavigationHandler NavigationHandler { get; private set; }

		private protected Action<Vec2> FastMoveCameraToPosition { protected get; private set; }

		public InformationData Data { get; private set; }

		public MapNotificationItemBaseVM(InformationData data)
		{
			this.Data = data;
			this.ForceInspection = false;
			this.SoundId = data.SoundEventPath;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = this.Data.TitleText.ToString();
			this.DescriptionText = this.Data.DescriptionText.ToString();
			this._removeHintText = this._removeHintTextObject.ToString();
		}

		public void SetNavigationHandler(INavigationHandler navigationHandler)
		{
			this.NavigationHandler = navigationHandler;
		}

		public void SetFastMoveCameraToPosition(Action<Vec2> fastMoveCameraToPosition)
		{
			this.FastMoveCameraToPosition = fastMoveCameraToPosition;
		}

		public void ExecuteAction()
		{
			Action onInspect = this._onInspect;
			if (onInspect == null)
			{
				return;
			}
			onInspect();
		}

		public void ExecuteRemove()
		{
			Action<MapNotificationItemBaseVM> onRemove = this.OnRemove;
			if (onRemove != null)
			{
				onRemove(this);
			}
			Action<MapNotificationItemBaseVM> onFocus = this.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(null);
		}

		public void ExecuteSetFocused()
		{
			this.IsFocused = true;
			Action<MapNotificationItemBaseVM> onFocus = this.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(this);
		}

		public void ExecuteSetUnfocused()
		{
			this.IsFocused = false;
			Action<MapNotificationItemBaseVM> onFocus = this.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(null);
		}

		public virtual void ManualRefreshRelevantStatus()
		{
		}

		internal void GoToMapPosition(Vec2 position)
		{
			Action<Vec2> fastMoveCameraToPosition = this.FastMoveCameraToPosition;
			if (fastMoveCameraToPosition == null)
			{
				return;
			}
			fastMoveCameraToPosition(position);
		}

		public void SetRemoveInputKey(HotKey hotKey)
		{
			this.RemoveInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM RemoveInputKey
		{
			get
			{
				return this._removeInputKey;
			}
			set
			{
				if (value != this._removeInputKey)
				{
					this._removeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RemoveInputKey");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
					Action<MapNotificationItemBaseVM> onFocus = this.OnFocus;
					if (onFocus == null)
					{
						return;
					}
					onFocus(this);
				}
			}
		}

		[DataSourceProperty]
		public string NotificationIdentifier
		{
			get
			{
				return this._notificationIdentifier;
			}
			set
			{
				if (value != this._notificationIdentifier)
				{
					this._notificationIdentifier = value;
					base.OnPropertyChangedWithValue<string>(value, "NotificationIdentifier");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public bool ForceInspection
		{
			get
			{
				return this._forceInspection;
			}
			set
			{
				if (value != this._forceInspection)
				{
					Game game = Game.Current;
					if (game != null && !game.IsDevelopmentMode)
					{
						this._forceInspection = value;
						base.OnPropertyChangedWithValue(value, "ForceInspection");
					}
				}
			}
		}

		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public string SoundId
		{
			get
			{
				return this._soundId;
			}
			set
			{
				if (value != this._soundId)
				{
					this._soundId = value;
					base.OnPropertyChangedWithValue<string>(value, "SoundId");
				}
			}
		}

		internal Action<MapNotificationItemBaseVM> OnRemove;

		internal Action<MapNotificationItemBaseVM> OnFocus;

		protected Action _onInspect;

		private readonly TextObject _removeHintTextObject = new TextObject("{=Bcs9s2tC}Right Click to Remove", null);

		private string _removeHintText;

		private InputKeyItemVM _removeInputKey;

		private bool _isFocused;

		private string _titleText;

		private string _descriptionText;

		private string _soundId;

		private bool _forceInspection;

		private string _notificationIdentifier = "Default";
	}
}
