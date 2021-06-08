using Assets.Scripts.Mathx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Buildings.Controller
{
    public class MovableObject : MonoBehaviour
    {
        private SpriteRenderer backSpriteRenderer;//Çarpştığında oluşacak renk efekti için
        private bool isCollided;//Çarpıştımı?
        private bool isDrag;//Sürüklenebilirmi
        private bool isJustInstianitate;//Şimdi oluşturuluyorsa ve uygunsuz bir yerdeyse gameobjeyi destroy etmek için
        private Vector3 resetPosition;//Uygunsuz bir yere yerleştirmeye çalışıldığında geri dönebilmek için
        private void Awake()
        {
            backSpriteRenderer = transform.Find("bg").GetComponent<SpriteRenderer>();//Childrendan backgroundu bulmak için
        }
        private void Update()
        {

            if (Input.GetMouseButtonDown(0))//Mouse sol tıkına ilk basılma anı
            {
                //On mouse down methodu sıkıntı çıkardığı için bunu kullanıyoruz
                //Mouse un deydiği tüm colliderleri topla
                var hits =
                    Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //Eğer UI üzerindeysem return
                if (EventSystem.current.IsPointerOverGameObject()) return;
                //Hits döngü
                foreach (var hit in hits)
                    if (hit.collider != null)
                        if (hit.transform == transform)
                        {
                            //sürükleme değişkeni true
                            isDrag = true;
                            resetPosition = transform.position;
                        }
            }
            //sürükleme aktif değilse geri dön
            if (!isDrag) return;

            if (Input.GetMouseButton(0))//Sol tıka basılı tutuluyorsa taşıma işlemlerini bitir
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Gridde hareket ediyomuş hissi için değere yuvarlama işlemi kullan
                transform.position = new Vector2(Mathematic.RoundToMultiple(pos.x, 0.32f)+0.16f,
                    Mathematic.RoundToMultiple(pos.y, 0.32f)+0.16f);
            }
            //Sol tık tan parmak kalkmadığı sürece buradan devam etme
            if (!Input.GetMouseButtonUp(0)) return;
            //sol tık a basıldıysa isDrag false yap
            isDrag = false;
            //
            if (isCollided)
            {
                //collided true ise ve yeni oluşturulan bir drag objeyse objeyi yoket
                if (isJustInstianitate)
                    Destroy(gameObject);
                transform.position = resetPosition;
            }
            else
            {
                isJustInstianitate = false;
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            //Collision oyun tahtamız veya Asker değilse
            if (collision.gameObject.name != "Board" && collision.gameObject.layer != 9)
            {
                //Kırmızı renk
                backSpriteRenderer.color = new Color(0.5f, 0, 0, 0.6f);
                isCollided = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            //Collision oyun tahtamız veya Asker değilse
            if (collision.gameObject.name != "Board" && collision.gameObject.layer != 9)
            {
                //Beyaz renk
                backSpriteRenderer.color = new Color(1, 1, 1, 0.3f);
                isCollided = false;
            }
        }
        /// <summary>
        /// Yeni oluşturma için ayarlama yapar
        /// </summary>
        public void SetForCreating()
        {
            isDrag = true;
            isJustInstianitate = true;
        }
    }
}