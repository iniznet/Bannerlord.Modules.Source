using System;
using System.Collections.Specialized;

namespace TaleWorlds.Core
{
	public class StackArray
	{
		public struct StackArray5Float
		{
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

			private float _element0;

			private float _element1;

			private float _element2;

			private float _element3;

			private float _element4;

			public const int Length = 5;
		}

		public struct StackArray3Int
		{
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

			private int _element0;

			private int _element1;

			private int _element2;

			public const int Length = 3;
		}

		public struct StackArray4Int
		{
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

			private int _element0;

			private int _element1;

			private int _element2;

			private int _element3;

			public const int Length = 4;
		}

		public struct StackArray2Bool
		{
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

			private byte _element;

			public const int Length = 2;
		}

		public struct StackArray8Int
		{
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

			private int _element0;

			private int _element1;

			private int _element2;

			private int _element3;

			private int _element4;

			private int _element5;

			private int _element6;

			private int _element7;

			public const int Length = 8;
		}

		public struct StackArray10FloatFloatTuple
		{
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

			private ValueTuple<float, float> _element0;

			private ValueTuple<float, float> _element1;

			private ValueTuple<float, float> _element2;

			private ValueTuple<float, float> _element3;

			private ValueTuple<float, float> _element4;

			private ValueTuple<float, float> _element5;

			private ValueTuple<float, float> _element6;

			private ValueTuple<float, float> _element7;

			private ValueTuple<float, float> _element8;

			private ValueTuple<float, float> _element9;

			public const int Length = 10;
		}

		public struct StackArray3Bool
		{
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

			private byte _element;

			public const int Length = 3;
		}

		public struct StackArray4Bool
		{
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

			private byte _element;

			public const int Length = 4;
		}

		public struct StackArray5Bool
		{
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

			private byte _element;

			public const int Length = 5;
		}

		public struct StackArray6Bool
		{
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

			private byte _element;

			public const int Length = 6;
		}

		public struct StackArray7Bool
		{
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

			private byte _element;

			public const int Length = 7;
		}

		public struct StackArray8Bool
		{
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

			private byte _element;

			public const int Length = 8;
		}

		public struct StackArray32Bool
		{
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

			public StackArray32Bool(int init)
			{
				this._element = new BitVector32(init);
			}

			private BitVector32 _element;

			public const int Length = 32;
		}
	}
}
