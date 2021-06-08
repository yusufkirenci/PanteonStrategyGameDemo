using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GamePlay;
using Assets.Scripts.PathFinding;
using UnityEngine;
using Grid = Assets.Scripts.PathFinding.Grid;

namespace Assets.Scripts.Soldier
{
    public class SoldierController : MonoBehaviour
    {

        [SerializeField]
        [Header("Target")]
        private Transform target; //Saldırmak için veya Konuma gitmek için hedef

        [Header("Attack")] 
        public float attackDistance = 0.32f; //Saldırmak için minimum Mesafe
        public float damageIntervalSeconds = 0.5f; //Vurma zaman aralığı
        public int damagePerAttack = 10; //Saldırı başına hasar
        public bool isAttack;

        [Header("Move")] 
        public float moveSpeed = 0.1f; // Hareket Hızı
        public float smoothDamp = 7f;

        //Private variables
        private Animator animator; // Karakter animasyonları için animasyon komponenti
        private float attackTimer;
        private SpriteRenderer spriteRenderer; //Karakter yönü için Sprite renderer
        private PathFinder pathFinder; // Yol bulmak için  komponent
        private List<Tile> path; //hedefe varmak için izlenilecek yol
        private float nextActionTime;
        private Tile lastTile; // Sıradaki kutum
        public Transform Target
        {
            get => target;
            set
            {
                if (value != transform) target = value;
                isAttack = false;
            }
        }

        private void Awake()
        {
            // Childrendan animator komponentini bul
            animator = GetComponentInChildren<Animator>();
            if (animator == null) Debug.LogError("Animator not found in Character children!");
            // Childrendan pathFinder komponentini bul
            pathFinder = GetComponent<PathFinder>();
            if (pathFinder == null) Debug.LogError("PathFinder not found in Character!");
            // Childrendan spriteRenderer komponentini bul
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null) Debug.LogError("spriteRenderer not found in Character children!");
        }
        private void Update()
        {
            //Hedefimiz yoksa olduğumuz yerde bekliyelim
            if (!target) return;
            // Path yoksa veya lasttile yoksa iki değişkeni set ediyorz
            if (path == null || lastTile == null)
            {
                path = pathFinder.FindPath(transform, target);
                lastTile = path.FirstOrDefault();
            }

            if (isAttack)
            {
                //Bizimle temas eden objeleri buluyoruz
                var overlapedColliders = Physics2D.OverlapCircleAll(transform.position, attackDistance);

                foreach (var collider in overlapedColliders)
                    if (collider.transform == target)//Objelerin hepsini tek tek gezerek hedefimizmi diye kontrol ediyoruz
                    {
                        //Belli bir intervalde saldırı gerçekleştiriyoruz
                        if (Time.time > nextActionTime)
                        {
                            nextActionTime = damageIntervalSeconds + Time.time;
                            animator.Play("slash");
                            target.GetComponent<Healt>().HealtPoint -= damagePerAttack;
                        }

                        lastTile = null;
                    }
            }

            // Last tile null ise duruyoruz
            if (lastTile != null)
            {
                var lastTilePosition = lastTile.worldPosition;
                //Look at kodu
                var difference = lastTilePosition - transform.position;
                difference.Normalize();
                var rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                // Lerp ile yumuşak bir dönüş sağlıyoruz
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, rotZ - 90),
                    10f * Time.deltaTime);
                // Hareket sağlıyoruz (Time.deltaTime kare hızından bağımsız çalışmayı sağlıyor)
                transform.Translate(0, Time.deltaTime * moveSpeed, 0);
                animator.Play("run"); //Koşma animasyonunu oynatıyoruz
                //Sıradaki tilemız bize yeterince yakınsa diğer tile a geçmek için null yapıyoruz
                if (Vector2.Distance(transform.position, lastTilePosition) <= moveSpeed / 5)
                    //Her seferinde path i yeniden düzenliyoruz ki önümüze yeni bir engel çıktığında bilelim
                    lastTile = null;
            }

            //Karakterin sprite yönünü bakış yönüne göre sağa veya sola çeviriyoruz.    
            spriteRenderer.flipY = transform.rotation.eulerAngles.z <= 180 && transform.rotation.eulerAngles.z >= 0;
        }
        private void OnDrawGizmos()
        {
            if (path == null) return;
            for (var i = 1; i < path.Count; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(path[i - 1].worldPosition, path[i].worldPosition);
            }

            var grid = Grid.Instance.CreateGrid();
            foreach (var x in grid)
            foreach (var y in x)
            {
                Gizmos.color = Color.blue;
                if (y.isWall)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    Gizmos.DrawCube(y.worldPosition,
                        new Vector3(Grid.Instance.tileRadius, Grid.Instance.tileRadius, Grid.Instance.tileRadius));
                }

                Gizmos.DrawWireCube(y.worldPosition,
                    new Vector3(Grid.Instance.tileRadius, Grid.Instance.tileRadius, Grid.Instance.tileRadius));
            }
        }
    }
}