using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class InformationVM : ViewModel
	{
		public InformationVM()
		{
			this.Tooltip = new PropertyBasedTooltipVM();
			this.Hint = new HintVM();
		}

		public void Tick(float dt)
		{
			this.Tooltip.Tick(dt);
			this.Hint.Tick(dt);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Tooltip.OnFinalize();
			this.Hint.OnFinalize();
		}

		[DataSourceProperty]
		public PropertyBasedTooltipVM Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value != this._tooltip)
				{
					this._tooltip = value;
					base.OnPropertyChangedWithValue<PropertyBasedTooltipVM>(value, "Tooltip");
				}
			}
		}

		[DataSourceProperty]
		public HintVM Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintVM>(value, "Hint");
				}
			}
		}

		private PropertyBasedTooltipVM _tooltip;

		private HintVM _hint;
	}
}
