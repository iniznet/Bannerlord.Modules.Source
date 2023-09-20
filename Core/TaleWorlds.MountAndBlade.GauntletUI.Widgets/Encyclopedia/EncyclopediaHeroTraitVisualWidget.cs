using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	// Token: 0x02000138 RID: 312
	public class EncyclopediaHeroTraitVisualWidget : Widget
	{
		// Token: 0x06001076 RID: 4214 RVA: 0x0002E37F File Offset: 0x0002C57F
		public EncyclopediaHeroTraitVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x0002E388 File Offset: 0x0002C588
		private void SetVisual(string traitCode, int value)
		{
			if (!string.IsNullOrEmpty(traitCode))
			{
				string text = string.Concat(new object[]
				{
					"SPGeneral\\SPTraits\\",
					traitCode.ToLower(),
					"_",
					value
				});
				base.Sprite = base.Context.SpriteData.GetSprite(text);
				base.Sprite = base.Context.SpriteData.GetSprite(text);
				if (value < 0)
				{
					base.Color = new Color(0.738f, 0.113f, 0.113f, 1f);
					return;
				}
				base.Color = new Color(0.992f, 0.75f, 0.33f, 1f);
			}
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06001078 RID: 4216 RVA: 0x0002E43F File Offset: 0x0002C63F
		// (set) Token: 0x06001079 RID: 4217 RVA: 0x0002E447 File Offset: 0x0002C647
		[Editor(false)]
		public string TraitId
		{
			get
			{
				return this._traitId;
			}
			set
			{
				if (this._traitId != value)
				{
					this._traitId = value;
					base.OnPropertyChanged<string>(value, "TraitId");
					this.SetVisual(value, this.TraitValue);
				}
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x0600107A RID: 4218 RVA: 0x0002E477 File Offset: 0x0002C677
		// (set) Token: 0x0600107B RID: 4219 RVA: 0x0002E47F File Offset: 0x0002C67F
		[Editor(false)]
		public int TraitValue
		{
			get
			{
				return this._traitValue;
			}
			set
			{
				if (this._traitValue != value)
				{
					this._traitValue = value;
					base.OnPropertyChanged(value, "TraitValue");
					this.SetVisual(this.TraitId, value);
				}
			}
		}

		// Token: 0x04000792 RID: 1938
		private string _traitId;

		// Token: 0x04000793 RID: 1939
		private int _traitValue;
	}
}
