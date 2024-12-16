using System;
using System.Diagnostics;
using System.IO;

namespace Sdl.FileTypeSupport.Framework
{
	public class TempFileManager : IDisposable
	{
		private static readonly string AutoDeleteLockName = "AutoDeleteLocked{0}.lock";

		private static readonly string TempAutoDeleteFolder = Path.Combine(Path.GetTempPath(), "SDLTempFileManager");

		private static TempFileManager _autoDeleteLockFile = null;

		private FileJanitor _janitoredFile;

		private FileStream _lockedFileStream;

		private Process _fileProcess;

		private bool _readOnly;

		private bool disposed;

		public string FilePath
		{
			get
			{
				if (_janitoredFile != null)
				{
					return _janitoredFile.FilePath;
				}
				return null;
			}
		}

		public bool DeleteDirectoryIfEmpty
		{
			get
			{
				if (_janitoredFile != null)
				{
					return _janitoredFile.DeleteDirectoryIfEmpty;
				}
				return false;
			}
			set
			{
				if (_janitoredFile != null)
				{
					_janitoredFile.DeleteDirectoryIfEmpty = value;
				}
			}
		}

		public bool Locked
		{
			get
			{
				return _lockedFileStream != null;
			}
			set
			{
				if (value)
				{
					if (_lockedFileStream != null)
					{
						_lockedFileStream.Close();
						_lockedFileStream.Dispose();
						_lockedFileStream = null;
					}
					if (_janitoredFile != null && !string.IsNullOrEmpty(_janitoredFile.FilePath))
					{
						bool readOnly = ReadOnly;
						ReadOnly = false;
						string directoryName = Path.GetDirectoryName(_janitoredFile.FilePath);
						if (!Directory.Exists(directoryName))
						{
							Directory.CreateDirectory(directoryName);
						}
						try
						{
							_lockedFileStream = File.Open(_janitoredFile.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
						}
						catch (Exception)
						{
						}
						try
						{
							if (readOnly)
							{
								ReadOnly = true;
							}
						}
						catch (Exception)
						{
						}
					}
				}
				else
				{
					if (_lockedFileStream != null)
					{
						_lockedFileStream.Close();
						_lockedFileStream.Dispose();
						_lockedFileStream = null;
					}
					if (_janitoredFile != null && !string.IsNullOrEmpty(_janitoredFile.FilePath))
					{
						CreateAutoDeleteLockFile();
					}
				}
			}
		}

		public bool ReadOnly
		{
			get
			{
				if (_janitoredFile != null && File.Exists(_janitoredFile.FilePath))
				{
					_readOnly = ((File.GetAttributes(_janitoredFile.FilePath) & FileAttributes.ReadOnly) != 0);
				}
				return _readOnly;
			}
			set
			{
				_readOnly = value;
				if (_janitoredFile != null && File.Exists(_janitoredFile.FilePath))
				{
					try
					{
						if (value)
						{
							File.SetAttributes(_janitoredFile.FilePath, File.GetAttributes(_janitoredFile.FilePath) | FileAttributes.ReadOnly);
						}
						else
						{
							File.SetAttributes(_janitoredFile.FilePath, File.GetAttributes(_janitoredFile.FilePath) & ~FileAttributes.ReadOnly);
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}

		public FileStream LockedFileStream
		{
			get
			{
				return _lockedFileStream;
			}
			set
			{
				if (_lockedFileStream != value)
				{
					if (_lockedFileStream != null)
					{
						_lockedFileStream.Close();
						_lockedFileStream.Dispose();
					}
					_lockedFileStream = value;
				}
			}
		}

		public Process FileProcess
		{
			get
			{
				return _fileProcess;
			}
			set
			{
				if (_fileProcess != null)
				{
					_fileProcess.Exited -= _fileProcess_Exited;
				}
				_fileProcess = value;
				if (_fileProcess != null)
				{
					_fileProcess.EnableRaisingEvents = true;
					_fileProcess.Exited += _fileProcess_Exited;
				}
			}
		}

		public bool CancelDelete
		{
			get
			{
				if (_janitoredFile != null)
				{
					return _janitoredFile.CancelDelete;
				}
				return false;
			}
			set
			{
				if (_janitoredFile != null)
				{
					_janitoredFile.CancelDelete = value;
				}
			}
		}

		public bool Deleted
		{
			get
			{
				if (_janitoredFile != null)
				{
					return _janitoredFile.Deleted;
				}
				return true;
			}
		}

		public TempFileManager()
			: this(null, lockFile: false)
		{
		}

		public TempFileManager(string filePath)
			: this(filePath, lockFile: false)
		{
		}

		public TempFileManager(string filePath, bool lockFile)
			: this(filePath, lockFile, autoDeleteLockFile: false)
		{
		}

		private TempFileManager(string filePath, bool lockFile, bool autoDeleteLockFile)
		{
			string directoryName = Path.GetDirectoryName(filePath);
			string fileName = Path.GetFileName(filePath);
			string text = null;
			bool deleteDirectoryIfEmpty = true;
			directoryName = ((!string.IsNullOrEmpty(directoryName)) ? Path.GetFileName(directoryName) : "");
			if (!Directory.Exists(TempAutoDeleteFolder))
			{
				Directory.CreateDirectory(TempAutoDeleteFolder);
			}
			if (string.IsNullOrEmpty(directoryName) && !string.IsNullOrEmpty(fileName) && !autoDeleteLockFile)
			{
				do
				{
					directoryName = Path.Combine(TempAutoDeleteFolder, Path.GetRandomFileName());
				}
				while (Directory.Exists(directoryName));
			}
			else
			{
				if (string.IsNullOrEmpty(directoryName))
				{
					deleteDirectoryIfEmpty = false;
				}
				directoryName = Path.Combine(TempAutoDeleteFolder, directoryName);
			}
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			if (string.IsNullOrEmpty(fileName))
			{
				do
				{
					fileName = Path.GetRandomFileName();
					text = Path.Combine(directoryName, fileName);
				}
				while (File.Exists(text));
			}
			else
			{
				text = Path.Combine(directoryName, fileName);
			}
			_janitoredFile = new FileJanitor(text);
			_janitoredFile.DeleteDirectoryIfEmpty = deleteDirectoryIfEmpty;
			Locked = lockFile;
		}

		~TempFileManager()
		{
			Dispose(disposing: false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				DeleteManagedFile(disposing);
				if (_janitoredFile.Deleted || _janitoredFile.CancelDelete)
				{
					disposed = true;
				}
			}
		}

		private void _fileProcess_Exited(object sender, EventArgs e)
		{
			DeleteManagedFile(disposing: false);
		}

		private void DeleteManagedFile(bool disposing)
		{
			if (_lockedFileStream != null)
			{
				_lockedFileStream.Close();
				if (disposing)
				{
					_lockedFileStream.Dispose();
				}
			}
			if (_janitoredFile != null)
			{
				_janitoredFile.DeleteFile();
				if (disposing)
				{
					_janitoredFile.Dispose();
				}
			}
		}

		public static void TakeOverManagedFile(ref TempFileManager managedFile, TempFileManager value)
		{
			if (managedFile == value)
			{
				return;
			}
			if (managedFile != null && value != null && managedFile.FilePath == value.FilePath)
			{
				if (managedFile.CancelDelete && !value.CancelDelete)
				{
					managedFile.Dispose();
					managedFile = value;
				}
				else if (!managedFile.CancelDelete && !value.CancelDelete)
				{
					managedFile.CancelDelete = true;
					managedFile.Dispose();
					managedFile = value;
				}
				else if (!managedFile.CancelDelete)
				{
					_ = value.CancelDelete;
				}
			}
			else
			{
				if (managedFile != null)
				{
					managedFile.Dispose();
				}
				managedFile = value;
			}
		}

		private static void CreateAutoDeleteLockFile()
		{
			if (_autoDeleteLockFile != null)
			{
				return;
			}
			int num = 1;
			do
			{
				_autoDeleteLockFile = new TempFileManager(string.Format(AutoDeleteLockName, num++), lockFile: true, autoDeleteLockFile: true);
				if (!_autoDeleteLockFile.Locked)
				{
					_autoDeleteLockFile.Dispose();
					_autoDeleteLockFile = null;
				}
			}
			while (_autoDeleteLockFile == null && num < 100);
		}

		public static void AutoDeleteNonLockedFiles()
		{
			if (_autoDeleteLockFile != null || !Directory.Exists(TempAutoDeleteFolder) || !Directory.Exists(TempAutoDeleteFolder))
			{
				return;
			}
			string[] files = Directory.GetFiles(TempAutoDeleteFolder);
			string[] array = files;
			foreach (string path in array)
			{
				bool flag = false;
				string[] files2 = Directory.GetFiles(TempAutoDeleteFolder, string.Format(AutoDeleteLockName, "*"));
				string[] array2 = files2;
				foreach (string path2 in array2)
				{
					try
					{
						File.Delete(path2);
					}
					catch (Exception)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				try
				{
					string path3 = Path.Combine(TempAutoDeleteFolder, path);
					File.SetAttributes(path3, File.GetAttributes(path3) & ~FileAttributes.ReadOnly);
					File.Delete(path3);
				}
				catch (Exception)
				{
				}
			}
			string[] directories = Directory.GetDirectories(TempAutoDeleteFolder);
			string[] array3 = directories;
			foreach (string path4 in array3)
			{
				bool flag2 = false;
				string[] files3 = Directory.GetFiles(TempAutoDeleteFolder, string.Format(AutoDeleteLockName, "*"));
				string[] array4 = files3;
				foreach (string path5 in array4)
				{
					try
					{
						File.Delete(path5);
					}
					catch (Exception)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					try
					{
						Directory.Delete(Path.Combine(TempAutoDeleteFolder, path4), recursive: true);
					}
					catch (Exception)
					{
					}
					continue;
				}
				break;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
