using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Liv_in_Paris
{
    public class CuisinierService
    {
        private string connectionString = "SERVER=127.0.0.1;PORT=3306;" +
                                          "DATABASE=LivInParis;" +
                                          "UID=root;PASSWORD=Poulet69!270705;" + "CharSet=utf8mb4;";

        /// <summary>
        /// Ajoute un nouveau plat à la base de données pour un cuisinier donné.
        /// </summary>
        /// <param name="idUtilisateur">L'identifiant du cuisinier connecté</param>
        public void AjouterPlat(int idUtilisateur)
        {
            Console.Clear();
            Console.WriteLine("=== Ajout d'un nouveau plat ===\n");
            Console.Write("Nom du plat : ");
            string nom = Console.ReadLine();
            Console.WriteLine();
            string type;
            do
            {
                Console.Write("Type (entrée, plat principal, dessert) : ");
                type = Console.ReadLine().ToLower();
                Console.WriteLine();
            } while (type != "entrée" && type != "plat principal" && type != "dessert");
            var nationalites = ObtenirNationalitesExistantes();
            Console.WriteLine("\nChoisissez une nationalité existante ou entrez une nouvelle :");
            for (int i = 0; i < nationalites.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {nationalites[i]}");
            }
            Console.WriteLine($"{nationalites.Count + 1}. Autre (ajouter une nouvelle)");

            string nationalite;
            int choixNat = 0;
            do
            {
                Console.Write("\nVotre choix : ");
                bool valid = int.TryParse(Console.ReadLine(), out choixNat) && choixNat >= 1 && choixNat <= nationalites.Count + 1;
                if (valid && choixNat <= nationalites.Count)
                {
                    nationalite = nationalites[choixNat - 1];
                }
                else if (choixNat == nationalites.Count + 1)
                {
                    Console.Write("Entrez une nouvelle nationalité : ");
                    nationalite = Console.ReadLine();
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("Choix invalide, veuillez réessayer.");
                    continue;
                }
                break;
            } while (true);

            Console.Write("Ingrédients principaux : ");
            string ingredients = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Régime alimentaire : ");
            string regime = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Recette : ");
            string recette = Console.ReadLine();
            Console.WriteLine();

            decimal prix;
            do
            {
                Console.Write("Prix du plat : ");
            } while (!decimal.TryParse(Console.ReadLine(), out prix) || prix <= 0);
            Console.WriteLine();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string requete = @"
                        INSERT INTO Plat (Nom, Type, Date_Fabrication, Date_Péremption, Nationalite,
                        Ingrédients_Principaux, Régime_Alimentaire, Recette, Déclinaison, Id_Utilisateur, Prix)
                        VALUES (@Nom, @Type, @DateFabrication, @DatePeremption, @Nationalite,
                        @Ingredients, @Regime, @Recette, @Declinaison, @IdUtilisateur, @Prix)";

                    MySqlCommand commande = new MySqlCommand(requete, connection);
                    commande.Parameters.AddWithValue("@Nom", nom);
                    commande.Parameters.AddWithValue("@Type", type);
                    commande.Parameters.AddWithValue("@DateFabrication", DateTime.Now);
                    commande.Parameters.AddWithValue("@DatePeremption", DateTime.Now.AddDays(5));
                    commande.Parameters.AddWithValue("@Nationalite", nationalite);
                    commande.Parameters.AddWithValue("@Ingredients", ingredients);
                    commande.Parameters.AddWithValue("@Regime", regime);
                    commande.Parameters.AddWithValue("@Recette", recette);
                    commande.Parameters.AddWithValue("@Declinaison", "Classique");
                    commande.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);
                    commande.Parameters.AddWithValue("@Prix", prix);

                    commande.ExecuteNonQuery();
                    Console.WriteLine("\nPlat ajouté avec succès !");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur SQL lors de l'ajout du plat : " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Récupère la liste des nationalités de plats déjà existantes dans la base de données.
        /// </summary>
        /// <returns>Liste des nationalités uniques</returns>
        private List<string> ObtenirNationalitesExistantes()
        {
            List<string> nationalites = new List<string>();
            using (MySqlConnection connexion = new MySqlConnection(connectionString))
            {
                try
                {
                    connexion.Open();
                    var commande = new MySqlCommand("SELECT DISTINCT Nationalite FROM Plat", connexion);
                    var lecteur = commande.ExecuteReader();
                    while (lecteur.Read())
                    {
                        nationalites.Add(lecteur.GetString("Nationalite"));
                    }
                    lecteur.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du chargement des nationalités : " + ex.Message);
                }
            }
            return nationalites;
        }
    }
}
