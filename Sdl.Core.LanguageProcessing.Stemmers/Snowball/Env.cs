using System.Text;

namespace Snowball
{
	public class Env
	{
		protected StringBuilder current;

		protected int cursor;

		protected int limit;

		protected int limit_backward;

		protected int bra;

		protected int ket;

		protected Env()
		{
		}

		public Env(Env other)
		{
			copy_from(other);
		}

		protected void copy_from(Env other)
		{
			current = other.current;
			cursor = other.cursor;
			limit = other.limit;
			limit_backward = other.limit_backward;
			bra = other.bra;
			ket = other.ket;
		}
	}
}
