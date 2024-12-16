namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	public interface ISdlAlignPackageHandler
	{
		void Save(string sdlAlignPath);

		void Zip(string archiveName, params string[] files);

		void UnZip(string archiveName, string destination);
	}
}
