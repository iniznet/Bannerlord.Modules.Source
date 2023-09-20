using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000035 RID: 53
	public class MissionDuelLandmarkMarkerVM : ViewModel
	{
		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000453 RID: 1107 RVA: 0x00013D82 File Offset: 0x00011F82
		// (set) Token: 0x06000454 RID: 1108 RVA: 0x00013D8A File Offset: 0x00011F8A
		public bool IsInScreenBoundaries { get; private set; }

		// Token: 0x06000455 RID: 1109 RVA: 0x00013D93 File Offset: 0x00011F93
		public MissionDuelLandmarkMarkerVM(GameEntity entity)
		{
			this.Entity = entity;
			this.FocusableComponent = this.Entity.GetFirstScriptOfType<DuelZoneLandmark>();
			this.TroopType = (int)this.Entity.GetFirstScriptOfType<DuelZoneLandmark>().ZoneTroopType;
			this.RefreshValues();
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00013DD0 File Offset: 0x00011FD0
		public override void RefreshValues()
		{
			base.RefreshValues();
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
			GameTexts.SetVariable("KEY", keyHyperlinkText);
			GameTexts.SetVariable("ACTION", new TextObject("{=7jMnNlXG}Change Arena Preference", null));
			this.ActionDescriptionText = GameTexts.FindText("str_key_action", null).ToString();
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00013E2C File Offset: 0x0001202C
		public void UpdateScreenPosition(Camera missionCamera)
		{
			Vec3 globalPosition = this.Entity.GlobalPosition;
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			MBWindowManager.WorldToScreen(missionCamera, globalPosition, ref this._latestX, ref this._latestY, ref this._latestW);
			this.IsInScreenBoundaries = this._latestW > 0f && (this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f) && this._latestY + 100f >= 0f;
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x00013EDC File Offset: 0x000120DC
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x00013EE4 File Offset: 0x000120E4
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

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x00013F02 File Offset: 0x00012102
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x00013F0A File Offset: 0x0001210A
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

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x00013F28 File Offset: 0x00012128
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x00013F30 File Offset: 0x00012130
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

		// Token: 0x04000228 RID: 552
		public readonly GameEntity Entity;

		// Token: 0x04000229 RID: 553
		public readonly IFocusable FocusableComponent;

		// Token: 0x0400022A RID: 554
		private float _latestX;

		// Token: 0x0400022B RID: 555
		private float _latestY;

		// Token: 0x0400022C RID: 556
		private float _latestW;

		// Token: 0x0400022E RID: 558
		private bool _isFocused;

		// Token: 0x0400022F RID: 559
		private int _troopType;

		// Token: 0x04000230 RID: 560
		private string _actionDescriptionText;
	}
}
