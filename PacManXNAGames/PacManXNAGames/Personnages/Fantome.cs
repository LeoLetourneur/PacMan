using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe Fantome qui correspont aussi au fantôme rouge Blinky
    /// </summary>
    public class Fantome
    {
        private SpriteBatch spriteBatch;
        protected ObjetAnime fantome;
        protected GamePacMan game;
        protected PacMan pacman;

        protected Constantes.Fantomes Nom;
        private Constantes.Direction Direction;
        protected Constantes.EtatFantome Etat;
        private Constantes.Couleurs Couleur;
        
        private BoundingBox boxPixelFantome;
        protected Texture2D[] spritesFantome;

        public TimeSpan TempsStopSauvegarde { get; set; }
        private DateTime tempsStopDebut;
        private TimeSpan tempsStop_;
        public TimeSpan tempsStop
        {
            get { return tempsStop_; }
            set { tempsStopDebut = DateTime.Now; tempsStop_ = value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name=game>Partie en cours</param>
        /// <param name=pacman>PacMan en jeu</param>
        /// <param name=nom>Nom du fantôme</param>
        public Fantome(GamePacMan game, PacMan pacman, Constantes.Fantomes nom)
        {
            this.game = game;
            this.pacman = pacman;
            this.Nom = nom;
            
            Couleur = (Constantes.Couleurs)(int)nom;
            tempsStop = TimeSpan.FromMilliseconds((int)nom * Constantes.PAUSE_FANTOME + Constantes.PAUSE_JEU_DEBUT);

            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            spritesFantome = new Texture2D[6];
            spritesFantome[0] = game.Content.Load<Texture2D>(@"Fantomes\" + Couleur.ToString()+"_haut");
            spritesFantome[1] = game.Content.Load<Texture2D>(@"Fantomes\" + Couleur.ToString() + "_bas");
            spritesFantome[2] = game.Content.Load<Texture2D>(@"Fantomes\" + Couleur.ToString() + "_gauche");
            spritesFantome[3] = game.Content.Load<Texture2D>(@"Fantomes\" + Couleur.ToString() + "_droite");
            spritesFantome[4] = game.Content.Load<Texture2D>(@"Fantomes\Peur0");
            spritesFantome[5] = game.Content.Load<Texture2D>(@"Fantomes\Mort");

            float vitesse = 1f;
            fantome = new ObjetAnime(null, new Vector2(), new Vector2(Constantes.COTE_CASE, Constantes.COTE_CASE), vitesse);
            reinitialiser();
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(fantome.Texture, fantome.PositionPixel, Color.Azure);
        }

        public void Update(GameTime gameTime)
        {
            if (DateTime.Now - tempsStopDebut < tempsStop)
                return;

            if (Etat == Constantes.EtatFantome.Maison)
                Etat = Constantes.EtatFantome.Offensif;

            int precision = 4;
            boxPixelFantome = new BoundingBox(new Vector3((int)fantome.PositionPixel.X+precision, (int)fantome.PositionPixel.Y+precision, 0),
                new Vector3((int)fantome.PositionPixel.X + Constantes.COTE_CASE-2*precision, (int)fantome.PositionPixel.Y + Constantes.COTE_CASE-2*precision, 0));

            if (fantome.PositionPixel.X % Constantes.COTE_CASE == 0 && fantome.PositionPixel.Y % Constantes.COTE_CASE == 0)
                changerDirection();
            deplacer();

            if (Etat == Constantes.EtatFantome.Mort)
            {
                if (fantome.PositionGrille.X == Constantes.POSITION_INITIALE_FANTOME.X + (int)Nom
                && fantome.PositionGrille.Y == Constantes.POSITION_INITIALE_FANTOME.Y)
                {
                    Etat = Constantes.EtatFantome.Maison;
                    fantome.Texture = spritesFantome[0];
                    tempsStop = TimeSpan.FromMilliseconds(Constantes.PAUSE_FANTOME);
                }
            }
            else if (pacman.Etat == Constantes.EtatPacman.Invincible)
            {
                Etat = Constantes.EtatFantome.Defensif;
                fantome.Texture = spritesFantome[4];
            }
            else if (Etat == Constantes.EtatFantome.Defensif)
            {
                Etat = Constantes.EtatFantome.Offensif;
                fantome.Texture = spritesFantome[0];
            }

            verifierCollisionPM();
        }

        /// <summary>
        /// Réinitialiser le fantôme à sa position initiale.
        /// </summary>
        public virtual void reinitialiser()
        {
            fantome.PositionPixel = new Vector2((Constantes.POSITION_INITIALE_FANTOME.X + (int)Nom) * Constantes.COTE_CASE,
                                                 Constantes.POSITION_INITIALE_FANTOME.Y * Constantes.COTE_CASE);
            fantome.Texture = spritesFantome[0];
            Direction = Constantes.Direction.Immobile;
            Etat = Constantes.EtatFantome.Maison;
        }

        /// <summary>
        /// Savoir si la Pacman rentre en collision avec un fantôme.
        /// Si le pacman est invicible, il mange le fantôme, sinon pacman meurt.
        /// </summary>
        private void verifierCollisionPM()
        {
            if (pacman.boxPacMan.Intersects(boxPixelFantome) && pacman.Etat != Constantes.EtatPacman.Mort)
            {
                if (Etat == Constantes.EtatFantome.Defensif)
                {
                    Etat = Constantes.EtatFantome.Mort;
                    fantome.Texture = spritesFantome[5];
                    pacman.NbFantomeMange++;
                    game.ScoreVal += Constantes.POINT_FANTOME * pacman.NbFantomeMange;
                    SonsManager.mangerFantome(game);
                }
                else if (Etat == Constantes.EtatFantome.Offensif)
                {
                    pacman.Etat = Constantes.EtatPacman.Mort;
                    game.TempsStop = TimeSpan.FromMilliseconds(Constantes.PAUSE_JEU_INTER);
                    game.pauseFantome(true);
                    game.Vies--;
                    if (game.Vies != 0)
                    {
                        game.reinitialiserFantome();
                        game.Etat = Constantes.EtatJeu.Debut;
                    }
                    SonsManager.pacmanMort(game);
                }
            }
        }

        /// <summary>
        /// Trouver une case aléatoire dans le labyrinthe qui n'est pas un mur, la maison des fantômes ou la porte.
        /// </summary>
        /// <returns>Le sommet aléatoire</returns>
        public Sommet caseAleatoire()
        {
            Random rand = new Random();
            int x,y;

            do
            {
                x = rand.Next(1, Constantes.HAUTEUR_NIVEAU - 1);
                y = rand.Next(1, Constantes.LARGEUR_NIVEAU - 1);
            } while (game.Carte[x, y] == (int)Constantes.TypeCase.Mur 
                || game.Carte[x, y] == (int)Constantes.TypeCase.MaisonFantome
                || game.Carte[x, y] == (int)Constantes.TypeCase.PorteFantome);

            return new Sommet(x, y);
        }

        /// <summary>
        /// Définir le sommet que le fantome doit rejoindre en fonction du fantome et de son état.
        /// </summary>
        /// <param name=sommetCourant>sommet correspondant aux coordonnées actuelles</param>
        public virtual Sommet trouverSommet(Sommet sommetCourant)
        {
            if (Etat == Constantes.EtatFantome.Mort)
                return game.Sommets[(int)Constantes.POSITION_INITIALE_FANTOME.Y, ((int)Constantes.POSITION_INITIALE_FANTOME.X + (int)Nom)];
            else if (Etat == Constantes.EtatFantome.Defensif)
                return prendreLaFuite();
            else
                return game.Sommets[(int)pacman.Position.Y, (int)pacman.Position.X];
        }

        /// <summary>
        /// Connaître la direction du fantôme grâce à l'algorithme de Dijkstra.
        /// </summary>
        public virtual void changerDirection()
        {
            
            Sommet sommetCourant = game.Sommets[(int)fantome.PositionGrille.Y, (int)fantome.PositionGrille.X];
            sommetCourant.Potentiel = 0;
            sommetCourant.Marque = true;
            Sommet sommetSuivant = sommetCourant;
            Sommet sommetPacman = game.Sommets[(int)pacman.Position.Y, (int)pacman.Position.X];
            //Trouver sommet permet de définir le sommet recherché en fonction de la configuration du jeu.
            //Méthode virtual qui est override dans chaque classe fantome.
            Sommet sommetViser = trouverSommet(sommetCourant);
            
            while (sommetSuivant.Coord != sommetViser.Coord)
            {
                sommetSuivant.Marque = true;

                if (sommetSuivant.Coord.X != 0 && sommetSuivant.Coord.Y != 0
                 && sommetSuivant.Coord.X != 30 && sommetSuivant.Coord.Y != 27)
                {
                    //Case du haut
                    if (testMur(new Vector2(sommetSuivant.Coord.X - 1, sommetSuivant.Coord.Y)))
                    {
                        Sommet s = game.Sommets[(int)sommetSuivant.Coord.X - 1, (int)sommetSuivant.Coord.Y];
                        if (s.Potentiel > sommetSuivant.Potentiel + 1)
                        {
                            game.Sommets[(int)sommetSuivant.Coord.X - 1, (int)sommetSuivant.Coord.Y].Pred = sommetSuivant;
                            game.Sommets[(int)sommetSuivant.Coord.X - 1, (int)sommetSuivant.Coord.Y].Potentiel = sommetSuivant.Potentiel + 1;
                        }
                    }
                    //Case du bas
                    if (testMur(new Vector2(sommetSuivant.Coord.X + 1, sommetSuivant.Coord.Y)))
                    {
                        Sommet s = game.Sommets[(int)sommetSuivant.Coord.X + 1, (int)sommetSuivant.Coord.Y];
                        if (s.Potentiel > sommetSuivant.Potentiel + 1)
                        {
                            game.Sommets[(int)sommetSuivant.Coord.X + 1, (int)sommetSuivant.Coord.Y].Pred = sommetSuivant;
                            game.Sommets[(int)sommetSuivant.Coord.X + 1, (int)sommetSuivant.Coord.Y].Potentiel = sommetSuivant.Potentiel + 1;
                        }
                    }
                    //Case de gauche
                    if (testMur(new Vector2(sommetSuivant.Coord.X, sommetSuivant.Coord.Y - 1)))
                    {
                        Sommet s = game.Sommets[(int)sommetSuivant.Coord.X, (int)sommetSuivant.Coord.Y - 1];
                        if (s.Potentiel > sommetSuivant.Potentiel + 1)
                        {
                            game.Sommets[(int)sommetSuivant.Coord.X, (int)sommetSuivant.Coord.Y - 1].Pred = sommetSuivant;
                            game.Sommets[(int)sommetSuivant.Coord.X, (int)sommetSuivant.Coord.Y - 1].Potentiel = sommetSuivant.Potentiel + 1;
                        }
                    }
                    //Case de droite
                    if (testMur(new Vector2(sommetSuivant.Coord.X, sommetSuivant.Coord.Y + 1)))
                    {
                        Sommet s = game.Sommets[(int)sommetSuivant.Coord.X, (int)sommetSuivant.Coord.Y + 1];
                        if (s.Potentiel > sommetSuivant.Potentiel + 1)
                        {
                            game.Sommets[(int)sommetSuivant.Coord.X, (int)sommetSuivant.Coord.Y + 1].Pred = sommetSuivant;
                            game.Sommets[(int)sommetSuivant.Coord.X, (int)sommetSuivant.Coord.Y + 1].Potentiel = sommetSuivant.Potentiel + 1;
                        }
                    }
                }

                int minimum = Sommet.INFINI;

                //Recherche du chemin le plus court
                for (int i = 0; i < game.Sommets.GetLength(0); i++)
                {
                    for (int j = 0; j < game.Sommets.GetLength(1); j++)
                    {
                        if (game.Sommets[i, j] != null && !game.Sommets[i, j].Marque && game.Sommets[i, j].Potentiel < minimum)
                        {
                            minimum = game.Sommets[i, j].Potentiel;
                            sommetSuivant = game.Sommets[i, j];
                        }
                    }
                }
            }

            //Recherche de la prochaine case où se déplacer
            Sommet caseSuivante = game.Sommets[(int)sommetSuivant.Coord.X, (int)sommetSuivant.Coord.Y];
            while (caseSuivante.Pred != null && caseSuivante.Pred.Coord != sommetCourant.Coord)
            {
                caseSuivante = caseSuivante.Pred;

                //Permet le déplacement du fantôme rose
                if (caseSuivante.Potentiel < 5
                && Math.Abs(caseSuivante.Coord.X - sommetPacman.Coord.X) < 4
                && Math.Abs(caseSuivante.Coord.Y - sommetPacman.Coord.Y) < 4
                && Etat == Constantes.EtatFantome.Offensif)
                {
                    pacman.VuParFantome = true;
                }
            }

            //Définition de la direction grâce à la caseSuivante
            if (fantome.PositionGrille.Y - caseSuivante.Coord.X == -1)
                Direction = Constantes.Direction.Bas;
            else if (fantome.PositionGrille.Y - caseSuivante.Coord.X == 1)
                Direction = Constantes.Direction.Haut;
            else if (fantome.PositionGrille.X - caseSuivante.Coord.Y == 1)
                Direction = Constantes.Direction.Gauche;
            else if (fantome.PositionGrille.X - caseSuivante.Coord.Y == -1)
                Direction = Constantes.Direction.Droite;
            else
                Direction = Constantes.Direction.Immobile;

            //Changement de sprite
            if (Direction != Constantes.Direction.Immobile && Etat != Constantes.EtatFantome.Mort && Etat != Constantes.EtatFantome.Defensif)
                fantome.Texture = spritesFantome[(int)Direction-1];

            //Réinitialisation des Sommets
            for (int i = 0; i < game.Sommets.GetLength(0); i++)
            {
                for (int j = 0; j < game.Sommets.GetLength(1); j++)
                {
                    if (game.Sommets[i, j] != null)
                    {
                        game.Sommets[i, j].Marque = false;
                        game.Sommets[i, j].Potentiel = Sommet.INFINI;
                        game.Sommets[i, j].Pred = null;
                    }
                }
            }
        }

        /// <summary>
        /// Prendre la fuite
        /// Si pacman est dans la partie bas/droite de la map, les fantomes fuient en haut à gauche.
        /// Si pacman est dans la partie bas/gauche de la map, les fantomes fuient en haut à droite.
        /// Si pacman est dans la partie haut/droite de la map, les fantomes fuient en bas à gauche.
        /// Si pacman est dans la partie haut/gauche de la map, les fantomes fuient en bas à droite.
        /// </summary>
        /// <returns>Le sommet de fuite</returns>
        public Sommet prendreLaFuite()
        {
            int x = 1;
            int y = 1;
            if (pacman.Position.X < Constantes.LARGEUR_NIVEAU / 2)
                x = Constantes.LARGEUR_NIVEAU - 2;
            if (pacman.Position.Y < Constantes.HAUTEUR_NIVEAU / 2)
                y = Constantes.HAUTEUR_NIVEAU - 2;
            return new Sommet(y, x);
        }

        /// <summary>
        /// Savoir si la case testée est un mur.
        /// </summary>
        /// <param name=position>Position de la case testée</param>
        /// <returns>True si le case est un mur, false sinon</returns>
        public Boolean testMur(Vector2 position)
        {
            int row = (int)position.X;
            int col = (int)position.Y;

            if (game.Carte[row, col] != 0)
                return true;
            return false;
        }

        /// <summary>
        /// Déplacer pixel par pixel le fantôme en fonction de la direction.
        /// </summary>
        public void deplacer()
        {
            Vector2 nouvellePosition = fantome.PositionPixel;

            if (Direction == Constantes.Direction.Haut)
                nouvellePosition.Y -= fantome.Vitesse;
            else if (Direction == Constantes.Direction.Bas)
                nouvellePosition.Y += fantome.Vitesse;
            else if (Direction == Constantes.Direction.Droite)
                nouvellePosition.X += fantome.Vitesse;
            else if (Direction == Constantes.Direction.Gauche)
                nouvellePosition.X -= fantome.Vitesse;

            fantome.PositionPixel = nouvellePosition;
        }

        /// <summary>
        /// Permet de garder le délai des fantômes lors d'une mise en pause du jeu
        /// </summary>
        public void conserverPause()
        {
            TempsStopSauvegarde = tempsStop - (DateTime.Now - tempsStopDebut);
            if (TempsStopSauvegarde.TotalMilliseconds < 0)
                TempsStopSauvegarde = TimeSpan.FromMilliseconds(0);
        }
    }
}
