using System.Reflection;

namespace Sdl.Core.PluginFramework
{
	public class InvalidSdlAssemblyReference
	{
		public AssemblyName AssemblyReference
		{
			get;
			private set;
		}

		public SdlAssemblyReferenceError ValidationError
		{
			get;
			private set;
		}

		public InvalidSdlAssemblyReference(AssemblyName assemblyReference, SdlAssemblyReferenceError validationError)
		{
			AssemblyReference = assemblyReference;
			ValidationError = validationError;
		}
	}
}
