using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	public class BilingualDocumentOutputProperties : IBilingualDocumentOutputProperties
	{
		private string _outputFilePath;

		private List<IDependencyFileProperties> _linkedDependencyFiles = new List<IDependencyFileProperties>();

		public string OutputFilePath
		{
			get
			{
				return _outputFilePath;
			}
			set
			{
				_outputFilePath = value;
			}
		}

		public IList<IDependencyFileProperties> LinkedDependencyFiles => _linkedDependencyFiles;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			BilingualDocumentOutputProperties bilingualDocumentOutputProperties = (BilingualDocumentOutputProperties)obj;
			if (_outputFilePath != bilingualDocumentOutputProperties._outputFilePath)
			{
				return false;
			}
			if (_linkedDependencyFiles.Count != bilingualDocumentOutputProperties._linkedDependencyFiles.Count)
			{
				return false;
			}
			for (int i = 0; i < _linkedDependencyFiles.Count; i++)
			{
				IDependencyFileProperties dependencyFileProperties = _linkedDependencyFiles[i];
				IDependencyFileProperties dependencyFileProperties2 = bilingualDocumentOutputProperties._linkedDependencyFiles[i];
				if (dependencyFileProperties == null != (dependencyFileProperties2 == null))
				{
					return false;
				}
				if (dependencyFileProperties != null && !dependencyFileProperties.Equals(dependencyFileProperties2))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = _linkedDependencyFiles.Count;
			foreach (IDependencyFileProperties linkedDependencyFile in _linkedDependencyFiles)
			{
				if (linkedDependencyFile != null)
				{
					num ^= linkedDependencyFile.GetHashCode();
				}
			}
			return num ^ ((_outputFilePath != null) ? _outputFilePath.GetHashCode() : 0);
		}
	}
}
