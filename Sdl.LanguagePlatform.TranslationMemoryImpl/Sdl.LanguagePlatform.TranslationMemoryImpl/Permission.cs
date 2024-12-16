using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[Flags]
	public enum Permission
	{
		None = 0x0,
		ReadTU = 0x1,
		WriteTU = 0x2,
		DeleteTU = 0x4,
		ReadTM = 0x8,
		EditTM = 0x10,
		CreateTM = 0x20,
		DeleteTM = 0x40,
		ReadResource = 0x200,
		WriteResource = 0x400,
		DeleteResource = 0x800,
		CreateSchema = 0x1000,
		DropSchema = 0x2000,
		BatchEditTU = 0x4000,
		BatchDeleteTU = 0x8000,
		ImportTU = 0x10000,
		ExportTU = 0x20000,
		ReindexTU = 0x40000,
		ReadOnly = 0x209,
		ReadWrite = 0x20F,
		Maintenance = 0xC20F,
		Administrator = 0xFFFFFFF
	}
}
