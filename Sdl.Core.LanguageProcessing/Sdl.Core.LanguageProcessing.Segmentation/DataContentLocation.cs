using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class DataContentLocation
	{
		public Location Location
		{
			get;
			set;
		}

		public int Offset
		{
			get;
			set;
		}

		public DataContentLocation(Location location, int offset)
		{
			Location = location;
			Offset = offset;
		}

		public override string ToString()
		{
			return string.Format("DataContentLocation offset={1}, data={0}", Debug.ToString(Location), Offset);
		}
	}
}
