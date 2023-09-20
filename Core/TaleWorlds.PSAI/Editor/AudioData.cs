using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using psai.net;

namespace psai.Editor
{
	// Token: 0x02000002 RID: 2
	[Serializable]
	public class AudioData : ICloneable
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		[XmlElement("Path")]
		public string FilePathRelativeToProjectDir
		{
			get
			{
				return this._filePathRelativeToProjectDir;
			}
			set
			{
				string text = value.Replace(Path.DirectorySeparatorChar, '/');
				this._filePathRelativeToProjectDir = text;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002072 File Offset: 0x00000272
		[XmlIgnore]
		public string FilePathRelativeToProjectDirForCurrentSystem
		{
			get
			{
				return this._filePathRelativeToProjectDir.Replace('/', Path.DirectorySeparatorChar);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002086 File Offset: 0x00000286
		// (set) Token: 0x06000005 RID: 5 RVA: 0x0000208E File Offset: 0x0000028E
		public float Bpm { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002097 File Offset: 0x00000297
		// (set) Token: 0x06000007 RID: 7 RVA: 0x0000209F File Offset: 0x0000029F
		public float PreBeats { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020A8 File Offset: 0x000002A8
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000020B0 File Offset: 0x000002B0
		public float PostBeats { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000020B9 File Offset: 0x000002B9
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020C1 File Offset: 0x000002C1
		public bool CalculatePostAndPrebeatLengthBasedOnBeats { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020CA File Offset: 0x000002CA
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000020E1 File Offset: 0x000002E1
		public int PreBeatLengthInSamples
		{
			get
			{
				if (this.CalculatePostAndPrebeatLengthBasedOnBeats)
				{
					return this.GetPrebeatLengthInSamplesBasedOnBeats();
				}
				return this._prebeatLengthInSamplesEnteredManually;
			}
			set
			{
				this._prebeatLengthInSamplesEnteredManually = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020EA File Offset: 0x000002EA
		// (set) Token: 0x0600000F RID: 15 RVA: 0x00002101 File Offset: 0x00000301
		public int PostBeatLengthInSamples
		{
			get
			{
				if (this.CalculatePostAndPrebeatLengthBasedOnBeats)
				{
					return this.GetPostbeatLengthInSamplesBasedOnBeats();
				}
				return this._postbeatLengthInSamplesEnteredManually;
			}
			set
			{
				this._postbeatLengthInSamplesEnteredManually = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000010 RID: 16 RVA: 0x0000210A File Offset: 0x0000030A
		// (set) Token: 0x06000011 RID: 17 RVA: 0x00002112 File Offset: 0x00000312
		public int TotalLengthInSamples { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000211B File Offset: 0x0000031B
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002123 File Offset: 0x00000323
		public int SampleRate { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000212C File Offset: 0x0000032C
		// (set) Token: 0x06000015 RID: 21 RVA: 0x00002134 File Offset: 0x00000334
		[XmlIgnore]
		public int BitsPerSample { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000016 RID: 22 RVA: 0x0000213D File Offset: 0x0000033D
		// (set) Token: 0x06000017 RID: 23 RVA: 0x00002145 File Offset: 0x00000345
		[XmlIgnore]
		public int ChannelCount { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000214E File Offset: 0x0000034E
		// (set) Token: 0x06000019 RID: 25 RVA: 0x00002156 File Offset: 0x00000356
		[XmlIgnore]
		public long ByteIndexOfWaveformDataWithinAudioFile { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001A RID: 26 RVA: 0x0000215F File Offset: 0x0000035F
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002167 File Offset: 0x00000367
		[XmlIgnore]
		public int LengthOfWaveformDataInBytes { get; set; }

		// Token: 0x0600001C RID: 28 RVA: 0x00002170 File Offset: 0x00000370
		public AudioData()
		{
			this.FilePathRelativeToProjectDir = "";
			this.BitsPerSample = 0;
			this.PostBeatLengthInSamples = 0;
			this.PreBeatLengthInSamples = 0;
			this.SampleRate = 0;
			this.LengthOfWaveformDataInBytes = 0;
			this.Bpm = 100f;
			this.PreBeats = 1f;
			this.PostBeats = 1f;
			this.CalculatePostAndPrebeatLengthBasedOnBeats = false;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000021E4 File Offset: 0x000003E4
		public AudioData CreatePsaiDotNetVersion()
		{
			AudioData audioData = new AudioData();
			audioData.filePathRelativeToProjectDir = this.FilePathRelativeToProjectDir;
			if (this.CalculatePostAndPrebeatLengthBasedOnBeats)
			{
				audioData.sampleCountPreBeat = this.GetPrebeatLengthInSamplesBasedOnBeats();
				audioData.sampleCountPostBeat = this.GetPostbeatLengthInSamplesBasedOnBeats();
			}
			else
			{
				audioData.sampleCountPreBeat = this.PreBeatLengthInSamples;
				audioData.sampleCountPostBeat = this.PostBeatLengthInSamples;
			}
			audioData.sampleCountTotal = this.TotalLengthInSamples;
			audioData.sampleRateHz = this.SampleRate;
			audioData.bpm = this.Bpm;
			return audioData;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002262 File Offset: 0x00000462
		public int GetMillisecondsFromSampleCount(int sampleCount)
		{
			return (int)((long)sampleCount * 1000L / (long)this.SampleRate);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002276 File Offset: 0x00000476
		public int GetSampleCountFromMilliseconds(int durationMs)
		{
			return this.SampleRate * durationMs / 1000;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002288 File Offset: 0x00000488
		public int GetLengthInSamplesBasedOnBeats(float bpm, float beats)
		{
			int num = (int)(60000f / bpm);
			return this.GetSampleCountFromMilliseconds((int)((float)num * beats));
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000022A9 File Offset: 0x000004A9
		public int GetPostbeatLengthInSamplesBasedOnBeats()
		{
			return this.GetLengthInSamplesBasedOnBeats(this.Bpm, this.PostBeats);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000022BD File Offset: 0x000004BD
		public int GetPrebeatLengthInSamplesBasedOnBeats()
		{
			return this.GetLengthInSamplesBasedOnBeats(this.Bpm, this.PreBeats);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000022D1 File Offset: 0x000004D1
		public static int CalculateTotalLengthInSamples(int lengthOfWaveformDataInBytes, int bitsPerSample, int channelCount)
		{
			if (lengthOfWaveformDataInBytes > 0 && bitsPerSample > 0 && channelCount > 0)
			{
				return lengthOfWaveformDataInBytes / (bitsPerSample / 8) / channelCount;
			}
			return 0;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000022E8 File Offset: 0x000004E8
		public bool DoUpdateMembersBasedOnWaveHeader(string fullPathToAudioFile, out string errorMessage)
		{
			bool flag = false;
			if (fullPathToAudioFile != null && fullPathToAudioFile.Length > 0)
			{
				string text = fullPathToAudioFile.Replace('/', Path.DirectorySeparatorChar);
				text = text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				if (File.Exists(text))
				{
					Stream stream = null;
					int num = 0;
					while (stream == null && num < 100)
					{
						try
						{
							stream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read);
						}
						catch (IOException ex)
						{
							errorMessage = ex.ToString() + "   numberOfTries=" + num;
							Thread.Sleep(50);
						}
						num++;
					}
					if (stream != null)
					{
						int num2;
						int num3;
						int num4;
						int num5;
						long num6;
						if (AudioData.ReadWaveHeader(stream, out num2, out num3, out num4, out num5, out num6) == PsaiResult.OK)
						{
							this.ChannelCount = num2;
							this.SampleRate = num3;
							this.LengthOfWaveformDataInBytes = num5;
							this.BitsPerSample = num4;
							this.ByteIndexOfWaveformDataWithinAudioFile = num6;
							this.TotalLengthInSamples = AudioData.CalculateTotalLengthInSamples(num5, num4, num2);
							errorMessage = "";
							flag = true;
						}
						else
						{
							errorMessage = "ERROR: file '" + text + "' contains an unsupported format. Please make sure your audio files are standard RIFF WAV files with up to 16 bits / 44.1kHz.";
						}
						stream.Close();
						return flag;
					}
					errorMessage = "ERROR: audio file '" + text + "' could not be opened. ";
					return false;
				}
			}
			errorMessage = "ERROR: audio file '" + fullPathToAudioFile + "' could not be found. Please make sure that all audio files reside within a subfolder of your project directory";
			return false;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002430 File Offset: 0x00000630
		public static bool SeekChunkInWaveHeader(ref BinaryReader reader, string chunk)
		{
			if (chunk.Length != 4)
			{
				return false;
			}
			Queue<byte> queue = new Queue<byte>(4);
			try
			{
				while (reader.BaseStream.CanRead)
				{
					byte b;
					do
					{
						b = reader.ReadByte();
						queue.Enqueue(b);
						if (queue.Count > 4)
						{
							queue.Dequeue();
						}
					}
					while ((char)b != chunk[3]);
					if (Encoding.ASCII.GetString(queue.ToArray()).Equals(chunk))
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return false;
			}
			return false;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000024CC File Offset: 0x000006CC
		public static PsaiResult ReadWaveHeader(Stream stream, out int outChannelCount, out int outSampleRate, out int outBitsPerSample, out int outLengthOfWaveformDatablockInBytes, out long outBytePositionOfWaveformData)
		{
			outChannelCount = 0;
			outBitsPerSample = 0;
			outSampleRate = 0;
			outLengthOfWaveformDatablockInBytes = 0;
			outBytePositionOfWaveformData = 0L;
			BinaryReader binaryReader = new BinaryReader(stream);
			if (new string(binaryReader.ReadChars(4)) != "RIFF")
			{
				binaryReader.Close();
				return PsaiResult.format_error;
			}
			binaryReader.ReadInt32();
			if (new string(binaryReader.ReadChars(4)) != "WAVE")
			{
				binaryReader.Close();
				return PsaiResult.format_error;
			}
			try
			{
				if (!AudioData.SeekChunkInWaveHeader(ref binaryReader, "fmt "))
				{
					Console.WriteLine(".wave file corrupt! format-chunk not found.");
					binaryReader.Close();
					return PsaiResult.format_error;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				binaryReader.Close();
				return PsaiResult.format_error;
			}
			long position = binaryReader.BaseStream.Position;
			int num = binaryReader.ReadInt32();
			binaryReader.ReadInt16();
			int num2 = (int)binaryReader.ReadInt16();
			int num3 = binaryReader.ReadInt32();
			binaryReader.ReadInt32();
			binaryReader.ReadInt16();
			int num4 = (int)binaryReader.ReadInt16();
			if (num4 > 16)
			{
				Console.WriteLine("OpenAL does not support playback of 24 bits. Please convert to 16 bits.");
				return PsaiResult.output_format_error;
			}
			long num5 = position + (long)num + 4L;
			binaryReader.BaseStream.Seek(num5, SeekOrigin.Begin);
			if (!AudioData.SeekChunkInWaveHeader(ref binaryReader, "data"))
			{
				binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				if (!AudioData.SeekChunkInWaveHeader(ref binaryReader, "data"))
				{
					Console.WriteLine("wave file corrupt! no 'data' chunk found!");
					binaryReader.Close();
					return PsaiResult.format_error;
				}
			}
			int num6 = stream.ReadByte() + stream.ReadByte() * 256 + stream.ReadByte() * 65536 + stream.ReadByte() * 16777216;
			outLengthOfWaveformDatablockInBytes = num6;
			outBytePositionOfWaveformData = stream.Position;
			outChannelCount = num2;
			outBitsPerSample = num4;
			outSampleRate = num3;
			binaryReader.Close();
			return PsaiResult.OK;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002688 File Offset: 0x00000888
		public static byte[] LoadWaveformDataToByteArray(string fullFilePath, long byteIndexOfWaveformDataWithinAudioFile, int lengthOfWaveformDataInBytes)
		{
			Stream stream = null;
			int num = 0;
			while (stream == null && num < 100)
			{
				string text = fullFilePath.Replace('/', Path.DirectorySeparatorChar);
				text = text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				try
				{
					stream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read);
				}
				catch (IOException ex)
				{
					Console.WriteLine(ex.ToString() + "   numberOfTries=" + num);
					Thread.Sleep(50);
				}
				num++;
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] array = null;
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				try
				{
					binaryReader.BaseStream.Position = byteIndexOfWaveformDataWithinAudioFile;
					array = binaryReader.ReadBytes(lengthOfWaveformDataInBytes);
				}
				catch (Exception ex2)
				{
					Console.WriteLine("Exception reading Audio Data! e=" + ex2.ToString() + "  " + ex2.Message);
				}
			}
			stream.Close();
			return array;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002788 File Offset: 0x00000988
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		// Token: 0x04000001 RID: 1
		private string _filePathRelativeToProjectDir = "";

		// Token: 0x04000002 RID: 2
		public int _prebeatLengthInSamplesEnteredManually;

		// Token: 0x04000003 RID: 3
		public int _postbeatLengthInSamplesEnteredManually;
	}
}
