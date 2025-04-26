using Liv_in_Paris;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Liv_in_Paris
{
    class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// Initialise les services et affiche le menu d'accueil.
        /// </summary>
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            AuthService authService = new AuthService();
            CuisinierService cuisinierService = new CuisinierService();
            PlatService platService = new PlatService();
            CommandeService commandeService = new CommandeService();
            AdminService adminService = new AdminService();

            Console.WriteLine("Bienvenue dans LivInParis !");
            Console.WriteLine("1. Inscription");
            Console.WriteLine("2. Connexion");
            Console.Write("Votre choix : ");
            string choix = Console.ReadLine();

            if (choix == "1")
                Inscription(authService, cuisinierService, platService, commandeService);
            else if (choix == "2")
                Connexion(authService, cuisinierService, platService, commandeService, adminService);
            else
                Console.WriteLine("Choix invalide.");
        }

        /// <summary>
        /// Gère le processus d'inscription d'un nouvel utilisateur,
        /// avec la possibilité de devenir cuisinier.
        /// </summary>
        static void Inscription(AuthService authService, CuisinierService cuisinierService, PlatService platService, CommandeService commandeService)
        {
            Graph graphe = new Graph("metro.csv");

            Console.Clear();
            Console.WriteLine("=== INSCRIPTION ===");

            Console.Write("Nom : ");
            string nom = Console.ReadLine();
            Console.Write("Prénom : ");
            string prénom = Console.ReadLine();
            Console.Write("Email : ");
            string email = Console.ReadLine();
            Console.Write("Mot de passe : ");
            string motDePasse = Console.ReadLine();
            Console.Write("Adresse : ");
            string adresse = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Stations disponibles :");
            foreach (var noeud in graphe.Noeuds.Values)
                Console.WriteLine($"ID: {noeud.Id} - {noeud.Nom}");

            Console.Write("\nID de la station proche : ");
            string stationProche = Console.ReadLine();

            Console.Write("Souhaitez-vous être cuisinier ? (oui/non) : ");
            bool estCuisinier = Console.ReadLine().ToLower() == "oui";

            string specialite = null;
            if (estCuisinier)
            {
                var specialites = platService.ObtenirNationalitesDisponibles();
                Console.WriteLine("Spécialités existantes :");
                for (int i = 0; i < specialites.Count; i++)
                    Console.WriteLine($"{i + 1}. {specialites[i]}");
                Console.WriteLine($"{specialites.Count + 1}. Autre");

                Console.Write("Votre choix : ");
                int index = int.Parse(Console.ReadLine());
                if (index <= specialites.Count)
                    specialite = specialites[index - 1];
                else
                {
                    Console.Write("Entrez la nouvelle spécialité : ");
                    specialite = Console.ReadLine();
                }
            }

            bool inscription = authService.InscrireUtilisateur(nom, prénom, email, motDePasse, adresse, stationProche, estCuisinier, specialite);
            if (inscription)
            {
                Console.Clear();
                Console.WriteLine("Inscription réussie !");
                if (estCuisinier)
                    MenuPostInscription(cuisinierService, platService, commandeService);
                else
                    MenuClient(commandeService, platService);
            }
            else Console.WriteLine("Inscription échouée.");
        }

        /// <summary>
        /// Gère le processus de connexion.
        /// Redirige vers l’espace client, cuisinier ou administrateur en fonction du rôle.
        /// </summary>
        static void Connexion(AuthService authService, CuisinierService cuisinierService, PlatService platService, CommandeService commandeService, AdminService adminService)
        {
            Console.Clear();
            Console.WriteLine("=== CONNEXION ===");
            Console.WriteLine("(ID Admin : admin@livinparis.fr, MDP ADMIN : admin123)\n");
            Console.Write("Email : ");
            string email = Console.ReadLine();
            Console.Write("Mot de passe : ");
            string motDePasse = Console.ReadLine();

            bool connecte = authService.ConnecterUtilisateur(email, motDePasse);
            if (!connecte)
            {
                Console.WriteLine("Identifiants incorrects.");
                return;
            }

            Console.Clear();
            Console.WriteLine("Connexion réussie.");

            if (AuthService.RoleUtilisateurActuel == "Admin")
                MenuAdministrateur(adminService);
            else if (authService.EstCuisinier(email))
            {
                Console.WriteLine("1. Accéder à l’espace client");
                Console.WriteLine("2. Accéder à l’espace cuisinier");
                string choix = Console.ReadLine();
                if (choix == "1") MenuClient(commandeService, platService);
                else if (choix == "2") MenuCuisinier(cuisinierService, platService);
                else Console.WriteLine("Choix invalide.");
            }
            else
            {
                MenuClient(commandeService, platService);
            }
        }

        /// <summary>
        /// Menu affiché juste après une inscription.
        /// Permet à un cuisinier de choisir entre l’espace client et cuisinier.
        /// </summary>
        static void MenuPostInscription(CuisinierService cuisinierService, PlatService platService, CommandeService commandeService)
        {
            Console.Clear();
            Console.WriteLine("1. Espace client");
            Console.WriteLine("2. Espace cuisinier");
            string choix = Console.ReadLine();

            if (choix == "1") MenuClient(commandeService, platService);
            else if (choix == "2") MenuCuisinier(cuisinierService, platService);
            else Console.WriteLine("Choix invalide.");
        }

        /// <summary>
        /// Espace interactif destiné aux clients.
        /// Permet de filtrer les plats ou de passer une commande.
        /// </summary>
        static void MenuClient(CommandeService commandeService, PlatService platService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ESPACE CLIENT ===\n");
                Console.WriteLine("1. Filtrer les plats disponibles");
                Console.WriteLine("2. Passer une commande");
                Console.WriteLine("3. Se déconnecter");
                Console.Write("Choix : ");
                string choix = Console.ReadLine();

                if (choix == "1")
                {
                    Console.Clear();
                    var nationalites = platService.ObtenirNationalitesDisponibles();
                    Console.WriteLine("Choisissez une nationalité :");
                    for (int i = 0; i < nationalites.Count; i++)
                        Console.WriteLine($"{i + 1}. {nationalites[i]}");
                    Console.Write("Votre choix : ");
                    string nat = nationalites[int.Parse(Console.ReadLine()) - 1];

                    Console.Write("\nType de plat (entrée, plat principal, dessert) : ");
                    string type = Console.ReadLine().ToLower();
                    while (type != "entrée" && type != "plat principal" && type != "dessert")
                    {
                        Console.Write("Type invalide. Réessayez : ");
                        type = Console.ReadLine().ToLower();
                    }

                    Console.Clear();
                    platService.AfficherPlatsFiltres(nat, type);
                    Console.WriteLine("\nAppuyez sur une touche pour revenir...");
                    Console.ReadKey();
                }
                else if (choix == "2")
                {
                    Console.Clear();
                    Console.WriteLine("=== QUANTITES ===\n");
                    Console.Write("Nombre de plats différents à commander : ");
                    int n = int.Parse(Console.ReadLine());

                    int[] ids = new int[n];
                    int[] qtes = new int[n];
                    Console.Clear();

                    for (int i = 0; i < n; i++)
                    {
                        Console.WriteLine("=== FILTRES ===\n");
                        Console.WriteLine($"Entrer le numéro correspondant à la nationalité souhaitée pour le plat {i + 1} :");
                        var nationalites = platService.ObtenirNationalitesDisponibles();
                        for (int j = 0; j < nationalites.Count; j++)
                            Console.WriteLine($"{j + 1}. {nationalites[j]}");
                        string nat = nationalites[int.Parse(Console.ReadLine()) - 1];

                        Console.Write("\nType de plat (entrée, plat principal, dessert): ");
                        string type = Console.ReadLine().ToLower();
                        while (type != "entrée" && type != "plat principal" && type != "dessert")
                        {
                            Console.Write("Type invalide. Réessayez : ");
                            type = Console.ReadLine().ToLower();
                        }

                        Console.Clear();
                        platService.AfficherPlatsFiltres(nat, type);
                        Console.Write("\nID du plat : ");
                        ids[i] = int.Parse(Console.ReadLine());
                        Console.Write("Pour combien de personnes : ");
                        qtes[i] = int.Parse(Console.ReadLine());
                        Console.Clear();
                    }

                    commandeService.PasserCommande(ids, qtes);
                    Console.WriteLine("\nCommande enregistrée. Appuyez sur une touche pour revenir.");
                    Console.ReadKey();
                }
                else if (choix == "3") break;
                else
                {
                    Console.WriteLine("Choix invalide.");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Espace interactif pour les cuisiniers.
        /// Permet de consulter et d’ajouter des plats.
        /// </summary>
        static void MenuCuisinier(CuisinierService cuisinierService, PlatService platService)
        {
            int idUtilisateur = AuthService.IdentifiantUtilisateurActuel;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ESPACE CUISINIER ===");
                Console.WriteLine("1. Voir mes plats");
                Console.WriteLine("2. Ajouter un plat");
                Console.WriteLine("3. Se déconnecter");
                Console.Write("Choix : ");
                string choix = Console.ReadLine();

                if (choix == "1") platService.AfficherPlatsParCuisinier(idUtilisateur);
                else if (choix == "2") cuisinierService.AjouterPlat(idUtilisateur);
                else if (choix == "3") break;
                else Console.WriteLine("Choix invalide.");
                Console.WriteLine("\nAppuyez sur une touche pour continuer.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Interface de gestion pour les administrateurs.
        /// Accès aux opérations de gestion et de statistiques sur les utilisateurs et commandes.
        /// </summary>
        static void MenuAdministrateur(AdminService adminService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ESPACE ADMINISTRATEUR ===");
                Console.WriteLine("1. Supprimer un utilisateur");
                Console.WriteLine("2. Modifier un utilisateur");
                Console.WriteLine("3. Clients par ordre alphabétique");
                Console.WriteLine("4. Clients par rue");
                Console.WriteLine("5. Clients par montant total");
                Console.WriteLine("6. Livraisons par cuisinier");
                Console.WriteLine("7. Commandes dans une période");
                Console.WriteLine("8. Moyenne prix des commandes");
                Console.WriteLine("9. Commandes d’un client filtrées");
                Console.WriteLine("0. Quitter");
                Console.Write("Choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1": adminService.SupprimerUtilisateur(); break;
                    case "2": adminService.ModifierUtilisateur(); break;
                    case "3": adminService.AfficherClientsOrdreAlphabetique(); break;
                    case "4": adminService.AfficherClientsParRue(); break;
                    case "5": adminService.AfficherClientsParMontantTotal(); break;
                    case "6": adminService.AfficherLivraisonsParCuisinier(); break;
                    case "7": adminService.AfficherCommandesParPeriode(); break;
                    case "8": adminService.AfficherMoyennePrixCommandes(); break;
                    case "9": adminService.AfficherCommandesClientFiltres(); break;
                    case "0": return;
                    default: Console.WriteLine("Choix invalide."); break;
                }

                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();

            }
        }
    }
}
