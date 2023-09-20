using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.GameOver
{
	// Token: 0x02000036 RID: 54
	public class GameOverStatItemVM : ViewModel
	{
		// Token: 0x0600040C RID: 1036 RVA: 0x000126A7 File Offset: 0x000108A7
		public GameOverStatItemVM(StatItem item)
		{
			this._item = item;
			this.RefreshValues();
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x000126BC File Offset: 0x000108BC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DefinitionText = GameTexts.FindText("str_game_over_stat_item", this._item.ID).ToString();
			this.ValueText = this._item.Value;
			this.StatTypeAsString = Enum.GetName(typeof(StatItem.StatType), this._item.Type);
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x00012725 File Offset: 0x00010925
		// (set) Token: 0x0600040F RID: 1039 RVA: 0x0001272D File Offset: 0x0001092D
		[DataSourceProperty]
		public string DefinitionText
		{
			get
			{
				return this._definitionText;
			}
			set
			{
				if (value != this._definitionText)
				{
					this._definitionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DefinitionText");
				}
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x00012750 File Offset: 0x00010950
		// (set) Token: 0x06000411 RID: 1041 RVA: 0x00012758 File Offset: 0x00010958
		[DataSourceProperty]
		public string ValueText
		{
			get
			{
				return this._valueText;
			}
			set
			{
				if (value != this._valueText)
				{
					this._valueText = value;
					base.OnPropertyChangedWithValue<string>(value, "ValueText");
				}
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x0001277B File Offset: 0x0001097B
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x00012783 File Offset: 0x00010983
		[DataSourceProperty]
		public string StatTypeAsString
		{
			get
			{
				return this._statTypeAsString;
			}
			set
			{
				if (value != this._statTypeAsString)
				{
					this._statTypeAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "StatTypeAsString");
				}
			}
		}

		// Token: 0x04000219 RID: 537
		private readonly StatItem _item;

		// Token: 0x0400021A RID: 538
		private string _definitionText;

		// Token: 0x0400021B RID: 539
		private string _valueText;

		// Token: 0x0400021C RID: 540
		private string _statTypeAsString;
	}
}
