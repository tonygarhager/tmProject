using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class PermissionConverter
	{
		private static Dictionary<string, Permission> _index;

		private static Dictionary<Permission, string> _reverseIndex;

		private static Permission[] _individualPermissions;

		static PermissionConverter()
		{
			_individualPermissions = new Permission[15]
			{
				Permission.ReadTM,
				Permission.EditTM,
				Permission.CreateTM,
				Permission.DeleteTM,
				Permission.WriteTU,
				Permission.DeleteTU,
				Permission.ReadTU,
				Permission.ReadResource,
				Permission.WriteResource,
				Permission.DeleteResource,
				Permission.BatchEditTU,
				Permission.BatchDeleteTU,
				Permission.ImportTU,
				Permission.ExportTU,
				Permission.ReindexTU
			};
			_index = new Dictionary<string, Permission>();
			_reverseIndex = new Dictionary<Permission, string>();
			AddMapping("tm.view", Permission.ReadTM);
			AddMapping("tm.edit", Permission.EditTM);
			AddMapping("tm.delete", Permission.DeleteTM);
			AddMapping("tm.add", Permission.CreateTM);
			AddMapping("tm.writetu", Permission.WriteTU);
			AddMapping("tm.deletetu", Permission.DeleteTU);
			AddMapping("tm.readtu", Permission.ReadTU);
			AddMapping("tm.batchedittu", Permission.BatchEditTU);
			AddMapping("tm.batchdeletetu", Permission.BatchDeleteTU);
			AddMapping("tm.importtu", Permission.ImportTU);
			AddMapping("tm.exporttu", Permission.ExportTU);
			AddMapping("tm.reindextu", Permission.ReindexTU);
			AddMapping("tmlangresource.view", Permission.ReadResource);
			AddMapping("tmlangresource.edit", Permission.WriteResource);
			AddMapping("tmlangresource.delete", Permission.DeleteResource);
		}

		private static void AddMapping(string role, Permission permission)
		{
			_index.Add(role, permission);
			_reverseIndex.Add(permission, role);
		}

		public static Permission Convert(string role)
		{
			if (_index.TryGetValue(role, out Permission value))
			{
				return value;
			}
			return Permission.None;
		}

		public static string[] Convert(Permission flags)
		{
			List<string> list = new List<string>();
			Permission[] individualPermissions = _individualPermissions;
			foreach (Permission permission in individualPermissions)
			{
				if (Contains(permission, flags))
				{
					list.Add(_reverseIndex[permission]);
				}
			}
			return list.ToArray();
		}

		private static bool Contains(Permission permission, Permission flags)
		{
			return (flags & permission) != 0;
		}

		/// <summary>
		/// Returns the file access mode corresponding to the specified combi-permission, or null if no mapping exists.
		/// </summary>
		/// <param name="combiPermission"></param>
		/// <returns></returns>
		public static TranslationMemoryFileAccessMode? GetFileAccessMode(Permission combiPermission)
		{
			switch (combiPermission)
			{
			case Permission.Administrator:
				return TranslationMemoryFileAccessMode.Administrator;
			case Permission.Maintenance:
				return TranslationMemoryFileAccessMode.Maintenance;
			case Permission.ReadWrite:
				return TranslationMemoryFileAccessMode.ReadWrite;
			case Permission.ReadOnly:
				return TranslationMemoryFileAccessMode.ReadOnly;
			default:
				return null;
			}
		}

		/// <summary>
		/// Returns the combi-permission corresponding to the specified access mode
		/// </summary>
		/// <param name="accessMode"></param>
		/// <returns></returns>
		public static Permission GetPermissions(TranslationMemoryFileAccessMode accessMode)
		{
			switch (accessMode)
			{
			case TranslationMemoryFileAccessMode.Administrator:
				return Permission.Administrator;
			case TranslationMemoryFileAccessMode.Maintenance:
				return Permission.Maintenance;
			case TranslationMemoryFileAccessMode.ReadWrite:
				return Permission.ReadWrite;
			case TranslationMemoryFileAccessMode.ReadOnly:
				return Permission.ReadOnly;
			default:
				return Permission.ReadOnly;
			}
		}

		/// <summary>
		/// Returns a Permission bitwise flag combination which represents all permissions that are explicitly defined for an access mode,
		/// i.e. does not include permissions that are "inherited" from lower access modes.
		/// </summary>
		/// <param name="accessMode"></param>
		/// <returns></returns>
		public static Permission GetExplicitPermissionsInAccessMode(TranslationMemoryFileAccessMode accessMode)
		{
			switch (accessMode)
			{
			case TranslationMemoryFileAccessMode.Administrator:
				return Subtract(Permission.Administrator, Permission.Maintenance);
			case TranslationMemoryFileAccessMode.Maintenance:
				return Subtract(Permission.Maintenance, Permission.ReadWrite);
			case TranslationMemoryFileAccessMode.ReadWrite:
				return Subtract(Permission.ReadWrite, Permission.ReadOnly);
			case TranslationMemoryFileAccessMode.ReadOnly:
				return Permission.ReadOnly;
			default:
				return Permission.ReadOnly;
			}
		}

		/// <summary>
		/// Return permission set  left - right
		/// Assumes "right" is a subset of the permissions in "left"
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private static Permission Subtract(Permission left, Permission right)
		{
			return left ^ right;
		}
	}
}
