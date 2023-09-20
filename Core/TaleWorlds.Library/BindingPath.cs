using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Library
{
	public class BindingPath
	{
		public string Path
		{
			get
			{
				return this._path;
			}
		}

		public string[] Nodes { get; private set; }

		public string FirstNode
		{
			get
			{
				return this.Nodes[0];
			}
		}

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

		private BindingPath(string path, string[] nodes)
		{
			this._path = path;
			this.Nodes = nodes;
		}

		public BindingPath(string path)
		{
			this._path = path;
			this.Nodes = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public BindingPath(int path)
		{
			this._path = path.ToString();
			this.Nodes = new string[] { this._path };
		}

		public static BindingPath CreateFromProperty(string propertyName)
		{
			return new BindingPath(propertyName, new string[] { propertyName });
		}

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

		public override int GetHashCode()
		{
			return this._path.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			BindingPath bindingPath = obj as BindingPath;
			return !(bindingPath == null) && this.Path == bindingPath.Path;
		}

		public static bool operator ==(BindingPath a, BindingPath b)
		{
			bool flag = a == null;
			bool flag2 = b == null;
			return (flag && flag2) || (!flag && !flag2 && a.Path == b.Path);
		}

		public static bool operator !=(BindingPath a, BindingPath b)
		{
			return !(a == b);
		}

		public static bool IsRelatedWithPathAsString(string path, string referencePath)
		{
			return referencePath.StartsWith(path);
		}

		public static bool IsRelatedWithPath(string path, BindingPath referencePath)
		{
			return referencePath.Path.StartsWith(path);
		}

		public bool IsRelatedWith(BindingPath referencePath)
		{
			return BindingPath.IsRelatedWithPath(this.Path, referencePath);
		}

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

		public BindingPath Append(BindingPath bindingPath)
		{
			return new BindingPath(this.Nodes, bindingPath.Nodes);
		}

		public override string ToString()
		{
			return this.Path;
		}

		private readonly string _path;
	}
}
