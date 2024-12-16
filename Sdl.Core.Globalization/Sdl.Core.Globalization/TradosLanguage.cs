using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Globalization
{
	internal class TradosLanguage
	{
		public enum et_Primary
		{
			ec_Neutral = 0,
			ec_Arabic = 1,
			ec_Bulgarian = 2,
			ec_Catalan = 3,
			ec_Chinese = 4,
			ec_Czech = 5,
			ec_Danish = 6,
			ec_German = 7,
			ec_Greek = 8,
			ec_English = 9,
			ec_Spanish = 10,
			ec_Finnish = 11,
			ec_French = 12,
			ec_Hebrew = 13,
			ec_Hungarian = 14,
			ec_Icelandic = 0xF,
			ec_Italian = 0x10,
			ec_Japanese = 17,
			ec_Korean = 18,
			ec_Dutch = 19,
			ec_Norwegian = 20,
			ec_Polish = 21,
			ec_Portuguese = 22,
			ec_Romansh = 23,
			ec_Romanian = 24,
			ec_Russian = 25,
			ec_Serbian = 26,
			ec_Slovak = 27,
			ec_Albanian = 28,
			ec_Swedish = 29,
			ec_Thai = 30,
			ec_Turkish = 0x1F,
			ec_Urdu = 0x20,
			ec_Indonesian = 33,
			ec_Ukrainian = 34,
			ec_Byelorussian = 35,
			ec_Slovenian = 36,
			ec_Estonian = 37,
			ec_Latvian = 38,
			ec_Lithuanian = 39,
			ec_Maori = 40,
			ec_Persian = 41,
			ec_Vietnamese = 42,
			ec_Armenian = 43,
			ec_Azerbaijani = 44,
			ec_Basque = 45,
			ec_Sorbian = 46,
			ec_Macedonian = 47,
			ec_Tsonga = 48,
			ec_isiXhosa = 49,
			ec_isiZulu = 50,
			ec_Afrikaans = 51,
			ec_Faroese = 52,
			ec_Maltese = 53,
			ec_Irish = 54,
			ec_Malay = 55,
			ec_Kazakh = 56,
			ec_Kiswahili = 57,
			ec_Filipino = 58,
			ec_Kyrgyz = 59,
			ec_Uzbek = 60,
			ec_Tatar = 61,
			ec_Mongolian = 62,
			ec_Galician = 0x3F,
			ec_Sotho = 0x40,
			ec_Sami = 65,
			ec_Georgian = 66,
			ec_Hindi = 67,
			ec_Bangla = 68,
			ec_Punjabi = 69,
			ec_Gujarati = 70,
			ec_Odia = 71,
			ec_Tamil = 72,
			ec_Telugu = 73,
			ec_Kannada = 74,
			ec_Malayalam = 75,
			ec_Assamese = 76,
			ec_Marathi = 77,
			ec_Sanskrit = 78,
			ec_Konkani = 79,
			ec_Manipuri = 80,
			ec_Sindhi = 81,
			ec_Kashmiri = 82,
			ec_Nepali = 83,
			ec_Divehi = 84,
			ec_Welsh = 85,
			ec_Syriac = 86,
			ec_SesothoSaLeboa = 87,
			ec_Quechua = 88,
			ec_Setswana = 89,
			ec_Greenlandic = 90,
			ec_Afar = 91,
			ec_Aghem = 92,
			ec_Akan = 93,
			ec_Alsatian = 94,
			ec_Amharic = 95,
			ec_Asturian = 96,
			ec_Asu = 97,
			ec_Bafia = 98,
			ec_Bambara = 99,
			ec_Basaa = 100,
			ec_Bashkir = 101,
			ec_Bena = 102,
			ec_Blin = 103,
			ec_Bodo = 104,
			ec_Breton = 105,
			ec_Burmese = 106,
			ec_CentralKurdish = 107,
			ec_Cherokee = 108,
			ec_Chiga = 109,
			ec_Colognian = 110,
			ec_Cornish = 111,
			ec_Corsican = 112,
			ec_Dari = 113,
			ec_Duala = 114,
			ec_Dzongkha = 115,
			ec_Embu = 116,
			ec_Esperanto = 117,
			ec_Ewe = 118,
			ec_Ewondo = 119,
			ec_Frisian = 120,
			ec_Friulian = 121,
			ec_Fulah = 122,
			ec_Ganda = 123,
			ec_Guarani = 124,
			ec_Gusii = 125,
			ec_Hausa = 126,
			ec_Hawaiian = 0x7F,
			ec_Igbo = 0x80,
			ec_Interlingua = 129,
			ec_Inuktitut = 130,
			ec_Javanese = 131,
			ec_JolaFonyi = 132,
			ec_Kiche = 133,
			ec_Kabuverdianu = 134,
			ec_Kabyle = 135,
			ec_Kako = 136,
			ec_Kalenjin = 137,
			ec_Kamba = 138,
			ec_Khmer = 139,
			ec_Kikuyu = 140,
			ec_Kinyarwanda = 141,
			ec_KoyraChiini = 142,
			ec_KoyraboroSenni = 143,
			ec_Kwasio = 144,
			ec_Lakota = 145,
			ec_Langi = 146,
			ec_Lao = 147,
			ec_Lingala = 148,
			ec_LubaKatanga = 149,
			ec_Luo = 150,
			ec_Luxembourgish = 151,
			ec_Luyia = 152,
			ec_Machame = 153,
			ec_MakhuwaMeetto = 154,
			ec_Makonde = 155,
			ec_Malagasy = 156,
			ec_Manx = 157,
			ec_Mapudungun = 158,
			ec_Masai = 159,
			ec_Meru = 160,
			ec_Meta = 161,
			ec_Mohawk = 162,
			ec_Morisyen = 163,
			ec_Mundang = 164,
			ec_Nko = 165,
			ec_Nama = 166,
			ec_Ngiemboon = 167,
			ec_Ngomba = 168,
			ec_NorthNdebele = 169,
			ec_Nuer = 170,
			ec_Nyankole = 171,
			ec_Occitan = 172,
			ec_Oromo = 173,
			ec_Ossetian = 174,
			ec_Pashto = 175,
			ec_Rombo = 176,
			ec_Rundi = 177,
			ec_Rwa = 178,
			ec_Saho = 179,
			ec_Sakha = 180,
			ec_Samburu = 181,
			ec_Sango = 182,
			ec_Sangu = 183,
			ec_ScottishGaelic = 184,
			ec_Sena = 185,
			ec_Shambala = 186,
			ec_Shona = 187,
			ec_Sinhala = 188,
			ec_Soga = 189,
			ec_Somali = 190,
			ec_SouthNdebele = 191,
			ec_Moroccan = 192,
			ec_Swati = 193,
			ec_Tachelhit = 194,
			ec_Taita = 195,
			ec_Tajik = 196,
			ec_Tamazight = 197,
			ec_Tasawaq = 198,
			ec_Teso = 199,
			ec_Tibetan = 200,
			ec_Tigre = 201,
			ec_Tigrinya = 202,
			ec_Tongan = 203,
			ec_Turkmen = 204,
			ec_Uyghur = 205,
			ec_Vai = 206,
			ec_Venda = 207,
			ec_Volapuk = 208,
			ec_Vunjo = 209,
			ec_Walser = 210,
			ec_Wolaytta = 211,
			ec_Wolof = 212,
			ec_Yangben = 213,
			ec_Yi = 214,
			ec_Yiddish = 215,
			ec_Yoruba = 216,
			ec_Zarma = 217,
			ec_AmSignLng = 218,
			ec_Aymara = 219,
			ec_Bikolano = 220,
			ec_Bislama = 221,
			ec_Cakchiquel = 222,
			ec_Cebuano = 223,
			ec_Chuukese = 224,
			ec_Efik = 225,
			ec_Fante = 226,
			ec_Fijian = 227,
			ec_Gilbertese = 228,
			ec_Haitian = 229,
			ec_Hiligaynon = 230,
			ec_Hmong = 231,
			ec_Iban = 232,
			ec_Ilokano = 233,
			ec_Inupiaq = 234,
			ec_Kekchi = 235,
			ec_Kosraean = 236,
			ec_Mam = 237,
			ec_Marshallese = 238,
			ec_Navajo = 239,
			ec_Nivacle = 240,
			ec_Palauan = 241,
			ec_Pampangan = 242,
			ec_Pangasinan = 243,
			ec_Papiamento = 244,
			ec_Pohnpeian = 245,
			ec_Rarotongan = 246,
			ec_Samoan = 247,
			ec_Tahitian = 248,
			ec_TokPisin = 249,
			ec_Tshiluba = 250,
			ec_Twi = 251,
			ec_Tzotzil = 252,
			ec_Waray = 253,
			ec_Yapese = 254,
			ec_Yupik = 0xFF,
			ec_LAST_LANGUAGE = 0xFF
		}

		public enum et_Sub
		{
			ec_SubNeutral,
			ec_SubDefault,
			ec_SubSysDefault,
			ec_SubArabicSaudiArabia,
			ec_SubArabicIraq,
			ec_SubArabicEgypt,
			ec_SubArabicLibya,
			ec_SubArabicAlgeria,
			ec_SubArabicMorocco,
			ec_SubArabicTunisia,
			ec_SubArabicOman,
			ec_SubArabicYemen,
			ec_SubArabicSyria,
			ec_SubArabicJordan,
			ec_SubArabicLebanon,
			ec_SubArabicKuwait,
			ec_SubArabicUAE,
			ec_SubArabicBahrain,
			ec_SubArabicQatar,
			ec_SubChineseTrdTaiwan,
			ec_SubChineseSimChina,
			ec_SubChineseTrdHongkong,
			ec_SubChineseSimSingapore,
			ec_SubGerman,
			ec_SubGermanSwiss,
			ec_SubGermanAustrian,
			ec_SubGermanLuxembourg,
			ec_SubGermanLiechtenstein,
			ec_SubEnglishUS,
			ec_SubEnglishUK,
			ec_SubEnglishAUS,
			ec_SubEnglishCAN,
			ec_SubEnglishNZ,
			ec_SubEnglishIreland,
			ec_SubEnglishSouthAfrica,
			ec_SubEnglishJamaica,
			ec_SubEnglishCaribbean,
			ec_SubEnglishBelize,
			ec_SubEnglishTrinidad,
			ec_SubSpanish,
			ec_SubSpanishMexican,
			ec_SubSpanishModern,
			ec_SubSpanishGuatemala,
			ec_SubSpanishCostaRica,
			ec_SubSpanishPanama,
			ec_SubSpanishDominicanRepublic,
			ec_SubSpanishVenezuela,
			ec_SubSpanishColombia,
			ec_SubSpanishPeru,
			ec_SubSpanishArgentina,
			ec_SubSpanishEcuador,
			ec_SubSpanishChile,
			ec_SubSpanishUruguay,
			ec_SubSpanishParaguay,
			ec_SubSpanishBolivia,
			ec_SubSpanishElSalvador,
			ec_SubSpanishHonduras,
			ec_SubSpanishNicaragua,
			ec_SubSpanishPuertoRico,
			ec_SubFrench,
			ec_SubFrenchBelgian,
			ec_SubFrenchCanadian,
			ec_SubFrenchSwiss,
			ec_SubFrenchLuxembourg,
			ec_SubItalian,
			ec_SubItalianSwiss,
			ec_SubKorean,
			ec_SubKoreanJohab,
			ec_SubDutch,
			ec_SubDutchBelgian,
			ec_SubNorwegianBokmal,
			ec_SubNorwegianNynorsk,
			ec_SubPortugueseBrazilian,
			ec_SubPortuguese,
			ec_SubRomanian,
			ec_SubRomanianMoldavia,
			ec_SubRussian,
			ec_SubRussianMoldavia,
			ec_SubBCSSerbianLatin,
			ec_SubBCSSerbianCyrillic,
			ec_SubSwedish,
			ec_SubSwedishFinland,
			ec_SubGaelicScotlandDEPRECATED2,
			ec_SubIrishIreland,
			ec_SubGaelicScotlandDEPRECATED,
			ec_SubMalayMalaysia,
			ec_SubMalayBruneiDarussalam,
			ec_SubUzbekLatin,
			ec_SubUzbekCyrillic,
			ec_SubAzerbaijaniLatin,
			ec_SubAzerbaijaniCyrillic,
			ec_SubQuechuaBolivia,
			ec_SubQuechuaEcuador,
			ec_SubQuechuaPeru,
			ec_SubBCSCroatianCroatia,
			ec_SubBCSBosnianLatBH,
			ec_SubBCSSerbianCyrBH,
			ec_SubBCSSerbianLatBH,
			ec_SubBCSCroatianLatBH,
			ec_SubSpanishUSA,
			ec_SubAfarDjibouti,
			ec_SubAfarEritrea,
			ec_SubAfarEthiopia,
			ec_SubAfrikaansSouthAfrica,
			ec_SubAfrikaansNamibia,
			ec_SubAghemCameroon,
			ec_SubAkanGhana,
			ec_SubAlbanianAlbania,
			ec_SubAlbanianMacedonia,
			ec_SubAlbanianKosovo,
			ec_SubAlsatianFrance,
			ec_SubAlsatianSwitzerland,
			ec_SubAlsatianLiechtenstein,
			ec_SubAmharicEthiopia,
			ec_SubArabicDjibouti,
			ec_SubArabicEritrea,
			ec_SubArabicIsrael,
			ec_SubArabicComoros,
			ec_SubArabicMauritania,
			ec_SubArabicPalestina,
			ec_SubArabicSudan,
			ec_SubArabicSomalia,
			ec_SubArabicSouthSudan,
			ec_SubArabicChad,
			ec_SubArmenianArmenia,
			ec_SubAssameseIndia,
			ec_SubAsturianSpain,
			ec_SubAsuTanzania,
			ec_SubAzerbaijaniLatAzerbaijan,
			ec_SubAzerbaijaniCyrAzerbaijan,
			ec_SubBafiaCameroon,
			ec_SubBambaraLatMali,
			ec_SubBambaraLatin,
			ec_SubBanglaIndia,
			ec_SubBanglaBangladesh,
			ec_SubBasaaCameroon,
			ec_SubBashkirRussia,
			ec_SubBasqueSpain,
			ec_SubBelarusianBelarus,
			ec_SubBenaTanzania,
			ec_SubBlinEritrea,
			ec_SubBodoIndia,
			ec_SubBCSBosnianCyrBH,
			ec_SubBCSSerbianLatSerbia,
			ec_SubBCSSerbianCyrSerbia,
			ec_SubBCSSerbianLatMontenegro,
			ec_SubBCSSerbianCyrMontenegro,
			ec_SubBCSBosnianCyrillic,
			ec_SubBCSBosnianLatin,
			ec_SubBCSSerbianCyrKosovo,
			ec_SubBCSSerbianLatKosovo,
			ec_SubBretonFrance,
			ec_SubBulgarianBulgaria,
			ec_SubBurmeseMyanmar,
			ec_SubCatalanCatalan,
			ec_SubCatalanValencian,
			ec_SubCatalanAndora,
			ec_SubCatalanFrance,
			ec_SubCatalanItaly,
			ec_SubCentralKurdishIraq,
			ec_SubCentralKurdish,
			ec_SubCherokeeUS,
			ec_SubCherokeeCherokee,
			ec_SubChigaUganda,
			ec_SubChineseTrdMacao,
			ec_SubChineseSimplified,
			ec_SubChineseTraditional,
			ec_SubColognianRipGermany,
			ec_SubCornishUK,
			ec_SubCorsicanFrance,
			ec_SubCzechCzech,
			ec_SubDanishDenmark,
			ec_SubDanishGreenland,
			ec_SubDariAfghanistan,
			ec_SubDivehiMaldives,
			ec_SubDualaCameroon,
			ec_SubDutchAruba,
			ec_SubDutchBonaireSES,
			ec_SubDutchCuracao,
			ec_SubDutchSuriname,
			ec_SubDutchSintMaarten,
			ec_SubDzongkhaBhutan,
			ec_SubEmbuKenya,
			ec_SubEnglishZimbabwe,
			ec_SubEnglishPhilippines,
			ec_SubEnglishHongkong,
			ec_SubEnglishIndia,
			ec_SubEnglishMalaysia,
			ec_SubEnglishSingapore,
			ec_SubEnglishBahamas,
			ec_SubEnglishBelgium,
			ec_SubEnglishBotswana,
			ec_SubEnglishBIOT,
			ec_SubEnglishBritishVIsl,
			ec_SubEnglishCameroon,
			ec_SubEnglishCaymanIsl,
			ec_SubEnglishEritrea,
			ec_SubEnglishEurope,
			ec_SubEnglishFalklandIsl,
			ec_SubEnglishFiji,
			ec_SubEnglishGambia,
			ec_SubEnglishGhana,
			ec_SubEnglishGibraltar,
			ec_SubEnglishGuam,
			ec_SubEnglishGuyana,
			ec_SubEnglishKenya,
			ec_SubEnglishLesotho,
			ec_SubEnglishLiberia,
			ec_SubEnglishMacaoSAR,
			ec_SubEnglishMadagascar,
			ec_SubEnglishMalawi,
			ec_SubEnglishMalta,
			ec_SubEnglishMauritius,
			ec_SubEnglishNamibia,
			ec_SubEnglishNigeria,
			ec_SubEnglishPakistan,
			ec_SubEnglishPapuaNG,
			ec_SubEnglishPuertoRico,
			ec_SubEnglishRwanda,
			ec_SubEnglishSamoa,
			ec_SubEnglishSeychelles,
			ec_SubEnglishSierraLeone,
			ec_SubEnglishSouthSudan,
			ec_SubEnglishSudan,
			ec_SubEnglishSwaziland,
			ec_SubEnglishTanzania,
			ec_SubEnglishTokelau,
			ec_SubEnglishTonga,
			ec_SubEnglishTuvalu,
			ec_SubEnglishUganda,
			ec_SubEnglishUSVirginIsl,
			ec_SubEnglishVanuatu,
			ec_SubEnglishWorld,
			ec_SubEnglishZambia,
			ec_SubEsperantoWorld,
			ec_SubEstonianEstonia,
			ec_SubEweGhana,
			ec_SubEweTogo,
			ec_SubEwondoCameroon,
			ec_SubFaroeseFaroeIsl,
			ec_SubFilipinoPhilippines,
			ec_SubFinnishFinland,
			ec_SubFrenchMonaco,
			ec_SubFrenchReunion,
			ec_SubFrenchCongoDRC,
			ec_SubFrenchSenegal,
			ec_SubFrenchCameroon,
			ec_SubFrenchIvoryCoast,
			ec_SubFrenchMali,
			ec_SubFrenchMorocco,
			ec_SubFrenchHaiti,
			ec_SubFrenchBurkinaFaso,
			ec_SubFrenchBurundi,
			ec_SubFrenchBenin,
			ec_SubFrenchSaintBarthelemy,
			ec_SubFrenchCAR,
			ec_SubFrenchCongo,
			ec_SubFrenchDjibouti,
			ec_SubFrenchAlgeria,
			ec_SubFrenchGabon,
			ec_SubFrenchFrenchGuiana,
			ec_SubFrenchGuinea,
			ec_SubFrenchGuadeloupe,
			ec_SubFrenchEquatorialGuinea,
			ec_SubFrenchComoros,
			ec_SubFrenchSaintMartin,
			ec_SubFrenchMadagascar,
			ec_SubFrenchMartinique,
			ec_SubFrenchMauritania,
			ec_SubFrenchMauritius,
			ec_SubFrenchNewCaledonia,
			ec_SubFrenchNiger,
			ec_SubFrenchFrenchPolynesia,
			ec_SubFrenchSPM,
			ec_SubFrenchRwanda,
			ec_SubFrenchSeychelles,
			ec_SubFrenchSyria,
			ec_SubFrenchChad,
			ec_SubFrenchTogo,
			ec_SubFrenchTunisia,
			ec_SubFrenchVanuatu,
			ec_SubFrenchWallisFutuna,
			ec_SubFrenchMayotte,
			ec_SubFrisianNetherlands,
			ec_SubFriulianItaly,
			ec_SubFulahLatSenegal,
			ec_SubFulahLatin,
			ec_SubFulahCameroon,
			ec_SubFulahGuinea,
			ec_SubFulahMauritania,
			ec_SubGalicianSpain,
			ec_SubGandaUganda,
			ec_SubGeorgianGeorgia,
			ec_SubGermanBelgium,
			ec_SubGreekGreece,
			ec_SubGreekCyprus,
			ec_SubGreenlandicGreenland,
			ec_SubGuaraniParaguay,
			ec_SubGujaratiIndia,
			ec_SubGusiiKenya,
			ec_SubHausaLatNigeria,
			ec_SubHausaLatin,
			ec_SubHausaLatGhana,
			ec_SubHawaiianUS,
			ec_SubHebrewIsrael,
			ec_SubHindiIndia,
			ec_SubHungarianHungary,
			ec_SubIcelandicIceland,
			ec_SubIgboNigeria,
			ec_SubIndonesianIndonesia,
			ec_SubInterlinguaWorld,
			ec_SubInterlinguaFrance,
			ec_SubInuktitutSylCanada,
			ec_SubInuktitutLatCanada,
			ec_SubInuktitutSyllabics,
			ec_SubInuktitutLatin,
			ec_SubIsiXhosaSouthAfrica,
			ec_SubIsiZuluSouthAfrica,
			ec_SubItalianSanMarino,
			ec_SubJapaneseJapan,
			ec_SubJavaneseLatIndonesia,
			ec_SubJavaneseLatin,
			ec_SubJolaFonyiSenegal,
			ec_SubKicheGuatemala,
			ec_SubKabuverdianuCaboVerde,
			ec_SubKabyleAlgeria,
			ec_SubKakoCameroon,
			ec_SubKalenjinKenya,
			ec_SubKambaKenya,
			ec_SubKannadaIndia,
			ec_SubKashmiriPersoArabic,
			ec_SubKashmiriPAIndia,
			ec_SubKazakhKazakhstan,
			ec_SubKhmerCambodia,
			ec_SubKikuyuKenya,
			ec_SubKinyarwandaRwanda,
			ec_SubKiswahiliKenya,
			ec_SubKiswahiliTanzania,
			ec_SubKiswahiliUganda,
			ec_SubKonkaniIndia,
			ec_SubKoyraChiiniMali,
			ec_SubKoyraboroSenniMali,
			ec_SubKwasioCameroon,
			ec_SubKyrgyzKyrgyzstan,
			ec_SubLakotaUS,
			ec_SubLangiTanzania,
			ec_SubLaoLaoPDR,
			ec_SubLatvianLatvia,
			ec_SubLingalaAngola,
			ec_SubLingalaCongoDRC,
			ec_SubLingalaCAR,
			ec_SubLingalaCongo,
			ec_SubLithuanianLithuania,
			ec_SubSorbianUpperGermany,
			ec_SubSorbianLowerGermany,
			ec_SubLubaKatangaCongoDRC,
			ec_SubLuoKenya,
			ec_SubLuxembourgishLuxembourg,
			ec_SubLuyiaKenya,
			ec_SubMacedonianMacedonia,
			ec_SubMachameTanzania,
			ec_SubMakhuwaMeettoMozambique,
			ec_SubMakondeTanzania,
			ec_SubMalagasyMadagascar,
			ec_SubMalayLatSingapore,
			ec_SubMalayalamIndia,
			ec_SubMalteseMalta,
			ec_SubManxIsleOfMan,
			ec_SubMaoriNewZealand,
			ec_SubMapudungunChile,
			ec_SubMarathiIndia,
			ec_SubMasaiKenya,
			ec_SubMasaiTanzania,
			ec_SubMeruKenya,
			ec_SubMetaCameroon,
			ec_SubMohawkCanada,
			ec_SubMongolianCyrMongolia,
			ec_SubMongolianTrdChina,
			ec_SubMongolianTrdMongolia,
			ec_SubMongolianCyrillic,
			ec_SubMongolianTraditional,
			ec_SubMorisyenMauritius,
			ec_SubMundangCameroon,
			ec_SubNkoGuinea,
			ec_SubNamaNamibia,
			ec_SubNgiemboonCameroon,
			ec_SubNgombaCameroon,
			ec_SubNepaliNepal,
			ec_SubNepaliIndia,
			ec_SubNorthNdebeleZimbabwe,
			ec_SubNorwegianBokmalSJM,
			ec_SubNuerSudan,
			ec_SubNyankoleUganda,
			ec_SubOccitanFrance,
			ec_SubOdiaIndia,
			ec_SubOromoEthiopia,
			ec_SubOromoKenya,
			ec_SubOssetianCyrGeorgia,
			ec_SubOssetianCyrRussia,
			ec_SubPashtoAfghanistan,
			ec_SubPersianIran,
			ec_SubPersianAfghanistan,
			ec_SubPolishPoland,
			ec_SubPortugueseAngola,
			ec_SubPortugueseCaboVerde,
			ec_SubPortugueseGuineaBissau,
			ec_SubPortugueseMacaoSAR,
			ec_SubPortugueseMozambique,
			ec_SubPortugueseSaoTome,
			ec_SubPortugueseTimorLeste,
			ec_SubPunjabiIndia,
			ec_SubPunjabiPakistan,
			ec_SubPunjabiPunjabi,
			ec_SubRomanshSwitzerland,
			ec_SubRomboTanzania,
			ec_SubRundiBurundi,
			ec_SubRussianBelarus,
			ec_SubRussianKyrgyzstan,
			ec_SubRussianKazakhstan,
			ec_SubRussianUkraine,
			ec_SubRwaTanzania,
			ec_SubSahoEritrea,
			ec_SubSakhaRussia,
			ec_SubSamburuKenya,
			ec_SubSamiNorthernNorway,
			ec_SubSamiNorthernSweden,
			ec_SubSamiNorthernFinland,
			ec_SubSamiLuleNorway,
			ec_SubSamiLuleSweden,
			ec_SubSamiSouthernNorway,
			ec_SubSamiSouthernSweden,
			ec_SubSamiSkoltFinland,
			ec_SubSamiInariFinland,
			ec_SubSamiInari,
			ec_SubSamiSkolt,
			ec_SubSamiSouthern,
			ec_SubSamiLule,
			ec_SubSangoCAR,
			ec_SubSanguTanzania,
			ec_SubSanskritIndia,
			ec_SubScottishGaelicUK,
			ec_SubSenaMozambique,
			ec_SubSesothoSaLeboaSouthAfrica,
			ec_SubSetswanaSouthAfrica,
			ec_SubSetswanaBotswana,
			ec_SubShambalaTanzania,
			ec_SubShonaLatZimbabwe,
			ec_SubShonaLatin,
			ec_SubSindhiPakistan,
			ec_SubSindhiSindhi,
			ec_SubSinhalaSriLanka,
			ec_SubSlovakSlovakia,
			ec_SubSlovenianSlovenia,
			ec_SubSogaUganda,
			ec_SubSomaliSomalia,
			ec_SubSomaliDjibouti,
			ec_SubSomaliEthiopia,
			ec_SubSomaliKenya,
			ec_SubSothoSouthAfrica,
			ec_SubSothoLesotho,
			ec_SubSouthNdebeleSouthAfrica,
			ec_SubSpanishLatinAmerica,
			ec_SubSpanishCuba,
			ec_SubSpanishPhilippines,
			ec_SubMoroccanTifMorocco,
			ec_SubMoroccanTifinagh,
			ec_SubSwatiSwaziland,
			ec_SubSwatiSouthAfrica,
			ec_SubSwedishAlandIsl,
			ec_SubSyriacSyria,
			ec_TachelhitLatMorocco,
			ec_TachelhitTifMorocco,
			ec_TachelhitLatin,
			ec_TachelhitTifinagh,
			ec_SubTaitaKenya,
			ec_SubTajikCyrTajikistan,
			ec_SubTajikCyrillic,
			ec_SubTamazightLatAlgeria,
			ec_SubTamazightTifMorocco,
			ec_SubTamazightTifinagh,
			ec_SubTamazightLatin,
			ec_SubTamilIndia,
			ec_SubTamilSriLanka,
			ec_SubTamilMalaysia,
			ec_SubTamilSingapore,
			ec_SubTasawaqNiger,
			ec_SubTatarRussia,
			ec_SubTeluguIndia,
			ec_SubTesoKenya,
			ec_SubTesoUganda,
			ec_SubThaiThailand,
			ec_SubTibetanChina,
			ec_SubTibetanIndia,
			ec_SubTigreEritrea,
			ec_SubTigrinyaEthiopia,
			ec_SubTigrinyaEritrea,
			ec_SubTonganTonga,
			ec_SubTsongaSouthAfrica,
			ec_SubTurkishTurkey,
			ec_SubTurkishCyprus,
			ec_SubTurkmenTurkmenistan,
			ec_SubUkrainianUkraine,
			ec_SubUrduPakistan,
			ec_SubUrduIndia,
			ec_SubUyghurChina,
			ec_SubUzbekPersoArabic,
			ec_SubUzbekPAAfghanistan,
			ec_SubVaiLatin,
			ec_SubVaiLatLiberia,
			ec_SubVaiVai,
			ec_SubVaiVaiLiberia,
			ec_SubVendaSouthAfrica,
			ec_SubVietnameseVietnam,
			ec_SubVolapukWorld,
			ec_SubVunjoTanzania,
			ec_SubWalserSwitzerland,
			ec_SubWelshUK,
			ec_SubWolayttaEthiopia,
			ec_SubWolofSenegal,
			ec_SubYangbenCameroon,
			ec_SubYiChina,
			ec_SubYiddishWorld,
			ec_SubYorubaNigeria,
			ec_SubYorubaBenin,
			ec_SubZarmaNiger,
			ec_SubAmSignLngUS,
			ec_SubAymaraPeru,
			ec_SubBikolanoPhilippines,
			ec_SubBislamaVanuatu,
			ec_SubCakchiquelGuatemala,
			ec_SubCebuanoPhilippines,
			ec_SubChuukeseMicronesia,
			ec_SubEfikNigeria,
			ec_SubFanteGhana,
			ec_SubFijianFiji,
			ec_SubGilberteseKiribati,
			ec_SubHaitianHaiti,
			ec_SubHiligaynonPhilippines,
			ec_SubHindiFiji,
			ec_SubHmongChina,
			ec_SubIbanMalaysia,
			ec_SubIlokanoPhilippines,
			ec_SubInupiaqUS,
			ec_SubKekchiGuatemala,
			ec_SubKosraeanMicronesia,
			ec_SubMamGuatemala,
			ec_SubMarshalleseMarshallIsl,
			ec_SubNavajoUS,
			ec_SubNivacleParaguay,
			ec_SubPalauanPalau,
			ec_SubPampanganPhilippines,
			ec_SubPangasinanPhilippines,
			ec_SubPapiamentoDutchAntilles,
			ec_SubPohnpeianMicronesia,
			ec_SubRarotonganCookIsl,
			ec_SubSamoanWesternSamoa,
			ec_SubTahitianFrenchPolynesia,
			ec_SubTokPisinPapuaNG,
			ec_SubTshilubaCongoDRC,
			ec_SubTwiGhana,
			ec_SubTzotzilMexico,
			ec_SubWarayAustralia,
			ec_SubYapeseMicronesia,
			ec_SubYupikUS,
			ec_SubBislamaBislamaSDL,
			ec_SubChineseHmongSDL,
			ec_SubFijianFijianSDL,
			ec_SubHawaiianNauruSDL,
			ec_SubHawaiianSamoanSDL,
			ec_SubHindiBihariSDL,
			ec_SubInuktitutInupiaqSDL,
			ec_SubKiswahiliChewaSDL,
			ec_SubMalayalamSundaneseSDL,
			ec_SubQuechuaAymaraSDL,
			ec_SubSpanishInternationalSDL,
			ec_SubSpanishModernSDL
		}

		public struct st_PrimaryLanguage
		{
			public et_Primary eo_Primary;

			public ushort n2_PrimaryLanguageID;

			public string qcuz_LanguageCode;

			public string qcuz_Language;

			public uint n_SubLanguages;

			public int i_SubLanguagesOffset;

			public st_PrimaryLanguage(et_Primary eo_Primary, ushort n2_PrimaryLanguageID, string qcuz_LanguageCode, string qcuz_Language, uint n_SubLanguages, int i_SubLanguagesOffset)
			{
				this.eo_Primary = eo_Primary;
				this.n2_PrimaryLanguageID = n2_PrimaryLanguageID;
				this.qcuz_LanguageCode = qcuz_LanguageCode;
				this.qcuz_Language = qcuz_Language;
				this.n_SubLanguages = n_SubLanguages;
				this.i_SubLanguagesOffset = i_SubLanguagesOffset;
			}
		}

		public struct st_SubLanguage
		{
			public et_Sub eo_Sub;

			public ushort n2_PrimaryLanguageID;

			public ushort n2_SubLanguageID;

			public string qcuz_CountryCode;

			public string qcuz_Country;

			public uint n_PrimaryLanguageOffset;

			public st_SubLanguage(et_Sub eo_Sub, ushort n2_PrimaryLanguageID, ushort n2_SubLanguageID, string qcuz_CountryCode, string qcuz_Country, uint n_PrimaryLanguageOffset)
			{
				this.eo_Sub = eo_Sub;
				this.n2_PrimaryLanguageID = n2_PrimaryLanguageID;
				this.n2_SubLanguageID = n2_SubLanguageID;
				this.qcuz_CountryCode = qcuz_CountryCode;
				this.qcuz_Country = qcuz_Country;
				this.n_PrimaryLanguageOffset = n_PrimaryLanguageOffset;
			}
		}

		public const ushort LANG_NEUTRAL = 0;

		public const ushort LANG_INVARIANT = 127;

		public const ushort LANG_AFRIKAANS = 54;

		public const ushort LANG_ALBANIAN = 28;

		public const ushort LANG_ALSATIAN = 132;

		public const ushort LANG_AMHARIC = 94;

		public const ushort LANG_ARABIC = 1;

		public const ushort LANG_ARMENIAN = 43;

		public const ushort LANG_ASSAMESE = 77;

		public const ushort LANG_AZERBAIJANI = 44;

		public const ushort LANG_BANGLA = 69;

		public const ushort LANG_BASHKIR = 109;

		public const ushort LANG_BASQUE = 45;

		public const ushort LANG_BELARUSIAN = 35;

		public const ushort LANG_BRETON = 126;

		public const ushort LANG_BOSNIAN = 26;

		public const ushort LANG_BULGARIAN = 2;

		public const ushort LANG_CATALAN = 3;

		public const ushort LANG_CENTRAL_KURDISH = 146;

		public const ushort LANG_CHEROKEE = 92;

		public const ushort LANG_CHINESE = 4;

		public const ushort LANG_CORSICAN = 131;

		public const ushort LANG_CROATIAN = 26;

		public const ushort LANG_CZECH = 5;

		public const ushort LANG_DANISH = 6;

		public const ushort LANG_DARI = 140;

		public const ushort LANG_DIVEHI = 101;

		public const ushort LANG_DUTCH = 19;

		public const ushort LANG_ENGLISH = 9;

		public const ushort LANG_ESTONIAN = 37;

		public const ushort LANG_FAEROESE = 56;

		public const ushort LANG_FILIPINO = 100;

		public const ushort LANG_FINNISH = 11;

		public const ushort LANG_FRENCH = 12;

		public const ushort LANG_FRISIAN = 98;

		public const ushort LANG_FULAH = 103;

		public const ushort LANG_GALICIAN = 86;

		public const ushort LANG_GEORGIAN = 55;

		public const ushort LANG_GERMAN = 7;

		public const ushort LANG_GREEK = 8;

		public const ushort LANG_GREENLANDIC = 111;

		public const ushort LANG_GUJARATI = 71;

		public const ushort LANG_HAUSA = 104;

		public const ushort LANG_HAWAIIAN = 117;

		public const ushort LANG_HEBREW = 13;

		public const ushort LANG_HINDI = 57;

		public const ushort LANG_HUNGARIAN = 14;

		public const ushort LANG_ICELANDIC = 15;

		public const ushort LANG_IGBO = 112;

		public const ushort LANG_INDONESIAN = 33;

		public const ushort LANG_INUKTITUT = 93;

		public const ushort LANG_IRISH = 60;

		public const ushort LANG_ITALIAN = 16;

		public const ushort LANG_JAPANESE = 17;

		public const ushort LANG_KANNADA = 75;

		public const ushort LANG_KASHMIRI = 96;

		public const ushort LANG_KAZAK = 63;

		public const ushort LANG_KHMER = 83;

		public const ushort LANG_KICHE = 134;

		public const ushort LANG_KINYARWANDA = 135;

		public const ushort LANG_KONKANI = 87;

		public const ushort LANG_KOREAN = 18;

		public const ushort LANG_KYRGYZ = 64;

		public const ushort LANG_LAO = 84;

		public const ushort LANG_LATVIAN = 38;

		public const ushort LANG_LITHUANIAN = 39;

		public const ushort LANG_LOWER_SORBIAN = 46;

		public const ushort LANG_LUXEMBOURGISH = 110;

		public const ushort LANG_MACEDONIAN = 47;

		public const ushort LANG_MALAY = 62;

		public const ushort LANG_MALAYALAM = 76;

		public const ushort LANG_MALTESE = 58;

		public const ushort LANG_MANIPURI = 88;

		public const ushort LANG_MAORI = 129;

		public const ushort LANG_MAPUDUNGUN = 122;

		public const ushort LANG_MARATHI = 78;

		public const ushort LANG_MOHAWK = 124;

		public const ushort LANG_MONGOLIAN = 80;

		public const ushort LANG_NEPALI = 97;

		public const ushort LANG_NORWEGIAN = 20;

		public const ushort LANG_OCCITAN = 130;

		public const ushort LANG_ODIA = 72;

		public const ushort LANG_PASHTO = 99;

		public const ushort LANG_PERSIAN = 41;

		public const ushort LANG_POLISH = 21;

		public const ushort LANG_PORTUGUESE = 22;

		public const ushort LANG_PUNJABI = 70;

		public const ushort LANG_QUECHUA = 107;

		public const ushort LANG_ROMANIAN = 24;

		public const ushort LANG_ROMANSH = 23;

		public const ushort LANG_RUSSIAN = 25;

		public const ushort LANG_SAKHA = 133;

		public const ushort LANG_SAMI = 59;

		public const ushort LANG_SANSKRIT = 79;

		public const ushort LANG_SCOTTISH_GAELIC = 145;

		public const ushort LANG_SERBIAN = 26;

		public const ushort LANG_SINDHI = 89;

		public const ushort LANG_SINHALESE = 91;

		public const ushort LANG_SLOVAK = 27;

		public const ushort LANG_SLOVENIAN = 36;

		public const ushort LANG_SOTHO = 108;

		public const ushort LANG_SPANISH = 10;

		public const ushort LANG_SWAHILI = 65;

		public const ushort LANG_SWEDISH = 29;

		public const ushort LANG_SYRIAC = 90;

		public const ushort LANG_TAJIK = 40;

		public const ushort LANG_TAMAZIGHT = 95;

		public const ushort LANG_TAMIL = 73;

		public const ushort LANG_TATAR = 68;

		public const ushort LANG_TELUGU = 74;

		public const ushort LANG_THAI = 30;

		public const ushort LANG_TIBETAN = 81;

		public const ushort LANG_TIGRINYA = 115;

		public const ushort LANG_TSWANA = 50;

		public const ushort LANG_TURKISH = 31;

		public const ushort LANG_TURKMEN = 66;

		public const ushort LANG_UIGHUR = 128;

		public const ushort LANG_UKRAINIAN = 34;

		public const ushort LANG_UPPER_SORBIAN = 46;

		public const ushort LANG_URDU = 32;

		public const ushort LANG_UZBEK = 67;

		public const ushort LANG_VALENCIAN = 3;

		public const ushort LANG_VIETNAMESE = 42;

		public const ushort LANG_WELSH = 82;

		public const ushort LANG_WOLOF = 136;

		public const ushort LANG_XHOSA = 52;

		public const ushort LANG_YI = 120;

		public const ushort LANG_YORUBA = 106;

		public const ushort LANG_ZULU = 53;

		public const ushort SDL_LANG_AFAR = 512;

		public const ushort SDL_LANG_AGHEM = 513;

		public const ushort SDL_LANG_AKAN = 514;

		public const ushort SDL_LANG_ASTURIAN = 515;

		public const ushort SDL_LANG_ASU = 516;

		public const ushort SDL_LANG_BAFIA = 517;

		public const ushort SDL_LANG_BAMBARA = 518;

		public const ushort SDL_LANG_BASAA = 519;

		public const ushort SDL_LANG_BENA = 520;

		public const ushort SDL_LANG_BLIN = 521;

		public const ushort SDL_LANG_BODO = 522;

		public const ushort SDL_LANG_CHIGA = 523;

		public const ushort SDL_LANG_COLOGNIAN = 524;

		public const ushort SDL_LANG_CORNISH = 525;

		public const ushort SDL_LANG_DUALA = 526;

		public const ushort SDL_LANG_DZONGKHA = 527;

		public const ushort SDL_LANG_EMBU = 528;

		public const ushort SDL_LANG_ESPERANTO = 529;

		public const ushort SDL_LANG_EWE = 530;

		public const ushort SDL_LANG_EWONDO = 531;

		public const ushort SDL_LANG_FRIULIAN = 532;

		public const ushort SDL_LANG_GANDA = 533;

		public const ushort SDL_LANG_GUSII = 534;

		public const ushort SDL_LANG_INTERLINGUA = 535;

		public const ushort SDL_LANG_JAVANESE = 536;

		public const ushort SDL_LANG_JOLAFONYI = 537;

		public const ushort SDL_LANG_KABUVERDIANU = 538;

		public const ushort SDL_LANG_KABYLE = 539;

		public const ushort SDL_LANG_KAKO = 540;

		public const ushort SDL_LANG_KALENJIN = 541;

		public const ushort SDL_LANG_KAMBA = 542;

		public const ushort SDL_LANG_KIKUYU = 543;

		public const ushort SDL_LANG_KOYRACHIINI = 544;

		public const ushort SDL_LANG_KOYRABORO = 545;

		public const ushort SDL_LANG_KWASIO = 546;

		public const ushort SDL_LANG_LAKOTA = 547;

		public const ushort SDL_LANG_LANGI = 548;

		public const ushort SDL_LANG_LINGALA = 549;

		public const ushort SDL_LANG_LUBAKATANGA = 550;

		public const ushort SDL_LANG_LUO = 551;

		public const ushort SDL_LANG_LUYIA = 552;

		public const ushort SDL_LANG_MACHAME = 553;

		public const ushort SDL_LANG_MAKHUWA = 554;

		public const ushort SDL_LANG_MAKONDE = 555;

		public const ushort SDL_LANG_MALAGASY = 556;

		public const ushort SDL_LANG_MANX = 557;

		public const ushort SDL_LANG_MASAI = 558;

		public const ushort SDL_LANG_MERU = 559;

		public const ushort SDL_LANG_META = 560;

		public const ushort SDL_LANG_MORISYEN = 561;

		public const ushort SDL_LANG_MUNDANG = 562;

		public const ushort SDL_LANG_NKO = 563;

		public const ushort SDL_LANG_NAMA = 564;

		public const ushort SDL_LANG_NGIEMBOON = 565;

		public const ushort SDL_LANG_NGOMBA = 566;

		public const ushort SDL_LANG_NORTHNDEBELE = 567;

		public const ushort SDL_LANG_NUER = 568;

		public const ushort SDL_LANG_NYANKOLE = 569;

		public const ushort SDL_LANG_OSSETIAN = 570;

		public const ushort SDL_LANG_ROMBO = 571;

		public const ushort SDL_LANG_RUNDI = 572;

		public const ushort SDL_LANG_RWA = 573;

		public const ushort SDL_LANG_SAHO = 574;

		public const ushort SDL_LANG_SAMBURU = 575;

		public const ushort SDL_LANG_SANGO = 576;

		public const ushort SDL_LANG_SANGU = 577;

		public const ushort SDL_LANG_SENA = 578;

		public const ushort SDL_LANG_SHAMBALA = 579;

		public const ushort SDL_LANG_SHONA = 580;

		public const ushort SDL_LANG_SOGA = 581;

		public const ushort SDL_LANG_SOUTHNDEBELE = 582;

		public const ushort SDL_LANG_MOROCCAN = 583;

		public const ushort SDL_LANG_SWATI = 584;

		public const ushort SDL_LANG_TACHELHIT = 585;

		public const ushort SDL_LANG_TAITA = 586;

		public const ushort SDL_LANG_TASAWAQ = 587;

		public const ushort SDL_LANG_TESO = 588;

		public const ushort SDL_LANG_TIGRE = 589;

		public const ushort SDL_LANG_TONGAN = 590;

		public const ushort SDL_LANG_VAI = 591;

		public const ushort SDL_LANG_VOLAPUK = 592;

		public const ushort SDL_LANG_VUNJO = 593;

		public const ushort SDL_LANG_WALSER = 594;

		public const ushort SDL_LANG_WOLAYTTA = 595;

		public const ushort SDL_LANG_YANGBEN = 596;

		public const ushort SDL_LANG_ZARMA = 597;

		public const ushort SDL_LANG_AMSIGNLNG = 598;

		public const ushort SDL_LANG_AYMARA = 599;

		public const ushort SDL_LANG_BIKOLANO = 600;

		public const ushort SDL_LANG_BISLAMA = 601;

		public const ushort SDL_LANG_CAKCHIQUEL = 602;

		public const ushort SDL_LANG_CEBUANO = 603;

		public const ushort SDL_LANG_CHUUKESE = 604;

		public const ushort SDL_LANG_EFIK = 605;

		public const ushort SDL_LANG_FANTE = 606;

		public const ushort SDL_LANG_FIJIAN = 607;

		public const ushort SDL_LANG_GILBERTESE = 608;

		public const ushort SDL_LANG_HAITIAN = 609;

		public const ushort SDL_LANG_HILIGAYNON = 610;

		public const ushort SDL_LANG_HMONG = 611;

		public const ushort SDL_LANG_IBAN = 612;

		public const ushort SDL_LANG_ILOKANO = 613;

		public const ushort SDL_LANG_INUPIAQ = 614;

		public const ushort SDL_LANG_KEKCHI = 615;

		public const ushort SDL_LANG_KOSRAEAN = 616;

		public const ushort SDL_LANG_MAM = 617;

		public const ushort SDL_LANG_MARSHALLESE = 618;

		public const ushort SDL_LANG_NAVAJO = 619;

		public const ushort SDL_LANG_NIVACLE = 620;

		public const ushort SDL_LANG_PALAUAN = 621;

		public const ushort SDL_LANG_PAMPANGAN = 622;

		public const ushort SDL_LANG_PANGASINAN = 623;

		public const ushort SDL_LANG_PAPIAMENTO = 624;

		public const ushort SDL_LANG_POHNPEIAN = 625;

		public const ushort SDL_LANG_RAROTONGAN = 626;

		public const ushort SDL_LANG_SAMOAN = 627;

		public const ushort SDL_LANG_TAHITIAN = 628;

		public const ushort SDL_LANG_TOKPISIN = 629;

		public const ushort SDL_LANG_TSHILUBA = 630;

		public const ushort SDL_LANG_TWI = 631;

		public const ushort SDL_LANG_TZOTZIL = 632;

		public const ushort SDL_LANG_WARAY = 633;

		public const ushort SDL_LANG_YAPESE = 634;

		public const ushort SDL_LANG_YUPIK = 635;

		public const ushort SUBLANG_NEUTRAL = 0;

		public const ushort SUBLANG_DEFAULT = 1;

		public const ushort SUBLANG_SYS_DEFAULT = 2;

		public const ushort SUBLANG_AFRIKAANS_SOUTH_AFRICA = 1;

		public const ushort SUBLANG_ALBANIAN_ALBANIA = 1;

		public const ushort SUBLANG_ALSATIAN_FRANCE = 1;

		public const ushort SUBLANG_AMHARIC_ETHIOPIA = 1;

		public const ushort SUBLANG_ARABIC_SAUDI_ARABIA = 1;

		public const ushort SUBLANG_ARABIC_IRAQ = 2;

		public const ushort SUBLANG_ARABIC_EGYPT = 3;

		public const ushort SUBLANG_ARABIC_LIBYA = 4;

		public const ushort SUBLANG_ARABIC_ALGERIA = 5;

		public const ushort SUBLANG_ARABIC_MOROCCO = 6;

		public const ushort SUBLANG_ARABIC_TUNISIA = 7;

		public const ushort SUBLANG_ARABIC_OMAN = 8;

		public const ushort SUBLANG_ARABIC_YEMEN = 9;

		public const ushort SUBLANG_ARABIC_SYRIA = 10;

		public const ushort SUBLANG_ARABIC_JORDAN = 11;

		public const ushort SUBLANG_ARABIC_LEBANON = 12;

		public const ushort SUBLANG_ARABIC_KUWAIT = 13;

		public const ushort SUBLANG_ARABIC_UAE = 14;

		public const ushort SUBLANG_ARABIC_BAHRAIN = 15;

		public const ushort SUBLANG_ARABIC_QATAR = 16;

		public const ushort SUBLANG_ARMENIAN_ARMENIA = 1;

		public const ushort SUBLANG_ASSAMESE_INDIA = 1;

		public const ushort SUBLANG_AZERI_LATIN = 1;

		public const ushort SUBLANG_AZERI_CYRILLIC = 2;

		public const ushort SUBLANG_AZERBAIJANI_AZERBAIJAN_LATIN = 1;

		public const ushort SUBLANG_AZERBAIJANI_AZERBAIJAN_CYRILLIC = 2;

		public const ushort SUBLANG_BANGLA_INDIA = 1;

		public const ushort SUBLANG_BANGLA_BANGLADESH = 2;

		public const ushort SUBLANG_BASHKIR_RUSSIA = 1;

		public const ushort SUBLANG_BASQUE_BASQUE = 1;

		public const ushort SUBLANG_BELARUSIAN_BELARUS = 1;

		public const ushort SUBLANG_BENGALI_INDIA = 1;

		public const ushort SUBLANG_BENGALI_BANGLADESH = 2;

		public const ushort SUBLANG_BOSNIAN_BOSNIA_HERZEGOVINA_LATIN = 5;

		public const ushort SUBLANG_BOSNIAN_BOSNIA_HERZEGOVINA_CYRILLIC = 8;

		public const ushort SUBLANG_BRETON_FRANCE = 1;

		public const ushort SUBLANG_BULGARIAN_BULGARIA = 1;

		public const ushort SUBLANG_CATALAN_CATALAN = 1;

		public const ushort SUBLANG_CENTRAL_KURDISH_IRAQ = 1;

		public const ushort SUBLANG_CHEROKEE_CHEROKEE = 1;

		public const ushort SUBLANG_CHINESE_TRADITIONAL = 1;

		public const ushort SUBLANG_CHINESE_SIMPLIFIED = 2;

		public const ushort SUBLANG_CHINESE_HONGKONG = 3;

		public const ushort SUBLANG_CHINESE_SINGAPORE = 4;

		public const ushort SUBLANG_CHINESE_MACAU = 5;

		public const ushort SUBLANG_CORSICAN_FRANCE = 1;

		public const ushort SUBLANG_CZECH_CZECH_REPUBLIC = 1;

		public const ushort SUBLANG_CROATIAN_CROATIA = 1;

		public const ushort SUBLANG_CROATIAN_BOSNIA_HERZEGOVINA_LATIN = 4;

		public const ushort SUBLANG_DANISH_DENMARK = 1;

		public const ushort SUBLANG_DARI_AFGHANISTAN = 1;

		public const ushort SUBLANG_DIVEHI_MALDIVES = 1;

		public const ushort SUBLANG_DUTCH = 1;

		public const ushort SUBLANG_DUTCH_BELGIAN = 2;

		public const ushort SUBLANG_ENGLISH_US = 1;

		public const ushort SUBLANG_ENGLISH_UK = 2;

		public const ushort SUBLANG_ENGLISH_AUS = 3;

		public const ushort SUBLANG_ENGLISH_CAN = 4;

		public const ushort SUBLANG_ENGLISH_NZ = 5;

		public const ushort SUBLANG_ENGLISH_EIRE = 6;

		public const ushort SUBLANG_ENGLISH_SOUTH_AFRICA = 7;

		public const ushort SUBLANG_ENGLISH_JAMAICA = 8;

		public const ushort SUBLANG_ENGLISH_CARIBBEAN = 9;

		public const ushort SUBLANG_ENGLISH_BELIZE = 10;

		public const ushort SUBLANG_ENGLISH_TRINIDAD = 11;

		public const ushort SUBLANG_ENGLISH_ZIMBABWE = 12;

		public const ushort SUBLANG_ENGLISH_PHILIPPINES = 13;

		public const ushort SUBLANG_ENGLISH_INDIA = 16;

		public const ushort SUBLANG_ENGLISH_MALAYSIA = 17;

		public const ushort SUBLANG_ENGLISH_SINGAPORE = 18;

		public const ushort SUBLANG_ESTONIAN_ESTONIA = 1;

		public const ushort SUBLANG_FAEROESE_FAROE_ISLANDS = 1;

		public const ushort SUBLANG_FILIPINO_PHILIPPINES = 1;

		public const ushort SUBLANG_FINNISH_FINLAND = 1;

		public const ushort SUBLANG_FRENCH = 1;

		public const ushort SUBLANG_FRENCH_BELGIAN = 2;

		public const ushort SUBLANG_FRENCH_CANADIAN = 3;

		public const ushort SUBLANG_FRENCH_SWISS = 4;

		public const ushort SUBLANG_FRENCH_LUXEMBOURG = 5;

		public const ushort SUBLANG_FRENCH_MONACO = 6;

		public const ushort SUBLANG_FRISIAN_NETHERLANDS = 1;

		public const ushort SUBLANG_FULAH_SENEGAL = 2;

		public const ushort SUBLANG_GALICIAN_GALICIAN = 1;

		public const ushort SUBLANG_GEORGIAN_GEORGIA = 1;

		public const ushort SUBLANG_GERMAN = 1;

		public const ushort SUBLANG_GERMAN_SWISS = 2;

		public const ushort SUBLANG_GERMAN_AUSTRIAN = 3;

		public const ushort SUBLANG_GERMAN_LUXEMBOURG = 4;

		public const ushort SUBLANG_GERMAN_LIECHTENSTEIN = 5;

		public const ushort SUBLANG_GREEK_GREECE = 1;

		public const ushort SUBLANG_GREENLANDIC_GREENLAND = 1;

		public const ushort SUBLANG_GUJARATI_INDIA = 1;

		public const ushort SUBLANG_HAUSA_NIGERIA_LATIN = 1;

		public const ushort SUBLANG_HAWAIIAN_US = 1;

		public const ushort SUBLANG_HEBREW_ISRAEL = 1;

		public const ushort SUBLANG_HINDI_INDIA = 1;

		public const ushort SUBLANG_HUNGARIAN_HUNGARY = 1;

		public const ushort SUBLANG_ICELANDIC_ICELAND = 1;

		public const ushort SUBLANG_IGBO_NIGERIA = 1;

		public const ushort SUBLANG_INDONESIAN_INDONESIA = 1;

		public const ushort SUBLANG_INUKTITUT_CANADA = 1;

		public const ushort SUBLANG_INUKTITUT_CANADA_LATIN = 2;

		public const ushort SUBLANG_IRISH_IRELAND = 2;

		public const ushort SUBLANG_ITALIAN = 1;

		public const ushort SUBLANG_ITALIAN_SWISS = 2;

		public const ushort SUBLANG_JAPANESE_JAPAN = 1;

		public const ushort SUBLANG_KANNADA_INDIA = 1;

		public const ushort SUBLANG_KASHMIRI_SASIA = 2;

		public const ushort SUBLANG_KASHMIRI_INDIA = 2;

		public const ushort SUBLANG_KAZAK_KAZAKHSTAN = 1;

		public const ushort SUBLANG_KHMER_CAMBODIA = 1;

		public const ushort SUBLANG_KICHE_GUATEMALA = 1;

		public const ushort SUBLANG_KINYARWANDA_RWANDA = 1;

		public const ushort SUBLANG_KONKANI_INDIA = 1;

		public const ushort SUBLANG_KOREAN = 1;

		public const ushort SUBLANG_KYRGYZ_KYRGYZSTAN = 1;

		public const ushort SUBLANG_LAO_LAO = 1;

		public const ushort SUBLANG_LATVIAN_LATVIA = 1;

		public const ushort SUBLANG_LITHUANIAN = 1;

		public const ushort SUBLANG_LOWER_SORBIAN_GERMANY = 2;

		public const ushort SUBLANG_LUXEMBOURGISH_LUXEMBOURG = 1;

		public const ushort SUBLANG_MACEDONIAN_MACEDONIA = 1;

		public const ushort SUBLANG_MALAY_MALAYSIA = 1;

		public const ushort SUBLANG_MALAY_BRUNEI_DARUSSALAM = 2;

		public const ushort SUBLANG_MALAYALAM_INDIA = 1;

		public const ushort SUBLANG_MALTESE_MALTA = 1;

		public const ushort SUBLANG_MAORI_NEW_ZEALAND = 1;

		public const ushort SUBLANG_MAPUDUNGUN_CHILE = 1;

		public const ushort SUBLANG_MARATHI_INDIA = 1;

		public const ushort SUBLANG_MOHAWK_MOHAWK = 1;

		public const ushort SUBLANG_MONGOLIAN_CYRILLIC_MONGOLIA = 1;

		public const ushort SUBLANG_MONGOLIAN_PRC = 2;

		public const ushort SUBLANG_NEPALI_INDIA = 2;

		public const ushort SUBLANG_NEPALI_NEPAL = 1;

		public const ushort SUBLANG_NORWEGIAN_BOKMAL = 1;

		public const ushort SUBLANG_NORWEGIAN_NYNORSK = 2;

		public const ushort SUBLANG_OCCITAN_FRANCE = 1;

		public const ushort SUBLANG_ODIA_INDIA = 1;

		public const ushort SUBLANG_ORIYA_INDIA = 1;

		public const ushort SUBLANG_PASHTO_AFGHANISTAN = 1;

		public const ushort SUBLANG_PERSIAN_IRAN = 1;

		public const ushort SUBLANG_POLISH_POLAND = 1;

		public const ushort SUBLANG_PORTUGUESE = 2;

		public const ushort SUBLANG_PORTUGUESE_BRAZILIAN = 1;

		public const ushort SUBLANG_PULAR_SENEGAL = 2;

		public const ushort SUBLANG_PUNJABI_INDIA = 1;

		public const ushort SUBLANG_PUNJABI_PAKISTAN = 2;

		public const ushort SUBLANG_QUECHUA_BOLIVIA = 1;

		public const ushort SUBLANG_QUECHUA_ECUADOR = 2;

		public const ushort SUBLANG_QUECHUA_PERU = 3;

		public const ushort SUBLANG_ROMANIAN_ROMANIA = 1;

		public const ushort SUBLANG_ROMANSH_SWITZERLAND = 1;

		public const ushort SUBLANG_RUSSIAN_RUSSIA = 1;

		public const ushort SUBLANG_SAKHA_RUSSIA = 1;

		public const ushort SUBLANG_SAMI_NORTHERN_NORWAY = 1;

		public const ushort SUBLANG_SAMI_NORTHERN_SWEDEN = 2;

		public const ushort SUBLANG_SAMI_NORTHERN_FINLAND = 3;

		public const ushort SUBLANG_SAMI_LULE_NORWAY = 4;

		public const ushort SUBLANG_SAMI_LULE_SWEDEN = 5;

		public const ushort SUBLANG_SAMI_SOUTHERN_NORWAY = 6;

		public const ushort SUBLANG_SAMI_SOUTHERN_SWEDEN = 7;

		public const ushort SUBLANG_SAMI_SKOLT_FINLAND = 8;

		public const ushort SUBLANG_SAMI_INARI_FINLAND = 9;

		public const ushort SUBLANG_SANSKRIT_INDIA = 1;

		public const ushort SUBLANG_SCOTTISH_GAELIC = 1;

		public const ushort SUBLANG_SERBIAN_BOSNIA_HERZEGOVINA_LATIN = 6;

		public const ushort SUBLANG_SERBIAN_BOSNIA_HERZEGOVINA_CYRILLIC = 7;

		public const ushort SUBLANG_SERBIAN_MONTENEGRO_LATIN = 11;

		public const ushort SUBLANG_SERBIAN_MONTENEGRO_CYRILLIC = 12;

		public const ushort SUBLANG_SERBIAN_SERBIA_LATIN = 9;

		public const ushort SUBLANG_SERBIAN_SERBIA_CYRILLIC = 10;

		public const ushort SUBLANG_SERBIAN_CROATIA = 1;

		public const ushort SUBLANG_SERBIAN_LATIN = 2;

		public const ushort SUBLANG_SERBIAN_CYRILLIC = 3;

		public const ushort SUBLANG_SINDHI_INDIA = 1;

		public const ushort SUBLANG_SINDHI_PAKISTAN = 2;

		public const ushort SUBLANG_SINDHI_AFGHANISTAN = 2;

		public const ushort SUBLANG_SINHALESE_SRI_LANKA = 1;

		public const ushort SUBLANG_SOTHO_NORTHERN_SOUTH_AFRICA = 1;

		public const ushort SUBLANG_SLOVAK_SLOVAKIA = 1;

		public const ushort SUBLANG_SLOVENIAN_SLOVENIA = 1;

		public const ushort SUBLANG_SPANISH = 1;

		public const ushort SUBLANG_SPANISH_MEXICAN = 2;

		public const ushort SUBLANG_SPANISH_MODERN = 3;

		public const ushort SUBLANG_SPANISH_GUATEMALA = 4;

		public const ushort SUBLANG_SPANISH_COSTA_RICA = 5;

		public const ushort SUBLANG_SPANISH_PANAMA = 6;

		public const ushort SUBLANG_SPANISH_DOMINICAN_REPUBLIC = 7;

		public const ushort SUBLANG_SPANISH_VENEZUELA = 8;

		public const ushort SUBLANG_SPANISH_COLOMBIA = 9;

		public const ushort SUBLANG_SPANISH_PERU = 10;

		public const ushort SUBLANG_SPANISH_ARGENTINA = 11;

		public const ushort SUBLANG_SPANISH_ECUADOR = 12;

		public const ushort SUBLANG_SPANISH_CHILE = 13;

		public const ushort SUBLANG_SPANISH_URUGUAY = 14;

		public const ushort SUBLANG_SPANISH_PARAGUAY = 15;

		public const ushort SUBLANG_SPANISH_BOLIVIA = 16;

		public const ushort SUBLANG_SPANISH_EL_SALVADOR = 17;

		public const ushort SUBLANG_SPANISH_HONDURAS = 18;

		public const ushort SUBLANG_SPANISH_NICARAGUA = 19;

		public const ushort SUBLANG_SPANISH_PUERTO_RICO = 20;

		public const ushort SUBLANG_SPANISH_US = 21;

		public const ushort SUBLANG_SWAHILI_KENYA = 1;

		public const ushort SUBLANG_SWEDISH = 1;

		public const ushort SUBLANG_SWEDISH_FINLAND = 2;

		public const ushort SUBLANG_SYRIAC_SYRIA = 1;

		public const ushort SUBLANG_TAJIK_TAJIKISTAN = 1;

		public const ushort SUBLANG_TAMAZIGHT_ALGERIA_LATIN = 2;

		public const ushort SUBLANG_TAMAZIGHT_MOROCCO_TIFINAGH = 4;

		public const ushort SUBLANG_TAMIL_INDIA = 1;

		public const ushort SUBLANG_TAMIL_SRI_LANKA = 2;

		public const ushort SUBLANG_TATAR_RUSSIA = 1;

		public const ushort SUBLANG_TELUGU_INDIA = 1;

		public const ushort SUBLANG_THAI_THAILAND = 1;

		public const ushort SUBLANG_TIBETAN_PRC = 1;

		public const ushort SUBLANG_TIGRIGNA_ERITREA = 2;

		public const ushort SUBLANG_TIGRINYA_ERITREA = 2;

		public const ushort SUBLANG_TIGRINYA_ETHIOPIA = 1;

		public const ushort SUBLANG_TSWANA_BOTSWANA = 2;

		public const ushort SUBLANG_TSWANA_SOUTH_AFRICA = 1;

		public const ushort SUBLANG_TURKISH_TURKEY = 1;

		public const ushort SUBLANG_TURKMEN_TURKMENISTAN = 1;

		public const ushort SUBLANG_UIGHUR_PRC = 1;

		public const ushort SUBLANG_UKRAINIAN_UKRAINE = 1;

		public const ushort SUBLANG_UPPER_SORBIAN_GERMANY = 1;

		public const ushort SUBLANG_URDU_PAKISTAN = 1;

		public const ushort SUBLANG_URDU_INDIA = 2;

		public const ushort SUBLANG_UZBEK_LATIN = 1;

		public const ushort SUBLANG_UZBEK_CYRILLIC = 2;

		public const ushort SUBLANG_VALENCIAN_VALENCIA = 2;

		public const ushort SUBLANG_VIETNAMESE_VIETNAM = 1;

		public const ushort SUBLANG_WELSH_UNITED_KINGDOM = 1;

		public const ushort SUBLANG_WOLOF_SENEGAL = 1;

		public const ushort SUBLANG_XHOSA_SOUTH_AFRICA = 1;

		public const ushort SUBLANG_YAKUT_RUSSIA = 1;

		public const ushort SUBLANG_YI_PRC = 1;

		public const ushort SUBLANG_YORUBA_NIGERIA = 1;

		public const ushort SUBLANG_ZULU_SOUTH_AFRICA = 1;

		private static st_PrimaryLanguage[] saso_PrimaryLanguages;

		private static st_SubLanguage[] saso_SubLanguages;

		private static uint sn_PrimaryLanguages;

		private static uint sn_SubLanguages;

		private static string sauz_SerbianLatin;

		private static string sauz_SerbianCyrillic;

		private static string sauz_Croatian;

		private static string sauz_BCSCroatianCroatia;

		private static string sauz_BCSBosnianLatBH;

		private static string sauz_BCSSerbianCyrBH;

		private static string sauz_BCSSerbianLatBH;

		private static string sauz_BCSCroatianLatBH;

		private static string sauz_BCSBosnianCyrBH;

		private static string sauz_BCSSerbianLatSerbia;

		private static string sauz_BCSSerbianCyrSerbia;

		private static string sauz_BCSSerbianLatMontenegro;

		private static string sauz_BCSSerbianCyrMontenegro;

		private static string sauz_BCSBosnianCyrillic;

		private static string sauz_BCSBosnianLatin;

		private static string sauz_BCSSerbianCyrKosovo;

		private static string sauz_BCSSerbianLatKosovo;

		private static string sauz_KiswahiliChewa;

		private static string sauz_HindiBihari;

		private static string sauz_HawaiianNauru;

		private static string sauz_HawaiianSamoan;

		private static string sauz_MalayalamSundanese;

		private static string sauz_ChineseHmong;

		private static string sauz_QuechuaAymara;

		private static string sauz_BislamaBislama;

		private static string sauz_FijianFijian;

		private static string sauz_InuktitutInupiaq;

		private static Dictionary<string, uint> spco_Primaries;

		static TradosLanguage()
		{
			saso_PrimaryLanguages = new st_PrimaryLanguage[256]
			{
				new st_PrimaryLanguage(et_Primary.ec_Neutral, 0, "NE", "Neutral", 0u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Arabic, 1, "AR", "Arabic", 26u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bulgarian, 2, "BG", "Bulgarian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Catalan, 3, "CA", "Catalan", 5u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Chinese, 4, "ZH", "Chinese", 8u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Czech, 5, "CS", "Czech", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Danish, 6, "DA", "Danish", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_German, 7, "DE", "German", 6u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Greek, 8, "EL", "Greek", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_English, 9, "EN", "English", 62u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Spanish, 10, "ES", "Spanish", 26u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Finnish, 11, "FI", "Finnish", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_French, 12, "FR", "French", 46u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Hebrew, 13, "IW", "Hebrew", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Hungarian, 14, "HU", "Hungarian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Icelandic, 15, "IS", "Icelandic", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Italian, 16, "IT", "Italian", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Japanese, 17, "JA", "Japanese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Korean, 18, "KO", "Korean", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Dutch, 19, "NL", "Dutch", 7u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Norwegian, 20, "NO", "Norwegian", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Polish, 21, "PL", "Polish", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Portuguese, 22, "PT", "Portuguese", 9u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Romansh, 23, "RM", "Romansh", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Romanian, 24, "RO", "Romanian", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Russian, 25, "RU", "Russian", 6u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Serbian, 26, "SH", "Bosnian-Croatian-Serbian", 16u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Slovak, 27, "SK", "Slovak", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Albanian, 28, "SQ", "Albanian", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Swedish, 29, "SV", "Swedish", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Thai, 30, "TH", "Thai", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Turkish, 31, "TR", "Turkish", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Urdu, 32, "UR", "Urdu", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Indonesian, 33, "ID", "Indonesian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Ukrainian, 34, "UK", "Ukrainian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Byelorussian, 35, "BE", "Belarusian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Slovenian, 36, "SL", "Slovenian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Estonian, 37, "ET", "Estonian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Latvian, 38, "LV", "Latvian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Lithuanian, 39, "LT", "Lithuanian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Maori, 129, "MI", "Maori", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Persian, 41, "FA", "Persian", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Vietnamese, 42, "VI", "Vietnamese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Armenian, 43, "AM", "Armenian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Azerbaijani, 44, "AZ", "Azerbaijani", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Basque, 45, "EU", "Basque", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sorbian, 46, "SO", "Sorbian", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Macedonian, 47, "MK", "Macedonian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tsonga, 49, "TS", "Tsonga", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_isiXhosa, 52, "XH", "isiXhosa", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_isiZulu, 53, "ZU", "isiZulu", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Afrikaans, 54, "AF", "Afrikaans", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Faroese, 56, "FO", "Faroese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Maltese, 58, "MT", "Maltese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Irish, 60, "GA", "Irish", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Malay, 62, "MS", "Malay", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kazakh, 63, "KK", "Kazakh", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kyrgyz, 64, "KY", "Kyrgyz", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kiswahili, 65, "SW", "Kiswahili", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Uzbek, 67, "UZ", "Uzbek", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tatar, 68, "TT", "Tatar", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Filipino, 100, "TL", "Filipino", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Mongolian, 80, "MN", "Mongolian", 5u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Galician, 86, "GL", "Galician", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sotho, 48, "ST", "Southern Sotho", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sami, 59, "SE", "Sami", 13u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Georgian, 55, "KA", "Georgian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Hindi, 57, "HI", "Hindi", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bangla, 69, "BN", "Bangla", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Punjabi, 70, "PA", "Punjabi", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Gujarati, 71, "GU", "Gujarati", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Odia, 72, "OR", "Odia", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tamil, 73, "TA", "Tamil", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Telugu, 74, "TE", "Telugu", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kannada, 75, "KN", "Kannada", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Malayalam, 76, "ML", "Malayalam", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Assamese, 77, "AS", "Assamese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Marathi, 78, "MR", "Marathi", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sanskrit, 79, "SA", "Sanskrit", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Konkani, 87, "KOK", "Konkani", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Manipuri, 88, "MNI", "Manipuri", 0u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sindhi, 89, "SD", "Sindhi", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kashmiri, 96, "KS", "Kashmiri", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Nepali, 97, "NEP", "Nepali", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Divehi, 101, "DIV", "Divehi", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Welsh, 82, "CY", "Welsh", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Syriac, 90, "SYR", "Syriac", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_SesothoSaLeboa, 108, "NS", "Sesotho sa Leboa", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Quechua, 107, "QU", "Quechua", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Setswana, 50, "TN", "Setswana", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Greenlandic, 111, "KL", "Greenlandic", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Afar, 512, "AA", "Afar", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Aghem, 513, "AGQ", "Aghem", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Akan, 514, "AK", "Akan", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Alsatian, 132, "GSW", "Alsatian", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Amharic, 94, "AMH", "Amharic", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Asturian, 515, "AST", "Asturian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Asu, 516, "ASA", "Asu", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bafia, 517, "KSF", "Bafia", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bambara, 518, "BM", "Bambara", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Basaa, 519, "BAS", "Basaa", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bashkir, 109, "BA", "Bashkir", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bena, 520, "BEZ", "Bena", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Blin, 521, "BYN", "Blin", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bodo, 522, "BRX", "Bodo", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Breton, 126, "BR", "Breton", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Burmese, 85, "MY", "Burmese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_CentralKurdish, 146, "KU", "Central Kurdish", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Cherokee, 92, "CHR", "Cherokee", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Chiga, 523, "CGG", "Chiga", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Colognian, 524, "KSH", "Colognian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Cornish, 525, "KW", "Cornish", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Corsican, 131, "CO", "Corsican", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Dari, 140, "PRS", "Dari", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Duala, 526, "DUA", "Duala", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Dzongkha, 527, "DZ", "Dzongkha", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Embu, 528, "EBU", "Embu", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Esperanto, 529, "EO", "Esperanto", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Ewe, 530, "EE", "Ewe", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Ewondo, 531, "EWO", "Ewondo", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Frisian, 98, "FY", "Frisian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Friulian, 532, "FUR", "Friulian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Fulah, 103, "FF", "Fulah", 5u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Ganda, 533, "LG", "Ganda", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Guarani, 116, "GN", "Guarani", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Gusii, 534, "GUZ", "Gusii", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Hausa, 104, "HA", "Hausa", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Hawaiian, 117, "HAW", "Hawaiian", 3u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Igbo, 112, "IG", "Igbo", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Interlingua, 535, "IA", "Interlingua", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Inuktitut, 93, "IU", "Inuktitut", 5u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Javanese, 536, "JV", "Javanese", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_JolaFonyi, 537, "DYO", "Jola-Fonyi", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kiche, 134, "QUT", "Kiche", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kabuverdianu, 538, "KEA", "Kabuverdianu", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kabyle, 539, "KAB", "Kabyle", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kako, 540, "KKJ", "Kako", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kalenjin, 541, "KLN", "Kalenjin", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kamba, 542, "KAM", "Kamba", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Khmer, 83, "KM", "Khmer", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kikuyu, 543, "KI", "Kikuyu", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kinyarwanda, 135, "RW", "Kinyarwanda", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_KoyraChiini, 544, "KHQ", "Koyra Chiini", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_KoyraboroSenni, 545, "SES", "Koyraboro Senni", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kwasio, 546, "NMG", "Kwasio", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Lakota, 547, "LKT", "Lakota", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Langi, 548, "LAG", "Langi", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Lao, 84, "LO", "Lao", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Lingala, 549, "LN", "Lingala", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_LubaKatanga, 550, "LU", "Luba-Katanga", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Luo, 551, "LUO", "Luo", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Luxembourgish, 110, "LB", "Luxembourgish", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Luyia, 552, "LUY", "Luyia", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Machame, 553, "JMC", "Machame", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_MakhuwaMeetto, 554, "MGH", "Makhuwa-Meetto", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Makonde, 555, "KDE", "Makonde", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Malagasy, 556, "MG", "Malagasy", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Manx, 557, "GV", "Manx", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Mapudungun, 122, "ARN", "Mapudungun", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Masai, 558, "MAS", "Masai", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Meru, 559, "MER", "Meru", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Meta, 560, "MGO", "Meta", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Mohawk, 124, "MOH", "Mohawk", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Morisyen, 561, "MFE", "Morisyen", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Mundang, 562, "MUA", "Mundang", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Nko, 563, "NQO", "Nko", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Nama, 564, "NAQ", "Nama", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Ngiemboon, 565, "NNH", "Ngiemboon", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Ngomba, 566, "JGO", "Ngomba", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_NorthNdebele, 567, "ND", "North Ndebele", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Nuer, 568, "NUS", "Nuer", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Nyankole, 569, "NYN", "Nyankole", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Occitan, 130, "OC", "Occitan", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Oromo, 114, "OM", "Oromo", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Ossetian, 570, "OS", "Ossetian", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Pashto, 99, "PS", "Pashto", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Rombo, 571, "ROF", "Rombo", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Rundi, 572, "RN", "Rundi", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Rwa, 573, "RWK", "Rwa", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Saho, 574, "SSY", "Saho", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sakha, 133, "SAH", "Sakha", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Samburu, 575, "SAQ", "Samburu", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sango, 576, "SG", "Sango", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sangu, 577, "SBP", "Sangu", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_ScottishGaelic, 145, "GD", "Scottish Gaelic", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sena, 578, "SEH", "Sena", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Shambala, 579, "KSB", "Shambala", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Shona, 580, "SN", "Shona", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Sinhala, 91, "SI", "Sinhala", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Soga, 581, "XOG", "Soga", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Somali, 119, "SOM", "Somali", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_SouthNdebele, 582, "NR", "South Ndebele", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Moroccan, 583, "ZGH", "Standard Moroccan Tamazight", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Swati, 584, "SS", "Swati", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tachelhit, 585, "SHI", "Tachelhit", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Taita, 586, "DAV", "Taita", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tajik, 40, "TG", "Tajik", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tamazight, 95, "TZM", "Tamazight", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tasawaq, 587, "TWQ", "Tasawaq", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Teso, 588, "TEO", "Teso", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tibetan, 81, "BO", "Tibetan", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tigre, 589, "TIG", "Tigre", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tigrinya, 115, "TI", "Tigrinya", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tongan, 590, "TO", "Tongan", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Turkmen, 66, "TK", "Turkmen", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Uyghur, 128, "UG", "Uyghur", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Vai, 591, "VAI", "Vai", 4u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Venda, 51, "VE", "Venda", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Volapuk, 592, "VO", "Volapuk", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Vunjo, 593, "VUN", "Vunjo", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Walser, 594, "WAE", "Walser", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Wolaytta, 595, "WAL", "Wolaytta", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Wolof, 136, "WO", "Wolof", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Yangben, 596, "YAV", "Yangben", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Yi, 120, "II", "Yi", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Yiddish, 61, "YI", "Yiddish", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Yoruba, 106, "YO", "Yoruba", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Zarma, 597, "DJE", "Zarma", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_AmSignLng, 598, "ASE", "American Sign Language", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Aymara, 599, "AYC", "Aymara", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bikolano, 600, "BIK", "Bikolano", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Bislama, 601, "BIS", "Bislama", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Cakchiquel, 602, "CAK", "Cakchiquel", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Cebuano, 603, "CEB", "Cebuano", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Chuukese, 604, "CHK", "Chuukese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Efik, 605, "EFI", "Efik", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Fante, 606, "FAN", "Fante", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Fijian, 607, "FIJ", "Fijian", 2u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Gilbertese, 608, "GIL", "Gilbertese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Haitian, 609, "HAT", "Haitian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Hiligaynon, 610, "HIL", "Hiligaynon", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Hmong, 611, "MWW", "Hmong", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Iban, 612, "IBA", "Iban", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Ilokano, 613, "ILO", "Ilokano", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Inupiaq, 614, "ESI", "Inupiaq", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kekchi, 615, "KEK", "Kekchi", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Kosraean, 616, "KOS", "Kosraean", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Mam, 617, "MVC", "Mam", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Marshallese, 618, "MAH", "Marshallese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Navajo, 619, "NAV", "Navajo", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Nivacle, 620, "CAG", "Nivacle", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Palauan, 621, "PAU", "Palauan", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Pampangan, 622, "PAM", "Pampangan", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Pangasinan, 623, "PAG", "Pangasinan", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Papiamento, 624, "PAP", "Papiamento", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Pohnpeian, 625, "PON", "Pohnpeian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Rarotongan, 626, "RAR", "Rarotongan", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Samoan, 627, "SMO", "Samoan", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tahitian, 628, "TAH", "Tahitian", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_TokPisin, 629, "TPI", "Tok Pisin", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tshiluba, 630, "LUA", "Tshiluba", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Twi, 631, "TWI", "Twi", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Tzotzil, 632, "TZO", "Tzotzil", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Waray, 633, "WRZ", "Waray", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Yapese, 634, "YAP", "Yapese", 1u, 0),
				new st_PrimaryLanguage(et_Primary.ec_Yupik, 635, "ESS", "Yupik", 1u, 0)
			};
			saso_SubLanguages = new st_SubLanguage[576]
			{
				new st_SubLanguage(et_Sub.ec_SubNeutral, 0, 0, "00", "Neutral", 0u),
				new st_SubLanguage(et_Sub.ec_SubDefault, 0, 1, "01", "Default", 0u),
				new st_SubLanguage(et_Sub.ec_SubSysDefault, 0, 2, "02", "SysDefault", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicSaudiArabia, 1, 1, "SA", "Saudi Arabia", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicIraq, 1, 2, "IQ", "Iraq", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicEgypt, 1, 3, "EG", "Egypt", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicLibya, 1, 4, "LY", "Libya", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicAlgeria, 1, 5, "DZ", "Algeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicMorocco, 1, 6, "MA", "Morocco", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicTunisia, 1, 7, "TN", "Tunisia", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicOman, 1, 8, "OM", "Oman", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicYemen, 1, 9, "YE", "Yemen", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicSyria, 1, 10, "SY", "Syria", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicJordan, 1, 11, "JO", "Jordan", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicLebanon, 1, 12, "LB", "Lebanon", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicKuwait, 1, 13, "KW", "Kuwait", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicUAE, 1, 14, "AE", "U.A.E.", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicBahrain, 1, 15, "BH", "Bahrain", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicQatar, 1, 16, "QA", "Qatar", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicDjibouti, 1, 32, "DJ", "Djibouti", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicEritrea, 1, 33, "ER", "Eritrea", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicIsrael, 1, 34, "IL", "Israel", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicComoros, 1, 35, "KM", "Comoros", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicMauritania, 1, 36, "MR", "Mauritania", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicPalestina, 1, 37, "PS", "Palestinian Authority", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicSudan, 1, 38, "SD", "Sudan", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicSomalia, 1, 39, "SO", "Somalia", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicSouthSudan, 1, 40, "SS", "South Sudan", 0u),
				new st_SubLanguage(et_Sub.ec_SubArabicChad, 1, 41, "TD", "Chad", 0u),
				new st_SubLanguage(et_Sub.ec_SubChineseTrdTaiwan, 4, 1, "TW", "Traditional, Taiwan", 0u),
				new st_SubLanguage(et_Sub.ec_SubChineseSimChina, 4, 2, "CN", "Simplified, China", 0u),
				new st_SubLanguage(et_Sub.ec_SubChineseTrdHongkong, 4, 3, "HK", "Traditional, Hong Kong SAR", 0u),
				new st_SubLanguage(et_Sub.ec_SubChineseSimSingapore, 4, 4, "SG", "Simplified, Singapore", 0u),
				new st_SubLanguage(et_Sub.ec_SubChineseTrdMacao, 4, 5, "MO", "Traditional, Macao SAR", 0u),
				new st_SubLanguage(et_Sub.ec_SubChineseSimplified, 4, 30, "HANS", "Simplified", 0u),
				new st_SubLanguage(et_Sub.ec_SubChineseTraditional, 4, 31, "HANT", "Traditional", 0u),
				new st_SubLanguage(et_Sub.ec_SubChineseHmongSDL, 4, 32, "HMN-SDL", "Hmong", 0u),
				new st_SubLanguage(et_Sub.ec_SubGerman, 7, 1, "DE", "Germany", 0u),
				new st_SubLanguage(et_Sub.ec_SubGermanSwiss, 7, 2, "CH", "Switzerland", 0u),
				new st_SubLanguage(et_Sub.ec_SubGermanAustrian, 7, 3, "AT", "Austria", 0u),
				new st_SubLanguage(et_Sub.ec_SubGermanLuxembourg, 7, 4, "LU", "Luxembourg", 0u),
				new st_SubLanguage(et_Sub.ec_SubGermanLiechtenstein, 7, 5, "LI", "Liechtenstein", 0u),
				new st_SubLanguage(et_Sub.ec_SubGermanBelgium, 7, 32, "BE", "Belgium", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishUS, 9, 1, "US", "United States", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishUK, 9, 2, "GB", "United Kingdom", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishAUS, 9, 3, "AU", "Australia", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishCAN, 9, 4, "CA", "Canada", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishNZ, 9, 5, "NZ", "New Zealand", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishIreland, 9, 6, "IE", "Ireland", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishSouthAfrica, 9, 7, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishJamaica, 9, 8, "JM", "Jamaica", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishCaribbean, 9, 9, "CB", "Caribbean", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishBelize, 9, 10, "BZ", "Belize", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishTrinidad, 9, 11, "TT", "Trinidad and Tobago", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishZimbabwe, 9, 12, "ZW", "Zimbabwe", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishPhilippines, 9, 13, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishHongkong, 9, 15, "HK", "Hong Kong", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishIndia, 9, 16, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishMalaysia, 9, 17, "MY", "Malaysia", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishSingapore, 9, 18, "SG", "Singapore", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishBahamas, 9, 19, "BS", "Bahamas", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishBelgium, 9, 20, "BE", "Belgium", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishBotswana, 9, 21, "BW", "Botswana", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishBIOT, 9, 22, "IO", "British Indian Ocean Territory", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishBritishVIsl, 9, 23, "VG", "British Virgin Islands", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishCameroon, 9, 24, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishCaymanIsl, 9, 25, "KY", "Cayman Islands", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishEritrea, 9, 26, "ER", "Eritrea", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishEurope, 9, 27, "150", "Europe", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishFalklandIsl, 9, 28, "FK", "Falkland Islands", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishFiji, 9, 29, "FJ", "Fiji", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishGambia, 9, 30, "GM", "Gambia", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishGhana, 9, 31, "GH", "Ghana", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishGibraltar, 9, 32, "GI", "Gibraltar", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishGuam, 9, 33, "GU", "Guam", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishGuyana, 9, 34, "GY", "Guyana", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishKenya, 9, 35, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishLesotho, 9, 36, "LS", "Lesotho", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishLiberia, 9, 37, "LR", "Liberia", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishMacaoSAR, 9, 38, "MO", "Macao SAR", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishMadagascar, 9, 39, "MG", "Madagascar", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishMalawi, 9, 40, "MW", "Malawi", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishMalta, 9, 41, "MT", "Malta", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishMauritius, 9, 42, "MU", "Mauritius", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishNamibia, 9, 43, "NA", "Namibia", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishNigeria, 9, 44, "NG", "Nigeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishPakistan, 9, 45, "PK", "Pakistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishPapuaNG, 9, 46, "PG", "Papua New Guinea", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishPuertoRico, 9, 47, "PR", "Puerto Rico", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishRwanda, 9, 48, "RW", "Rwanda", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishSamoa, 9, 49, "WS", "Samoa", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishSeychelles, 9, 50, "SC", "Seychelles", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishSierraLeone, 9, 51, "SL", "Sierra Leone", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishSouthSudan, 9, 52, "SS", "South Sudan", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishSudan, 9, 53, "SD", "Sudan", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishSwaziland, 9, 54, "SZ", "Swaziland", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishTanzania, 9, 55, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishTokelau, 9, 56, "TK", "Tokelau", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishTonga, 9, 57, "TO", "Tonga", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishTuvalu, 9, 58, "TV", "Tuvalu", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishUganda, 9, 59, "UG", "Uganada", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishUSVirginIsl, 9, 60, "VI", "US Virgin Islands", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishVanuatu, 9, 61, "VU", "Vanuatu", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishWorld, 9, 62, "001", "World", 0u),
				new st_SubLanguage(et_Sub.ec_SubEnglishZambia, 9, 63, "ZM", "Zambia", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanish, 10, 1, "ES", "Spain", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishMexican, 10, 2, "MX", "Mexico", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishModern, 10, 3, "EM", "International Sort", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishGuatemala, 10, 4, "GT", "Guatemala", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishCostaRica, 10, 5, "CR", "Costa Rica", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishPanama, 10, 6, "PA", "Panama", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishDominicanRepublic, 10, 7, "DO", "Dominican Republic", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishVenezuela, 10, 8, "VE", "Bolivarian Republic of Venezuela", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishColombia, 10, 9, "CO", "Colombia", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishPeru, 10, 10, "PE", "Peru", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishArgentina, 10, 11, "AR", "Argentina", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishEcuador, 10, 12, "EC", "Ecuador", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishChile, 10, 13, "CL", "Chile", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishUruguay, 10, 14, "UY", "Uruguay", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishParaguay, 10, 15, "PY", "Paraguay", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishBolivia, 10, 16, "BO", "Bolivia", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishElSalvador, 10, 17, "SV", "El Salvador", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishHonduras, 10, 18, "HN", "Honduras", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishNicaragua, 10, 19, "NI", "Nicaragua", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishPuertoRico, 10, 20, "PR", "Puerto Rico", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishUSA, 10, 21, "US", "United States", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishLatinAmerica, 10, 22, "419", "Latin America", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishCuba, 10, 32, "CU", "Cuba", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishPhilippines, 10, 33, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishInternationalSDL, 10, 34, "INT-SDL", "International", 0u),
				new st_SubLanguage(et_Sub.ec_SubSpanishModernSDL, 10, 35, "MO-SDL", "Modern", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrench, 12, 1, "FR", "France", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchBelgian, 12, 2, "BE", "Belgium", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchCanadian, 12, 3, "CA", "Canada", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchSwiss, 12, 4, "CH", "Switzerland", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchLuxembourg, 12, 5, "LU", "Luxembourg", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchMonaco, 12, 6, "MC", "Monaco", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchReunion, 12, 8, "RE", "Reunion", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchCongoDRC, 12, 9, "CD", "Congo DRC", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchSenegal, 12, 10, "SN", "Senegal", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchCameroon, 12, 11, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchIvoryCoast, 12, 12, "CI", "Ivory Coast", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchMali, 12, 13, "ML", "Mali", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchMorocco, 12, 14, "MA", "Morocco", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchHaiti, 12, 15, "HT", "Haiti", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchBurkinaFaso, 12, 16, "BF", "Burkina Faso", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchBurundi, 12, 17, "BI", "Burundi", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchBenin, 12, 18, "BJ", "Benin", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchSaintBarthelemy, 12, 19, "BL", "Saint Barthelemy", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchCAR, 12, 20, "CF", "Central African Republic", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchCongo, 12, 21, "CG", "Congo", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchDjibouti, 12, 22, "DJ", "Djibouti", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchAlgeria, 12, 23, "DZ", "Algeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchGabon, 12, 24, "GA", "Gabon", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchFrenchGuiana, 12, 25, "GF", "French Guiana", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchGuinea, 12, 26, "GN", "Guinea", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchGuadeloupe, 12, 27, "GP", "Guadeloupe", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchEquatorialGuinea, 12, 28, "GQ", "Equatorial Guinea", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchComoros, 12, 29, "KM", "Comoros", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchSaintMartin, 12, 30, "MF", "Saint Martin", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchMadagascar, 12, 31, "MG", "Madagascar", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchMartinique, 12, 32, "MQ", "Martinique", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchMauritania, 12, 33, "MR", "Mauritania", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchMauritius, 12, 34, "MU", "Mauritius", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchNewCaledonia, 12, 35, "NC", "New Caledonia", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchNiger, 12, 36, "NE", "Niger", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchFrenchPolynesia, 12, 37, "PF", "French Polynesia", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchSPM, 12, 38, "PM", "Saint Pierre and Miquelon", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchRwanda, 12, 39, "RW", "Rwanda", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchSeychelles, 12, 40, "SC", "Seychelles", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchSyria, 12, 41, "SY", "Syria", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchChad, 12, 42, "TD", "Chad", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchTogo, 12, 43, "TG", "Togo", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchTunisia, 12, 44, "TN", "Tunisia", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchVanuatu, 12, 45, "VU", "Vanuatu", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchWallisFutuna, 12, 46, "WF", "Wallis and Futuna", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrenchMayotte, 12, 47, "YT", "Mayotte", 0u),
				new st_SubLanguage(et_Sub.ec_SubItalian, 16, 1, "IT", "Italy", 0u),
				new st_SubLanguage(et_Sub.ec_SubItalianSwiss, 16, 2, "CH", "Switzerland", 0u),
				new st_SubLanguage(et_Sub.ec_SubItalianSanMarino, 16, 32, "SM", "San Marino", 0u),
				new st_SubLanguage(et_Sub.ec_SubKorean, 18, 1, "KR", "Korea", 0u),
				new st_SubLanguage(et_Sub.ec_SubKoreanJohab, 18, 2, "JH", "Johab", 0u),
				new st_SubLanguage(et_Sub.ec_SubDutch, 19, 1, "NL", "Netherlands", 0u),
				new st_SubLanguage(et_Sub.ec_SubDutchBelgian, 19, 2, "BE", "Belgium", 0u),
				new st_SubLanguage(et_Sub.ec_SubDutchAruba, 19, 32, "AW", "Aruba", 0u),
				new st_SubLanguage(et_Sub.ec_SubDutchBonaireSES, 19, 33, "BQ", "Bonaire, Sint Eustatius and Saba", 0u),
				new st_SubLanguage(et_Sub.ec_SubDutchCuracao, 19, 34, "CW", "Curacao", 0u),
				new st_SubLanguage(et_Sub.ec_SubDutchSuriname, 19, 35, "SR", "Suriname", 0u),
				new st_SubLanguage(et_Sub.ec_SubDutchSintMaarten, 19, 36, "SX", "Sint Maarten", 0u),
				new st_SubLanguage(et_Sub.ec_SubNorwegianBokmal, 20, 1, "NO", "Bokmal, Norway", 0u),
				new st_SubLanguage(et_Sub.ec_SubNorwegianNynorsk, 20, 2, "NY", "Nynorsk, Norway", 0u),
				new st_SubLanguage(et_Sub.ec_SubNorwegianBokmalSJM, 20, 32, "SJ", "Bokmal, Svalbard and Jan Mayen", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortugueseBrazilian, 22, 1, "BR", "Brazil", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortuguese, 22, 2, "PT", "Portugal", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortugueseAngola, 22, 32, "AO", "Angola", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortugueseCaboVerde, 22, 33, "CV", "Cabo Verde", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortugueseGuineaBissau, 22, 34, "GW", "Guinea-Bissau", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortugueseMacaoSAR, 22, 35, "MO", "Macao SAR", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortugueseMozambique, 22, 36, "MZ", "Mozambique", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortugueseSaoTome, 22, 37, "ST", "Sao Tome and Principe", 0u),
				new st_SubLanguage(et_Sub.ec_SubPortugueseTimorLeste, 22, 38, "TL", "Timor-Leste", 0u),
				new st_SubLanguage(et_Sub.ec_SubRomanian, 24, 1, "RO", "Romania", 0u),
				new st_SubLanguage(et_Sub.ec_SubRomanianMoldavia, 24, 2, "MD", "Moldova", 0u),
				new st_SubLanguage(et_Sub.ec_SubRussian, 25, 1, "RU", "Russia", 0u),
				new st_SubLanguage(et_Sub.ec_SubRussianMoldavia, 25, 2, "MD", "Moldova", 0u),
				new st_SubLanguage(et_Sub.ec_SubRussianBelarus, 25, 32, "BY", "Belarus", 0u),
				new st_SubLanguage(et_Sub.ec_SubRussianKyrgyzstan, 25, 33, "KG", "Kyrgyzstan", 0u),
				new st_SubLanguage(et_Sub.ec_SubRussianKazakhstan, 25, 34, "KZ", "Kazakhstan", 0u),
				new st_SubLanguage(et_Sub.ec_SubRussianUkraine, 25, 35, "UA", "Ukraine", 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSCroatianCroatia, 26, 1, "HR", sauz_BCSCroatianCroatia, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianLatin, 26, 2, "SR", sauz_SerbianLatin, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianCyrillic, 26, 3, "YU", sauz_SerbianCyrillic, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSCroatianLatBH, 26, 4, "B1", sauz_BCSCroatianLatBH, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSBosnianLatBH, 26, 5, "B2", sauz_BCSBosnianLatBH, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianLatBH, 26, 6, "B3", sauz_BCSSerbianLatBH, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianCyrBH, 26, 7, "B4", sauz_BCSSerbianCyrBH, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSBosnianCyrBH, 26, 8, "CYRL-BA", sauz_BCSBosnianCyrBH, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianLatSerbia, 26, 9, "LATN-RS", sauz_BCSSerbianLatSerbia, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianCyrSerbia, 26, 10, "CYRL-RS", sauz_BCSSerbianCyrSerbia, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianLatMontenegro, 26, 11, "LATN-ME", sauz_BCSSerbianLatMontenegro, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianCyrMontenegro, 26, 12, "CYRL-ME", sauz_BCSSerbianCyrMontenegro, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSBosnianCyrillic, 26, 25, "BCY", sauz_BCSBosnianCyrillic, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSBosnianLatin, 26, 26, "BLT", sauz_BCSBosnianLatin, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianCyrKosovo, 26, 32, "CYRL-XK", sauz_BCSSerbianCyrKosovo, 0u),
				new st_SubLanguage(et_Sub.ec_SubBCSSerbianLatKosovo, 26, 33, "LATN-XK", sauz_BCSSerbianLatKosovo, 0u),
				new st_SubLanguage(et_Sub.ec_SubSwedish, 29, 1, "SE", "Sweden", 0u),
				new st_SubLanguage(et_Sub.ec_SubSwedishFinland, 29, 2, "FI", "Finland", 0u),
				new st_SubLanguage(et_Sub.ec_SubSwedishAlandIsl, 29, 32, "AX", "Aland Islands", 0u),
				new st_SubLanguage(et_Sub.ec_SubGaelicScotlandDEPRECATED2, 60, 1, "CT", "Scottish Gaelic, DEPRECATED", 0u),
				new st_SubLanguage(et_Sub.ec_SubIrishIreland, 60, 2, "IE", "Ireland", 0u),
				new st_SubLanguage(et_Sub.ec_SubGaelicScotlandDEPRECATED, 60, 3, "CT", "Scottish Gaelic, DEPRECATED", 0u),
				new st_SubLanguage(et_Sub.ec_SubMalayMalaysia, 62, 1, "MY", "Malaysia", 0u),
				new st_SubLanguage(et_Sub.ec_SubMalayBruneiDarussalam, 62, 2, "BN", "Brunei Darussalam", 0u),
				new st_SubLanguage(et_Sub.ec_SubMalayLatSingapore, 62, 32, "SG", "Latin, Singapore", 0u),
				new st_SubLanguage(et_Sub.ec_SubUzbekLatin, 67, 1, "LT", "Latin, Uzbekistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubUzbekCyrillic, 67, 2, "CY", "Cyrillic, Uzbekistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubUzbekPersoArabic, 67, 32, "ARAB", "Perso-Arabic", 0u),
				new st_SubLanguage(et_Sub.ec_SubUzbekPAAfghanistan, 67, 33, "ARAB-AF", "Perso-Arabic, Afghanistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubAzerbaijaniLatin, 44, 30, "LT", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubAzerbaijaniCyrillic, 44, 29, "CY", "Cyrillic", 0u),
				new st_SubLanguage(et_Sub.ec_SubAzerbaijaniLatAzerbaijan, 44, 1, "LATN-AZ", "Latin, Azerbaijan", 0u),
				new st_SubLanguage(et_Sub.ec_SubAzerbaijaniCyrAzerbaijan, 44, 2, "CYRL-AZ", "Cyrillic, Azerbaijan", 0u),
				new st_SubLanguage(et_Sub.ec_SubQuechuaBolivia, 107, 1, "BO", "Bolivia", 0u),
				new st_SubLanguage(et_Sub.ec_SubQuechuaEcuador, 107, 2, "EC", "Ecuador", 0u),
				new st_SubLanguage(et_Sub.ec_SubQuechuaPeru, 107, 3, "PE", "Peru", 0u),
				new st_SubLanguage(et_Sub.ec_SubQuechuaAymaraSDL, 107, 32, "AY-SDL", "Aymara", 0u),
				new st_SubLanguage(et_Sub.ec_SubAfarDjibouti, 512, 1, "DJ", "Djibouti", 0u),
				new st_SubLanguage(et_Sub.ec_SubAfarEritrea, 512, 2, "ER", "Eritrea", 0u),
				new st_SubLanguage(et_Sub.ec_SubAfarEthiopia, 512, 3, "ET", "Ethiopia", 0u),
				new st_SubLanguage(et_Sub.ec_SubAfrikaansSouthAfrica, 54, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubAfrikaansNamibia, 54, 32, "NA", "Namibia", 0u),
				new st_SubLanguage(et_Sub.ec_SubAghemCameroon, 513, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubAkanGhana, 514, 1, "GH", "Ghana", 0u),
				new st_SubLanguage(et_Sub.ec_SubAlbanianAlbania, 28, 1, "AL", "Albania", 0u),
				new st_SubLanguage(et_Sub.ec_SubAlbanianMacedonia, 28, 32, "MK", "Macedonia, FYRO", 0u),
				new st_SubLanguage(et_Sub.ec_SubAlbanianKosovo, 28, 33, "XK", "Kosovo", 0u),
				new st_SubLanguage(et_Sub.ec_SubAlsatianFrance, 132, 1, "FR", "France", 0u),
				new st_SubLanguage(et_Sub.ec_SubAlsatianSwitzerland, 132, 32, "CH", "Switzerland", 0u),
				new st_SubLanguage(et_Sub.ec_SubAlsatianLiechtenstein, 132, 33, "LI", "Liechtenstein", 0u),
				new st_SubLanguage(et_Sub.ec_SubAmharicEthiopia, 94, 1, "ET", "Ethiopia", 0u),
				new st_SubLanguage(et_Sub.ec_SubArmenianArmenia, 43, 1, "AM", "Armenia", 0u),
				new st_SubLanguage(et_Sub.ec_SubAssameseIndia, 77, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubAsturianSpain, 515, 1, "ES", "Spain", 0u),
				new st_SubLanguage(et_Sub.ec_SubAsuTanzania, 516, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubBafiaCameroon, 517, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubBambaraLatMali, 518, 1, "LATN-ML", "Latin, Mali", 0u),
				new st_SubLanguage(et_Sub.ec_SubBambaraLatin, 518, 31, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubBanglaIndia, 69, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubBanglaBangladesh, 69, 2, "BD", "Bangladesh", 0u),
				new st_SubLanguage(et_Sub.ec_SubBasaaCameroon, 519, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubBashkirRussia, 109, 1, "RU", "Russia", 0u),
				new st_SubLanguage(et_Sub.ec_SubBasqueSpain, 45, 1, "ES", "Basque", 0u),
				new st_SubLanguage(et_Sub.ec_SubBelarusianBelarus, 35, 1, "BY", "Belarus", 0u),
				new st_SubLanguage(et_Sub.ec_SubBenaTanzania, 520, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubBlinEritrea, 521, 1, "ER", "Eritrea", 0u),
				new st_SubLanguage(et_Sub.ec_SubBodoIndia, 522, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubBretonFrance, 126, 1, "FR", "France", 0u),
				new st_SubLanguage(et_Sub.ec_SubBulgarianBulgaria, 2, 1, "BG", "Bulgaria", 0u),
				new st_SubLanguage(et_Sub.ec_SubBurmeseMyanmar, 85, 1, "MM", "Myanmar", 0u),
				new st_SubLanguage(et_Sub.ec_SubCatalanCatalan, 3, 1, "ES", "Catalan", 0u),
				new st_SubLanguage(et_Sub.ec_SubCatalanValencian, 3, 2, "ES-VALE", "Valencian, Spain", 0u),
				new st_SubLanguage(et_Sub.ec_SubCatalanAndora, 3, 32, "AD", "Andora", 0u),
				new st_SubLanguage(et_Sub.ec_SubCatalanFrance, 3, 33, "FR", "France", 0u),
				new st_SubLanguage(et_Sub.ec_SubCatalanItaly, 3, 34, "IT", "Italy", 0u),
				new st_SubLanguage(et_Sub.ec_SubCentralKurdishIraq, 146, 1, "ARAB-IQ", "Iraq", 0u),
				new st_SubLanguage(et_Sub.ec_SubCentralKurdish, 146, 31, "ARAB", "Central Kurdish", 0u),
				new st_SubLanguage(et_Sub.ec_SubCherokeeUS, 92, 1, "CHE-US", "Cherokee, United States", 0u),
				new st_SubLanguage(et_Sub.ec_SubCherokeeCherokee, 92, 31, "CHER", "Cherokee", 0u),
				new st_SubLanguage(et_Sub.ec_SubChigaUganda, 523, 1, "UG", "Uganda", 0u),
				new st_SubLanguage(et_Sub.ec_SubColognianRipGermany, 524, 1, "DE", "Ripuarian, Germany", 0u),
				new st_SubLanguage(et_Sub.ec_SubCornishUK, 525, 1, "GB", "United Knigdom", 0u),
				new st_SubLanguage(et_Sub.ec_SubCorsicanFrance, 131, 1, "FR", "France", 0u),
				new st_SubLanguage(et_Sub.ec_SubCzechCzech, 5, 1, "CZ", "Czech Republic", 0u),
				new st_SubLanguage(et_Sub.ec_SubDanishDenmark, 6, 1, "DK", "Denmark", 0u),
				new st_SubLanguage(et_Sub.ec_SubDanishGreenland, 6, 32, "GL", "Greenland", 0u),
				new st_SubLanguage(et_Sub.ec_SubDariAfghanistan, 140, 1, "AF", "Afghanistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubDivehiMaldives, 101, 1, "MV", "Maldives", 0u),
				new st_SubLanguage(et_Sub.ec_SubDualaCameroon, 526, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubDzongkhaBhutan, 527, 1, "BT", "Bhutan", 0u),
				new st_SubLanguage(et_Sub.ec_SubEmbuKenya, 528, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubEsperantoWorld, 529, 1, "001", "World", 0u),
				new st_SubLanguage(et_Sub.ec_SubEstonianEstonia, 37, 1, "EE", "Estonia", 0u),
				new st_SubLanguage(et_Sub.ec_SubEweGhana, 530, 1, "GH", "Ghana", 0u),
				new st_SubLanguage(et_Sub.ec_SubEweTogo, 530, 2, "TG", "Togo", 0u),
				new st_SubLanguage(et_Sub.ec_SubEwondoCameroon, 531, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubFaroeseFaroeIsl, 56, 1, "FO", "Faroe Islands", 0u),
				new st_SubLanguage(et_Sub.ec_SubFilipinoPhilippines, 100, 1, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubFinnishFinland, 11, 1, "FI", "Finland", 0u),
				new st_SubLanguage(et_Sub.ec_SubFrisianNetherlands, 98, 1, "NL", "Netherlands", 0u),
				new st_SubLanguage(et_Sub.ec_SubFriulianItaly, 532, 1, "IT", "Friulian", 0u),
				new st_SubLanguage(et_Sub.ec_SubFulahLatSenegal, 103, 2, "LATN-SN", "Latin, Senegal", 0u),
				new st_SubLanguage(et_Sub.ec_SubFulahLatin, 103, 31, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubFulahCameroon, 103, 32, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubFulahGuinea, 103, 33, "GN", "Guinea", 0u),
				new st_SubLanguage(et_Sub.ec_SubFulahMauritania, 103, 34, "MR", "Mauritania", 0u),
				new st_SubLanguage(et_Sub.ec_SubGalicianSpain, 86, 1, "ES", "Galician", 0u),
				new st_SubLanguage(et_Sub.ec_SubGandaUganda, 533, 1, "UG", "Uganda", 0u),
				new st_SubLanguage(et_Sub.ec_SubGeorgianGeorgia, 55, 1, "GE", "Georgia", 0u),
				new st_SubLanguage(et_Sub.ec_SubGreekGreece, 8, 1, "GR", "Greece", 0u),
				new st_SubLanguage(et_Sub.ec_SubGreekCyprus, 8, 32, "CY", "Cyprus", 0u),
				new st_SubLanguage(et_Sub.ec_SubGreenlandicGreenland, 111, 1, "GL", "Greenland", 0u),
				new st_SubLanguage(et_Sub.ec_SubGuaraniParaguay, 116, 1, "PY", "Paraguay", 0u),
				new st_SubLanguage(et_Sub.ec_SubGujaratiIndia, 71, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubGusiiKenya, 534, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubHausaLatNigeria, 104, 1, "LATN-NG", "Latin, Nigeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubHausaLatin, 104, 31, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubHausaLatGhana, 104, 32, "LATN-GH", "Latin, Ghana", 0u),
				new st_SubLanguage(et_Sub.ec_SubHawaiianUS, 117, 1, "US", "United States", 0u),
				new st_SubLanguage(et_Sub.ec_SubHawaiianNauruSDL, 117, 32, "NA-SDL", "Nauru", 0u),
				new st_SubLanguage(et_Sub.ec_SubHawaiianSamoanSDL, 117, 33, "SM-SDL", "Samoan", 0u),
				new st_SubLanguage(et_Sub.ec_SubHebrewIsrael, 13, 1, "IL", "Israel", 0u),
				new st_SubLanguage(et_Sub.ec_SubHindiIndia, 57, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubHindiFiji, 57, 32, "FJ", "Fiji", 0u),
				new st_SubLanguage(et_Sub.ec_SubHindiBihariSDL, 57, 33, "BH-SDL", "Bihari", 0u),
				new st_SubLanguage(et_Sub.ec_SubHungarianHungary, 14, 1, "HU", "Hungary", 0u),
				new st_SubLanguage(et_Sub.ec_SubIcelandicIceland, 15, 1, "IS", "Iceland", 0u),
				new st_SubLanguage(et_Sub.ec_SubIgboNigeria, 112, 1, "NG", "Nigeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubIndonesianIndonesia, 33, 1, "ID", "Indonesia", 0u),
				new st_SubLanguage(et_Sub.ec_SubInterlinguaWorld, 535, 1, "001", "World", 0u),
				new st_SubLanguage(et_Sub.ec_SubInterlinguaFrance, 535, 2, "FR", "France", 0u),
				new st_SubLanguage(et_Sub.ec_SubInuktitutSylCanada, 93, 1, "CANS-CA", "Syllabics, Canada", 0u),
				new st_SubLanguage(et_Sub.ec_SubInuktitutLatCanada, 93, 2, "LATN-CA", "Latin, Canada", 0u),
				new st_SubLanguage(et_Sub.ec_SubInuktitutSyllabics, 93, 30, "CANS", "Syllabics", 0u),
				new st_SubLanguage(et_Sub.ec_SubInuktitutLatin, 93, 31, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubInuktitutInupiaqSDL, 93, 32, "IK-SDL", "Inupiaq", 0u),
				new st_SubLanguage(et_Sub.ec_SubIsiXhosaSouthAfrica, 52, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubIsiZuluSouthAfrica, 53, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubJapaneseJapan, 17, 1, "JP", "Japan", 0u),
				new st_SubLanguage(et_Sub.ec_SubJavaneseLatIndonesia, 536, 1, "LATN-ID", "Latin, Indonesia", 0u),
				new st_SubLanguage(et_Sub.ec_SubJavaneseLatin, 536, 31, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubJolaFonyiSenegal, 537, 1, "SN", "Senegal", 0u),
				new st_SubLanguage(et_Sub.ec_SubKicheGuatemala, 134, 1, "GT", "Guatemala", 0u),
				new st_SubLanguage(et_Sub.ec_SubKabuverdianuCaboVerde, 538, 1, "CV", "Cabo Verde", 0u),
				new st_SubLanguage(et_Sub.ec_SubKabyleAlgeria, 539, 1, "DZ", "Algeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubKakoCameroon, 540, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubKalenjinKenya, 541, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubKambaKenya, 542, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubKannadaIndia, 75, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubKashmiriPersoArabic, 96, 1, "ARAB", "Perso-Arabic", 0u),
				new st_SubLanguage(et_Sub.ec_SubKashmiriPAIndia, 96, 32, "ARAB-IN", "Perso-Arabic, India", 0u),
				new st_SubLanguage(et_Sub.ec_SubKazakhKazakhstan, 63, 1, "KZ", "Kazakhstan", 0u),
				new st_SubLanguage(et_Sub.ec_SubKhmerCambodia, 83, 1, "KH", "Cambodia", 0u),
				new st_SubLanguage(et_Sub.ec_SubKikuyuKenya, 543, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubKinyarwandaRwanda, 135, 1, "RW", "Rwanda", 0u),
				new st_SubLanguage(et_Sub.ec_SubKiswahiliKenya, 65, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubKiswahiliTanzania, 65, 32, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubKiswahiliUganda, 65, 33, "UG", "Uganda", 0u),
				new st_SubLanguage(et_Sub.ec_SubKiswahiliChewaSDL, 65, 34, "NY-SDL", "Chewa", 0u),
				new st_SubLanguage(et_Sub.ec_SubKonkaniIndia, 87, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubKoyraChiiniMali, 544, 1, "ML", "Mali", 0u),
				new st_SubLanguage(et_Sub.ec_SubKoyraboroSenniMali, 545, 1, "ML", "Mali", 0u),
				new st_SubLanguage(et_Sub.ec_SubKwasioCameroon, 546, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubKyrgyzKyrgyzstan, 64, 1, "KG", "Kyrgyzstan", 0u),
				new st_SubLanguage(et_Sub.ec_SubLakotaUS, 547, 1, "US", "United States", 0u),
				new st_SubLanguage(et_Sub.ec_SubLangiTanzania, 548, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubLaoLaoPDR, 84, 1, "LA", "Lao PDR", 0u),
				new st_SubLanguage(et_Sub.ec_SubLatvianLatvia, 38, 1, "LV", "Latvia", 0u),
				new st_SubLanguage(et_Sub.ec_SubLingalaAngola, 549, 1, "AO", "Angola", 0u),
				new st_SubLanguage(et_Sub.ec_SubLingalaCongoDRC, 549, 2, "CD", "Congo DRC", 0u),
				new st_SubLanguage(et_Sub.ec_SubLingalaCAR, 549, 3, "CF", "Central African Republic", 0u),
				new st_SubLanguage(et_Sub.ec_SubLingalaCongo, 549, 4, "CG", "Congo", 0u),
				new st_SubLanguage(et_Sub.ec_SubLithuanianLithuania, 39, 1, "LT", "Lithuania", 0u),
				new st_SubLanguage(et_Sub.ec_SubSorbianUpperGermany, 46, 1, "HDE", "Upper, Germany", 0u),
				new st_SubLanguage(et_Sub.ec_SubSorbianLowerGermany, 46, 2, "DDE", "Lower, Germany", 0u),
				new st_SubLanguage(et_Sub.ec_SubLubaKatangaCongoDRC, 550, 1, "CD", "Congo DRC", 0u),
				new st_SubLanguage(et_Sub.ec_SubLuoKenya, 551, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubLuxembourgishLuxembourg, 110, 1, "LU", "Luxembourg", 0u),
				new st_SubLanguage(et_Sub.ec_SubLuyiaKenya, 552, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubMacedonianMacedonia, 47, 1, "MK", "Macedonia", 0u),
				new st_SubLanguage(et_Sub.ec_SubMachameTanzania, 553, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubMakhuwaMeettoMozambique, 554, 1, "MZ", "Mozambique", 0u),
				new st_SubLanguage(et_Sub.ec_SubMakondeTanzania, 555, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubMalagasyMadagascar, 556, 1, "MG", "Madagascar", 0u),
				new st_SubLanguage(et_Sub.ec_SubMalayalamIndia, 76, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubMalayalamSundaneseSDL, 76, 32, "SU-SDL", "Roman, Sundanese", 0u),
				new st_SubLanguage(et_Sub.ec_SubMalteseMalta, 58, 1, "MT", "Malta", 0u),
				new st_SubLanguage(et_Sub.ec_SubManxIsleOfMan, 557, 1, "IM", "Isle of Man", 0u),
				new st_SubLanguage(et_Sub.ec_SubMaoriNewZealand, 129, 1, "NZ", "New Zealand", 0u),
				new st_SubLanguage(et_Sub.ec_SubMapudungunChile, 122, 1, "CL", "Chile", 0u),
				new st_SubLanguage(et_Sub.ec_SubMarathiIndia, 78, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubMasaiKenya, 558, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubMasaiTanzania, 558, 2, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubMeruKenya, 559, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubMetaCameroon, 560, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubMohawkCanada, 124, 1, "CA", "Mohawk", 0u),
				new st_SubLanguage(et_Sub.ec_SubMongolianCyrMongolia, 80, 1, "MN", "Cyrillic, Mongolia", 0u),
				new st_SubLanguage(et_Sub.ec_SubMongolianTrdChina, 80, 2, "MONG-CN", "Traditional, China", 0u),
				new st_SubLanguage(et_Sub.ec_SubMongolianTrdMongolia, 80, 3, "MONG-MN", "Traditional, Mongolia", 0u),
				new st_SubLanguage(et_Sub.ec_SubMongolianCyrillic, 80, 30, "CYRL", "Cyrillic", 0u),
				new st_SubLanguage(et_Sub.ec_SubMongolianTraditional, 80, 31, "MONG", "Traditional", 0u),
				new st_SubLanguage(et_Sub.ec_SubMorisyenMauritius, 561, 1, "MU", "Mauritius", 0u),
				new st_SubLanguage(et_Sub.ec_SubMundangCameroon, 562, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubNkoGuinea, 563, 1, "GN", "Guinea", 0u),
				new st_SubLanguage(et_Sub.ec_SubNamaNamibia, 564, 1, "NA", "Namibia", 0u),
				new st_SubLanguage(et_Sub.ec_SubNgiemboonCameroon, 565, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubNgombaCameroon, 566, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubNepaliNepal, 97, 1, "NP", "Nepal", 0u),
				new st_SubLanguage(et_Sub.ec_SubNepaliIndia, 97, 2, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubNorthNdebeleZimbabwe, 567, 1, "ZW", "Zimbabwe", 0u),
				new st_SubLanguage(et_Sub.ec_SubNuerSudan, 568, 1, "SD", "Sudan", 0u),
				new st_SubLanguage(et_Sub.ec_SubNyankoleUganda, 569, 1, "UG", "Uganda", 0u),
				new st_SubLanguage(et_Sub.ec_SubOccitanFrance, 130, 1, "FR", "France", 0u),
				new st_SubLanguage(et_Sub.ec_SubOdiaIndia, 72, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubOromoEthiopia, 114, 1, "ET", "Ethiopia", 0u),
				new st_SubLanguage(et_Sub.ec_SubOromoKenya, 114, 32, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubOssetianCyrGeorgia, 570, 1, "GE", "Cyrillic, Georgia", 0u),
				new st_SubLanguage(et_Sub.ec_SubOssetianCyrRussia, 570, 2, "RU", "Cyrillic, Russia", 0u),
				new st_SubLanguage(et_Sub.ec_SubPashtoAfghanistan, 99, 1, "AF", "Afghanistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubPersianIran, 41, 1, "IR", "Iran", 0u),
				new st_SubLanguage(et_Sub.ec_SubPersianAfghanistan, 41, 32, "AF", "Afghanistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubPolishPoland, 21, 1, "PL", "Poland", 0u),
				new st_SubLanguage(et_Sub.ec_SubPunjabiIndia, 70, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubPunjabiPakistan, 70, 2, "ARAB-PK", "Pakistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubPunjabiPunjabi, 70, 31, "ARAB", "Punjabi", 0u),
				new st_SubLanguage(et_Sub.ec_SubRomanshSwitzerland, 23, 1, "CH", "Switzerland", 0u),
				new st_SubLanguage(et_Sub.ec_SubRomboTanzania, 571, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubRundiBurundi, 572, 1, "BI", "Burundi", 0u),
				new st_SubLanguage(et_Sub.ec_SubRwaTanzania, 573, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubSahoEritrea, 574, 1, "ER", "Eritrea", 0u),
				new st_SubLanguage(et_Sub.ec_SubSakhaRussia, 133, 1, "RU", "Russia", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamburuKenya, 575, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiNorthernNorway, 59, 1, "NO", "Northern, Norway", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiNorthernSweden, 59, 2, "SE", "Northern, Sweden", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiNorthernFinland, 59, 3, "FI", "Northern, Finland", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiLuleNorway, 59, 4, "JNO", "Lule, Norway", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiLuleSweden, 59, 5, "JSE", "Lule, Sweden", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiSouthernNorway, 59, 6, "ANO", "Southern, Norway", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiSouthernSweden, 59, 7, "ASE", "Southern, Sweden", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiSkoltFinland, 59, 8, "SFI", "Skolt, Finland", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiInariFinland, 59, 9, "NFI", "Inari, Finland", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiInari, 59, 28, "SMN", "Inari", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiSkolt, 59, 29, "SMS", "Skolt", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiSouthern, 59, 30, "SMA", "Southern", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamiLule, 59, 31, "SMJ", "Lule", 0u),
				new st_SubLanguage(et_Sub.ec_SubSangoCAR, 576, 1, "CF", "Central African Republic", 0u),
				new st_SubLanguage(et_Sub.ec_SubSanguTanzania, 577, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubSanskritIndia, 79, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubScottishGaelicUK, 145, 1, "GB", "United Kingdom", 0u),
				new st_SubLanguage(et_Sub.ec_SubSenaMozambique, 578, 1, "MZ", "Mozambique", 0u),
				new st_SubLanguage(et_Sub.ec_SubSesothoSaLeboaSouthAfrica, 108, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubSetswanaSouthAfrica, 50, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubSetswanaBotswana, 50, 2, "BW", "Botswana", 0u),
				new st_SubLanguage(et_Sub.ec_SubShambalaTanzania, 579, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubShonaLatZimbabwe, 580, 1, "LATN-ZW", "Latin, Zimbabwe", 0u),
				new st_SubLanguage(et_Sub.ec_SubShonaLatin, 580, 2, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubSindhiPakistan, 89, 2, "ARAB-PK", "Pakistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubSindhiSindhi, 89, 31, "ARAB", "Sindhi", 0u),
				new st_SubLanguage(et_Sub.ec_SubSinhalaSriLanka, 91, 1, "LK", "Sri Lanka", 0u),
				new st_SubLanguage(et_Sub.ec_SubSlovakSlovakia, 27, 1, "SK", "Slovakia", 0u),
				new st_SubLanguage(et_Sub.ec_SubSlovenianSlovenia, 36, 1, "SI", "Slovenia", 0u),
				new st_SubLanguage(et_Sub.ec_SubSogaUganda, 581, 1, "UG", "Uganda", 0u),
				new st_SubLanguage(et_Sub.ec_SubSomaliSomalia, 119, 1, "SO", "Somalia", 0u),
				new st_SubLanguage(et_Sub.ec_SubSomaliDjibouti, 119, 32, "DJ", "Djibouti", 0u),
				new st_SubLanguage(et_Sub.ec_SubSomaliEthiopia, 119, 33, "ET", "Ethiopia", 0u),
				new st_SubLanguage(et_Sub.ec_SubSomaliKenya, 119, 34, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubSothoSouthAfrica, 48, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubSothoLesotho, 48, 32, "LS", "Lesotho", 0u),
				new st_SubLanguage(et_Sub.ec_SubSouthNdebeleSouthAfrica, 582, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubMoroccanTifMorocco, 583, 1, "TFN-MA", "Tifinagh, Morocco", 0u),
				new st_SubLanguage(et_Sub.ec_SubMoroccanTifinagh, 583, 2, "TFNG", "Tifinagh", 0u),
				new st_SubLanguage(et_Sub.ec_SubSwatiSwaziland, 584, 1, "SZ", "Swaziland", 0u),
				new st_SubLanguage(et_Sub.ec_SubSwatiSouthAfrica, 584, 2, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubSyriacSyria, 90, 1, "SY", "Syria", 0u),
				new st_SubLanguage(et_Sub.ec_TachelhitLatMorocco, 585, 1, "LAT-MA", "Latin, Morocco", 0u),
				new st_SubLanguage(et_Sub.ec_TachelhitTifMorocco, 585, 2, "TFN-MA", "Tifinagh, Morocco", 0u),
				new st_SubLanguage(et_Sub.ec_TachelhitLatin, 585, 30, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_TachelhitTifinagh, 585, 31, "TFNG", "Tifinagh", 0u),
				new st_SubLanguage(et_Sub.ec_SubTaitaKenya, 586, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubTajikCyrTajikistan, 40, 1, "CYRL-TJ", "Cyrillic, Tajikistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubTajikCyrillic, 40, 31, "CYRL", "Cyrillic", 0u),
				new st_SubLanguage(et_Sub.ec_SubTamazightLatAlgeria, 95, 2, "LAT-DZ", "Latin, Algeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubTamazightTifMorocco, 95, 4, "TFN-MA", "Tifinagh, Morocco", 0u),
				new st_SubLanguage(et_Sub.ec_SubTamazightTifinagh, 95, 30, "TFNG", "Tifinagh", 0u),
				new st_SubLanguage(et_Sub.ec_SubTamazightLatin, 95, 31, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubTamilIndia, 73, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubTamilSriLanka, 73, 2, "LK", "Sri Lanka", 0u),
				new st_SubLanguage(et_Sub.ec_SubTamilMalaysia, 73, 32, "MY", "Malaysia", 0u),
				new st_SubLanguage(et_Sub.ec_SubTamilSingapore, 73, 33, "SG", "Singapore", 0u),
				new st_SubLanguage(et_Sub.ec_SubTasawaqNiger, 587, 1, "NE", "Niger", 0u),
				new st_SubLanguage(et_Sub.ec_SubTatarRussia, 68, 1, "RU", "Russia", 0u),
				new st_SubLanguage(et_Sub.ec_SubTeluguIndia, 74, 1, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubTesoKenya, 588, 1, "KE", "Kenya", 0u),
				new st_SubLanguage(et_Sub.ec_SubTesoUganda, 588, 2, "UG", "Uganda", 0u),
				new st_SubLanguage(et_Sub.ec_SubThaiThailand, 30, 1, "TH", "Thailand", 0u),
				new st_SubLanguage(et_Sub.ec_SubTibetanChina, 81, 1, "CN", "China", 0u),
				new st_SubLanguage(et_Sub.ec_SubTibetanIndia, 81, 32, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubTigreEritrea, 589, 1, "ER", "Eritrea", 0u),
				new st_SubLanguage(et_Sub.ec_SubTigrinyaEthiopia, 115, 1, "ET", "Ethiopia", 0u),
				new st_SubLanguage(et_Sub.ec_SubTigrinyaEritrea, 115, 2, "ER", "Eritrea", 0u),
				new st_SubLanguage(et_Sub.ec_SubTonganTonga, 590, 1, "TO", "Tonga", 0u),
				new st_SubLanguage(et_Sub.ec_SubTsongaSouthAfrica, 49, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubTurkishTurkey, 31, 1, "TR", "Turkey", 0u),
				new st_SubLanguage(et_Sub.ec_SubTurkishCyprus, 31, 32, "CY", "Cyprus", 0u),
				new st_SubLanguage(et_Sub.ec_SubTurkmenTurkmenistan, 66, 1, "TM", "Turkmenistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubUkrainianUkraine, 34, 1, "UA", "Ukraine", 0u),
				new st_SubLanguage(et_Sub.ec_SubUrduPakistan, 32, 1, "PK", "Pakistan", 0u),
				new st_SubLanguage(et_Sub.ec_SubUrduIndia, 32, 2, "IN", "India", 0u),
				new st_SubLanguage(et_Sub.ec_SubUyghurChina, 128, 1, "CN", "China", 0u),
				new st_SubLanguage(et_Sub.ec_SubVaiLatin, 591, 1, "LATN", "Latin", 0u),
				new st_SubLanguage(et_Sub.ec_SubVaiLatLiberia, 591, 2, "LAT-LR", "Latin, Liberia", 0u),
				new st_SubLanguage(et_Sub.ec_SubVaiVai, 591, 3, "VAII", "Vai", 0u),
				new st_SubLanguage(et_Sub.ec_SubVaiVaiLiberia, 591, 4, "VAI-LR", "Vai, Liberia", 0u),
				new st_SubLanguage(et_Sub.ec_SubVendaSouthAfrica, 51, 1, "ZA", "South Africa", 0u),
				new st_SubLanguage(et_Sub.ec_SubVietnameseVietnam, 42, 1, "VN", "Vietnam", 0u),
				new st_SubLanguage(et_Sub.ec_SubVolapukWorld, 592, 1, "001", "World", 0u),
				new st_SubLanguage(et_Sub.ec_SubVunjoTanzania, 593, 1, "TZ", "Tanzania", 0u),
				new st_SubLanguage(et_Sub.ec_SubWalserSwitzerland, 594, 1, "CH", "Switzerland", 0u),
				new st_SubLanguage(et_Sub.ec_SubWelshUK, 82, 1, "GB", "United Kingdom", 0u),
				new st_SubLanguage(et_Sub.ec_SubWolayttaEthiopia, 595, 1, "ET", "Ethiopia", 0u),
				new st_SubLanguage(et_Sub.ec_SubWolofSenegal, 136, 1, "SN", "Senegal", 0u),
				new st_SubLanguage(et_Sub.ec_SubYangbenCameroon, 596, 1, "CM", "Cameroon", 0u),
				new st_SubLanguage(et_Sub.ec_SubYiChina, 120, 1, "CN", "China", 0u),
				new st_SubLanguage(et_Sub.ec_SubYiddishWorld, 61, 32, "001", "World", 0u),
				new st_SubLanguage(et_Sub.ec_SubYorubaNigeria, 106, 1, "NG", "Nigeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubYorubaBenin, 106, 32, "BJ", "Benin", 0u),
				new st_SubLanguage(et_Sub.ec_SubZarmaNiger, 597, 1, "NE", "Niger", 0u),
				new st_SubLanguage(et_Sub.ec_SubAmSignLngUS, 598, 1, "US", "United States", 0u),
				new st_SubLanguage(et_Sub.ec_SubAymaraPeru, 599, 1, "PE", "Peru", 0u),
				new st_SubLanguage(et_Sub.ec_SubBikolanoPhilippines, 600, 1, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubBislamaVanuatu, 601, 1, "VU", "Vanuatu", 0u),
				new st_SubLanguage(et_Sub.ec_SubBislamaBislamaSDL, 601, 2, "BI-SDL", "Bislama", 0u),
				new st_SubLanguage(et_Sub.ec_SubCakchiquelGuatemala, 602, 1, "GT", "Guatemala", 0u),
				new st_SubLanguage(et_Sub.ec_SubCebuanoPhilippines, 603, 1, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubChuukeseMicronesia, 604, 1, "FM", "Micronesia", 0u),
				new st_SubLanguage(et_Sub.ec_SubEfikNigeria, 605, 1, "NG", "Nigeria", 0u),
				new st_SubLanguage(et_Sub.ec_SubFanteGhana, 606, 1, "GH", "Ghana", 0u),
				new st_SubLanguage(et_Sub.ec_SubFijianFiji, 607, 1, "FJ", "Fiji", 0u),
				new st_SubLanguage(et_Sub.ec_SubFijianFijianSDL, 607, 2, "FJ-SDL", "Fijian", 0u),
				new st_SubLanguage(et_Sub.ec_SubGilberteseKiribati, 608, 1, "KI", "Kiribati", 0u),
				new st_SubLanguage(et_Sub.ec_SubHaitianHaiti, 609, 1, "HT", "Haiti", 0u),
				new st_SubLanguage(et_Sub.ec_SubHiligaynonPhilippines, 610, 1, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubHmongChina, 611, 1, "CN", "China", 0u),
				new st_SubLanguage(et_Sub.ec_SubIbanMalaysia, 612, 1, "MY", "Malaysia", 0u),
				new st_SubLanguage(et_Sub.ec_SubIlokanoPhilippines, 613, 1, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubInupiaqUS, 614, 1, "US", "United States", 0u),
				new st_SubLanguage(et_Sub.ec_SubKekchiGuatemala, 615, 1, "GT", "Guatemala", 0u),
				new st_SubLanguage(et_Sub.ec_SubKosraeanMicronesia, 616, 1, "FM", "Micronesia", 0u),
				new st_SubLanguage(et_Sub.ec_SubMamGuatemala, 617, 1, "GT", "Guatemala", 0u),
				new st_SubLanguage(et_Sub.ec_SubMarshalleseMarshallIsl, 618, 1, "MH", "Marshall Islands", 0u),
				new st_SubLanguage(et_Sub.ec_SubNavajoUS, 619, 1, "US", "United States", 0u),
				new st_SubLanguage(et_Sub.ec_SubNivacleParaguay, 620, 1, "PY", "Paraguay", 0u),
				new st_SubLanguage(et_Sub.ec_SubPalauanPalau, 621, 1, "PW", "Palau", 0u),
				new st_SubLanguage(et_Sub.ec_SubPampanganPhilippines, 622, 1, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubPangasinanPhilippines, 623, 1, "PH", "Philippines", 0u),
				new st_SubLanguage(et_Sub.ec_SubPapiamentoDutchAntilles, 624, 1, "AN", "Dutch Antilles", 0u),
				new st_SubLanguage(et_Sub.ec_SubPohnpeianMicronesia, 625, 1, "FM", "Micronesia", 0u),
				new st_SubLanguage(et_Sub.ec_SubRarotonganCookIsl, 626, 1, "CK", "Cook Islands", 0u),
				new st_SubLanguage(et_Sub.ec_SubSamoanWesternSamoa, 627, 1, "WS", "Western Samoa", 0u),
				new st_SubLanguage(et_Sub.ec_SubTahitianFrenchPolynesia, 628, 1, "PF", "French Polynesia", 0u),
				new st_SubLanguage(et_Sub.ec_SubTokPisinPapuaNG, 629, 1, "PG", "Papua New Guinea", 0u),
				new st_SubLanguage(et_Sub.ec_SubTshilubaCongoDRC, 630, 1, "CD", "Congo DRC", 0u),
				new st_SubLanguage(et_Sub.ec_SubTwiGhana, 631, 1, "GH", "Ghana", 0u),
				new st_SubLanguage(et_Sub.ec_SubTzotzilMexico, 632, 1, "MX", "Mexico", 0u),
				new st_SubLanguage(et_Sub.ec_SubWarayAustralia, 633, 1, "AU", "Australia", 0u),
				new st_SubLanguage(et_Sub.ec_SubYapeseMicronesia, 634, 1, "FM", "Micronesia", 0u),
				new st_SubLanguage(et_Sub.ec_SubYupikUS, 635, 1, "US", "United States", 0u)
			};
			sn_PrimaryLanguages = (uint)saso_PrimaryLanguages.Length;
			sn_SubLanguages = (uint)saso_SubLanguages.Length;
			sauz_SerbianLatin = "Serbian (Latin)";
			sauz_SerbianCyrillic = "Serbian (Cyrillic)";
			sauz_Croatian = "Croatian";
			sauz_BCSCroatianCroatia = "Croatian (Croatia)";
			sauz_BCSBosnianLatBH = "Bosnian (Latin, Bosnia and Herzegovina)";
			sauz_BCSSerbianCyrBH = "Serbian (Cyrillic, Bosnia and Herzegovina)";
			sauz_BCSSerbianLatBH = "Serbian (Latin, Bosnia and Herzegovina)";
			sauz_BCSCroatianLatBH = "Croatian (Latin, Bosnia and Herzegovina)";
			sauz_BCSBosnianCyrBH = "Bosnian (Cyrillic, Bosnia and Herzegovina)";
			sauz_BCSSerbianLatSerbia = "Serbian (Latin, Serbia)";
			sauz_BCSSerbianCyrSerbia = "Serbian (Cyrillic, Serbia)";
			sauz_BCSSerbianLatMontenegro = "Serbian (Latin, Montenegro)";
			sauz_BCSSerbianCyrMontenegro = "Serbian (Cyrillic, Montenegro)";
			sauz_BCSBosnianCyrillic = "Bosnian (Cyrillic)";
			sauz_BCSBosnianLatin = "Bosnian (Latin)";
			sauz_BCSSerbianCyrKosovo = "Serbian (Cyrillic, Kosovo)";
			sauz_BCSSerbianLatKosovo = "Serbian (Latin, Kosovo)";
			sauz_KiswahiliChewa = "Chewa";
			sauz_HindiBihari = "Bihari";
			sauz_HawaiianNauru = "Nauru";
			sauz_HawaiianSamoan = "Samoan";
			sauz_MalayalamSundanese = "Sundanese (Roman)";
			sauz_ChineseHmong = "Hmong";
			sauz_QuechuaAymara = "Aymara";
			sauz_BislamaBislama = "Bislama";
			sauz_FijianFijian = "Fijian";
			sauz_InuktitutInupiaq = "Inupiaq";
			spco_Primaries = new Dictionary<string, uint>();
			for (uint num = 0u; num < sn_PrimaryLanguages; num++)
			{
				spco_Primaries[saso_PrimaryLanguages[num].qcuz_LanguageCode] = num;
			}
			for (uint num = 0u; num < sn_PrimaryLanguages; num++)
			{
				saso_PrimaryLanguages[num].n_SubLanguages = 0u;
				saso_PrimaryLanguages[num].i_SubLanguagesOffset = -1;
				for (uint num2 = 0u; num2 < sn_SubLanguages; num2++)
				{
					if (saso_SubLanguages[num2].n2_PrimaryLanguageID == saso_PrimaryLanguages[num].n2_PrimaryLanguageID)
					{
						saso_PrimaryLanguages[num].i_SubLanguagesOffset = (int)num2;
						uint num3 = 0u;
						while (num2 < sn_SubLanguages && saso_SubLanguages[num2].n2_PrimaryLanguageID == saso_PrimaryLanguages[num].n2_PrimaryLanguageID)
						{
							saso_SubLanguages[num2].n_PrimaryLanguageOffset = num;
							num2++;
							num3++;
						}
						saso_PrimaryLanguages[num].n_SubLanguages = num3;
						break;
					}
				}
			}
		}

		private static uint MAKELANGID(uint p, uint s)
		{
			return (s << 10) | p;
		}

		private static uint PRIMARYLANGID(uint lcid)
		{
			return lcid & 0x3FF;
		}

		private static uint SUBLANGID(uint lcid)
		{
			return lcid >> 10;
		}

		private static uint LANGIDFROMLCID(uint lcid)
		{
			return lcid & 0xFFFF;
		}

		public static int[] GetAllLcids()
		{
			List<int> list = new List<int>();
			for (int i = 1; i <= GetPrimaryCount(); i++)
			{
				et_Primary eo_primary = (et_Primary)i;
				uint subCount = GetSubCount(eo_primary);
				for (uint num = (subCount != 0) ? 1u : 0u; num <= subCount; num++)
				{
					et_Sub subAt = GetSubAt(eo_primary, num);
					uint item = MakeLanguageID(eo_primary, subAt);
					list.Add((int)item);
				}
			}
			return list.ToArray();
		}

		public static uint MakeLanguageID(et_Primary eo_primary, et_Sub eo_sub)
		{
			int num = FindPrimaryRecord(eo_primary);
			if (num < 0)
			{
				return MAKELANGID(0u, 0u);
			}
			if (eo_sub == et_Sub.ec_SubNeutral || eo_sub == et_Sub.ec_SubDefault || eo_sub == et_Sub.ec_SubSysDefault)
			{
				return MAKELANGID(saso_PrimaryLanguages[num].n2_PrimaryLanguageID, (ushort)eo_sub);
			}
			if (saso_PrimaryLanguages[num].n_SubLanguages == 0)
			{
				return MAKELANGID(saso_PrimaryLanguages[num].n2_PrimaryLanguageID, 1u);
			}
			int num2 = FindSubRecord(saso_PrimaryLanguages[num].i_SubLanguagesOffset, eo_sub);
			if (num2 < 0)
			{
				return MAKELANGID(saso_PrimaryLanguages[num].n2_PrimaryLanguageID, 1u);
			}
			return MAKELANGID(saso_SubLanguages[num2].n2_PrimaryLanguageID, saso_SubLanguages[num2].n2_SubLanguageID);
		}

		public static void GetLanguages(uint n_locale, out et_Primary peo_primary, out et_Sub peo_sub)
		{
			uint lcid = LANGIDFROMLCID(n_locale);
			uint n_primaryLanguageId = PRIMARYLANGID(lcid);
			uint n_subLangId = SUBLANGID(lcid);
			int num = FindPrimaryRecord(n_primaryLanguageId);
			if (num < 0)
			{
				peo_primary = et_Primary.ec_Neutral;
				peo_sub = et_Sub.ec_SubNeutral;
				return;
			}
			peo_primary = saso_PrimaryLanguages[num].eo_Primary;
			int num2 = FindSubRecord(saso_PrimaryLanguages[num].i_SubLanguagesOffset, n_subLangId);
			if (num2 < 0)
			{
				peo_sub = ((saso_PrimaryLanguages[num].n_SubLanguages == 0) ? et_Sub.ec_SubDefault : et_Sub.ec_SubNeutral);
			}
			else
			{
				peo_sub = saso_SubLanguages[num2].eo_Sub;
			}
		}

		public static et_Primary GetPrimary(uint n_locale)
		{
			GetLanguages(n_locale, out et_Primary peo_primary, out et_Sub _);
			return peo_primary;
		}

		public static et_Sub GetSub(uint n_locale)
		{
			GetLanguages(n_locale, out et_Primary _, out et_Sub peo_sub);
			return peo_sub;
		}

		public static uint GetPrimaryCount()
		{
			return sn_PrimaryLanguages - 1;
		}

		public static uint GetSubCount(et_Primary eo_primary)
		{
			int num = FindPrimaryRecord(eo_primary);
			if (num < 0)
			{
				return 0u;
			}
			return saso_PrimaryLanguages[num].n_SubLanguages;
		}

		public static et_Sub GetSubAt(et_Primary eo_primary, uint index)
		{
			if (index == 0)
			{
				return et_Sub.ec_SubDefault;
			}
			int num = FindPrimaryRecord(eo_primary);
			if (num < 0)
			{
				return et_Sub.ec_SubNeutral;
			}
			uint n_SubLanguages = saso_PrimaryLanguages[num].n_SubLanguages;
			if (n_SubLanguages == 0 || index > n_SubLanguages)
			{
				return et_Sub.ec_SubDefault;
			}
			return saso_SubLanguages[saso_PrimaryLanguages[num].i_SubLanguagesOffset + index - 1].eo_Sub;
		}

		public static string GetLanguageCode(et_Primary eo_primary)
		{
			int num = FindPrimaryRecord(eo_primary);
			if (num >= 0)
			{
				return saso_PrimaryLanguages[num].qcuz_LanguageCode;
			}
			return null;
		}

		public static string TryGetLanguageCode(et_Primary eo_primary)
		{
			if (eo_primary < et_Primary.ec_Arabic || eo_primary > et_Primary.ec_Yupik)
			{
				return null;
			}
			int num = FindPrimaryRecord(eo_primary);
			if (num >= 0)
			{
				return saso_PrimaryLanguages[num].qcuz_LanguageCode;
			}
			return null;
		}

		public static int[] GetLcids(et_Primary eo_primary)
		{
			uint subCount = GetSubCount(eo_primary);
			if (subCount == 0)
			{
				return new int[1]
				{
					(int)MakeLanguageID(eo_primary, et_Sub.ec_SubDefault)
				};
			}
			int[] array = new int[subCount];
			for (int i = 1; i <= subCount; i++)
			{
				array[i - 1] = (int)MakeLanguageID(eo_primary, GetSubAt(eo_primary, (uint)i));
			}
			return array;
		}

		public static string GetLanguage(et_Primary eo_primary)
		{
			int num = FindPrimaryRecord(eo_primary);
			if (num >= 0)
			{
				return saso_PrimaryLanguages[num].qcuz_Language;
			}
			return null;
		}

		public static string GetCountryCode(et_Sub eo_sub)
		{
			int num = FindSubRecord(0, eo_sub);
			if (num >= 0)
			{
				return saso_SubLanguages[num].qcuz_CountryCode;
			}
			return null;
		}

		public static string GetCountry(et_Sub eo_sub)
		{
			int num = FindSubRecord(0, eo_sub);
			if (num >= 0)
			{
				return saso_SubLanguages[num].qcuz_Country;
			}
			return null;
		}

		public static bool GetPrimary(out et_Primary peo_primary, string code)
		{
			if (spco_Primaries.TryGetValue(code, out uint value))
			{
				peo_primary = saso_PrimaryLanguages[value].eo_Primary;
				return true;
			}
			if (code.Equals("he", StringComparison.OrdinalIgnoreCase))
			{
				peo_primary = et_Primary.ec_Hebrew;
				return true;
			}
			peo_primary = et_Primary.ec_Neutral;
			return false;
		}

		public static bool GetPrimaryLanguage(out et_Primary peo_primary, string qcuz_language)
		{
			for (uint num = 0u; num < sn_PrimaryLanguages; num++)
			{
				if (saso_PrimaryLanguages[num].qcuz_Language == qcuz_language)
				{
					peo_primary = saso_PrimaryLanguages[num].eo_Primary;
					return true;
				}
			}
			peo_primary = et_Primary.ec_Neutral;
			return false;
		}

		public static bool GetSubCountry(out et_Sub peo_sub, string qcuz_country, et_Primary eo_primary)
		{
			int num = FindPrimaryRecord(eo_primary);
			if (num < 0 || saso_PrimaryLanguages[num].n_SubLanguages == 0)
			{
				peo_sub = et_Sub.ec_SubNeutral;
				return false;
			}
			for (int i = saso_PrimaryLanguages[num].i_SubLanguagesOffset; i < (int)sn_SubLanguages && saso_SubLanguages[i].n2_PrimaryLanguageID == saso_PrimaryLanguages[num].n2_PrimaryLanguageID; i++)
			{
				if (qcuz_country == saso_SubLanguages[i].qcuz_Country)
				{
					peo_sub = saso_SubLanguages[i].eo_Sub;
					return true;
				}
			}
			peo_sub = et_Sub.ec_SubNeutral;
			return false;
		}

		public static string MakeLanguageString(et_Primary eo_primary, et_Sub eo_sub)
		{
			if (eo_primary == et_Primary.ec_Serbian)
			{
				switch (eo_sub)
				{
				case et_Sub.ec_SubBCSSerbianLatin:
					return sauz_SerbianLatin;
				case et_Sub.ec_SubBCSSerbianCyrillic:
					return sauz_SerbianCyrillic;
				case et_Sub.ec_SubBCSCroatianCroatia:
					return sauz_BCSCroatianCroatia;
				case et_Sub.ec_SubBCSBosnianLatBH:
					return sauz_BCSBosnianLatBH;
				case et_Sub.ec_SubBCSSerbianCyrBH:
					return sauz_BCSSerbianCyrBH;
				case et_Sub.ec_SubBCSSerbianLatBH:
					return sauz_BCSSerbianLatBH;
				case et_Sub.ec_SubBCSCroatianLatBH:
					return sauz_BCSCroatianLatBH;
				case et_Sub.ec_SubBCSBosnianCyrBH:
					return sauz_BCSBosnianCyrBH;
				case et_Sub.ec_SubBCSSerbianLatSerbia:
					return sauz_BCSSerbianLatSerbia;
				case et_Sub.ec_SubBCSSerbianCyrSerbia:
					return sauz_BCSSerbianCyrSerbia;
				case et_Sub.ec_SubBCSSerbianLatMontenegro:
					return sauz_BCSSerbianLatMontenegro;
				case et_Sub.ec_SubBCSSerbianCyrMontenegro:
					return sauz_BCSSerbianCyrMontenegro;
				case et_Sub.ec_SubBCSBosnianCyrillic:
					return sauz_BCSBosnianCyrillic;
				case et_Sub.ec_SubBCSBosnianLatin:
					return sauz_BCSBosnianLatin;
				case et_Sub.ec_SubBCSSerbianCyrKosovo:
					return sauz_BCSSerbianCyrKosovo;
				case et_Sub.ec_SubBCSSerbianLatKosovo:
					return sauz_BCSSerbianLatKosovo;
				default:
					return sauz_Croatian;
				}
			}
			if (et_Primary.ec_Kiswahili == eo_primary && et_Sub.ec_SubKiswahiliChewaSDL == eo_sub)
			{
				return sauz_KiswahiliChewa;
			}
			if (et_Primary.ec_Hindi == eo_primary && et_Sub.ec_SubHindiBihariSDL == eo_sub)
			{
				return sauz_HindiBihari;
			}
			if (et_Primary.ec_Hawaiian == eo_primary && et_Sub.ec_SubHawaiianNauruSDL == eo_sub)
			{
				return sauz_HawaiianNauru;
			}
			if (et_Primary.ec_Hawaiian == eo_primary && et_Sub.ec_SubHawaiianSamoanSDL == eo_sub)
			{
				return sauz_HawaiianSamoan;
			}
			if (et_Primary.ec_Malayalam == eo_primary && et_Sub.ec_SubMalayalamSundaneseSDL == eo_sub)
			{
				return sauz_MalayalamSundanese;
			}
			if (et_Primary.ec_Chinese == eo_primary && et_Sub.ec_SubChineseHmongSDL == eo_sub)
			{
				return sauz_ChineseHmong;
			}
			if (et_Primary.ec_Quechua == eo_primary && et_Sub.ec_SubQuechuaAymaraSDL == eo_sub)
			{
				return sauz_QuechuaAymara;
			}
			if (et_Primary.ec_Bislama == eo_primary && et_Sub.ec_SubBislamaBislamaSDL == eo_sub)
			{
				return sauz_BislamaBislama;
			}
			if (et_Primary.ec_Fijian == eo_primary && et_Sub.ec_SubFijianFijianSDL == eo_sub)
			{
				return sauz_FijianFijian;
			}
			if (et_Primary.ec_Inuktitut == eo_primary && et_Sub.ec_SubInuktitutInupiaqSDL == eo_sub)
			{
				return sauz_InuktitutInupiaq;
			}
			if (GetSubCount(eo_primary) != 0 && eo_sub != 0)
			{
				return $"{GetLanguage(eo_primary)} ({GetCountry(eo_sub)})";
			}
			return GetLanguage(eo_primary);
		}

		public static string MakeLanguageCode(et_Primary eo_primary, et_Sub eo_sub)
		{
			if (GetSubCount(eo_primary) == 0 || eo_sub == et_Sub.ec_SubNeutral)
			{
				return GetLanguageCode(eo_primary);
			}
			if (eo_primary == et_Primary.ec_Serbian)
			{
				string languageCode = GetLanguageCode(eo_primary);
				string text = "HR";
				switch (eo_sub)
				{
				case et_Sub.ec_SubDefault:
				case et_Sub.ec_SubBCSSerbianLatin:
				case et_Sub.ec_SubBCSSerbianCyrillic:
				case et_Sub.ec_SubBCSCroatianCroatia:
				case et_Sub.ec_SubBCSBosnianLatBH:
				case et_Sub.ec_SubBCSSerbianCyrBH:
				case et_Sub.ec_SubBCSSerbianLatBH:
				case et_Sub.ec_SubBCSCroatianLatBH:
				case et_Sub.ec_SubBCSBosnianCyrBH:
				case et_Sub.ec_SubBCSSerbianLatSerbia:
				case et_Sub.ec_SubBCSSerbianCyrSerbia:
				case et_Sub.ec_SubBCSSerbianLatMontenegro:
				case et_Sub.ec_SubBCSSerbianCyrMontenegro:
				case et_Sub.ec_SubBCSBosnianCyrillic:
				case et_Sub.ec_SubBCSBosnianLatin:
				case et_Sub.ec_SubBCSSerbianCyrKosovo:
				case et_Sub.ec_SubBCSSerbianLatKosovo:
					text = GetCountryCode(eo_sub);
					break;
				default:
					text = "HR";
					break;
				}
				return $"{languageCode}-{text}";
			}
			return $"{GetLanguageCode(eo_primary)}-{GetCountryCode(eo_sub)}";
		}

		public static bool GetLanguages(out et_Primary peo_primary, out et_Sub peo_sub, string pcco_code)
		{
			peo_primary = et_Primary.ec_Neutral;
			peo_sub = et_Sub.ec_SubNeutral;
			if (pcco_code == null)
			{
				return false;
			}
			uint length = (uint)pcco_code.Length;
			if (length < 2)
			{
				return false;
			}
			string text = pcco_code.ToUpper(CultureInfo.InvariantCulture);
			uint num = 0u;
			uint num2 = 3u;
			uint num3 = (uint)Math.Min(num2 + 1, text.Length);
			while (num < num3 && char.IsLetter(text[(int)num++]))
			{
			}
			bool flag = false;
			if (num < text.Length)
			{
				flag = true;
			}
			string code = (!flag) ? text : text.Substring(0, (int)(num - 1));
			if (GetPrimary(out peo_primary, code))
			{
				uint subCount = GetSubCount(peo_primary);
				if (!flag)
				{
					if (subCount == 0)
					{
						peo_sub = et_Sub.ec_SubDefault;
					}
					else
					{
						peo_sub = et_Sub.ec_SubNeutral;
					}
					return true;
				}
				if (subCount == 0)
				{
					peo_sub = et_Sub.ec_SubDefault;
					return true;
				}
				string text2 = text.Substring((int)num);
				if (text2 == "01")
				{
					peo_sub = et_Sub.ec_SubDefault;
					return true;
				}
				if (text2 == "00")
				{
					peo_sub = et_Sub.ec_SubNeutral;
					return true;
				}
				for (uint num4 = 1u; num4 <= subCount; num4++)
				{
					et_Sub subAt = GetSubAt(peo_primary, num4);
					string countryCode = GetCountryCode(subAt);
					if (countryCode == text2)
					{
						peo_sub = subAt;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static bool IsFarEastLanguage(et_Primary eo_primary)
		{
			if (eo_primary != et_Primary.ec_Chinese && eo_primary != et_Primary.ec_Thai && eo_primary != et_Primary.ec_Korean)
			{
				return eo_primary == et_Primary.ec_Japanese;
			}
			return true;
		}

		public static bool IsBiDiLanguage(string locale, et_Primary eo_primary)
		{
			try
			{
				CultureInfo cultureInfo = new CultureInfo(locale);
				return cultureInfo.TextInfo.IsRightToLeft;
			}
			catch (ArgumentException)
			{
				switch (eo_primary)
				{
				case et_Primary.ec_Arabic:
				case et_Primary.ec_Hebrew:
				case et_Primary.ec_Urdu:
				case et_Primary.ec_Persian:
				case et_Primary.ec_Divehi:
				case et_Primary.ec_Syriac:
				case et_Primary.ec_Nko:
					return true;
				default:
					return false;
				}
			}
		}

		public static bool IsBiDiLanguage(uint n_locale)
		{
			bool result = false;
			uint lcid = LANGIDFROMLCID(n_locale);
			uint n_primaryLanguageId = PRIMARYLANGID(lcid);
			int num = FindPrimaryRecord(n_primaryLanguageId);
			if (num >= 0)
			{
				string qcuz_LanguageCode = saso_PrimaryLanguages[num].qcuz_LanguageCode;
				et_Primary eo_Primary = saso_PrimaryLanguages[num].eo_Primary;
				result = IsBiDiLanguage(qcuz_LanguageCode, eo_Primary);
			}
			return result;
		}

		public static bool IsTRADOSDefined(et_Primary eo_primary, et_Sub eo_sub)
		{
			return false;
		}

		public static bool IsTRADOSDefined(et_Primary eo_primary)
		{
			return IsTRADOSDefined(eo_primary, et_Sub.ec_SubNeutral);
		}

		public static bool IsLanguageCompatible(uint n_lcid1, uint n_lcid2)
		{
			GetLanguages(n_lcid1, out et_Primary peo_primary, out et_Sub peo_sub);
			GetLanguages(n_lcid2, out et_Primary peo_primary2, out et_Sub peo_sub2);
			if (peo_primary == peo_primary2 && (peo_sub == peo_sub2 || peo_sub == et_Sub.ec_SubNeutral || peo_sub2 == et_Sub.ec_SubNeutral))
			{
				return true;
			}
			return false;
		}

		private static int FindPrimaryRecord(uint n_primaryLanguageId)
		{
			for (int i = 0; i < sn_PrimaryLanguages; i++)
			{
				if (saso_PrimaryLanguages[i].n2_PrimaryLanguageID == n_primaryLanguageId)
				{
					return i;
				}
			}
			return -1;
		}

		private static int FindPrimaryRecord(et_Primary eo_primary)
		{
			for (int i = 0; i < sn_PrimaryLanguages; i++)
			{
				if (saso_PrimaryLanguages[i].eo_Primary == eo_primary)
				{
					return i;
				}
			}
			return -1;
		}

		private static int FindSubRecord(int i_startOffset, et_Sub eo_sub)
		{
			if (i_startOffset < 0 || i_startOffset >= (int)sn_SubLanguages)
			{
				return -1;
			}
			for (int i = i_startOffset; i < sn_SubLanguages; i++)
			{
				if (saso_SubLanguages[i].eo_Sub == eo_sub)
				{
					return i;
				}
				if (i_startOffset > 0 && saso_SubLanguages[i].n2_PrimaryLanguageID != saso_SubLanguages[i_startOffset].n2_PrimaryLanguageID)
				{
					break;
				}
			}
			return -1;
		}

		private static int FindSubRecord(int i_startOffset, uint n_subLangId)
		{
			if (i_startOffset < 0 || i_startOffset >= (int)sn_SubLanguages)
			{
				return -1;
			}
			for (int i = i_startOffset; i < sn_SubLanguages; i++)
			{
				if (saso_SubLanguages[i].n2_SubLanguageID == n_subLangId)
				{
					return i;
				}
				if (i_startOffset > 0 && saso_SubLanguages[i].n2_PrimaryLanguageID != saso_SubLanguages[i_startOffset].n2_PrimaryLanguageID)
				{
					break;
				}
			}
			return -1;
		}
	}
}
