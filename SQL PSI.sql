CREATE DATABASE  IF NOT EXISTS `livinparis` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `livinparis`;
-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: livinparis
-- ------------------------------------------------------
-- Server version	9.1.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `client`
--

DROP TABLE IF EXISTS `client`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `client` (
  `Id_Utilisateur` int NOT NULL,
  `Particulier` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id_Utilisateur`),
  CONSTRAINT `client_ibfk_1` FOREIGN KEY (`Id_Utilisateur`) REFERENCES `utilisateur` (`Id_Utilisateur`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `client`
--

LOCK TABLES `client` WRITE;
/*!40000 ALTER TABLE `client` DISABLE KEYS */;
INSERT INTO `client` VALUES (1,1),(2,1),(3,1),(4,0),(5,1),(6,0);
/*!40000 ALTER TABLE `client` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `commande`
--

DROP TABLE IF EXISTS `commande`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `commande` (
  `Id_Commande` int NOT NULL AUTO_INCREMENT,
  `Prix_Total` decimal(10,2) DEFAULT NULL,
  `Payé` tinyint(1) DEFAULT NULL,
  `Id_Utilisateur` int DEFAULT NULL,
  `Date_Commande` date DEFAULT NULL,
  PRIMARY KEY (`Id_Commande`),
  KEY `Id_Utilisateur` (`Id_Utilisateur`),
  CONSTRAINT `commande_ibfk_1` FOREIGN KEY (`Id_Utilisateur`) REFERENCES `client` (`Id_Utilisateur`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commande`
--

LOCK TABLES `commande` WRITE;
/*!40000 ALTER TABLE `commande` DISABLE KEYS */;
INSERT INTO `commande` VALUES (1,26.00,1,3,'2025-01-03'),(2,30.00,1,1,'2025-01-04'),(3,34.00,0,2,'2025-01-05'),(4,25.00,1,3,'2025-01-06'),(5,28.00,0,5,'2025-01-07');
/*!40000 ALTER TABLE `commande` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cuisinier`
--

DROP TABLE IF EXISTS `cuisinier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuisinier` (
  `Id_Utilisateur` int NOT NULL,
  `Spécialité` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id_Utilisateur`),
  CONSTRAINT `cuisinier_ibfk_1` FOREIGN KEY (`Id_Utilisateur`) REFERENCES `utilisateur` (`Id_Utilisateur`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuisinier`
--

LOCK TABLES `cuisinier` WRITE;
/*!40000 ALTER TABLE `cuisinier` DISABLE KEYS */;
INSERT INTO `cuisinier` VALUES (1,'Cuisine Française'),(2,'Cuisine Italienne'),(3,'Cuisine Végétarienne'),(4,'Cuisine Chinoise'),(5,'Cuisine Mexicaine');
/*!40000 ALTER TABLE `cuisinier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ligne_de_commande`
--

DROP TABLE IF EXISTS `ligne_de_commande`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ligne_de_commande` (
  `Id_Ligne_de_commande` int NOT NULL AUTO_INCREMENT,
  `Quantité` int DEFAULT NULL,
  `Date_Livraison` date DEFAULT NULL,
  `Station_Livraison` int DEFAULT NULL,
  `Prix` decimal(10,2) DEFAULT NULL,
  `Id_Commande` int DEFAULT NULL,
  `Id_Plat` int DEFAULT NULL,
  PRIMARY KEY (`Id_Ligne_de_commande`),
  KEY `Id_Commande` (`Id_Commande`),
  KEY `Id_Plat` (`Id_Plat`),
  CONSTRAINT `ligne_de_commande_ibfk_1` FOREIGN KEY (`Id_Commande`) REFERENCES `commande` (`Id_Commande`),
  CONSTRAINT `ligne_de_commande_ibfk_2` FOREIGN KEY (`Id_Plat`) REFERENCES `plat` (`Id_Plat`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ligne_de_commande`
--

LOCK TABLES `ligne_de_commande` WRITE;
/*!40000 ALTER TABLE `ligne_de_commande` DISABLE KEYS */;
INSERT INTO `ligne_de_commande` VALUES (1,2,'2025-01-05',1,13.00,1,2),(2,1,'2025-01-06',1,9.50,2,1),(3,1,'2025-01-06',1,13.00,2,2),(4,1,'2025-01-06',1,7.50,2,3),(5,1,'2025-01-07',2,5.50,3,4),(6,1,'2025-01-07',2,12.00,3,5),(7,1,'2025-01-07',2,10.50,3,7),(8,1,'2025-01-07',2,6.00,3,8),(9,2,'2025-01-08',3,6.50,4,6),(10,1,'2025-01-08',3,12.00,4,5),(11,1,'2025-01-09',5,5.00,5,9),(12,1,'2025-01-09',5,11.50,5,10),(13,1,'2025-01-09',5,11.50,5,10);
/*!40000 ALTER TABLE `ligne_de_commande` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plat`
--

DROP TABLE IF EXISTS `plat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plat` (
  `Id_Plat` int NOT NULL AUTO_INCREMENT,
  `Nom` varchar(255) DEFAULT NULL,
  `Type` enum('entrée','plat principal','dessert') DEFAULT NULL,
  `Date_Fabrication` date DEFAULT NULL,
  `Date_Péremption` date DEFAULT NULL,
  `Nationalite` varchar(100) DEFAULT NULL,
  `Ingrédients_Principaux` text,
  `Régime_Alimentaire` varchar(100) DEFAULT NULL,
  `Recette` text,
  `Déclinaison` varchar(100) DEFAULT NULL,
  `Id_Utilisateur` int DEFAULT NULL,
  `Prix` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`Id_Plat`),
  KEY `Id_Utilisateur` (`Id_Utilisateur`),
  CONSTRAINT `plat_ibfk_1` FOREIGN KEY (`Id_Utilisateur`) REFERENCES `cuisinier` (`Id_Utilisateur`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plat`
--

LOCK TABLES `plat` WRITE;
/*!40000 ALTER TABLE `plat` DISABLE KEYS */;
INSERT INTO `plat` VALUES (1,'Salade de chèvre chaud','entrée','2025-01-01','2025-01-05','Française','salade, chèvre, miel','Végétarien','Chauffer chèvre, dresser sur salade','chaud',1,9.50),(2,'Quiche Lorraine','plat principal','2025-01-01','2025-01-06','Française','lardons, crème, pâte brisée','Omnivore','Mélanger lardons, crème, cuire dans une pâte','classique',1,13.00),(3,'Tarte Tatin','dessert','2025-01-01','2025-01-07','Française','pommes, pâte feuilletée, sucre','Végétarien','Cuire pommes, ajouter pâte feuilletée, retourner','classique',1,7.50),(4,'Crème brûlée','dessert','2025-01-01','2025-01-07','Française','crème, sucre, vanille','Sans gluten','Chauffer crème, caraméliser le sucre','classique',1,8.00),(5,'Bruschetta','entrée','2025-01-01','2025-01-05','Italienne','tomates, pain grillé, ail, basilic','Végétarien','Mélanger tomates et basilic, servir sur pain','classique',2,5.50),(6,'Spaghetti Bolognese','plat principal','2025-01-01','2025-01-06','Italienne','pâtes, sauce tomate, viande','Omnivore','Cuire viande, ajouter sauce','classique',2,12.00),(7,'Lasagne','plat principal','2025-01-01','2025-01-06','Italienne','pâtes, sauce tomate, viande, béchamel','Omnivore','Monter la lasagne, cuire','classique',2,15.00),(8,'Tiramisu','dessert','2025-01-01','2025-01-07','Italienne','mascarpone, café, biscuit','Sans gluten','Monter crème, alterner couches','classique',2,6.50),(9,'Salade composée','entrée','2025-01-01','2025-01-05','Végétarienne','salade, tomates, concombre, avocat','Végétarien','Mélanger ingrédients, assaisonner','classique',3,7.00),(10,'Soupe de légumes','entrée','2025-01-01','2025-01-05','Végétarienne','carottes, courgettes, oignons, pommes de terre','Végétarien','Cuire légumes, mixer','classique',3,6.50),(11,'Risotto aux champignons','plat principal','2025-01-01','2025-01-06','Végétarienne','riz, champignons, bouillon','Végétarien','Cuire riz, ajouter champignons et bouillon','classique',3,12.00),(12,'Tarte aux légumes','plat principal','2025-01-01','2025-01-06','Végétarienne','pâte brisée, légumes','Végétarien','Cuire légumes, monter la tarte','classique',3,11.00),(13,'Raviolis chinois','entrée','2025-01-01','2025-01-05','Chinoise','farce de porc, pâte','Omnivore','Préparer la farce, envelopper et cuire','classique',4,7.00),(14,'Poulet au citron','plat principal','2025-01-01','2025-01-06','Chinoise','poulet, citron, sauce soja','Omnivore','Cuire poulet, ajouter sauce citron','classique',4,10.50),(15,'Soupe aigre-piquante','entrée','2025-01-01','2025-01-05','Chinoise','bouillon, tofu, légumes','Végétarien','Faire bouillir les légumes et tofu','classique',4,6.00),(16,'Canard laqué','plat principal','2025-01-01','2025-01-06','Chinoise','canard, miel, sauce soja','Omnivore','Laquer le canard, cuire au four','classique',4,15.00),(17,'Guacamole','entrée','2025-01-01','2025-01-05','Mexicaine','avocat, tomate, oignon','Végétarien','Écraser avocat, mélanger avec tomate et oignon','classique',5,5.00),(18,'Burritos','plat principal','2025-01-01','2025-01-06','Mexicaine','tortilla, viande, légumes, sauce','Omnivore','Farce tortilla, rouler et cuire','classique',5,12.00),(19,'Fajitas','plat principal','2025-01-01','2025-01-06','Mexicaine','poulet, poivrons, tortillas','Omnivore','Cuire poivrons et poulet, servir avec tortillas','classique',5,11.50),(20,'Churros','dessert','2025-01-01','2025-01-07','Mexicaine','farine, sucre, cannelle','Végétarien','Faire la pâte, frire et saupoudrer','classique',5,4.00);
/*!40000 ALTER TABLE `plat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `utilisateur`
--

DROP TABLE IF EXISTS `utilisateur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `utilisateur` (
  `Id_Utilisateur` int NOT NULL AUTO_INCREMENT,
  `Nom` varchar(100) DEFAULT NULL,
  `Prénom` varchar(100) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Mot_de_passe` varchar(255) DEFAULT NULL,
  `Adresse` varchar(255) DEFAULT NULL,
  `Station_Proche` int DEFAULT NULL,
  `Role` enum('Client','Admin') DEFAULT 'Client',
  PRIMARY KEY (`Id_Utilisateur`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `utilisateur`
--

LOCK TABLES `utilisateur` WRITE;
/*!40000 ALTER TABLE `utilisateur` DISABLE KEYS */;
INSERT INTO `utilisateur` VALUES (1,'Lemoine','Alice','alice@example.com','pass1','45 rue des Lilas, Paris',1,'Client'),(2,'Bernard','Lucas','lucas@example.com','pass2','12 avenue de la République, Paris',2,'Client'),(3,'Dupuis','Sophie','sophie@example.com','pass3','98 rue de Montmartre, Paris',3,'Client'),(4,'Martin','Jean','jean@example.com','pass4','50 boulevard Saint-Germain, Paris',4,'Client'),(5,'Durand','Paul','paul@example.com','pass5','23 rue de la Paix, Paris',5,'Client'),(6,'Admin','LivInParis','admin@livinparis.fr','admin123','Siège Paris, 75000',6,'Admin');
/*!40000 ALTER TABLE `utilisateur` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-04 17:48:41
