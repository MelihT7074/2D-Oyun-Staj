using UnityEngine;

public class DamageWall : MonoBehaviour
{
    [Header("Managers")]
    public Player _player;
    public LevelManager levelManager;

    [Header("Components")]
    public BoxCollider2D bc;
    public Rigidbody2D rb;
    public SpriteRenderer sr;

    [Header("Properties")]
    public bool attackPlayer = false;   //  Sald�r� �zni
    public string dmgWallType;      //  Engelin Se�ilen T�r�
    public int dwIndex;             //  Olu�turulma Indexi, Nesne Yok Edildi�inde Listeden Silmek ��in
    public float dmgCount;          //  Verece�i Hasar
    public float dmgGivenTime;      //  Hasar Verme Aral���
    public float lastDmgTime = 0f;  //  Hasar Aral���n� �l�mek ��in Zamanlay�c�, Oyun S�resinden ��kar�l�nca Hasar Aral���ndan B�y�kse Hasar Veriyor


    public void ActivateDmgWall(Player player)
    {
        _player = player;

        levelManager = FindFirstObjectByType<LevelManager>();

        rb = GetComponent<Rigidbody2D>();       //  Fiziksel Nesnesine Ula�ma
        bc = GetComponent<BoxCollider2D>();     //  �arp��t�r�c�s�na Ula�ma
        sr = GetComponent<SpriteRenderer>();    //  D�� G�r�n�m�ne Ula�ma


        if (dmgWallType == "stationary")        //  Normal Engel
        {
            TypeStationary();
        }
        else if (dmgWallType == "laser")        //  ��inden Ge�inilinebilinecek Engel
        {
            TypeLaser();
        }
        else if (dmgWallType == "barrier")      //  Hareket Ettirebilece�imiz Engel
        {
            TypeBarrier();
        }
    }

    public void TypeStationary()
    {
        dmgCount = 25;
        dmgGivenTime = 1;

        sr.color = new Color(1, 0, 0, 1);           //  Rengini Ve Opakl���n� Ayarlama
        
        transform.localScale = new Vector2(0.5f, 2);    //  Boyutunu Ayarlama

        rb.bodyType = RigidbodyType2D.Kinematic;    //   Fizikten Pek Etkilenmez (Yer�ekimi, �arp��malar fln ��lemiyor), Sadece Kod �le Haraket Ettirilinebilinir
    }

    public void TypeLaser()
    {
        dmgCount = 10;
        dmgGivenTime = 0.5f;

        sr.color = new Color(1, 0, 0, 0.75f);

        transform.localScale = new Vector2(0.35f, 2f);

        rb.bodyType = RigidbodyType2D.Static;       //  Sabit Engel, Fizik Motorundan Hi� Etkilenmez
        
        bc.isTrigger = true;            //  Colliderdaki �arp��malar� Kapat�r Ve Sadece Temas Alg�lar, Birnevi Hayalet, Duman Tarz� Bir�eye d�n���r
    }   //  Rigidbody Dynamicse Yer�ekiminden Etkilenece�i Ve Art�k Di�er Nesnelerle �arp��mayaca�� i�in Cisimlerin ��inden Ge�erek A�a��ya Do�ru Sonusz D����e Ge�er

    public void TypeBarrier()
    {
        dmgCount = 30;
        dmgGivenTime = 20;

        sr.color = new Color(0.75f, 0, 0, 1);

        transform.localScale = new Vector2(1, 1);

        rb.bodyType = RigidbodyType2D.Dynamic;     //   Fiziksel ��lemlerden Tamamen Etkilenen T�r
        rb.mass = 25;                           //  K�tlesi

        bc.size = new Vector2(0.90f, 0.90f);    //  Collider�n Boyutu, Dar Alanlardan Daha Rahat Ge�mesi ��in Biraz K���ltt�m
    }

    private void Update()
    {
        if (attackPlayer == true && Time.time - lastDmgTime >= dmgGivenTime)
        {
            _player.TakeDamage(dmgCount);

            lastDmgTime = Time.time;

            Debug.Log("Hasar Verildi : " + dmgWallType + " : " + Time.time);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            attackPlayer = true;

            //  Oyun �lk A��ld���nda Direkt Kutuya Temas Edersek Oyun Vakti Daha Hasar Aral��� S�resini Ge�medi�i ��in Hasar Vermiyordu, Buda Tek Seferlik �zin
            if (dmgWallType == "barrier" && Time.time < dmgGivenTime && lastDmgTime == 0)
            {
                _player.TakeDamage(dmgCount);

                lastDmgTime = Time.time;

                Debug.Log("�lk Hasar� Verildi : " + dmgWallType + " : " + Time.time);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            attackPlayer = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            attackPlayer = true;
        }

        if (collision.CompareTag("Deadzone"))               //  Nesneyi Mapden D��erse Yok Etme Yeri
        {
            Destroy(gameObject);        // Siliyor

            levelManager.lst_dmgWalls.RemoveAt(dwIndex);    //  Listedende ��kar�yor, Ama ��kar�ld�ktan Sonra Nesnenin �st�ndeki Nesnelerin �ndexi 1 Azal�yor

            foreach (var d in levelManager.lst_dmgWalls)    //  Burdada Azalanlar� Bulup Nesneye Atanan Indexide Azalt�yoruzki Listeyle E�le�ebilsin
            {
                if (d.dwIndex > dwIndex)
                {
                    d.dwIndex -= 1;
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            attackPlayer = false;
        }
    }

}
