using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PathFinding
{
    public class Grid : MonoBehaviour
    {
        //Grid ayarlarını ayırmak için header
        [Header("Grid Settings")] public Transform startPosition; //Gridin başlama pozisyonu
        public float tileRadius = 0.32f; //Kutucukların boyu
        public LayerMask wallMask; //Gidilemeyen yer katmanları
        public int countXY = 25; //X ve Y de kaç kutucuk olacağı 25*25=toplam kutucuk
        private static Grid _instance;
        public static Grid Instance//Grid için singleton
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Grid>();
                    if (_instance == null) Debug.LogError("You need add Grid to a GameObject!");
                    //Inspector üzerinden Ayarlama yapmam gerektiği için new işime yaramaz
                    // _instance = new Grid();
                }

                return _instance;
            }
        }
        /// <summary>
        ///     A-Star algoritmasını kullanabilmemiz için kutucuklar oluşturur.
        /// </summary>
        /// <param name="startVector">Başlama pozisyonu "Player" konumu gibi</param>
        /// <param name="targetVector">Hedef konum</param>
        /// <returns></returns>
        public List<List<Tile>> CreateGrid()
        {
            if (startPosition == null)
            {
                Debug.LogError("Start position not set!");
                return null;
            }

            var tileArray = new List<List<Tile>>(); // Boş liste örneği
            for (var x = 0; x < countXY; x++)
            {
                var t = new List<Tile>(); // Boş liste örneği
                tileArray.Add(t);
                for (var y = 0; y < countXY; y++)
                {
                    //Tile bizim için griddeki kutucuktur
                    var tile = new Tile
                    {
                        x = x,
                        y = y
                    };
                    // Döngüdeki sıradaki kutucuğun pozisyonunun hesaplaması
                    var worldPos = new Vector3(startPosition.position.x + tileRadius * x,
                        startPosition.position.y + tileRadius * y);
                    tile.worldPosition = worldPos;

                    // Duvar kontrolü
                    if (Physics2D.OverlapCircle(worldPos, tileRadius, wallMask)) tile.isWall = true;
                    t.Add(tile);
                }
            }

            return tileArray; 
        }
    }
}