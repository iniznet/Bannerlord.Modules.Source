using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000037 RID: 55
	public class SkillIconVisualWidget : Widget
	{
		// Token: 0x0600030A RID: 778 RVA: 0x00009D8D File Offset: 0x00007F8D
		public SkillIconVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00009DA0 File Offset: 0x00007FA0
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

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x0600030C RID: 780 RVA: 0x00009E96 File Offset: 0x00008096
		// (set) Token: 0x0600030D RID: 781 RVA: 0x00009E9E File Offset: 0x0000809E
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

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x0600030E RID: 782 RVA: 0x00009EC8 File Offset: 0x000080C8
		// (set) Token: 0x0600030F RID: 783 RVA: 0x00009ED0 File Offset: 0x000080D0
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

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000310 RID: 784 RVA: 0x00009EF5 File Offset: 0x000080F5
		// (set) Token: 0x06000311 RID: 785 RVA: 0x00009EFD File Offset: 0x000080FD
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

		// Token: 0x04000142 RID: 322
		private bool _requiresRefresh = true;

		// Token: 0x04000143 RID: 323
		private string _skillId;

		// Token: 0x04000144 RID: 324
		private bool _useSmallVariation;

		// Token: 0x04000145 RID: 325
		private bool _useSmallestVariation;
	}
}
