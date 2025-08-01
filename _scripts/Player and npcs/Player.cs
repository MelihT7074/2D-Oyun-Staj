using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Managers")]
    public GameDirector gameDirector;
    public CameraContorller cameraContorller;

    [Header("Can Ayarlarý")]
    public bool isAlive = true;
    public float hp;                //  Toplam Can
    public float currentHp;         //  Mecvut Can
    public bool hpRefillTimerStart; //  Alttaki Süreyi Aktif Etmek Ýçin, Dolum Baþlayýnca Kapanýyor, Hasar Alýnca Tekrar Aktif oluoyr
    public float hpRefillTimer;     //  Kaç Sn Boyunca Hasar Almazsak Can Doldurulacaðýný Belirlemek Ýçin
    public bool hpRefillStart;      //  Can Doldurmaya Baþlama Onayý, Hasar Alýnca Kapanýyor
    public float hpRefillAmount;    //  Can Dolum Miktarý
    public Image healtBar;          //  Can Barý
        
                                //  Bunlar Ýlerde Düþmanlara Geçmeli,   Þuanlýk Test Amaçlý Buradalar
    public float dmgTakenTime;      //  
    public float lastDmgTime = 0f;  //  

    [Header("Hareket Ayarlarý")]
    public Rigidbody2D rb;              //  Fiziksel Ýþlemlerin Yapýlýnabilinmesi Ýçin Bileþen, Ýnsan Ýskeleti, Kas Sistemi Gibi Birþey
    public float speed;
    public float jumpForce;

    public Directions direction;
    public bool isMoving =>     //  Get Set Kullanmanýn Kýsa Yolu, Fazla Bir Bilgim Yok Ama Aþþaðýdaki Ýþlemlere Göre Return Tarzý Deðer Gönderiyor
        Mathf.Abs(moveInput) > 0.01f && Mathf.Abs(rb.linearVelocity.x) > 0.05f && Mathf.Abs(rb.linearVelocity.y) > 0.05f;   //  Mathf.Abs : Mutlak Deðer Alma

    public float maxSpeed;          //  Karakterin Ulaþabileceði Max Hýz
    public float fallSpeedLimit;    //  Düþme Hýzý Sýnýrý
    public float jumpForceLimit;    //  Maksimum Zýplama Yüksekliði, Gereksiz Gibi Aslýnda

    public float moveInput = 0f;    //  linearVelocity Rigidbodynin Doðrusal Hýzýnýna Eriþmek Ýçin Kullanýlýyor, X Eksenindeki +- Sað-Soldaki Süratini, Ydeki Yukarý-Aþþaðýki Süratini Gösteriyor
                                        //  moveInputlada Yön Bilgisini Girip speedle Çarpýp Sürati Veriyoruz 
    public bool jumpRequested = false;
                                    //  Ayaðýn Yerden Temasýnýn Kesilmesinden Kýsa Bir Süre Sonra Zýplamayý Mümkün Kýlmak Ýçin Zamanalyýcý
    public float coyoteTime;
    public float coyoteTimeCounter;
                                    //  Yere Deðmeden Kýsa Bir Süre Önce Zýplamaya Basarsak Yere Deðidiðimiz Gibi Zýplatmasý Ýçin Zamanlayýcý
    public float jumpBufferTime;
    public float jumpBufferTimeCounter;

    [Header("Raycast ile Zemin Kontrolü")]      //  Karakterin Ayaðýndan Aþaðýya Doðru Kýsa Iþýnlar Göndererek Yere Temas Edip Etmediðini Anlamak Ýçin Sistem  
    public Transform leftRayOrigin;         //  Karakterin Sol Ayaðýndaki Iþýnýn Baþlangýç Konumu
    public Transform rightRayOrigin;        //  Karakterin Sað Ayaðýndaki Iþýnýn Baþlangýç Konumu
    public float rayLength;                 //  Iþýnlarýn Uzunluðu
    public LayerMask groundLayer;           //  Ýstenilen Temas Bölgesi

    public bool isGrounded = false;         //  Iþýnlardan Herhangibiri Deðidiðinde True Oluyor Ve Zýplayabiliyoruz
    public bool isFacingRight = false;      //  Karakterin Baktýðý Yön, Tersine Harekette Karakterin Boyutunu Tersine Çevirerek Baktýðý Yönü Deðiþtiriyoruz


    public enum Directions
    {   //   0    1    2      3     4
            Up, Down, Left, Right, None 
    }

    void Start()        //  Oyun Baþlayýp Herþey Hazýrlanýdðýnda Ýlk Çalýþan Method
    {
        rb = GetComponent<Rigidbody2D>();
        currentHp = hp;
    }

    public void RestartPlayer()
    {
        gameObject.SetActive(true);
        isAlive = true;

        currentHp = hp;             //

        rb = GetComponent<Rigidbody2D>();
        rb.position = new Vector2(2, 2);        //  Ýlerde Spawn noktalarý Oluþturup Oradan Seçmek Daha Ýyi olur
    }

    public void Death()
    {
        isAlive = false;
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damageCount)
    {
        currentHp -= damageCount;                           //  Hasar Miktarýna Göre Can Azaltýyor
        hpRefillStart = false;                              //  Hasar Alýnca Can Dolumunu Durdurmak Ýçin
        hpRefillTimer = 3;                                  //  Doldurma Zamanlayýcýsýný Sýfýrlýyor
        hpRefillTimerStart = true;                          //  Can Doldurma Süresini Aktifleþtiriyor
    }

    void Update()   //  Her Framede Çalýþan Kýsým, Fps'e Baðlý Olduðu Ýçin Pc nin Kalitesine Göre Çalýþma Sýklýðý Deðiþiyor, Girdi Alma, Fizik Dýþý Ýþlemler Ýçin fln kullanýlýr
    {
        if (Time.timeScale == 0) return;    //  Oyun Durduðunda Girdi Alýmlarýný Falan Kapatmak Ýçin Çünkü;
            //  !!  Update Pcnin Ürettiði Her Karede Çalýþtýðý Ýçin Oyun Zamanýnýn Durmasýndan Etkilenmiyor, Ýçindeki Time.deltaTime Kýsýmlarý Hariç Herþey Çalýþmaya Devam Ediyor
                        //  Time.deltaTime : 2 Frame Arasý Geçen Süreyi Hesaplar, Bu Süre Farkýný timeScalea Göre Verir, Buyüzden Zaman 0 Olunca Duruyor.
            //  !!  FixedUpdate Ýse Sistemin Ürettiði Zaman Aralýklarýyla Çalýþtýðý Ýçin TimeScale=0dan Etkileniyor, Ýindeki Herþey Duruyor

                                //  Yön Ve Haraket Girdileri
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f; 
            direction = Directions.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            direction = Directions.Right;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            direction = Directions.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction = Directions.Down;
        }
        else
        {
            if (isFacingRight) direction = Directions.Right;
            if (!isFacingRight) direction = Directions.Left;
        }

                                //  Zýplama Mekanikleri
        if (isGrounded) //  Yerle Temasta Ýken Sayaç Kapalý Ve Pozitif durumda, Zýplama Ýzinlerinden Biri Buranýn Pozitif Olmasý
        {
            coyoteTimeCounter = coyoteTime;
        }
        else            //  Yerle Temas Kesildiðinde Sayaç Baþlýyor, Negatife Düþerse Zýplama Avantajý Kayboluyor
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))    //  Zýplama Tuþuna Basýldýðýnda Sayaç Sýfýrlanýyor Ve Pozitifteyken Avantaj Saðlanýyor
        {
            jumpBufferTimeCounter = jumpBufferTime;
        }
        else                                    //  Sayaç Eksilmeye Baþlýyor Ve Pozitifte Kaldýðý Kýsa Süre Boyunca Zýplama Ýzinlerinden Diðerini Veriyor
        {
            if (jumpBufferTimeCounter >= -2f)   //  Fazla Eksilmesin Diye, Editörde Deðerin Sürekli Sonsuza Doðru Durmadan Düþtüðünü Görmek Sinirimi Bozuyor
            {
                jumpBufferTimeCounter -= Time.deltaTime;
            }
        }

        if (coyoteTimeCounter > 0f && jumpBufferTimeCounter > 0f)   //  Þartlar Saðlanýrsa Zýplama Onayý Ýþlemin Daha Doðru Yapýlamsý Ýçin FixedUpdate e Gönderiliyor
        {
            jumpRequested = true;
            jumpBufferTimeCounter = 0f; //  Sýfýrlanmassa Zýplama Süresi Gereksiz Artýyor
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)     //  Space Tuþu Býrakýldýðýnda Zýplamayý Kýsaltmak Ýçin Var
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);   //  Kýsa Bir Ýþlem Olduðu Ýçin Burada Yapmanýn Sakýncasý yok
            coyoteTimeCounter = 0f;     //  Doublejumpý Falan Önlemek Ýçin
        }


                                //  Can Ayarlarý
        if (currentHp <= hp)
        {
            if (hpRefillStart == true)                          //  Onay Gelirse Can Doldurma Ýþlemi Baþlýyor
            {
                currentHp += hpRefillAmount * Time.deltaTime;   //  Can Doldurma Ýþlemi
                healtBar.fillAmount = currentHp / hp;
            }
            healtBar.fillAmount = currentHp / hp;               //  Can Barýnýn Doluluk Oranýný Ayarlýyor
        }
        else
        {
            hpRefillStart = false;
            currentHp = hp;                                     //  Can Fullendikten Sonra Küsüratlý Bi Fazlalýk Oluyor, Onu Kýrpmak Ýçin
        }

        if (hpRefillTimerStart == true)                         //  Hasar Aldýktan Sonra Can Doldurmaya Baþlamak Ýçn
        {
            if (hpRefillTimer > 0)                              //  Sayacý Azaltma Yeri
            {
                hpRefillTimer -= Time.deltaTime;
            }
            else                                                //  Sayac Biterse Dolumu Baþlatma Yeri
            {
                hpRefillStart = true;                           //  Dolum Onayý
                hpRefillTimerStart = false;                     //  Sayacý Durdurup
                hpRefillTimer = 5;                              //  Süreyi Eski Haline getirme
            }
        }

        if (currentHp <= 0)
        {
            Death();        //  Ölüm
        }



        GroundCheckRaycast();
        FlipPlayerX();
    }

    private void FixedUpdate()  //  Sabit Aralýklarla Çalýþan kýsým, Sanýrým 0.02sn de Bir Çaðrýlýyor, Fiziksel Ýþlemler Yapmak Ýçin Daha Ýyi
    {
            
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);    //  Yatay Haraket,

        if (jumpRequested)      //  Zýplama
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
                                //  Hýz Sýnýrlamalarý
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);    //  Mathf.Clamp : Belirtilen Deðeri Sýnýrlar, Burasý Yatay Hýzý Sýnýrlýyor
        float clampedY = Mathf.Clamp(rb.linearVelocity.y, -fallSpeedLimit, jumpForceLimit);    // Burasýda Düþme Ve Zýplama Hýzýný Sýnýrlýyor

        rb.linearVelocity = new Vector2(clampedX, clampedY);
    }

    private void OnTriggerEnter2D(Collider2D collision)     //  Collider Bileþeni Üzerinden Rigidbodysiz Diðer Nesnelerle Etkileþime Girmek Ýçin
                                                            //  Collider Cisimlerin Etkileþim Alaný, Çarpýþmalar fln Burada Yapýlýyor, Ýnsanýn Derisi Gibi
    {
        if (collision.CompareTag("FallZone"))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 20);
        }
        if (collision.CompareTag("Deadzone"))
        {
            Death();
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("dmgWall"))      //  Hasar Test Kýsmý Þuanlýk        Ýncelemek Lazým
        {
            if (Time.time - lastDmgTime >= dmgTakenTime)
            {
                TakeDamage(45);

                lastDmgTime = Time.time;
            }

        }
    }


    private void GroundCheckRaycast()
    {
                    //  Iþýn Çekilirken: Baþlangýç Noktasý, Iþýnýn Yönü, Iþýnýn Uzunluðu, Temas Bölgesi
        bool leftHit = Physics2D.Raycast(leftRayOrigin.position, Vector2.down, rayLength, groundLayer);
        bool rightHit = Physics2D.Raycast(rightRayOrigin.position, Vector2.down, rayLength, groundLayer);

        isGrounded = leftHit || rightHit;

        // Editörde Gözükmesi Ýçin.                                     (Sorgu ? true : false) Kýsa Ýf Else Gibi
        Debug.DrawRay(leftRayOrigin.position, Vector2.down * rayLength, leftHit ? Color.green : Color.red);
        Debug.DrawRay(rightRayOrigin.position, Vector2.down * rayLength, rightHit ? Color.green : Color.red);
    
    }

    private void FlipPlayerX()
    {
        if ((moveInput > 0 || direction == Directions.Right)&& !isFacingRight)        //  Eðer Hýz Pozitif Yani Saða Doðruysa Ve Karakterde Sola Bakýyorsa Karakterin Yönünü Saða Çevirmek Ýçin 
        {
            FlipX();
        }
        else if ((moveInput < 0 || direction == Directions.Left) && isFacingRight)    //  Eðer Hýz Negatif Yani Sola Doðruysa Ve Karakterde Saða Bakýyorsa Karakterin Yönünü Sola Çevirmek Ýçin  
        {
            FlipX();
        }
    }

    private void FlipX()
    {
        isFacingRight = !isFacingRight;
        Vector3 ls = transform.localScale;      //  Karakterin Boyutu
        ls.x *= -1;                             //  Döndürmek Ýçin Boyutu X Ekseninde Ters Çeviriyoruz
        transform.localScale = ls;
    }


}
