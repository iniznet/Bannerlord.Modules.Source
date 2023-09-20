using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x020000BB RID: 187
	public class SceneNotificationData
	{
		// Token: 0x1700031D RID: 797
		// (get) Token: 0x0600095A RID: 2394 RVA: 0x0001F565 File Offset: 0x0001D765
		public virtual string SceneID { get; }

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x0600095B RID: 2395 RVA: 0x0001F56D File Offset: 0x0001D76D
		public virtual string SoundEventPath { get; }

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x0001F575 File Offset: 0x0001D775
		public virtual TextObject TitleText { get; }

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x0001F57D File Offset: 0x0001D77D
		public virtual TextObject AffirmativeDescriptionText { get; }

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x0600095E RID: 2398 RVA: 0x0001F585 File Offset: 0x0001D785
		public virtual TextObject NegativeDescriptionText { get; }

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x0001F58D File Offset: 0x0001D78D
		public virtual TextObject AffirmativeHintText { get; }

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x0001F595 File Offset: 0x0001D795
		public virtual TextObject AffirmativeHintTextExtended { get; }

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000961 RID: 2401 RVA: 0x0001F59D File Offset: 0x0001D79D
		public virtual TextObject AffirmativeTitleText { get; }

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000962 RID: 2402 RVA: 0x0001F5A5 File Offset: 0x0001D7A5
		public virtual TextObject NegativeTitleText { get; }

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06000963 RID: 2403 RVA: 0x0001F5AD File Offset: 0x0001D7AD
		public virtual TextObject AffirmativeText { get; }

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000964 RID: 2404 RVA: 0x0001F5B5 File Offset: 0x0001D7B5
		public virtual TextObject NegativeText { get; }

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06000965 RID: 2405 RVA: 0x0001F5BD File Offset: 0x0001D7BD
		public virtual bool IsAffirmativeOptionShown { get; }

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06000966 RID: 2406 RVA: 0x0001F5C5 File Offset: 0x0001D7C5
		public virtual bool IsNegativeOptionShown { get; }

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000967 RID: 2407 RVA: 0x0001F5CD File Offset: 0x0001D7CD
		public virtual bool PauseActiveState { get; } = true;

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000968 RID: 2408 RVA: 0x0001F5D5 File Offset: 0x0001D7D5
		public virtual SceneNotificationData.RelevantContextType RelevantContext { get; }

		// Token: 0x06000969 RID: 2409 RVA: 0x0001F5DD File Offset: 0x0001D7DD
		public virtual void OnAffirmativeAction()
		{
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0001F5DF File Offset: 0x0001D7DF
		public virtual void OnNegativeAction()
		{
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0001F5E1 File Offset: 0x0001D7E1
		public virtual void OnCloseAction()
		{
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0001F5E3 File Offset: 0x0001D7E3
		public virtual IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>();
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0001F5EA File Offset: 0x0001D7EA
		public virtual IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			return new List<SceneNotificationData.SceneNotificationCharacter>();
		}

		// Token: 0x02000109 RID: 265
		public readonly struct SceneNotificationCharacter
		{
			// Token: 0x06000A58 RID: 2648 RVA: 0x0002165E File Offset: 0x0001F85E
			public SceneNotificationCharacter(BasicCharacterObject character, Equipment overriddenEquipment = null, BodyProperties overriddenBodyProperties = default(BodyProperties), bool useCivilianEquipment = false, uint customColor1 = 4294967295U, uint customColor2 = 4294967295U, bool useHorse = false)
			{
				this.Character = character;
				this.OverriddenEquipment = overriddenEquipment;
				this.OverriddenBodyProperties = overriddenBodyProperties;
				this.UseCivilianEquipment = useCivilianEquipment;
				this.CustomColor1 = customColor1;
				this.CustomColor2 = customColor2;
				this.UseHorse = useHorse;
			}

			// Token: 0x040006E5 RID: 1765
			public readonly BasicCharacterObject Character;

			// Token: 0x040006E6 RID: 1766
			public readonly Equipment OverriddenEquipment;

			// Token: 0x040006E7 RID: 1767
			public readonly BodyProperties OverriddenBodyProperties;

			// Token: 0x040006E8 RID: 1768
			public readonly bool UseCivilianEquipment;

			// Token: 0x040006E9 RID: 1769
			public readonly bool UseHorse;

			// Token: 0x040006EA RID: 1770
			public readonly uint CustomColor1;

			// Token: 0x040006EB RID: 1771
			public readonly uint CustomColor2;
		}

		// Token: 0x0200010A RID: 266
		public enum RelevantContextType
		{
			// Token: 0x040006ED RID: 1773
			Any,
			// Token: 0x040006EE RID: 1774
			MPLobby,
			// Token: 0x040006EF RID: 1775
			CustomBattle,
			// Token: 0x040006F0 RID: 1776
			Mission,
			// Token: 0x040006F1 RID: 1777
			Map
		}
	}
}
