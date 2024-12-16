using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Diagnostics;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// This is a simple generic external preview application that 
	/// launches the associated or specified application for the preview file.
	/// Then when this object is disposed or the launched process exits the temp file is deleted.
	/// </summary>
	public class GenericExteralPreviewApplication : AbstractFileTypeDefinitionComponent, ISingleFilePreviewApplication, IAbstractPreviewApplication, IAbstractPreviewController, ISingleFilePreviewController, IDisposable
	{
		private string _applicationPath;

		private bool _monitorApplicationExit = true;

		private TempFileManager _previewFile;

		private Process _applicationProcess;

		private EventHandler _applicationProcessExitEvent;

		/// <summary>
		/// The path of the external app to preview the generated file (or null for default associated app)
		/// </summary>
		public string ApplicationPath
		{
			get
			{
				return _applicationPath;
			}
			set
			{
				_applicationPath = value;
			}
		}

		/// <summary>
		/// Indicates if the application should be monitored when exiting.
		/// This will perform some tidy up if set to true
		/// </summary>
		public bool MonitorApplicationExit
		{
			get
			{
				return _monitorApplicationExit;
			}
			set
			{
				_monitorApplicationExit = value;
			}
		}

		/// <summary>
		/// Default implementation is accessor for member field.
		/// </summary>
		public virtual TempFileManager PreviewFile
		{
			get
			{
				return _previewFile;
			}
			set
			{
				_previewFile = value;
			}
		}

		/// <summary>
		/// Raised when the launched application terminates.
		/// </summary>
		public event EventHandler<PreviewClosedEventArgs> PreviewClosed;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GenericExteralPreviewApplication()
		{
			_applicationProcessExitEvent = _applicationProcess_Exited;
		}

		/// <summary>
		/// Calls <see cref="M:Sdl.FileTypeSupport.Framework.PreviewControls.GenericExteralPreviewApplication.Dispose(System.Boolean)" />.
		/// </summary>
		~GenericExteralPreviewApplication()
		{
			Dispose(disposing: false);
		}

		/// <summary>
		/// Launch a process to view the file. If an application path has been
		/// set it is used to launch the application with the preview file
		/// as a parameter. If not the file is launched using the default
		/// application associated with it (if any).
		/// </summary>
		/// <param name="managedTempFile"></param>
		/// <returns></returns>
		protected virtual Process LaunchApplicationForFile(TempFileManager managedTempFile)
		{
			Process result = null;
			if (managedTempFile != null)
			{
				result = (string.IsNullOrEmpty(ApplicationPath) ? Process.Start(managedTempFile.FilePath) : Process.Start(ApplicationPath, managedTempFile.FilePath));
			}
			return result;
		}

		/// <summary>
		/// Raise the <see cref="E:Sdl.FileTypeSupport.Framework.PreviewControls.GenericExteralPreviewApplication.PreviewClosed" /> event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public virtual void OnPreviewClosed(object sender, PreviewClosedEventArgs args)
		{
			if (this.PreviewClosed != null)
			{
				this.PreviewClosed(sender, args);
			}
		}

		/// <summary>
		/// Display the file in the external application.
		/// </summary>
		public virtual void Launch()
		{
			if (_applicationProcess != null)
			{
				if (_monitorApplicationExit)
				{
					_applicationProcess.Exited -= _applicationProcessExitEvent;
				}
				_applicationProcess = null;
			}
			_applicationProcess = LaunchApplicationForFile(_previewFile);
			if (_applicationProcess != null && _monitorApplicationExit)
			{
				_applicationProcess.EnableRaisingEvents = true;
				_applicationProcess.Exited += _applicationProcessExitEvent;
			}
		}

		private void _applicationProcess_Exited(object sender, EventArgs e)
		{
			_applicationProcess.Exited -= _applicationProcessExitEvent;
			OnPreviewClosed(this, new PreviewClosedEventArgs(this));
			_applicationProcess = null;
		}

		/// <summary>
		/// Default implementation sends a close message to the process main window,
		/// if it is open.
		/// </summary>
		public virtual void NotifyCanClose()
		{
			if (_applicationProcess != null && !_applicationProcess.HasExited)
			{
				if (_monitorApplicationExit)
				{
					_applicationProcess.Exited -= _applicationProcessExitEvent;
				}
				try
				{
					bool flag = _applicationProcess.CloseMainWindow();
				}
				catch (Exception)
				{
				}
			}
			_applicationProcess = null;
		}

		/// <summary>
		/// Calls <see cref="M:Sdl.FileTypeSupport.Framework.PreviewControls.GenericExteralPreviewApplication.Dispose(System.Boolean)" />.
		/// </summary>
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose of resources used by this object. Implementation
		/// attempts to close the preview application and delete the temp file.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (_applicationProcess != null)
			{
				if (_monitorApplicationExit)
				{
					_applicationProcess.Exited -= _applicationProcessExitEvent;
				}
				if (!_applicationProcess.HasExited)
				{
					try
					{
						_applicationProcess.CloseMainWindow();
					}
					catch (Exception)
					{
					}
				}
				_applicationProcess = null;
			}
			if (_previewFile != null)
			{
				_previewFile.Dispose();
				_previewFile = null;
			}
		}
	}
}
