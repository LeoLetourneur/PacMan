using Microsoft.Xna.Framework.Graphics;
using System;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe du fantome Clyde
    ///  <see cref="Fantome.cs">Classe mère</see>
    /// </summary>
    class Clyde : Fantome
    {
        private const int TEMPS_AVANT_DODO_MIN = 10000;
        private const int TEMPS_AVANT_DODO_MAX = 15000;
        private const int TEMPS_DODO = 5000;

        private Sommet sommetAleatoireClyde;
        private DateTime tempsDodoDebut;
        private TimeSpan tempsDodo_;
        private TimeSpan tempsDodo
        {
            get { return tempsDodo_; }
            set { tempsDodoDebut = DateTime.Now; tempsDodo_ = value; }
        }

        public Clyde(GamePacMan game, PacMan pacman, Constantes.Fantomes nom) : base(game, pacman, nom)
        {
            reinitialiser();
        }

        public override void reinitialiser()
        {
            definirTempsDodo();
            sommetAleatoireClyde = new Sommet((int)Constantes.POSITION_INITIALE_FANTOME.Y,
                                              (int)Constantes.POSITION_INITIALE_FANTOME.X + (int)Nom);
            base.reinitialiser();
        }

        public void definirTempsDodo()
        {
            Random rand = new Random();
            tempsDodo = TimeSpan.FromMilliseconds(rand.Next(TEMPS_AVANT_DODO_MIN, TEMPS_AVANT_DODO_MAX));
        }

        public override Sommet trouverSommet(Sommet sommetCourant)
        {
            if (Etat == Constantes.EtatFantome.Mort)
                return game.Sommets[(int)Constantes.POSITION_INITIALE_FANTOME.Y, ((int)Constantes.POSITION_INITIALE_FANTOME.X + (int)Constantes.Fantomes.Clyde)];

            if (Etat != Constantes.EtatFantome.Defensif)
            {
                if (DateTime.Now - tempsDodoDebut > (tempsDodo + TimeSpan.FromMilliseconds(TEMPS_DODO)))
                {
                    definirTempsDodo();
                    fantome.Texture = spritesFantome[0];
                }

                if (DateTime.Now - tempsDodoDebut > tempsDodo)
                {
                    fantome.Texture = game.Content.Load<Texture2D>(@"Fantomes\vertDort");
                    return sommetCourant;
                }
            }

            if (sommetCourant.Coord.X == sommetAleatoireClyde.Coord.X && sommetCourant.Coord.Y == sommetAleatoireClyde.Coord.Y)
                sommetAleatoireClyde = base.caseAleatoire();
            return sommetAleatoireClyde;
        }
    }
}
