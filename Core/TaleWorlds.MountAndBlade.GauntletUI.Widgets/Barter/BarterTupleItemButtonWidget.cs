using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Barter
{
	public class BarterTupleItemButtonWidget : ButtonWidget
	{
		public ListPanel SliderParentList { get; set; }

		public TextWidget CountText { get; set; }

		public BarterTupleItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.Refresh();
				this._initialized = true;
			}
		}

		private void Refresh()
		{
			this.SliderParentList.IsVisible = this.IsMultiple && this.IsOffered;
			this.CountText.IsHidden = this.IsMultiple && this.IsOffered;
			base.IsSelected = this.IsOffered;
			base.DoNotAcceptEvents = this.IsOffered;
		}

		[Editor(false)]
		public bool IsMultiple
		{
			get
			{
				return this._isMultiple;
			}
			set
			{
				if (this._isMultiple != value)
				{
					this._isMultiple = value;
					base.OnPropertyChanged(value, "IsMultiple");
					this.Refresh();
				}
			}
		}

		[Editor(false)]
		public bool IsOffered
		{
			get
			{
				return this._isOffered;
			}
			set
			{
				if (this._isOffered != value)
				{
					this._isOffered = value;
					base.OnPropertyChanged(value, "IsOffered");
					this.Refresh();
				}
			}
		}

		private bool _initialized;

		private bool _isMultiple;

		private bool _isOffered;
	}
}
