using System;
using System.Collections.Generic;

namespace psai.Editor
{
	// Token: 0x02000007 RID: 7
	[Serializable]
	public abstract class PsaiMusicEntity : ICloneable
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002DD8 File Offset: 0x00000FD8
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00002DE0 File Offset: 0x00000FE0
		public string Name { get; set; }

		// Token: 0x06000054 RID: 84
		public abstract string GetClassString();

		// Token: 0x06000055 RID: 85
		public abstract CompatibilitySetting GetCompatibilitySetting(PsaiMusicEntity targetEntity);

		// Token: 0x06000056 RID: 86
		public abstract CompatibilityType GetCompatibilityType(PsaiMusicEntity targetEntity, out CompatibilityReason reason);

		// Token: 0x06000057 RID: 87
		public abstract PsaiMusicEntity GetParent();

		// Token: 0x06000058 RID: 88
		public abstract List<PsaiMusicEntity> GetChildren();

		// Token: 0x06000059 RID: 89
		public abstract int GetIndexPositionWithinParentEntity(PsaiProject parentProject);

		// Token: 0x0600005A RID: 90 RVA: 0x00002DE9 File Offset: 0x00000FE9
		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002DF1 File Offset: 0x00000FF1
		public virtual PsaiMusicEntity ShallowCopy()
		{
			return (PsaiMusicEntity)base.MemberwiseClone();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002DFE File Offset: 0x00000FFE
		public virtual bool PropertyDifferencesAffectCompatibilities(PsaiMusicEntity otherEntity)
		{
			return false;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002E04 File Offset: 0x00001004
		public Theme GetTheme()
		{
			PsaiMusicEntity psaiMusicEntity = this;
			Theme theme;
			do
			{
				theme = psaiMusicEntity as Theme;
				psaiMusicEntity = psaiMusicEntity.GetParent();
			}
			while (theme == null && psaiMusicEntity != null);
			return theme;
		}
	}
}
