using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200007B RID: 123
	public struct Ray
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x0000E444 File Offset: 0x0000C644
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x0000E44C File Offset: 0x0000C64C
		public Vec3 Origin
		{
			get
			{
				return this._origin;
			}
			private set
			{
				this._origin = value;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x0000E455 File Offset: 0x0000C655
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x0000E45D File Offset: 0x0000C65D
		public Vec3 Direction
		{
			get
			{
				return this._direction;
			}
			private set
			{
				this._direction = value;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x0000E466 File Offset: 0x0000C666
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x0000E46E File Offset: 0x0000C66E
		public float MaxDistance
		{
			get
			{
				return this._maxDistance;
			}
			private set
			{
				this._maxDistance = value;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x0000E477 File Offset: 0x0000C677
		public Vec3 EndPoint
		{
			get
			{
				return this.Origin + this.Direction * this.MaxDistance;
			}
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x0000E495 File Offset: 0x0000C695
		public Ray(Vec3 origin, Vec3 direction, float maxDistance = 3.4028235E+38f)
		{
			this = default(Ray);
			this.Reset(origin, direction, maxDistance);
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x0000E4A8 File Offset: 0x0000C6A8
		public Ray(Vec3 origin, Vec3 direction, bool useDirectionLenForMaxDistance)
		{
			this._origin = origin;
			this._direction = direction;
			float num = this._direction.Normalize();
			if (useDirectionLenForMaxDistance)
			{
				this._maxDistance = num;
				return;
			}
			this._maxDistance = float.MaxValue;
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x0000E4E5 File Offset: 0x0000C6E5
		public void Reset(Vec3 origin, Vec3 direction, float maxDistance = 3.4028235E+38f)
		{
			this._origin = origin;
			this._direction = direction;
			this._maxDistance = maxDistance;
			this._direction.Normalize();
		}

		// Token: 0x04000140 RID: 320
		private Vec3 _origin;

		// Token: 0x04000141 RID: 321
		private Vec3 _direction;

		// Token: 0x04000142 RID: 322
		private float _maxDistance;
	}
}
