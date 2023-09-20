using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	// Token: 0x0200015D RID: 349
	public class EncyclopediaManager
	{
		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06001854 RID: 6228 RVA: 0x0007B458 File Offset: 0x00079658
		// (set) Token: 0x06001855 RID: 6229 RVA: 0x0007B460 File Offset: 0x00079660
		public IViewDataTracker ViewDataTracker { get; private set; }

		// Token: 0x06001856 RID: 6230 RVA: 0x0007B46C File Offset: 0x0007966C
		public void CreateEncyclopediaPages()
		{
			this._pages = new Dictionary<Type, EncyclopediaPage>();
			this.ViewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			List<Type> list = new List<Type>();
			List<Assembly> list2 = new List<Assembly>();
			Assembly assembly = typeof(EncyclopediaModelBase).Assembly;
			list2.Add(assembly);
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
				for (int j = 0; j < referencedAssemblies.Length; j++)
				{
					if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
					{
						list2.Add(assembly2);
						break;
					}
				}
			}
			foreach (Assembly assembly3 in list2)
			{
				list.AddRange(assembly3.GetTypes());
			}
			foreach (Type type in list)
			{
				if (typeof(EncyclopediaPage).IsAssignableFrom(type))
				{
					object[] array = type.GetCustomAttributes(typeof(OverrideEncyclopediaModel), false);
					for (int i = 0; i < array.Length; i++)
					{
						OverrideEncyclopediaModel overrideEncyclopediaModel = array[i] as OverrideEncyclopediaModel;
						if (overrideEncyclopediaModel != null)
						{
							EncyclopediaPage encyclopediaPage = Activator.CreateInstance(type) as EncyclopediaPage;
							foreach (Type type2 in overrideEncyclopediaModel.PageTargetTypes)
							{
								this._pages.Add(type2, encyclopediaPage);
							}
						}
					}
				}
			}
			foreach (Type type3 in list)
			{
				if (typeof(EncyclopediaPage).IsAssignableFrom(type3))
				{
					object[] array = type3.GetCustomAttributes(typeof(EncyclopediaModel), false);
					for (int i = 0; i < array.Length; i++)
					{
						EncyclopediaModel encyclopediaModel = array[i] as EncyclopediaModel;
						if (encyclopediaModel != null)
						{
							EncyclopediaPage encyclopediaPage2 = Activator.CreateInstance(type3) as EncyclopediaPage;
							foreach (Type type4 in encyclopediaModel.PageTargetTypes)
							{
								if (!this._pages.ContainsKey(type4))
								{
									this._pages.Add(type4, encyclopediaPage2);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x0007B708 File Offset: 0x00079908
		public IEnumerable<EncyclopediaPage> GetEncyclopediaPages()
		{
			return this._pages.Values.Distinct<EncyclopediaPage>();
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x0007B71A File Offset: 0x0007991A
		public EncyclopediaPage GetPageOf(Type type)
		{
			return this._pages[type];
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x0007B728 File Offset: 0x00079928
		public string GetIdentifier(Type type)
		{
			return this._pages[type].GetIdentifier(type);
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x0007B73C File Offset: 0x0007993C
		public void GoToLink(string pageType, string stringID)
		{
			if (this._executeLink == null || string.IsNullOrEmpty(pageType))
			{
				return;
			}
			if (pageType == "Home" || pageType == "LastPage")
			{
				this._executeLink(pageType, null);
				return;
			}
			if (pageType == "ListPage")
			{
				EncyclopediaPage encyclopediaPage = Campaign.Current.EncyclopediaManager.GetEncyclopediaPages().FirstOrDefault((EncyclopediaPage e) => e.HasIdentifier(stringID));
				this._executeLink(pageType, encyclopediaPage);
				return;
			}
			EncyclopediaPage encyclopediaPage2 = Campaign.Current.EncyclopediaManager.GetEncyclopediaPages().FirstOrDefault((EncyclopediaPage e) => e.HasIdentifier(pageType));
			MBObjectBase @object = encyclopediaPage2.GetObject(pageType, stringID);
			if (encyclopediaPage2 != null && encyclopediaPage2.IsValidEncyclopediaItem(@object))
			{
				this._executeLink(pageType, @object);
			}
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x0007B848 File Offset: 0x00079A48
		public void GoToLink(string link)
		{
			string[] array = link.ToString().Split(new char[] { '-' });
			this.GoToLink(array[0], array[1]);
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x0007B878 File Offset: 0x00079A78
		public void SetLinkCallback(Action<string, object> ExecuteLink)
		{
			this._executeLink = ExecuteLink;
		}

		// Token: 0x0400089D RID: 2205
		private Dictionary<Type, EncyclopediaPage> _pages;

		// Token: 0x0400089F RID: 2207
		public const string HOME_ID = "Home";

		// Token: 0x040008A0 RID: 2208
		public const string LIST_PAGE_ID = "ListPage";

		// Token: 0x040008A1 RID: 2209
		public const string LAST_PAGE_ID = "LastPage";

		// Token: 0x040008A2 RID: 2210
		private Action<string, object> _executeLink;
	}
}
