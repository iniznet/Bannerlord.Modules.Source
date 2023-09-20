using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000168 RID: 360
	public class QueryData<T> : IQueryData
	{
		// Token: 0x06001254 RID: 4692 RVA: 0x00047264 File Offset: 0x00045464
		public QueryData(Func<T> valueFunc, float lifetime)
		{
			this._cachedValue = default(T);
			this._expireTime = 0f;
			this._lifetime = lifetime;
			this._valueFunc = valueFunc;
			this._syncGroup = null;
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x00047298 File Offset: 0x00045498
		public QueryData(Func<T> valueFunc, float lifetime, T defaultCachedValue)
		{
			this._cachedValue = defaultCachedValue;
			this._expireTime = 0f;
			this._lifetime = lifetime;
			this._valueFunc = valueFunc;
			this._syncGroup = null;
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x000472C7 File Offset: 0x000454C7
		public void Evaluate(float currentTime)
		{
			this.SetValue(this._valueFunc(), currentTime);
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x000472DB File Offset: 0x000454DB
		public void SetValue(T value, float currentTime)
		{
			this._cachedValue = value;
			this._expireTime = currentTime + this._lifetime;
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x000472F2 File Offset: 0x000454F2
		public T GetCachedValue()
		{
			return this._cachedValue;
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x000472FA File Offset: 0x000454FA
		public T GetCachedValueWithMaxAge(float age)
		{
			if (Mission.Current.CurrentTime > this._expireTime - this._lifetime + MathF.Min(this._lifetime, age))
			{
				this.Expire();
				return this.Value;
			}
			return this._cachedValue;
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x0600125A RID: 4698 RVA: 0x00047338 File Offset: 0x00045538
		public T Value
		{
			get
			{
				float currentTime = Mission.Current.CurrentTime;
				if (currentTime >= this._expireTime)
				{
					if (this._syncGroup != null)
					{
						IQueryData[] syncGroup = this._syncGroup;
						for (int i = 0; i < syncGroup.Length; i++)
						{
							syncGroup[i].Evaluate(currentTime);
						}
					}
					this.Evaluate(currentTime);
				}
				return this._cachedValue;
			}
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x0004738C File Offset: 0x0004558C
		public void Expire()
		{
			this._expireTime = 0f;
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x0004739C File Offset: 0x0004559C
		public static void SetupSyncGroup(params IQueryData[] groupItems)
		{
			for (int i = 0; i < groupItems.Length; i++)
			{
				groupItems[i].SetSyncGroup(groupItems);
			}
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x000473C2 File Offset: 0x000455C2
		public void SetSyncGroup(IQueryData[] syncGroup)
		{
			this._syncGroup = syncGroup;
		}

		// Token: 0x040004FD RID: 1277
		private T _cachedValue;

		// Token: 0x040004FE RID: 1278
		private float _expireTime;

		// Token: 0x040004FF RID: 1279
		private readonly float _lifetime;

		// Token: 0x04000500 RID: 1280
		private readonly Func<T> _valueFunc;

		// Token: 0x04000501 RID: 1281
		private IQueryData[] _syncGroup;
	}
}
