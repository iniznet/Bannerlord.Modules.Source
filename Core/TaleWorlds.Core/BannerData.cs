﻿using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	public class BannerData
	{
		public int LocalVersion
		{
			get
			{
				return this._localVersion;
			}
		}

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

		public float Rotation
		{
			get
			{
				return 6.2831855f * this.RotationValue;
			}
		}

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

		public BannerData(BannerData bannerData)
			: this(bannerData.MeshId, bannerData.ColorId, bannerData.ColorId2, bannerData.Size, bannerData.Position, bannerData.DrawStroke, bannerData.Mirror, bannerData.RotationValue)
		{
		}

		public override bool Equals(object obj)
		{
			BannerData bannerData;
			return (bannerData = obj as BannerData) != null && bannerData.MeshId == this.MeshId && bannerData.ColorId == this.ColorId && bannerData.ColorId2 == this.ColorId2 && bannerData.Size.X == this.Size.X && bannerData.Size.Y == this.Size.Y && bannerData.Position.X == this.Position.X && bannerData.Position.Y == this.Position.Y && bannerData.DrawStroke == this.DrawStroke && bannerData.Mirror == this.Mirror && bannerData.RotationValue == this.RotationValue;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static void AutoGeneratedStaticCollectObjectsBannerData(object o, List<object> collectedObjects)
		{
			((BannerData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		internal static object AutoGeneratedGetMemberValue_colorId2(object o)
		{
			return ((BannerData)o)._colorId2;
		}

		internal static object AutoGeneratedGetMemberValue_size(object o)
		{
			return ((BannerData)o)._size;
		}

		internal static object AutoGeneratedGetMemberValue_position(object o)
		{
			return ((BannerData)o)._position;
		}

		internal static object AutoGeneratedGetMemberValue_drawStroke(object o)
		{
			return ((BannerData)o)._drawStroke;
		}

		internal static object AutoGeneratedGetMemberValue_mirror(object o)
		{
			return ((BannerData)o)._mirror;
		}

		internal static object AutoGeneratedGetMemberValue_rotationValue(object o)
		{
			return ((BannerData)o)._rotationValue;
		}

		internal static object AutoGeneratedGetMemberValue_meshId(object o)
		{
			return ((BannerData)o)._meshId;
		}

		internal static object AutoGeneratedGetMemberValue_colorId(object o)
		{
			return ((BannerData)o)._colorId;
		}

		public const float RotationPrecision = 0.00278f;

		[CachedData]
		private int _localVersion;

		[SaveableField(1)]
		private int _meshId;

		[SaveableField(2)]
		private int _colorId;

		[SaveableField(3)]
		public int _colorId2;

		[SaveableField(4)]
		public Vec2 _size;

		[SaveableField(5)]
		public Vec2 _position;

		[SaveableField(6)]
		public bool _drawStroke;

		[SaveableField(7)]
		public bool _mirror;

		[SaveableField(8)]
		public float _rotationValue;
	}
}
