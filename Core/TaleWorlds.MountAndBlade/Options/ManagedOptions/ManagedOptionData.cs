using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options.ManagedOptions
{
	// Token: 0x0200039C RID: 924
	public abstract class ManagedOptionData : IOptionData
	{
		// Token: 0x06003286 RID: 12934 RVA: 0x000D149C File Offset: 0x000CF69C
		protected ManagedOptionData(ManagedOptions.ManagedOptionsType type)
		{
			this.Type = type;
			this._value = ManagedOptions.GetConfig(type);
		}

		// Token: 0x06003287 RID: 12935 RVA: 0x000D14B7 File Offset: 0x000CF6B7
		public virtual float GetDefaultValue()
		{
			return ManagedOptions.GetDefaultConfig(this.Type);
		}

		// Token: 0x06003288 RID: 12936 RVA: 0x000D14C4 File Offset: 0x000CF6C4
		public void Commit()
		{
			if (this._value != ManagedOptions.GetConfig(this.Type))
			{
				ManagedOptions.SetConfig(this.Type, this._value);
			}
		}

		// Token: 0x06003289 RID: 12937 RVA: 0x000D14EA File Offset: 0x000CF6EA
		public float GetValue(bool forceRefresh)
		{
			if (forceRefresh)
			{
				this._value = ManagedOptions.GetConfig(this.Type);
			}
			return this._value;
		}

		// Token: 0x0600328A RID: 12938 RVA: 0x000D1506 File Offset: 0x000CF706
		public void SetValue(float value)
		{
			this._value = value;
		}

		// Token: 0x0600328B RID: 12939 RVA: 0x000D150F File Offset: 0x000CF70F
		public object GetOptionType()
		{
			return this.Type;
		}

		// Token: 0x0600328C RID: 12940 RVA: 0x000D151C File Offset: 0x000CF71C
		public bool IsNative()
		{
			return false;
		}

		// Token: 0x0600328D RID: 12941 RVA: 0x000D151F File Offset: 0x000CF71F
		public bool IsAction()
		{
			return false;
		}

		// Token: 0x0600328E RID: 12942 RVA: 0x000D1522 File Offset: 0x000CF722
		public ValueTuple<string, bool> GetIsDisabledAndReasonID()
		{
			return new ValueTuple<string, bool>(string.Empty, false);
		}

		// Token: 0x0400154E RID: 5454
		public readonly ManagedOptions.ManagedOptionsType Type;

		// Token: 0x0400154F RID: 5455
		private float _value;
	}
}
