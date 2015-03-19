using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe PacMan
    /// </summary>
    public class PacMan 
    {
        //Framework
        private SpriteBatch spriteBatch;
        private GamePacMan game;
        private ObjetAnime pacman;
        private Texture2D[] spritesPacman;
        private int nbSonsJoues;

        //PacMan
        private int indexMort;
        public Vector2 Position {
            get { return pacman.PositionGrille; }
        }
        private BoundingBox boxCaseActuelle;
        public BoundingBox boxPacMan { get; set; }
        public int NbFantomeMange { get; set; }
        public Constantes.Direction Direction { get; set; }
        public Constantes.EtatPacman Etat { get; set; }
        public Boolean VuParFantome { get; set; }

        //Mise à jour Frame
        private int tempsFenetre;
        public int DivisionVitesse { get; set; }
        //Mise à jour sprite
        private int tempsAutreSprite;
        private const int latence = 128;
        private int indexSprite;

        //Délais
        private DateTime tempsInvincibleDebut;
        private TimeSpan tempsInvicible_;
        private TimeSpan tempsInvincible
        {
            get { return tempsInvicible_; }
            set { tempsInvincibleDebut = DateTime.Now; tempsInvicible_ = value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name=game>Partie en cours</param>
        public PacMan(GamePacMan game)
        {
            this.game = game;

            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            spritesPacman = new Texture2D[24];
            spritesPacman[0] = game.Content.Load<Texture2D>(@"Pacman\pacmanHaut0");
            spritesPacman[1] = game.Content.Load<Texture2D>(@"Pacman\pacmanBas0");
            spritesPacman[2] = game.Content.Load<Texture2D>(@"Pacman\pacmanGauche0");
            spritesPacman[3] = game.Content.Load<Texture2D>(@"Pacman\pacmanDroite0");
            spritesPacman[4] = game.Content.Load<Texture2D>(@"Pacman\pacmanHaut1");
            spritesPacman[5] = game.Content.Load<Texture2D>(@"Pacman\pacmanBas1");
            spritesPacman[6] = game.Content.Load<Texture2D>(@"Pacman\pacmanGauche1");
            spritesPacman[7] = game.Content.Load<Texture2D>(@"Pacman\pacmanDroite1");
            spritesPacman[12] = game.Content.Load<Texture2D>(@"Pacman\pacmort0");
            spritesPacman[13] = game.Content.Load<Texture2D>(@"Pacman\pacmort1");
            spritesPacman[14] = game.Content.Load<Texture2D>(@"Pacman\pacmort2");
            spritesPacman[15] = game.Content.Load<Texture2D>(@"Pacman\pacmort3");
            spritesPacman[16] = game.Content.Load<Texture2D>(@"Pacman\pacmort4");
            spritesPacman[17] = game.Content.Load<Texture2D>(@"Pacman\pacmort5");
            spritesPacman[18] = game.Content.Load<Texture2D>(@"Pacman\pacmort6");
            spritesPacman[19] = game.Content.Load<Texture2D>(@"Pacman\pacmort7");
            spritesPacman[20] = game.Content.Load<Texture2D>(@"Pacman\pacmort8");
            spritesPacman[21] = game.Content.Load<Texture2D>(@"Pacman\pacmort9");
            spritesPacman[22] = game.Content.Load<Texture2D>(@"Pacman\pacmort10");
            spritesPacman[23] = game.Content.Load<Texture2D>(@"Pacman\pacmort11");

            DivisionVitesse = Constantes.VITESSE_PACMAN_DEBUT;
            float vitesse = 2f;
            pacman = new ObjetAnime(null, new Vector2(), new Vector2(Constantes.COTE_CASE, Constantes.COTE_CASE), vitesse);
            reinitialiser();
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(pacman.Texture, pacman.PositionPixel, Color.Azure);
        }

        public void Update(GameTime gameTime)
        {
            tempsFenetre += gameTime.ElapsedGameTime.Milliseconds;
            VuParFantome = false;
            if (tempsFenetre > DivisionVitesse)
            {
                if (DivisionVitesse < gameTime.ElapsedGameTime.Milliseconds)
                    tempsFenetre = 0;
                else
                    tempsFenetre -= DivisionVitesse;

                if (Etat == Constantes.EtatPacman.Mort)
                {
                    if (indexMort < Constantes.NB_SPRITE_PACMORT)
                    {
                        DivisionVitesse = Constantes.VITESSE_SPRITE_PACMORT;
                        Direction = Constantes.Direction.Immobile;
                        pacman.Texture = spritesPacman[Constantes.NB_SPRITE_PACMORT + indexMort];
                    }
                    else
                        reinitialiser();
                    indexMort++;
                }
                else if (Etat == Constantes.EtatPacman.Invincible)
                {
                    if (DateTime.Now - tempsInvincibleDebut >= tempsInvincible)
                    {
                        nbSonsJoues = 1;
                        NbFantomeMange = 0;
                        Etat = Constantes.EtatPacman.Normal;
                    }
                    else //Permet de jouer plusieurs fois le son jusqu'à ce que le fantôme ne soit plus invincible
                    {
                        //Vérification fin son
                        if ((DateTime.Now - tempsInvincibleDebut).TotalMilliseconds - (1200 * nbSonsJoues) >= 0) 
                            //Vérification temps restant pour que le son ce termine avant le bonus
                            if (DateTime.Now - tempsInvincibleDebut < tempsInvincible - TimeSpan.FromMilliseconds(1500)) 
                            {
                                SonsManager.fantomePeur(game);
                                nbSonsJoues++;
                            }
                    }
                }

                boxPacMan = new BoundingBox(new Vector3(pacman.PositionPixel.X, pacman.PositionPixel.Y, 0),
                                            new Vector3(pacman.PositionPixel.X + Constantes.COTE_CASE, pacman.PositionPixel.Y + Constantes.COTE_CASE, 0));

                boxCaseActuelle = new BoundingBox(new Vector3(pacman.PositionGrille.X * Constantes.COTE_CASE, pacman.PositionGrille.Y * Constantes.COTE_CASE, 0),
                                            new Vector3(pacman.PositionGrille.X * Constantes.COTE_CASE + Constantes.COTE_CASE, 
                                                        pacman.PositionGrille.Y * Constantes.COTE_CASE + Constantes.COTE_CASE, 0));

                //Si PacMan est entièrement sur une case
                if (boxCaseActuelle.Contains(boxPacMan) == ContainmentType.Contains)
                {
                    verifierContenuCase();
                    verifierToucheClavier();
                }
                deplacerPacMan(gameTime);
            }
        }

        /// <summary>
        /// Interagir avec les objets sur la map.
        /// </summary>
        private void verifierContenuCase()
        {
            int X = (int)pacman.PositionGrille.X;
            int Y = (int)pacman.PositionGrille.Y;

            if (game.Carte[Y, X] == (int)Constantes.TypeCase.Piece)
            {
                game.ScoreVal += Constantes.POINT_GOMME;
                game.Carte[Y, X] = (int)Constantes.TypeCase.Rien;
                game.NombrePiece--;
                SonsManager.mangerPacGomme(game);

                if (game.NombrePiece == 0)
                    Direction = Constantes.Direction.Immobile;
            }
            else if (game.Carte[Y, X] == (int)Constantes.TypeCase.Pouvoir)
            {
                game.ScoreVal += Constantes.POINT_SUPER;
                game.Carte[Y, X] = (int)Constantes.TypeCase.Rien;
                game.definirIsProchePouvoir(Y, X, false);
                SonsManager.mangerPouvoir(game);

                Etat = Constantes.EtatPacman.Invincible;
                tempsInvincible = TimeSpan.FromMilliseconds(Constantes.TEMPS_PEUR[game.NiveauEnCours%Constantes.NB_NIVEAUX]);
                SonsManager.fantomePeur(game);
            }
            else if (game.Carte[Y, X] == (int)Constantes.TypeCase.BonusActif)
            {
                game.ScoreVal += Constantes.POINT_BONUS*(game.BonusCourant+1);
                game.BonusObtenus[game.BonusCourant]++;
                game.Carte[Y, X] = (int)Constantes.TypeCase.Bonus;
                SonsManager.mangerFruit(game);
            }
        }
        /// <summary>
        /// Savoir si la case testée est un mur ou la maison des fantômes.
        /// </summary>
        /// <param name=position>Position de la case testée</param>
        /// <returns>True si le case est un mur, false sinon</returns>
        public Boolean testMur(Vector2 position)
        {
            int row = (int)position.Y / Constantes.COTE_CASE;
            int col = (int)position.X / Constantes.COTE_CASE;

            if (game.Carte[row, col] != 0 && game.Carte[row, col] != 7)
                return true;
            return false;
        }
        /// <summary>
        /// Changement de direction du PacMan en fonction des touches pressées
        /// </summary>
        public void verifierToucheClavier()
        {
            int direction = Controles.VerifierTouchePressee();
            if ( direction != (int)Constantes.Direction.Immobile)
            {
                float vitesseX = 0;
                float vitesseY = 0;

                if (direction == (int)Constantes.Direction.Haut)
                    vitesseY = -pacman.Vitesse;
                else if (direction == (int)Constantes.Direction.Bas)
                    vitesseY = pacman.Vitesse + Constantes.COTE_CASE - 1;
                else if (direction == (int)Constantes.Direction.Gauche)
                    vitesseX = -pacman.Vitesse;
                else if (direction == (int)Constantes.Direction.Droite)
                    vitesseX = pacman.Vitesse + Constantes.COTE_CASE -1;

                Vector2 pos = pacman.PositionPixel;
                pos.X += vitesseX;
                pos.Y += vitesseY;

                if (testMur(pos))
                {
                    Direction = (Constantes.Direction)direction;
                    indexSprite = direction - 1;
                    pacman.Texture = spritesPacman[indexSprite];
                }
            }
        }
        /// <summary>
        /// Déplacer pixel par pixel le PacMan en fonction de la direction
        /// </summary>
        public void deplacerPacMan(GameTime gameTime)
        {
            if (Direction != Constantes.Direction.Immobile)
            {
                tempsAutreSprite += gameTime.ElapsedGameTime.Milliseconds;
                if (tempsAutreSprite > latence)
                {
                    tempsAutreSprite = 0;
                    if(indexSprite>3)
                        indexSprite -= 4;
                    else
                        indexSprite += 4;
                    pacman.Texture = spritesPacman[indexSprite];
                }
                
                float vitesseX = 0;
                float vitesseY = 0;
                float vitesseXMur = 0;
                float vitesseYMur = 0;

                if (Direction == Constantes.Direction.Haut)
                {
                    vitesseY = -pacman.Vitesse;
                    vitesseYMur = -pacman.Vitesse;
                }
                else if (Direction == Constantes.Direction.Bas)
                {
                    vitesseY = pacman.Vitesse;
                    vitesseYMur = pacman.Vitesse + Constantes.COTE_CASE - 1;
                }
                else if (Direction == Constantes.Direction.Gauche)
                {
                    vitesseX = -pacman.Vitesse;
                    vitesseXMur = -pacman.Vitesse;
                }
                else if (Direction == Constantes.Direction.Droite)
                {
                    vitesseX = pacman.Vitesse;
                    vitesseXMur = pacman.Vitesse + Constantes.COTE_CASE - 1;
                }

                Vector2 pos = pacman.PositionPixel;
                pos.X += vitesseX;
                pos.Y += vitesseY;
                if (Direction == Constantes.Direction.Gauche)
                    if (pos.X <= 0)
                        pos.X = 538;
                if (Direction == Constantes.Direction.Droite)
                    if (pos.X >= 540)
                        pos.X = 0;

                Vector2 posMur = pacman.PositionPixel;
                posMur.X += vitesseXMur;
                posMur.Y += vitesseYMur;
                if (Direction == Constantes.Direction.Gauche)
                    if (posMur.X <= 0)
                        posMur.X = (Constantes.LARGEUR_NIVEAU - 1) * Constantes.COTE_CASE - 2;
                if (Direction == Constantes.Direction.Droite)
                    if (posMur.X >= (Constantes.LARGEUR_NIVEAU-1) * Constantes.COTE_CASE)
                        posMur.X = 0;

                if (testMur(posMur))
                {
                    pacman.PositionPixel = pos;
                }
                else
                    Direction = Constantes.Direction.Immobile;
            }
        }
        /// <summary>
        /// Réinitialiser le PacMan.
        /// </summary>
        public void reinitialiser()
        {
            indexMort = 0;
            nbSonsJoues = 1;
            NbFantomeMange = 0;
            pacman.PositionPixel = Constantes.POSITION_INITIALE_PAC * Constantes.COTE_CASE;
            pacman.Texture = pacman.Texture = spritesPacman[3];
            Direction = Constantes.Direction.Immobile;
            Etat = Constantes.EtatPacman.Normal;
            DivisionVitesse = game.IndexVitesse;
            VuParFantome = false;
            tempsFenetre = 0;
            tempsAutreSprite = 0;
            indexSprite = 0;
        }
    }
}