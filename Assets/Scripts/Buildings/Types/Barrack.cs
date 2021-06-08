using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Soldier;
using UnityEngine;
namespace Assets.Scripts.Buildings
{
    public class Barrack : MonoBehaviour
    {
        public static int soldierCounter;//Her bir yeni askere numara vermek için yapılmıştır. Random da kullanılabilir.
        public float randomSpawnPositionFactor = 0.3f;//Biraz daha fazla ramdomizasyon için kullanılır
        public GameObject soldier;//Asker Prefab
        public List<Transform> spawnPositions;//doğma pozisyonları
        public void CreateSoldier(int damagePerAttack)
        {
            //doğabileceğimiz yer açıktamı
            var spawnAblePositions = spawnPositions
                .Where(x => !Physics2D.OverlapCircle(x.position, randomSpawnPositionFactor,8)).ToList();


            //Gereksiz bir kod muhtemelen gerçek oyunda ekrana bilgi yazdırırdım
            if (spawnAblePositions.Count == 0)
            {
                print("All spawn position is closed! Soldier Can't be created");
                return;
            }
            //Spawnable posizyonlardan rasgele bir tanesi
            var randomSpawnPosition = spawnAblePositions[Random.Range(0, spawnAblePositions.Count)];
            //Pozisyon randomizasyonu
            var someMoreRandom = new Vector3(
                randomSpawnPosition.position.x + Random.Range(-randomSpawnPositionFactor, randomSpawnPositionFactor),
                randomSpawnPosition.position.y + Random.Range(-randomSpawnPositionFactor, randomSpawnPositionFactor));
            //Askerleri oluşturma
            var soldierInstantiated =
                Instantiate(soldier, someMoreRandom, randomSpawnPosition.rotation).GetComponent<SoldierController>();
            soldierInstantiated.damagePerAttack = damagePerAttack;
            //Unit Komponentine isim değişkenini değiştiriyoruz
            soldierInstantiated.GetComponent<Unit>().name =
                "Soldier" + soldierCounter + " (DAMAGE:" + damagePerAttack + ")";
            soldierCounter++;
        }
    }
}