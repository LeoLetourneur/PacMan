using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PacManXNAGames
{
    /// <summary>
    ///  LETOURNEUR Léo - ISI
    ///  Classe des résultat qui permet de générer un classement
    /// </summary>
    [Serializable]
    class Resultat
    {
        private static String chemin = "../../../../PacManXNAGamesContent/Niveaux/scores.bin";
        private int[] bonusObtenus_;
        private int valeur_;
        private int niveau_;
        public int[] BonusObtenus { get { return bonusObtenus_; } }
        public int Valeur { get { return valeur_; } }
        public int Niveau { get { return niveau_; } }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name=bonus>Tableau du nombre de bonus</param>
        /// <param name=score>Valeur du score</param>
        /// <param name=level>Dernier niveau</param>
        public Resultat(int[] bonus, int score, int level)
        {
            bonusObtenus_ = bonus;
            valeur_ = score;
            niveau_ = level+1;
        }

        public static void sauvegarder(Resultat score)
        {
            List<Resultat> listResultats = Resultat.charger();
            listResultats.Add(score);
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(chemin, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, listResultats);
            stream.Close();
        }

        /// <summary>
        /// Méthode static pour lire les résultats enregistrés
        /// </summary>
        /// <returns>Liste des résultats contenus dans un fichier binaire</returns>
        public static List<Resultat> charger()
        {
            IFormatter formatter = new BinaryFormatter();
            List<Resultat> listScore = new List<Resultat>(); ;
            if (File.Exists(chemin))
            {
                Stream stream = new FileStream(chemin, FileMode.Open, FileAccess.Read, FileShare.Read);
                try
                {
                    listScore = (List<Resultat>)formatter.Deserialize(stream);
                }
                catch (SerializationException e) { Console.Write("Problème de serialisation :\n"+e.Message); }
                finally {stream.Close();}
            }
            return listScore;
        }

        /// <summary>
        /// Savoir si les deux résultats sont équivalents
        /// </summary>
        /// <param name=score2>Résultat à comparer</param>
        /// <returns>True si les deux résultats sont équivalents, false sinon</returns>
        public Boolean correspond(Resultat score2)
        {
            if (this.Valeur == score2.Valeur && this.BonusObtenus[0] == score2.BonusObtenus[0]
            && this.BonusObtenus[1] == score2.BonusObtenus[1]
            && this.BonusObtenus[2] == score2.BonusObtenus[2] && this.Niveau == score2.Niveau)
                return true;
            return false;
        }
    }
}
