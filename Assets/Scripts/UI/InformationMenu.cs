using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Buildings;
using Assets.Scripts.Common;
using Assets.Scripts.GamePlay;
using Assets.Scripts.Soldier;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class InformationMenu : MonoBehaviour
    {
        //InformationMenu için unity singleton
        [Header("UI Elements")]
        public int[] damagePerAttacks = {10, 5, 2}; //Daha çok buton koyabilmek için ek ayar
        public Image healtBarImage; //Can barı
        public GameObject healtBarParent; //Healt Barın parentı
        public Text healtBarText; //Canın yazdığı bir label
        public Text infoText; //Birden fazla asker seçildiğinde çıkan yazı
        public Text nameText; //İsim için label
        public Button[] soldierCreateButtons; //Asker üretme butonları
        public Image unitImage; //Unit resmi
        private static InformationMenu _instance;
        public static InformationMenu Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InformationMenu>();
                    if (_instance == null) Debug.LogError("You need add InformationMenu to a GameObject!");
                    //Ayarlama yapmam gerektiği için new işime yaramaz
                    // _instance = new InformationMenu();
                }

                return _instance;
            }
        }
        /// <summary>
        ///     Diğerlerini kapatıp sadece infotext i kullanabilmek için veya tam tersini yapabilmek için method
        /// </summary>
        /// <param name="active"></param>
        private void ShowInfoText(bool active)
        {
            if (active)
            {
                healtBarParent.SetActive(false);
                unitImage.gameObject.SetActive(false);
                nameText.gameObject.SetActive(false);
                healtBarText.gameObject.SetActive(false);
                healtBarImage.gameObject.SetActive(false);
                foreach (var button in soldierCreateButtons) button.gameObject.SetActive(false);
                infoText.gameObject.SetActive(true);
            }
            else
            {
                healtBarParent.SetActive(true);
                unitImage.gameObject.SetActive(true);
                nameText.gameObject.SetActive(true);
                healtBarText.gameObject.SetActive(true);
                healtBarImage.gameObject.SetActive(true);
                foreach (var button in soldierCreateButtons) button.gameObject.SetActive(true);
                infoText.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// Bilgi ekranını Güncellemek için kullanılan method
        /// </summary>
        /// <param name="selectedGameObjects"></param>
        public void UpdateInfoScreen(List<GameObject> selectedGameObjects)
        {
            if (selectedGameObjects.Count == 0)// Eğer seçili obje yoksa bu mesajı veriyoruz=>No Unit Selected! Left click to select a Unit or Create a unit from Production Menu.
            {
                ShowInfoText(true);
                infoText.text = "No Unit Selected! Left click to select a Unit or Create a unit from Production Menu.";
            }
            else if (selectedGameObjects.Count > 1)// Eğer birden fazla obje yoksa bu mesajı veriyoruz=> selectedGameObjects.Count + " soldiers selected! Right Click to Attack!
            {
                ShowInfoText(true);
                infoText.text = selectedGameObjects.Count + " soldiers selected! Right Click to Attack!";
            }
            else// Eğer bir obje seçiliyse özelliklerini info panelimizde gösteriyoruz
            {
                var selectedObject = selectedGameObjects.First();//birinci objemizi listeye atıyoruz
                ShowInfoText(false);//infoyu kapatıyoruz

                if (selectedObject.GetComponent<Barrack>())//Barracksa butonları göstereceğiz
                {
                    if (soldierCreateButtons != null)
                        for (var i = 0; i < soldierCreateButtons.Length; i++)
                        {
                            var button = soldierCreateButtons[i];
                            button.gameObject.SetActive(true);
                            button.onClick.RemoveAllListeners();
                            var index = i; //Bu şekilde yapmazsam captured variable modified other scope diyor ve değer 3te kalıyor
                            //Onclick evente listener ataması yapıyorum ki istediğimiz askeri istediğimiz barracktan doğuralım
                            var act = new UnityAction(() =>
                            {
                                selectedObject.GetComponent<Barrack>().CreateSoldier(damagePerAttacks[index]);
                            });
                            button.onClick.AddListener(act);
                        }
                }
                else
                {
                    //Eğer barrack değilse butonları kapat
                    if (soldierCreateButtons != null)
                        foreach (var btn in soldierCreateButtons)
                            btn.gameObject.SetActive(false);
                }
                //UI Çıktılarını veriyoruz
                var unit = selectedObject.GetComponent<Unit>();
                var healt = selectedObject.GetComponent<Healt>();
                unitImage.sprite = unit.unitImage;
                nameText.text = unit.name;
                healtBarText.text = healt.HealtPoint.ToString();
                healtBarImage.fillAmount = healt.HealtPoint / (float) healt.startHealt * 1f;
                if (selectedObject.GetComponent<SoldierController>())
                {
                    infoText.gameObject.SetActive(true);
                    infoText.text = "Right Click to Attack or Move to Position!";
                }
            }
        }
    }
}