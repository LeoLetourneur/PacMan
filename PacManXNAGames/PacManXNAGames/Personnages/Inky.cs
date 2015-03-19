namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe du fantome Inky
    ///  <see cref="Fantome.cs">Classe mère</see>
    /// </summary>
    class Inky : Fantome
    {

        public Inky(GamePacMan game, PacMan pacman, Constantes.Fantomes nom) : base(game, pacman, nom)
        {
            reinitialiser();
        }

        public override Sommet trouverSommet(Sommet sommetCourant)
        {
            if (Etat == Constantes.EtatFantome.Mort)
                return game.Sommets[(int)Constantes.POSITION_INITIALE_FANTOME.Y, ((int)Constantes.POSITION_INITIALE_FANTOME.X + (int)Constantes.Fantomes.Inky)];
            else if (Etat == Constantes.EtatFantome.Defensif || (game.Sommets[(int)pacman.Position.Y, (int)pacman.Position.X].isProchePouvoir))
                return prendreLaFuite();
            else
                return game.Sommets[(int)pacman.Position.Y, (int)pacman.Position.X];
        }
    }
}
