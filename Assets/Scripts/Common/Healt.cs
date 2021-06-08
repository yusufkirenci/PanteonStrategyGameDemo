using System.Collections;
using System.Linq;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.GamePlay
{
    public class Healt : MonoBehaviour
    {
        private Animator animator; // Karakter animasyonları için animasyon komponenti

        [Header("Healt")] [SerializeField] //Canını editörden ayarlayabilmemiz için
        private int healtPoint; //

        [HideInInspector] //Bunu inspecterda görüntülememize ihtiyaç yok
        public int startHealt; //UImızda bir bar olduğu için buna ihtiyacımız var

        public int HealtPoint
        {
            get => healtPoint;
            set
            {
                //Eğer bu bir karakterse ve can düşerse animasyon oynat
                if (animator != null && value < healtPoint) animator.Play("hurt");
                //Eğer bir gameobje seçilyse ve canımız düşerse information menüyü güncelleyerek bunu görüyoruz
                if (GameManager.Instance.SelectedGameObjects.Any())
                    InformationMenu.Instance.UpdateInfoScreen(GameManager.Instance.SelectedGameObjects);
                //Propertymizin gerekliliğini yerine getiriyoruz
                healtPoint = value;
                //Canımız 0 dan büyükse buradan ileri gitmiyoruz
                if (healtPoint > 0) return;
                //Objemiz bir anda yokolup gittiğinde güzel bir görüntü olmadığından Coroutine ile basit bir animasyo veriyoruz
                StartCoroutine(ScaleDownAnimation());
                //Eğer öldüyse tekrar güncelliyoruz
                if (GameManager.Instance.SelectedGameObjects.Any())
                    InformationMenu.Instance.UpdateInfoScreen(GameManager.Instance.SelectedGameObjects);
            }
        }

        private void Awake()
        {
            // Childrendan animator komponentini buluyoruz
            animator = GetComponentInChildren<Animator>();
            startHealt = healtPoint;
        }

        private IEnumerator ScaleDownAnimation() //Coroutune örneği
        {
            //Coroutune her başa sardığında 0.1 oranında objemizi küçültüyoruz
            transform.localScale = transform.localScale - new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSeconds(0.05f);
            //transform.localScale.x < 0.1f olduğunda objemizi yok ediyoruz tersi durumda sar başa
            if (transform.localScale.x < 0.1f)
                Destroy(gameObject);
            else
                StartCoroutine(ScaleDownAnimation());
        }
    }
}