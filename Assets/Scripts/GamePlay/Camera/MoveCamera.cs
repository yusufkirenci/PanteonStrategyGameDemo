using UnityEngine;

namespace Assets.Scripts.GamePlay.Camera
{
    public class MoveCamera : MonoBehaviour
    {
        private Vector3 difference;//Mouse pozisyonu camera arasındaki fark için değişken
        private bool isDrag;//Şuan sürükleniyormu?
        private UnityEngine.Camera mainCamera;//Ana kamera için değişken
        private Vector3 origin;//Mouse merkezi
        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
        }
        private void Update()
        {
            if (Input.GetMouseButton(2))//Middle mouse a tıklanıyormu
            {
                //Mouse pozisyonu camera arasındaki fark
                difference = mainCamera.ScreenToWorldPoint(Input.mousePosition) - mainCamera.transform.position;
                if (isDrag == false)
                {
                    isDrag = true;
                    origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                isDrag = false;
            }
            //Drag etkinse camera pozisyonunu değiştir
            if (isDrag) mainCamera.transform.position = origin - difference;
        }
    }
}