using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Friendships
{
    class Program
    {
        static Random rand = new Random();

        static void Main(string[] args)
        {
            List<string> userIds = new List<string>(File.ReadAllLines("users.txt"));

            Dictionary<string, HashSet<string>> friendships = new Dictionary<string, HashSet<string>>();
            foreach (var user in userIds)
            {
                friendships[user] = new HashSet<string>();
            }

            foreach (var user in userIds)
            {
                int friendCount = Clamp((int)Math.Round(GetNormal(25, 10)), 0, 50);
                var possibleFriends = new HashSet<string>(userIds);
                possibleFriends.Remove(user);
                possibleFriends.ExceptWith(friendships[user]);

                List<string> candidates = new List<string>(possibleFriends);
                Shuffle(candidates);

                int added = 0;
                foreach (var friend in candidates)
                {
                    if (added >= friendCount) break;

                    if (!friendships[user].Contains(friend))
                    {
                        friendships[user].Add(friend);
                        friendships[friend].Add(user);
                        added++;
                    }
                }
            }

            var serializableDict = new Dictionary<string, List<string>>();
            foreach (var kvp in friendships)
            {
                serializableDict[kvp.Key] = new List<string>(kvp.Value);
            }

            string json = JsonSerializer.Serialize(new { friends = serializableDict }, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("friendships.json", json);

            Console.WriteLine("Arkadaşlıklar friendships.json dosyasına yazıldı.");
        }

        static double GetNormal(double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = rand.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }
    }
}
