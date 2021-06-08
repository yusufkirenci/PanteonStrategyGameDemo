using UnityEngine;

namespace Assets.Scripts.PathFinding
{
    /// <summary>
    ///     Bunu A-Star algoritmasındaki griddeki kutucuklar olarak kullanıyoruz.
    /// </summary>
    public class Tile
    {
        internal int distanceCost; //Mesafe maliyeti
        public bool isWall; //Gidilemeyen bir kutucukmu?
        internal int moveCost; //Hareket Maliyeti
        public Tile parentTile;
        public Vector3 worldPosition; // Unity deki pozisyonu
        public int x; //Grid x
        public int y; //Grid y
    }
}