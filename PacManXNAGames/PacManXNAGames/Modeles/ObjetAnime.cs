using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe permettant de définir des objets mobiles.
    /// </summary>
    public class ObjetAnime
    {
        public float Vitesse { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Size { get; set; }
        /// <summary>
        ///  Position de l'objet en fonction des pixels.
        /// </summary>
        public Vector2 PositionPixel { get; set; }
        /// <summary>
        ///  Position de l'objet en fonction de la grille de case.
        /// </summary>
        public Vector2 PositionGrille 
        {
            get { return new Vector2((int)PositionPixel.X / Constantes.COTE_CASE, (int)PositionPixel.Y / Constantes.COTE_CASE); }
        }
        /// <summary>
        ///  Position de la case où est le pixel haut gauche de l'objet animé.
        /// </summary>
        public Vector2 PositionCaseActuelle
        {
            get { return new Vector2((int)PositionGrille.X * Constantes.COTE_CASE, (int)PositionGrille.Y * Constantes.COTE_CASE); }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public ObjetAnime(Texture2D texture, Vector2 positionPixel, Vector2 size, float vitesse)
        {
            Texture = texture;
            PositionPixel = positionPixel;
            Size = size;
            Vitesse = vitesse;
        }

    }
}