using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct FormOrder
	{
		public float CustomFlankWidth
		{
			get
			{
				return this._customFlankWidth;
			}
			set
			{
				this._customFlankWidth = value;
			}
		}

		private FormOrder(FormOrder.FormOrderEnum orderEnum, float customFlankWidth = -1f)
		{
			this.OrderEnum = orderEnum;
			this._customFlankWidth = customFlankWidth;
		}

		public static FormOrder FormOrderCustom(float customWidth)
		{
			return new FormOrder(FormOrder.FormOrderEnum.Custom, customWidth);
		}

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

		public void OnApply(Formation formation)
		{
			this.OnApplyToArrangement(formation, formation.Arrangement);
		}

		public static int GetUnitCountOf(Formation formation)
		{
			if (formation.OverridenUnitCount == null)
			{
				return formation.CountOfUnitsWithoutDetachedOnes;
			}
			return formation.OverridenUnitCount.Value;
		}

		public bool OnApplyToCustomArrangement(Formation formation, IFormationArrangement arrangement)
		{
			return false;
		}

		private void OnApplyToArrangement(Formation formation, IFormationArrangement arrangement)
		{
			if (!this.OnApplyToCustomArrangement(formation, arrangement))
			{
				if (arrangement is ColumnFormation)
				{
					ColumnFormation columnFormation = arrangement as ColumnFormation;
					if (FormOrder.GetUnitCountOf(formation) > 0)
					{
						columnFormation.FormFromWidth((float)this.GetRankVerticalFormFileCount(formation));
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
						int unitCountOf5 = FormOrder.GetUnitCountOf(formation);
						if (unitCountOf5 > 0)
						{
							int? fileCount5 = this.GetFileCount(unitCountOf5);
							if (fileCount5 == null)
							{
								fileCount5 = new int?(transposedLineFormation.GetFileCountFromWidth(this.CustomFlankWidth));
							}
							MathF.Ceiling((float)unitCountOf5 * 1f / (float)fileCount5.Value);
							transposedLineFormation.FormFromFlankWidth(this.GetRankVerticalFormFileCount(formation), false);
							return;
						}
					}
					else if (arrangement is LineFormation)
					{
						LineFormation lineFormation = arrangement as LineFormation;
						int unitCountOf6 = FormOrder.GetUnitCountOf(formation);
						int? fileCount6 = this.GetFileCount(unitCountOf6);
						if (fileCount6 != null)
						{
							lineFormation.FormFromFlankWidth(fileCount6.Value, unitCountOf6 > 40);
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

		private int? GetFileCount(int unitCount)
		{
			return FormOrder.GetFileCountStatic(this.OrderEnum, unitCount);
		}

		public static int? GetFileCountStatic(FormOrder.FormOrderEnum order, int unitCount)
		{
			return FormOrder.GetFileCountAux(order, unitCount);
		}

		private int GetRankVerticalFormFileCount(IFormation formation)
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
				return MathF.Floor((this._customFlankWidth + formation.Interval) / (formation.UnitDiameter + formation.Interval));
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\FormOrder.cs", "GetRankVerticalFormFileCount", 265);
				return 1;
			}
		}

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

		public override bool Equals(object obj)
		{
			if (obj is FormOrder)
			{
				FormOrder formOrder = (FormOrder)obj;
				return formOrder == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		public static bool operator !=(FormOrder f1, FormOrder f2)
		{
			return f1.OrderEnum != f2.OrderEnum;
		}

		public static bool operator ==(FormOrder f1, FormOrder f2)
		{
			return f1.OrderEnum == f2.OrderEnum;
		}

		private float _customFlankWidth;

		public readonly FormOrder.FormOrderEnum OrderEnum;

		public static readonly FormOrder FormOrderDeep = new FormOrder(FormOrder.FormOrderEnum.Deep, -1f);

		public static readonly FormOrder FormOrderWide = new FormOrder(FormOrder.FormOrderEnum.Wide, -1f);

		public static readonly FormOrder FormOrderWider = new FormOrder(FormOrder.FormOrderEnum.Wider, -1f);

		public enum FormOrderEnum
		{
			Deep,
			Wide,
			Wider,
			Custom
		}
	}
}
