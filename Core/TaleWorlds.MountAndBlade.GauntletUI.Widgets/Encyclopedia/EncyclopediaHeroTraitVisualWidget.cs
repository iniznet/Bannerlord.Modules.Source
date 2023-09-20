using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	public class EncyclopediaHeroTraitVisualWidget : Widget
	{
		public EncyclopediaHeroTraitVisualWidget(UIContext context)
			: base(context)
		{
		}

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

		private string _traitId;

		private int _traitValue;
	}
}
