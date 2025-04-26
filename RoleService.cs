using MySql.Data.MySqlClient;
using System;

namespace Liv_in_Paris
{
    public class RoleService
    {
        /// <summary>
        /// Permet à l'utilisateur de choisir un rôle parmi les options disponibles.
        /// </summary>
        /// <returns>Le rôle choisi sous forme de chaîne ("Client" ou "Cuisinier"). Retourne null si choix invalide.</returns>
        public string ChoisirRole()
        {
            Console.WriteLine("Choisissez votre rôle :");
            Console.WriteLine("1 - Client");
            Console.WriteLine("2 - Cuisinier");

            string choix = Console.ReadLine();

            if (choix == "1")
            {
                return "Client";
            }
            else if (choix == "2")
            {
                return "Cuisinier";
            }
            else
            {
                Console.WriteLine("Choix invalide.");
                return null;
            }
        }
    }
}
