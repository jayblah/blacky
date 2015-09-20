using System;
using LeagueSharp.Common;

namespace EnemyRange
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += EnemyRange.OnLoad;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }
    }
}