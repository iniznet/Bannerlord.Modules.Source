using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x02000018 RID: 24
	public class InformationVM : ViewModel
	{
		// Token: 0x0600010A RID: 266 RVA: 0x0000400E File Offset: 0x0000220E
		public InformationVM()
		{
			this.Tooltip = new PropertyBasedTooltipVM();
			this.Hint = new HintVM();
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000402C File Offset: 0x0000222C
		public void Tick(float dt)
		{
			this.Tooltip.Tick(dt);
			this.Hint.Tick(dt);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00004046 File Offset: 0x00002246
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Tooltip.OnFinalize();
			this.Hint.OnFinalize();
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00004064 File Offset: 0x00002264
		// (set) Token: 0x0600010E RID: 270 RVA: 0x0000406C File Offset: 0x0000226C
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

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600010F RID: 271 RVA: 0x0000408A File Offset: 0x0000228A
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00004092 File Offset: 0x00002292
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

		// Token: 0x0400006C RID: 108
		private PropertyBasedTooltipVM _tooltip;

		// Token: 0x0400006D RID: 109
		private HintVM _hint;
	}
}
