using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Managers")]
    public GameDirector gameDirector;
    public CameraContorller cameraContorller;

    [Header("Can Ayarlarý")]
    public bool isAlive = true;

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

    }

    void Update()   //  Her Framede Çalýþan Kýsým, Fps'e Baðlý Olduðu Ýçin Pc nin Kalitesine Göre Çalýþma Sýklýðý Deðiþiyor, Girdi Alma, Fizik Dýþý Ýþlemler Ýçin fln kullanýlýr
    {
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

            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);  // 


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
            gameObject.SetActive(false);            //  Ölüm Methodu Çaðrýlmalý
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
