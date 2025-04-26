using Liv_in_Paris;

namespace Testes_unitaires
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string lien = "test.txt";
            File.WriteAllLines(lien, new[] {
                "1;1;Porte Maillot;2.282583847361549;48.87816265269652;Paris 17ème;75117",
                "2;1;Argentine;2.2894354185422134;48.87566737565167;Paris 17ème;75117",
                "3;1;Charles de Gaulle - Etoile;2.295811775235762;48.87566737565167;Paris 17ème;75117 ",
                "4;2;Charles de Gaulle - Etoile;2.295811775235762;48.874994575223035;Paris 17ème;75117 ",
                "5;2;Ternes;2.298113288617243;48.878227729914364;Paris 17ème;75117 ",
            });
            Graph graphe = new Graph(lien);
            Assert.AreEqual(graphe.Creation_Noeud("test.txt").Count, 5);
            Assert.AreEqual(graphe.Creation_Noeud("test.txt")[1].Id, 1);
            Assert.AreEqual(graphe.Creation_Noeud("test.txt")[1].Longitude, 2.282583847361549);
            Assert.AreEqual(graphe.Creation_Noeud("test.txt")[1].Latitude, 48.87816265269652);

            Assert.AreEqual(graphe.Liste_stations_par_ligne(graphe.Noeuds)["1"].Count, 3);
            Assert.AreEqual(graphe.Liste_stations_par_ligne(graphe.Noeuds)["2"].Count, 2);

            Assert.AreEqual(Math.Round(graphe.Distance(graphe.Noeuds[1], graphe.Noeuds[2]), 3), 0.573);
            List<Noeud> interdit1 = new List<Noeud>();
            List<Noeud> interdit2 = new List<Noeud> { graphe.Noeuds[3] };
            double[,] matrice1 = new double[,]
            {
                {0, 0.5727525760677799, 1.0063704324395517},
                {0.5727525760677799, 0, 0.4663180830659841},
                {1.0063704324395517, 0.4663180830659841, 0}
            };
            CollectionAssert.AreEquivalent(graphe.Matrice_Distance(graphe.Liste_stations_par_ligne(graphe.Noeuds)["1"], interdit1), matrice1);
            double[,] matrice2 = new double[,]
            {
                {0, 0.5727525760677799, 50},
                {0.5727525760677799, 0, 50},
                {50, 50, 50}
            };
            CollectionAssert.AreEquivalent(graphe.Matrice_Distance(graphe.Liste_stations_par_ligne(graphe.Noeuds)["1"], interdit2), matrice2);
            Assert.AreEqual(graphe.Noeud_Plus_Proche(matrice1, graphe.Noeuds[1], graphe.Noeuds[1]), graphe.Noeuds[2]);
            Assert.AreEqual(graphe.Noeud_Plus_Proche(matrice1, graphe.Noeuds[1], graphe.Noeuds[2]), graphe.Noeuds[3]);
            Assert.AreEqual(graphe.Noeud_Plus_Proche(matrice2, graphe.Noeuds[1], graphe.Noeuds[1]), graphe.Noeuds[2]);
            Lien l1 = new Lien(graphe.Noeuds[4], graphe.Noeuds[3], graphe.Distance(graphe.Noeuds[3], graphe.Noeuds[4]) / 25);
            Lien l2 = new Lien(graphe.Noeuds[3], graphe.Noeuds[4], graphe.Distance(graphe.Noeuds[3], graphe.Noeuds[4]) / 25);
            List<Lien> liens = new List<Lien> { l1, l2 };
            Assert.AreEqual(liens.Count, graphe.Creation_Lien_Changements().Count);
            Assert.AreEqual(graphe.Creation_Lien_Changements()[0].Initiale, graphe.Noeuds[4]);
            Assert.AreEqual(graphe.Creation_Lien_Changements()[0].Terminale, graphe.Noeuds[3]);
            Assert.AreEqual(graphe.Creation_Lien_Changements()[1].Initiale, graphe.Noeuds[3]);
            Assert.AreEqual(graphe.Creation_Lien_Changements()[1].Terminale, graphe.Noeuds[4]);

            Assert.AreEqual(graphe.Creation_Lien(graphe.Liste_stations_par_ligne(graphe.Noeuds)).Count, 8);
            Noeud noeud = new Noeud(6, "4", "Porte", 2.282583847361549, 48.87816265269652, "Paris 17ème", "75117", "1");
            List<Noeud> noeuds = new List<Noeud> { noeud };
            CollectionAssert.AreEquivalent(noeuds, graphe.Interdit(noeuds, "1"));
            double[,] matrice = {
                { 0, 1, 2, 3, 4, 5 },
                { 1, 0, 1.3746061825626716, 0, 0, 0 },
                { 2, 1.3746061825626716, 0, 1.1191633993583618, 0, 0 },
                { 3, 0, 1.1191633993583618, 0, 3.9000000000000004, 0 },
                { 4, 0, 0, 3.9000000000000004, 0, 0.9527021842216206 },
                { 5, 0, 0, 0, 0.9527021842216206, 0 }
            };
            CollectionAssert.AreEquivalent(graphe.Matrice_Adjacente(), matrice);
            List<Noeud> noeud_court = new List<Noeud> { graphe.Noeuds[1], graphe.Noeuds[2], graphe.Noeuds[3], graphe.Noeuds[4], graphe.Noeuds[5] };
            CollectionAssert.AreEquivalent(graphe.Dijkstra(graphe.Noeuds[1], graphe.Noeuds[5]), noeud_court);
            CollectionAssert.AreEquivalent(graphe.BellmanFord(graphe.Noeuds[1], graphe.Noeuds[5]), noeud_court);
            CollectionAssert.AreEquivalent(graphe.FloydWarshall(graphe.Noeuds[1], graphe.Noeuds[5]), noeud_court);
            Assert.AreEqual(graphe.Temps_Plus_Court_Chemin(graphe.Dijkstra(graphe.Noeuds[1], graphe.Noeuds[5])), 7.3);
        }
    }
}