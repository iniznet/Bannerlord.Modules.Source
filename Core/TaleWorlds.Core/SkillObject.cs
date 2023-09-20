﻿using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000034 RID: 52
	public sealed class SkillObject : PropertyObject
	{
		// Token: 0x060003CF RID: 975 RVA: 0x0000F12B File Offset: 0x0000D32B
		internal static void AutoGeneratedStaticCollectObjectsSkillObject(object o, List<object> collectedObjects)
		{
			((SkillObject)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0000F139 File Offset: 0x0000D339
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0000F142 File Offset: 0x0000D342
		public SkillObject(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0000F14B File Offset: 0x0000D34B
		public void SetAttribute(CharacterAttribute attribute)
		{
			this.CharacterAttribute = attribute;
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060003D3 RID: 979 RVA: 0x0000F154 File Offset: 0x0000D354
		// (set) Token: 0x060003D4 RID: 980 RVA: 0x0000F15C File Offset: 0x0000D35C
		public CharacterAttribute CharacterAttribute
		{
			get
			{
				return this._characterAttribute;
			}
			private set
			{
				this._characterAttribute = value;
				this._characterAttribute.AddSkill(this);
			}
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0000F171 File Offset: 0x0000D371
		public override string ToString()
		{
			return base.Name.ToString();
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0000F17E File Offset: 0x0000D37E
		public SkillObject Initialize(TextObject name, TextObject description, SkillObject.SkillTypeEnum skillType)
		{
			base.Initialize(name, description);
			this._skillType = skillType;
			base.AfterInitialized();
			return this;
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x0000F196 File Offset: 0x0000D396
		public bool IsLeaderSkill
		{
			get
			{
				return this._skillType == SkillObject.SkillTypeEnum.Leader;
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x0000F1A1 File Offset: 0x0000D3A1
		public bool IsPartySkill
		{
			get
			{
				return this._skillType == SkillObject.SkillTypeEnum.Party;
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x0000F1AC File Offset: 0x0000D3AC
		public bool IsPersonalSkill
		{
			get
			{
				return this._skillType == SkillObject.SkillTypeEnum.Personal;
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060003DA RID: 986 RVA: 0x0000F1B7 File Offset: 0x0000D3B7
		public TextObject HowToLearnSkillText
		{
			get
			{
				if (GameTexts.FindText("str_how_to_learn_skill", base.StringId) == null)
				{
					return new TextObject("{=Aj3zqQq4}Not available", null);
				}
				return GameTexts.FindText("str_how_to_learn_skill", base.StringId);
			}
		}

		// Token: 0x04000206 RID: 518
		private SkillObject.SkillTypeEnum _skillType;

		// Token: 0x04000207 RID: 519
		private CharacterAttribute _characterAttribute;

		// Token: 0x020000EA RID: 234
		public enum SkillTypeEnum
		{
			// Token: 0x04000679 RID: 1657
			Personal,
			// Token: 0x0400067A RID: 1658
			Leader,
			// Token: 0x0400067B RID: 1659
			Party
		}
	}
}
