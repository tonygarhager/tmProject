using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;
using System.Text;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_el : Stemmer
	{
		private StringBuilder S_s = new StringBuilder();

		private bool B_test1;

		private static string g_v = "αεηιουω";

		private static string g_v2 = "αεηιοω";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private readonly Among[] a_4;

		private readonly Among[] a_5;

		private readonly Among[] a_6;

		private readonly Among[] a_7;

		private readonly Among[] a_8;

		private readonly Among[] a_9;

		private readonly Among[] a_10;

		private readonly Among[] a_11;

		private readonly Among[] a_12;

		private readonly Among[] a_13;

		private readonly Among[] a_14;

		private readonly Among[] a_15;

		private readonly Among[] a_16;

		private readonly Among[] a_17;

		private readonly Among[] a_18;

		private readonly Among[] a_19;

		private readonly Among[] a_20;

		private readonly Among[] a_21;

		private readonly Among[] a_22;

		private readonly Among[] a_23;

		private readonly Among[] a_24;

		private readonly Among[] a_25;

		private readonly Among[] a_26;

		private readonly Among[] a_27;

		private readonly Among[] a_28;

		private readonly Among[] a_29;

		private readonly Among[] a_30;

		private readonly Among[] a_31;

		private readonly Among[] a_32;

		private readonly Among[] a_33;

		private readonly Among[] a_34;

		private readonly Among[] a_35;

		private readonly Among[] a_36;

		private readonly Among[] a_37;

		private readonly Among[] a_38;

		private readonly Among[] a_39;

		private readonly Among[] a_40;

		private readonly Among[] a_41;

		private readonly Among[] a_42;

		private readonly Among[] a_43;

		private readonly Among[] a_44;

		private readonly Among[] a_45;

		private readonly Among[] a_46;

		private readonly Among[] a_47;

		private readonly Among[] a_48;

		private readonly Among[] a_49;

		private readonly Among[] a_50;

		private readonly Among[] a_51;

		private readonly Among[] a_52;

		private readonly Among[] a_53;

		private readonly Among[] a_54;

		private readonly Among[] a_55;

		private readonly Among[] a_56;

		private readonly Among[] a_57;

		private readonly Among[] a_58;

		private readonly Among[] a_59;

		private readonly Among[] a_60;

		private readonly Among[] a_61;

		private readonly Among[] a_62;

		private readonly Among[] a_63;

		private readonly Among[] a_64;

		private readonly Among[] a_65;

		private readonly Among[] a_66;

		private readonly Among[] a_67;

		private readonly Among[] a_68;

		private readonly Among[] a_69;

		private readonly Among[] a_70;

		private readonly Among[] a_71;

		private readonly Among[] a_72;

		private readonly Among[] a_73;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_el()
		{
			a_0 = new Among[46]
			{
				new Among("", -1, 25),
				new Among("Ά", 0, 1),
				new Among("Έ", 0, 5),
				new Among("Ή", 0, 7),
				new Among("Ί", 0, 9),
				new Among("Ό", 0, 15),
				new Among("Ύ", 0, 20),
				new Among("Ώ", 0, 24),
				new Among("ΐ", 0, 7),
				new Among("Α", 0, 1),
				new Among("Β", 0, 2),
				new Among("Γ", 0, 3),
				new Among("Δ", 0, 4),
				new Among("Ε", 0, 5),
				new Among("Ζ", 0, 6),
				new Among("Η", 0, 7),
				new Among("Θ", 0, 8),
				new Among("Ι", 0, 9),
				new Among("Κ", 0, 10),
				new Among("Λ", 0, 11),
				new Among("Μ", 0, 12),
				new Among("Ν", 0, 13),
				new Among("Ξ", 0, 14),
				new Among("Ο", 0, 15),
				new Among("Π", 0, 16),
				new Among("Ρ", 0, 17),
				new Among("Σ", 0, 18),
				new Among("Τ", 0, 19),
				new Among("Υ", 0, 20),
				new Among("Φ", 0, 21),
				new Among("Χ", 0, 22),
				new Among("Ψ", 0, 23),
				new Among("Ω", 0, 24),
				new Among("Ϊ", 0, 9),
				new Among("Ϋ", 0, 20),
				new Among("ά", 0, 1),
				new Among("έ", 0, 5),
				new Among("ή", 0, 7),
				new Among("ί", 0, 9),
				new Among("ΰ", 0, 20),
				new Among("ς", 0, 18),
				new Among("ϊ", 0, 7),
				new Among("ϋ", 0, 20),
				new Among("ό", 0, 15),
				new Among("ύ", 0, 20),
				new Among("ώ", 0, 24)
			};
			a_1 = new Among[40]
			{
				new Among("σκαγια", -1, 2),
				new Among("φαγια", -1, 1),
				new Among("ολογια", -1, 3),
				new Among("σογια", -1, 4),
				new Among("τατογια", -1, 5),
				new Among("κρεατα", -1, 6),
				new Among("περατα", -1, 7),
				new Among("τερατα", -1, 8),
				new Among("γεγονοτα", -1, 11),
				new Among("καθεστωτα", -1, 10),
				new Among("φωτα", -1, 9),
				new Among("περατη", -1, 7),
				new Among("σκαγιων", -1, 2),
				new Among("φαγιων", -1, 1),
				new Among("ολογιων", -1, 3),
				new Among("σογιων", -1, 4),
				new Among("τατογιων", -1, 5),
				new Among("κρεατων", -1, 6),
				new Among("περατων", -1, 7),
				new Among("τερατων", -1, 8),
				new Among("γεγονοτων", -1, 11),
				new Among("καθεστωτων", -1, 10),
				new Among("φωτων", -1, 9),
				new Among("κρεασ", -1, 6),
				new Among("περασ", -1, 7),
				new Among("τερασ", -1, 8),
				new Among("γεγονοσ", -1, 11),
				new Among("κρεατοσ", -1, 6),
				new Among("περατοσ", -1, 7),
				new Among("τερατοσ", -1, 8),
				new Among("γεγονοτοσ", -1, 11),
				new Among("καθεστωτοσ", -1, 10),
				new Among("φωτοσ", -1, 9),
				new Among("καθεστωσ", -1, 10),
				new Among("φωσ", -1, 9),
				new Among("σκαγιου", -1, 2),
				new Among("φαγιου", -1, 1),
				new Among("ολογιου", -1, 3),
				new Among("σογιου", -1, 4),
				new Among("τατογιου", -1, 5)
			};
			a_2 = new Among[9]
			{
				new Among("πα", -1, 1),
				new Among("ξαναπα", 0, 1),
				new Among("επα", 0, 1),
				new Among("περιπα", 0, 1),
				new Among("αναμπα", 0, 1),
				new Among("εμπα", 0, 1),
				new Among("δανε", -1, 1),
				new Among("αθρο", -1, 1),
				new Among("συναθρο", 7, 1)
			};
			a_3 = new Among[22]
			{
				new Among("β", -1, 1),
				new Among("βαθυρι", -1, 1),
				new Among("βαρκ", -1, 1),
				new Among("μαρκ", -1, 1),
				new Among("λ", -1, 1),
				new Among("μ", -1, 1),
				new Among("κορν", -1, 1),
				new Among("π", -1, 1),
				new Among("ιμπ", 7, 1),
				new Among("ρ", -1, 1),
				new Among("μαρ", 9, 1),
				new Among("αμπαρ", 9, 1),
				new Among("γκρ", 9, 1),
				new Among("βολβορ", 9, 1),
				new Among("γλυκορ", 9, 1),
				new Among("πιπερορ", 9, 1),
				new Among("πρ", 9, 1),
				new Among("μπρ", 16, 1),
				new Among("αρρ", 9, 1),
				new Among("γλυκυρ", 9, 1),
				new Among("πολυρ", 9, 1),
				new Among("λου", -1, 1)
			};
			a_4 = new Among[14]
			{
				new Among("ιζα", -1, 1),
				new Among("ιζε", -1, 1),
				new Among("ιζαμε", -1, 1),
				new Among("ιζουμε", -1, 1),
				new Among("ιζανε", -1, 1),
				new Among("ιζουνε", -1, 1),
				new Among("ιζατε", -1, 1),
				new Among("ιζετε", -1, 1),
				new Among("ιζει", -1, 1),
				new Among("ιζαν", -1, 1),
				new Among("ιζουν", -1, 1),
				new Among("ιζεσ", -1, 1),
				new Among("ιζεισ", -1, 1),
				new Among("ιζω", -1, 1)
			};
			a_5 = new Among[8]
			{
				new Among("βι", -1, 1),
				new Among("λι", -1, 1),
				new Among("αλ", -1, 1),
				new Among("εν", -1, 1),
				new Among("σ", -1, 1),
				new Among("χ", -1, 1),
				new Among("υψ", -1, 1),
				new Among("ζω", -1, 1)
			};
			a_6 = new Among[7]
			{
				new Among("ωθηκα", -1, 1),
				new Among("ωθηκε", -1, 1),
				new Among("ωθηκαμε", -1, 1),
				new Among("ωθηκανε", -1, 1),
				new Among("ωθηκατε", -1, 1),
				new Among("ωθηκαν", -1, 1),
				new Among("ωθηκεσ", -1, 1)
			};
			a_7 = new Among[19]
			{
				new Among("ξαναπα", -1, 1),
				new Among("επα", -1, 1),
				new Among("περιπα", -1, 1),
				new Among("αναμπα", -1, 1),
				new Among("εμπα", -1, 1),
				new Among("χαρτοπα", -1, 1),
				new Among("εξαρχα", -1, 1),
				new Among("κλε", -1, 1),
				new Among("εκλε", 7, 1),
				new Among("απεκλε", 8, 1),
				new Among("αποκλε", 7, 1),
				new Among("εσωκλε", 7, 1),
				new Among("δανε", -1, 1),
				new Among("πε", -1, 1),
				new Among("επε", 13, 1),
				new Among("μετεπε", 14, 1),
				new Among("εσε", -1, 1),
				new Among("αθρο", -1, 1),
				new Among("συναθρο", 17, 1)
			};
			a_8 = new Among[13]
			{
				new Among("γε", -1, 1),
				new Among("γκε", -1, 1),
				new Among("γκ", -1, 1),
				new Among("μ", -1, 1),
				new Among("πουκαμ", 3, 1),
				new Among("κομ", 3, 1),
				new Among("αν", -1, 1),
				new Among("ολο", -1, 1),
				new Among("π", -1, 1),
				new Among("λαρ", -1, 1),
				new Among("δημοκρατ", -1, 1),
				new Among("αφ", -1, 1),
				new Among("γιγαντοαφ", 11, 1)
			};
			a_9 = new Among[7]
			{
				new Among("ισα", -1, 1),
				new Among("ισαμε", -1, 1),
				new Among("ισανε", -1, 1),
				new Among("ισε", -1, 1),
				new Among("ισατε", -1, 1),
				new Among("ισαν", -1, 1),
				new Among("ισεσ", -1, 1)
			};
			a_10 = new Among[19]
			{
				new Among("ξαναπα", -1, 1),
				new Among("επα", -1, 1),
				new Among("περιπα", -1, 1),
				new Among("αναμπα", -1, 1),
				new Among("εμπα", -1, 1),
				new Among("χαρτοπα", -1, 1),
				new Among("εξαρχα", -1, 1),
				new Among("κλε", -1, 1),
				new Among("εκλε", 7, 1),
				new Among("απεκλε", 8, 1),
				new Among("αποκλε", 7, 1),
				new Among("εσωκλε", 7, 1),
				new Among("δανε", -1, 1),
				new Among("πε", -1, 1),
				new Among("επε", 13, 1),
				new Among("μετεπε", 14, 1),
				new Among("εσε", -1, 1),
				new Among("αθρο", -1, 1),
				new Among("συναθρο", 17, 1)
			};
			a_11 = new Among[7]
			{
				new Among("ισουμε", -1, 1),
				new Among("ισουνε", -1, 1),
				new Among("ισετε", -1, 1),
				new Among("ισει", -1, 1),
				new Among("ισουν", -1, 1),
				new Among("ισεισ", -1, 1),
				new Among("ισω", -1, 1)
			};
			a_12 = new Among[7]
			{
				new Among("κλε", -1, 1),
				new Among("εσωκλε", 0, 1),
				new Among("πλε", -1, 1),
				new Among("δανε", -1, 1),
				new Among("σε", -1, 1),
				new Among("ασε", 4, 1),
				new Among("συναθρο", -1, 1)
			};
			a_13 = new Among[33]
			{
				new Among("ατα", -1, 1),
				new Among("φα", -1, 1),
				new Among("ηφα", 1, 1),
				new Among("μεγ", -1, 1),
				new Among("λυγ", -1, 1),
				new Among("ηδ", -1, 1),
				new Among("καθ", -1, 1),
				new Among("εχθ", -1, 1),
				new Among("κακ", -1, 1),
				new Among("μακ", -1, 1),
				new Among("σκ", -1, 1),
				new Among("φιλ", -1, 1),
				new Among("κυλ", -1, 1),
				new Among("μ", -1, 1),
				new Among("γεμ", 13, 1),
				new Among("αχν", -1, 1),
				new Among("π", -1, 1),
				new Among("απ", 16, 1),
				new Among("εμπ", 16, 1),
				new Among("ευπ", 16, 1),
				new Among("αρ", -1, 1),
				new Among("αορ", -1, 1),
				new Among("γυρ", -1, 1),
				new Among("χρ", -1, 1),
				new Among("χωρ", -1, 1),
				new Among("κτ", -1, 1),
				new Among("ακτ", 25, 1),
				new Among("χτ", -1, 1),
				new Among("αχτ", 27, 1),
				new Among("ταχ", -1, 1),
				new Among("σχ", -1, 1),
				new Among("ασχ", 30, 1),
				new Among("υψ", -1, 1)
			};
			a_14 = new Among[11]
			{
				new Among("ιστα", -1, 1),
				new Among("ιστε", -1, 1),
				new Among("ιστη", -1, 1),
				new Among("ιστοι", -1, 1),
				new Among("ιστων", -1, 1),
				new Among("ιστο", -1, 1),
				new Among("ιστεσ", -1, 1),
				new Among("ιστησ", -1, 1),
				new Among("ιστοσ", -1, 1),
				new Among("ιστουσ", -1, 1),
				new Among("ιστου", -1, 1)
			};
			a_15 = new Among[5]
			{
				new Among("εγκλε", -1, 1),
				new Among("αποκλε", -1, 1),
				new Among("σε", -1, 1),
				new Among("μετασε", 2, 1),
				new Among("μικροσε", 2, 1)
			};
			a_16 = new Among[2]
			{
				new Among("δανε", -1, 1),
				new Among("αντιδανε", 0, 1)
			};
			a_17 = new Among[10]
			{
				new Among("ατομικ", -1, 2),
				new Among("εθνικ", -1, 4),
				new Among("τοπικ", -1, 7),
				new Among("εκλεκτικ", -1, 5),
				new Among("σκεπτικ", -1, 6),
				new Among("γνωστικ", -1, 3),
				new Among("αγνωστικ", 5, 1),
				new Among("αλεξανδριν", -1, 8),
				new Among("θεατριν", -1, 10),
				new Among("βυζαντιν", -1, 9)
			};
			a_18 = new Among[6]
			{
				new Among("ισμοι", -1, 1),
				new Among("ισμων", -1, 1),
				new Among("ισμο", -1, 1),
				new Among("ισμοσ", -1, 1),
				new Among("ισμουσ", -1, 1),
				new Among("ισμου", -1, 1)
			};
			a_19 = new Among[2]
			{
				new Among("σ", -1, 1),
				new Among("χ", -1, 1)
			};
			a_20 = new Among[4]
			{
				new Among("ουδακια", -1, 1),
				new Among("αρακια", -1, 1),
				new Among("ουδακι", -1, 1),
				new Among("αρακι", -1, 1)
			};
			a_21 = new Among[33]
			{
				new Among("βαμβ", -1, 1),
				new Among("σλοβ", -1, 1),
				new Among("τσεχοσλοβ", 1, 1),
				new Among("τζ", -1, 1),
				new Among("κ", -1, 1),
				new Among("καπακ", 4, 1),
				new Among("σοκ", 4, 1),
				new Among("σκ", 4, 1),
				new Among("μαλ", -1, 1),
				new Among("πλ", -1, 1),
				new Among("λουλ", -1, 1),
				new Among("φυλ", -1, 1),
				new Among("καιμ", -1, 1),
				new Among("κλιμ", -1, 1),
				new Among("φαρμ", -1, 1),
				new Among("σπαν", -1, 1),
				new Among("κον", -1, 1),
				new Among("κατραπ", -1, 1),
				new Among("ρ", -1, 1),
				new Among("βρ", 18, 1),
				new Among("λαβρ", 19, 1),
				new Among("αμβρ", 19, 1),
				new Among("μερ", 18, 1),
				new Among("ανθρ", 18, 1),
				new Among("κορ", 18, 1),
				new Among("σ", -1, 1),
				new Among("ναγκασ", 25, 1),
				new Among("μουστ", -1, 1),
				new Among("ρυ", -1, 1),
				new Among("φ", -1, 1),
				new Among("σφ", 29, 1),
				new Among("αλισφ", 30, 1),
				new Among("χ", -1, 1)
			};
			a_22 = new Among[15]
			{
				new Among("β", -1, 1),
				new Among("καρδ", -1, 1),
				new Among("ζ", -1, 1),
				new Among("σκ", -1, 1),
				new Among("βαλ", -1, 1),
				new Among("γλ", -1, 1),
				new Among("τριπολ", -1, 1),
				new Among("γιαν", -1, 1),
				new Among("ηγουμεν", -1, 1),
				new Among("κον", -1, 1),
				new Among("μακρυν", -1, 1),
				new Among("π", -1, 1),
				new Among("πατερ", -1, 1),
				new Among("τοσ", -1, 1),
				new Among("νυφ", -1, 1)
			};
			a_23 = new Among[8]
			{
				new Among("ακια", -1, 1),
				new Among("αρακια", 0, 1),
				new Among("ιτσα", -1, 1),
				new Among("ακι", -1, 1),
				new Among("αρακι", 3, 1),
				new Among("ιτσων", -1, 1),
				new Among("ιτσασ", -1, 1),
				new Among("ιτσεσ", -1, 1)
			};
			a_24 = new Among[4]
			{
				new Among("ψαλ", -1, 1),
				new Among("αιφν", -1, 1),
				new Among("ολο", -1, 1),
				new Among("ιρ", -1, 1)
			};
			a_25 = new Among[2]
			{
				new Among("ε", -1, 1),
				new Among("παιχν", -1, 1)
			};
			a_26 = new Among[3]
			{
				new Among("ιδια", -1, 1),
				new Among("ιδιων", -1, 1),
				new Among("ιδιο", -1, 1)
			};
			a_27 = new Among[7]
			{
				new Among("ιβ", -1, 1),
				new Among("δ", -1, 1),
				new Among("φραγκ", -1, 1),
				new Among("λυκ", -1, 1),
				new Among("οβελ", -1, 1),
				new Among("μην", -1, 1),
				new Among("ρ", -1, 1)
			};
			a_28 = new Among[4]
			{
				new Among("ισκε", -1, 1),
				new Among("ισκο", -1, 1),
				new Among("ισκοσ", -1, 1),
				new Among("ισκου", -1, 1)
			};
			a_29 = new Among[2]
			{
				new Among("αδων", -1, 1),
				new Among("αδεσ", -1, 1)
			};
			a_30 = new Among[10]
			{
				new Among("γιαγι", -1, -1),
				new Among("θει", -1, -1),
				new Among("οκ", -1, -1),
				new Among("μαμ", -1, -1),
				new Among("μαν", -1, -1),
				new Among("μπαμπ", -1, -1),
				new Among("πεθερ", -1, -1),
				new Among("πατερ", -1, -1),
				new Among("κυρ", -1, -1),
				new Among("νταντ", -1, -1)
			};
			a_31 = new Among[2]
			{
				new Among("εδων", -1, 1),
				new Among("εδεσ", -1, 1)
			};
			a_32 = new Among[8]
			{
				new Among("μιλ", -1, 1),
				new Among("δαπ", -1, 1),
				new Among("γηπ", -1, 1),
				new Among("ιπ", -1, 1),
				new Among("εμπ", -1, 1),
				new Among("οπ", -1, 1),
				new Among("κρασπ", -1, 1),
				new Among("υπ", -1, 1)
			};
			a_33 = new Among[2]
			{
				new Among("ουδων", -1, 1),
				new Among("ουδεσ", -1, 1)
			};
			a_34 = new Among[15]
			{
				new Among("τραγ", -1, 1),
				new Among("φε", -1, 1),
				new Among("καλιακ", -1, 1),
				new Among("αρκ", -1, 1),
				new Among("σκ", -1, 1),
				new Among("πεταλ", -1, 1),
				new Among("βελ", -1, 1),
				new Among("λουλ", -1, 1),
				new Among("φλ", -1, 1),
				new Among("χν", -1, 1),
				new Among("πλεξ", -1, 1),
				new Among("σπ", -1, 1),
				new Among("φρ", -1, 1),
				new Among("σ", -1, 1),
				new Among("λιχ", -1, 1)
			};
			a_35 = new Among[2]
			{
				new Among("εων", -1, 1),
				new Among("εωσ", -1, 1)
			};
			a_36 = new Among[8]
			{
				new Among("δ", -1, 1),
				new Among("ιδ", 0, 1),
				new Among("θ", -1, 1),
				new Among("γαλ", -1, 1),
				new Among("ελ", -1, 1),
				new Among("ν", -1, 1),
				new Among("π", -1, 1),
				new Among("παρ", -1, 1)
			};
			a_37 = new Among[3]
			{
				new Among("ια", -1, 1),
				new Among("ιων", -1, 1),
				new Among("ιου", -1, 1)
			};
			a_38 = new Among[4]
			{
				new Among("ικα", -1, 1),
				new Among("ικων", -1, 1),
				new Among("ικο", -1, 1),
				new Among("ικου", -1, 1)
			};
			a_39 = new Among[36]
			{
				new Among("αδ", -1, 1),
				new Among("συναδ", 0, 1),
				new Among("καταδ", 0, 1),
				new Among("αντιδ", -1, 1),
				new Among("ενδ", -1, 1),
				new Among("φυλοδ", -1, 1),
				new Among("υποδ", -1, 1),
				new Among("πρωτοδ", -1, 1),
				new Among("εξωδ", -1, 1),
				new Among("ηθ", -1, 1),
				new Among("ανηθ", 9, 1),
				new Among("ξικ", -1, 1),
				new Among("αλ", -1, 1),
				new Among("αμμοχαλ", 12, 1),
				new Among("συνομηλ", -1, 1),
				new Among("μπολ", -1, 1),
				new Among("μουλ", -1, 1),
				new Among("τσαμ", -1, 1),
				new Among("βρωμ", -1, 1),
				new Among("αμαν", -1, 1),
				new Among("μπαν", -1, 1),
				new Among("καλλιν", -1, 1),
				new Among("ποστελν", -1, 1),
				new Among("φιλον", -1, 1),
				new Among("καλπ", -1, 1),
				new Among("γερ", -1, 1),
				new Among("χασ", -1, 1),
				new Among("μποσ", -1, 1),
				new Among("πλιατσ", -1, 1),
				new Among("πετσ", -1, 1),
				new Among("πιτσ", -1, 1),
				new Among("φυσ", -1, 1),
				new Among("μπαγιατ", -1, 1),
				new Among("νιτ", -1, 1),
				new Among("πικαντ", -1, 1),
				new Among("σερτ", -1, 1)
			};
			a_40 = new Among[5]
			{
				new Among("αγαμε", -1, 1),
				new Among("ηκαμε", -1, 1),
				new Among("ηθηκαμε", 1, 1),
				new Among("ησαμε", -1, 1),
				new Among("ουσαμε", -1, 1)
			};
			a_41 = new Among[12]
			{
				new Among("βουβ", -1, 1),
				new Among("ξεθ", -1, 1),
				new Among("πεθ", -1, 1),
				new Among("αποθ", -1, 1),
				new Among("αποκ", -1, 1),
				new Among("ουλ", -1, 1),
				new Among("αναπ", -1, 1),
				new Among("πικρ", -1, 1),
				new Among("ποτ", -1, 1),
				new Among("αποστ", -1, 1),
				new Among("χ", -1, 1),
				new Among("σιχ", 10, 1)
			};
			a_42 = new Among[2]
			{
				new Among("τρ", -1, 1),
				new Among("τσ", -1, 1)
			};
			a_43 = new Among[11]
			{
				new Among("αγανε", -1, 1),
				new Among("ηκανε", -1, 1),
				new Among("ηθηκανε", 1, 1),
				new Among("ησανε", -1, 1),
				new Among("ουσανε", -1, 1),
				new Among("οντανε", -1, 1),
				new Among("ιοντανε", 5, 1),
				new Among("ουντανε", -1, 1),
				new Among("ιουντανε", 7, 1),
				new Among("οτανε", -1, 1),
				new Among("ιοτανε", 9, 1)
			};
			a_44 = new Among[95]
			{
				new Among("ταβ", -1, 1),
				new Among("νταβ", 0, 1),
				new Among("ψηλοταβ", 0, 1),
				new Among("λιβ", -1, 1),
				new Among("κλιβ", 3, 1),
				new Among("ξηροκλιβ", 4, 1),
				new Among("γ", -1, 1),
				new Among("αγ", 6, 1),
				new Among("τραγ", 7, 1),
				new Among("τσαγ", 7, 1),
				new Among("αθιγγ", 6, 1),
				new Among("τσιγγ", 6, 1),
				new Among("ατσιγγ", 11, 1),
				new Among("στεγ", 6, 1),
				new Among("απηγ", 6, 1),
				new Among("σιγ", 6, 1),
				new Among("ανοργ", 6, 1),
				new Among("ενοργ", 6, 1),
				new Among("καλπουζ", -1, 1),
				new Among("θ", -1, 1),
				new Among("μωαμεθ", 19, 1),
				new Among("πιθ", 19, 1),
				new Among("απιθ", 21, 1),
				new Among("δεκ", -1, 1),
				new Among("πελεκ", -1, 1),
				new Among("ικ", -1, 1),
				new Among("ανικ", 25, 1),
				new Among("βουλκ", -1, 1),
				new Among("βασκ", -1, 1),
				new Among("βραχυκ", -1, 1),
				new Among("γαλ", -1, 1),
				new Among("καταγαλ", 30, 1),
				new Among("ολογαλ", 30, 1),
				new Among("βαθυγαλ", 30, 1),
				new Among("μελ", -1, 1),
				new Among("καστελ", -1, 1),
				new Among("πορτολ", -1, 1),
				new Among("πλ", -1, 1),
				new Among("διπλ", 37, 1),
				new Among("λαοπλ", 37, 1),
				new Among("ψυχοπλ", 37, 1),
				new Among("ουλ", -1, 1),
				new Among("μ", -1, 1),
				new Among("ολιγοδαμ", 42, 1),
				new Among("μουσουλμ", 42, 1),
				new Among("δραδουμ", 42, 1),
				new Among("βραχμ", 42, 1),
				new Among("ν", -1, 1),
				new Among("αμερικαν", 47, 1),
				new Among("π", -1, 1),
				new Among("αδαπ", 49, 1),
				new Among("χαμηλοδαπ", 49, 1),
				new Among("πολυδαπ", 49, 1),
				new Among("κοπ", 49, 1),
				new Among("υποκοπ", 53, 1),
				new Among("τσοπ", 49, 1),
				new Among("σπ", 49, 1),
				new Among("ερ", -1, 1),
				new Among("γερ", 57, 1),
				new Among("βετερ", 57, 1),
				new Among("λουθηρ", -1, 1),
				new Among("κορμορ", -1, 1),
				new Among("περιτρ", -1, 1),
				new Among("ουρ", -1, 1),
				new Among("σ", -1, 1),
				new Among("βασ", 64, 1),
				new Among("πολισ", 64, 1),
				new Among("σαρακατσ", 64, 1),
				new Among("θυσ", 64, 1),
				new Among("διατ", -1, 1),
				new Among("πλατ", -1, 1),
				new Among("τσαρλατ", -1, 1),
				new Among("τετ", -1, 1),
				new Among("πουριτ", -1, 1),
				new Among("σουλτ", -1, 1),
				new Among("μαιντ", -1, 1),
				new Among("ζωντ", -1, 1),
				new Among("καστ", -1, 1),
				new Among("φ", -1, 1),
				new Among("διαφ", 78, 1),
				new Among("στεφ", 78, 1),
				new Among("φωτοστεφ", 80, 1),
				new Among("περηφ", 78, 1),
				new Among("υπερηφ", 82, 1),
				new Among("κοιλαρφ", 78, 1),
				new Among("πενταρφ", 78, 1),
				new Among("ορφ", 78, 1),
				new Among("χ", -1, 1),
				new Among("αμηχ", 87, 1),
				new Among("βιομηχ", 87, 1),
				new Among("μεγλοβιομηχ", 89, 1),
				new Among("καπνοβιομηχ", 89, 1),
				new Among("μικροβιομηχ", 89, 1),
				new Among("πολυμηχ", 87, 1),
				new Among("λιχ", 87, 1)
			};
			a_45 = new Among[1]
			{
				new Among("ησετε", -1, 1)
			};
			a_46 = new Among[31]
			{
				new Among("ενδ", -1, 1),
				new Among("συνδ", -1, 1),
				new Among("οδ", -1, 1),
				new Among("διαθ", -1, 1),
				new Among("καθ", -1, 1),
				new Among("ραθ", -1, 1),
				new Among("ταθ", -1, 1),
				new Among("τιθ", -1, 1),
				new Among("εκθ", -1, 1),
				new Among("ενθ", -1, 1),
				new Among("συνθ", -1, 1),
				new Among("ροθ", -1, 1),
				new Among("υπερθ", -1, 1),
				new Among("σθ", -1, 1),
				new Among("ευθ", -1, 1),
				new Among("αρκ", -1, 1),
				new Among("ωφελ", -1, 1),
				new Among("βολ", -1, 1),
				new Among("αιν", -1, 1),
				new Among("πον", -1, 1),
				new Among("ρον", -1, 1),
				new Among("συν", -1, 1),
				new Among("βαρ", -1, 1),
				new Among("βρ", -1, 1),
				new Among("αιρ", -1, 1),
				new Among("φορ", -1, 1),
				new Among("ευρ", -1, 1),
				new Among("πυρ", -1, 1),
				new Among("χωρ", -1, 1),
				new Among("νετ", -1, 1),
				new Among("σχ", -1, 1)
			};
			a_47 = new Among[25]
			{
				new Among("παγ", -1, 1),
				new Among("δ", -1, 1),
				new Among("αδ", 1, 1),
				new Among("θ", -1, 1),
				new Among("αθ", 3, 1),
				new Among("τοκ", -1, 1),
				new Among("σκ", -1, 1),
				new Among("παρακαλ", -1, 1),
				new Among("σκελ", -1, 1),
				new Among("απλ", -1, 1),
				new Among("εμ", -1, 1),
				new Among("αν", -1, 1),
				new Among("βεν", -1, 1),
				new Among("βαρον", -1, 1),
				new Among("κοπ", -1, 1),
				new Among("σερπ", -1, 1),
				new Among("αβαρ", -1, 1),
				new Among("εναρ", -1, 1),
				new Among("αβρ", -1, 1),
				new Among("μπορ", -1, 1),
				new Among("θαρρ", -1, 1),
				new Among("ντρ", -1, 1),
				new Among("υ", -1, 1),
				new Among("νιφ", -1, 1),
				new Among("συρφ", -1, 1)
			};
			a_48 = new Among[2]
			{
				new Among("οντασ", -1, 1),
				new Among("ωντασ", -1, 1)
			};
			a_49 = new Among[2]
			{
				new Among("ομαστε", -1, 1),
				new Among("ιομαστε", 0, 1)
			};
			a_50 = new Among[6]
			{
				new Among("π", -1, 1),
				new Among("απ", 0, 1),
				new Among("ακαταπ", 1, 1),
				new Among("συμπ", 0, 1),
				new Among("ασυμπ", 3, 1),
				new Among("αμεταμφ", -1, 1)
			};
			a_51 = new Among[9]
			{
				new Among("ζ", -1, 1),
				new Among("αλ", -1, 1),
				new Among("παρακαλ", 1, 1),
				new Among("εκτελ", -1, 1),
				new Among("μ", -1, 1),
				new Among("ξ", -1, 1),
				new Among("προ", -1, 1),
				new Among("αρ", -1, 1),
				new Among("νισ", -1, 1)
			};
			a_52 = new Among[3]
			{
				new Among("ηθηκα", -1, 1),
				new Among("ηθηκε", -1, 1),
				new Among("ηθηκεσ", -1, 1)
			};
			a_53 = new Among[6]
			{
				new Among("πιθ", -1, 1),
				new Among("οθ", -1, 1),
				new Among("ναρθ", -1, 1),
				new Among("σκουλ", -1, 1),
				new Among("σκωλ", -1, 1),
				new Among("σφ", -1, 1)
			};
			a_54 = new Among[5]
			{
				new Among("θ", -1, 1),
				new Among("διαθ", 0, 1),
				new Among("παρακαταθ", 0, 1),
				new Among("συνθ", 0, 1),
				new Among("προσθ", 0, 1)
			};
			a_55 = new Among[3]
			{
				new Among("ηκα", -1, 1),
				new Among("ηκε", -1, 1),
				new Among("ηκεσ", -1, 1)
			};
			a_56 = new Among[12]
			{
				new Among("φαγ", -1, 1),
				new Among("ληγ", -1, 1),
				new Among("φρυδ", -1, 1),
				new Among("μαντιλ", -1, 1),
				new Among("μαλλ", -1, 1),
				new Among("ομ", -1, 1),
				new Among("βλεπ", -1, 1),
				new Among("ποδαρ", -1, 1),
				new Among("κυματ", -1, 1),
				new Among("πρωτ", -1, 1),
				new Among("λαχ", -1, 1),
				new Among("πανταχ", -1, 1)
			};
			a_57 = new Among[25]
			{
				new Among("τσα", -1, 1),
				new Among("χαδ", -1, 1),
				new Among("μεδ", -1, 1),
				new Among("λαμπιδ", -1, 1),
				new Among("δε", -1, 1),
				new Among("πλε", -1, 1),
				new Among("μεσαζ", -1, 1),
				new Among("δεσποζ", -1, 1),
				new Among("αιθ", -1, 1),
				new Among("φαρμακ", -1, 1),
				new Among("αγκ", -1, 1),
				new Among("ανηκ", -1, 1),
				new Among("λ", -1, 1),
				new Among("μ", -1, 1),
				new Among("αμ", 13, 1),
				new Among("βρομ", 13, 1),
				new Among("υποτειν", -1, 1),
				new Among("εκλιπ", -1, 1),
				new Among("ρ", -1, 1),
				new Among("ενδιαφερ", 18, 1),
				new Among("αναρρ", 18, 1),
				new Among("πατ", -1, 1),
				new Among("καθαρευ", -1, 1),
				new Among("δευτερευ", -1, 1),
				new Among("λεχ", -1, 1)
			};
			a_58 = new Among[3]
			{
				new Among("ουσα", -1, 1),
				new Among("ουσε", -1, 1),
				new Among("ουσεσ", -1, 1)
			};
			a_59 = new Among[2]
			{
				new Among("ψοφ", -1, -1),
				new Among("ναυλοχ", -1, -1)
			};
			a_60 = new Among[10]
			{
				new Among("πελ", -1, 1),
				new Among("λλ", -1, 1),
				new Among("σμην", -1, 1),
				new Among("ρπ", -1, 1),
				new Among("πρ", -1, 1),
				new Among("φρ", -1, 1),
				new Among("χορτ", -1, 1),
				new Among("οφ", -1, 1),
				new Among("σφ", -1, 1),
				new Among("λοχ", -1, 1)
			};
			a_61 = new Among[44]
			{
				new Among("αμαλλι", -1, 1),
				new Among("λ", -1, 1),
				new Among("αμαλ", 1, 1),
				new Among("μ", -1, 1),
				new Among("ουλαμ", 3, 1),
				new Among("εν", -1, 1),
				new Among("δερβεν", 5, 1),
				new Among("π", -1, 1),
				new Among("αειπ", 7, 1),
				new Among("αρτιπ", 7, 1),
				new Among("συμπ", 7, 1),
				new Among("νεοπ", 7, 1),
				new Among("κροκαλοπ", 7, 1),
				new Among("ολοπ", 7, 1),
				new Among("προσωποπ", 7, 1),
				new Among("σιδηροπ", 7, 1),
				new Among("δροσοπ", 7, 1),
				new Among("ασπ", 7, 1),
				new Among("ανυπ", 7, 1),
				new Among("ρ", -1, 1),
				new Among("ασπαρ", 19, 1),
				new Among("χαρ", 19, 1),
				new Among("αχαρ", 21, 1),
				new Among("απερ", 19, 1),
				new Among("τρ", 19, 1),
				new Among("ουρ", 19, 1),
				new Among("τ", -1, 1),
				new Among("διατ", 26, 1),
				new Among("επιτ", 26, 1),
				new Among("συντ", 26, 1),
				new Among("ομοτ", 26, 1),
				new Among("νομοτ", 30, 1),
				new Among("αποτ", 26, 1),
				new Among("υποτ", 26, 1),
				new Among("αβαστ", 26, 1),
				new Among("αιμοστ", 26, 1),
				new Among("προστ", 26, 1),
				new Among("ανυστ", 26, 1),
				new Among("ναυ", -1, 1),
				new Among("αφ", -1, 1),
				new Among("ξεφ", -1, 1),
				new Among("αδηφ", -1, 1),
				new Among("παμφ", -1, 1),
				new Among("πολυφ", -1, 1)
			};
			a_62 = new Among[3]
			{
				new Among("αγα", -1, 1),
				new Among("αγε", -1, 1),
				new Among("αγεσ", -1, 1)
			};
			a_63 = new Among[3]
			{
				new Among("ησα", -1, 1),
				new Among("ησε", -1, 1),
				new Among("ησου", -1, 1)
			};
			a_64 = new Among[6]
			{
				new Among("ν", -1, 1),
				new Among("δωδεκαν", 0, 1),
				new Among("επταν", 0, 1),
				new Among("μεγαλον", 0, 1),
				new Among("ερημον", 0, 1),
				new Among("χερσον", 0, 1)
			};
			a_65 = new Among[1]
			{
				new Among("ηστε", -1, 1)
			};
			a_66 = new Among[10]
			{
				new Among("σβ", -1, 1),
				new Among("ασβ", 0, 1),
				new Among("απλ", -1, 1),
				new Among("αειμν", -1, 1),
				new Among("χρ", -1, 1),
				new Among("αχρ", 4, 1),
				new Among("κοινοχρ", 4, 1),
				new Among("δυσχρ", 4, 1),
				new Among("ευχρ", 4, 1),
				new Among("παλιμψ", -1, 1)
			};
			a_67 = new Among[3]
			{
				new Among("ουνε", -1, 1),
				new Among("ηθουνε", 0, 1),
				new Among("ησουνε", 0, 1)
			};
			a_68 = new Among[6]
			{
				new Among("σπι", -1, 1),
				new Among("ν", -1, 1),
				new Among("εξων", 1, 1),
				new Among("ρ", -1, 1),
				new Among("στραβομουτσ", -1, 1),
				new Among("κακομουτσ", -1, 1)
			};
			a_69 = new Among[3]
			{
				new Among("ουμε", -1, 1),
				new Among("ηθουμε", 0, 1),
				new Among("ησουμε", 0, 1)
			};
			a_70 = new Among[7]
			{
				new Among("αζ", -1, 1),
				new Among("ωριοπλ", -1, 1),
				new Among("ασουσ", -1, 1),
				new Among("παρασουσ", 2, 1),
				new Among("αλλοσουσ", -1, 1),
				new Among("φ", -1, 1),
				new Among("χ", -1, 1)
			};
			a_71 = new Among[3]
			{
				new Among("ματα", -1, 1),
				new Among("ματων", -1, 1),
				new Among("ματοσ", -1, 1)
			};
			a_72 = new Among[84]
			{
				new Among("α", -1, 1),
				new Among("ιουμα", 0, 1),
				new Among("ομουνα", 0, 1),
				new Among("ιομουνα", 2, 1),
				new Among("οσουνα", 0, 1),
				new Among("ιοσουνα", 4, 1),
				new Among("ε", -1, 1),
				new Among("αγατε", 6, 1),
				new Among("ηκατε", 6, 1),
				new Among("ηθηκατε", 8, 1),
				new Among("ησατε", 6, 1),
				new Among("ουσατε", 6, 1),
				new Among("ειτε", 6, 1),
				new Among("ηθειτε", 12, 1),
				new Among("ιεμαστε", 6, 1),
				new Among("ουμαστε", 6, 1),
				new Among("ιουμαστε", 15, 1),
				new Among("ιεσαστε", 6, 1),
				new Among("οσαστε", 6, 1),
				new Among("ιοσαστε", 18, 1),
				new Among("η", -1, 1),
				new Among("ι", -1, 1),
				new Among("αμαι", 21, 1),
				new Among("ιεμαι", 21, 1),
				new Among("ομαι", 21, 1),
				new Among("ουμαι", 21, 1),
				new Among("ασαι", 21, 1),
				new Among("εσαι", 21, 1),
				new Among("ιεσαι", 27, 1),
				new Among("αται", 21, 1),
				new Among("εται", 21, 1),
				new Among("ιεται", 30, 1),
				new Among("ονται", 21, 1),
				new Among("ουνται", 21, 1),
				new Among("ιουνται", 33, 1),
				new Among("ει", 21, 1),
				new Among("αει", 35, 1),
				new Among("ηθει", 35, 1),
				new Among("ησει", 35, 1),
				new Among("οι", 21, 1),
				new Among("αν", -1, 1),
				new Among("αγαν", 40, 1),
				new Among("ηκαν", 40, 1),
				new Among("ηθηκαν", 42, 1),
				new Among("ησαν", 40, 1),
				new Among("ουσαν", 40, 1),
				new Among("οντουσαν", 45, 1),
				new Among("ιοντουσαν", 46, 1),
				new Among("ονταν", 40, 1),
				new Among("ιονταν", 48, 1),
				new Among("ουνταν", 40, 1),
				new Among("ιουνταν", 50, 1),
				new Among("οταν", 40, 1),
				new Among("ιοταν", 52, 1),
				new Among("ομασταν", 40, 1),
				new Among("ιομασταν", 54, 1),
				new Among("οσασταν", 40, 1),
				new Among("ιοσασταν", 56, 1),
				new Among("ουν", -1, 1),
				new Among("ηθουν", 58, 1),
				new Among("ομουν", 58, 1),
				new Among("ιομουν", 60, 1),
				new Among("ησουν", 58, 1),
				new Among("οσουν", 58, 1),
				new Among("ιοσουν", 63, 1),
				new Among("ων", -1, 1),
				new Among("ηδων", 65, 1),
				new Among("ο", -1, 1),
				new Among("ασ", -1, 1),
				new Among("εσ", -1, 1),
				new Among("ηδεσ", 69, 1),
				new Among("ησεσ", 69, 1),
				new Among("ησ", -1, 1),
				new Among("εισ", -1, 1),
				new Among("ηθεισ", 73, 1),
				new Among("οσ", -1, 1),
				new Among("υσ", -1, 1),
				new Among("ουσ", 76, 1),
				new Among("υ", -1, 1),
				new Among("ου", 78, 1),
				new Among("ω", -1, 1),
				new Among("αω", 80, 1),
				new Among("ηθω", 80, 1),
				new Among("ησω", 80, 1)
			};
			a_73 = new Among[8]
			{
				new Among("οτερ", -1, 1),
				new Among("εστερ", -1, 1),
				new Among("υτερ", -1, 1),
				new Among("ωτερ", -1, 1),
				new Among("οτατ", -1, 1),
				new Among("εστατ", -1, 1),
				new Among("υτατ", -1, 1),
				new Among("ωτατ", -1, 1)
			};
		}

		private bool r_has_min_length()
		{
			if (current.Length < 3)
			{
				return false;
			}
			return true;
		}

		private bool r_tolower()
		{
			int num;
			while (true)
			{
				num = limit - cursor;
				ket = cursor;
				int num2 = find_among_b(a_0);
				if (num2 == 0)
				{
					break;
				}
				bra = cursor;
				switch (num2)
				{
				default:
					continue;
				case 1:
					slice_from("α");
					continue;
				case 2:
					slice_from("β");
					continue;
				case 3:
					slice_from("γ");
					continue;
				case 4:
					slice_from("δ");
					continue;
				case 5:
					slice_from("ε");
					continue;
				case 6:
					slice_from("ζ");
					continue;
				case 7:
					slice_from("η");
					continue;
				case 8:
					slice_from("θ");
					continue;
				case 9:
					slice_from("ι");
					continue;
				case 10:
					slice_from("κ");
					continue;
				case 11:
					slice_from("λ");
					continue;
				case 12:
					slice_from("μ");
					continue;
				case 13:
					slice_from("ν");
					continue;
				case 14:
					slice_from("ξ");
					continue;
				case 15:
					slice_from("ο");
					continue;
				case 16:
					slice_from("π");
					continue;
				case 17:
					slice_from("ρ");
					continue;
				case 18:
					slice_from("σ");
					continue;
				case 19:
					slice_from("τ");
					continue;
				case 20:
					slice_from("υ");
					continue;
				case 21:
					slice_from("φ");
					continue;
				case 22:
					slice_from("χ");
					continue;
				case 23:
					slice_from("ψ");
					continue;
				case 24:
					slice_from("ω");
					continue;
				case 25:
					break;
				}
				if (cursor <= limit_backward)
				{
					break;
				}
				cursor--;
			}
			cursor = limit - num;
			return true;
		}

		private bool r_step1()
		{
			ket = cursor;
			int num = find_among_b(a_1);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				slice_from("φα");
				break;
			case 2:
				slice_from("σκα");
				break;
			case 3:
				slice_from("ολο");
				break;
			case 4:
				slice_from("σο");
				break;
			case 5:
				slice_from("τατο");
				break;
			case 6:
				slice_from("κρε");
				break;
			case 7:
				slice_from("περ");
				break;
			case 8:
				slice_from("τερ");
				break;
			case 9:
				slice_from("φω");
				break;
			case 10:
				slice_from("καθεστ");
				break;
			case 11:
				slice_from("γεγον");
				break;
			}
			B_test1 = false;
			return true;
		}

		private bool r_steps1()
		{
			ket = base.cursor;
			if (find_among_b(a_4) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_2) != 0)
			{
				bra = base.cursor;
				if (base.cursor <= limit_backward)
				{
					slice_to(S_s);
					slice_from("ι");
					int cursor = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor;
					goto IL_013c;
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (find_among_b(a_3) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ιζ");
			int cursor2 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor2;
			goto IL_013c;
			IL_013c:
			return true;
		}

		private bool r_steps2()
		{
			ket = base.cursor;
			if (find_among_b(a_6) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_5) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ων");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_steps3()
		{
			ket = base.cursor;
			if (find_among_b(a_9) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			if (eq_s_b("ισα") && base.cursor <= limit_backward)
			{
				slice_from("ισ");
			}
			else
			{
				base.cursor = limit - num;
				ket = base.cursor;
				if (find_among_b(a_7) != 0)
				{
					bra = base.cursor;
					if (base.cursor <= limit_backward)
					{
						slice_to(S_s);
						slice_from("ι");
						int cursor = base.cursor;
						insert(base.cursor, base.cursor, S_s);
						base.cursor = cursor;
						goto IL_0175;
					}
				}
				base.cursor = limit - num;
				ket = base.cursor;
				if (find_among_b(a_8) == 0)
				{
					return false;
				}
				bra = base.cursor;
				if (base.cursor > limit_backward)
				{
					return false;
				}
				slice_to(S_s);
				slice_from("ισ");
				int cursor2 = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor2;
			}
			goto IL_0175;
			IL_0175:
			return true;
		}

		private bool r_steps4()
		{
			ket = base.cursor;
			if (find_among_b(a_11) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_10) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ι");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_steps5()
		{
			ket = base.cursor;
			if (find_among_b(a_14) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_12) != 0)
			{
				bra = base.cursor;
				if (base.cursor <= limit_backward)
				{
					slice_to(S_s);
					slice_from("ι");
					int cursor = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor;
					goto IL_013c;
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (find_among_b(a_13) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ιστ");
			int cursor2 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor2;
			goto IL_013c;
			IL_013c:
			return true;
		}

		private bool r_steps6()
		{
			ket = base.cursor;
			if (find_among_b(a_18) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_15) != 0)
			{
				bra = base.cursor;
				if (base.cursor <= limit_backward)
				{
					slice_to(S_s);
					slice_from("ισμ");
					int cursor = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor;
					goto IL_022a;
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (find_among_b(a_16) != 0)
			{
				bra = base.cursor;
				if (base.cursor <= limit_backward)
				{
					slice_to(S_s);
					slice_from("ι");
					int cursor2 = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor2;
					goto IL_022a;
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			int num2 = find_among_b(a_17);
			if (num2 == 0)
			{
				return false;
			}
			bra = base.cursor;
			switch (num2)
			{
			case 1:
				slice_from("αγνωστ");
				break;
			case 2:
				slice_from("ατομ");
				break;
			case 3:
				slice_from("γνωστ");
				break;
			case 4:
				slice_from("εθν");
				break;
			case 5:
				slice_from("εκλεκτ");
				break;
			case 6:
				slice_from("σκεπτ");
				break;
			case 7:
				slice_from("τοπ");
				break;
			case 8:
				slice_from("αλεξανδρ");
				break;
			case 9:
				slice_from("βυζαντ");
				break;
			case 10:
				slice_from("θεατρ");
				break;
			}
			goto IL_022a;
			IL_022a:
			return true;
		}

		private bool r_steps7()
		{
			ket = base.cursor;
			if (find_among_b(a_20) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_19) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("αρακ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_steps8()
		{
			ket = base.cursor;
			if (find_among_b(a_23) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_21) != 0)
			{
				bra = base.cursor;
				if (base.cursor <= limit_backward)
				{
					slice_to(S_s);
					slice_from("ακ");
					int cursor = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor;
					goto IL_01ac;
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (find_among_b(a_22) != 0)
			{
				bra = base.cursor;
				if (base.cursor <= limit_backward)
				{
					slice_to(S_s);
					slice_from("ιτσ");
					int cursor2 = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor2;
					goto IL_01ac;
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (!eq_s_b("κορ"))
			{
				return false;
			}
			bra = base.cursor;
			slice_to(S_s);
			slice_from("ιτσ");
			int cursor3 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor3;
			goto IL_01ac;
			IL_01ac:
			return true;
		}

		private bool r_steps9()
		{
			ket = base.cursor;
			if (find_among_b(a_26) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_24) != 0)
			{
				bra = base.cursor;
				if (base.cursor <= limit_backward)
				{
					slice_to(S_s);
					slice_from("ιδ");
					int cursor = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor;
					goto IL_0129;
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (find_among_b(a_25) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_to(S_s);
			slice_from("ιδ");
			int cursor2 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor2;
			goto IL_0129;
			IL_0129:
			return true;
		}

		private bool r_steps10()
		{
			ket = base.cursor;
			if (find_among_b(a_28) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_27) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ισκ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step2a()
		{
			ket = base.cursor;
			if (find_among_b(a_29) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_30) != 0)
			{
				bra = base.cursor;
				return false;
			}
			base.cursor = limit - num;
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, "αδ");
			base.cursor = cursor;
			return true;
		}

		private bool r_step2b()
		{
			ket = base.cursor;
			if (find_among_b(a_31) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			ket = base.cursor;
			if (find_among_b(a_32) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_to(S_s);
			slice_from("εδ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step2c()
		{
			ket = base.cursor;
			if (find_among_b(a_33) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			ket = base.cursor;
			if (find_among_b(a_34) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_to(S_s);
			slice_from("ουδ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step2d()
		{
			ket = base.cursor;
			if (find_among_b(a_35) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_36) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ε");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step3()
		{
			ket = base.cursor;
			if (find_among_b(a_37) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (in_grouping_b(g_v, 945, 969, repeat: false) != 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_to(S_s);
			slice_from("ι");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step4()
		{
			ket = base.cursor;
			if (find_among_b(a_38) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (in_grouping_b(g_v, 945, 969, repeat: false) == 0)
			{
				bra = base.cursor;
				slice_to(S_s);
				slice_from("ικ");
				int cursor = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor;
			}
			else
			{
				base.cursor = limit - num;
				ket = base.cursor;
			}
			if (find_among_b(a_39) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ικ");
			int cursor2 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor2;
			return true;
		}

		private bool r_step5a()
		{
			int num = limit - base.cursor;
			if (eq_s_b("αγαμε") && base.cursor <= limit_backward)
			{
				slice_from("αγαμ");
			}
			base.cursor = limit - num;
			int num2 = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_40) != 0)
			{
				bra = base.cursor;
				slice_del();
				B_test1 = false;
			}
			base.cursor = limit - num2;
			ket = base.cursor;
			if (!eq_s_b("αμε"))
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_41) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("αμ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step5b()
		{
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_43) != 0)
			{
				bra = base.cursor;
				slice_del();
				B_test1 = false;
				ket = base.cursor;
				if (find_among_b(a_42) != 0)
				{
					bra = base.cursor;
					if (base.cursor <= limit_backward)
					{
						slice_to(S_s);
						slice_from("αγαν");
						int cursor = base.cursor;
						insert(base.cursor, base.cursor, S_s);
						base.cursor = cursor;
					}
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (!eq_s_b("ανε"))
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num2 = limit - base.cursor;
			ket = base.cursor;
			if (in_grouping_b(g_v2, 945, 969, repeat: false) == 0)
			{
				bra = base.cursor;
				slice_to(S_s);
				slice_from("αν");
				int cursor2 = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor2;
			}
			else
			{
				base.cursor = limit - num2;
				ket = base.cursor;
			}
			if (find_among_b(a_44) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("αν");
			int cursor3 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor3;
			return true;
		}

		private bool r_step5c()
		{
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_45) != 0)
			{
				bra = base.cursor;
				slice_del();
				B_test1 = false;
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (!eq_s_b("ετε"))
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num2 = limit - base.cursor;
			ket = base.cursor;
			if (in_grouping_b(g_v2, 945, 969, repeat: false) == 0)
			{
				bra = base.cursor;
				slice_to(S_s);
				slice_from("ετ");
				int cursor = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor;
			}
			else
			{
				base.cursor = limit - num2;
				ket = base.cursor;
				if (find_among_b(a_46) != 0)
				{
					bra = base.cursor;
					slice_to(S_s);
					slice_from("ετ");
					int cursor2 = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor2;
				}
				else
				{
					base.cursor = limit - num2;
					ket = base.cursor;
				}
			}
			if (find_among_b(a_47) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ετ");
			int cursor3 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor3;
			return true;
		}

		private bool r_step5d()
		{
			ket = base.cursor;
			if (find_among_b(a_48) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (eq_s_b("αρχ"))
			{
				bra = base.cursor;
				if (base.cursor <= limit_backward)
				{
					slice_to(S_s);
					slice_from("οντ");
					int cursor = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor;
					goto IL_0127;
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (!eq_s_b("κρε"))
			{
				return false;
			}
			bra = base.cursor;
			slice_to(S_s);
			slice_from("ωντ");
			int cursor2 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor2;
			goto IL_0127;
			IL_0127:
			return true;
		}

		private bool r_step5e()
		{
			ket = base.cursor;
			if (find_among_b(a_49) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (!eq_s_b("ον"))
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ομαστ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step5f()
		{
			int num = limit - base.cursor;
			ket = base.cursor;
			if (eq_s_b("ιεστε"))
			{
				bra = base.cursor;
				slice_del();
				B_test1 = false;
				ket = base.cursor;
				if (find_among_b(a_50) != 0)
				{
					bra = base.cursor;
					if (base.cursor <= limit_backward)
					{
						slice_to(S_s);
						slice_from("ιεστ");
						int cursor = base.cursor;
						insert(base.cursor, base.cursor, S_s);
						base.cursor = cursor;
					}
				}
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (!eq_s_b("εστε"))
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_51) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ιεστ");
			int cursor2 = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor2;
			return true;
		}

		private bool r_step5g()
		{
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_52) != 0)
			{
				bra = base.cursor;
				slice_del();
				B_test1 = false;
			}
			base.cursor = limit - num;
			ket = base.cursor;
			if (find_among_b(a_55) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num2 = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_53) != 0)
			{
				bra = base.cursor;
				slice_to(S_s);
				slice_from("ηκ");
				int cursor = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor;
			}
			else
			{
				base.cursor = limit - num2;
				ket = base.cursor;
				if (find_among_b(a_54) == 0)
				{
					return false;
				}
				bra = base.cursor;
				if (base.cursor > limit_backward)
				{
					return false;
				}
				slice_to(S_s);
				slice_from("ηκ");
				int cursor2 = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor2;
			}
			return true;
		}

		private bool r_step5h()
		{
			ket = base.cursor;
			if (find_among_b(a_58) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (find_among_b(a_56) != 0)
			{
				bra = base.cursor;
				slice_to(S_s);
				slice_from("ουσ");
				int cursor = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor;
			}
			else
			{
				base.cursor = limit - num;
				ket = base.cursor;
				if (find_among_b(a_57) == 0)
				{
					return false;
				}
				bra = base.cursor;
				if (base.cursor > limit_backward)
				{
					return false;
				}
				slice_to(S_s);
				slice_from("ουσ");
				int cursor2 = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor2;
			}
			return true;
		}

		private bool r_step5i()
		{
			ket = base.cursor;
			if (find_among_b(a_62) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			int num = limit - base.cursor;
			ket = base.cursor;
			if (eq_s_b("κολλ"))
			{
				bra = base.cursor;
				slice_to(S_s);
				slice_from("αγ");
				int cursor = base.cursor;
				insert(base.cursor, base.cursor, S_s);
				base.cursor = cursor;
			}
			else
			{
				base.cursor = limit - num;
				int num2 = limit - base.cursor;
				ket = base.cursor;
				if (find_among_b(a_59) != 0)
				{
					bra = base.cursor;
					return false;
				}
				base.cursor = limit - num2;
				int num3 = limit - base.cursor;
				ket = base.cursor;
				if (find_among_b(a_60) != 0)
				{
					bra = base.cursor;
					slice_to(S_s);
					slice_from("αγ");
					int cursor2 = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor2;
				}
				else
				{
					base.cursor = limit - num3;
					ket = base.cursor;
					if (find_among_b(a_61) == 0)
					{
						return false;
					}
					bra = base.cursor;
					if (base.cursor > limit_backward)
					{
						return false;
					}
					slice_to(S_s);
					slice_from("αγ");
					int cursor3 = base.cursor;
					insert(base.cursor, base.cursor, S_s);
					base.cursor = cursor3;
				}
			}
			return true;
		}

		private bool r_step5j()
		{
			ket = base.cursor;
			if (find_among_b(a_63) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_64) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ησ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step5k()
		{
			ket = base.cursor;
			if (find_among_b(a_65) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_66) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ηστ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step5l()
		{
			ket = base.cursor;
			if (find_among_b(a_67) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_68) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ουν");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step5m()
		{
			ket = base.cursor;
			if (find_among_b(a_69) == 0)
			{
				return false;
			}
			bra = base.cursor;
			slice_del();
			B_test1 = false;
			ket = base.cursor;
			if (find_among_b(a_70) == 0)
			{
				return false;
			}
			bra = base.cursor;
			if (base.cursor > limit_backward)
			{
				return false;
			}
			slice_to(S_s);
			slice_from("ουμ");
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, S_s);
			base.cursor = cursor;
			return true;
		}

		private bool r_step6()
		{
			int num = limit - cursor;
			ket = cursor;
			if (find_among_b(a_71) != 0)
			{
				bra = cursor;
				slice_from("μα");
			}
			cursor = limit - num;
			if (!B_test1)
			{
				return false;
			}
			ket = cursor;
			if (find_among_b(a_72) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_step7()
		{
			ket = cursor;
			if (find_among_b(a_73) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			return true;
		}

		protected override bool stem()
		{
			limit_backward = cursor;
			cursor = limit;
			int num = limit - cursor;
			r_tolower();
			cursor = limit - num;
			if (!r_has_min_length())
			{
				return false;
			}
			B_test1 = true;
			int num2 = limit - cursor;
			r_step1();
			cursor = limit - num2;
			int num3 = limit - cursor;
			r_steps1();
			cursor = limit - num3;
			int num4 = limit - cursor;
			r_steps2();
			cursor = limit - num4;
			int num5 = limit - cursor;
			r_steps3();
			cursor = limit - num5;
			int num6 = limit - cursor;
			r_steps4();
			cursor = limit - num6;
			int num7 = limit - cursor;
			r_steps5();
			cursor = limit - num7;
			int num8 = limit - cursor;
			r_steps6();
			cursor = limit - num8;
			int num9 = limit - cursor;
			r_steps7();
			cursor = limit - num9;
			int num10 = limit - cursor;
			r_steps8();
			cursor = limit - num10;
			int num11 = limit - cursor;
			r_steps9();
			cursor = limit - num11;
			int num12 = limit - cursor;
			r_steps10();
			cursor = limit - num12;
			int num13 = limit - cursor;
			r_step2a();
			cursor = limit - num13;
			int num14 = limit - cursor;
			r_step2b();
			cursor = limit - num14;
			int num15 = limit - cursor;
			r_step2c();
			cursor = limit - num15;
			int num16 = limit - cursor;
			r_step2d();
			cursor = limit - num16;
			int num17 = limit - cursor;
			r_step3();
			cursor = limit - num17;
			int num18 = limit - cursor;
			r_step4();
			cursor = limit - num18;
			int num19 = limit - cursor;
			r_step5a();
			cursor = limit - num19;
			int num20 = limit - cursor;
			r_step5b();
			cursor = limit - num20;
			int num21 = limit - cursor;
			r_step5c();
			cursor = limit - num21;
			int num22 = limit - cursor;
			r_step5d();
			cursor = limit - num22;
			int num23 = limit - cursor;
			r_step5e();
			cursor = limit - num23;
			int num24 = limit - cursor;
			r_step5f();
			cursor = limit - num24;
			int num25 = limit - cursor;
			r_step5g();
			cursor = limit - num25;
			int num26 = limit - cursor;
			r_step5h();
			cursor = limit - num26;
			int num27 = limit - cursor;
			r_step5j();
			cursor = limit - num27;
			int num28 = limit - cursor;
			r_step5i();
			cursor = limit - num28;
			int num29 = limit - cursor;
			r_step5k();
			cursor = limit - num29;
			int num30 = limit - cursor;
			r_step5l();
			cursor = limit - num30;
			int num31 = limit - cursor;
			r_step5m();
			cursor = limit - num31;
			int num32 = limit - cursor;
			r_step6();
			cursor = limit - num32;
			int num33 = limit - cursor;
			r_step7();
			cursor = limit - num33;
			cursor = limit_backward;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("el", () => new Stemmer_el(), 533686725);
				}
				_registered = true;
			}
		}
	}
}
