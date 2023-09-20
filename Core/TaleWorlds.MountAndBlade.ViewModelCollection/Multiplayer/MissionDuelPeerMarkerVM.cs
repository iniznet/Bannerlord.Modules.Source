using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000037 RID: 55
	public class MissionDuelPeerMarkerVM : ViewModel
	{
		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000477 RID: 1143 RVA: 0x00014937 File Offset: 0x00012B37
		// (set) Token: 0x06000478 RID: 1144 RVA: 0x0001493F File Offset: 0x00012B3F
		public MissionPeer TargetPeer { get; private set; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000479 RID: 1145 RVA: 0x00014948 File Offset: 0x00012B48
		// (set) Token: 0x0600047A RID: 1146 RVA: 0x00014950 File Offset: 0x00012B50
		public float Distance { get; private set; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x0600047B RID: 1147 RVA: 0x00014959 File Offset: 0x00012B59
		// (set) Token: 0x0600047C RID: 1148 RVA: 0x00014961 File Offset: 0x00012B61
		public bool IsInDuel { get; private set; }

		// Token: 0x0600047D RID: 1149 RVA: 0x0001496C File Offset: 0x00012B6C
		public MissionDuelPeerMarkerVM(MissionPeer peer)
		{
			this.TargetPeer = peer;
			this.Bounty = (peer.Representative as DuelMissionRepresentative).Bounty;
			this.IsEnabled = true;
			TargetIconType iconType = MultiplayerClassDivisions.GetMPHeroClassForPeer(this.TargetPeer, false).IconType;
			this.CompassElement = new MPTeammateCompassTargetVM(iconType, Color.White.ToUnsignedInteger(), Color.White.ToUnsignedInteger(), BannerCode.CreateFrom(new Banner()), true);
			this.SelectedPerks = new MBBindingList<MPPerkVM>();
			this.RefreshPerkSelection();
			this.RefreshValues();
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x00014A00 File Offset: 0x00012C00
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.TargetPeer.DisplayedName;
			this._acceptDuelRequestText = new TextObject("{=tidE1V1k}Accept duel", null);
			this._sendDuelRequestText = new TextObject("{=YLPJWgqF}Challenge", null);
			this._waitingForDuelResponseText = new TextObject("{=MPgnsZoo}Waiting for response", null);
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x00014A58 File Offset: 0x00012C58
		public void OnTick(float dt)
		{
			if (Agent.Main != null && this.TargetPeer.ControlledAgent != null)
			{
				this.Distance = this._latestW;
			}
			if (this.HasSentDuelRequest)
			{
				this._currentDuelRequestTimeRemaining -= dt;
				GameTexts.SetVariable("SECONDS", (int)this._currentDuelRequestTimeRemaining);
				GameTexts.SetVariable("ACTION", this._waitingForDuelResponseText);
				this.ActionDescriptionText = new TextObject("{=HXWpxvgT}{ACTION} ({SECONDS})", null).ToString();
				if (this._currentDuelRequestTimeRemaining <= 0f)
				{
					this.HasSentDuelRequest = false;
				}
			}
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00014AE8 File Offset: 0x00012CE8
		public void UpdateScreenPosition(Camera missionCamera)
		{
			if (this.TargetPeer.ControlledAgent == null)
			{
				return;
			}
			Vec3 vec = this.TargetPeer.ControlledAgent.GetWorldPosition().GetGroundVec3();
			vec += new Vec3(0f, 0f, this.TargetPeer.ControlledAgent.GetEyeGlobalHeight(), -1f);
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			MBWindowManager.WorldToScreen(missionCamera, vec, ref this._latestX, ref this._latestY, ref this._latestW);
			this.ScreenPosition = new Vec2(this._latestX, this._latestY);
			this.IsAgentInScreenBoundaries = this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f && this._latestY + 100f >= 0f;
			this._wPosAfterPositionCalculation = ((this._latestW < 0f) ? (-1f) : 1.1f);
			this.WSign = (int)this._wPosAfterPositionCalculation;
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00014C14 File Offset: 0x00012E14
		private void OnInteractionChanged()
		{
			this.ActionDescriptionText = "";
			if (this.HasDuelRequestForPlayer)
			{
				string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
				GameTexts.SetVariable("KEY", keyHyperlinkText);
				GameTexts.SetVariable("ACTION", this._acceptDuelRequestText);
				this.ActionDescriptionText = GameTexts.FindText("str_key_action", null).ToString();
				return;
			}
			if (this.HasSentDuelRequest)
			{
				this._currentDuelRequestTimeRemaining = 10f;
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00014C8C File Offset: 0x00012E8C
		private void SetFocused(bool isFocused)
		{
			if (!this.HasDuelRequestForPlayer && !this.HasSentDuelRequest)
			{
				if (isFocused)
				{
					string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
					GameTexts.SetVariable("KEY", keyHyperlinkText);
					GameTexts.SetVariable("ACTION", this._sendDuelRequestText);
					this.ActionDescriptionText = GameTexts.FindText("str_key_action", null).ToString();
					return;
				}
				this.ActionDescriptionText = string.Empty;
			}
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00014CFB File Offset: 0x00012EFB
		public void UpdateBounty()
		{
			this.Bounty = (this.TargetPeer.Representative as DuelMissionRepresentative).Bounty;
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00014D18 File Offset: 0x00012F18
		private void UpdateTracked()
		{
			if (!this.IsEnabled)
			{
				this.IsTracked = false;
			}
			else if (this.HasDuelRequestForPlayer || this.HasSentDuelRequest || this.IsFocused)
			{
				this.IsTracked = true;
			}
			else
			{
				this.IsTracked = false;
			}
			this.ShouldShowInformation = this.IsTracked || this.IsFocused;
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00014D75 File Offset: 0x00012F75
		public void OnDuelStarted()
		{
			this.IsEnabled = false;
			this.IsInDuel = true;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x00014D85 File Offset: 0x00012F85
		public void OnDuelEnded()
		{
			this.IsEnabled = true;
			this.IsInDuel = false;
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x00014D95 File Offset: 0x00012F95
		public void UpdateCurentDuelStatus(bool isInDuel)
		{
			this.IsInDuel = isInDuel;
			this.IsEnabled = !this.IsInDuel;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00014DB0 File Offset: 0x00012FB0
		public void RefreshPerkSelection()
		{
			this.SelectedPerks.Clear();
			foreach (MPPerkObject mpperkObject in this.TargetPeer.SelectedPerks)
			{
				this.SelectedPerks.Add(new MPPerkVM(null, mpperkObject, true, 0));
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x00014E20 File Offset: 0x00013020
		// (set) Token: 0x0600048A RID: 1162 RVA: 0x00014E28 File Offset: 0x00013028
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
					this.UpdateTracked();
				}
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x00014E4C File Offset: 0x0001304C
		// (set) Token: 0x0600048C RID: 1164 RVA: 0x00014E54 File Offset: 0x00013054
		[DataSourceProperty]
		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (value != this._isTracked)
				{
					this._isTracked = value;
					base.OnPropertyChangedWithValue(value, "IsTracked");
				}
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x00014E72 File Offset: 0x00013072
		// (set) Token: 0x0600048E RID: 1166 RVA: 0x00014E7A File Offset: 0x0001307A
		[DataSourceProperty]
		public bool ShouldShowInformation
		{
			get
			{
				return this._shouldShowInformation;
			}
			set
			{
				if (value != this._shouldShowInformation)
				{
					this._shouldShowInformation = value;
					base.OnPropertyChangedWithValue(value, "ShouldShowInformation");
				}
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00014E98 File Offset: 0x00013098
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x00014EA0 File Offset: 0x000130A0
		[DataSourceProperty]
		public bool IsAgentInScreenBoundaries
		{
			get
			{
				return this._isAgentInScreenBoundaries;
			}
			set
			{
				if (value != this._isAgentInScreenBoundaries)
				{
					this._isAgentInScreenBoundaries = value;
					base.OnPropertyChangedWithValue(value, "IsAgentInScreenBoundaries");
				}
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x00014EBE File Offset: 0x000130BE
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x00014EC6 File Offset: 0x000130C6
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
					this.SetFocused(value);
					this.UpdateTracked();
				}
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x00014EF1 File Offset: 0x000130F1
		// (set) Token: 0x06000494 RID: 1172 RVA: 0x00014EF9 File Offset: 0x000130F9
		[DataSourceProperty]
		public bool HasDuelRequestForPlayer
		{
			get
			{
				return this._hasDuelRequestForPlayer;
			}
			set
			{
				if (value != this._hasDuelRequestForPlayer)
				{
					this._hasDuelRequestForPlayer = value;
					base.OnPropertyChangedWithValue(value, "HasDuelRequestForPlayer");
					this.OnInteractionChanged();
					this.UpdateTracked();
				}
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000495 RID: 1173 RVA: 0x00014F23 File Offset: 0x00013123
		// (set) Token: 0x06000496 RID: 1174 RVA: 0x00014F2B File Offset: 0x0001312B
		[DataSourceProperty]
		public bool HasSentDuelRequest
		{
			get
			{
				return this._hasSentDuelRequest;
			}
			set
			{
				if (value != this._hasSentDuelRequest)
				{
					this._hasSentDuelRequest = value;
					base.OnPropertyChangedWithValue(value, "HasSentDuelRequest");
					this.OnInteractionChanged();
					this.UpdateTracked();
				}
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x00014F55 File Offset: 0x00013155
		// (set) Token: 0x06000498 RID: 1176 RVA: 0x00014F5D File Offset: 0x0001315D
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x00014F80 File Offset: 0x00013180
		// (set) Token: 0x0600049A RID: 1178 RVA: 0x00014F88 File Offset: 0x00013188
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

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x00014FAB File Offset: 0x000131AB
		// (set) Token: 0x0600049C RID: 1180 RVA: 0x00014FB3 File Offset: 0x000131B3
		[DataSourceProperty]
		public int Bounty
		{
			get
			{
				return this._bounty;
			}
			set
			{
				if (value != this._bounty)
				{
					this._bounty = value;
					base.OnPropertyChangedWithValue(value, "Bounty");
				}
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x00014FD1 File Offset: 0x000131D1
		// (set) Token: 0x0600049E RID: 1182 RVA: 0x00014FD9 File Offset: 0x000131D9
		[DataSourceProperty]
		public int PreferredArenaType
		{
			get
			{
				return this._preferredArenaType;
			}
			set
			{
				if (value != this._preferredArenaType)
				{
					this._preferredArenaType = value;
					base.OnPropertyChangedWithValue(value, "PreferredArenaType");
				}
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x0600049F RID: 1183 RVA: 0x00014FF7 File Offset: 0x000131F7
		// (set) Token: 0x060004A0 RID: 1184 RVA: 0x00014FFF File Offset: 0x000131FF
		[DataSourceProperty]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (value != this._wSign)
				{
					this._wSign = value;
					base.OnPropertyChangedWithValue(value, "WSign");
				}
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001501D File Offset: 0x0001321D
		// (set) Token: 0x060004A2 RID: 1186 RVA: 0x00015025 File Offset: 0x00013225
		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value.x != this._screenPosition.x || value.y != this._screenPosition.y)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060004A3 RID: 1187 RVA: 0x00015060 File Offset: 0x00013260
		// (set) Token: 0x060004A4 RID: 1188 RVA: 0x00015068 File Offset: 0x00013268
		[DataSourceProperty]
		public MPTeammateCompassTargetVM CompassElement
		{
			get
			{
				return this._compassElement;
			}
			set
			{
				if (value != this._compassElement)
				{
					this._compassElement = value;
					base.OnPropertyChangedWithValue<MPTeammateCompassTargetVM>(value, "CompassElement");
				}
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060004A5 RID: 1189 RVA: 0x00015086 File Offset: 0x00013286
		// (set) Token: 0x060004A6 RID: 1190 RVA: 0x0001508E File Offset: 0x0001328E
		[DataSourceProperty]
		public MBBindingList<MPPerkVM> SelectedPerks
		{
			get
			{
				return this._selectedPerks;
			}
			set
			{
				if (value != this._selectedPerks)
				{
					this._selectedPerks = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPerkVM>>(value, "SelectedPerks");
				}
			}
		}

		// Token: 0x04000246 RID: 582
		private float _currentDuelRequestTimeRemaining;

		// Token: 0x04000247 RID: 583
		private float _latestX;

		// Token: 0x04000248 RID: 584
		private float _latestY;

		// Token: 0x04000249 RID: 585
		private float _latestW;

		// Token: 0x0400024A RID: 586
		private float _wPosAfterPositionCalculation;

		// Token: 0x0400024B RID: 587
		private TextObject _acceptDuelRequestText;

		// Token: 0x0400024C RID: 588
		private TextObject _sendDuelRequestText;

		// Token: 0x0400024D RID: 589
		private TextObject _waitingForDuelResponseText;

		// Token: 0x0400024F RID: 591
		private bool _isEnabled;

		// Token: 0x04000250 RID: 592
		private bool _isTracked;

		// Token: 0x04000251 RID: 593
		private bool _shouldShowInformation;

		// Token: 0x04000252 RID: 594
		private bool _isAgentInScreenBoundaries;

		// Token: 0x04000253 RID: 595
		private bool _isFocused;

		// Token: 0x04000254 RID: 596
		private bool _hasDuelRequestForPlayer;

		// Token: 0x04000255 RID: 597
		private bool _hasSentDuelRequest;

		// Token: 0x04000256 RID: 598
		private string _name;

		// Token: 0x04000257 RID: 599
		private string _actionDescriptionText;

		// Token: 0x04000258 RID: 600
		private int _bounty;

		// Token: 0x04000259 RID: 601
		private int _preferredArenaType;

		// Token: 0x0400025A RID: 602
		private int _wSign;

		// Token: 0x0400025B RID: 603
		private Vec2 _screenPosition;

		// Token: 0x0400025C RID: 604
		private MPTeammateCompassTargetVM _compassElement;

		// Token: 0x0400025D RID: 605
		private MBBindingList<MPPerkVM> _selectedPerks;
	}
}
