using System;
using System.IO;
using System.Xml;

namespace TaleWorlds.Library
{
	public class ParameterFile
	{
		public string Path { get; private set; }

		public DateTime LastCheckedTime { get; private set; }

		public ParameterContainer ParameterContainer { get; private set; }

		public ParameterFile(string path)
		{
			this.ParameterContainer = new ParameterContainer();
			this.Path = path;
			this.LastCheckedTime = DateTime.MinValue;
		}

		public bool CheckIfNeedsToBeRefreshed()
		{
			return File.GetLastWriteTime(this.Path) > this.LastCheckedTime;
		}

		public void Refresh()
		{
			this.ParameterContainer.ClearParameters();
			DateTime lastWriteTime = File.GetLastWriteTime(this.Path);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(this.Path);
			}
			catch
			{
				this._failedAttemptsCount++;
				if (this._failedAttemptsCount >= 100)
				{
					Debug.FailedAssert("Could not load parameters file", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\ParameterFile.cs", "Refresh", 47);
				}
				return;
			}
			this._failedAttemptsCount = 0;
			foreach (object obj in xmlDocument.FirstChild.ChildNodes)
			{
				XmlElement xmlElement = (XmlElement)obj;
				string attribute = xmlElement.GetAttribute("name");
				string attribute2 = xmlElement.GetAttribute("value");
				this.ParameterContainer.AddParameter(attribute, attribute2, true);
			}
			this.LastCheckedTime = lastWriteTime;
		}

		private int _failedAttemptsCount;

		private const int MaxFailedAttemptsCount = 100;
	}
}
