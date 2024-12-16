namespace Sdl.Enterprise2.MultiTerm.Client.Security
{
	public static class AesInfo
	{
		private static byte[] aesKey = new byte[32]
		{
			20,
			148,
			97,
			229,
			149,
			19,
			241,
			129,
			145,
			66,
			29,
			26,
			163,
			222,
			250,
			59,
			94,
			59,
			214,
			115,
			18,
			133,
			69,
			205,
			127,
			163,
			63,
			4,
			90,
			174,
			90,
			43
		};

		private static byte[] aesIV = new byte[16]
		{
			225,
			214,
			143,
			38,
			186,
			44,
			166,
			75,
			113,
			81,
			116,
			184,
			166,
			34,
			71,
			21
		};

		public static byte[] Key => aesKey;

		public static byte[] IV => aesIV;
	}
}
