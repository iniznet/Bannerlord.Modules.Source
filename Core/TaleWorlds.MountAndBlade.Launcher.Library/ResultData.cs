﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class ResultData
	{
		public string Errors { get; set; } = "";

		public List<DLLResult> DLLs { get; set; } = new List<DLLResult>();

		public void AddDLLResult(string dllName, bool isSafe, string information)
		{
			this.DLLs.Add(new DLLResult(dllName, isSafe, information));
		}

		public override string ToString()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ResultData));
			string text;
			using (StringWriter stringWriter = new StringWriter())
			{
				xmlSerializer.Serialize(stringWriter, this);
				text = stringWriter.ToString();
			}
			return text;
		}
	}
}
