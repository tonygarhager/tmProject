using System;

namespace Sdl.Core.Processing.Alignment.Api
{
	public class SdlAlignPackageProgressEventArgs<T> : EventArgs
	{
		public string SdlAlignPackagePath
		{
			get;
			private set;
		}

		public byte Progress
		{
			get;
			private set;
		}

		public T Phase
		{
			get;
			private set;
		}

		public SdlAlignPackageProgressEventArgs()
		{
		}

		public SdlAlignPackageProgressEventArgs(T phase, byte progress)
			: this(phase, progress, new object[1]
			{
				string.Empty
			})
		{
		}

		public SdlAlignPackageProgressEventArgs(T phase, byte progress, params object[] info)
		{
			if (progress > 100)
			{
				progress = 100;
			}
			if (info != null)
			{
				try
				{
					SdlAlignPackagePath = info[0].ToString();
				}
				catch
				{
					throw new ArgumentNullException("sdlAlignPackagePath");
				}
			}
			Progress = progress;
			Phase = phase;
		}
	}
}
