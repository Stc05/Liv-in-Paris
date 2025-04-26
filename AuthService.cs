using MySql.Data.MySqlClient;
using System;

namespace Liv_in_Paris
{
    public class AuthService
    {

        private string connectionString = "SERVER=127.0.0.1;PORT=3306;DATABASE=LivInParis;UID=root;PASSWORD=Poulet69!270705;" + "CharSet=utf8mb4;";

        public static int IdentifiantUtilisateurActuel { get; private set; }
        public static string RoleUtilisateurActuel { get; private set; }

        /// <summary>
        /// Inscrit un nouvel utilisateur, en l'ajoutant comme client et éventuellement cuisinier.
        /// </summary>
        public bool InscrireUtilisateur(string nom, string prénom, string email, string motDePasse, string adresse, string stationProche, bool estCuisinier, string specialite)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string requete = "INSERT INTO Utilisateur (Nom, Prénom, Email, Mot_de_passe, Adresse, Station_Proche) " +
                                     "VALUES (@Nom, @Prénom, @Email, @MotDePasse, @Adresse, @StationProche)";

                    MySqlCommand commande = new MySqlCommand(requete, connection);
                    commande.Parameters.AddWithValue("@Nom", nom);
                    commande.Parameters.AddWithValue("@Prénom", prénom);
                    commande.Parameters.AddWithValue("@Email", email);
                    commande.Parameters.AddWithValue("@MotDePasse", motDePasse);
                    commande.Parameters.AddWithValue("@Adresse", adresse);
                    commande.Parameters.AddWithValue("@StationProche", stationProche);

                    int resultat = commande.ExecuteNonQuery();

                    if (resultat > 0)
                    {
                        string requeteId = "SELECT Id_Utilisateur FROM Utilisateur WHERE Email = @Email";
                        MySqlCommand commandeId = new MySqlCommand(requeteId, connection);
                        commandeId.Parameters.AddWithValue("@Email", email);
                        IdentifiantUtilisateurActuel = Convert.ToInt32(commandeId.ExecuteScalar());

                        string requeteClient = "INSERT INTO Client (Id_Utilisateur, Particulier) VALUES (@Id, @Particulier)";
                        MySqlCommand commandeClient = new MySqlCommand(requeteClient, connection);
                        commandeClient.Parameters.AddWithValue("@Id", IdentifiantUtilisateurActuel);
                        commandeClient.Parameters.AddWithValue("@Particulier", true);
                        commandeClient.ExecuteNonQuery();

                        if (estCuisinier && !string.IsNullOrEmpty(specialite))
                        {
                            string requeteCuisinier = "INSERT INTO Cuisinier (Id_Utilisateur, Spécialité) VALUES (@IdUtilisateur, @Specialite)";
                            MySqlCommand commandeCuisinier = new MySqlCommand(requeteCuisinier, connection);
                            commandeCuisinier.Parameters.AddWithValue("@IdUtilisateur", IdentifiantUtilisateurActuel);
                            commandeCuisinier.Parameters.AddWithValue("@Specialite", specialite);
                            commandeCuisinier.ExecuteNonQuery();
                        }

                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de l'inscription : " + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Vérifie les identifiants de l'utilisateur et connecte s'ils sont valides.
        /// </summary>
        public bool ConnecterUtilisateur(string email, string motDePasse)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string requete = "SELECT COUNT(*) FROM Utilisateur WHERE Email = @Email AND Mot_de_passe = @MotDePasse";
                    MySqlCommand commande = new MySqlCommand(requete, connection);
                    commande.Parameters.AddWithValue("@Email", email);
                    commande.Parameters.AddWithValue("@MotDePasse", motDePasse);

                    int nombre = Convert.ToInt32(commande.ExecuteScalar());

                    if (nombre > 0)
                    {
                        string requeteId = "SELECT Id_Utilisateur FROM Utilisateur WHERE Email = @Email";
                        MySqlCommand commandeId = new MySqlCommand(requeteId, connection);
                        commandeId.Parameters.AddWithValue("@Email", email);
                        IdentifiantUtilisateurActuel = Convert.ToInt32(commandeId.ExecuteScalar());

                        string requeteRole = "SELECT Role FROM Utilisateur WHERE Email = @Email";
                        MySqlCommand commandeRole = new MySqlCommand(requeteRole, connection);
                        commandeRole.Parameters.AddWithValue("@Email", email);
                        RoleUtilisateurActuel = commandeRole.ExecuteScalar().ToString();

                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Vérifie si l'utilisateur est également cuisinier.
        /// </summary>
        public bool EstCuisinier(string email)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string requete = "SELECT COUNT(*) FROM Cuisinier c JOIN Utilisateur u ON c.Id_Utilisateur = u.Id_Utilisateur WHERE u.Email = @Email";
                    MySqlCommand commande = new MySqlCommand(requete, connection);
                    commande.Parameters.AddWithValue("@Email", email);

                    int nombre = Convert.ToInt32(commande.ExecuteScalar());
                    return nombre > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la vérification du rôle : " + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Retourne l'identifiant de la station proche d'un utilisateur.
        /// </summary>
        public string ObtenirStationProche(int idUtilisateur)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string requete = "SELECT Station_Proche FROM Utilisateur WHERE Id_Utilisateur = @IdUtilisateur";
                    MySqlCommand commande = new MySqlCommand(requete, connection);
                    commande.Parameters.AddWithValue("@IdUtilisateur", idUtilisateur);
                    return commande.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la récupération de la station : " + ex.Message);
                    return null;
                }
            }
        }
    }
}
