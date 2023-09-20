using System;
using System.Collections.Specialized;

namespace TaleWorlds.Core
{
	// Token: 0x020000BF RID: 191
	public class StackArray
	{
		// Token: 0x0200010B RID: 267
		public struct StackArray5Float
		{
			// Token: 0x17000357 RID: 855
			public float this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					case 4:
						return this._element4;
					default:
						return 0f;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					case 4:
						this._element4 = value;
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x040006F2 RID: 1778
			private float _element0;

			// Token: 0x040006F3 RID: 1779
			private float _element1;

			// Token: 0x040006F4 RID: 1780
			private float _element2;

			// Token: 0x040006F5 RID: 1781
			private float _element3;

			// Token: 0x040006F6 RID: 1782
			private float _element4;

			// Token: 0x040006F7 RID: 1783
			public const int Length = 5;
		}

		// Token: 0x0200010C RID: 268
		public struct StackArray3Int
		{
			// Token: 0x17000358 RID: 856
			public int this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					default:
						return 0;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x040006F8 RID: 1784
			private int _element0;

			// Token: 0x040006F9 RID: 1785
			private int _element1;

			// Token: 0x040006FA RID: 1786
			private int _element2;

			// Token: 0x040006FB RID: 1787
			public const int Length = 3;
		}

		// Token: 0x0200010D RID: 269
		public struct StackArray4Int
		{
			// Token: 0x17000359 RID: 857
			public int this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					default:
						return 0;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x040006FC RID: 1788
			private int _element0;

			// Token: 0x040006FD RID: 1789
			private int _element1;

			// Token: 0x040006FE RID: 1790
			private int _element2;

			// Token: 0x040006FF RID: 1791
			private int _element3;

			// Token: 0x04000700 RID: 1792
			public const int Length = 4;
		}

		// Token: 0x0200010E RID: 270
		public struct StackArray2Bool
		{
			// Token: 0x1700035A RID: 858
			public bool this[int index]
			{
				get
				{
					return ((int)this._element & (1 << index)) != 0;
				}
				set
				{
					this._element = (byte)(value ? ((int)this._element | (1 << index)) : ((int)this._element & ~(1 << index)));
				}
			}

			// Token: 0x04000701 RID: 1793
			private byte _element;

			// Token: 0x04000702 RID: 1794
			public const int Length = 2;
		}

		// Token: 0x0200010F RID: 271
		public struct StackArray8Int
		{
			// Token: 0x1700035B RID: 859
			public int this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					case 4:
						return this._element4;
					case 5:
						return this._element5;
					case 6:
						return this._element6;
					case 7:
						return this._element7;
					default:
						return 0;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					case 4:
						this._element4 = value;
						return;
					case 5:
						this._element5 = value;
						return;
					case 6:
						this._element6 = value;
						return;
					case 7:
						this._element7 = value;
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x04000703 RID: 1795
			private int _element0;

			// Token: 0x04000704 RID: 1796
			private int _element1;

			// Token: 0x04000705 RID: 1797
			private int _element2;

			// Token: 0x04000706 RID: 1798
			private int _element3;

			// Token: 0x04000707 RID: 1799
			private int _element4;

			// Token: 0x04000708 RID: 1800
			private int _element5;

			// Token: 0x04000709 RID: 1801
			private int _element6;

			// Token: 0x0400070A RID: 1802
			private int _element7;

			// Token: 0x0400070B RID: 1803
			public const int Length = 8;
		}

		// Token: 0x02000110 RID: 272
		public struct StackArray10FloatFloatTuple
		{
			// Token: 0x1700035C RID: 860
			public ValueTuple<float, float> this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					case 4:
						return this._element4;
					case 5:
						return this._element5;
					case 6:
						return this._element6;
					case 7:
						return this._element7;
					case 8:
						return this._element8;
					case 9:
						return this._element9;
					default:
						return new ValueTuple<float, float>(0f, 0f);
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					case 4:
						this._element4 = value;
						return;
					case 5:
						this._element5 = value;
						return;
					case 6:
						this._element6 = value;
						return;
					case 7:
						this._element7 = value;
						return;
					case 8:
						this._element8 = value;
						return;
					case 9:
						this._element9 = value;
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x0400070C RID: 1804
			private ValueTuple<float, float> _element0;

			// Token: 0x0400070D RID: 1805
			private ValueTuple<float, float> _element1;

			// Token: 0x0400070E RID: 1806
			private ValueTuple<float, float> _element2;

			// Token: 0x0400070F RID: 1807
			private ValueTuple<float, float> _element3;

			// Token: 0x04000710 RID: 1808
			private ValueTuple<float, float> _element4;

			// Token: 0x04000711 RID: 1809
			private ValueTuple<float, float> _element5;

			// Token: 0x04000712 RID: 1810
			private ValueTuple<float, float> _element6;

			// Token: 0x04000713 RID: 1811
			private ValueTuple<float, float> _element7;

			// Token: 0x04000714 RID: 1812
			private ValueTuple<float, float> _element8;

			// Token: 0x04000715 RID: 1813
			private ValueTuple<float, float> _element9;

			// Token: 0x04000716 RID: 1814
			public const int Length = 10;
		}

		// Token: 0x02000111 RID: 273
		public struct StackArray3Bool
		{
			// Token: 0x1700035D RID: 861
			public bool this[int index]
			{
				get
				{
					return ((int)this._element & (1 << index)) != 0;
				}
				set
				{
					this._element = (byte)(value ? ((int)this._element | (1 << index)) : ((int)this._element & ~(1 << index)));
				}
			}

			// Token: 0x04000717 RID: 1815
			private byte _element;

			// Token: 0x04000718 RID: 1816
			public const int Length = 3;
		}

		// Token: 0x02000112 RID: 274
		public struct StackArray4Bool
		{
			// Token: 0x1700035E RID: 862
			public bool this[int index]
			{
				get
				{
					return ((int)this._element & (1 << index)) != 0;
				}
				set
				{
					this._element = (byte)(value ? ((int)this._element | (1 << index)) : ((int)this._element & ~(1 << index)));
				}
			}

			// Token: 0x04000719 RID: 1817
			private byte _element;

			// Token: 0x0400071A RID: 1818
			public const int Length = 4;
		}

		// Token: 0x02000113 RID: 275
		public struct StackArray5Bool
		{
			// Token: 0x1700035F RID: 863
			public bool this[int index]
			{
				get
				{
					return ((int)this._element & (1 << index)) != 0;
				}
				set
				{
					this._element = (byte)(value ? ((int)this._element | (1 << index)) : ((int)this._element & ~(1 << index)));
				}
			}

			// Token: 0x0400071B RID: 1819
			private byte _element;

			// Token: 0x0400071C RID: 1820
			public const int Length = 5;
		}

		// Token: 0x02000114 RID: 276
		public struct StackArray6Bool
		{
			// Token: 0x17000360 RID: 864
			public bool this[int index]
			{
				get
				{
					return ((int)this._element & (1 << index)) != 0;
				}
				set
				{
					this._element = (byte)(value ? ((int)this._element | (1 << index)) : ((int)this._element & ~(1 << index)));
				}
			}

			// Token: 0x0400071D RID: 1821
			private byte _element;

			// Token: 0x0400071E RID: 1822
			public const int Length = 6;
		}

		// Token: 0x02000115 RID: 277
		public struct StackArray7Bool
		{
			// Token: 0x17000361 RID: 865
			public bool this[int index]
			{
				get
				{
					return ((int)this._element & (1 << index)) != 0;
				}
				set
				{
					this._element = (byte)(value ? ((int)this._element | (1 << index)) : ((int)this._element & ~(1 << index)));
				}
			}

			// Token: 0x0400071F RID: 1823
			private byte _element;

			// Token: 0x04000720 RID: 1824
			public const int Length = 7;
		}

		// Token: 0x02000116 RID: 278
		public struct StackArray8Bool
		{
			// Token: 0x17000362 RID: 866
			public bool this[int index]
			{
				get
				{
					return ((int)this._element & (1 << index)) != 0;
				}
				set
				{
					this._element = (byte)(value ? ((int)this._element | (1 << index)) : ((int)this._element & ~(1 << index)));
				}
			}

			// Token: 0x04000721 RID: 1825
			private byte _element;

			// Token: 0x04000722 RID: 1826
			public const int Length = 8;
		}

		// Token: 0x02000117 RID: 279
		public struct StackArray32Bool
		{
			// Token: 0x17000363 RID: 867
			public bool this[int index]
			{
				get
				{
					return this._element[1 << index];
				}
				set
				{
					this._element[1 << index] = value;
				}
			}

			// Token: 0x06000A73 RID: 2675 RVA: 0x00021BCC File Offset: 0x0001FDCC
			public StackArray32Bool(int init)
			{
				this._element = new BitVector32(init);
			}

			// Token: 0x04000723 RID: 1827
			private BitVector32 _element;

			// Token: 0x04000724 RID: 1828
			public const int Length = 32;
		}
	}
}
