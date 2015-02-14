﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Net;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.WPF;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Windows.Media.Imaging;


namespace Cluedo
{
    /// <summary>
    /// Logique d'interaction pour MainWindowCluedo.xaml
    /// </summary>
    public partial class MainWindowCluedo : SurfaceWindow
    {
        public static MainWindowCluedo instance;
        public int nbJoueurs = 0;
        public static List<String> joueurs = new List<String>();
        public int de = 0;
        public Boolean joueurPret = false;

        public static string idCasePoseTemporaire;
        public static bool dansUneSalle = false;

        //sortir de hall, 7, en haut 4 souis
        //public string idCaseCourant = "2.0";  
        //public string idCaseCourant = "3.5";
       // public string idCaseCourant = "11.4";    
        public string idCaseCourant = "5.5";    //dans hall
        static List<String> cases = new List<String> { "0.4", "0.5", "1.3", "1.4", "1.5", "1.6", "2.0", "2.1", "2.2", "2.3", "2.4", "2.5", "2.6", "2.7", "2.8", "3.1", "3.2", "3.3", "3.4", "3.5", "3.6", "3.7", "3.8", "3.9", "4.1", "4.2", "4.3", "4.4", "4.5", "4.6", "4.7", "4.8", "5.1", "5.2", "5.5", "5.7", "6.0", "6.1", "6.2", "6.3", "6.6", "6.7", "7.1", "7.2", "7.5", "7.7", "7.8", "8.1", "8.2", "8.3", "8.4", "8.5", "8.6", "8.7", "9.1", "9.2", "9.3", "9.4", "9.5", "9.6", "9.7", "10.2", "10.3", "10.4", "10.5", "10.6", "10.7", "10.8", "11.2", "11.3", "11.4", "11.5", "11.6", "11.7", "11.8", "11.9", "12.0", "12.1", "12.2", "12.3", "12.4", "13.1", "13.2", "13.3", "13.4", "13.5", "14.3" };
        static String[] GARAGE = { "2.0" };
        static String[] SALLEDEJEUX = { "6.0" };
        static String[] CHAMBRE = { "12.0" };                      //???? 14.0
        static String[] SALLEDEBAIN = { "14.3" };
        static String[] BUREAU = { "13.5" };
        static String[] CUISINE = { "11.9" };
        static String[] SALLEAMANGER = { "7.8" };
        static String[] SALON = { "3.9" };
        static String[] ENTREE = { "0.4", "0.5" };
        static String[] HALL = { "5.5", "6.6", "7.5", "6.3" };

        static public ArrayList tagsGarage = new ArrayList();
        static public ArrayList tagsSalledejeu = new ArrayList();
        static public ArrayList tagsChambre = new ArrayList();
        static public ArrayList tagsSalledebain = new ArrayList();
        static public ArrayList tagsBureau = new ArrayList();
        static public ArrayList tagsCuisine = new ArrayList();
        static public ArrayList tagsSalleamanger = new ArrayList();
        static public ArrayList tagsSalon = new ArrayList();
        static public ArrayList tagsEntree = new ArrayList();
        static public ArrayList tagsHall = new ArrayList();

        public static Boolean tagListEnvoye = false;
        public static Boolean lancementSupposition = false;

        public MainWindowCluedo()
        {
            InitializeComponent();

            instance = this;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 360;
            da.Duration = new Duration(TimeSpan.FromSeconds(5));
            da.RepeatBehavior = RepeatBehavior.Forever;
            rotateTransform1.BeginAnimation(RotateTransform.AngleProperty, da);
            rotateTransform2.BeginAnimation(RotateTransform.AngleProperty, da);
            rotateTransform3.BeginAnimation(RotateTransform.AngleProperty, da);
            rotateTransform4.BeginAnimation(RotateTransform.AngleProperty, da);
            //jouerImage1.Source = new BitmapImage(new Uri("Resources/personHead/leblanc.jpg", UriKind.Relative));/////////////////////////////////////////////////
            
        }

        public static MainWindowCluedo getInstance()
        {
            if (instance == null)
                instance = new MainWindowCluedo();
            return instance;
        }

        private void verifyPlace(object sender, TagVisualizerEventArgs e)
        {
            String[] list;
            String salleName = getSalleFromCase(idCaseCourant);
            Console.WriteLine("idCaseCourant " + idCaseCourant + " sallename: " + salleName);
            if (salleName != null)
            {
                list = getDoors(salleName);
            }
            else {
               String[] list2 = {idCaseCourant};
               list = list2;
            }
            
            //get "dé" number
            //int de = int.Parse(this.points1.Content.ToString());

            string joueurCourant = this.jouerCourant1.Content.ToString();

            //récupérer le tag
            string tag = "0x" + Convert.ToString(e.TagVisualization.VisualizedTag.Value, 16);
            Console.WriteLine("tag: " + tag);
            string joueurActuel = getJoueurFromTag(tag);

            string idCase = ((TagVisualizer)e.OriginalSource).DataContext.ToString();
            Ellipse zoneCase = getZoneCaseFromName(idCase);

            if (joueurCourant.Equals(joueurActuel))
            {
                Console.WriteLine("longeur du list: " + list.Count());
                List<String> tab = seDeplacer(list, de);

                Boolean positionCorrect = false;
                foreach (string caseMvt in tab)
                {
                    if (idCase.Equals(caseMvt))
                    {
                        zoneCase.Opacity = 0.5;
                        zoneCase.Fill = Brushes.Green;

                        positionCorrect = true;

                        //SocketIO.tourTermine(idCase);
                        SocketIO.ValidationCase(idCase);
                        idCasePoseTemporaire = idCase;
                        dansUneSalle = false;
                        Console.WriteLine("idCasePoseTemporaire: " + idCasePoseTemporaire);
                    }
                }
                if (!positionCorrect)
                {
                    zoneCase.Opacity = 0.5;
                    zoneCase.Fill = Brushes.Red;

                    SocketIO.ValidationCase("NON");
                    idCasePoseTemporaire = idCase;
                    dansUneSalle = false;

                    Console.WriteLine("idCasePoseTemporaire: " + idCasePoseTemporaire);
                }
            }
            else {
                zoneCase.Opacity = 0.5;
                zoneCase.Fill = Brushes.Orange;
            }

        }

        private void verifySalle(string idCase, string tag)
        {
            
            String[] list;
            String salleName = getSalleFromCase(idCaseCourant);

            if (salleName != null)
            {
                list = getDoors(salleName);
            }
            else
            {
                String[] list2 = { idCaseCourant };
                list = list2;
            }

            //get "dé" number
            //int de = int.Parse(this.points1.Content.ToString());

            
            Console.WriteLine("tag: " + tag);
            string joueurActuel = getJoueurFromTag(tag);
            string joueurCourant = this.jouerCourant1.Content.ToString();

            Rectangle zoneCase = getRoomCaseFromName(idCase);

            if (joueurCourant.Equals(joueurActuel))
            {
                //get possible cases to go
                List<String> tab = seDeplacer(list, de);
                //check if the player is in a room now
                if (salleName != null) {
                    //if the player is in the chambre, he can go to sallon, if the player is in the cuisine, he can go to the garage, vice-versa
                    if (salleName.Equals("CUISINE"))
                    {
                        //cuisine->garage
                        tab.Add("2.0");

                        //cusine->salleamanger
                        tab.Add("7.8");
                    }
                    if (salleName.Equals("GARAGE"))
                    {
                        //garage->cuisine
                        tab.Add("11.9");
                    }
                    if (salleName.Equals("SALON"))
                    {
                        //salon->chambre
                        tab.Add("12.0");
                    }
                    if (salleName.Equals("CHAMBRE"))
                    {
                        //chambre->salon
                        tab.Add("3.9");

                        //chambre->salle de bain
                        tab.Add("14.3");
                    }
                    if (salleName.Equals("SALLEAMANGER")) { 
                        //salleamanger->cusine
                        tab.Add("11.9");
                    }
                    if (salleName.Equals("SALLEDEBAIN")) {
                        //salledebain->chambre
                        tab.Add("12.0");
                    }

                }


                Boolean positionCorrect = false;

                //if any door of the room is in the doorList, the player can go into that room
                string[] doorList = getDoors(idCase);

                foreach (string caseMvt in tab)
                {
                    for (int i = 0; i < doorList.Length; i++)
                    {
                        if (doorList[i].Equals(caseMvt))
                        {
                            zoneCase.Opacity = 0.5;
                            zoneCase.Fill = Brushes.Green;

                            positionCorrect = true;
                            //4. if position correct send current position 

                            //envoyer le id au lieu de nom de la salle
                            
                            //SocketIO.tourChoixSupposition(doorList[0]);

                            SocketIO.ValidationCase(doorList[0]);

                            idCasePoseTemporaire = doorList[0];
                            dansUneSalle = true;

                            Console.WriteLine("idCasePoseTemporaire: " + idCasePoseTemporaire);
                        }
                    }
                }
                if (!positionCorrect)
                {
                    zoneCase.Opacity = 0.5;
                    zoneCase.Fill = Brushes.Red;

                    SocketIO.ValidationCase("NON");

                    idCasePoseTemporaire = doorList[0];
                    dansUneSalle = true;

                    Console.WriteLine("idCasePoseTemporaire: " + idCasePoseTemporaire);
                }
            }
            else {
                zoneCase.Opacity = 0.5;
                zoneCase.Fill = Brushes.Orange;
            }
            
        }

        private string getJoueurFromTag(String tag) {
            switch (tag) { 
                case "0x3":
                    return  "Violet";
                case "0x6":
                    return "Leblanc";
                case "0x10":
                    return "Rose";
                case "0x11":
                    return "Olive";
                case "0x12":
                    return "Moutarde";
                case "0x13":
                    return "Pervenche";
                default :
                    return "Arme";

            }
        }

        private string getSalleFromCase(string idcase) {
            switch (idcase) { 
                case "0.4":
                    return "ENTREE";
                case "0.5":
                    return "ENTREE";
                case "2.0":
                    return "GARAGE";
                case "3.9":
                    return "SALON";
                case "6.0":
                    return "SALLEDEJEUX";
                case "7.8":
                    return "SALLEAMANGER";
                case "11.9":
                    return "CUISINE";
                case "12.0":
                    return "CHAMBRE";
                case "13.5":
                    return "BUREAU";
                case "14.3":
                    return "SALLEDEBAIN";
                case "5.5": 
                    return "HALL";
                case "6.3":
                    return "HALL";
                case "6.6":
                    return "HALL";
                case "7.5":
                    return "HALL";
                default:
                    return null;

            }
        }

        private void verifyJoueurPret(object sender, TagVisualizerEventArgs e)
        {

            string idCase = ((TagVisualizer)e.OriginalSource).DataContext.ToString();
            string tag = "0x" + Convert.ToString(e.TagVisualization.VisualizedTag.Value, 16);
            Rectangle zoneCase = getRoomCaseFromName(idCase);

            string joueurActuel = getJoueurFromTag(tag);
            string joueurCourant = this.jouerCourant1.Content.ToString();
            //c'est la deuxième fois qu'il entre dans hall pour faire accusation
            if (joueurPret)
            {
                //au cas ou le joueur pose une arme au lieu de son personage

                    if (joueurActuel.Equals(joueurCourant))
                    {
                        zoneCase.Opacity = 0.5;
                        zoneCase.Fill = Brushes.Green;
                        SocketIO.tourChoixAccusation(idCase);
                    }
                    else
                    {

                        ArrayList al = new ArrayList();
                        al.Add(getIdFromTag(tag));
                        SocketIO.tagsDansPiece(al);

                        /*zoneCase.Opacity = 0.5;
                        zoneCase.Fill = Brushes.Orange;*/
                    }

            }
            else {

                /*nbJoueurs++;
                if (joueurs.Count == nbJoueurs)
                {
                    Console.WriteLine("joueurs sont prets!");
                    SocketIO.lancementPionsPrets();

                    joueurPret = true;
                }*/

               
                for (int i = 0; i < joueurs.Count; i++) {
                    if (joueurs[i].Equals(joueurActuel)) {
                        nbJoueurs++;
                        if (nbJoueurs == joueurs.Count)
                        {
                            Console.WriteLine("joueurs sont prets!");
                            SocketIO.lancementPionsPrets();

                            joueurPret = true;
                        }
                    }
                }
            }
        }

        private void entrerDansPiece(object sender, TagVisualizerEventArgs e)
        {
            string idCase = ((TagVisualizer)e.OriginalSource).DataContext.ToString();
            Console.WriteLine("entrer dans " + idCase);

            string tag = "0x" + Convert.ToString(e.TagVisualization.VisualizedTag.Value, 16);

            getTagsDansPiece(idCase).Add(getIdFromTag(tag));

            
            if (!lancementSupposition)
            {
                verifySalle(idCase, tag);
            }
            //entrain de faire supposition, ne verifie plus si joueur est entré dans une salle correcte
            else {
                //emit de son
                if (tagPersonne(tag))
                {
                    Sounds.Play(EnumSound.POSERPERSONNAGE);
                }
                else {
                    Sounds.Play(EnumSound.POSERARME);
                }

                //si la liste est déjà envoyé, après, il faut juste envoyer ce qui est ajouté de nouveau
                Console.WriteLine("lancement supposition " + lancementSupposition);
                if (tagListEnvoye)
                {
                    ArrayList al = new ArrayList();
                    al.Add(getIdFromTag(tag));
                    SocketIO.tagsDansPiece(al);
                    Console.WriteLine("envoyer le nouveau tag");
                }
                else {
                    SocketIO.tagsDansPiece(getTagsDansPiece(idCase));
                    tagListEnvoye = true;
                    Console.WriteLine("envoyer la liste des tags");
                }
            }
        }

        private string getIdFromTag(string tag) {
            switch (tag)
            {
                case "0x3":
                    return "0";
                case "0x6":
                    return "1";
                case "0x10":
                    //Console.WriteLine("return 2");
                    return "2";
                case "0x11":
                    return "3";
                case "0x12":
                    return "4";
                case "0x13":
                    return "5";
                case "0x14":
                    return "6";
                case "0x15":
                    return "7";
                case "0x16":
                    return "8";
                case "0x21":
                    return "9";
                case "0x22":
                    return "10";
                case "0x25":
                    return "11";
                default:
                    return null;

            }
        }

        private bool tagPersonne(string tag)
        {
            switch (tag)
            {
                case "0x3":
                    return true;
                case "0x6":
                    return true;
                case "0x10":
                    //Console.WriteLine("return 2");
                    return true;
                case "0x11":
                    return true;
                case "0x12":
                    return true;
                case "0x13":
                    return true;
                case "0x14":
                    return false;
                case "0x15":
                    return false;
                case "0x16":
                    return false;
                case "0x21":
                    return false;
                case "0x22":
                    return false;
                case "0x25":
                    return false;
                default:
                    return true;

            }
        }


        private ArrayList getTagsDansPiece(string idcase){
             switch (idcase)
            {
                case "GARAGE":
                    return tagsGarage;
                case "SALLEDEJEUX":
                    return tagsSalledejeu;
                case "CHAMBRE":
                    return tagsChambre;
                case "SALLEDEBAIN":
                    return tagsSalledebain;
                case "BUREAU":
                    return tagsBureau;
                case "CUISINE":
                    return tagsCuisine;
                case "SALLEAMANGER":
                    return tagsSalleamanger;
                case "SALON":
                    return tagsSalon;
                case "ENTREE":
                    return tagsEntree;
                case "HALL":
                    return tagsHall;
                default:
                    return null;
            }   
        }

        private String[] getDoors(String room) {
            switch (room)
            {
                case "GARAGE":
                    return GARAGE;
                case "SALLEDEJEUX":
                    return SALLEDEJEUX;
                case "CHAMBRE":
                    return CHAMBRE;
                case "SALLEDEBAIN":
                    return SALLEDEBAIN;
                case "BUREAU":
                    return BUREAU;
                case "CUISINE":
                    return CUISINE;
                case "SALLEAMANGER":
                    return SALLEAMANGER;
                case "SALON":
                    return SALON;
                case "ENTREE":
                    return ENTREE;
                case "HALL":
                    return HALL;
                default:
                    return null;
            }

        }


        private void cleanColor(object sender, TagVisualizerEventArgs e){
            string idCase = ((TagVisualizer)e.OriginalSource).DataContext.ToString();
            Ellipse zoneCase = getZoneCaseFromName(idCase);

            zoneCase.Fill = Brushes.Transparent;

        }

        private void sortirDePiece(object sender, TagVisualizerEventArgs e)
        {
            string idCase = ((TagVisualizer)e.OriginalSource).DataContext.ToString();
            Rectangle zoneCase = getRoomCaseFromName(idCase);
            zoneCase.Fill = Brushes.Transparent;

            string tag = "0x" + Convert.ToString(e.TagVisualization.VisualizedTag.Value, 16);

            
            for (int i = 0; i < getTagsDansPiece(idCase).Count; i++)
            {
                string value = getTagsDansPiece(idCase)[i] as string;
                if (value == getIdFromTag(tag))
                {
                    getTagsDansPiece(idCase).RemoveAt(i);
                     Console.WriteLine("attention! tag supprimé");
                }
                else {
                    Console.WriteLine("attention! " + tag + " n'est pas trouvé");
                }
            }

        }

        private void sortirDeHall(object sender, TagVisualizerEventArgs e)
        {
            if (joueurPret) {
                string idCase = ((TagVisualizer)e.OriginalSource).DataContext.ToString();
                Rectangle zoneCase = getRoomCaseFromName(idCase);
                zoneCase.Fill = Brushes.Transparent;
            }
        }

        public Rectangle getRoomCaseFromName(string idCase) {
            switch (idCase)
            {
                case "GARAGE":
                    return this.NGARAGE;
                case "SALLEDEJEUX":
                    return this.NSALLEDEJEUX;
                case "CHAMBRE":
                    return this.NCHAMBRE;
                case "SALLEDEBAIN":
                    return this.NSALLEDEBAIN;
                case "BUREAU":
                    return this.NBUREAU;
                case "CUISINE":
                    return this.NCUISINE;
                case "SALLEAMANGER":
                    return this.NSALLEAMANGER;
                case "SALON":
                    return this.NSALON;
                case "ENTREE":
                    return this.NENTREE;
                case "HALL":
                    return this.NHALL;
            }
            return null;
        }

        public Ellipse getZoneCaseFromName(string idCase)
        {
            switch (idCase)
            {
                case "1.3":
                    return this.N13;
                case "1.4":
                    return this.N14;
                case "1.5":
                    return this.N15;
                case "1.6":
                    return this.N16;
                case "2.1":
                    return this.N21;
                case "2.2":
                    return this.N22;
                case "2.3":
                    return this.N23;
                case "2.4":
                    return this.N24;
                case "2.5":
                    return this.N25;
                case "2.6":
                    return this.N26;
                case "2.7":
                    return this.N27;
                case "2.8":
                    return this.N28;
                case "3.1":
                    return this.N31;
                case "3.2":
                    return this.N32;
                case "3.3":
                    return this.N33;
                case "3.4":
                    return this.N34;
                case "3.5":
                    return this.N35;
                case "3.6":
                    return this.N36;
                case "3.7":
                    return this.N37;
                case "3.8":
                    return this.N38;
                case "4.1":
                    return this.N41;
                case "4.2":
                    return this.N42;
                case "4.3":
                    return this.N43;
                case "4.4":
                    return this.N44;
                case "4.5":
                    return this.N45;
                case "4.6":
                    return this.N46;
                case "4.7":
                    return this.N47;
                case "4.8":
                    return this.N48;
                case "5.1":
                    return this.N51;
                case "5.2":
                    return this.N52;
                case "5.7":
                    return this.N57;
                case "6.1":
                    return this.N61;
                case "6.2":
                    return this.N62;
                case "6.7":
                    return this.N67;
                case "7.1":
                    return this.N71;
                case "7.2":
                    return this.N72;
                case "7.7":
                    return this.N77;
                case "8.1":
                    return this.N81;
                case "8.2":
                    return this.N82;
                case "8.3":
                    return this.N83;
                case "8.4":
                    return this.N84;
                case "8.5":
                    return this.N85;
                case "8.6":
                    return this.N86;
                case "8.7":
                    return this.N87;
                case "9.1":
                    return this.N91;
                case "9.2":
                    return this.N92;
                case "9.3":
                    return this.N93;
                case "9.4":
                    return this.N94;
                case "9.5":
                    return this.N95;
                case "9.6":
                    return this.N96;
                case "9.7":
                    return this.N97;
                case "10.2":
                    return this.N102;
                case "10.3":
                    return this.N103;
                case "10.4":
                    return this.N104;
                case "10.5":
                    return this.N105;
                case "10.6":
                    return this.N106;
                case "10.7":
                    return this.N107;
                case "10.8":
                    return this.N108;
                case "11.2":
                    return this.N112;
                case "11.3":
                    return this.N113;
                case "11.4":
                    return this.N114;
                case "11.5":
                    return this.N115;
                case "11.6":
                    return this.N116;
                case "11.7":
                    return this.N117;
                case "11.8":
                    return this.N118;
                case "12.1":
                    return this.N121;
                case "12.2":
                    return this.N122;
                case "12.3":
                    return this.N123;
                case "12.4":
                    return this.N124;
                case "13.1":
                    return this.N131;
                case "13.2":
                    return this.N132;
                case "13.3":
                    return this.N133;
                case "13.4":
                    return this.N134;
                

            }

            return null;
        }

        public static List<String> seDeplacer(string[] positions, int de)
        {
            List<String> tab = new List<string>();
            foreach (string pos in positions)
            {
                tab.AddRange(choixDeplacement(pos, de));
            }
            // on enlève les entrées de la pièce d'origine
            foreach (string pos in positions)
            {
                tab.Remove(pos);
            }
            // on enlève les doublons
            tab = tab.Distinct().ToList<String>();
            return tab;
        }

        static Boolean isRoom(string pos)
        {
            if (pos == "0.4" || pos == "0.5" || pos == "2.0" || pos == "3.9" || pos == "6.0" || pos == "6.3" || pos == "5.5" || pos == "7.5" || pos == "6.6" || pos == "7.8" || pos == "11.9" || pos == "12.0" || pos == "13.5" || pos == "14.3")
            {
                //Console.WriteLine("salle "+pos);
                return true;
            }
            return false;
        }

        static List<String> choixDeplacement(string pos, int de)
        {
            List<String> tab = new List<String>();
            // sinon si l'on peut encore se déplacer
            if (de > 0)
            {
                string[] position = pos.Split('.');
                int x = Int32.Parse(position[0]);
                int y = Int32.Parse(position[1]);
                string temp = "";
                // tout droit
                temp = (x + 1) + "." + y;
                // si pièce, retourner la case
                if (isRoom(temp))
                {
                    tab.Add(temp);
                }
                else if (cases.Contains(temp))
                {
                    tab.AddRange(choixDeplacement(temp, de - 1));
                }
                // à droite
                temp = x + "." + (y + 1);
                if (isRoom(temp))
                {
                    tab.Add(temp);
                }
                else if (cases.Contains(temp))
                {
                    tab.AddRange(choixDeplacement(temp, de - 1));
                }
                // à gauche
                temp = x + "." + (y - 1);
                if (isRoom(temp))
                {
                    tab.Add(temp); ;
                }
                else if (cases.Contains(temp))
                {
                    tab.AddRange(choixDeplacement(temp, de - 1));
                }
                // en arrière
                temp = (x - 1) + "." + y;
                // si pièce, retourner la case
                if (isRoom(temp))
                {
                    tab.Add(temp);
                }
                else if (cases.Contains(temp))
                {
                    tab.AddRange(choixDeplacement(temp, de - 1));
                }
            }
            // sinon,  retourne la case actuelle
            else
            {
                tab.Add(pos);
            }

            return tab;
        }

        public void choixMouvement(string nomJoueur, string numCase, int de) {
            
            this.jouerCourant1.Content = nomJoueur;
            this.jouerCourant2.Content = nomJoueur;
            jouerImage1.Source = getJoueurImage(nomJoueur);
            jouerImage2.Source = getJoueurImage(nomJoueur);
            //this.points1.Content = de;
            // affichier l'image de de !!!
            //this.points2.Content = de;
            idCaseCourant = numCase;
            this.de = de;

            Uri ude = new Uri("Resources/dices/"+de+".png", UriKind.Relative);
            points1.Source = new BitmapImage(ude);
            points2.Source = new BitmapImage(ude);
        }

        public void suppReponse(string reponse) {
            this.suppositionResponse1.Content = reponse;
            this.suppositionResponse2.Content = reponse;
        }

        private BitmapImage getJoueurImage(string nomJoueur) {
            switch (nomJoueur) { 
                case "Violet":
                    return new BitmapImage(new Uri("Resources/personHead/violet.jpg", UriKind.Relative));
                case "Leblanc":
                    return new BitmapImage(new Uri("Resources/personHead/leblanc.jpg", UriKind.Relative));
                case "Moutarde":
                    return new BitmapImage(new Uri("Resources/personHead/moutarde.jpg", UriKind.Relative));
                case "Olive":
                    return new BitmapImage(new Uri("Resources/personHead/Olive.jpg", UriKind.Relative));
                case "Rose":
                    return new BitmapImage(new Uri("Resources/personHead/Rose.jpg", UriKind.Relative));
                case "Pervenche":
                    return new BitmapImage(new Uri("Resources/personHead/Pervenche.jpg", UriKind.Relative));
            }
            return null;
        }

        public void finPartie(bool gagne, string pseudo, string perso, string arme, string lieu)
        {
            Storyboard st1 = globalGrid.Resources["closeQueryCanvas1"] as Storyboard;
            st1.Begin();
            Storyboard st11 = globalGrid.Resources["showQueryCanvas1"] as Storyboard;
            st11.Begin();
            Storyboard st2 = globalGrid.Resources["closeQueryCanvas2"] as Storyboard;
            st2.Begin();
            Storyboard st22 = globalGrid.Resources["showQueryCanvas2"] as Storyboard;
            st22.Begin();

            Uri personneuri = new Uri("Resources/personCard/" + perso.Replace(" ", String.Empty) + ".png", UriKind.Relative);
            Uri armeuri = new Uri("Resources/armCard/" + arme.Replace(" ", String.Empty) + ".png", UriKind.Relative);
            Uri lieuuri = new Uri("Resources/pieceCard/" + lieu.Replace(" ", String.Empty) + ".png", UriKind.Relative);

            Console.WriteLine("fin partie: " + perso + " " + arme + " " + lieu);

            this.rightPerson1.Source = new BitmapImage(personneuri);
            this.rightPerson2.Source = new BitmapImage(personneuri);

            this.rightArm1.Source = new BitmapImage(armeuri);
            this.rightArm2.Source = new BitmapImage(armeuri);

            this.rightPiece1.Source = new BitmapImage(lieuuri);
            this.rightPiece2.Source = new BitmapImage(lieuuri);

            
            this.winnerName1.Content = pseudo;
            this.winnerName2.Content = pseudo;

            if (gagne)
            {
                guessResult1.Text = " a gagné avec les supposition au desous:";
                guessResult2.Text = " a gagné avec les supposition au desous:";
            }
            else {
                guessResult1.Text = " a perdu. Le vrai résultat est:";
                guessResult2.Text = " a perdu. Le vrai résultat est:";
            }
            
        }

        public void goToStartPage()
        {
            
            Page1 startPage = new Page1();
            startPage.Show();
            this.Close();
        }
    }
}
