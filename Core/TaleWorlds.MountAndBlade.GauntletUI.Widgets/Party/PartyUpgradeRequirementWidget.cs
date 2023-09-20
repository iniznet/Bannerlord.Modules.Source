using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x02000061 RID: 97
	public class PartyUpgradeRequirementWidget : Widget
	{
		// Token: 0x0600052C RID: 1324 RVA: 0x0000FBE4 File Offset: 0x0000DDE4
		public PartyUpgradeRequirementWidget(UIContext context)
			: base(context)
		{
			this.NormalColor = new Color(1f, 1f, 1f, 1f);
			this.InsufficientColor = new Color(0.753f, 0.071f, 0.098f, 1f);
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x0000FC40 File Offset: 0x0000DE40
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._requiresRefresh)
			{
				if (this.RequirementId != null)
				{
					string text = (this.IsPerkRequirement ? "SPGeneral\\Skills\\gui_skills_icon_" : "StdAssets\\ItemIcons\\");
					string text2 = (this.IsPerkRequirement ? "_tiny" : "");
					base.Sprite = base.Context.SpriteData.GetSprite(text + this.RequirementId + text2);
				}
				base.Color = (this.IsSufficient ? this.NormalColor : this.InsufficientColor);
				this._requiresRefresh = false;
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x0600052E RID: 1326 RVA: 0x0000FCD4 File Offset: 0x0000DED4
		// (set) Token: 0x0600052F RID: 1327 RVA: 0x0000FCDC File Offset: 0x0000DEDC
		[Editor(false)]
		public string RequirementId
		{
			get
			{
				return this._requirementId;
			}
			set
			{
				if (value != this._requirementId)
				{
					this._requirementId = value;
					base.OnPropertyChanged<string>(value, "RequirementId");
					this._requiresRefresh = true;
				}
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000530 RID: 1328 RVA: 0x0000FD06 File Offset: 0x0000DF06
		// (set) Token: 0x06000531 RID: 1329 RVA: 0x0000FD0E File Offset: 0x0000DF0E
		[Editor(false)]
		public bool IsSufficient
		{
			get
			{
				return this._isSufficient;
			}
			set
			{
				if (value != this._isSufficient)
				{
					this._isSufficient = value;
					base.OnPropertyChanged(value, "IsSufficient");
					this._requiresRefresh = true;
				}
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000532 RID: 1330 RVA: 0x0000FD33 File Offset: 0x0000DF33
		// (set) Token: 0x06000533 RID: 1331 RVA: 0x0000FD3B File Offset: 0x0000DF3B
		[Editor(false)]
		public bool IsPerkRequirement
		{
			get
			{
				return this._isPerkRequirement;
			}
			set
			{
				if (value != this._isPerkRequirement)
				{
					this._isPerkRequirement = value;
					base.OnPropertyChanged(value, "IsPerkRequirement");
					this._requiresRefresh = true;
				}
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000534 RID: 1332 RVA: 0x0000FD60 File Offset: 0x0000DF60
		// (set) Token: 0x06000535 RID: 1333 RVA: 0x0000FD68 File Offset: 0x0000DF68
		public Color NormalColor
		{
			get
			{
				return this._normalColor;
			}
			set
			{
				if (value != this._normalColor)
				{
					this._normalColor = value;
					this._requiresRefresh = true;
				}
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x0000FD86 File Offset: 0x0000DF86
		// (set) Token: 0x06000537 RID: 1335 RVA: 0x0000FD8E File Offset: 0x0000DF8E
		public Color InsufficientColor
		{
			get
			{
				return this._insufficientColor;
			}
			set
			{
				if (value != this._insufficientColor)
				{
					this._insufficientColor = value;
					this._requiresRefresh = true;
				}
			}
		}

		// Token: 0x0400023D RID: 573
		private bool _requiresRefresh = true;

		// Token: 0x0400023E RID: 574
		private string _requirementId;

		// Token: 0x0400023F RID: 575
		private bool _isSufficient;

		// Token: 0x04000240 RID: 576
		private bool _isPerkRequirement;

		// Token: 0x04000241 RID: 577
		private Color _normalColor;

		// Token: 0x04000242 RID: 578
		private Color _insufficientColor;
	}
}
