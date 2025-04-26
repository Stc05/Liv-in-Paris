using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using GraphX.Controls;
using GraphX.Logic.Models;
using QuickGraph;
using System.IO;
using SkiaSharp;
using Liv_in_Paris;


namespace Liv_in_Paris
{
    public class CommandeService
    {
        private string connectionString = "SERVER=127.0.0.1;PORT=3306;DATABASE=LivInParis;UID=root;PASSWORD=Poulet69!270705;" + "CharSet=utf8mb4;";
        private Graph graphe;

        public CommandeService()
        {
            graphe = new Graph("metro.csv");
        }

        /// <summary>
        /// Passe une commande avec plusieurs plats et quantités.
        /// </summary>
        public void PasserCommande(int[] platIds, int[] quantites)
        {
            int idUtilisateur = AuthService.IdentifiantUtilisateurActuel;
            if (idUtilisateur == -1)
            {
                Console.WriteLine("Utilisateur non trouvé.");
                return;
            }

            Console.Clear();
            Console.WriteLine("=== Commande de plats ===\n");

            string stationClient = new AuthService().ObtenirStationProche(idUtilisateur);
            string[] stations = new string[platIds.Length];
            string[] dates = new string[platIds.Length];

            for (int i = 0; i < platIds.Length; i++)
            {
                Console.Clear();
                string platNom = ObtenirNomPlatParId(platIds[i]);
                Console.WriteLine($"=== Plat: {platNom} ===\n");

                Console.WriteLine($"Souhaitez-vous être livré à votre station habituelle pour le plat {platNom} ? (oui/non)");
                string rep = Console.ReadLine().ToLower();
                if (rep == "oui")
                {
                    stations[i] = stationClient;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Liste des stations disponibles :");
                    foreach (var noeud in graphe.Noeuds.Values)
                    {
                        Console.WriteLine("ID: " + noeud.Id + " - Station: " + noeud.Nom);
                    }

                    Console.WriteLine("\nEntrez l'ID de la station de livraison pour ce plat :");
                    stations[i] = Console.ReadLine();
                }

                Console.WriteLine($"\nEntrez la date de livraison pour le plat {platNom} (YYYY-MM-DD) :");
                string dateLivraison = Console.ReadLine();

                while (!DateTime.TryParseExact(dateLivraison, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    Console.WriteLine("Format de date invalide. Veuillez entrer la date sous le format 'YYYY-MM-DD' :");
                    dateLivraison = Console.ReadLine();
                }

                dates[i] = dateLivraison;
            }

            decimal prixTotal = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var cmdCommande = new MySqlCommand("INSERT INTO Commande (Prix_Total, Payé, Id_Utilisateur, Date_Commande) VALUES (@PrixTotal, @Paye, @IdUtilisateur, @DateCommande)", connection);
                cmdCommande.Parameters.AddWithValue("@PrixTotal", 0);
                cmdCommande.Parameters.AddWithValue("@Paye", false);
                cmdCommande.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);
                cmdCommande.Parameters.AddWithValue("@DateCommande", DateTime.Now.Date);
                cmdCommande.ExecuteNonQuery();

                cmdCommande.CommandText = "SELECT LAST_INSERT_ID();";
                int idCommande = Convert.ToInt32(cmdCommande.ExecuteScalar());
                Console.Clear();
                for (int i = 0; i < platIds.Length; i++)
                {
                    var prixCmd = new MySqlCommand("SELECT Prix FROM Plat WHERE Id_Plat = @IdPlat", connection);
                    prixCmd.Parameters.AddWithValue("@IdPlat", platIds[i]);
                    decimal prix = (decimal)prixCmd.ExecuteScalar();
                    prixTotal += prix * quantites[i];

                    var stationCuisinierCmd = new MySqlCommand("SELECT u.Station_Proche FROM Utilisateur u JOIN Plat p ON u.Id_Utilisateur = p.Id_Utilisateur WHERE p.Id_Plat = @IdPlat", connection);
                    stationCuisinierCmd.Parameters.AddWithValue("@IdPlat", platIds[i]);
                    string stationCuisinier = stationCuisinierCmd.ExecuteScalar().ToString();

                    Noeud noeudCuisinier = null;
                    foreach (var noeud in graphe.Noeuds.Values)
                    {
                        if (noeud.Id.ToString() == stationCuisinier)
                        {
                            noeudCuisinier = noeud;
                            break;
                        }
                    }

                    Noeud noeudLivraison = null;
                    foreach (var noeud in graphe.Noeuds.Values)
                    {
                        if (noeud.Id.ToString() == stations[i])
                        {
                            noeudLivraison = noeud;
                            break;
                        }
                    }

                    Console.WriteLine($"\n=== Trajet entre les stations pour plat {i + 1} ===\n");
                    Console.WriteLine("Station du cuisinier : " + noeudCuisinier?.Nom + ", Station de livraison : " + noeudLivraison?.Nom);
                    List<Noeud> chemin = graphe.Dijkstra(noeudCuisinier, noeudLivraison);

                    graphe.AfficherChemin(chemin);
                    string titre = $"Trajet Plat {i + 1}";
                    graphe.VisualiserCheminSurCarte(graphe, chemin, titre);

                    double tempsTrajet = graphe.Temps_Plus_Court_Chemin(chemin);
                    Console.WriteLine("Durée du trajet le plus court : " + tempsTrajet + " minutes\n");

                    string query = "INSERT INTO Ligne_de_commande (Quantité, Date_Livraison, Station_Livraison, Prix, Id_Commande, Id_Plat) VALUES (@q, @d, @s, @p, @idCmd, @idPlat)";
                    var cmdLigne = new MySqlCommand(query, connection);
                    cmdLigne.Parameters.AddWithValue("@q", quantites[i]);
                    cmdLigne.Parameters.AddWithValue("@d", dates[i]);
                    cmdLigne.Parameters.AddWithValue("@s", stations[i]);
                    cmdLigne.Parameters.AddWithValue("@p", prix);
                    cmdLigne.Parameters.AddWithValue("@idCmd", idCommande);
                    cmdLigne.Parameters.AddWithValue("@idPlat", platIds[i]);
                    cmdLigne.ExecuteNonQuery();
                    Thread.Sleep(2000);
                }

                var update = new MySqlCommand("UPDATE Commande SET Prix_Total = @pt WHERE Id_Commande = @id", connection);
                update.Parameters.AddWithValue("@pt", prixTotal);
                update.Parameters.AddWithValue("@id", idCommande);
                update.ExecuteNonQuery();

                Console.WriteLine($"Commande enregistrée. Prix total : {prixTotal} EUR\n");
            }
        }



        /// <summary>
        /// Récupère le nom d’un plat à partir de son identifiant.
        /// </summary>
        public string ObtenirNomPlatParId(int idPlat)
        {
            string nom = "";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new MySqlCommand("SELECT Nom FROM Plat WHERE Id_Plat = @Id", connection);
                cmd.Parameters.AddWithValue("@Id", idPlat);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    nom = result.ToString();
                }
            }
            return nom;
        }
    }
}
