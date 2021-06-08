using UnityEngine;

namespace Assets.Scripts.Common
{
    public class Unit : MonoBehaviour
    {
        //Bunu base class olarak kullanmak isterdim ama
        //maalesef unity base classtan componente erişim sağlayamıyor
        //Unitlerin ortak özelliklerini tutuyor

        public new string name;
        public Sprite unitImage;
    }
}