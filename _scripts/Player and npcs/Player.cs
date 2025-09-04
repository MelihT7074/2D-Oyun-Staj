using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Managers")]
    public GameDirector gameDirector;
    public CameraContorller cameraContorller;
    public KeybindingManager KBM;

    [Space]
    [Header("Can Ayarlarý")]
    public GameObject SpawnPos;
    public bool isAlive = true;
    public float hp;                //  Toplam Can
    public float currentHp;         //  Mecvut Can
    public bool hpRefillTimerStart; //  Alttaki Süreyi Aktif Etmek Ýçin, Dolum Baþlayýnca Kapanýyor, Hasar Alýnca Tekrar Aktif oluoyr
    public float hpRefillTimer;     //  Kaç Sn Boyunca Hasar Almazsak Can Doldurulacaðýný Belirlemek Ýçin
    public bool hpRefillStart;      //  Can Doldurmaya Baþlama Onayý, Hasar Alýnca Kapanýyor
    public float hpRefillAmount;    //  Can Dolum Miktarý
    public Image healtBar;          //  Can Barý

    //[Header("Yedekler")]    //  
    private float hpClon;
    private float hpRefillTimerClon;
    private float borderDmgStartTimerClon;

    [Space]
    [Header("Hasar Efektleri")]
    public bool isPlayerDmgTaken;       //  Hasar Aldýðýmýzýn Bilgisi
    public Image img_DmgEffect;         //  Hasar Efekti Görseli, Hasar Alýnca Opaklýðý Fulleniyor Sonra Azalmaya Baþlýyor, Opaklýk Miktarýna Göre Diðer Tuþ Girdilerini Flnda Kilitliyorum
    public bool isDmgImgActive;         //  Görselin Opaklýðýný Düþürmek Ýçin Onay Veriyor
    public float dmgPunchPower;         //  Savrulma Miktarý, Düzgün Deðerleri Bulmak Zor


    [Space]
    [Header("Hareket Ayarlarý")]
    public Rigidbody2D rb;              //  Fiziksel Ýþlemlerin Yapýlýnabilinmesi Ýçin Bileþen, Ýnsan Ýskeleti, Kas Sistemi Gibi Birþey
    public bool showVelocity;
    public float speed;
    public float jumpForce;

    public Directions direction;
    public Vector2 directionInputs;
    public bool isFacingRight = false;      //  Karakterin Baktýðý Yön, Tersine Harekette Karakterin Boyutunu Tersine Çevirerek Baktýðý Yönü Deðiþtiriyoruz
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

    [Space]     //  Karaker Zýplayýp Duvara Temas Ederse Ve Duvar Yönünde Ýlerlemeye Çalýþýrsa Havada Asýlý Kalýyordu, Bu Onu Birazcýk Fixliyor Ve Yeni Mekanikler Ekliyor
    [Header("Duvardan Kayma")]                  //  Asýl Fixleyen Þey Karakterin Colliderýna Sürtünmesiz Bir Materyal Vermek Oldu
    public Transform wallCheck;
    public float lapLength;
    public LayerMask wallAreas;

    public bool isWallSliding = false;
    public float wallSlidingSpeed;

    [Space]
    [Header("Duvardan Zýplama")]
    public bool isWallJumping;          //  Zýplama Onayý Ve Diðer Hareket Girdilierini Çakýþma Olmamasý Ýçin Kapatma,
    public float wallJumpingDirection;  //  Zýplamanýn Yönü, Duvara Ters Yani Oyuncunun Baktýðý Yönün Zýttý Olamlý
    public float wallJumpingTime;       
    public float wallJumpingCounter;    
    public float wallJumpingDuration;   
    public Vector2 walljumpingPower;    //  X ve Y Ekseninde Uygulanacak Güç

    [Space]
    [Header("Raycast ile Zemin Kontrolü")]      //  Karakterin Ayaðýndan Aþaðýya Doðru Kýsa Iþýnlar Göndererek Yere Temas Edip Etmediðini Anlamak Ýçin Sistem  
    public Transform leftRayOrigin;         //  Karakterin Sol Ayaðýndaki Iþýnýn Baþlangýç Konumu
    public bool leftHit;
    public Transform middleRayOrigin;       //  Karakterin Ortasýndaki Iþýnýn Baþlangýç Konumu,     Ýnce Nesnelerin Üstündeyken Atlamasýný Kolaylaþtýrmak Ýçin
    public bool middleHit;
    public Transform rightRayOrigin;        //  Karakterin Sað Ayaðýndaki Iþýnýn Baþlangýç Konumu
    public bool rightHit;

    public float rayLength;                 //  Iþýnlarýn Uzunluðu
    public LayerMask jumpAreas;             //  Ýstenilen Temas Bölgeleri,  Engellerinde Üstünden Zýplayabilmesi için

    public bool isGrounded = false;         //  Iþýnlardan Herhangibiri Deðidiðinde True Oluyor Ve Zýplayabiliyoruz

    [Space]
    [Header("Map Sýnýrý Hasar Ayarlarý")]
    public GameObject borderWarning;
    public Text txt_leaveTimer;
    public bool isBorderWarning;            //  Ekraný Aktifleþtirme Ve Kontrolü
    public int fallLoopCount;               //  Düþme Döngüsüne Girersek Uyarýyý Göstermek Ýçin
    public bool onBorder = false;           //  Map Sýnýrýndan Çýkarsak Uyarýyý Göstermek Ýçin
    public float borderDmgStartTimer = 5;   //  Hasar Almaya Baþlamadan Önceki Süre
    public float borderDmgTakenTime;        //  Hasar Alma Aralýðý
    public float lastBorderDmgTakenTime;    //  Hasar Aldýðýmýzdaki Süre, Oyun Süresinden Çýkartýnca Hasar Aralýðýndan Fazla Ýse Hasar Alýyoruz


    public enum Directions
    {   //   0    1    2      3     
            Up, Down, Left, Right,
    }

    void Start()        //  Oyun Baþlayýp Herþey Hazýrlanýdðýnda Ýlk Çalýþan Method
    {
        PlayerAbilitysClon();
    }

    public void RestartPlayer()
    {
        gameDirector.escAndOptions.deathScreen.SetActive(false);

        gameObject.SetActive(true);
        isAlive = true;

        gameDirector.GD_SwitchMusic();      //  Tekrar Canlanýnca Normal Oyun Müziðine Geçmek Ýçin

        currentHp = hpClon;

        rb = GetComponent<Rigidbody2D>();
        rb.position = new Vector2(SpawnPos.transform.position.x, SpawnPos.transform.position.y + 1);
    }

    public void PlayerAbilitysClon()    //  Deðerlerin Yedeklerini Alma
    {
        hpClon = hp;
        hpRefillTimerClon = hpRefillTimer;
        borderDmgStartTimerClon = borderDmgStartTimer;
    }

    public void Death()
    {
        DamageEffectStart();    //  

        isAlive = false;
        gameObject.SetActive(false);

        gameDirector.GD_SwitchMusic();      //  Ölüm Ekraný Müziðine Geçmek Ýçin

                //  Hasar Alarak Öldüysek Etkileri Sýfýrlama yeri
        isPlayerDmgTaken = false;
        img_DmgEffect.color = new Color(255, 255, 255, 0);
                //  Uyarý Ekranýyla Öldüysek Onun Etkilerini Kapatmak Ýçin
        fallLoopCount = 0;
        onBorder = false;
    }

    public void TakeDamage(float damageCount)
    {
        currentHp -= damageCount;                   //  Hasar Miktarýna Göre Can Azaltýyor
        isPlayerDmgTaken = true;      //**
        hpRefillStart = false;                      //  Hasar Alýnca Can Dolumunu Durdurmak Ýçin
        hpRefillTimer = hpRefillTimerClon;          //  Doldurma Zamanlayýcýsýný Sýfýrlýyor
        hpRefillTimerStart = true;                  //  Can Doldurma Süresini Aktifleþtiriyor
    }

    public void DamageEffectStart()
    {
        isPlayerDmgTaken = false;
        SoundManager.Instance.PlayMusic("PlayerDamageSound");   //  Ses Efekti
        img_DmgEffect.color = Color.white;                      //  Görseli Aktifleþtiriyor
        isDmgImgActive = true;
    }

    public void DmgScreenCheck()
    {
        if (isDmgImgActive)
        {
            img_DmgEffect.color = new Color(255, 255, 255, img_DmgEffect.color.a - Time.deltaTime * 2);

            if (img_DmgEffect.color.a <= 0)
            {
                isDmgImgActive = false;
            }
        }
    }

    void Update()   //  Her Framede Çalýþan Kýsým, Fps'e Baðlý Olduðu Ýçin Pc nin Kalitesine Göre Çalýþma Sýklýðý Deðiþiyor, Girdi Alma, Fizik Dýþý Ýþlemler Ýçin fln kullanýlýr
    {
        if (Time.timeScale == 0) return;    //  Oyun Durduðunda Girdi Alýmlarýný Falan Kapatmak Ýçin Çünkü;
                                            //  !!  Update Pcnin Ürettiði Her Karede Çalýþtýðý Ýçin Oyun Zamanýnýn Durmasýndan Etkilenmiyor, Ýçindeki Time.deltaTime Kýsýmlarý Hariç Herþey Çalýþmaya Devam Ediyor
                                            //  Time.deltaTime : 2 Frame Arasý Geçen Süreyi Hesaplar, Bu Süre Farkýný timeScalea Göre Verir, Buyüzden Zaman 0 Olunca Duruyor.
                                            //  !!  FixedUpdate Ýse Sistemin Ürettiði Zaman Aralýklarýyla Çalýþtýðý Ýçin TimeScale=0dan Etkileniyor, Ýindeki Herþey Duruyor

                                    //  Yön Ve Haraket Girdileri
        if (img_DmgEffect.color.a <= 0.5f)  
        {
            moveInput = 0f;

            if (Input.GetKey(KBM.GetKey("MoveLeft")) || Input.GetKey(KBM.GetKey("MoveLeft_Sc")))
            {
                moveInput = -1f;
                direction = Directions.Left;
                directionInputs = new Vector2(-1, 0);

                if (showVelocity)               //  Hareket, Yön, Velocity Kontrolleri //
                { 
                    Debug.Log("Yatay Hareket Girdisi (Sol) : " + (moveInput > 0 ? "1 (Sað)" : "-1 (Sol)") + ", velocity : " + rb.linearVelocity);
                }
            }
            else if (Input.GetKey(KBM.GetKey("MoveRight")) || Input.GetKey(KBM.GetKey("MoveRight_Sc")))
            {
                moveInput = 1f;
                direction = Directions.Right;
                directionInputs = new Vector2(1, 0);

            if (showVelocity)               //  Hareket, Yön, Velocity Kontrolleri //
            {
                Debug.Log("Yatay Hareket Girdisi (Sað) : " + (moveInput > 0 ? "1 (Sað)" : "-1 (Sol)") + ", velocity : " + rb.linearVelocity);
            }
            }
            else if (Input.GetKey(KBM.GetKey("LookUp")) || Input.GetKey(KBM.GetKey("LookUp_Sc")))
            {
                direction = Directions.Up;
                directionInputs = new Vector2(directionInputs.x, 1);
            }
            else if (Input.GetKey(KBM.GetKey("LookDown")) || Input.GetKey(KBM.GetKey("LookDown_Sc")))
            {
                direction = Directions.Down;
                directionInputs = new Vector2(directionInputs.x, -1);
            }
            else
            {
                if (isFacingRight)
                {
                    direction = Directions.Right;
                    directionInputs = new Vector2(1, 0);
                }
                else if (!isFacingRight)
                {
                    direction = Directions.Left;
                    directionInputs = new Vector2(-1, 0);
                }
            }
        }
                                    //  Zýplama Mekanikleri
        if (isGrounded) //  Yerle Temasta Ýken Sayaç Kapalý Ve Pozitif durumda, Zýplama Ýzinlerinden Biri Buranýn Pozitif Olmasý
        {
            coyoteTimeCounter = coyoteTime;
            fallLoopCount = 0;      //  Düþme Döngüsü Bitiyor
        }
        else            //  Yerle Temas Kesildiðinde Sayaç Baþlýyor, Negatife Düþerse Zýplama Avantajý Kayboluyor
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KBM.GetKey("Jump")))   //  Zýplama Tuþuna Basýldýðýnda Sayaç Sýfýrlanýyor Ve Pozitifteyken Avantaj Saðlanýyor
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

        if (Input.GetKeyUp(KBM.GetKey("Jump")) && rb.linearVelocity.y > 0f)   //  Space Tuþu Býrakýldýðýnda Zýplamayý Kýsaltmak Ýçin Var
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
                hpRefillTimer = hpRefillTimerClon;              //  Süreyi Eski Haline getirme
            }
        }

        if (currentHp <= 0)
        {
            Death();        //  Ölüm
        }

                                    //  Map Sýnýrýný Geçince Veya Düþme Döngüsüne Girince Uyarý Ve Hasar Alma
        if ((onBorder || fallLoopCount > 10) && isAlive)
        {
            isBorderWarning = true;
            borderWarning.SetActive(true);

            if (borderDmgStartTimer >= 0)
            {
                borderDmgStartTimer -= Time.deltaTime;

                txt_leaveTimer.text = borderDmgStartTimer.ToString("0");    //  Tam Sayý olarak Yazdýrýlýyor, Belli Bir Küsürat Ýçin = "0.###"
            }
            else
            {
                if (Time.time - lastBorderDmgTakenTime >= borderDmgTakenTime)
                {
                    TakeDamage(15);

                    lastBorderDmgTakenTime = Time.time;
                }
                txt_leaveTimer.text = "!!!     !!!".ToString();
            }
        }
        else
        {
            isBorderWarning = false;
            borderWarning.SetActive(false);

            borderDmgStartTimer += Time.deltaTime;

            if (borderDmgStartTimer >= borderDmgStartTimerClon)
            {
                borderDmgStartTimer = borderDmgStartTimerClon;  //  Fazlalýk Küsüratý Kýrpma
            }
        }

        WallSlide();
        WallJumping();

                            
        if (isPlayerDmgTaken)   //  Hasar Efektleri
        {
            if (!isBorderWarning)
            {
                moveInput = -directionInputs.x * dmgPunchPower;     //  Doðru Yermi Emin Deðilim, Etkisi Kýsa Sürüyor
                        //  moveInput Tuþa Basýlmadýðýnda 0 Olduðu Ve Yatay Hareketde Doðrudan Buna Baðlý Olduðu Ýçin Ýyice Kýsalýyordu,
                                //  Hasar Ekraný Opaklýðýna Göre Oradaki Girdileri Kapatýnca Durum Ýyileþti Gibi
            }
            DamageEffectStart();
        }
        DmgScreenCheck();

        GroundCheckRaycast();

        if (!isWallJumping && img_DmgEffect.color.a <= 0.25f)
        {
            FlipPlayerX();
        }
    }

    private void LateUpdate()
    {
        if (onBorder || fallLoopCount > 10)
        {
            borderWarning.transform.position = new Vector2(transform.position.x, transform.position.y);
        }
    }

    private void FixedUpdate()  //  Sabit Aralýklarla Çalýþan kýsým, Sanýrým 0.02sn de Bir Çaðrýlýyor, Fiziksel Ýþlemler Yapmak Ýçin Daha Ýyi
    {
        if (!isWallJumping)
        {

            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);    //  Yatay Haraket

            if (isWallSliding)      //  Kayma Þartlarý Gerçekleþtiyse x Eksenindeki Hareketi Kýsýtlayýp Yavaþça Aþaðýya Doðru Kayýyoruz
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));    //**

                if (showVelocity)               //  Hareket, Yön, Velocity Kontrolleri //
                {
                    Debug.Log("Kayma Gerçekleþti, velocity : " + rb.linearVelocity);
                }
            }

            if (jumpRequested)      //  Zýplama
            {
                SoundManager.Instance.PlayMusic("JumpSound");           //  Ses Efekti

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                if (showVelocity)               //  Hareket, Yön, Velocity Kontrolleri //
                {
                    Debug.Log("Zýplama Gerçekleþti, velocity : " + rb.linearVelocity);
                }

                jumpRequested = false;
            }
            //  Hýz Sýnýrlamalarý
            float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);    //  Mathf.Clamp : Belirtilen Deðeri Sýnýrlar, Burasý Yatay Hýzý Sýnýrlýyor
            float clampedY = Mathf.Clamp(rb.linearVelocity.y, -fallSpeedLimit, jumpForceLimit);    // Burasýda Düþme Ve Zýplama Hýzýný Sýnýrlýyor

            rb.linearVelocity = new Vector2(clampedX, clampedY);
        }
        if (showVelocity)               //  Hareket, Yön, Velocity Kontrolleri //
        {
            Debug.Log("Player velocity : " + rb.linearVelocity);
        }
    }

    private void WallJumping()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -directionInputs.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KBM.GetKey("Jump")) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;

            SoundManager.Instance.PlayMusic("WallJumpSound");           //  Ses Efekti

            rb.linearVelocity = new Vector2(wallJumpingDirection * walljumpingPower.x, walljumpingPower.y);
            wallJumpingCounter = 0f;

            if (showVelocity)               //  Hareket, Yön, Velocity Kontrolleri //
            {
                Debug.Log("Player direction : " + (transform.localScale.x > 0 ? "1 (Sol)" : "-1 (Sað)") +
                    ", WallJump direction : " + (wallJumpingDirection > 0 ? "1 (Sað)" : "-1 (Sol)") +
                    ", velocity : " + rb.linearVelocity);
            }

            if (transform.localScale.x == wallJumpingDirection)
            {
                FlipX();
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);

            if (showVelocity)               //  Hareket, Yön, Velocity Kontrolleri //
            {
                Debug.Log("Duvardan Zýplama Gerçekleþti, velocity : " + rb.linearVelocity);
            }

        }

        if (moveInput == wallJumpingDirection)      //  Kýsa Sýçramalar Yapmak Ýçin
        {
            StopWallJumping();
        }
    }

    private void StopWallJumping()  //  Invoke Ýle Belli Bir Zaman Sonra Çaðýrmak Ýçin Ayrý Methodda
    {
        isWallJumping = false;
    }

    private void GroundCheckRaycast()
    {
        //  Iþýn Çekilirken: Baþlangýç Noktasý, Iþýnýn Yönü, Iþýnýn Uzunluðu, Temas Bölgesi
        leftHit     = Physics2D.Raycast(leftRayOrigin.position, Vector2.down, rayLength, jumpAreas);
        middleHit   = Physics2D.Raycast(middleRayOrigin.position, Vector2.down, rayLength, jumpAreas);
        rightHit    = Physics2D.Raycast(rightRayOrigin.position, Vector2.down, rayLength, jumpAreas);

        isGrounded = leftHit || middleHit || rightHit;
    }

    private void WallSlide()    //  Sadece Duvarla Temas Halindeysek Ve Duvara Doðru Hareket Etmeye Çalýþýyorsak Yavaþça Kaydýrmak Ýçin Onay Veriyor
    {
        bool touchingWall = Physics2D.OverlapCircle(wallCheck.position, lapLength, wallAreas);  //  Raycaste Benzer Mantýðý Var, Bu Daire Þeklinde Çiziyor, Raycestse Düz Çizgi Olarak çiziyor

        isWallSliding = touchingWall && !isGrounded && moveInput != 0;
    }

    private void FlipPlayerX()
    {
        if ((moveInput > 0 || direction == Directions.Right) && !isFacingRight)         //  Eðer Hýz Pozitif Yani Saða Doðruysa Ve Karakterde Sola Bakýyorsa Karakterin Yönünü Saða Çevirmek Ýçin 
        {
            FlipX();
        }
        else if ((moveInput < 0 || direction == Directions.Left) && isFacingRight)      //  Eðer Hýz Negatif Yani Sola Doðruysa Ve Karakterde Saða Bakýyorsa Karakterin Yönünü Sola Çevirmek Ýçin  
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
                                        //  Can Barýnýn Yönünü Sabit Kýlmak Ýçin
        Vector3 hpls = healtBar.transform.localScale;
        hpls.x *= -1;
        healtBar.transform.localScale = hpls;
    }

    private void OnDrawGizmos()     //  Çizgileri Editörde Daha Ýyi Gösteriyor
    {
        //      Raycastler, Zemin Kontrolü için

        Gizmos.color = leftHit ? Color.green : Color.red;
        Gizmos.DrawLine(leftRayOrigin.position, leftRayOrigin.position + Vector3.down * rayLength);
        Gizmos.color = middleHit ? Color.green : Color.red;
        Gizmos.DrawLine(middleRayOrigin.position, middleRayOrigin.position + Vector3.down * rayLength);
        Gizmos.color = rightHit ? Color.green : Color.red;
        Gizmos.DrawLine(rightRayOrigin.position, rightRayOrigin.position + Vector3.down * rayLength);

        //      Duvar Kontrolü Ýçin Daire Alaný Gösteriyor

        //  (Sorgu ? true : false) Kýsa Ýf Else Gibi
        Gizmos.color = isWallSliding ? Color.blue : Color.black;
        Gizmos.DrawWireSphere(wallCheck.position, lapLength);
    }

                                                        //  Etkileþimler
    private void OnTriggerEnter2D(Collider2D collision)     //  Collider Bileþeni Üzerinden Rigidbodysiz Diðer Nesnelerle Etkileþime Girmek Ýçin
                                                            //  Collider Cisimlerin Etkileþim Alaný, Çarpýþmalar fln Burada Yapýlýyor, Ýnsanýn Derisi Gibi
    {
        if (collision.CompareTag("FallZone"))
        {
            fallLoopCount += 1;     //  Her Düþtüðümüzde Artýyor, Yerle Temasda Sýfýrlanýyor;

            transform.position = new Vector2(transform.position.x, transform.position.y + 15);
        }
        if (collision.CompareTag("Deadzone"))
        {
            Death();
        }
        if (collision.CompareTag("MapBorder"))
        {
            onBorder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MapBorder"))
        {
            onBorder = false;
        }
    }

}
