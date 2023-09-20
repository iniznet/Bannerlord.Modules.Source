using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network
{
	public static class DebugNetworkEventStatistics
	{
		public static event Action<IEnumerable<DebugNetworkEventStatistics.TotalEventData>> OnEventDataUpdated;

		public static event Action<DebugNetworkEventStatistics.PerSecondEventData> OnPerSecondEventDataUpdated;

		public static event Action<IEnumerable<float>> OnFPSEventUpdated;

		public static event Action OnOpenExternalMonitor;

		public static int SamplesPerSecond
		{
			get
			{
				return DebugNetworkEventStatistics._samplesPerSecond;
			}
			set
			{
				DebugNetworkEventStatistics._samplesPerSecond = value;
				DebugNetworkEventStatistics.MaxGraphPointCount = value * 5;
			}
		} = 10;

		public static bool IsActive { get; private set; }

		internal static void StartEvent(string eventName, int eventType)
		{
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics._curEventType = eventType;
			if (!DebugNetworkEventStatistics._statistics.ContainsKey(DebugNetworkEventStatistics._curEventType))
			{
				DebugNetworkEventStatistics._statistics.Add(DebugNetworkEventStatistics._curEventType, new DebugNetworkEventStatistics.PerEventData
				{
					Name = eventName
				});
			}
			DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType].Count++;
			DebugNetworkEventStatistics._totalData.TotalCount++;
		}

		internal static void EndEvent()
		{
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics.PerEventData perEventData = DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType];
			perEventData.DataSize = perEventData.TotalDataSize / perEventData.Count;
			DebugNetworkEventStatistics._curEventType = -1;
		}

		internal static void AddDataToStatistic(int bitCount)
		{
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType].TotalDataSize += bitCount;
			DebugNetworkEventStatistics._totalData.TotalDataSize += bitCount;
		}

		public static void OpenExternalMonitor()
		{
			if (DebugNetworkEventStatistics.OnOpenExternalMonitor != null)
			{
				DebugNetworkEventStatistics.OnOpenExternalMonitor();
			}
		}

		public static void ControlActivate()
		{
			DebugNetworkEventStatistics.IsActive = true;
		}

		public static void ControlDeactivate()
		{
			DebugNetworkEventStatistics.IsActive = false;
		}

		public static void ControlJustDump()
		{
			DebugNetworkEventStatistics.DumpData();
		}

		public static void ControlDumpAll()
		{
			DebugNetworkEventStatistics.DumpData();
			DebugNetworkEventStatistics.DumpReplicationData();
		}

		public static void ControlClear()
		{
			DebugNetworkEventStatistics.Clear();
		}

		public static void ClearNetGraphs()
		{
			DebugNetworkEventStatistics._eventSamples.Clear();
			DebugNetworkEventStatistics._lossSamples.Clear();
			DebugNetworkEventStatistics._prevEventData = new DebugNetworkEventStatistics.TotalEventData();
			DebugNetworkEventStatistics._currEventData = new DebugNetworkEventStatistics.TotalEventData();
			DebugNetworkEventStatistics._collectSampleCheck = 0f;
		}

		public static void ClearFpsGraph()
		{
			DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Clear();
			DebugNetworkEventStatistics._fpsSamples.Clear();
			DebugNetworkEventStatistics._collectFpsSampleCheck = 0f;
		}

		public static void ControlClearAll()
		{
			DebugNetworkEventStatistics.Clear();
			DebugNetworkEventStatistics.ClearFpsGraph();
			DebugNetworkEventStatistics.ClearNetGraphs();
			DebugNetworkEventStatistics.ClearReplicationData();
		}

		public static void ControlDumpReplicationData()
		{
			DebugNetworkEventStatistics.DumpReplicationData();
			DebugNetworkEventStatistics.ClearReplicationData();
		}

		public static void EndTick(float dt)
		{
			if (DebugNetworkEventStatistics._useImgui && Input.DebugInput.IsHotKeyPressed("DebugNetworkEventStatisticsHotkeyToggleActive"))
			{
				DebugNetworkEventStatistics.ToggleActive();
				if (DebugNetworkEventStatistics.IsActive)
				{
					Imgui.NewFrame();
				}
			}
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics._totalData.TotalTime += dt;
			DebugNetworkEventStatistics._totalData.TotalFrameCount++;
			if (DebugNetworkEventStatistics._useImgui)
			{
				Imgui.BeginMainThreadScope();
				Imgui.Begin("Network panel");
				if (Imgui.Button("Disable Network Panel"))
				{
					DebugNetworkEventStatistics.ToggleActive();
				}
				Imgui.Separator();
				if (Imgui.Button("Show Upload Data (screen)"))
				{
					DebugNetworkEventStatistics._showUploadDataText = !DebugNetworkEventStatistics._showUploadDataText;
				}
				Imgui.Separator();
				if (Imgui.Button("Clear Data"))
				{
					DebugNetworkEventStatistics.Clear();
				}
				if (Imgui.Button("Dump Data (console)"))
				{
					DebugNetworkEventStatistics.DumpData();
				}
				Imgui.Separator();
				if (Imgui.Button("Clear Replication Data"))
				{
					DebugNetworkEventStatistics.ClearReplicationData();
				}
				if (Imgui.Button("Dump Replication Data (console)"))
				{
					DebugNetworkEventStatistics.DumpReplicationData();
				}
				if (Imgui.Button("Dump & Clear Replication Data (console)"))
				{
					DebugNetworkEventStatistics.DumpReplicationData();
					DebugNetworkEventStatistics.ClearReplicationData();
				}
				if (DebugNetworkEventStatistics._showUploadDataText)
				{
					Imgui.Separator();
					DebugNetworkEventStatistics.ShowUploadData();
				}
				Imgui.End();
			}
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics.CollectFpsSample(dt);
			DebugNetworkEventStatistics._collectSampleCheck += dt;
			if (DebugNetworkEventStatistics._collectSampleCheck >= 1f / (float)DebugNetworkEventStatistics.SamplesPerSecond)
			{
				DebugNetworkEventStatistics._currEventData = DebugNetworkEventStatistics.GetCurrentEventData();
				if (DebugNetworkEventStatistics._currEventData.HasData && DebugNetworkEventStatistics._prevEventData.HasData && DebugNetworkEventStatistics._currEventData != DebugNetworkEventStatistics._prevEventData)
				{
					DebugNetworkEventStatistics._lossSamples.Enqueue(GameNetwork.GetAveragePacketLossRatio());
					DebugNetworkEventStatistics._eventSamples.Enqueue(DebugNetworkEventStatistics._currEventData - DebugNetworkEventStatistics._prevEventData);
					DebugNetworkEventStatistics._prevEventData = DebugNetworkEventStatistics._currEventData;
					if (DebugNetworkEventStatistics._eventSamples.Count > DebugNetworkEventStatistics.MaxGraphPointCount)
					{
						DebugNetworkEventStatistics._eventSamples.Dequeue();
						DebugNetworkEventStatistics._lossSamples.Dequeue();
					}
					if (DebugNetworkEventStatistics._eventSamples.Count >= DebugNetworkEventStatistics.SamplesPerSecond)
					{
						List<DebugNetworkEventStatistics.TotalEventData> range = DebugNetworkEventStatistics._eventSamples.ToList<DebugNetworkEventStatistics.TotalEventData>().GetRange(DebugNetworkEventStatistics._eventSamples.Count - DebugNetworkEventStatistics.SamplesPerSecond, DebugNetworkEventStatistics.SamplesPerSecond);
						DebugNetworkEventStatistics.UploadPerSecondEventData = new DebugNetworkEventStatistics.PerSecondEventData(range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalConstantsUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalReliableUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalReplicationUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalUnreliableUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalOtherUpload));
						if (DebugNetworkEventStatistics.OnPerSecondEventDataUpdated != null)
						{
							DebugNetworkEventStatistics.OnPerSecondEventDataUpdated(DebugNetworkEventStatistics.UploadPerSecondEventData);
						}
					}
					if (DebugNetworkEventStatistics.OnEventDataUpdated != null)
					{
						DebugNetworkEventStatistics.OnEventDataUpdated(DebugNetworkEventStatistics._eventSamples.ToList<DebugNetworkEventStatistics.TotalEventData>());
					}
					DebugNetworkEventStatistics._collectSampleCheck -= 1f / (float)DebugNetworkEventStatistics.SamplesPerSecond;
				}
			}
			if (DebugNetworkEventStatistics._useImgui)
			{
				Imgui.Begin("Network Graph panel");
				float[] array = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalUpload / 8192f).ToArray<float>();
				float num = ((array.Length != 0) ? array.Max() : 0f);
				DebugNetworkEventStatistics._targetMaxGraphHeight = (DebugNetworkEventStatistics._useAbsoluteMaximum ? MathF.Max(num, DebugNetworkEventStatistics._targetMaxGraphHeight) : num);
				float num2 = MBMath.ClampFloat(3f * dt, 0f, 1f);
				DebugNetworkEventStatistics._curMaxGraphHeight = MBMath.Lerp(DebugNetworkEventStatistics._curMaxGraphHeight, DebugNetworkEventStatistics._targetMaxGraphHeight, num2, 1E-05f);
				if (DebugNetworkEventStatistics.UploadPerSecondEventData != null)
				{
					Imgui.Text(string.Concat(new object[]
					{
						"Taking ",
						DebugNetworkEventStatistics.SamplesPerSecond,
						" samples per second. Total KiB per second:",
						(float)DebugNetworkEventStatistics.UploadPerSecondEventData.TotalUploadPerSecond / 8192f
					}));
				}
				float[] array2 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalConstantsUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array2, DebugNetworkEventStatistics._eventSamples.Count, 0, "Constants upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				float[] array3 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalReliableUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array3, DebugNetworkEventStatistics._eventSamples.Count, 0, "Reliable upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				float[] array4 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalReplicationUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array4, DebugNetworkEventStatistics._eventSamples.Count, 0, "Replication upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				float[] array5 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalUnreliableUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array5, DebugNetworkEventStatistics._eventSamples.Count, 0, "Unreliable upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				float[] array6 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalOtherUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array6, DebugNetworkEventStatistics._eventSamples.Count, 0, "Other upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				Imgui.Separator();
				float[] array7 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalUpload / (float)x.TotalPackets / 8f).ToArray<float>();
				Imgui.PlotLines("", array7, DebugNetworkEventStatistics._eventSamples.Count, 0, "Data per package (in B)", 0f, 1400f, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + 1400);
				Imgui.Separator();
				float num3 = ((DebugNetworkEventStatistics._lossSamples.Count > 0) ? DebugNetworkEventStatistics._lossSamples.Max() : 0f);
				DebugNetworkEventStatistics._targetMaxLossGraphHeight = (DebugNetworkEventStatistics._useAbsoluteMaximum ? MathF.Max(num3, DebugNetworkEventStatistics._targetMaxLossGraphHeight) : num3);
				float num4 = MBMath.ClampFloat(3f * dt, 0f, 1f);
				DebugNetworkEventStatistics._currMaxLossGraphHeight = MBMath.Lerp(DebugNetworkEventStatistics._currMaxLossGraphHeight, DebugNetworkEventStatistics._targetMaxLossGraphHeight, num4, 1E-05f);
				Imgui.PlotLines("", DebugNetworkEventStatistics._lossSamples.ToArray(), DebugNetworkEventStatistics._lossSamples.Count, 0, "Averaged loss ratio", 0f, DebugNetworkEventStatistics._currMaxLossGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._currMaxLossGraphHeight);
				Imgui.Checkbox("Use absolute Maximum", ref DebugNetworkEventStatistics._useAbsoluteMaximum);
				Imgui.End();
			}
			Imgui.EndMainThreadScope();
		}

		private static void CollectFpsSample(float dt)
		{
			if (DebugNetworkEventStatistics.TrackFps)
			{
				float fps = Utilities.GetFps();
				if (!float.IsInfinity(fps) && !float.IsNegativeInfinity(fps) && !float.IsNaN(fps))
				{
					DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Add(fps);
				}
				DebugNetworkEventStatistics._collectFpsSampleCheck += dt;
				if (DebugNetworkEventStatistics._collectFpsSampleCheck >= 1f / (float)DebugNetworkEventStatistics.SamplesPerSecond)
				{
					if (DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Count > 0)
					{
						DebugNetworkEventStatistics._fpsSamples.Enqueue(DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Min());
						DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Clear();
						if (DebugNetworkEventStatistics._fpsSamples.Count > DebugNetworkEventStatistics.MaxGraphPointCount)
						{
							DebugNetworkEventStatistics._fpsSamples.Dequeue();
						}
						Action<IEnumerable<float>> onFPSEventUpdated = DebugNetworkEventStatistics.OnFPSEventUpdated;
						if (onFPSEventUpdated != null)
						{
							onFPSEventUpdated(DebugNetworkEventStatistics._fpsSamples.ToList<float>());
						}
					}
					DebugNetworkEventStatistics._collectFpsSampleCheck -= 1f / (float)DebugNetworkEventStatistics.SamplesPerSecond;
				}
			}
		}

		private static void ToggleActive()
		{
			DebugNetworkEventStatistics.IsActive = !DebugNetworkEventStatistics.IsActive;
		}

		private static void Clear()
		{
			DebugNetworkEventStatistics._totalData = new DebugNetworkEventStatistics.TotalData();
			DebugNetworkEventStatistics._statistics = new Dictionary<int, DebugNetworkEventStatistics.PerEventData>();
			GameNetwork.ResetDebugUploads();
			DebugNetworkEventStatistics._curEventType = -1;
		}

		private static void DumpData()
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "DumpData");
			mbstringBuilder.AppendLine();
			mbstringBuilder.AppendLine<string>("///GENERAL DATA///");
			mbstringBuilder.AppendLine<string>("Total elapsed time: " + DebugNetworkEventStatistics._totalData.TotalTime + " seconds.");
			mbstringBuilder.AppendLine<string>("Total frame count: " + DebugNetworkEventStatistics._totalData.TotalFrameCount);
			mbstringBuilder.AppendLine<string>("Total avg packet count: " + (int)(DebugNetworkEventStatistics._totalData.TotalTime / 60f));
			mbstringBuilder.AppendLine<string>("Total event data size: " + DebugNetworkEventStatistics._totalData.TotalDataSize + " bits.");
			mbstringBuilder.AppendLine<string>("Total event count: " + DebugNetworkEventStatistics._totalData.TotalCount);
			mbstringBuilder.AppendLine();
			mbstringBuilder.AppendLine<string>("///ALL DATA///");
			List<DebugNetworkEventStatistics.PerEventData> list = new List<DebugNetworkEventStatistics.PerEventData>();
			list.AddRange(DebugNetworkEventStatistics._statistics.Values);
			list.Sort();
			foreach (DebugNetworkEventStatistics.PerEventData perEventData in list)
			{
				mbstringBuilder.AppendLine<string>("Event name: " + perEventData.Name);
				mbstringBuilder.AppendLine<string>("\tEvent size (for one event): " + perEventData.DataSize + " bits.");
				mbstringBuilder.AppendLine<string>("\tTotal count: " + perEventData.Count);
				mbstringBuilder.AppendLine<string>(string.Concat(new object[]
				{
					"\tTotal size: ",
					perEventData.TotalDataSize,
					"bits | ~",
					perEventData.TotalDataSize / 8 + ((perEventData.TotalDataSize % 8 == 0) ? 0 : 1),
					" bytes."
				}));
				mbstringBuilder.AppendLine<string>("\tTotal count per frame: " + (float)perEventData.Count / (float)DebugNetworkEventStatistics._totalData.TotalFrameCount);
				mbstringBuilder.AppendLine<string>("\tTotal size per frame: " + (float)perEventData.TotalDataSize / (float)DebugNetworkEventStatistics._totalData.TotalFrameCount + " bits per frame.");
				mbstringBuilder.AppendLine();
			}
			DebugNetworkEventStatistics.GetFormattedDebugUploadDataOutput(ref mbstringBuilder);
			mbstringBuilder.AppendLine<string>("NetworkEventStaticticsLogLength: " + mbstringBuilder.Length + "\n");
			MBDebug.Print(mbstringBuilder.ToStringAndRelease(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		private static void GetFormattedDebugUploadDataOutput(ref MBStringBuilder outStr)
		{
			GameNetwork.DebugNetworkPacketStatisticsStruct debugNetworkPacketStatisticsStruct = default(GameNetwork.DebugNetworkPacketStatisticsStruct);
			GameNetwork.DebugNetworkPositionCompressionStatisticsStruct debugNetworkPositionCompressionStatisticsStruct = default(GameNetwork.DebugNetworkPositionCompressionStatisticsStruct);
			GameNetwork.GetDebugUploadsInBits(ref debugNetworkPacketStatisticsStruct, ref debugNetworkPositionCompressionStatisticsStruct);
			outStr.AppendLine<string>("REAL NETWORK UPLOAD PERCENTS");
			if (debugNetworkPacketStatisticsStruct.TotalUpload == 0)
			{
				outStr.AppendLine<string>("Total Upload is ZERO");
				return;
			}
			int num = debugNetworkPacketStatisticsStruct.TotalUpload - (debugNetworkPacketStatisticsStruct.TotalConstantsUpload + debugNetworkPacketStatisticsStruct.TotalReliableEventUpload + debugNetworkPacketStatisticsStruct.TotalReplicationUpload + debugNetworkPacketStatisticsStruct.TotalUnreliableEventUpload);
			if (num == debugNetworkPacketStatisticsStruct.TotalUpload)
			{
				outStr.AppendLine<string>("USE_DEBUG_NETWORK_PACKET_PERCENTS not defined!");
			}
			else
			{
				outStr.AppendLine<string>("\tAverage Ping: " + debugNetworkPacketStatisticsStruct.AveragePingTime);
				outStr.AppendLine<string>("\tTime out period: " + debugNetworkPacketStatisticsStruct.TimeOutPeriod);
				outStr.AppendLine<string>("\tLost Percent: " + debugNetworkPacketStatisticsStruct.LostPercent);
				outStr.AppendLine<string>("\tlost_count: " + debugNetworkPacketStatisticsStruct.LostCount);
				outStr.AppendLine<string>("\ttotal_count_on_lost_check: " + debugNetworkPacketStatisticsStruct.TotalCountOnLostCheck);
				outStr.AppendLine<string>("\tround_trip_time: " + debugNetworkPacketStatisticsStruct.RoundTripTime);
				float num2 = (float)debugNetworkPacketStatisticsStruct.TotalUpload;
				float num3 = 1f / (float)debugNetworkPacketStatisticsStruct.TotalPackets;
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tConstants Upload: percent: ",
					(float)debugNetworkPacketStatisticsStruct.TotalConstantsUpload / num2 * 100f,
					"; size in bits: ",
					(float)debugNetworkPacketStatisticsStruct.TotalConstantsUpload * num3,
					";"
				}));
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tReliable Upload: percent: ",
					(float)debugNetworkPacketStatisticsStruct.TotalReliableEventUpload / num2 * 100f,
					"; size in bits: ",
					(float)debugNetworkPacketStatisticsStruct.TotalReliableEventUpload * num3,
					";"
				}));
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tReplication Upload: percent: ",
					(float)debugNetworkPacketStatisticsStruct.TotalReplicationUpload / num2 * 100f,
					"; size in bits: ",
					(float)debugNetworkPacketStatisticsStruct.TotalReplicationUpload * num3,
					";"
				}));
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tUnreliable Upload: percent: ",
					(float)debugNetworkPacketStatisticsStruct.TotalUnreliableEventUpload / num2 * 100f,
					"; size in bits: ",
					(float)debugNetworkPacketStatisticsStruct.TotalUnreliableEventUpload * num3,
					";"
				}));
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tOthers (headers, ack etc.) Upload: percent: ",
					(float)num / num2 * 100f,
					"; size in bits: ",
					(float)num * num3,
					";"
				}));
				int num4 = debugNetworkPositionCompressionStatisticsStruct.totalPositionCoarseBitCountX + debugNetworkPositionCompressionStatisticsStruct.totalPositionCoarseBitCountY + debugNetworkPositionCompressionStatisticsStruct.totalPositionCoarseBitCountZ;
				float num5 = 1f / (float)debugNetworkPacketStatisticsStruct.TotalCellPriorityChecks;
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\n\tTotal PPS: ",
					(float)debugNetworkPacketStatisticsStruct.TotalPackets / DebugNetworkEventStatistics._totalData.TotalTime,
					"; bps: ",
					(float)debugNetworkPacketStatisticsStruct.TotalUpload / DebugNetworkEventStatistics._totalData.TotalTime,
					";"
				}));
			}
			outStr.AppendLine<string>(string.Concat(new object[]
			{
				"\n\tTotal packets: ",
				debugNetworkPacketStatisticsStruct.TotalPackets,
				"; bits per packet: ",
				(float)debugNetworkPacketStatisticsStruct.TotalUpload / (float)debugNetworkPacketStatisticsStruct.TotalPackets,
				";"
			}));
			outStr.AppendLine<string>("Total Upload: " + debugNetworkPacketStatisticsStruct.TotalUpload + " in bits");
		}

		private static void ShowUploadData()
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "ShowUploadData");
			DebugNetworkEventStatistics.GetFormattedDebugUploadDataOutput(ref mbstringBuilder);
			string[] array = mbstringBuilder.ToStringAndRelease().Split(new char[] { '\n' });
			for (int i = 0; i < array.Length; i++)
			{
				Imgui.Text(array[i]);
			}
		}

		private static DebugNetworkEventStatistics.TotalEventData GetCurrentEventData()
		{
			GameNetwork.DebugNetworkPacketStatisticsStruct debugNetworkPacketStatisticsStruct = default(GameNetwork.DebugNetworkPacketStatisticsStruct);
			GameNetwork.DebugNetworkPositionCompressionStatisticsStruct debugNetworkPositionCompressionStatisticsStruct = default(GameNetwork.DebugNetworkPositionCompressionStatisticsStruct);
			GameNetwork.GetDebugUploadsInBits(ref debugNetworkPacketStatisticsStruct, ref debugNetworkPositionCompressionStatisticsStruct);
			return new DebugNetworkEventStatistics.TotalEventData(debugNetworkPacketStatisticsStruct.TotalPackets, debugNetworkPacketStatisticsStruct.TotalUpload, debugNetworkPacketStatisticsStruct.TotalConstantsUpload, debugNetworkPacketStatisticsStruct.TotalReliableEventUpload, debugNetworkPacketStatisticsStruct.TotalReplicationUpload, debugNetworkPacketStatisticsStruct.TotalUnreliableEventUpload);
		}

		private static void DumpReplicationData()
		{
			GameNetwork.PrintReplicationTableStatistics();
		}

		private static void ClearReplicationData()
		{
			GameNetwork.ClearReplicationTableStatistics();
		}

		private static DebugNetworkEventStatistics.TotalData _totalData = new DebugNetworkEventStatistics.TotalData();

		private static int _curEventType = -1;

		private static Dictionary<int, DebugNetworkEventStatistics.PerEventData> _statistics = new Dictionary<int, DebugNetworkEventStatistics.PerEventData>();

		private static int _samplesPerSecond;

		public static int MaxGraphPointCount;

		private static bool _showUploadDataText = false;

		private static bool _useAbsoluteMaximum = false;

		private static float _collectSampleCheck = 0f;

		private static float _collectFpsSampleCheck = 0f;

		private static float _curMaxGraphHeight = 0f;

		private static float _targetMaxGraphHeight = 0f;

		private static float _currMaxLossGraphHeight = 0f;

		private static float _targetMaxLossGraphHeight = 0f;

		private static DebugNetworkEventStatistics.PerSecondEventData UploadPerSecondEventData;

		private static readonly Queue<DebugNetworkEventStatistics.TotalEventData> _eventSamples = new Queue<DebugNetworkEventStatistics.TotalEventData>();

		private static readonly Queue<float> _lossSamples = new Queue<float>();

		private static DebugNetworkEventStatistics.TotalEventData _prevEventData = new DebugNetworkEventStatistics.TotalEventData();

		private static DebugNetworkEventStatistics.TotalEventData _currEventData = new DebugNetworkEventStatistics.TotalEventData();

		private static readonly List<float> _fpsSamplesUntilNextSampling = new List<float>();

		private static readonly Queue<float> _fpsSamples = new Queue<float>();

		private static bool _useImgui = !GameNetwork.IsDedicatedServer;

		public static bool TrackFps = false;

		public class TotalEventData
		{
			protected bool Equals(DebugNetworkEventStatistics.TotalEventData other)
			{
				return this.TotalPackets == other.TotalPackets && this.TotalUpload == other.TotalUpload && this.TotalConstantsUpload == other.TotalConstantsUpload && this.TotalReliableUpload == other.TotalReliableUpload && this.TotalReplicationUpload == other.TotalReplicationUpload && this.TotalUnreliableUpload == other.TotalUnreliableUpload && this.TotalOtherUpload == other.TotalOtherUpload;
			}

			public override bool Equals(object obj)
			{
				return obj != null && (this == obj || (obj.GetType() == base.GetType() && this.Equals((DebugNetworkEventStatistics.TotalEventData)obj)));
			}

			public override int GetHashCode()
			{
				return (((((((((((this.TotalPackets * 397) ^ this.TotalUpload) * 397) ^ this.TotalConstantsUpload) * 397) ^ this.TotalReliableUpload) * 397) ^ this.TotalReplicationUpload) * 397) ^ this.TotalUnreliableUpload) * 397) ^ this.TotalOtherUpload;
			}

			public TotalEventData()
			{
			}

			public TotalEventData(int totalPackets, int totalUpload, int totalConstants, int totalReliable, int totalReplication, int totalUnreliable)
			{
				this.TotalPackets = totalPackets;
				this.TotalUpload = totalUpload;
				this.TotalConstantsUpload = totalConstants;
				this.TotalReliableUpload = totalReliable;
				this.TotalReplicationUpload = totalReplication;
				this.TotalUnreliableUpload = totalUnreliable;
				this.TotalOtherUpload = totalUpload - (totalConstants + totalReliable + totalReplication + totalUnreliable);
			}

			public bool HasData
			{
				get
				{
					return this.TotalUpload > 0;
				}
			}

			public static DebugNetworkEventStatistics.TotalEventData operator -(DebugNetworkEventStatistics.TotalEventData d1, DebugNetworkEventStatistics.TotalEventData d2)
			{
				return new DebugNetworkEventStatistics.TotalEventData(d1.TotalPackets - d2.TotalPackets, d1.TotalUpload - d2.TotalUpload, d1.TotalConstantsUpload - d2.TotalConstantsUpload, d1.TotalReliableUpload - d2.TotalReliableUpload, d1.TotalReplicationUpload - d2.TotalReplicationUpload, d1.TotalUnreliableUpload - d2.TotalUnreliableUpload);
			}

			public static bool operator ==(DebugNetworkEventStatistics.TotalEventData d1, DebugNetworkEventStatistics.TotalEventData d2)
			{
				return d1.TotalPackets == d2.TotalPackets && d1.TotalUpload == d2.TotalUpload && d1.TotalConstantsUpload == d2.TotalConstantsUpload && d1.TotalReliableUpload == d2.TotalReliableUpload && d1.TotalReplicationUpload == d2.TotalReplicationUpload && d1.TotalUnreliableUpload == d2.TotalUnreliableUpload;
			}

			public static bool operator !=(DebugNetworkEventStatistics.TotalEventData d1, DebugNetworkEventStatistics.TotalEventData d2)
			{
				return !(d1 == d2);
			}

			public readonly int TotalPackets;

			public readonly int TotalUpload;

			public readonly int TotalConstantsUpload;

			public readonly int TotalReliableUpload;

			public readonly int TotalReplicationUpload;

			public readonly int TotalUnreliableUpload;

			public readonly int TotalOtherUpload;
		}

		private class PerEventData : IComparable<DebugNetworkEventStatistics.PerEventData>
		{
			public int CompareTo(DebugNetworkEventStatistics.PerEventData other)
			{
				return other.TotalDataSize - this.TotalDataSize;
			}

			public string Name;

			public int DataSize;

			public int TotalDataSize;

			public int Count;
		}

		public class PerSecondEventData
		{
			public PerSecondEventData(int totalUploadPerSecond, int constantsUploadPerSecond, int reliableUploadPerSecond, int replicationUploadPerSecond, int unreliableUploadPerSecond, int otherUploadPerSecond)
			{
				this.TotalUploadPerSecond = totalUploadPerSecond;
				this.ConstantsUploadPerSecond = constantsUploadPerSecond;
				this.ReliableUploadPerSecond = reliableUploadPerSecond;
				this.ReplicationUploadPerSecond = replicationUploadPerSecond;
				this.UnreliableUploadPerSecond = unreliableUploadPerSecond;
				this.OtherUploadPerSecond = otherUploadPerSecond;
			}

			public readonly int TotalUploadPerSecond;

			public readonly int ConstantsUploadPerSecond;

			public readonly int ReliableUploadPerSecond;

			public readonly int ReplicationUploadPerSecond;

			public readonly int UnreliableUploadPerSecond;

			public readonly int OtherUploadPerSecond;
		}

		private class TotalData
		{
			public float TotalTime;

			public int TotalFrameCount;

			public int TotalCount;

			public int TotalDataSize;
		}
	}
}
