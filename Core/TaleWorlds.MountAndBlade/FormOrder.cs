using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000142 RID: 322
	public struct FormOrder
	{
		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06001066 RID: 4198 RVA: 0x0003494B File Offset: 0x00032B4B
		// (set) Token: 0x06001067 RID: 4199 RVA: 0x00034953 File Offset: 0x00032B53
		public float CustomFlankWidth
		{
			get
			{
				return this._customFlankWidth;
			}
			private set
			{
				this._customFlankWidth = value;
			}
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x0003495C File Offset: 0x00032B5C
		private FormOrder(FormOrder.FormOrderEnum orderEnum, float customFlankWidth = -1f)
		{
			this.OrderEnum = orderEnum;
			this._customFlankWidth = customFlankWidth;
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x0003496C File Offset: 0x00032B6C
		public static FormOrder FormOrderCustom(float customWidth)
		{
			return new FormOrder(FormOrder.FormOrderEnum.Custom, customWidth);
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x0600106A RID: 4202 RVA: 0x00034978 File Offset: 0x00032B78
		public OrderType OrderType
		{
			get
			{
				switch (this.OrderEnum)
				{
				case FormOrder.FormOrderEnum.Wide:
					return OrderType.FormWide;
				case FormOrder.FormOrderEnum.Wider:
					return OrderType.FormWider;
				case FormOrder.FormOrderEnum.Custom:
					return OrderType.FormCustom;
				default:
					return OrderType.FormDeep;
				}
			}
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x000349AD File Offset: 0x00032BAD
		public void OnApply(Formation formation)
		{
			this.OnApplyToArrangement(formation, formation.Arrangement);
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x000349BC File Offset: 0x00032BBC
		public static int GetUnitCountOf(Formation formation)
		{
			if (formation.OverridenUnitCount == null)
			{
				return formation.CountOfUnitsWithoutDetachedOnes;
			}
			return formation.OverridenUnitCount.Value;
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x000349EE File Offset: 0x00032BEE
		public bool OnApplyToCustomArrangement(Formation formation, IFormationArrangement arrangement)
		{
			return false;
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x000349F4 File Offset: 0x00032BF4
		private void OnApplyToArrangement(Formation formation, IFormationArrangement arrangement)
		{
			if (!this.OnApplyToCustomArrangement(formation, arrangement))
			{
				if (arrangement is ColumnFormation)
				{
					ColumnFormation columnFormation = arrangement as ColumnFormation;
					if (FormOrder.GetUnitCountOf(formation) > 0)
					{
						columnFormation.FormFromWidth((float)this.GetRankVerticalFormFileCount());
						return;
					}
				}
				else
				{
					if (arrangement is RectilinearSchiltronFormation)
					{
						(arrangement as RectilinearSchiltronFormation).Form();
						return;
					}
					if (arrangement is CircularSchiltronFormation)
					{
						(arrangement as CircularSchiltronFormation).Form();
						return;
					}
					if (arrangement is CircularFormation)
					{
						CircularFormation circularFormation = arrangement as CircularFormation;
						int unitCountOf = FormOrder.GetUnitCountOf(formation);
						int? fileCount = this.GetFileCount(unitCountOf);
						float num2;
						if (fileCount != null)
						{
							int num = MathF.Max(1, MathF.Ceiling((float)unitCountOf * 1f / (float)fileCount.Value));
							num2 = circularFormation.GetCircumferenceFromRankCount(num);
						}
						else
						{
							num2 = 3.1415927f * this.CustomFlankWidth;
						}
						circularFormation.FormFromCircumference(num2);
						return;
					}
					if (arrangement is SquareFormation)
					{
						SquareFormation squareFormation = arrangement as SquareFormation;
						int unitCountOf2 = FormOrder.GetUnitCountOf(formation);
						int? fileCount2 = this.GetFileCount(unitCountOf2);
						if (fileCount2 != null)
						{
							int num3 = MathF.Max(1, MathF.Ceiling((float)unitCountOf2 * 1f / (float)fileCount2.Value));
							squareFormation.FormFromRankCount(num3);
							return;
						}
						squareFormation.FormFromBorderSideWidth(this.CustomFlankWidth);
						return;
					}
					else if (arrangement is SkeinFormation)
					{
						SkeinFormation skeinFormation = arrangement as SkeinFormation;
						int unitCountOf3 = FormOrder.GetUnitCountOf(formation);
						int? fileCount3 = this.GetFileCount(unitCountOf3);
						if (fileCount3 != null)
						{
							skeinFormation.FormFromFlankWidth(fileCount3.Value, false);
							return;
						}
						skeinFormation.FlankWidth = this.CustomFlankWidth;
						return;
					}
					else if (arrangement is WedgeFormation)
					{
						WedgeFormation wedgeFormation = arrangement as WedgeFormation;
						int unitCountOf4 = FormOrder.GetUnitCountOf(formation);
						int? fileCount4 = this.GetFileCount(unitCountOf4);
						if (fileCount4 != null)
						{
							wedgeFormation.FormFromFlankWidth(fileCount4.Value, false);
							return;
						}
						wedgeFormation.FlankWidth = this.CustomFlankWidth;
						return;
					}
					else if (arrangement is TransposedLineFormation)
					{
						TransposedLineFormation transposedLineFormation = arrangement as TransposedLineFormation;
						if (FormOrder.GetUnitCountOf(formation) > 0)
						{
							transposedLineFormation.FormFromFlankWidth(this.GetRankVerticalFormFileCount(), false);
							return;
						}
					}
					else if (arrangement is LineFormation)
					{
						LineFormation lineFormation = arrangement as LineFormation;
						int unitCountOf5 = FormOrder.GetUnitCountOf(formation);
						int? fileCount5 = this.GetFileCount(unitCountOf5);
						if (fileCount5 != null)
						{
							lineFormation.FormFromFlankWidth(fileCount5.Value, unitCountOf5 > 40);
							return;
						}
						lineFormation.FlankWidth = this.CustomFlankWidth;
						return;
					}
					else
					{
						Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\FormOrder.cs", "OnApplyToArrangement", 224);
					}
				}
			}
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x00034C51 File Offset: 0x00032E51
		private int? GetFileCount(int unitCount)
		{
			return FormOrder.GetFileCountStatic(this.OrderEnum, unitCount);
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x00034C5F File Offset: 0x00032E5F
		public static int? GetFileCountStatic(FormOrder.FormOrderEnum order, int unitCount)
		{
			return FormOrder.GetFileCountAux(order, unitCount);
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x00034C68 File Offset: 0x00032E68
		private int GetRankVerticalFormFileCount()
		{
			switch (this.OrderEnum)
			{
			case FormOrder.FormOrderEnum.Deep:
				return 1;
			case FormOrder.FormOrderEnum.Wide:
				return 3;
			case FormOrder.FormOrderEnum.Wider:
				return 5;
			case FormOrder.FormOrderEnum.Custom:
				return MathF.Ceiling(this._customFlankWidth);
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\FormOrder.cs", "GetRankVerticalFormFileCount", 265);
				return 1;
			}
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00034CC0 File Offset: 0x00032EC0
		private static int? GetFileCountAux(FormOrder.FormOrderEnum order, int unitCount)
		{
			switch (order)
			{
			case FormOrder.FormOrderEnum.Deep:
				return new int?(MathF.Max(MathF.Round(MathF.Sqrt((float)unitCount / 4f)), 1) * 4);
			case FormOrder.FormOrderEnum.Wide:
				return new int?(MathF.Max(MathF.Round(MathF.Sqrt((float)unitCount / 16f)), 1) * 16);
			case FormOrder.FormOrderEnum.Wider:
				return new int?(MathF.Max(MathF.Round(MathF.Sqrt((float)unitCount / 64f)), 1) * 64);
			case FormOrder.FormOrderEnum.Custom:
				return null;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\FormOrder.cs", "GetFileCountAux", 285);
				return null;
			}
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x00034D74 File Offset: 0x00032F74
		public override bool Equals(object obj)
		{
			if (obj is FormOrder)
			{
				FormOrder formOrder = (FormOrder)obj;
				return formOrder == this;
			}
			return false;
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x00034DA0 File Offset: 0x00032FA0
		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x00034DA8 File Offset: 0x00032FA8
		public static bool operator !=(FormOrder f1, FormOrder f2)
		{
			return f1.OrderEnum != f2.OrderEnum;
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x00034DBB File Offset: 0x00032FBB
		public static bool operator ==(FormOrder f1, FormOrder f2)
		{
			return f1.OrderEnum == f2.OrderEnum;
		}

		// Token: 0x0400041E RID: 1054
		private float _customFlankWidth;

		// Token: 0x0400041F RID: 1055
		public readonly FormOrder.FormOrderEnum OrderEnum;

		// Token: 0x04000420 RID: 1056
		public static readonly FormOrder FormOrderDeep = new FormOrder(FormOrder.FormOrderEnum.Deep, -1f);

		// Token: 0x04000421 RID: 1057
		public static readonly FormOrder FormOrderWide = new FormOrder(FormOrder.FormOrderEnum.Wide, -1f);

		// Token: 0x04000422 RID: 1058
		public static readonly FormOrder FormOrderWider = new FormOrder(FormOrder.FormOrderEnum.Wider, -1f);

		// Token: 0x02000484 RID: 1156
		public enum FormOrderEnum
		{
			// Token: 0x04001976 RID: 6518
			Deep,
			// Token: 0x04001977 RID: 6519
			Wide,
			// Token: 0x04001978 RID: 6520
			Wider,
			// Token: 0x04001979 RID: 6521
			Custom
		}
	}
}
