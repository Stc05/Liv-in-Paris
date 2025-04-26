# Liv'in Paris

Liv'in Paris est une application C# qui facilite le partage de repas faits maison entre voisins à Paris. Le projet met en avant la programmation orientée objet, la gestion de bases de données SQL, et l'exploitation d'algorithmes de graphes pour optimiser les trajets de livraison.

## Fonctionnalités principales

- **Partage de repas entre particuliers** : met en relation des cuisiniers amateurs et des clients dans Paris.
- **Gestion complète des utilisateurs** : inscription, rôles (client, cuisinier, admin), authentification sécurisée.
- **Base de données SQL** : stockage des informations des utilisateurs, plats, commandes et trajets.
- **Trajets optimisés en métro** : exploitation des données du métro parisien pour livrer de manière efficace grâce à des algorithmes de graphes (Dijkstra, Bellman-Ford, Floyd-Warshall).
- **Visualisation de parcours** : représentation graphique des trajets optimaux sur une carte.

## Structure du projet / Description des classes principales

### Modélisation du métro et des trajets

- **`Noeud`** : représente une station de métro avec toutes ses informations (id, nom, ligne, coordonnées géographiques, commune, code postal, sens de circulation).
- **`Lien`** : représente un lien (arc) entre deux stations de métro, avec la durée du trajet entre elles.
- **`Graph`** : construit le graphe complet du métro à partir de fichiers CSV, crée la matrice d’adjacence, gère les stations, les liens et implémente les algorithmes de chemin optimal (Dijkstra, Bellman-Ford, Floyd-Warshall), permet la visualisation graphique des trajets sur une carte.

### Gestion des utilisateurs et rôles

- **`AuthService`** : gère l’inscription, la connexion, et la vérification du rôle de l’utilisateur (client, cuisinier, administrateur)&#8203;:contentReference[oaicite:3]{index=3}.
- **`RoleService`** : permet à l'utilisateur de choisir son rôle lors de l'inscription ou de la connexion (client ou cuisinier)&#8203;:contentReference[oaicite:4]{index=4}.
- **`AdminService`** : fonctionnalités avancées de gestion pour l’administrateur : suppression et modification d'utilisateurs, statistiques sur clients et commandes, gestion des livraisons, etc.&#8203;:contentReference[oaicite:5]{index=5}

### Gestion des plats et des commandes

- **`PlatService`** : permet aux cuisiniers d’ajouter des plats et aux clients de filtrer et visualiser les plats disponibles selon différents critères (nationalité, type, station proche)&#8203;:contentReference[oaicite:6]{index=6}.
- **`CommandeService`** : gère la passation des commandes, calcule les trajets de livraison entre cuisinier et client, utilise les algorithmes de graphe pour optimiser les parcours, et enregistre le détail de chaque livraison (plat, quantité, station, date, etc.)&#8203;:contentReference[oaicite:7]{index=7}.
- **`CuisinierService`** : fonctionnalités spécifiques pour les cuisiniers (ajout de plats, gestion des spécialités, etc.)&#8203;:contentReference[oaicite:8]{index=8}.

###Gestion de l’application

- **`Program.cs`** : point d’entrée principal de l’application, gestion de l’interface console, interaction utilisateur (inscription, connexion, menus spécifiques selon le rôle)&#8203;:contentReference[oaicite:9]{index=9}.

