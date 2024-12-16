using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class SegmentSerializer
	{
		private enum SerializationVersion
		{
			Version1 = 1,
			Version1Compact
		}

		private static string ReadStringOrNull(BinaryReader reader)
		{
			if (reader.ReadBoolean())
			{
				return reader.ReadString();
			}
			return null;
		}

		private static void WriteStringOrNull(string s, BinaryWriter writer)
		{
			if (s == null)
			{
				writer.Write(value: false);
				return;
			}
			writer.Write(value: true);
			writer.Write(s);
		}

		public static string Save(Segment segment, List<byte> binaryData)
		{
			bool compactSerialization = true;
			string result = SaveInternal(segment, binaryData, ref compactSerialization);
			if (!compactSerialization)
			{
				result = SaveInternal(segment, binaryData, ref compactSerialization);
			}
			return result;
		}

		private static bool WriteIntAsSByte(BinaryWriter writer, int i)
		{
			if (i > 127 || i < -128)
			{
				return false;
			}
			writer.Write((sbyte)i);
			return true;
		}

		private static bool WriteUIntAsByte(BinaryWriter writer, uint i)
		{
			if (i > 255)
			{
				return false;
			}
			writer.Write((byte)i);
			return true;
		}

		private static bool WriteUnsignedInt(BinaryWriter writer, uint i, bool compactSerialization)
		{
			if (compactSerialization)
			{
				return WriteUIntAsByte(writer, i);
			}
			writer.Write(i);
			return true;
		}

		private static bool WriteInt(BinaryWriter writer, int i, bool compactSerialization)
		{
			if (compactSerialization)
			{
				return WriteIntAsSByte(writer, i);
			}
			writer.Write(i);
			return true;
		}

		private static string SaveInternal(Segment segment, List<byte> binaryData, ref bool compactSerialization)
		{
			binaryData.Clear();
			if (segment.Elements == null)
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = segment.Elements.Count((SegmentElement e) => e is Tag);
			using (MemoryStream memoryStream = (num3 > 0) ? new MemoryStream() : null)
			{
				SerializationVersion serializationVersion = (!compactSerialization) ? SerializationVersion.Version1 : SerializationVersion.Version1Compact;
				using (BinaryWriter binaryWriter = (num3 > 0) ? new BinaryWriter(memoryStream) : null)
				{
					if (binaryWriter != null)
					{
						binaryWriter.Write((byte)serializationVersion);
						if (!WriteInt(binaryWriter, num3, compactSerialization))
						{
							compactSerialization = false;
							return null;
						}
					}
					foreach (SegmentElement element in segment.Elements)
					{
						Text text = element as Text;
						if (text == null)
						{
							Tag tag = element as Tag;
							if (tag == null || binaryWriter == null)
							{
								throw new Exception("Unexpected Element type: " + element.GetType().Name);
							}
							int i = num - num2;
							num2 = num;
							if (!WriteUnsignedInt(binaryWriter, (uint)i, compactSerialization))
							{
								compactSerialization = false;
								return null;
							}
							binaryWriter.Write((byte)tag.Type);
							if (!WriteInt(binaryWriter, tag.Anchor, compactSerialization))
							{
								compactSerialization = false;
								return null;
							}
							if (tag.Type != TagType.End && !WriteInt(binaryWriter, tag.AlignmentAnchor, compactSerialization))
							{
								compactSerialization = false;
								return null;
							}
							WriteStringOrNull(tag.TagID, binaryWriter);
							if (tag.Type == TagType.TextPlaceholder || tag.Type == TagType.LockedContent)
							{
								WriteStringOrNull(tag.TextEquivalent, binaryWriter);
							}
							binaryWriter.Write(tag.CanHide);
						}
						else
						{
							num += text.Value.Length;
							stringBuilder.Append(text.Value);
						}
					}
					if (binaryWriter == null)
					{
						return stringBuilder.ToString();
					}
					binaryWriter.Close();
					memoryStream.Close();
					binaryData.AddRange(memoryStream.ToArray());
					return stringBuilder.ToString();
				}
			}
		}

		private static int ReadInt(BinaryReader reader, bool compactSerialization)
		{
			if (!compactSerialization)
			{
				return reader.ReadInt32();
			}
			return reader.ReadSByte();
		}

		private static uint ReadUnsignedInt(BinaryReader reader, bool compactSerialization)
		{
			if (!compactSerialization)
			{
				return reader.ReadUInt32();
			}
			return reader.ReadByte();
		}

		public static Segment Load(string text, byte[] binaryData, CultureInfo culture)
		{
			Segment segment = new Segment(culture);
			if (binaryData == null)
			{
				if (text == null)
				{
					return segment;
				}
				segment.Elements.Add(new Text(text));
				return segment;
			}
			uint num = 0u;
			using (MemoryStream input = new MemoryStream(binaryData))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					byte b = binaryReader.ReadByte();
					SerializationVersion serializationVersion = (SerializationVersion)b;
					bool compactSerialization = false;
					switch (serializationVersion)
					{
					case SerializationVersion.Version1Compact:
						compactSerialization = true;
						break;
					default:
						throw new Exception("Unknown segment serialization version: " + b.ToString());
					case SerializationVersion.Version1:
						break;
					}
					for (int num2 = ReadInt(binaryReader, compactSerialization); num2 > 0; num2--)
					{
						uint num3 = ReadUnsignedInt(binaryReader, compactSerialization);
						uint num4 = num + num3;
						byte num5 = binaryReader.ReadByte();
						int anchor = ReadInt(binaryReader, compactSerialization);
						int alignmentAnchor = 0;
						if (num5 != 2)
						{
							alignmentAnchor = ReadInt(binaryReader, compactSerialization);
						}
						string tagId = ReadStringOrNull(binaryReader);
						Tag tag = new Tag((TagType)num5, tagId, anchor);
						if (num5 != 2)
						{
							tag.AlignmentAnchor = alignmentAnchor;
						}
						if (tag.Type == TagType.TextPlaceholder || tag.Type == TagType.LockedContent)
						{
							tag.TextEquivalent = ReadStringOrNull(binaryReader);
						}
						tag.CanHide = binaryReader.ReadBoolean();
						if (text != null && text.Length >= num4 && num4 > num)
						{
							Text item = new Text(text.Substring((int)num, (int)(num4 - num)));
							segment.Elements.Add(item);
						}
						num = num4;
						segment.Elements.Add(tag);
					}
				}
			}
			if (text == null || num >= text.Length)
			{
				return segment;
			}
			segment.Elements.Add(new Text(text.Substring((int)num)));
			return segment;
		}
	}
}
