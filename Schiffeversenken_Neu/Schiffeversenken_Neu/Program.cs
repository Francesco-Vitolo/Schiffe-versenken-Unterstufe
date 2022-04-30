using System;

namespace Schiffeversenken_Neu
{
    class Program
    {
        static int größespielfeld = 10; //Größe Spielfeld

        static string[,] SpielfeldSpieler;
        static string[,] SpielfeldBot;
        static string[] SchiffeSpieler = new string[4];
        static string[] SchiffeBot = new string[4];
        static int[] Schiffelänge = { 3, 4, 5, 6 };

        static bool unsichtbar = true;
        static bool spielerAmZug = false;
        static int eingabeXachse = 0, zahlEingabeYachse = 0;
        static char buchstabeEingabeYachse = '/';

        static int[] MunitionBot = { 9999, 9999, 9999, 9999 };
        static int[] MunitionSpieler = { 9999, 9999, 9999, 9999 };

        static bool spielerverloren = false;
        static bool botverloren = false;

        static Schwierigkeit level;
        static int score = 0;
        static int highscore = 0;

        static bool neustart = false;

        enum Waffenart
        {
            Kanone = 1,
            Rakete,
            Seemine,
            Torpedo
        }

        enum Schwierigkeit
        {
            leicht = 1,
            mittel,
            schwer,
            hardcore,
            custom
        }

        static void Main(string[] args)
        {
            do
            {
                Console.CursorVisible = false;
                int spielerwechsel = 1;
                score = 0;
                spielerAmZug = false;
                spielerverloren = false;
                botverloren = false;


                LevelAuswaehlen();

                ErstelleSpielfeld(größespielfeld);
                SchiffePlatzieren(ref SpielfeldBot, ref SchiffeBot);
                Autoplatzieren();
                do
                {
                    spielerwechsel++;
                    if (spielerwechsel % 2 == 0)
                    {
                        spielerAmZug = false;
                    }
                    else
                    {
                        spielerAmZug = true;
                    }

                    if (spielerAmZug)
                    {
                        SpielerGibtSchussAb();
                    }
                    else
                    {
                        BotgibtSchussAb();
                    }
                    //BotBekommtNullMuni();

                    if (PrüfenObMunitionVerbraucht(MunitionBot) || PrüfenObSchiffeVersenkt(SchiffeBot))
                    {
                        botverloren = true;
                        score = score + 50;
                        Console.Clear();
                        Console.WriteLine("Bot hat verloren");
                    }

                    if (PrüfenObMunitionVerbraucht(MunitionSpieler) || PrüfenObSchiffeVersenkt(SchiffeSpieler))
                    {
                        spielerverloren = true;
                        Console.Clear();
                        Console.WriteLine("Spieler hat verloren");
                    }

                } while (botverloren == false && spielerverloren == false);

                ScoreBerechnen();
                PrüfenObHighscore();
                Console.WriteLine("score: {0}\thighscore: {1}",score, highscore);
                NeuesSpiel();
            } while (neustart == true);
        }

    //Neu
        static void ScoreBerechnen()
        {
            if (level == Schwierigkeit.leicht)
            {
                score = 100;
            }
            else
            {
                foreach (string s in SpielfeldBot)
                {
                    if (s == "X")
                        score++;
                }
                score = score + (MunitionSpieler[0] / 2);
                score = score + MunitionSpieler[1];
                score = score + (MunitionSpieler[2] * 2);
                score = score + (MunitionSpieler[3] * 5);
            }

            
        }
        static void NeuesSpiel()
        {
            Console.WriteLine("Erneut spielen?\nEnter - ja\tEscape - nein");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter)
            {
                neustart = true;
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                neustart = false;
            }
            else
            {
                Console.Clear();
                NeuesSpiel();
            }
            Console.Clear();
        }
        static void PrüfenObHighscore()
        {
            if(score > highscore)
            {
                highscore = score;
            }
        }
    //Neu

   // Level
        static void LevelAuswaehlen()
        {
            Console.WriteLine("Level auswählen:\nleicht - 1\tschwer - 3\nmittel - 2\thardcore - 4\n\tcustom - 5");
            int l = Convert.ToInt32(Console.ReadLine());
            level = (Schwierigkeit)l;

            switch (level)
            {
                case Schwierigkeit.leicht:
                break;

                case Schwierigkeit.mittel:
                    LevelMittel();
                    break;

                case Schwierigkeit.schwer:
                    LevelSchwer();
                    break;

                case Schwierigkeit.hardcore:
                    LevelHardcore();
                    break;

                case Schwierigkeit.custom:
                    LevelCustom();
                    break;
            }
        }

        static void LevelMittel()
        {
            MunitionBot[0] = 20;
            MunitionBot[1] = 2;
            MunitionBot[2] = 2;
            MunitionBot[3] = 2;

            MunitionSpieler[0] = 20;
            MunitionSpieler[1] = 2;
            MunitionSpieler[2] = 2;
            MunitionSpieler[3] = 2;
            score = score + 100;
        }

        static void LevelSchwer()
        {
            MunitionBot[0] = 10;
            MunitionBot[1] = 3;
            MunitionBot[2] = 2;
            MunitionBot[3] = 1;

            MunitionSpieler[0] = 10;
            MunitionSpieler[1] = 3;
            MunitionSpieler[2] = 2;
            MunitionSpieler[3] = 1;

            Schiffelänge[0] = 1;
            Schiffelänge[1] = 2;
            Schiffelänge[2] = 3;
            Schiffelänge[3] = 0;
            score = score + 200;
        }

        static void LevelHardcore()
        {
            größespielfeld = 26;

            MunitionBot[0] = 20;
            MunitionBot[1] = 6;
            MunitionBot[2] = 7;
            MunitionBot[3] = 4;

            MunitionSpieler[0] = 20;
            MunitionSpieler[1] = 6;
            MunitionSpieler[2] = 7;
            MunitionSpieler[3] = 4;
            score = score + 300;
        }

        static void LevelCustom()
        {
            Console.Write("Größe Spielfeld eingeben: ");
            größespielfeld = Convert.ToInt32(Console.ReadLine());

            Console.Write("Munition Kanone: ");
            MunitionBot[0] = Convert.ToInt32(Console.ReadLine());
            MunitionSpieler[0] = MunitionBot[0];

            Console.Write("Munition Rakete: ");
            MunitionBot[1] = Convert.ToInt32(Console.ReadLine());
            MunitionSpieler[1] = MunitionBot[1];

            Console.Write("Munition Seemine: ");
            MunitionBot[2] = Convert.ToInt32(Console.ReadLine());
            MunitionSpieler[2] = MunitionBot[2];

            Console.Write("Munition Torpedo: ");
            MunitionBot[3] = Convert.ToInt32(Console.ReadLine());
            MunitionSpieler[3] = MunitionBot[3];

            Console.WriteLine("Länge Schiffe eingeben: ");
            Console.Write("Schiff 1: ");
            Schiffelänge[0] = Convert.ToInt32(Console.ReadLine());
            Console.Write("Schiff 2: ");
            Schiffelänge[1] = Convert.ToInt32(Console.ReadLine());
            Console.Write("Schiff 3: ");
            Schiffelänge[2] = Convert.ToInt32(Console.ReadLine());
            Console.Write("Schiff 4: ");
            Schiffelänge[3] = Convert.ToInt32(Console.ReadLine());
        }
   //Level


   //Initialisieren
        static void ErstelleSpielfeld(int größespielfeld)
        {
            SpielfeldSpieler = new string[größespielfeld, größespielfeld];
            SpielfeldBot = new string[größespielfeld, größespielfeld];

            for (int i = 0; i < SpielfeldSpieler.GetLength(0); i++)
            {
                for (int j = 0; j < SpielfeldSpieler.GetLength(1); j++)
                {
                    SpielfeldSpieler[i, j] = "~";
                    SpielfeldBot[i, j] = "~";
                }
            }
        }

        static void Autoplatzieren()
        {
            unsichtbar = false;
            Console.WriteLine("\n    Automatisch platzieren?\n     (j = ja)\t(n = nein)");
            ConsoleKeyInfo  key = Console.ReadKey();
            if(key.Key == ConsoleKey.J)
            {
                spielerAmZug = false;
                SchiffePlatzieren(ref SpielfeldSpieler, ref SchiffeSpieler);
                Console.Clear();
                Console.WriteLine("SpielfeldSpieler");
                SpielfeldAusgeben(SpielfeldSpieler);
                Console.ReadKey();
            }
            else if((key.Key == ConsoleKey.N))
            {
                spielerAmZug = true;
                SchiffePlatzieren(ref SpielfeldSpieler, ref SchiffeSpieler);
            }
            else
            {
                Console.Clear();
                Autoplatzieren();
            }
            unsichtbar = true;
        }
   //Initialisieren


   //Ausgabe
        static void SpielfeldAusgeben(string [,] Spielfeld)
        {            
            Console.Write("   ");
            for (int xachseausgeben = 1; xachseausgeben < größespielfeld + 1; xachseausgeben++)        //x-Achse ausgeben
            {
                Console.Write(xachseausgeben + " ");
                if (xachseausgeben < 10)
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();

            char buchstabe = '@'; // utf-16 eins vor 'A'
            for (int i = 0; i < größespielfeld; i++)
            {
                buchstabe++;                        //y-Achse ausgeben
                Console.Write(buchstabe + "  ");    //y-Achse asugeben
                for (int j = 0; j < Spielfeld.GetLength(1); j++)
                {
                    if (!unsichtbar)    // bei Schussabgeben();
                    {
                        Console.Write(Spielfeld[j, i] + "  "); //Feld ausgeben
                    }
                    else // wenn unsichtbar = true
                    {
                        if (Spielfeld[j, i] == "X" || Spielfeld[j, i] == "O")
                        {
                            Console.Write(Spielfeld[j, i] + "  ");
                        }
                        else // wenn Spielfeld[i,j] == "S" --> "~"
                        {
                            Console.Write("~  ");
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        static void MunitionAusgeben(ref int[] Munition)
        {
            Console.Write("\tKanone\tRakete\tSeemine\tTorpedo\nMuni:   ");
            foreach (int m in Munition)
            {
                Console.Write(m + "\t");
            }
        }
   //Ausgabe

        static void Kontrolle()
        {
            Console.WriteLine("\nKontrolle Schiffe Spieler");
            foreach (string s in SchiffeSpieler)
            {
                Console.Write("( " + s + ")");
            }
            Console.WriteLine("\nKontrolle Schiffe Bot");
            foreach (string s in SchiffeBot)
            {
                Console.Write("( " + s + ")");
            }
        }


   //Eingabe
        static void Koordinateneingeben()
        {
            eingabeXachse = 0;
            buchstabeEingabeYachse = '/';
            zahlEingabeYachse = 0;
            do
            {
                try
                {
                    Console.Write("Zahl eingeben (Startposition): "); // x - Achse
                    eingabeXachse = Convert.ToInt32(Console.ReadLine());
                }
                catch { }
            } while (eingabeXachse > größespielfeld || eingabeXachse == 0);
            eingabeXachse--; // für Array (beginnt bei 0)
            do
            {
                try
                {
                    Console.Write("Buchstabe eingeben (Startposition): ");  // y - Achse
                    buchstabeEingabeYachse = Convert.ToChar(Console.ReadLine());
                    buchstabeEingabeYachse = char.ToUpper(buchstabeEingabeYachse);  // Buchstabe wird in groß geschrieben
                    zahlEingabeYachse = BuchstabeInZahl(buchstabeEingabeYachse);

                }
                catch { }
            } while (buchstabeEingabeYachse < '\u0040' || zahlEingabeYachse + 1 > größespielfeld);
        }

        static bool SchiffePlatzieren(ref string[,] Spielfeld, ref string[] Schiffe)
        {
            //vertikal platzieren
            int x = 0, y = 0;
            bool platzierenmöglich = true;
            string schiff = "";
            int anzahlschiffe = 0;
            do
            {
                int länge = Schiffelänge[anzahlschiffe];
                schiff = "";
                if (spielerAmZug)
                {
                    Console.Clear();
                    SpielfeldAusgeben(Spielfeld);
                    Console.WriteLine("Schiff mit der Länge {0} eingeben (vertikal)", länge);
                    Koordinateneingeben();
                    x = eingabeXachse;
                    y = zahlEingabeYachse;
                }
                else
                {
                    Zufall(ref x, ref y);
                    buchstabeEingabeYachse = ZahlinBuchstabe(y);
                }

                if (y + länge <= Spielfeld.GetLength(1))
                {
                    platzierenmöglich = true;
                    for (int i = 0; i < länge; i++)
                    {
                        if (Spielfeld[x, y + i] == "S")
                        {
                            platzierenmöglich = false;
                        }
                    }
                    if (platzierenmöglich)
                    {
                        for (int i = 0; i < länge; i++)
                        {
                            if (Spielfeld[x, y + i] != "S")
                            {
                                Spielfeld[x, y + i] = "S";
                                schiff = schiff + Convert.ToString(x + 1) + Convert.ToString(buchstabeEingabeYachse) + " ";
                                buchstabeEingabeYachse++;
                            }
                        }                       
                        Schiffe[anzahlschiffe] = schiff;
                        anzahlschiffe++;
                    }
                }
            } while (anzahlschiffe < (Schiffelänge.Length / 2));
            

            //horizontal platzieren
            x = 0;
            y = 0;
            platzierenmöglich = true;
            do
            {
                int länge = Schiffelänge[anzahlschiffe];
                schiff = "";
                if (spielerAmZug)
                {
                    Console.Clear();
                    SpielfeldAusgeben(Spielfeld);
                    Console.WriteLine("Schiff mit der Länge {0} eingeben (horizontal)", länge);
                    Koordinateneingeben();
                    x = eingabeXachse;
                    y = zahlEingabeYachse;
                }
                else
                {
                    Zufall(ref x, ref y);
                    buchstabeEingabeYachse = ZahlinBuchstabe(y);
                }

                if (x + länge <= Spielfeld.GetLength(0))
                {
                    platzierenmöglich = true;
                    for (int i = 0; i < länge; i++)
                    {
                        if (Spielfeld[x + i, y] == "S")
                        {
                            platzierenmöglich = false;
                        }
                    }
                    if (platzierenmöglich)
                    {
                        for (int i = 0; i < länge; i++)
                        {
                            if (Spielfeld[x, y] != "S")
                            {
                                Spielfeld[x, y] = "S";
                                schiff = schiff + Convert.ToString(x + 1) + Convert.ToString(buchstabeEingabeYachse) + " ";
                                x++;
                            }
                        }                        
                        Schiffe[anzahlschiffe] = schiff;
                        anzahlschiffe++;
                    }
                }

            } while (anzahlschiffe < Schiffelänge.Length);  
            return true;
        }
       

        //Schuss abgeben
        static void SchussAbgeben(ref int[] Munition)
        {
            int x = 0;
            int y = 0;
            bool belegt = false;
            Console.WriteLine("\nSchuss abgeben");
            if (spielerAmZug)
            {
                Koordinateneingeben();
                if (SpielfeldBot[eingabeXachse, zahlEingabeYachse] == "X" || SpielfeldBot[eingabeXachse, zahlEingabeYachse] == "O") // Prüfen, ob Feld belegt
                {
                    Console.WriteLine("Es wurde schon auf das Feld geschossen");
                    SchussAbgeben(ref Munition);
                    belegt = true;
                }
            }
            else
            {
                Zufall(ref x, ref y);
                if (SpielfeldSpieler[x, y] == "X" || SpielfeldSpieler[x, y] == "O") // Prüfen, ob Feld belegt
                {
                    SchussAbgeben(ref Munition);
                    belegt = true;
                }                
                eingabeXachse = x;
                zahlEingabeYachse = y;

            }
            if (!belegt)
            {
                Waffenart wa = WaffeAuswaehlen();
                switch (wa)
                {
                    case Waffenart.Kanone:
                        if (Munition[0] != 0)
                        {
                            if (spielerAmZug == true)
                            {
                                SchussAbgebenKanone(ref SpielfeldBot, ref SchiffeBot);
                            }
                            else
                            {
                                SchussAbgebenKanone(ref SpielfeldSpieler, ref SchiffeSpieler);
                            }
                            Munition[0]--;
                        }
                        else
                        {
                            Console.WriteLine("Keine Munition vorhanden");
                            SchussAbgeben(ref Munition);
                            Console.ReadKey();
                        }
                        break;



                    case Waffenart.Rakete:
                        if (Munition[1] != 0)
                        {
                            if (spielerAmZug == true)
                            {
                                SchussAbgebenRakete(ref SpielfeldBot, ref SchiffeBot);
                            }
                            else
                            {
                                SchussAbgebenRakete(ref SpielfeldSpieler, ref SchiffeSpieler);
                            }
                            Munition[1]--;
                        }
                        else
                        {
                            Console.WriteLine("Keine Munition vorhanden");
                            SchussAbgeben(ref Munition);
                            Console.ReadKey();
                        }
                        break;


                    case Waffenart.Seemine:

                        if (Munition[2] != 0)
                        {
                            if (spielerAmZug == true)
                            {
                                SchussAbgebenSeemine(ref SpielfeldBot, ref SchiffeBot);
                            }
                            else
                            {
                                SchussAbgebenSeemine(ref SpielfeldSpieler, ref SchiffeSpieler);
                            }
                            Munition[2]--;
                        }
                        else
                        {
                            Console.WriteLine("Keine Munition vorhanden");
                            SchussAbgeben(ref Munition);
                            Console.ReadKey();
                        }
                        break;


                    case Waffenart.Torpedo:
                        if (Munition[3] != 0)
                        {
                            if (spielerAmZug == true)
                            {
                                SchussAbgebenTorpedo(ref SpielfeldBot, ref SchiffeBot);
                            }
                            else
                            {
                                SchussAbgebenTorpedo(ref SpielfeldSpieler, ref SchiffeSpieler);
                            }
                            Munition[3]--;
                        }
                        else
                        {
                            Console.WriteLine("Keine Munition vorhanden");
                            SchussAbgeben(ref Munition);
                            Console.ReadKey();
                        }
                        break;
                    default:
                        Console.WriteLine("Waffe auswählen");
                        break;
                }
            }
        }

        static Waffenart WaffeAuswaehlen()
        {
            int w = 0;
            if (spielerAmZug)
            {               
                do
                {
                    try
                    {
                        Console.WriteLine("Waffe auswählen:\nKanone  - 1\tRakete - 2\nSeemine - 3\tTorpedo - 4");
                        w = Convert.ToInt32(Console.ReadLine());
                    }
                    catch { }
                } while (w > 4);
            }
            else
            {
                Random rand = new Random();
                w = rand.Next(0, 5);

            }
            Waffenart wa = (Waffenart)w;
            return wa;
        }
        
        static void SchussAbgebenKanone(ref string [,] Spielfeld, ref string [] Schiffe)
        {
            int x = 0;
            int y = 0;
            if (spielerAmZug)
            {
                x = eingabeXachse;
                y = zahlEingabeYachse;
                Console.Clear();
            }
            else
            {
                Zufall(ref x, ref y);
                x++;
                y++;
            }

            if (LiegenKoordinatenAufSpielfeld(x, y))
            {
                if (Spielfeld[x, y] == "S")
                {
                    Spielfeld[x, y] = "X";
                    PrüfenObSchiffGetroffen(x, buchstabeEingabeYachse, ref Schiffe);
                }
                else
                {
                    Spielfeld[x, y] = "O";
                }
            }
        }

        static void SchussAbgebenRakete(ref string[,] Spielfeld, ref string[] Schiffe)
        {
            int x = 0;
            int y = 0;
            if (spielerAmZug)
            {
                x = eingabeXachse;
                y = zahlEingabeYachse;
            }
            else
            {
                Zufall(ref x, ref y);
                x++;
                y++;
            }

            Console.Clear();
            if (Spielfeld[x, y] == "S")
            {
                Spielfeld[x, y] = "X";
                PrüfenObSchiffGetroffen(x, buchstabeEingabeYachse, ref Schiffe);
            }
            else
            {
                Spielfeld[x, y] = "O";
            }

            if (LiegenKoordinatenAufSpielfeld(x, y + 1))
            {
                if (Spielfeld[x, y + 1] == "S")
                {

                    Spielfeld[x, y + 1] = "X";
                    PrüfenObSchiffGetroffen(x, Convert.ToChar(buchstabeEingabeYachse + 1), ref Schiffe);
                }
                else
                {
                    Spielfeld[x, y + 1] = "O";
                }
            }

            if (LiegenKoordinatenAufSpielfeld(x + 1, y))
            {
                if (Spielfeld[x + 1, y] == "S")
                {
                    Spielfeld[x + 1, y] = "X";
                    PrüfenObSchiffGetroffen(x + 1, buchstabeEingabeYachse, ref Schiffe);
                }
                else
                {
                    Spielfeld[x + 1, y] = "O";
                }
            }
        }
        
        static void SchussAbgebenSeemine(ref string[,] Spielfeld, ref string[] Schiffe)
        {
            int x = 0;
            int y = 0;
            if (spielerAmZug)
            {
                x = eingabeXachse;
                y = zahlEingabeYachse;
            }
            else
            {
                Zufall(ref x, ref y);
                x++;
                y++;
            }
            Console.Clear();
            if (Spielfeld[x, y] == "S" )

            {
                Spielfeld[x, y] = "X";
                PrüfenObSchiffGetroffen(x, buchstabeEingabeYachse, ref Schiffe);
            }
            else
            {
                Spielfeld[x, y] = "O";
                if (LiegenKoordinatenAufSpielfeld(x, y + 1))
                {
                    if (Spielfeld[x, y + 1] == "S" || Spielfeld[x, y + 1] == "X") //unten
                    {

                        Spielfeld[x, y + 1] = "X";
                        PrüfenObSchiffGetroffen(x, Convert.ToChar(buchstabeEingabeYachse + 1), ref Schiffe);
                    }
                    else
                    {
                        Spielfeld[x, y + 1] = "O";
                    }
                }

                if (LiegenKoordinatenAufSpielfeld(x+1, y))
                {
                    if (Spielfeld[x + 1, y] == "S" || Spielfeld[x + 1, y] == "X") // rechts
                    {
                        Spielfeld[x + 1, y] = "X";
                        PrüfenObSchiffGetroffen(x + 1, buchstabeEingabeYachse, ref Schiffe);
                    }
                    else
                    {
                        Spielfeld[x + 1, y] = "O";
                    }
                }

                if (x != 0)
                {
                    if (Spielfeld[x - 1, y] == "S" || Spielfeld[x - 1, y] == "X") // links
                    {
                        Spielfeld[x - 1, y] = "X";
                        PrüfenObSchiffGetroffen(x - 1, buchstabeEingabeYachse, ref Schiffe);
                    }
                    else
                    {
                        Spielfeld[x - 1, y] = "O";
                    }
                }

                if (y != 0)
                {
                    if (Spielfeld[x, y - 1] == "S" || Spielfeld[x, y - 1] == "X") //oben
                    {

                        Spielfeld[x, y - 1] = "X";
                        PrüfenObSchiffGetroffen(x, Convert.ToChar(buchstabeEingabeYachse - 1), ref Schiffe);
                    }
                    else
                    {
                        Spielfeld[x, y - 1] = "O";
                    }
                }
            }

        }

        static void SchussAbgebenTorpedo(ref string[,] Spielfeld, ref string[] Schiffe)
        {
            string[] Array = new string[10];
            string koordinaten = "";
            int x = 0;
            int y = 0;
            string a = "";
            string b = "";
            if (spielerAmZug)
            {
                x = eingabeXachse;
                y = zahlEingabeYachse;
            }
            else
            {
                Zufall(ref x , ref y);
                x++;
                y++;
            }

            if (Spielfeld[x,y] == "S" || Spielfeld[x,y] == "X")
            {
                koordinaten = Convert.ToString(x + 1) + Convert.ToString(buchstabeEingabeYachse);
                Console.WriteLine(koordinaten);
                for (int feld = 0; feld < 4; feld++)
                {
                    if (Schiffe[feld].Contains(koordinaten)) // es wird überprüft an welcher Position die Koordinaten sind
                    {
                        Schiffe[feld] = Schiffe[feld].Trim();
                        Array = Schiffe[feld].Split(' ');  // Die Koordinaten werden getrennt wird getrennt und in Array geschrieben

                        foreach (string einzelnekoordinate in Array)
                        {
                            Console.WriteLine(einzelnekoordinate.Length);
                            if (einzelnekoordinate.Length == 3)  // wenn Koordinate größer als 10, dann sind es 3 chars
                            {
                                a = einzelnekoordinate.Remove(2);
                                b = einzelnekoordinate.Remove(0, 2);
                            }
                            else
                            {
                                a = einzelnekoordinate.Remove(1);
                                b = einzelnekoordinate.Remove(0, 1);
                            }
                            int xachse = Convert.ToInt32(a);
                            int yachse =BuchstabeInZahl(Convert.ToChar(b));
                            Spielfeld[xachse - 1, yachse] = "X";
                        }
                    }
                }

                foreach (string ko in Array) // die Koordinaten werden aus dem Array Schiffe gelöscht
                {
                    for (int feld = 0; feld < 4; feld++)
                    {
                        if (Schiffe[feld].Contains(ko))
                        {
                            Schiffe[feld] = Schiffe[feld].Replace(ko, "");
                        }
                    }
                }
            }
            else
            {
                Spielfeld[x,y] = "O";
            }
        }

        static void BotgibtSchussAb()
        {
            SchussAbgeben(ref MunitionBot);
            Console.Clear();

            unsichtbar = false;
            Console.WriteLine("SpielfeldSpieler");
            SpielfeldAusgeben(SpielfeldSpieler);
            unsichtbar = true;

            MunitionAusgeben(ref MunitionBot);
            Console.ReadKey();
        }

        static void SpielerGibtSchussAb()
        {
            unsichtbar = true;
            Console.Clear();
            Console.WriteLine("SpielfeldBot");
            SpielfeldAusgeben(SpielfeldBot);
            MunitionAusgeben(ref MunitionSpieler);
            Kontrolle();
            SchussAbgeben(ref MunitionSpieler);
            Console.Clear();
            Console.WriteLine("SpielfeldBot");
            SpielfeldAusgeben(SpielfeldBot);
            MunitionAusgeben(ref MunitionSpieler);
            Kontrolle();
            Console.ReadKey();
        }
        //Schuss abgeben
   //Eingabe

   //Hintergrund
        static void BotBekommtNullMuni()
        {
            for (int i = 0; i < 4; i++)
            {
                MunitionBot[i] = 0;
            }
        }

        static void Zufall(ref int x, ref int y) //für bot
        {
            Random rand = new Random();
            x = rand.Next(0, größespielfeld - 1);
            y = rand.Next(0, größespielfeld - 1);
        }
        

        //Überprüfungen
        static bool LiegenKoordinatenAufSpielfeld(int x, int y)
        {
            if (x >= größespielfeld || y >= größespielfeld)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static void PrüfenObSchiffGetroffen(int x, char y, ref string [] Schiffe)
        {
            string koordinaten = "";
            koordinaten = Convert.ToString(x + 1) + Convert.ToString(y);
            for (int feld = 0; feld < 4; feld++)
            {
                if (Schiffe[feld].Contains(koordinaten))
                {
                    Schiffe[feld] = Schiffe[feld].Replace(koordinaten, "");
                }
            }
        }

        static bool PrüfenObMunitionVerbraucht (int[] Munition)
        {
            int zaehler = 0;
            foreach (int m in Munition)
            {
                zaehler = zaehler + m;                
            }

            if (zaehler == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        static bool PrüfenObSchiffeVersenkt(string[] Schiffe)
        {
            for (int feld = 0; feld < 4; feld++)
            {
                Schiffe[feld] = Schiffe[feld].Trim();
            }

            string ergebnis = "";
            foreach(string stelle in Schiffe)
            {
                ergebnis = ergebnis + stelle;
            }
            if(ergebnis == "")
            {
                return true;
            }
            return false;
        }
      
        //Überprüfungen


        //Umwandlung
        static int BuchstabeInZahl(char eingabe)      //umwandeln von Buchstabe in Zahl für array
        {
            int zeile = 0; // A
            int j = -1;
            for (int Buchstabe = 'A'; Buchstabe <= '\u007F'; Buchstabe++) // muss bis letztes Zeichen von UTF-16 gehen sonst wird falsches Zeichen nicht abgefangen in Koordinateneingeben
            {
                j++;
                if (Buchstabe == eingabe)
                {
                    zeile = j; // wenn Buchstabe = Eingabe von User, dann wird in Zeile geschrieben
                }
            }
            return zeile;
        }

        static char ZahlinBuchstabe(int eingabe)
        {
            eingabe = eingabe + 65;
            char b = (char)eingabe;
            return b;
        }
        //Umwandlung
   //Hintergrund

    }
}
