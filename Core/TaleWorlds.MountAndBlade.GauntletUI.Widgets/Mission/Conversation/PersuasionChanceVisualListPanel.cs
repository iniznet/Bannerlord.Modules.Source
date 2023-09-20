using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.Conversation
{
	public class PersuasionChanceVisualListPanel : ListPanel
	{
		public bool IsFailChance { get; set; }

		public PersuasionChanceVisualListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.IsVisible = !this.IsFailChance && this.ChanceValue > 0;
		}

		public int ChanceValue
		{
			get
			{
				return this._chanceValue;
			}
			set
			{
				if (this._chanceValue != value)
				{
					this._chanceValue = value;
					base.OnPropertyChanged(value, "ChanceValue");
				}
			}
		}

		private int _chanceValue;
	}
}
