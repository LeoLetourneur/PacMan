using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe d'affichage du classement
    /// </summary>
    class Classement : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphics;
        private SpriteFont font;
        private GamePacMan game;
        private List<Resultat> resultats;
        private Resultat dernierResultat;
        private int blocage;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name=game>Partie en cours</param>
        /// <param name=resultatPartie>Resultat de la partie en cours</param>
        public Classement(GamePacMan game, Resultat resultatPartie) : base(game) 
        {
            this.game = game;
            this.dernierResultat = resultatPartie;

            resultats = Resultat.charger();
            resultats.OrderBy(order => order.Valeur);
            resultats.Sort(delegate(Resultat x, Resultat y)
            {
                return x.Valeur.CompareTo(y.Valeur);
            });
            resultats.Reverse();
            blocage = -1000;
        }

        public override void Initialize() {

            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(GraphicsDeviceManager));
            font = Game.Content.Load<SpriteFont>("Fonts/Lucida");
            
            base.Initialize();
        }

        public override void Update(GameTime gameTime) {
            blocage += gameTime.ElapsedGameTime.Milliseconds;
            if (blocage > 0)
            {
                if (Controles.VerifierToucheEntrer())
                {
                    Game.Components.Remove(this);
                    Game.Components.Add(new Menu(game, false));
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);
            Vector2 positionMilieu = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Classement", new Vector2(positionMilieu.X - 120, 30), Color.Orange);
            spriteBatch.DrawString(font, "Score", new Vector2(positionMilieu.X - 220, 90), Color.Orange);
            spriteBatch.DrawString(font, "Niveau", new Vector2(positionMilieu.X + 200, 90), Color.Orange);
            spriteBatch.DrawString(font, "\"Entrer\" pour retourner au menu", new Vector2(positionMilieu.X - 340, positionMilieu.Y + 250), Color.Orange);

            for (int i = 0; i < Constantes.NB_BONUS; i++)
                spriteBatch.Draw(game.Bonus[i], new Vector2(positionMilieu.X - 30 + 80 * i, positionMilieu.Y - 230), Color.White);

            Color couleur;
            bool dejaClasser = false;
            for (int i = 0; i < 10; i++)
            {
                couleur = Color.Orange;
                if (i < resultats.Count)
                {
                    if (!dejaClasser && dernierResultat != null && dernierResultat.correspond(resultats[i]))
                    {
                        couleur = Color.Red;
                        dejaClasser = true;
                    }
                    spriteBatch.DrawString(font, resultats[i].Valeur.ToString(), new Vector2(positionMilieu.X - 220, positionMilieu.Y - 200 + (40 * i)), couleur);
                    for (int j = 0; j < Constantes.NB_BONUS; j++)
                        spriteBatch.DrawString(font, resultats[i].BonusObtenus[j].ToString(), new Vector2(positionMilieu.X - 30 + 80 * j, positionMilieu.Y - 200 + (40 * i)), couleur);
                    spriteBatch.DrawString(font, resultats[i].Niveau.ToString(), new Vector2(positionMilieu.X + 250, positionMilieu.Y - 200 + (40 * i)), couleur);
                }
                spriteBatch.DrawString(font, (i + 1).ToString() + ".", new Vector2(positionMilieu.X - 300, positionMilieu.Y - 200 + (40 * i)), couleur);
            }

            spriteBatch.End();
        }
    }
}
