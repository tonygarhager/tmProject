#define TRACE
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Sdl.FileTypeSupport.Framework
{
	public class FileJanitor : IDisposable
	{
		private string _filePath;

		private bool _deleteDirectoryIfEmpty;

		private bool _cancelDelete;

		private bool _deleted;

		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;
			}
		}

		public bool DeleteDirectoryIfEmpty
		{
			get
			{
				return _deleteDirectoryIfEmpty;
			}
			set
			{
				_deleteDirectoryIfEmpty = value;
			}
		}

		public bool CancelDelete
		{
			get
			{
				return _cancelDelete;
			}
			set
			{
				_cancelDelete = value;
			}
		}

		public bool Deleted => _deleted;

		public FileJanitor(string filePath)
		{
			_filePath = filePath;
		}

		~FileJanitor()
		{
			Dispose(disposing: false);
		}

		protected virtual void Dispose(bool disposing)
		{
			DeleteFile();
		}

		public virtual void DeleteFile()
		{
			if (!_cancelDelete && !_deleted)
			{
				try
				{
					if (File.Exists(_filePath))
					{
						File.SetAttributes(_filePath, File.GetAttributes(_filePath) & ~FileAttributes.ReadOnly);
						File.Delete(_filePath);
						_deleted = true;
					}
					if (_deleteDirectoryIfEmpty)
					{
						string directoryName = Path.GetDirectoryName(_filePath);
						if (Directory.Exists(directoryName) && Directory.GetFiles(directoryName).Length == 0)
						{
							Directory.Delete(directoryName, recursive: true);
							_deleted = true;
						}
					}
				}
				catch (Exception ex)
				{
					Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Failed to delete file '{0}': {1}", _filePath, ex.ToString()));
				}
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
