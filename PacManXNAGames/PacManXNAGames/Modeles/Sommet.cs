
namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe de Sommet pour l'utilisation de Dijkstra
    /// </summary>
    public class Sommet
    {
        public const int INFINI = 1000000;
        public int Potentiel;
        public bool Marque;
        public Coord Coord;
        public Sommet Pred;
        public bool isProchePouvoir;

        /// <summary>
        /// Constructeur vide
        /// </summary>
        public Sommet()
        {
            Potentiel = INFINI;
            Marque = false;
            Pred = null;
            isProchePouvoir = false;
        }
        /// <summary>
        /// Constructeur avec 2 integer pour créer le sommet
        /// </summary>
        /// <param name=x>int x</param>
        /// <param name=y>int y</param>
        public Sommet(int x, int y)
        {
            Potentiel = INFINI;
            Marque = false;
            Coord = new Coord(x,y);
            Pred = null;
            isProchePouvoir = false;
        }
        /// <summary>
        /// Constructeur avec 1 Sommet pour créer le sommet
        /// </summary>
        /// <param name=sommet>Sommet sommet</param>
        public Sommet(Sommet sommet)
        {
            this.Potentiel = sommet.Potentiel;
            this.Marque = sommet.Marque;
            this.Coord = sommet.Coord;
            this.Pred = sommet.Pred;
        }
    }
}
