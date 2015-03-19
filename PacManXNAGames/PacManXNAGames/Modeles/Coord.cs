using Microsoft.Xna.Framework;
using System;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe de Coordonnées
    /// </summary>
    public class Coord
    {
        public int X, Y;

        /// <summary>
        /// Constructeur avec 2 integer pour créer la coordonnée
        /// </summary>
        /// <param name=x>int x</param>
        /// <param name=y>int y</param>
        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Constructeur avec un Vector2 pour créer la coordonnée
        /// </summary>
        /// <param name=pos>Vector2 pos</param>
        public Coord(Vector2 pos)
        {
            X = (int)pos.X / Constantes.COTE_CASE;
            Y = (int)pos.Y / Constantes.COTE_CASE;
        }
        /// <summary>
        /// Redifinission de l'opérateur ==
        /// </summary>
        public static Boolean operator ==(Coord c1, Coord c2)
        {
            return ((c1.X == c2.X) && (c1.Y == c2.Y));
        }
        /// <summary>
        /// Redifinission de l'opérateur !=
        /// </summary>
        public static Boolean operator !=(Coord c1, Coord c2)
        {
            return ((c1.X != c2.X) || (c1.Y != c2.Y));
        }
    }
}
