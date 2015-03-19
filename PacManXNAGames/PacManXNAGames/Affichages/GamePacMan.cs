using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe d'affichage du jeu
    /// </summary>
    public class GamePacMan : Microsoft.Xna.Framework.Game
    {
        //Framework
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont policeJeu;
        private Texture2D[] mur;
        private Texture2D porte;
        private Texture2D piece;
        private Texture2D pouvoir;
        private Texture2D vie;
        private PacMan pacman;
        private Fantome[] fantomes;

        //Bonus
        private List<Vector2> positionPopBonus;
        private bool bonusPop;
        public Dictionary<int, Texture2D> Bonus { get; set; }
        public int BonusCourant { get; set; }
        public int[] BonusObtenus { get; set; }
        
        //Map
        public byte[,] Carte { get; set; }
        public Sommet[,] Sommets { get; set; }

        //Partie en cours
        private int totalPiece;
        public int IndexVitesse { get; set; }
        public int NiveauEnCours { get; set; }
        public Constantes.EtatJeu Etat { get; set; }
        public int NombrePiece { get; set; }
        public int Vies { get; set; }
        public int ScoreVal { get; set; }
        public Boolean Son { get; set; }

        //Delai bonus
        private int tempsSupprimerBonus;
        private int tempsBonus;

        //Delai avant jeu
        private DateTime tempsStopDebut;
        private TimeSpan tempsStop_;
        public TimeSpan TempsStop {
            get { return tempsStop_; }
            set { tempsStopDebut = DateTime.Now; tempsStop_ = value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public GamePacMan()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = Constantes.HAUTEUR_NIVEAU * Constantes.COTE_CASE + 40;
            graphics.PreferredBackBufferWidth = Constantes.LARGEUR_NIVEAU * Constantes.COTE_CASE + 200;
            
            Content.RootDirectory = "Content";

            Son = true;
            Etat = Constantes.EtatJeu.Menu;
            Components.Add(new Menu(this, false));

            reinitialiserPartie();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            Services.AddService(typeof(GraphicsDeviceManager), graphics);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            pacman = new PacMan(this);
            fantomes = new Fantome[Constantes.NB_FANTOMES];
            fantomes[0] = new Fantome(this, pacman, (Constantes.Fantomes)0);
            fantomes[1] = new Inky(this, pacman, (Constantes.Fantomes)1);
            fantomes[2] = new Clyde(this, pacman, (Constantes.Fantomes)2);
            fantomes[3] = new Pinky(this, pacman, (Constantes.Fantomes)3);

            mur = new Texture2D[5];
            for(int i = 0;i<Constantes.NB_NIVEAUX;i++)
                mur[i] = Content.Load<Texture2D>(@"Niveaux\mur"+i);
            porte = Content.Load<Texture2D>(@"Niveaux\porte");
            piece = Content.Load<Texture2D>(@"Bonus\Piece");
            pouvoir = Content.Load<Texture2D>(@"Bonus\Pouvoir");
            vie = Content.Load<Texture2D>(@"Pacman\vie");
            
            Bonus = new Dictionary<int, Texture2D>(Constantes.NB_BONUS);
            Bonus.Add(0, Content.Load<Texture2D>("bonus/Cerise"));
            Bonus.Add(1, Content.Load<Texture2D>("bonus/Fraise"));
            Bonus.Add(2, Content.Load<Texture2D>("bonus/Pomme"));

            policeJeu = Content.Load<SpriteFont>("Fonts/Lucida");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /// 
        protected override void UnloadContent() {}
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if(pacman.Etat == Constantes.EtatPacman.Mort )
                pacman.Update(gameTime);
            if (DateTime.Now - tempsStopDebut < TempsStop || Etat == Constantes.EtatJeu.Menu)
            {
                base.Update(gameTime);
                return;
            }

            if(Etat == Constantes.EtatJeu.Debut)
                Etat = Constantes.EtatJeu.EnJeu;
            else if (Etat == Constantes.EtatJeu.FinNiveau)
            {
                SonsManager.nouveauNiveau(this);
                NiveauEnCours++;
                if (NiveauEnCours % Constantes.NB_NIVEAUX == 0)
                    IndexVitesse += Constantes.INCREMENT_VITESSE;
                pacman.reinitialiser();
                chargerNiveau();
            }
            else if (Etat == Constantes.EtatJeu.FinPartie)
            {
                Resultat scoreFinal = new Resultat(BonusObtenus, ScoreVal, NiveauEnCours);
                Resultat.sauvegarder(scoreFinal);
                Etat = Constantes.EtatJeu.Menu;
                Components.Add(new Classement(this, scoreFinal));
            }

            pacman.Update(gameTime);
            for (int i = 0; i < Constantes.NB_FANTOMES; i++)
                fantomes[i].Update(gameTime);

            //Vérification de la mise en pause
            if(Controles.VerifierToucheEspace())
            {
                for (int i = 0; i < Constantes.NB_FANTOMES; i++)
                    fantomes[i].conserverPause();
                Etat = Constantes.EtatJeu.Menu;
                Components.Add(new Menu(this, true));
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (Etat == Constantes.EtatJeu.Menu)
            {
                base.Draw(gameTime);
                return;
            }

            spriteBatch.Begin();

            for (int x = 0; x < Constantes.HAUTEUR_NIVEAU; x++)
            {
                for (int y = 0; y < Constantes.LARGEUR_NIVEAU; y++)
                {
                    int xpos = x * Constantes.COTE_CASE;
                    int ypos = y * Constantes.COTE_CASE;
                    Vector2 pos = new Vector2(ypos, xpos);

                    if (Carte[x, y] == (int)Constantes.TypeCase.Mur)
                        spriteBatch.Draw(mur[NiveauEnCours%Constantes.NB_NIVEAUX], pos, Color.Azure);
                    else if (Carte[x, y] == (int)Constantes.TypeCase.Piece)
                        spriteBatch.Draw(piece, pos, Color.Azure);
                    else if (Carte[x, y] == (int)Constantes.TypeCase.Pouvoir)
                        spriteBatch.Draw(pouvoir, pos, Color.Azure);
                    else if (Carte[x, y] == (int)Constantes.TypeCase.PorteFantome)
                        spriteBatch.Draw(porte, pos, Color.Azure);
                    else if (Carte[x, y] == (int)Constantes.TypeCase.BonusActif)
                        spriteBatch.Draw(Bonus[BonusCourant], pos, Color.Azure);
                }
            }

            if (Vies == 0 && Etat != Constantes.EtatJeu.FinPartie)
            {
                TempsStop = TimeSpan.FromMilliseconds(Constantes.PAUSE_JEU_INTER);
                Etat = Constantes.EtatJeu.FinPartie;
            }

            if (NombrePiece == 0 && Etat != Constantes.EtatJeu.FinNiveau)
            {
                TempsStop = TimeSpan.FromMilliseconds(Constantes.PAUSE_JEU_INTER);
                Etat = Constantes.EtatJeu.FinNiveau;
            }

            pacman.Draw(gameTime);
            for (int i = 0; i < Constantes.NB_FANTOMES; i++)
                fantomes[i].Draw(gameTime);

            //Faire apparaitre un bonus pendant un certain temps
            if (!bonusPop && NombrePiece <= totalPiece/2)
                popBonus();
            if (bonusPop && tempsSupprimerBonus != -1)
                supprimerBonus(gameTime);

            afficherEcrit();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Permet d'afficher toutes les indications de jeu, comme les vies, le score...
        /// </summary>
        public void afficherEcrit()
        {
            if (Etat == Constantes.EtatJeu.Debut)
            {
                spriteBatch.DrawString(policeJeu, "Preparez vous ! ",
                    new Vector2(118, this.GraphicsDevice.Viewport.Height - 35), Color.Yellow);
            }
            else if (Etat == Constantes.EtatJeu.FinNiveau)
            {
                spriteBatch.DrawString(policeJeu, "Niveau termine !",
                    new Vector2(100, this.GraphicsDevice.Viewport.Height - 35), Color.Yellow);
            }
            else if (Etat == Constantes.EtatJeu.FinPartie)
            {
                spriteBatch.DrawString(policeJeu, "Partie terminee !",
                    new Vector2(100, this.GraphicsDevice.Viewport.Height - 35), Color.Yellow);
            }
            else
                spriteBatch.DrawString(policeJeu, "Encore " + NombrePiece + " pac-gommes",
                    new Vector2(70, this.GraphicsDevice.Viewport.Height - 35), Color.Yellow);

            int posHauteur = 30;
            spriteBatch.DrawString(policeJeu, "Vitesse\n" + ((IndexVitesse - Constantes.VITESSE_PACMAN_DEBUT) / Constantes.INCREMENT_VITESSE), new Vector2(this.GraphicsDevice.Viewport.Width - 175, posHauteur), Color.Yellow);
            posHauteur += 95;
            spriteBatch.DrawString(policeJeu, "Niveau\n" + (NiveauEnCours + 1), new Vector2(this.GraphicsDevice.Viewport.Width - 175, posHauteur), Color.Yellow);
            posHauteur += 100;
            spriteBatch.DrawString(policeJeu, "Vies ", new Vector2(this.GraphicsDevice.Viewport.Width - 175, posHauteur), Color.Yellow);
            posHauteur += 38;
            for (int i = 0; i < Vies; i++)
                spriteBatch.Draw(vie, new Vector2(this.GraphicsDevice.Viewport.Width - 175 + 15 * i, posHauteur), Color.Azure);
            posHauteur += 50;
            spriteBatch.DrawString(policeJeu, "Score\n" + ScoreVal, new Vector2(this.GraphicsDevice.Viewport.Width - 175, posHauteur), Color.Yellow);
            posHauteur += 95;
            spriteBatch.DrawString(policeJeu, "Bonus ", new Vector2(this.GraphicsDevice.Viewport.Width - 175, posHauteur), Color.Yellow);
            posHauteur += 38;
            for (int i = 0; i < Bonus.Count; i++)
            {
                spriteBatch.Draw(Bonus[i], new Vector2(this.GraphicsDevice.Viewport.Width - 175, posHauteur + 10 + 50 * i), Color.Azure);
                spriteBatch.DrawString(policeJeu, "x " + BonusObtenus[i], new Vector2(this.GraphicsDevice.Viewport.Width - 140, posHauteur + 50 * i), Color.Yellow);
            }
        }

        /// <summary>
        /// Permet de réinitialiser les variables d'une partie.
        /// </summary>
        public void reinitialiserPartie()
        {
            BonusObtenus = new int[] { 0, 0, 0 };
            IndexVitesse = Constantes.VITESSE_PACMAN_DEBUT;
            NiveauEnCours = 0;
            ScoreVal = 0;
            Vies = Constantes.NB_VIES;
        }

        /// <summary>
        /// Permet de réinitialiser les variables d'un niveau et lire son contenu.
        /// </summary>
        public void chargerNiveau()
        {
            NombrePiece = 0;
            bonusPop = false;
            TempsStop = TimeSpan.FromMilliseconds(Constantes.PAUSE_JEU_DEBUT);
            lireNiveau();
            Etat = Constantes.EtatJeu.Debut;
            pacman.reinitialiser();
            reinitialiserFantome();
            pauseFantome(true);
        }

        /// <summary>
        /// Permet de lire le fichier texte qui contient le niveau et de le convertir en matrice de Integer.
        /// </summary>
        public void lireNiveau()
        {
            string nomFichier = "../../../../PacManXNAGamesContent/Niveaux/Niveau" + NiveauEnCours % Constantes.NB_NIVEAUX + ".txt";
            TextReader texte = new StreamReader(nomFichier);
            string ligne = texte.ReadLine();
            int ligneIndex = 0;
            int lettreIndex = 0;
            positionPopBonus = new List<Vector2>(Constantes.NB_CASE_POPBONUS);
            Carte = new byte[Constantes.HAUTEUR_NIVEAU, Constantes.LARGEUR_NIVEAU];
            Sommets = new Sommet[Constantes.HAUTEUR_NIVEAU, Constantes.LARGEUR_NIVEAU];
            while (ligne != null)
            {
                foreach (char mot in ligne)
                {
                    if (mot != ' ')
                    {
                        String chiffre = mot.ToString();
                        Carte[ligneIndex, lettreIndex] = Convert.ToByte(chiffre);
                        int caseMirroir = Constantes.LARGEUR_NIVEAU - lettreIndex - 1;
                        Carte[ligneIndex, caseMirroir] = Convert.ToByte(chiffre);

                        if (chiffre != ((int)Constantes.TypeCase.Mur).ToString())
                        {
                            Sommets[ligneIndex, lettreIndex] = new Sommet(ligneIndex, lettreIndex);
                            Sommets[ligneIndex, caseMirroir] = new Sommet(ligneIndex, caseMirroir);
                        }

                        if (chiffre == ((int)Constantes.TypeCase.Piece).ToString())
                            NombrePiece +=2;
                        else if (chiffre == ((int)Constantes.TypeCase.Bonus).ToString())
                        {
                            positionPopBonus.Add(new Vector2(ligneIndex, lettreIndex));
                            positionPopBonus.Add(new Vector2(ligneIndex, caseMirroir));
                        }
                        lettreIndex++;
                    }
                }
                lettreIndex = 0;
                ligneIndex++;
                ligne = texte.ReadLine();
                totalPiece = NombrePiece;
            }
            texte.Close();
            definirSommetAuVoisinagePouvoir();
        }

        /// <summary>
        /// Definir les sommets qui sont proches d'un super Pac-Gomme, pour que le fantôme Inky (Bleu) puisse fuir.
        /// </summary>
        public void definirSommetAuVoisinagePouvoir()
        {
            for (int a = 0; a < Constantes.HAUTEUR_NIVEAU; a++ )
                for (int b = 0; b < Constantes.LARGEUR_NIVEAU; b++)
                    if (Carte[a, b] == (int)Constantes.TypeCase.Pouvoir)
                    {
                        definirIsProchePouvoir(a, b, true);
                    }
        }

        /// <summary>
        /// Definir la propriété isProcheSommet des sommet proche du Sommet (a,b)
        /// <param name=a>Coordonnée a du Sommet</param>
        /// <param name=b>Coordonnée b du Sommet</param>
        /// <param name=actif>Defini la propriété</param>
        /// </summary>
        public void definirIsProchePouvoir(int a, int b, Boolean actif)
        {
            for (int i = 0; i <= 6; i++)
                for (int j = 0; j <= 6; j++)
                {
                    int w = a + i - 3;
                    int h = b + j - 3;

                    if (w >= 0 && 
                        h >= 0 && 
                        w < Constantes.HAUTEUR_NIVEAU && 
                        h < Constantes.LARGEUR_NIVEAU && 
                        Sommets[w, h] != null)
                            Sommets[w, h].isProchePouvoir = actif;
                }
        }

        /// <summary>
        /// Permet de faire apparaitre un bonus par niveau.
        /// </summary>
        public void popBonus()
        {
            Random rand = new Random();
            int numCase = rand.Next(Constantes.NB_CASE_POPBONUS);
            Vector2 posBonusCourant = positionPopBonus[numCase];
            Carte[(int)posBonusCourant.X, (int)posBonusCourant.Y] = (int)Constantes.TypeCase.BonusActif;
            BonusCourant = rand.Next(Constantes.NB_BONUS);
            bonusPop = true;
            tempsBonus = rand.Next(Constantes.TEMPS_MINIMUM_BONUS, Constantes.TEMPS_MAXIMUM_BONUS);
            tempsSupprimerBonus = 0;
        }

        /// <summary>
        /// Permet de faire disparaitre le bonus au bout d'un certain temps.
        /// </summary>
        public void supprimerBonus(GameTime gametime)
        {
            tempsSupprimerBonus += gametime.ElapsedGameTime.Milliseconds;
            if (tempsSupprimerBonus > tempsBonus)
            {
                tempsSupprimerBonus = -1; // Pour ne pas repasser dans la méthode quand le bonus à disparu
                foreach (Vector2 pos in positionPopBonus)
                {
                    if (Carte[(int)pos.X, (int)pos.Y] == (int)Constantes.TypeCase.BonusActif)
                        Carte[(int)pos.X, (int)pos.Y] = (int)Constantes.TypeCase.Bonus;
                }
            }
        }

        /// <summary>
        /// Permet de mettre en pause tous les fantômes.
        /// Si un joueur met pause alors que les fantômes étaient en stand-by, lorsque l'on reprend, la pause est encore effective.
        /// </summary>
        /// <param name=isTotalPause>Permet de savoir d'où provient la pause</param>
        public void pauseFantome(Boolean isTotalPause)
        {
            for (int i = 0; i < Constantes.NB_FANTOMES; i++)
            {
                if (isTotalPause)
                    fantomes[i].tempsStop = TimeSpan.FromMilliseconds(Constantes.PAUSE_JEU_DEBUT + (i+1) * Constantes.PAUSE_FANTOME);
                else
                    fantomes[i].tempsStop = fantomes[i].TempsStopSauvegarde;
            }
        }

        /// <summary>
        /// Permet de réinitialiser tous les fantômes
        /// </summary>
        public void reinitialiserFantome()
        {
            for (int i = 0; i < Constantes.NB_FANTOMES; i++)
                fantomes[i].reinitialiser();
        }
    }
}
