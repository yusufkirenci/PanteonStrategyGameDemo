using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PathFinding
{
    public class PathFinder : MonoBehaviour
    {
        private List<List<Tile>> currentGrid; //Grid

        //Private değişkenler
        private GameObject lastTarget;
        private List<Tile> path; //Son yol gizmozda kullanılması için global variable

        /// <summary>
        ///     Path bulmak istiyorsak çağıracağımız budur
        ///     Daha Temiz kod için overload
        /// </summary>
        /// <param name="startTransform"></param>
        /// <param name="targetTransform"></param>
        public List<Tile> FindPath(Transform startTransformParam, Transform targetTransformParam)
        {
            //Saldırdığımız eğer bir duvarsa duvarlığını kırarak saldırıyoruz
            List<Tile> tiles;
            if (targetTransformParam.gameObject.layer == 8)
            {
                if (lastTarget != null) lastTarget.layer = 8;
                targetTransformParam.gameObject.layer = 0;
                lastTarget = targetTransformParam.gameObject;
            }

            tiles = FindPath(startTransformParam.position, targetTransformParam.position);
            return tiles;
        }

        /// <summary>
        ///     Path bulmak istiyorsak çağıracağımız budur
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        public List<Tile> FindPath(Vector2 startPos, Vector2 targetPos)
        {
            currentGrid = Grid.Instance.CreateGrid(); // Gridi oluştur
            var startTile = TileFromWorldPoint(startPos); //Başlama kutusunu bul
            var targetTile = TileFromWorldPoint(targetPos); //Bitiş kutusunu bul

            var openList = new List<Tile>(); //Açık kutucuk listesi
            var closedList = new HashSet<Tile>(); //Ziyaret edilmişmi ve duvarmı kontrolü için liste

            openList.Add(startTile); //Başlama kutucuğunu açık listeye atarak başlıyoruz

            while (openList.Count > 0)
            {
                var currentTile = openList[0];
                for (var i = 1; i < openList.Count; i++) //İkinci objeden başlayarak döngü
                    if (openList[i].distanceCost < currentTile.distanceCost) //Mesafe masrafı kontrolü
                        currentTile = openList[i]; //objeyi current tile a atıyoruz
                openList.Remove(currentTile);
                closedList.Add(currentTile);

                if (currentTile == targetTile) //Eğer şuanki kutumuz hedefimiz ise sona geldik demektir :D
                    GetFinalPath(startTile, targetTile); //Son yolu hesaplamak için metodumuzu çağırıyoruz

                foreach (var neighborTile in
                    GetNeighboringTiles(currentTile)) //Yakındaki kutularla döngü oluşturuyoruz
                {
                    if (neighborTile.isWall ||
                        closedList.Contains(
                            neighborTile)) //Döngüdeki kutunun wall mı yoksa daha önceden gittiğimiz bir yermi olduğunu kontrol ediyoruz
                        continue; //Kapalı listedeyse geçiyoruz
                    var moveCost =
                        currentTile.moveCost + GetDistance(currentTile, neighborTile); //Masrafı tekrar hesaplıyoruz

                    if (moveCost < neighborTile.moveCost ||
                        !openList.Contains(
                            neighborTile)) //Yakındakilerle masraf kontrolü yapıp Açık listede olup olmadığına bakıyoruz
                    {
                        neighborTile.moveCost = moveCost; //hesapladığımız movecostu döngüdekine atıyoruz
                        neighborTile.distanceCost =
                            GetDistance(neighborTile, targetTile); //Hedef ile aradaki mesafeyi döngüdekine atıyoruz
                        neighborTile.parentTile =
                            currentTile; //Geri hareket için döngüdeki parent tile ı şimdikine atıyoruz

                        if (!openList.Contains(neighborTile)) //döngüdeki açık listede değilse atıyoruz
                            openList.Add(neighborTile);
                    }
                }
            }

            return path;
        }

        /// <summary>
        ///     Son işlem
        /// </summary>
        /// <param name="startingTile"></param>
        /// <param name="endTile"></param>
        private List<Tile> GetFinalPath(Tile startingTile, Tile endTile)
        {
            var finalPath = new List<Tile>();
            var currentTile = endTile;

            while (
                currentTile !=
                startingTile) //Parentlerden yolun başlangıcına kadar her kutuda çalışmak için while döngüsü
            {
                finalPath.Add(currentTile); //listemizde şuankini atıyorz
                currentTile = currentTile.parentTile; //Parentini yeniden dzenliyoruz
            }

            finalPath.Reverse(); //Düzgün bir sıralama için reverse yapıyoruz
            path = finalPath;
            return finalPath;
        }

        /// <summary>
        ///     İki tile arası mesafe hesabı
        /// </summary>
        /// <param name="tileA"></param>
        /// <param name="tileB"></param>
        /// <returns></returns>
        private int GetDistance(Tile tileA, Tile tileB)
        {
            //matematiksel uzaklık hesaplaması
            var ix = Mathf.Abs(tileA.x - tileB.x);
            var iy = Mathf.Abs(tileA.y - tileB.y);

            return ix + iy;
        }

        /// <summary>
        ///     Çapraz ve yanlardakileri getirmek için kullanılır
        /// </summary>
        /// <param name="neighborTile">Bakılacak tile</param>
        /// <returns></returns>
        private List<Tile> GetNeighboringTiles(Tile neighborTile)
        {
            /// Burada yakındaki kutuları buluyoruz.
            /// Aşağıda 8 tane tekrar göreceksiniz (Tüm düz giden yanlar ve çaprazlar için)

            var neighborList = new List<Tile>(); //Döndürmek için liste 
            var neighborNodeX = neighborTile.x + 1; //Gridden yanındakini almak için hesapma
            var neighborNodeY = neighborTile.y; //Gridden yanındakini almak için hesapma
            if (neighborNodeX >= 0 && neighborNodeX < Grid.Instance.countXY) //Xde Son kutumu?
                if (neighborNodeY >= 0 && neighborNodeY < Grid.Instance.countXY) //Yde Son kutumu?
                    neighborList.Add(
                        currentGrid[neighborNodeX][neighborNodeY]); //Eğer şartlar uyuyorsa Neighbor listesine atıyoruz
            // Aşağıdakiler içinde aynı şey geçerli bu yüzden yorum yazılmadı
            neighborNodeX = neighborTile.x + 1;
            neighborNodeY = neighborTile.y + 1;
            if (neighborNodeX >= 0 && neighborNodeX < Grid.Instance.countXY)
                if (neighborNodeY >= 0 && neighborNodeY < Grid.Instance.countXY)
                    neighborList.Add(currentGrid[neighborNodeX][neighborNodeY]);
            neighborNodeX = neighborTile.x - 1;
            neighborNodeY = neighborTile.y;
            if (neighborNodeX >= 0 && neighborNodeX < Grid.Instance.countXY)
                if (neighborNodeY >= 0 && neighborNodeY < Grid.Instance.countXY)
                    neighborList.Add(currentGrid[neighborNodeX][neighborNodeY]);
            neighborNodeX = neighborTile.x - 1;
            neighborNodeY = neighborTile.y - 1;
            if (neighborNodeX >= 0 && neighborNodeX < Grid.Instance.countXY)
                if (neighborNodeY >= 0 && neighborNodeY < Grid.Instance.countXY)
                    neighborList.Add(currentGrid[neighborNodeX][neighborNodeY]);
            neighborNodeX = neighborTile.x;
            neighborNodeY = neighborTile.y + 1;
            if (neighborNodeX >= 0 && neighborNodeX < Grid.Instance.countXY)
                if (neighborNodeY >= 0 && neighborNodeY < Grid.Instance.countXY)
                    neighborList.Add(currentGrid[neighborNodeX][neighborNodeY]);
            neighborNodeX = neighborTile.x - 1;
            neighborNodeY = neighborTile.y + 1;
            if (neighborNodeX >= 0 && neighborNodeX < Grid.Instance.countXY)
                if (neighborNodeY >= 0 && neighborNodeY < Grid.Instance.countXY)
                    neighborList.Add(currentGrid[neighborNodeX][neighborNodeY]);
            neighborNodeX = neighborTile.x;
            neighborNodeY = neighborTile.y - 1;
            if (neighborNodeX >= 0 && neighborNodeX < Grid.Instance.countXY)
                if (neighborNodeY >= 0 && neighborNodeY < Grid.Instance.countXY)
                    neighborList.Add(currentGrid[neighborNodeX][neighborNodeY]);
            neighborNodeX = neighborTile.x + 1;
            neighborNodeY = neighborTile.y - 1;
            if (neighborNodeX >= 0 && neighborNodeX < Grid.Instance.countXY)
                if (neighborNodeY >= 0 && neighborNodeY < Grid.Instance.countXY)
                    neighborList.Add(currentGrid[neighborNodeX][neighborNodeY]);

            return neighborList; //neighbors listesini döndürüyoruz.
        }

        /// <summary>
        ///     World pozisyonuna göre Tile döndürür.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        private Tile TileFromWorldPoint(Vector2 worldPos)
        {
            var ixPos = (worldPos.x + Grid.Instance.tileRadius * Grid.Instance.countXY / 2) /
                        (Grid.Instance.tileRadius * Grid.Instance.countXY);
            var iyPos = (worldPos.y + Grid.Instance.tileRadius * Grid.Instance.countXY / 2) /
                        (Grid.Instance.tileRadius * Grid.Instance.countXY);
            ixPos = Mathf.Clamp01(ixPos);
            iyPos = Mathf.Clamp01(iyPos);
            var ix = Mathf.RoundToInt((Grid.Instance.countXY - 1) * ixPos);
            var iy = Mathf.RoundToInt((Grid.Instance.countXY - 1) * iyPos);
            return currentGrid[ix][iy];
        }
    }
}