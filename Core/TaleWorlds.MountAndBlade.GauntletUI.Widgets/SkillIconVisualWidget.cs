using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class SkillIconVisualWidget : Widget
	{
		public SkillIconVisualWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._requiresRefresh)
			{
				string text = "SPGeneral\\Skills\\gui_skills_icon_" + this.SkillId.ToLower();
				if (this.UseSmallestVariation && base.Context.SpriteData.GetSprite(text + "_tiny") != null)
				{
					base.Sprite = base.Context.SpriteData.GetSprite(text + "_tiny");
				}
				else if (this.UseSmallVariation && base.Context.SpriteData.GetSprite(text + "_small") != null)
				{
					base.Sprite = base.Context.SpriteData.GetSprite(text + "_small");
				}
				else if (base.Context.SpriteData.GetSprite(text) != null)
				{
					base.Sprite = base.Context.SpriteData.GetSprite(text);
				}
				this._requiresRefresh = false;
			}
		}

		[Editor(false)]
		public string SkillId
		{
			get
			{
				return this._skillId;
			}
			set
			{
				if (this._skillId != value)
				{
					this._skillId = value;
					base.OnPropertyChanged<string>(value, "SkillId");
					this._requiresRefresh = true;
				}
			}
		}

		[Editor(false)]
		public bool UseSmallVariation
		{
			get
			{
				return this._useSmallVariation;
			}
			set
			{
				if (this._useSmallVariation != value)
				{
					this._useSmallVariation = value;
					base.OnPropertyChanged(value, "UseSmallVariation");
					this._requiresRefresh = true;
				}
			}
		}

		[Editor(false)]
		public bool UseSmallestVariation
		{
			get
			{
				return this._useSmallestVariation;
			}
			set
			{
				if (this._useSmallestVariation != value)
				{
					this._useSmallestVariation = value;
					base.OnPropertyChanged(value, "UseSmallestVariation");
					this._requiresRefresh = true;
				}
			}
		}

		private bool _requiresRefresh = true;

		private string _skillId;

		private bool _useSmallVariation;

		private bool _useSmallestVariation;
	}
}
