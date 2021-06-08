using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Soldier;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GamePlay
{
    //Oyunun kalbi
    public class GameManager : MonoBehaviour
    {
        //Gamemanager için bir singleton

        //Debug yapabilmemiz kolaylaşsın diye inspecterda görüntülüyoruz
        [SerializeField] [Header("Selected Objects")]
        private List<GameObject> selectedGameObjects; // Seçili objeler asker veya bina tutabilir
        public List<GameObject> SelectedGameObjects
        {
            set => selectedGameObjects = value;
            get => selectedGameObjects;
        }
        private static GameManager _instance;//GameManager için bir singleton
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null) Debug.LogError("You need add GameManager to a GameObject!");
                }

                return _instance;
            }
        }

        private void Update()
        {
            //Unit seçimi
            if (Input.GetMouseButtonDown(0))
            {
                var hits = Physics2D.RaycastAll(UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero);//Mouseun üzerinde olduğu yeri bul
                if (EventSystem.current.IsPointerOverGameObject()) return;//Mouse UI üzerindeyse return
                foreach (var hit in hits)//Hits için döngü
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<Unit>())// Döngüdeki eleman bir birim mi?
                        {
                            SelectedGameObjects.Clear();//Tekrar seçim olduğu içn listeyi temizle
                            SelectedGameObjects.Add(hit.collider.gameObject);//ve tekrar elimizdeki elemanı listeye at
                            //Information Menüyü güncelliyoruz
                            InformationMenu.Instance.UpdateInfoScreen(SelectedGameObjects);
                            break;//İlk uniti bulduğumuzda döngüyü kır
                        }

                        SelectedGameObjects.Clear();
                        //Information Menuyü güncelliyoruz
                        InformationMenu.Instance.UpdateInfoScreen(SelectedGameObjects);
                    }
            }
            //Saldırı
            if (Input.GetMouseButtonDown(1))
                if (selectedGameObjects.Count != 0 &&
                    selectedGameObjects.Count(x => x.GetComponent<SoldierController>() != null) ==
                    selectedGameObjects.Count)//Seçili objelerin hepsi askermi? Listede eleman varmı?
                {
                    var hits = Physics2D.RaycastAll(UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        Vector2.zero);//Mouseun üzerinde olduğu yeri bul
                    if (EventSystem.current.IsPointerOverGameObject()) return;//Mouse UI üzerindeyse return
                    if (hits.Length == 0) return;//Mouse hiç birşeyin üzerinde değilse return

                    var firstHealtUnit = hits.ToList()
                        .FirstOrDefault(x => x.collider.transform.GetComponent<Healt>() != null);//Mouse bir canı olan obje üzerindemi
                    if (firstHealtUnit)
                        foreach (var go in selectedGameObjects)
                        {
                            //Seçilen tüm askerleri hedefe gönder
                            var soldierController = go.GetComponent<SoldierController>();
                            soldierController.Target = firstHealtUnit.transform;
                            soldierController.isAttack = true;
                        }
                    else
                        foreach (var go in selectedGameObjects)
                            //Eğer canı olmayan bir obje ise seçilen obje hedefe yürü
                            go.GetComponent<SoldierController>().Target =
                                Instantiate(new GameObject(), hits.First().point, Quaternion.identity).transform;
                }
        }
    }
}