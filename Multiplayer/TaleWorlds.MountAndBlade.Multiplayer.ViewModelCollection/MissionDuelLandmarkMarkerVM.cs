using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MissionDuelLandmarkMarkerVM : ViewModel
	{
		public bool IsInScreenBoundaries { get; private set; }

		public MissionDuelLandmarkMarkerVM(GameEntity entity)
		{
			this.Entity = entity;
			this.FocusableComponent = this.Entity.GetFirstScriptOfType<DuelZoneLandmark>();
			this.TroopType = this.Entity.GetFirstScriptOfType<DuelZoneLandmark>().ZoneTroopType;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
			GameTexts.SetVariable("KEY", keyHyperlinkText);
			GameTexts.SetVariable("ACTION", new TextObject("{=7jMnNlXG}Change Arena Preference", null));
			this.ActionDescriptionText = GameTexts.FindText("str_key_action", null).ToString();
		}

		public void UpdateScreenPosition(Camera missionCamera)
		{
			Vec3 globalPosition = this.Entity.GlobalPosition;
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			MBWindowManager.WorldToScreen(missionCamera, globalPosition, ref this._latestX, ref this._latestY, ref this._latestW);
			this.IsInScreenBoundaries = this._latestW > 0f && (this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f) && this._latestY + 100f >= 0f;
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
				}
			}
		}

		[DataSourceProperty]
		public int TroopType
		{
			get
			{
				return this._troopType;
			}
			set
			{
				if (value != this._troopType)
				{
					this._troopType = value;
					base.OnPropertyChangedWithValue(value, "TroopType");
				}
			}
		}

		[DataSourceProperty]
		public string ActionDescriptionText
		{
			get
			{
				return this._actionDescriptionText;
			}
			set
			{
				if (value != this._actionDescriptionText)
				{
					this._actionDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionDescriptionText");
				}
			}
		}

		public readonly GameEntity Entity;

		public readonly IFocusable FocusableComponent;

		private float _latestX;

		private float _latestY;

		private float _latestW;

		private bool _isFocused;

		private int _troopType;

		private string _actionDescriptionText;
	}
}
