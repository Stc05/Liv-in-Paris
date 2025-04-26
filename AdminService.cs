using MySql.Data.MySqlClient;
using System;

namespace Liv_in_Paris
{
    public class AdminService
    {
        private string connectionString = "SERVER=127.0.0.1;PORT=3306;" +
                                          "DATABASE=LivInParis;" +
                                          "UID=root;PASSWORD=Poulet69!270705" + "CharSet=utf8mb4;";

        /// <summary>
        /// Supprime un utilisateur de la base ainsi que ses liens dans les tables Client et Cuisinier.
        /// </summary>
        public void SupprimerUtilisateur()
        {
            Console.Write("Entrez l'ID de l'utilisateur à supprimer : ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    var cmdCuisinier = new MySqlCommand("DELETE FROM Cuisinier WHERE Id_Utilisateur = @Id", conn);
                    cmdCuisinier.Parameters.AddWithValue("@Id", id);
                    cmdCuisinier.ExecuteNonQuery();

                    var cmdClient = new MySqlCommand("DELETE FROM Client WHERE Id_Utilisateur = @Id", conn);
                    cmdClient.Parameters.AddWithValue("@Id", id);
                    cmdClient.ExecuteNonQuery();

                    var cmdUser = new MySqlCommand("DELETE FROM Utilisateur WHERE Id_Utilisateur = @Id", conn);
                    cmdUser.Parameters.AddWithValue("@Id", id);
                    int result = cmdUser.ExecuteNonQuery();

                    Console.WriteLine(result > 0 ? "Utilisateur supprimé." : "Aucun utilisateur supprimé.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Modifie les informations d’un utilisateur.
        /// </summary>
        public void ModifierUtilisateur()
        {
            Console.Write("Entrez l'ID de l'utilisateur à modifier : ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            Console.Write("Nouveau nom : ");
            string nom = Console.ReadLine();
            Console.Write("Nouveau prénom : ");
            string prenom = Console.ReadLine();
            Console.Write("Nouvelle adresse : ");
            string adresse = Console.ReadLine();
            Console.Write("Nouvelle station proche : ");
            string station = Console.ReadLine();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"UPDATE Utilisateur 
                                     SET Nom = @Nom, Prénom = @Prenom, Adresse = @Adresse, Station_Proche = @Station 
                                     WHERE Id_Utilisateur = @Id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nom", nom);
                    cmd.Parameters.AddWithValue("@Prenom", prenom);
                    cmd.Parameters.AddWithValue("@Adresse", adresse);
                    cmd.Parameters.AddWithValue("@Station", station);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int result = cmd.ExecuteNonQuery();
                    Console.WriteLine(result > 0 ? "Informations mises à jour." : "Aucun changement.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Affiche tous les clients triés par nom (ordre alphabétique), en filtrant les rôles.
        /// </summary>
        public void AfficherClientsOrdreAlphabetique()
        {
            string query = @"SELECT Nom, Prénom, Adresse FROM Utilisateur u 
                             JOIN Client c ON u.Id_Utilisateur = c.Id_Utilisateur 
                             WHERE u.Role = 'Client'
                             ORDER BY Nom ASC";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                Console.WriteLine("\nClients par ordre alphabétique :");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Nom"]} {reader["Prénom"]} - {reader["Adresse"]}");
                }
                reader.Close();
            }
        }

        /// <summary>
        /// Affiche les clients triés par rue (adresse), en filtrant les rôles.
        /// </summary>
        public void AfficherClientsParRue()
        {
            string query = @"SELECT Nom, Prénom, Adresse FROM Utilisateur u 
                             JOIN Client c ON u.Id_Utilisateur = c.Id_Utilisateur 
                             WHERE u.Role = 'Client'
                             ORDER BY Adresse";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                Console.WriteLine("\nClients par rue :");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Nom"]} {reader["Prénom"]} - {reader["Adresse"]}");
                }
                reader.Close();
            }
        }

        /// <summary>
        /// Affiche les clients triés selon le montant total dépensé, en filtrant les rôles.
        /// </summary>
        public void AfficherClientsParMontantTotal()
        {
            string query = @"
                SELECT u.Nom, u.Prénom, u.Adresse, SUM(co.Prix_Total) AS Total
                FROM Utilisateur u
                JOIN Client c ON u.Id_Utilisateur = c.Id_Utilisateur
                JOIN Commande co ON co.Id_Utilisateur = u.Id_Utilisateur
                WHERE u.Role = 'Client'
                GROUP BY u.Id_Utilisateur
                ORDER BY Total DESC";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                Console.WriteLine("\nClients par montant total dépensé :");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Nom"]} {reader["Prénom"]} - {reader["Adresse"]} - Total : {reader["Total"]} EUR");
                }
                reader.Close();
            }
        }

        /// <summary>
        /// Affiche le nombre total de livraisons réalisées par chaque cuisinier.
        /// </summary>
        public void AfficherLivraisonsParCuisinier()
        {
            string query = @"
        SELECT u.Nom, u.Prénom, COUNT(ldc.Id_Ligne_de_commande) AS Livraisons
        FROM Ligne_de_commande ldc
        JOIN Plat p ON ldc.Id_Plat = p.Id_Plat
        JOIN Cuisinier c ON p.Id_Utilisateur = c.Id_Utilisateur
        JOIN Utilisateur u ON c.Id_Utilisateur = u.Id_Utilisateur
        GROUP BY u.Id_Utilisateur";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                Console.WriteLine("\nNombre de livraisons par cuisinier :");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Nom"]} {reader["Prénom"]} - {reader["Livraisons"]} livraisons");
                }
                reader.Close();
            }
        }

        /// <summary>
        /// Affiche toutes les commandes passées entre deux dates choisies par l’administrateur.
        /// </summary>
        public void AfficherCommandesParPeriode()
        {
            Console.Write("Date début (AAAA-MM-JJ) : ");
            string debut = Console.ReadLine();
            Console.Write("Date fin (AAAA-MM-JJ) : ");
            string fin = Console.ReadLine();

            string query = @"SELECT * FROM Commande 
                     WHERE Date_Commande BETWEEN @Debut AND @Fin";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Debut", debut);
                cmd.Parameters.AddWithValue("@Fin", fin);

                var reader = cmd.ExecuteReader();
                Console.WriteLine("\nCommandes pendant la période :");
                while (reader.Read())
                {
                    Console.WriteLine($"Commande #{reader["Id_Commande"]} - {reader["Prix_Total"]}€ - {reader["Date_Commande"]}");
                }
                reader.Close();
            }
        }

        /// <summary>
        /// Affiche le prix moyen de toutes les commandes enregistrées dans la base.
        /// </summary>
        public void AfficherMoyennePrixCommandes()
        {
            string query = "SELECT AVG(Prix_Total) AS Moyenne FROM Commande";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                var moyenne = cmd.ExecuteScalar();
                Console.WriteLine($"\nPrix moyen des commandes : {moyenne} EUR");
            }
        }

        /// <summary>
        /// Affiche les commandes d’un client spécifique filtrées par date et éventuellement par nationalité.
        /// </summary>
        public void AfficherCommandesClientFiltres()
        {
            Console.Write("ID du client : ");
            if (!int.TryParse(Console.ReadLine(), out int idClient)) return;

            Console.Write("Date début (AAAA-MM-JJ) : ");
            string debut = Console.ReadLine();
            Console.Write("Date fin (AAAA-MM-JJ) : ");
            string fin = Console.ReadLine();
            Console.Write("Nationalité du plat (laisser vide pour ignorer) : ");
            string nationalite = Console.ReadLine();

            string query = @"
        SELECT c.Id_Commande, c.Date_Commande, p.Nom, p.Nationalite, ldc.Prix
        FROM Commande c
        JOIN Ligne_de_commande ldc ON c.Id_Commande = ldc.Id_Commande
        JOIN Plat p ON ldc.Id_Plat = p.Id_Plat
        WHERE c.Id_Utilisateur = @ClientId 
        AND c.Date_Commande BETWEEN @Debut AND @Fin";

            if (!string.IsNullOrWhiteSpace(nationalite))
                query += " AND p.Nationalite = @Nationalite";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientId", idClient);
                cmd.Parameters.AddWithValue("@Debut", debut);
                cmd.Parameters.AddWithValue("@Fin", fin);
                if (!string.IsNullOrWhiteSpace(nationalite))
                    cmd.Parameters.AddWithValue("@Nationalite", nationalite);

                var reader = cmd.ExecuteReader();
                Console.WriteLine("\nCommandes du client :");
                while (reader.Read())
                {
                    Console.WriteLine($"Commande #{reader["Id_Commande"]} - {reader["Date_Commande"]} - {reader["Nom"]} ({reader["Nationalite"]}) - {reader["Prix"]} EUR");
                }
                reader.Close();
            }
        }

    }
}

