using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Library
{
	// Token: 0x0200001A RID: 26
	public class BindingPath
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600007B RID: 123 RVA: 0x000036E2 File Offset: 0x000018E2
		public string Path
		{
			get
			{
				return this._path;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600007C RID: 124 RVA: 0x000036EA File Offset: 0x000018EA
		// (set) Token: 0x0600007D RID: 125 RVA: 0x000036F2 File Offset: 0x000018F2
		public string[] Nodes { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600007E RID: 126 RVA: 0x000036FB File Offset: 0x000018FB
		public string FirstNode
		{
			get
			{
				return this.Nodes[0];
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00003705 File Offset: 0x00001905
		public string LastNode
		{
			get
			{
				if (this.Nodes.Length == 0)
				{
					return "";
				}
				return this.Nodes[this.Nodes.Length - 1];
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003727 File Offset: 0x00001927
		private BindingPath(string path, string[] nodes)
		{
			this._path = path;
			this.Nodes = nodes;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000373D File Offset: 0x0000193D
		public BindingPath(string path)
		{
			this._path = path;
			this.Nodes = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003764 File Offset: 0x00001964
		public BindingPath(int path)
		{
			this._path = path.ToString();
			this.Nodes = new string[] { this._path };
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000378E File Offset: 0x0000198E
		public static BindingPath CreateFromProperty(string propertyName)
		{
			return new BindingPath(propertyName, new string[] { propertyName });
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000037A0 File Offset: 0x000019A0
		public BindingPath(IEnumerable<string> nodes)
		{
			this.Nodes = nodes.ToArray<string>();
			this._path = "";
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, ".ctor");
			for (int i = 0; i < this.Nodes.Length; i++)
			{
				string text = this.Nodes[i];
				mbstringBuilder.Append<string>(text);
				if (i + 1 != this.Nodes.Length)
				{
					mbstringBuilder.Append('\\');
				}
			}
			this._path = mbstringBuilder.ToStringAndRelease();
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000382C File Offset: 0x00001A2C
		private BindingPath(string[] firstNodes, string[] secondNodes)
		{
			this.Nodes = new string[firstNodes.Length + secondNodes.Length];
			this._path = "";
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, ".ctor");
			for (int i = 0; i < firstNodes.Length; i++)
			{
				this.Nodes[i] = firstNodes[i];
			}
			for (int j = 0; j < secondNodes.Length; j++)
			{
				this.Nodes[j + firstNodes.Length] = secondNodes[j];
			}
			for (int k = 0; k < this.Nodes.Length; k++)
			{
				string text = this.Nodes[k];
				mbstringBuilder.Append<string>(text);
				if (k + 1 != this.Nodes.Length)
				{
					mbstringBuilder.Append('\\');
				}
			}
			this._path = mbstringBuilder.ToStringAndRelease();
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000086 RID: 134 RVA: 0x000038F4 File Offset: 0x00001AF4
		public BindingPath SubPath
		{
			get
			{
				if (this.Nodes.Length > 1)
				{
					MBStringBuilder mbstringBuilder = default(MBStringBuilder);
					mbstringBuilder.Initialize(16, "SubPath");
					for (int i = 1; i < this.Nodes.Length; i++)
					{
						mbstringBuilder.Append<string>(this.Nodes[i]);
						if (i + 1 < this.Nodes.Length)
						{
							mbstringBuilder.Append('\\');
						}
					}
					return new BindingPath(mbstringBuilder.ToStringAndRelease());
				}
				return null;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000087 RID: 135 RVA: 0x0000396C File Offset: 0x00001B6C
		public BindingPath ParentPath
		{
			get
			{
				if (this.Nodes.Length > 1)
				{
					string text = "";
					for (int i = 0; i < this.Nodes.Length - 1; i++)
					{
						text += this.Nodes[i];
						if (i + 1 < this.Nodes.Length - 1)
						{
							text += "\\";
						}
					}
					return new BindingPath(text);
				}
				return null;
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000039D1 File Offset: 0x00001BD1
		public override int GetHashCode()
		{
			return this._path.GetHashCode();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000039E0 File Offset: 0x00001BE0
		public override bool Equals(object obj)
		{
			BindingPath bindingPath = obj as BindingPath;
			return !(bindingPath == null) && this.Path == bindingPath.Path;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003A10 File Offset: 0x00001C10
		public static bool operator ==(BindingPath a, BindingPath b)
		{
			bool flag = a == null;
			bool flag2 = b == null;
			return (flag && flag2) || (!flag && !flag2 && a.Path == b.Path);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003A46 File Offset: 0x00001C46
		public static bool operator !=(BindingPath a, BindingPath b)
		{
			return !(a == b);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00003A52 File Offset: 0x00001C52
		public static bool IsRelatedWithPathAsString(string path, string referencePath)
		{
			return referencePath.StartsWith(path);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00003A60 File Offset: 0x00001C60
		public static bool IsRelatedWithPath(string path, BindingPath referencePath)
		{
			return referencePath.Path.StartsWith(path);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003A73 File Offset: 0x00001C73
		public bool IsRelatedWith(BindingPath referencePath)
		{
			return BindingPath.IsRelatedWithPath(this.Path, referencePath);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00003A84 File Offset: 0x00001C84
		public void DecrementIfRelatedWith(BindingPath path, int startIndex)
		{
			if (!this.IsRelatedWith(path) || path.Nodes.Length >= this.Nodes.Length)
			{
				return;
			}
			int num;
			if (int.TryParse(this.Nodes[path.Nodes.Length], out num) && num >= startIndex)
			{
				num--;
				this.Nodes[path.Nodes.Length] = num.ToString();
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00003AE8 File Offset: 0x00001CE8
		public BindingPath Simplify()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.Nodes.Length; i++)
			{
				string text = this.Nodes[i];
				if (text == ".." && list.Count > 0 && list[list.Count - 1] != "..")
				{
					list.RemoveAt(list.Count - 1);
				}
				else
				{
					list.Add(text);
				}
			}
			return new BindingPath(list);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003B64 File Offset: 0x00001D64
		public BindingPath Append(BindingPath bindingPath)
		{
			return new BindingPath(this.Nodes, bindingPath.Nodes);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00003B77 File Offset: 0x00001D77
		public override string ToString()
		{
			return this.Path;
		}

		// Token: 0x04000057 RID: 87
		private readonly string _path;
	}
}
