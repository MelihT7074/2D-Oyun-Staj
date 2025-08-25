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
    public bool attackPlayer = false;   //  Saldýrý Ýzni
    public string dmgWallType;      //  Engelin Seçilen Türü
    public int dwIndex;             //  Oluþturulma Indexi, Nesne Yok Edildiðinde Listeden Silmek Ýçin
    public float dmgCount;          //  Vereceði Hasar
    public float dmgGivenTime;      //  Hasar Verme Aralýðý
    public float lastDmgTime = 0f;  //  Hasar Aralýðýný Ölçmek Ýçin Zamanlayýcý, Oyun Süresinden Çýkarýlýnca Hasar Aralýðýndan Büyükse Hasar Veriyor


    public void ActivateDmgWall(Player player)
    {
        _player = player;

        levelManager = FindFirstObjectByType<LevelManager>();

        rb = GetComponent<Rigidbody2D>();       //  Fiziksel Nesnesine Ulaþma
        bc = GetComponent<BoxCollider2D>();     //  Çarpýþtýrýcýsýna Ulaþma
        sr = GetComponent<SpriteRenderer>();    //  Dýþ Görünümüne Ulaþma


        if (dmgWallType == "stationary")        //  Normal Engel
        {
            TypeStationary();
        }
        else if (dmgWallType == "laser")        //  Ýçinden Geçinilinebilinecek Engel
        {
            TypeLaser();
        }
        else if (dmgWallType == "barrier")      //  Hareket Ettirebileceðimiz Engel
        {
            TypeBarrier();
        }
    }

    public void TypeStationary()
    {
        dmgCount = 25;
        dmgGivenTime = 1;

        sr.color = new Color(1, 0, 0, 1);           //  Rengini Ve Opaklýðýný Ayarlama
        
        transform.localScale = new Vector2(0.5f, 2);    //  Boyutunu Ayarlama

        rb.bodyType = RigidbodyType2D.Kinematic;    //   Fizikten Pek Etkilenmez (Yerçekimi, Çarpýþmalar fln Ýþlemiyor), Sadece Kod Ýle Haraket Ettirilinebilinir
    }

    public void TypeLaser()
    {
        dmgCount = 10;
        dmgGivenTime = 0.5f;

        sr.color = new Color(1, 0, 0, 0.75f);

        transform.localScale = new Vector2(0.35f, 2f);

        rb.bodyType = RigidbodyType2D.Static;       //  Sabit Engel, Fizik Motorundan Hiç Etkilenmez
        
        bc.isTrigger = true;            //  Colliderdaki Çarpýþmalarý Kapatýr Ve Sadece Temas Algýlar, Birnevi Hayalet, Duman Tarzý Birþeye dönüþür
    }   //  Rigidbody Dynamicse Yerçekiminden Etkileneceði Ve Artýk Diðer Nesnelerle Çarpýþmayacaðý için Cisimlerin Ýçinden Geçerek Aþaðýya Doðru Sonusz Düþüþe Geçer

    public void TypeBarrier()
    {
        dmgCount = 30;
        dmgGivenTime = 20;

        sr.color = new Color(0.75f, 0, 0, 1);

        transform.localScale = new Vector2(1, 1);

        rb.bodyType = RigidbodyType2D.Dynamic;     //   Fiziksel Ýþlemlerden Tamamen Etkilenen Tür
        rb.mass = 25;                           //  Kütlesi

        bc.size = new Vector2(0.90f, 0.90f);    //  Colliderýn Boyutu, Dar Alanlardan Daha Rahat Geçmesi Ýçin Biraz Küçülttüm
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

            //  Oyun Ýlk Açýldýðýnda Direkt Kutuya Temas Edersek Oyun Vakti Daha Hasar Aralýðý Süresini Geçmediði Ýçin Hasar Vermiyordu, Buda Tek Seferlik Ýzin
            if (dmgWallType == "barrier" && Time.time < dmgGivenTime && lastDmgTime == 0)
            {
                _player.TakeDamage(dmgCount);

                lastDmgTime = Time.time;

                Debug.Log("Ýlk Hasarý Verildi : " + dmgWallType + " : " + Time.time);
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

        if (collision.CompareTag("Deadzone"))               //  Nesneyi Mapden Düþerse Yok Etme Yeri
        {
            Destroy(gameObject);        // Siliyor

            levelManager.lst_dmgWalls.RemoveAt(dwIndex);    //  Listedende Çýkarýyor, Ama Çýkarýldýktan Sonra Nesnenin Üstündeki Nesnelerin Ýndexi 1 Azalýyor

            foreach (var d in levelManager.lst_dmgWalls)    //  Burdada Azalanlarý Bulup Nesneye Atanan Indexide Azaltýyoruzki Listeyle Eþleþebilsin
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
