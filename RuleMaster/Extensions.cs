using Chess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuleMaster
{
    public static class DictionaryExtensions
    {
        public static void AddToIEnumerable(this Dictionary<ChessPiece, HashSet<Location>> dictionary, ChessPiece piece)
        {

        }

        public static HashSet<Location> GetAllValues(this Dictionary<ChessPiece, HashSet<Location>> dictionary)
        {
            HashSet<Location> allLocations = new HashSet<Location>();

            foreach (var item in dictionary.Values)
            {
                allLocations.UnionWith(item);
            }

            return allLocations;
        }
    }
}
