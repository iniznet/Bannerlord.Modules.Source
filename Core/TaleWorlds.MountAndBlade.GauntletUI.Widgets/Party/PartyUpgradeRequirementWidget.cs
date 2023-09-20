using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyUpgradeRequirementWidget : Widget
	{
		public PartyUpgradeRequirementWidget(UIContext context)
			: base(context)
		{
			this.NormalColor = new Color(1f, 1f, 1f, 1f);
			this.InsufficientColor = new Color(0.753f, 0.071f, 0.098f, 1f);
		}

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

		private bool _requiresRefresh = true;

		private string _requirementId;

		private bool _isSufficient;

		private bool _isPerkRequirement;

		private Color _normalColor;

		private Color _insufficientColor;
	}
}
