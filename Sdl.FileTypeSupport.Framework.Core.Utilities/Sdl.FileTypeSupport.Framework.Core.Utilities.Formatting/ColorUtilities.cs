using System.Collections.Generic;
using System.Drawing;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting
{
	public static class ColorUtilities
	{
		private static readonly Dictionary<string, Color> conversionTable;

		public static Dictionary<string, Color> KnownColors => conversionTable;

		static ColorUtilities()
		{
			conversionTable = new Dictionary<string, Color>();
			conversionTable["aliceblue"] = Color.FromArgb(255, 240, 248, 255);
			conversionTable["antiquewhite"] = Color.FromArgb(255, 250, 235, 215);
			conversionTable["aqua"] = Color.FromArgb(255, 0, 255, 255);
			conversionTable["aquamarine"] = Color.FromArgb(255, 127, 255, 212);
			conversionTable["azure"] = Color.FromArgb(255, 240, 255, 255);
			conversionTable["beige"] = Color.FromArgb(255, 245, 245, 220);
			conversionTable["bisque"] = Color.FromArgb(255, 255, 228, 196);
			conversionTable["black"] = Color.FromArgb(255, 0, 0, 0);
			conversionTable["blanchedalmond"] = Color.FromArgb(255, 255, 235, 205);
			conversionTable["blue"] = Color.FromArgb(255, 0, 0, 255);
			conversionTable["blueviolet"] = Color.FromArgb(255, 138, 43, 226);
			conversionTable["brown"] = Color.FromArgb(255, 165, 42, 42);
			conversionTable["burlywood"] = Color.FromArgb(255, 222, 184, 135);
			conversionTable["cadetblue"] = Color.FromArgb(255, 95, 158, 160);
			conversionTable["chartreuse"] = Color.FromArgb(255, 127, 255, 0);
			conversionTable["chocolate"] = Color.FromArgb(255, 210, 105, 30);
			conversionTable["coral"] = Color.FromArgb(255, 255, 127, 80);
			conversionTable["cornflowerblue"] = Color.FromArgb(255, 100, 149, 237);
			conversionTable["cornsilk"] = Color.FromArgb(255, 255, 248, 220);
			conversionTable["crimson"] = Color.FromArgb(255, 220, 20, 60);
			conversionTable["cyan"] = Color.FromArgb(255, 0, 255, 255);
			conversionTable["darkblue"] = Color.FromArgb(255, 0, 0, 139);
			conversionTable["darkcyan"] = Color.FromArgb(255, 0, 139, 139);
			conversionTable["darkgoldenrod"] = Color.FromArgb(255, 184, 134, 11);
			conversionTable["darkgray"] = Color.FromArgb(255, 169, 169, 169);
			conversionTable["darkgreen"] = Color.FromArgb(255, 0, 100, 0);
			conversionTable["darkkhaki"] = Color.FromArgb(255, 189, 183, 107);
			conversionTable["darkmagenta"] = Color.FromArgb(255, 139, 0, 139);
			conversionTable["darkolivegreen"] = Color.FromArgb(255, 85, 107, 47);
			conversionTable["darkorange"] = Color.FromArgb(255, 255, 140, 0);
			conversionTable["darkorchid"] = Color.FromArgb(255, 153, 50, 204);
			conversionTable["darkred"] = Color.FromArgb(255, 139, 0, 0);
			conversionTable["darksalmon"] = Color.FromArgb(255, 233, 150, 122);
			conversionTable["darkseagreen"] = Color.FromArgb(255, 143, 188, 139);
			conversionTable["darkslateblue"] = Color.FromArgb(255, 72, 61, 139);
			conversionTable["darkslategray"] = Color.FromArgb(255, 47, 79, 79);
			conversionTable["darkturquoise"] = Color.FromArgb(255, 0, 206, 209);
			conversionTable["darkviolet"] = Color.FromArgb(255, 148, 0, 211);
			conversionTable["deeppink"] = Color.FromArgb(255, 255, 20, 147);
			conversionTable["deepskyblue"] = Color.FromArgb(255, 0, 191, 255);
			conversionTable["dimgray"] = Color.FromArgb(255, 105, 105, 105);
			conversionTable["dodgerblue"] = Color.FromArgb(255, 30, 144, 255);
			conversionTable["firebrick"] = Color.FromArgb(255, 178, 34, 34);
			conversionTable["floralwhite"] = Color.FromArgb(255, 255, 250, 240);
			conversionTable["forestgreen"] = Color.FromArgb(255, 34, 139, 34);
			conversionTable["fuchsia"] = Color.FromArgb(255, 255, 0, 255);
			conversionTable["gainsboro"] = Color.FromArgb(255, 220, 220, 220);
			conversionTable["ghostwhite"] = Color.FromArgb(255, 248, 248, 255);
			conversionTable["gold"] = Color.FromArgb(255, 255, 215, 0);
			conversionTable["goldenrod"] = Color.FromArgb(255, 218, 165, 32);
			conversionTable["gray"] = Color.FromArgb(255, 128, 128, 128);
			conversionTable["green"] = Color.FromArgb(255, 0, 128, 0);
			conversionTable["greenyellow"] = Color.FromArgb(255, 173, 255, 47);
			conversionTable["honeydew"] = Color.FromArgb(255, 240, 255, 240);
			conversionTable["hotpink"] = Color.FromArgb(255, 255, 105, 180);
			conversionTable["indianred"] = Color.FromArgb(255, 205, 92, 92);
			conversionTable["indigo"] = Color.FromArgb(255, 75, 0, 130);
			conversionTable["ivory"] = Color.FromArgb(255, 255, 255, 240);
			conversionTable["khaki"] = Color.FromArgb(255, 240, 230, 140);
			conversionTable["lavender"] = Color.FromArgb(255, 230, 230, 250);
			conversionTable["lavenderblush"] = Color.FromArgb(255, 255, 240, 245);
			conversionTable["lawngreen"] = Color.FromArgb(255, 124, 252, 0);
			conversionTable["lemonchiffon"] = Color.FromArgb(255, 255, 250, 205);
			conversionTable["lightblue"] = Color.FromArgb(255, 173, 216, 230);
			conversionTable["lightcoral"] = Color.FromArgb(255, 240, 128, 128);
			conversionTable["lightcyan"] = Color.FromArgb(255, 224, 255, 255);
			conversionTable["lightgoldenrodyellow"] = Color.FromArgb(255, 250, 250, 210);
			conversionTable["lightgray"] = Color.FromArgb(255, 211, 211, 211);
			conversionTable["lightgreen"] = Color.FromArgb(255, 144, 238, 144);
			conversionTable["lightpink"] = Color.FromArgb(255, 255, 182, 193);
			conversionTable["lightsalmon"] = Color.FromArgb(255, 255, 160, 122);
			conversionTable["lightseagreen"] = Color.FromArgb(255, 32, 178, 170);
			conversionTable["lightskyblue"] = Color.FromArgb(255, 135, 206, 250);
			conversionTable["lightslategray"] = Color.FromArgb(255, 119, 136, 153);
			conversionTable["lightsteelblue"] = Color.FromArgb(255, 176, 196, 222);
			conversionTable["lightyellow"] = Color.FromArgb(255, 255, 255, 224);
			conversionTable["lime"] = Color.FromArgb(255, 0, 255, 0);
			conversionTable["limegreen"] = Color.FromArgb(255, 50, 205, 50);
			conversionTable["linen"] = Color.FromArgb(255, 250, 240, 230);
			conversionTable["magenta"] = Color.FromArgb(255, 255, 0, 255);
			conversionTable["maroon"] = Color.FromArgb(255, 128, 0, 0);
			conversionTable["mediumaquamarine"] = Color.FromArgb(255, 102, 205, 170);
			conversionTable["mediumblue"] = Color.FromArgb(255, 0, 0, 205);
			conversionTable["mediumorchid"] = Color.FromArgb(255, 186, 85, 211);
			conversionTable["mediumpurple"] = Color.FromArgb(255, 147, 112, 219);
			conversionTable["mediumseagreen"] = Color.FromArgb(255, 60, 179, 113);
			conversionTable["mediumslateblue"] = Color.FromArgb(255, 123, 104, 238);
			conversionTable["mediumspringgreen"] = Color.FromArgb(255, 0, 250, 154);
			conversionTable["mediumturquoise"] = Color.FromArgb(255, 72, 209, 204);
			conversionTable["mediumvioletred"] = Color.FromArgb(255, 199, 21, 133);
			conversionTable["midnightblue"] = Color.FromArgb(255, 25, 25, 112);
			conversionTable["mintcream"] = Color.FromArgb(255, 245, 255, 250);
			conversionTable["mistyrose"] = Color.FromArgb(255, 255, 228, 225);
			conversionTable["moccasin"] = Color.FromArgb(255, 255, 228, 181);
			conversionTable["navajowhite"] = Color.FromArgb(255, 255, 222, 173);
			conversionTable["navy"] = Color.FromArgb(255, 0, 0, 128);
			conversionTable["oldlace"] = Color.FromArgb(255, 253, 245, 230);
			conversionTable["olive"] = Color.FromArgb(255, 128, 128, 0);
			conversionTable["olivedrab"] = Color.FromArgb(255, 107, 142, 35);
			conversionTable["orange"] = Color.FromArgb(255, 255, 165, 0);
			conversionTable["orangered"] = Color.FromArgb(255, 255, 69, 0);
			conversionTable["orchid"] = Color.FromArgb(255, 218, 112, 214);
			conversionTable["palegoldenrod"] = Color.FromArgb(255, 238, 232, 170);
			conversionTable["palegreen"] = Color.FromArgb(255, 152, 251, 152);
			conversionTable["paleturquoise"] = Color.FromArgb(255, 175, 238, 238);
			conversionTable["palevioletred"] = Color.FromArgb(255, 219, 112, 147);
			conversionTable["papayawhip"] = Color.FromArgb(255, 255, 239, 213);
			conversionTable["peachpuff"] = Color.FromArgb(255, 255, 218, 185);
			conversionTable["peru"] = Color.FromArgb(255, 205, 133, 63);
			conversionTable["pink"] = Color.FromArgb(255, 255, 192, 203);
			conversionTable["plum"] = Color.FromArgb(255, 221, 160, 221);
			conversionTable["powderblue"] = Color.FromArgb(255, 176, 224, 230);
			conversionTable["purple"] = Color.FromArgb(255, 128, 0, 128);
			conversionTable["red"] = Color.FromArgb(255, 255, 0, 0);
			conversionTable["rosybrown"] = Color.FromArgb(255, 188, 143, 143);
			conversionTable["royalblue"] = Color.FromArgb(255, 65, 105, 225);
			conversionTable["saddlebrown"] = Color.FromArgb(255, 139, 69, 19);
			conversionTable["salmon"] = Color.FromArgb(255, 250, 128, 114);
			conversionTable["sandybrown"] = Color.FromArgb(255, 244, 164, 96);
			conversionTable["seagreen"] = Color.FromArgb(255, 46, 139, 87);
			conversionTable["seashell"] = Color.FromArgb(255, 255, 245, 238);
			conversionTable["sienna"] = Color.FromArgb(255, 160, 82, 45);
			conversionTable["silver"] = Color.FromArgb(255, 192, 192, 192);
			conversionTable["skyblue"] = Color.FromArgb(255, 135, 206, 235);
			conversionTable["slateblue"] = Color.FromArgb(255, 106, 90, 205);
			conversionTable["slategray"] = Color.FromArgb(255, 112, 128, 144);
			conversionTable["snow"] = Color.FromArgb(255, 255, 250, 250);
			conversionTable["springgreen"] = Color.FromArgb(255, 0, 255, 127);
			conversionTable["steelblue"] = Color.FromArgb(255, 70, 130, 180);
			conversionTable["tan"] = Color.FromArgb(255, 210, 180, 140);
			conversionTable["teal"] = Color.FromArgb(255, 0, 128, 128);
			conversionTable["thistle"] = Color.FromArgb(255, 216, 191, 216);
			conversionTable["tomato"] = Color.FromArgb(255, 255, 99, 71);
			conversionTable["turquoise"] = Color.FromArgb(255, 64, 224, 208);
			conversionTable["violet"] = Color.FromArgb(255, 238, 130, 238);
			conversionTable["wheat"] = Color.FromArgb(255, 245, 222, 179);
			conversionTable["white"] = Color.FromArgb(255, 255, 255, 255);
			conversionTable["whitesmoke"] = Color.FromArgb(255, 245, 245, 245);
			conversionTable["yellow"] = Color.FromArgb(255, 255, 255, 0);
			conversionTable["yellowgreen"] = Color.FromArgb(255, 154, 205, 50);
		}

		public static Color FromName(string colorName)
		{
			if (conversionTable.ContainsKey(colorName.ToLower()))
			{
				return conversionTable[colorName.ToLower()];
			}
			return Color.FromArgb(255, 0, 0, 0);
		}

		public static bool IsKnownName(string colorName)
		{
			return conversionTable.ContainsKey(colorName.ToLower());
		}

		public static string GetLocalizedColorName(string defaultColorName)
		{
			return ColorUtilitesResources.ResourceManager.GetString(defaultColorName);
		}
	}
}
