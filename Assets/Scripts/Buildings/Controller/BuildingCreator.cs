using UnityEngine;

namespace Assets.Scripts.Buildings.Controller
{
    public class BuildingCreator : MonoBehaviour
    {

        public GameObject[] buildingVarieties; //Building listesi
        private static BuildingCreator _instance;//GameManager için bir singleton
        public static BuildingCreator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BuildingCreator>();
                    if (_instance == null) Debug.LogError("You need add BuildingCreator to a GameObject!");
                }

                return _instance;
            }
        }
        /// <summary>
        ///     Building varieties indisine göre bina oluşturur.
        /// </summary>
        /// <param name="arrayIndex">buildingVarieties indexi</param>
        public void CreateBuilding(int arrayIndex)
        {
            //Binamızın clonunu oluşturuyoruz
            var buildingGameObject = Instantiate(buildingVarieties[arrayIndex], new Vector3(1000, 1000, 0),
                Quaternion.identity);
            //Sürükle bırak sisteminin çalışabilmesi için gerekli
            buildingGameObject.GetComponent<MovableObject>().SetForCreating();
        }
    }
}