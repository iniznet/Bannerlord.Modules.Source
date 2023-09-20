using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000011 RID: 17
	public class BannerData
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00003F1A File Offset: 0x0000211A
		public int LocalVersion
		{
			get
			{
				return this._localVersion;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00003F22 File Offset: 0x00002122
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00003F2A File Offset: 0x0000212A
		public int MeshId
		{
			get
			{
				return this._meshId;
			}
			set
			{
				if (value != this._meshId)
				{
					this._meshId = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00003F4A File Offset: 0x0000214A
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00003F52 File Offset: 0x00002152
		public int ColorId
		{
			get
			{
				return this._colorId;
			}
			set
			{
				if (value != this._colorId)
				{
					this._colorId = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00003F72 File Offset: 0x00002172
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00003F7A File Offset: 0x0000217A
		public int ColorId2
		{
			get
			{
				return this._colorId2;
			}
			set
			{
				if (value != this._colorId2)
				{
					this._colorId2 = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00003F9A File Offset: 0x0000219A
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00003FA2 File Offset: 0x000021A2
		public Vec2 Size
		{
			get
			{
				return this._size;
			}
			set
			{
				if (value != this._size)
				{
					this._size = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00003FC7 File Offset: 0x000021C7
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00003FCF File Offset: 0x000021CF
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (value != this._position)
				{
					this._position = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00003FF4 File Offset: 0x000021F4
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00003FFC File Offset: 0x000021FC
		public bool DrawStroke
		{
			get
			{
				return this._drawStroke;
			}
			set
			{
				if (value != this._drawStroke)
				{
					this._drawStroke = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000BD RID: 189 RVA: 0x0000401C File Offset: 0x0000221C
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00004024 File Offset: 0x00002224
		public bool Mirror
		{
			get
			{
				return this._mirror;
			}
			set
			{
				if (value != this._mirror)
				{
					this._mirror = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00004044 File Offset: 0x00002244
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x0000404C File Offset: 0x0000224C
		public float RotationValue
		{
			get
			{
				return this._rotationValue;
			}
			set
			{
				if (value != this._rotationValue)
				{
					this._rotationValue = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000406C File Offset: 0x0000226C
		public float Rotation
		{
			get
			{
				return 6.2831855f * this.RotationValue;
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000407C File Offset: 0x0000227C
		public BannerData(int meshId, int colorId, int colorId2, Vec2 size, Vec2 position, bool drawStroke, bool mirror, float rotationValue)
		{
			this.MeshId = meshId;
			this.ColorId = colorId;
			this.ColorId2 = colorId2;
			this.Size = size;
			this.Position = position;
			this.DrawStroke = drawStroke;
			this.Mirror = mirror;
			this.RotationValue = rotationValue;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000040CC File Offset: 0x000022CC
		public BannerData(BannerData bannerData)
			: this(bannerData.MeshId, bannerData.ColorId, bannerData.ColorId2, bannerData.Size, bannerData.Position, bannerData.DrawStroke, bannerData.Mirror, bannerData.RotationValue)
		{
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00004110 File Offset: 0x00002310
		public override bool Equals(object obj)
		{
			BannerData bannerData;
			return (bannerData = obj as BannerData) != null && bannerData.MeshId == this.MeshId && bannerData.ColorId == this.ColorId && bannerData.ColorId2 == this.ColorId2 && bannerData.Size.X == this.Size.X && bannerData.Size.Y == this.Size.Y && bannerData.Position.X == this.Position.X && bannerData.Position.Y == this.Position.Y && bannerData.DrawStroke == this.DrawStroke && bannerData.Mirror == this.Mirror && bannerData.RotationValue == this.RotationValue;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00004205 File Offset: 0x00002405
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000420D File Offset: 0x0000240D
		internal static void AutoGeneratedStaticCollectObjectsBannerData(object o, List<object> collectedObjects)
		{
			((BannerData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000421B File Offset: 0x0000241B
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000421D File Offset: 0x0000241D
		internal static object AutoGeneratedGetMemberValue_colorId2(object o)
		{
			return ((BannerData)o)._colorId2;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000422F File Offset: 0x0000242F
		internal static object AutoGeneratedGetMemberValue_size(object o)
		{
			return ((BannerData)o)._size;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00004241 File Offset: 0x00002441
		internal static object AutoGeneratedGetMemberValue_position(object o)
		{
			return ((BannerData)o)._position;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00004253 File Offset: 0x00002453
		internal static object AutoGeneratedGetMemberValue_drawStroke(object o)
		{
			return ((BannerData)o)._drawStroke;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00004265 File Offset: 0x00002465
		internal static object AutoGeneratedGetMemberValue_mirror(object o)
		{
			return ((BannerData)o)._mirror;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00004277 File Offset: 0x00002477
		internal static object AutoGeneratedGetMemberValue_rotationValue(object o)
		{
			return ((BannerData)o)._rotationValue;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00004289 File Offset: 0x00002489
		internal static object AutoGeneratedGetMemberValue_meshId(object o)
		{
			return ((BannerData)o)._meshId;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000429B File Offset: 0x0000249B
		internal static object AutoGeneratedGetMemberValue_colorId(object o)
		{
			return ((BannerData)o)._colorId;
		}

		// Token: 0x040000E4 RID: 228
		public const float RotationPrecision = 0.00278f;

		// Token: 0x040000E5 RID: 229
		[CachedData]
		private int _localVersion;

		// Token: 0x040000E6 RID: 230
		[SaveableField(1)]
		private int _meshId;

		// Token: 0x040000E7 RID: 231
		[SaveableField(2)]
		private int _colorId;

		// Token: 0x040000E8 RID: 232
		[SaveableField(3)]
		public int _colorId2;

		// Token: 0x040000E9 RID: 233
		[SaveableField(4)]
		public Vec2 _size;

		// Token: 0x040000EA RID: 234
		[SaveableField(5)]
		public Vec2 _position;

		// Token: 0x040000EB RID: 235
		[SaveableField(6)]
		public bool _drawStroke;

		// Token: 0x040000EC RID: 236
		[SaveableField(7)]
		public bool _mirror;

		// Token: 0x040000ED RID: 237
		[SaveableField(8)]
		public float _rotationValue;
	}
}
