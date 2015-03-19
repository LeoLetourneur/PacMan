using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe static permettant de jouer des sons.
    /// </summary>
    public static class SonsManager
    {
        /// <summary>
        /// Méthode static pour jouer le son MangerPacGomme
        /// </summary>
        public static void mangerPacGomme(GamePacMan game)
        {
            if (!game.Son)
                return;
            SoundEffect mangerPacGomme = game.Content.Load<SoundEffect>(@"sons\MangerPacGomme");
            SoundEffectInstance sound = mangerPacGomme.CreateInstance();
            sound.Play();
        }

        /// <summary>
        /// Méthode static pour jouer le son MangerPouvoir
        /// </summary>
        public static void mangerPouvoir(GamePacMan game)
        {
            if (!game.Son)
                return;
            SoundEffect mangerPouvoir = game.Content.Load<SoundEffect>(@"sons\MangerPouvoir");
            SoundEffectInstance sound = mangerPouvoir.CreateInstance();
            sound.Play();
        }

        /// <summary>
        /// Méthode static pour jouer le son MangerFruit
        /// </summary>
        public static void mangerFruit(GamePacMan game)
        {
            if (!game.Son)
                return;
            SoundEffect mangerFruit = game.Content.Load<SoundEffect>(@"sons\MangerFruit");
            SoundEffectInstance sound = mangerFruit.CreateInstance();
            sound.Play();
        }
        /// <summary>
        /// Méthode static pour jouer le son MangerFantome
        /// </summary>
        public static void mangerFantome(GamePacMan game)
        {
            if (!game.Son)
                return;
            SoundEffect mangerFantome = game.Content.Load<SoundEffect>(@"sons\MangerFantome");
            SoundEffectInstance sound = mangerFantome.CreateInstance();
            sound.Play();
        }
        /// <summary>
        /// Méthode static pour jouer le son NouveauJeu
        /// </summary>
        public static void nouveauJeu(GamePacMan game)
        {
            if (!game.Son)
                return;
            SoundEffect nouveauJeu = game.Content.Load<SoundEffect>(@"sons\NouveauJeu");
            SoundEffectInstance sound = nouveauJeu.CreateInstance();
            sound.Play();
        }
        /// <summary>
        /// Méthode static pour jouer le son NouveauNiveau
        /// </summary>
        public static void nouveauNiveau(GamePacMan game)
        {
            if (!game.Son)
                return;
            SoundEffect nouveauNiveau = game.Content.Load<SoundEffect>(@"sons\NouveauNiveau");
            SoundEffectInstance sound = nouveauNiveau.CreateInstance();
            sound.Play();
        }
        /// <summary>
        /// Méthode static pour jouer le son FantomePeur
        /// </summary>
        public static void fantomePeur(GamePacMan game)
        {
            if (!game.Son)
                return;
            SoundEffect fantomePeur = game.Content.Load<SoundEffect>(@"sons\FantomePeur");
            SoundEffectInstance sound = fantomePeur.CreateInstance();
            sound.Play();
        }
        /// <summary>
        /// Méthode static pour jouer le son PacManMort
        /// </summary>
        public static void pacmanMort(GamePacMan game)
        {
            if (!game.Son)
                return;
            SoundEffect pacmanMort = game.Content.Load<SoundEffect>(@"sons\PacmanMort");
            SoundEffectInstance sound = pacmanMort.CreateInstance();
            sound.Play();
        }
    }
}
