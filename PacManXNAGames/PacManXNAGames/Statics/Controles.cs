using Microsoft.Xna.Framework.Input;
using System;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe static permettant de lire les touches claviers.
    /// </summary>
    public static class  Controles
    {
        public const Keys HAUT = Keys.Up;
        public const Keys BAS = Keys.Down;
        public const Keys DROITE = Keys.Right;
        public const Keys GAUCHE = Keys.Left;
        public const Keys ESPACE = Keys.Space;
        public const Keys ENTRER = Keys.Enter;

        /// <summary>
        /// Savoir si une touche a été pressée
        /// </summary>
        /// <returns>Valeur int de l'enumération Direction en fonction de la touche pressée</returns>
        public static int VerifierTouchePressee()
        {
            KeyboardState clavier = Keyboard.GetState();
            if (clavier.IsKeyDown(HAUT))
                return (int)Constantes.Direction.Haut;
            else if (clavier.IsKeyDown(BAS))
                return (int)Constantes.Direction.Bas;
            else if (clavier.IsKeyDown(GAUCHE))
                return (int)Constantes.Direction.Gauche;
            else if (clavier.IsKeyDown(DROITE))
                return (int)Constantes.Direction.Droite;
            else
                return (int)Constantes.Direction.Immobile;
        }
        /// <summary>
        /// Savoir si la touche "Haut" a été pressée
        /// </summary>
        public static Boolean VerifierToucheHaut()
        {
            KeyboardState clavier = Keyboard.GetState();
            return clavier.IsKeyDown(HAUT);
        }
        /// <summary>
        /// Savoir si la touche "Bas" a été pressée
        /// </summary>
        public static Boolean VerifierToucheBas()
        {
            KeyboardState clavier = Keyboard.GetState();
            return clavier.IsKeyDown(BAS);
        }
        /// <summary>
        /// Savoir si la touche "Espace" a été pressée
        /// </summary>
        public static Boolean VerifierToucheEspace()
        {
            KeyboardState clavier = Keyboard.GetState();
            return clavier.IsKeyDown(ESPACE);
        }
        /// <summary>
        /// Savoir si la touche "Entrer" a été pressée
        /// </summary>
        public static Boolean VerifierToucheEntrer()
        {
            KeyboardState clavier = Keyboard.GetState();
            return clavier.IsKeyDown(ENTRER);
        }
    }
}
