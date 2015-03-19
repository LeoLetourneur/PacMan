using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe static permettant de définir des constantes et d'y avoir accès dans tout le projet.
    /// </summary>
    public static class Constantes
    {
        //Map
        public const int COTE_CASE = 20;
        public const int LARGEUR_NIVEAU = 28;
        public const int HAUTEUR_NIVEAU = 31;
        public const int NB_CASE_POPBONUS = 6;

        //Jeux
        public const int NB_BONUS = 3;
        public const int NB_NIVEAUX = 5;
        public const int NB_FANTOMES = 4;
        public const int NB_VIES = 3;
        public static Vector2 POSITION_INITIALE_PAC = new Vector2(14, 17);
        public static Vector2 POSITION_INITIALE_FANTOME = new Vector2(12, 14);

        //Temps
        public const int PAUSE_FANTOME = 2000;
        public const int PAUSE_JEU_DEBUT = 4200;
        public const int PAUSE_JEU_INTER = 2000;
        public const int TEMPS_MINIMUM_BONUS = 6000;
        public const int TEMPS_MAXIMUM_BONUS = 8000;
        public static readonly List<int> TEMPS_PEUR = new List<int> { 7000, 6500, 6000, 5500, 5000 };

        //Vitesse PacMan
        public const int NB_SPRITE_PACMORT = 12;
        public const int VITESSE_SPRITE_PACMORT = 50;
        public const int VITESSE_PACMAN_DEBUT = 15;
        public const int INCREMENT_VITESSE = 3; //Si l'on vaut voir une différence rapide de vitesse -> 10

        //Score
        public const int POINT_GOMME = 10;
        public const int POINT_SUPER = 50;
        public const int POINT_BONUS = 500;
        public const int POINT_FANTOME = 200;
        
        //Enumerations
        public enum TypeCase { Mur, Piece, MaisonFantome, Pouvoir, Bonus, Rien, BonusActif, PorteFantome };
        public enum Direction { Immobile, Haut, Bas, Gauche, Droite };
        public enum EtatJeu { Menu, Debut, EnJeu, Pause, FinNiveau, FinPartie } ;
        public enum EtatPacman { Normal, Invincible, Mort };
        public enum EtatFantome { Maison, Offensif, Defensif, Mort }
        public enum Fantomes { Blinky, Inky, Clyde, Pinky}
        public enum Couleurs { Rouge, Bleu, Vert, Rose}

    }
}
