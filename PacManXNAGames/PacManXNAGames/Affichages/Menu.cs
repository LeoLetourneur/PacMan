using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe d'affichage du menu
    /// </summary>
    class Menu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private GamePacMan game;
        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphics;
        private SpriteFont font;
        private Texture2D selectionVisuelle;
        private Texture2D logo;
        private Texture2D[] son;
        private Boolean isPause;
        private string[] items;
        private int selection;
        private int blocage;
        private const int ReductionVitesse = 80;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name=game>Partie en cours</param>
        /// <param name=pause>Si pause = false le menu de jeu s'affiche, sinon c'est la pause.</param>
        public Menu(GamePacMan game, Boolean pause) : base(game)
        {
            this.game = game;
            this.isPause = pause;
            blocage = -1000;
        }

        public override void Initialize()
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(GraphicsDeviceManager));
            selection = 0;
            if (!isPause)
                items = new string[] { "Jouer", "Sons", "Scores", "Quitter" };
            else
                items = new string[] { "Reprendre", "Sons", "Abandonner" };

            logo = Game.Content.Load<Texture2D>("Menu/logo");
            selectionVisuelle = Game.Content.Load<Texture2D>("Menu/pac");
            font = Game.Content.Load<SpriteFont>("Fonts/Lucida");
            son = new Texture2D[] { Game.Content.Load<Texture2D>("Menu/SonOn"), 
                                    Game.Content.Load<Texture2D>("Menu/SonOff") };
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            blocage += gameTime.ElapsedGameTime.Milliseconds;

            if (blocage > ReductionVitesse)
            {
                blocage = 0;

                if (Controles.VerifierToucheBas())
                {
                    selection++;
                    selection %= items.Length;
                }
                else if (Controles.VerifierToucheHaut())
                {
                    selection--;
                    if(selection < 0)
                        selection = items.Length - 1;
                }
                else if (Controles.VerifierToucheEntrer())
                {
                    if(items[selection] != "Sons")
                        Game.Components.Remove(this);
                    switch (items[selection])
                    {
                        case ("Jouer"):
                            game.reinitialiserPartie();
                            game.chargerNiveau();
                            SonsManager.nouveauJeu(game);
                            break;
                        case ("Scores"):
                            Game.Components.Add(new Classement(game, null));
                            break;
                        case ("Quitter"):
                            game.Exit();
                            break;
                        case ("Reprendre"):
                            game.Etat = Constantes.EtatJeu.EnJeu;
                            game.pauseFantome(false);
                            break;
                        case ("Sons"):
                            game.Son = !game.Son;
                            break;
                        case ("Abandonner"):
                            game.Etat = Constantes.EtatJeu.Menu;
                            Game.Components.Add(new Menu(game, false));
                            Resultat.sauvegarder(new Resultat(game.BonusObtenus, game.ScoreVal, game.NiveauEnCours));
                            break;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin();

            spriteBatch.Draw(logo, new Vector2((graphics.PreferredBackBufferWidth / 2) - (logo.Width / 2), 60), Color.White);
            spriteBatch.Draw(selectionVisuelle, new Vector2((graphics.PreferredBackBufferWidth / 2) - 160,
                                                    (graphics.PreferredBackBufferHeight / 2) -100 + (100 * selection)), Color.White);

            int indiceSon = 0;
            if (!game.Son) indiceSon = 1;
            spriteBatch.Draw(son[indiceSon], new Vector2((graphics.PreferredBackBufferWidth / 2) + 90,
                                                        (graphics.PreferredBackBufferHeight / 2) + 13), Color.White);
            
            Vector2 itemPosition;
            itemPosition.X = (graphics.PreferredBackBufferWidth / 2) - 60;
            for (int i = 0; i < items.Length; i++)
            {
                itemPosition.Y = (graphics.PreferredBackBufferHeight / 2) - 80 + (100 * i);
                spriteBatch.DrawString(font, items[i], itemPosition, Color.Orange);
            }

            spriteBatch.End();
        }
    }
}
