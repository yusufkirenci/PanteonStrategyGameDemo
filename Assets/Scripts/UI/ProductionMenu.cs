using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Buildings.Controller;
using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ProductionMenu : MonoBehaviour
    {
        [Header("Settings")]
        public Transform buttonElementPrefab;//Klonlanacak prefab
        public RectTransform buttonParentPanel;//Butonların Parenti
        public List<Unit> units;//Unitler

        private float buttonSize = 50;//Hesaplama yaparken kullanılan buttonSize
        private int totalPoolSize = 50;//Awake te pencere yükseliğine göre set ediliyor
        private float lastPositionY;//Temp değişken
        private RectTransform rectTransform;//Bunun rectTransform
        private ScrollRect scrollRect;//Bunun scrollRect
        private List<RectTransform> transformPoolingList;//Objeleri tuttuğumuz list
        private void Awake()
        {
            // Instanceları ayarlıyoruz
            scrollRect = GetComponent<ScrollRect>();
            rectTransform = GetComponent<RectTransform>();
            transformPoolingList = new List<RectTransform>();

            //Poolsize ımızı hesaplıyoruz
            var heigth = Screen.height + rectTransform.offsetMax.y;
            totalPoolSize = (int) heigth / (int) buttonSize + 3;
        }
        private void Start()
        {
            buttonParentPanel.sizeDelta = new Vector2(buttonSize * 2, buttonSize * totalPoolSize);//Panelimizin başlangıç yüksekliğini ayarlıyoruz

            for (var i = 0; i < totalPoolSize; i++)//poolSize kadar dönen döngü
            {
                //Butonlarımız yanyana olduğu için herşeyden iki tane var!!
                //Prefabımızdan objelerimizi klonluyoruz
                var g = Instantiate(buttonElementPrefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<RectTransform>();
                var g1 = Instantiate(buttonElementPrefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<RectTransform>();
                //Parentleri olarak panelimizi seçiyoruz
                g.transform.SetParent(buttonParentPanel.transform);
                g1.transform.SetParent(buttonParentPanel.transform);

                //Building oluşturabilmeleri için eventleri ayarlıyorz
                AddBuildingCreateEventToButton(g.gameObject.AddComponent<EventTrigger>(),0);
                AddBuildingCreateEventToButton(g1.gameObject.AddComponent<EventTrigger>(),1);

                //Textlerini ve Imagelerini atıyoruz
                g.GetComponent<Image>().sprite = units[0].unitImage;
                g.GetComponentInChildren<Text>().text = units[0].name + "(" + i + ")";
                g1.GetComponent<Image>().sprite = units[1].unitImage;
                g1.GetComponentInChildren<Text>().text = units[1].name + "(" + i + ")";

                //Pool listemizi dolduruyoruz
                transformPoolingList.Add(g);
                transformPoolingList.Add(g1);

                //Son olarak pozisyonlarını ayarlıyoruz
                g.anchoredPosition = new Vector2(0, -i * buttonSize + buttonSize);
                g1.anchoredPosition = new Vector2(buttonSize, -i * buttonSize + buttonSize);
            }
            //Scroll rectimize value changed event i ekliyoruz.
            scrollRect.onValueChanged.AddListener(ScrollRectValueChanged);
        }
        /// <summary>
        ///     Update fonksiyonun da hesaplayıp ek yük bindirmemek için event kullanıyoruz(unity action)
        /// </summary>
        /// <param name="vector2"></param>
        private void ScrollRectValueChanged(Vector2 vector2)
        {
            //panelimizin Y de aşağı yönlü buttonsize kadar hareketi varmı?
            if (lastPositionY + buttonSize < buttonParentPanel.anchoredPosition.y)
            {

                lastPositionY = lastPositionY + buttonSize;//Temprorary variableımızı ayarlıyoruz
                //Parent panelin sizeını tekrar hesaplıyoruz
                buttonParentPanel.sizeDelta = new Vector2(buttonParentPanel.sizeDelta.x, buttonSize + buttonParentPanel.sizeDelta.y);
                //Methodumuzdan ilk rect transformları ve son y pozisyonunu istiyoruz
                var lastYPos = GetLastYPosAndChangeList(out var rectTransform, out var rectTransform1);
                //Out paremetleriyle bize verilen rectTransformların konumlarını ayarlıyoruz
                var calculatedYPos = lastYPos - buttonSize;
                rectTransform.anchoredPosition = new Vector2(0, calculatedYPos);
                rectTransform1.anchoredPosition = new Vector2(buttonSize, calculatedYPos);
            }
            //panelimizin Y de yukarı yönlü buttonsize kadar hareketi varmı?
            if (lastPositionY - buttonSize > buttonParentPanel.anchoredPosition.y)
            {
                lastPositionY = lastPositionY - buttonSize;//Temprorary variableımızı ayarlıyoruz
                //Parent panelin sizeını tekrar hesaplıyoruz
                buttonParentPanel.sizeDelta = new Vector2(buttonParentPanel.sizeDelta.x, buttonParentPanel.sizeDelta.y - buttonSize);
                //Methodumuzdan son rect transformları ve ilk y pozisyonunu istiyoruz
                var firstYPos = GetFirstYPosAndChangeList(out var rectTransform, out var rectTransform1);
                //Out paremetleriyle bize verilen rectTransformların konumlarını ayarlıyoruz
                var calculatedYPos = firstYPos + buttonSize;
                rectTransform.anchoredPosition = new Vector2(0, calculatedYPos);
                rectTransform1.anchoredPosition = new Vector2(buttonSize, calculatedYPos);
            }
        }
        /// <summary>
        /// verilen event trigger componentine createbuilding fonksiyonunu atar
        /// </summary>
        /// <param name="eventTrigger"></param>
        /// <param name="buildingIndex"></param>
        private void AddBuildingCreateEventToButton(EventTrigger eventTrigger,int buildingIndex)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) =>
            {
                BuildingCreator.Instance.CreateBuilding(buildingIndex);
            });
            eventTrigger.triggers.Add(entry);
        }
        /// <summary>
        /// İlk rect transformlarını ve son y pozisyonunu verir
        /// </summary>
        /// <param name="ob1"></param>
        /// <param name="ob2"></param>
        /// <returns></returns>
        private float GetLastYPosAndChangeList(out RectTransform ob1, out RectTransform ob2)
        {

            //Son butonunu buluyoruz
            //Son butonun pozisyonunu alıyoruz
            //ilk butonları alıp listedeki konumunu sona atıyoruz
            //Out paremetrelerini doldurup son Y pozisyonunu geri döndürüyoruz

            var lastButton = transformPoolingList.Last();
            var firstButton = transformPoolingList.First();
            transformPoolingList.Remove(firstButton);
            var firstButton1 = transformPoolingList.First();
            transformPoolingList.Remove(firstButton1);
            transformPoolingList.Add(firstButton1);
            transformPoolingList.Add(firstButton);
            var heigth = lastButton.anchoredPosition.y;
            ob1 = firstButton;
            ob2 = firstButton1;
            return heigth;
        }
        /// <summary>
        /// Son rect transformlarını ve ilk y pozisyonunu verir
        /// </summary>
        /// <param name="ob1"></param>
        /// <param name="ob2"></param>
        /// <returns></returns>
        private float GetFirstYPosAndChangeList(out RectTransform ob1, out RectTransform ob2)
        {
            //ilk butonunu buluyoruz
            //ilk butonun pozisyonunu alıyoruz
            //Son butonları alıp listedeki konumunu başa atıyoruz
            //Out paremetrelerini doldurup son Y pozisyonunu geri döndürüyoruz
            var firstButton = transformPoolingList.First();
            var lastButton = transformPoolingList.Last();
            transformPoolingList.Remove(lastButton);
            var lastButton1 = transformPoolingList.Last();
            transformPoolingList.Remove(lastButton1);
            transformPoolingList.Insert(0, lastButton1);
            transformPoolingList.Insert(0, lastButton);
            var heigth = firstButton.anchoredPosition.y;
            ob1 = lastButton;
            ob2 = lastButton1;
            return heigth;
        }
    }
}