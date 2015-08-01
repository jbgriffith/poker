using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Poker.NHib;
using Poker.DbModels;

namespace Poker {
    class Program {
        static void Main(string[] args) {
            // This should point to a database that will contain the "new model" data.
            // Though NHibernate will write the schema to it, no data is currently written to this db because we are not interested in history.
            NHConfiguration nhConfig = new NHConfiguration(
                connectionName: "PokerDb",
                mapFromAssembliesOfType: new Type[] { typeof(Poker.DbModels.Card) },
                dbDropCreate: false,
                dbSchemaUpdate: false,
                auxShouldMap: (t) => { return (t.Namespace == "Poker.DbModels" && t.Name.StartsWith("Card") || t.Name == "Player" || t.Name == "Game" || t.Name == "Hand" || t.Name == "Deck" || t.Name == "ModelBase" || t.Name == "ModelBaseGuid"); }
                //auxShouldMap: (t) => { return (t.Namespace == "GroupCourses_Model" || t.Name == "ModelBase"); }
            );

            //List<Player> AllPlayers = new List<Player>();
            var Overallsw = Stopwatch.StartNew();
            var sw = Stopwatch.StartNew();

            //Console.WriteLine("Downloading & Creating users ...");
            //for (int users = 0; users < 1; users++)
            //    AllPlayers.AddRange(User.UserData.GetUserData("http://api.randomuser.me/?results=100&key=6QGL-Z2YJ-OL4O-MACS"));

            //Console.WriteLine("Created {0} users", AllPlayers.Count);
            //Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);

            var games = new List<Game>();

            Overallsw.Restart();
            for (int i = 0; i < 10; i++) {
                sw.Restart();
                using (var sess = nhConfig.SessionFactory.OpenSession()) {
                    using (var tx = sess.BeginTransaction()) {
                        var AllPlayers = sess.CreateCriteria(typeof(Player)).List<Player>();

                        for (int g = 1; g < 1001; g++) {
                            Random r = new Random();
                            //int range = 7;
                            //int rInt = r.Next(2, range); //for ints
                            int rInt = 5;

                            // Add each player to the table
                            var firstNPlayers = AllPlayers.Shuffle().Take(rInt).ToList();

                            var nPlayers = new HashSet<Player>(firstNPlayers);  // HashSet should speed up the RemoveAll http://stackoverflow.com/a/853551/3042939
                            //AllPlayers.RemoveAll(x => nPlayers.Contains(x));

                            var game = new Game(nPlayers.ToList());

                            foreach (var player in game.Players) {
                                var newHand = new Hand(game.Deck.DealCards(5), player, game);
                                newHand.CalculateHand();
                                player.CurrentHand = newHand;
                            }

                            var maxScore = game.Players.Max(x => x.CurrentHand.Score);
                            var playersWithMaxScore = game.Players.Where(x => x.CurrentHand.Score == maxScore).ToList();
                            var winner = playersWithMaxScore[0];
                            //Console.WriteLine("Winning Hand(s) is: {0}", (EvaluatePokerHand.PokerHand)maxScore);
                            // playersWithMaxScore.SetWinners();
                            // instead of all of the crap below

                            var ScoreDetails = new List<List<int>>();
                            foreach (var player in playersWithMaxScore) {
                                player.CurrentHand.SetScoreDetails();
                                ScoreDetails.Add(player.CurrentHand.ScoreDetail.ToList());
                            }

                            if (playersWithMaxScore.Count > 1) {
                                var p1 = ScoreDetails[0];
                                for (int numItem = 0; numItem < p1.Count; numItem++) {
                                    var blah = new List<int>();

                                    for (int ea = 0; ea < ScoreDetails.Count; ea++)
                                        blah.Add(ScoreDetails[ea][numItem]);

                                    ScoreDetails = ScoreDetails.Where(x => x[numItem] == blah.Max()).ToList();
                                    if (ScoreDetails.Count == 1)
                                        break;
                                }

                                foreach (var player in playersWithMaxScore) {
                                    if (player.CurrentHand.ScoreDetail.SequenceEqual(ScoreDetails[0])) {
                                        //Console.WriteLine("{0} is a winner!!!!!", player.Name);
                                        player.CurrentHand.HandWonGame = true;
                                    }
                                }
                            }
                            else {
                                //Console.WriteLine("{0} is a winner!!!!!", winner.Name);
                                winner.CurrentHand.HandWonGame = true;
                            }
                           
                            foreach (var player in game.Players)
                                player.SaveHand();

                            sess.SaveOrUpdate(game);
                            games.Add(game);
                            //AllPlayers.AddRange(game.Players);

                            if (g % 100 == 0) Console.WriteLine("Game #{0}", g);
                        }

                        Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
                        sw.Restart();
                        Console.WriteLine("Saving Changes");

                        //sess.Flush();
                        tx.Commit();
                        Console.WriteLine("All games have been saved");
                        Console.WriteLine("Time Elapsed: {0}", sw.Elapsed);
                        sw.Stop();
                        //#if DEBUG
                        //					Console.WriteLine("Press any key to close...");
                        //					Console.ReadLine();
                        //#endif
                    }
                }
            }
            Console.WriteLine("Entire Program Time Elapsed: {0}", Overallsw.Elapsed);
            Console.ReadLine();
        }
    }
}
