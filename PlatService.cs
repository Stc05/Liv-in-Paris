using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Liv_in_Paris
{
    public class PlatService
    {
        private string connectionString = "SERVER=127.0.0.1;PORT=3306;" +
                                          "DATABASE=LivInParis;" +
                                          "UID=root;PASSWORD=Poulet69!270705;" + "CharSet=utf8mb4;";
        private Graph graphe;

        public PlatService()
        {
            graphe = new Graph("metro.csv");
        }

        /// <summary>
        /// Affiche les plats disponibles en fonction d'une nationalité et d'un type, avec le nom de la station.
        /// </summary>
        public void AfficherPlatsFiltres(string nationalite, string typePlat)
        {
            string requete = @"
                SELECT p.Id_Plat, p.Nom, p.Type, p.Recette, p.Prix,
                       u.Nom AS CuisinierNom, u.Station_Proche AS StationId
                FROM Plat p
                JOIN Cuisinier c ON p.Id_Utilisateur = c.Id_Utilisateur
                JOIN Utilisateur u ON c.Id_Utilisateur = u.Id_Utilisateur
                WHERE p.Nationalite = @Nationalite AND p.Type = @Type";

            using (MySqlConnection connexion = new MySqlConnection(connectionString))
            {
                try
                {
                    connexion.Open();
                    MySqlCommand commande = new MySqlCommand(requete, connexion);
                    commande.Parameters.AddWithValue("@Nationalite", nationalite);
                    commande.Parameters.AddWithValue("@Type", typePlat);

                    MySqlDataReader lecteur = commande.ExecuteReader();

                    Console.WriteLine($"\nListe des {typePlat}s disponibles :");

                    while (lecteur.Read())
                    {
                        int idPlat = (int)lecteur["Id_Plat"];
                        string nomPlat = lecteur["Nom"].ToString();
                        string recette = lecteur["Recette"].ToString();
                        decimal prix = (decimal)lecteur["Prix"];
                        string nomCuisinier = lecteur["CuisinierNom"].ToString();
                        int stationId = Convert.ToInt32(lecteur["StationId"]);

                        string nomStation = graphe.Noeuds.ContainsKey(stationId) ? graphe.Noeuds[stationId].Nom : "Inconnue";

                        Console.OutputEncoding = System.Text.Encoding.UTF8;
                        Console.WriteLine($"ID: {idPlat} - Plat: {nomPlat} - Recette: {recette} - Prix: {prix} €");
                        Console.WriteLine($"Cuisinier: {nomCuisinier} - Station: {nomStation}");
                        Console.WriteLine("--------------------------------------------");
                    }

                    lecteur.Close();
                }
                catch (Exception erreur)
                {
                    Console.WriteLine("Erreur lors de la récupération des plats : " + erreur.Message);
                }
            }
        }

        /// <summary>
        /// Affiche tous les plats ajoutés par un cuisinier donné.
        /// </summary>
        public void AfficherPlatsParCuisinier(int identifiantUtilisateur)
        {
            string requete = @"
                SELECT p.Id_Plat, p.Nom, p.Type, p.Recette, p.Prix
                FROM Plat p
                WHERE p.Id_Utilisateur = @IdUtilisateur";

            using (MySqlConnection connexion = new MySqlConnection(connectionString))
            {
                try
                {
                    connexion.Open();
                    MySqlCommand commande = new MySqlCommand(requete, connexion);
                    commande.Parameters.AddWithValue("@IdUtilisateur", identifiantUtilisateur);

                    MySqlDataReader lecteur = commande.ExecuteReader();

                    Console.WriteLine("\nPlats que vous avez ajoutés :");

                    while (lecteur.Read())
                    {
                        int idPlat = (int)lecteur["Id_Plat"];
                        string nomPlat = lecteur["Nom"].ToString();
                        string recette = lecteur["Recette"].ToString();
                        decimal prix = (decimal)lecteur["Prix"];

                        Console.OutputEncoding = System.Text.Encoding.UTF8;
                        Console.WriteLine($"ID: {idPlat} - Plat: {nomPlat} - Recette: {recette} - Prix: {prix} €");
                    }

                    lecteur.Close();
                }
                catch (Exception erreur)
                {
                    Console.WriteLine("Erreur lors de la récupération des plats du cuisinier : " + erreur.Message);
                }
            }
        }

        /// <summary>
        /// Récupère la liste des nationalités disponibles dans la base de données.
        /// </summary>
        public List<string> ObtenirNationalitesDisponibles()
        {
            List<string> nationalites = new List<string>();

            using (MySqlConnection connexion = new MySqlConnection(connectionString))
            {
                try
                {
                    connexion.Open();
                    string requete = "SELECT DISTINCT Nationalite FROM Plat";
                    MySqlCommand commande = new MySqlCommand(requete, connexion);
                    MySqlDataReader lecteur = commande.ExecuteReader();

                    while (lecteur.Read())
                    {
                        string nationalite = lecteur["Nationalite"].ToString();
                        nationalites.Add(nationalite);
                    }

                    lecteur.Close();
                }
                catch (Exception erreur)
                {
                    Console.WriteLine("Erreur lors de la récupération des nationalités : " + erreur.Message);
                }
            }

            return nationalites;
        }
    }
}
