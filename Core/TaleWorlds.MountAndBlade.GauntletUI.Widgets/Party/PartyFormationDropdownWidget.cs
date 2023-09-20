using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyFormationDropdownWidget : DropdownWidget
	{
		public PartyFormationDropdownWidget(UIContext context)
			: base(context)
		{
			base.DoNotHandleDropdownListPanel = true;
		}

		private void ListStateChangerUpdated()
		{
		}

		private void SeperatorStateChangerUpdated()
		{
		}

		protected override void OpenPanel()
		{
			base.ListPanel.IsVisible = true;
			this.SeperatorStateChanger.IsVisible = true;
			this.ListStateChanger.Delay = this.SeperatorStateChanger.VisualDefinition.TransitionDuration;
			this.ListStateChanger.State = "Opened";
			this.ListStateChanger.Start();
			this.SeperatorStateChanger.Delay = 0f;
			this.SeperatorStateChanger.State = "Opened";
			this.SeperatorStateChanger.Start();
			base.Context.TwoDimensionContext.PlaySound("dropdown");
		}

		protected override void ClosePanel()
		{
			this.ListStateChanger.Delay = 0f;
			this.ListStateChanger.State = "Closed";
			this.ListStateChanger.Start();
			this.SeperatorStateChanger.Delay = this.ListStateChanger.TargetWidget.VisualDefinition.TransitionDuration;
			this.SeperatorStateChanger.State = "Closed";
			this.SeperatorStateChanger.Start();
			base.Context.TwoDimensionContext.PlaySound("dropdown");
		}

		[Editor(false)]
		public DelayedStateChanger SeperatorStateChanger
		{
			get
			{
				return this._seperatorStateChanger;
			}
			set
			{
				if (this._seperatorStateChanger != value)
				{
					this._seperatorStateChanger = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "SeperatorStateChanger");
					this.SeperatorStateChangerUpdated();
				}
			}
		}

		[Editor(false)]
		public DelayedStateChanger ListStateChanger
		{
			get
			{
				return this._listStateChanger;
			}
			set
			{
				if (this._listStateChanger != value)
				{
					this._listStateChanger = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "ListStateChanger");
					this.ListStateChangerUpdated();
				}
			}
		}

		private DelayedStateChanger _seperatorStateChanger;

		private DelayedStateChanger _listStateChanger;
	}
}
