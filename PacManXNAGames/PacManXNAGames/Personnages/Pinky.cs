using System;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe du fantome Pinky
    ///  <see cref="Fantome.cs">Classe mère</see>
    /// </summary>
    class Pinky : Fantome
    {
        private Boolean innocent;
        private Sommet sommetAleatoirePinky;

        public Pinky(GamePacMan game, PacMan pacman, Constantes.Fantomes nom) : base(game, pacman, nom)
        {
            reinitialiser();
        }

        public override void reinitialiser()
        {
            innocent = true;
            sommetAleatoirePinky = new Sommet((int)Constantes.POSITION_INITIALE_FANTOME.Y,
                                              (int)Constantes.POSITION_INITIALE_FANTOME.X + (int)Constantes.Fantomes.Pinky);
            base.reinitialiser();
        }

        public override Sommet trouverSommet(Sommet sommetCourant)
        {
            if (Etat == Constantes.EtatFantome.Mort)
                return game.Sommets[(int)Constantes.POSITION_INITIALE_FANTOME.Y, ((int)Constantes.POSITION_INITIALE_FANTOME.X + (int)Nom)];
            else if (Etat == Constantes.EtatFantome.Defensif)
                return prendreLaFuite();
            else if (innocent)
            {
                if (sommetCourant.Coord.X == sommetAleatoirePinky.Coord.X && sommetCourant.Coord.Y == sommetAleatoirePinky.Coord.Y)
                    sommetAleatoirePinky = caseAleatoire();
                return sommetAleatoirePinky;
            }
            else 
                return game.Sommets[(int)pacman.Position.Y, (int)pacman.Position.X];
        }

        public override void changerDirection()
        {
            innocent = true;
            if (pacman.VuParFantome == true)
                innocent = false;
            base.changerDirection();
        }
    }
}
