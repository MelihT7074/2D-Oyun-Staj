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
    [Header("Can Ayarlar�")]
    public GameObject SpawnPos;
    public bool isAlive = true;
    public float hp;                //  Toplam Can
    public float currentHp;         //  Mecvut Can
    public bool hpRefillTimerStart; //  Alttaki S�reyi Aktif Etmek ��in, Dolum Ba�lay�nca Kapan�yor, Hasar Al�nca Tekrar Aktif oluoyr
    public float hpRefillTimer;     //  Ka� Sn Boyunca Hasar Almazsak Can Doldurulaca��n� Belirlemek ��in
    public bool hpRefillStart;      //  Can Doldurmaya Ba�lama Onay�, Hasar Al�nca Kapan�yor
    public float hpRefillAmount;    //  Can Dolum Miktar�
    public Image healtBar;          //  Can Bar�

    //[Header("Yedekler")]    //  
    private float hpClon;
    private float hpRefillTimerClon;
    private float borderDmgStartTimerClon;

    [Space]
    [Header("Hasar Efektleri")]
    public bool isPlayerDmgTaken;       //  Hasar Ald���m�z�n Bilgisi
    public Image img_DmgEffect;         //  Hasar Efekti G�rseli, Hasar Al�nca Opakl��� Fulleniyor Sonra Azalmaya Ba�l�yor, Opakl�k Miktar�na G�re Di�er Tu� Girdilerini Flnda Kilitliyorum
    public bool isDmgImgActive;         //  G�rselin Opakl���n� D���rmek ��in Onay Veriyor
    public float dmgPunchPower;         //  Savrulma Miktar�, D�zg�n De�erleri Bulmak Zor


    [Space]
    [Header("Hareket Ayarlar�")]
    public Rigidbody2D rb;              //  Fiziksel ��lemlerin Yap�l�nabilinmesi ��in Bile�en, �nsan �skeleti, Kas Sistemi Gibi Bir�ey
    public bool showVelocity;
    public float speed;
    public float jumpForce;

    public Directions direction;
    public Vector2 directionInputs;
    public bool isFacingRight = false;      //  Karakterin Bakt��� Y�n, Tersine Harekette Karakterin Boyutunu Tersine �evirerek Bakt��� Y�n� De�i�tiriyoruz
    public bool isMoving =>     //  Get Set Kullanman�n K�sa Yolu, Fazla Bir Bilgim Yok Ama A��a��daki ��lemlere G�re Return Tarz� De�er G�nderiyor
        Mathf.Abs(moveInput) > 0.01f && Mathf.Abs(rb.linearVelocity.x) > 0.05f && Mathf.Abs(rb.linearVelocity.y) > 0.05f;   //  Mathf.Abs : Mutlak De�er Alma

    public float maxSpeed;          //  Karakterin Ula�abilece�i Max H�z
    public float fallSpeedLimit;    //  D��me H�z� S�n�r�
    public float jumpForceLimit;    //  Maksimum Z�plama Y�ksekli�i, Gereksiz Gibi Asl�nda

    public float moveInput = 0f;    //  linearVelocity Rigidbodynin Do�rusal H�z�n�na Eri�mek ��in Kullan�l�yor, X Eksenindeki +- Sa�-Soldaki S�ratini, Ydeki Yukar�-A��a��ki S�ratini G�steriyor
                                    //  moveInputlada Y�n Bilgisini Girip speedle �arp�p S�rati Veriyoruz 
    public bool jumpRequested = false;
    //  Aya��n Yerden Temas�n�n Kesilmesinden K�sa Bir S�re Sonra Z�plamay� M�mk�n K�lmak ��in Zamanaly�c�
    public float coyoteTime;
    public float coyoteTimeCounter;
    //  Yere De�meden K�sa Bir S�re �nce Z�plamaya Basarsak Yere De�idi�imiz Gibi Z�platmas� ��in Zamanlay�c�
    public float jumpBufferTime;
    public float jumpBufferTimeCounter;

    [Space]     //  Karaker Z�play�p Duvara Temas Ederse Ve Duvar Y�n�nde �lerlemeye �al���rsa Havada As�l� Kal�yordu, Bu Onu Birazc�k Fixliyor Ve Yeni Mekanikler Ekliyor
    [Header("Duvardan Kayma")]                  //  As�l Fixleyen �ey Karakterin Collider�na S�rt�nmesiz Bir Materyal Vermek Oldu
    public Transform wallCheck;
    public float lapLength;
    public LayerMask wallAreas;

    public bool isWallSliding = false;
    public float wallSlidingSpeed;

    [Space]
    [Header("Duvardan Z�plama")]
    public bool isWallJumping;          //  Z�plama Onay� Ve Di�er Hareket Girdilierini �ak��ma Olmamas� ��in Kapatma,
    public float wallJumpingDirection;  //  Z�plaman�n Y�n�, Duvara Ters Yani Oyuncunun Bakt��� Y�n�n Z�tt� Olaml�
    public float wallJumpingTime;       
    public float wallJumpingCounter;    
    public float wallJumpingDuration;   
    public Vector2 walljumpingPower;    //  X ve Y Ekseninde Uygulanacak G��

    [Space]
    [Header("Raycast ile Zemin Kontrol�")]      //  Karakterin Aya��ndan A�a��ya Do�ru K�sa I��nlar G�ndererek Yere Temas Edip Etmedi�ini Anlamak ��in Sistem  
    public Transform leftRayOrigin;         //  Karakterin Sol Aya��ndaki I��n�n Ba�lang�� Konumu
    public bool leftHit;
    public Transform middleRayOrigin;       //  Karakterin Ortas�ndaki I��n�n Ba�lang�� Konumu,     �nce Nesnelerin �st�ndeyken Atlamas�n� Kolayla�t�rmak ��in
    public bool middleHit;
    public Transform rightRayOrigin;        //  Karakterin Sa� Aya��ndaki I��n�n Ba�lang�� Konumu
    public bool rightHit;

    public float rayLength;                 //  I��nlar�n Uzunlu�u
    public LayerMask jumpAreas;             //  �stenilen Temas B�lgeleri,  Engellerinde �st�nden Z�playabilmesi i�in

    public bool isGrounded = false;         //  I��nlardan Herhangibiri De�idi�inde True Oluyor Ve Z�playabiliyoruz

    [Space]
    [Header("Map S�n�r� Hasar Ayarlar�")]
    public GameObject borderWarning;
    public Text txt_leaveTimer;
    public bool isBorderWarning;            //  Ekran� Aktifle�tirme Ve Kontrol�
    public int fallLoopCount;               //  D��me D�ng�s�ne Girersek Uyar�y� G�stermek ��in
    public bool onBorder = false;           //  Map S�n�r�ndan ��karsak Uyar�y� G�stermek ��in
    public float borderDmgStartTimer = 5;   //  Hasar Almaya Ba�lamadan �nceki S�re
    public float borderDmgTakenTime;        //  Hasar Alma Aral���
    public float lastBorderDmgTakenTime;    //  Hasar Ald���m�zdaki S�re, Oyun S�resinden ��kart�nca Hasar Aral���ndan Fazla �se Hasar Al�yoruz


    public enum Directions
    {   //   0    1    2      3     
            Up, Down, Left, Right,
    }

    void Start()        //  Oyun Ba�lay�p Her�ey Haz�rlan�d��nda �lk �al��an Method
    {
        PlayerAbilitysClon();
    }

    public void RestartPlayer()
    {
        gameDirector.escAndOptions.deathScreen.SetActive(false);

        gameObject.SetActive(true);
        isAlive = true;

        gameDirector.GD_SwitchMusic();      //  Tekrar Canlan�nca Normal Oyun M�zi�ine Ge�mek ��in

        currentHp = hpClon;

        rb = GetComponent<Rigidbody2D>();
        rb.position = new Vector2(SpawnPos.transform.position.x, SpawnPos.transform.position.y + 1);
    }

    public void PlayerAbilitysClon()    //  De�erlerin Yedeklerini Alma
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

        gameDirector.GD_SwitchMusic();      //  �l�m Ekran� M�zi�ine Ge�mek ��in

                //  Hasar Alarak �ld�ysek Etkileri S�f�rlama yeri
        isPlayerDmgTaken = false;
        img_DmgEffect.color = new Color(255, 255, 255, 0);
                //  Uyar� Ekran�yla �ld�ysek Onun Etkilerini Kapatmak ��in
        fallLoopCount = 0;
        onBorder = false;
    }

    public void TakeDamage(float damageCount)
    {
        currentHp -= damageCount;                   //  Hasar Miktar�na G�re Can Azalt�yor
        isPlayerDmgTaken = true;      //**
        hpRefillStart = false;                      //  Hasar Al�nca Can Dolumunu Durdurmak ��in
        hpRefillTimer = hpRefillTimerClon;          //  Doldurma Zamanlay�c�s�n� S�f�rl�yor
        hpRefillTimerStart = true;                  //  Can Doldurma S�resini Aktifle�tiriyor
    }

    public void DamageEffectStart()
    {
        isPlayerDmgTaken = false;
        SoundManager.Instance.PlayMusic("PlayerDamageSound");   //  Ses Efekti
        img_DmgEffect.color = Color.white;                      //  G�rseli Aktifle�tiriyor
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

    void Update()   //  Her Framede �al��an K�s�m, Fps'e Ba�l� Oldu�u ��in Pc nin Kalitesine G�re �al��ma S�kl��� De�i�iyor, Girdi Alma, Fizik D��� ��lemler ��in fln kullan�l�r
    {
        if (Time.timeScale == 0) return;    //  Oyun Durdu�unda Girdi Al�mlar�n� Falan Kapatmak ��in ��nk�;
                                            //  !!  Update Pcnin �retti�i Her Karede �al��t��� ��in Oyun Zaman�n�n Durmas�ndan Etkilenmiyor, ��indeki Time.deltaTime K�s�mlar� Hari� Her�ey �al��maya Devam Ediyor
                                            //  Time.deltaTime : 2 Frame Aras� Ge�en S�reyi Hesaplar, Bu S�re Fark�n� timeScalea G�re Verir, Buy�zden Zaman 0 Olunca Duruyor.
                                            //  !!  FixedUpdate �se Sistemin �retti�i Zaman Aral�klar�yla �al��t��� ��in TimeScale=0dan Etkileniyor, �indeki Her�ey Duruyor

                                    //  Y�n Ve Haraket Girdileri
        if (img_DmgEffect.color.a <= 0.5f)  
        {
            moveInput = 0f;

            if (Input.GetKey(KBM.GetKey("MoveLeft")) || Input.GetKey(KBM.GetKey("MoveLeft_Sc")))
            {
                moveInput = -1f;
                direction = Directions.Left;
                directionInputs = new Vector2(-1, 0);

                if (showVelocity)               //  Hareket, Y�n, Velocity Kontrolleri //
                { 
                    Debug.Log("Yatay Hareket Girdisi (Sol) : " + (moveInput > 0 ? "1 (Sa�)" : "-1 (Sol)") + ", velocity : " + rb.linearVelocity);
                }
            }
            else if (Input.GetKey(KBM.GetKey("MoveRight")) || Input.GetKey(KBM.GetKey("MoveRight_Sc")))
            {
                moveInput = 1f;
                direction = Directions.Right;
                directionInputs = new Vector2(1, 0);

            if (showVelocity)               //  Hareket, Y�n, Velocity Kontrolleri //
            {
                Debug.Log("Yatay Hareket Girdisi (Sa�) : " + (moveInput > 0 ? "1 (Sa�)" : "-1 (Sol)") + ", velocity : " + rb.linearVelocity);
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
                                    //  Z�plama Mekanikleri
        if (isGrounded) //  Yerle Temasta �ken Saya� Kapal� Ve Pozitif durumda, Z�plama �zinlerinden Biri Buran�n Pozitif Olmas�
        {
            coyoteTimeCounter = coyoteTime;
            fallLoopCount = 0;      //  D��me D�ng�s� Bitiyor
        }
        else            //  Yerle Temas Kesildi�inde Saya� Ba�l�yor, Negatife D��erse Z�plama Avantaj� Kayboluyor
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KBM.GetKey("Jump")))   //  Z�plama Tu�una Bas�ld���nda Saya� S�f�rlan�yor Ve Pozitifteyken Avantaj Sa�lan�yor
        {
            jumpBufferTimeCounter = jumpBufferTime;
        }
        else                                    //  Saya� Eksilmeye Ba�l�yor Ve Pozitifte Kald��� K�sa S�re Boyunca Z�plama �zinlerinden Di�erini Veriyor
        {
            if (jumpBufferTimeCounter >= -2f)   //  Fazla Eksilmesin Diye, Edit�rde De�erin S�rekli Sonsuza Do�ru Durmadan D��t���n� G�rmek Sinirimi Bozuyor
            {
                jumpBufferTimeCounter -= Time.deltaTime;
            }
        }

        if (coyoteTimeCounter > 0f && jumpBufferTimeCounter > 0f)   //  �artlar Sa�lan�rsa Z�plama Onay� ��lemin Daha Do�ru Yap�lams� ��in FixedUpdate e G�nderiliyor
        {
            jumpRequested = true;
            jumpBufferTimeCounter = 0f; //  S�f�rlanmassa Z�plama S�resi Gereksiz Art�yor
        }

        if (Input.GetKeyUp(KBM.GetKey("Jump")) && rb.linearVelocity.y > 0f)   //  Space Tu�u B�rak�ld���nda Z�plamay� K�saltmak ��in Var
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);   //  K�sa Bir ��lem Oldu�u ��in Burada Yapman�n Sak�ncas� yok
            coyoteTimeCounter = 0f;     //  Doublejump� Falan �nlemek ��in
        }


                                    //  Can Ayarlar�
        if (currentHp <= hp)
        {
            if (hpRefillStart == true)                          //  Onay Gelirse Can Doldurma ��lemi Ba�l�yor
            {
                currentHp += hpRefillAmount * Time.deltaTime;   //  Can Doldurma ��lemi
                healtBar.fillAmount = currentHp / hp;
            }
            healtBar.fillAmount = currentHp / hp;               //  Can Bar�n�n Doluluk Oran�n� Ayarl�yor
        }
        else
        {
            hpRefillStart = false;
            currentHp = hp;                                     //  Can Fullendikten Sonra K�s�ratl� Bi Fazlal�k Oluyor, Onu K�rpmak ��in
        }

        if (hpRefillTimerStart == true)                         //  Hasar Ald�ktan Sonra Can Doldurmaya Ba�lamak ��n
        {
            if (hpRefillTimer > 0)                              //  Sayac� Azaltma Yeri
            {
                hpRefillTimer -= Time.deltaTime;
            }
            else                                                //  Sayac Biterse Dolumu Ba�latma Yeri
            {
                hpRefillStart = true;                           //  Dolum Onay�
                hpRefillTimerStart = false;                     //  Sayac� Durdurup
                hpRefillTimer = hpRefillTimerClon;              //  S�reyi Eski Haline getirme
            }
        }

        if (currentHp <= 0)
        {
            Death();        //  �l�m
        }

                                    //  Map S�n�r�n� Ge�ince Veya D��me D�ng�s�ne Girince Uyar� Ve Hasar Alma
        if ((onBorder || fallLoopCount > 10) && isAlive)
        {
            isBorderWarning = true;
            borderWarning.SetActive(true);

            if (borderDmgStartTimer >= 0)
            {
                borderDmgStartTimer -= Time.deltaTime;

                txt_leaveTimer.text = borderDmgStartTimer.ToString("0");    //  Tam Say� olarak Yazd�r�l�yor, Belli Bir K�s�rat ��in = "0.###"
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
                borderDmgStartTimer = borderDmgStartTimerClon;  //  Fazlal�k K�s�rat� K�rpma
            }
        }

        WallSlide();
        WallJumping();

                            
        if (isPlayerDmgTaken)   //  Hasar Efektleri
        {
            if (!isBorderWarning)
            {
                moveInput = -directionInputs.x * dmgPunchPower;     //  Do�ru Yermi Emin De�ilim, Etkisi K�sa S�r�yor
                        //  moveInput Tu�a Bas�lmad���nda 0 Oldu�u Ve Yatay Hareketde Do�rudan Buna Ba�l� Oldu�u ��in �yice K�sal�yordu,
                                //  Hasar Ekran� Opakl���na G�re Oradaki Girdileri Kapat�nca Durum �yile�ti Gibi
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

    private void FixedUpdate()  //  Sabit Aral�klarla �al��an k�s�m, San�r�m 0.02sn de Bir �a�r�l�yor, Fiziksel ��lemler Yapmak ��in Daha �yi
    {
        if (!isWallJumping)
        {

            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);    //  Yatay Haraket

            if (isWallSliding)      //  Kayma �artlar� Ger�ekle�tiyse x Eksenindeki Hareketi K�s�tlay�p Yava��a A�a��ya Do�ru Kay�yoruz
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));    //**

                if (showVelocity)               //  Hareket, Y�n, Velocity Kontrolleri //
                {
                    Debug.Log("Kayma Ger�ekle�ti, velocity : " + rb.linearVelocity);
                }
            }

            if (jumpRequested)      //  Z�plama
            {
                SoundManager.Instance.PlayMusic("JumpSound");           //  Ses Efekti

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                if (showVelocity)               //  Hareket, Y�n, Velocity Kontrolleri //
                {
                    Debug.Log("Z�plama Ger�ekle�ti, velocity : " + rb.linearVelocity);
                }

                jumpRequested = false;
            }
            //  H�z S�n�rlamalar�
            float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);    //  Mathf.Clamp : Belirtilen De�eri S�n�rlar, Buras� Yatay H�z� S�n�rl�yor
            float clampedY = Mathf.Clamp(rb.linearVelocity.y, -fallSpeedLimit, jumpForceLimit);    // Buras�da D��me Ve Z�plama H�z�n� S�n�rl�yor

            rb.linearVelocity = new Vector2(clampedX, clampedY);
        }
        if (showVelocity)               //  Hareket, Y�n, Velocity Kontrolleri //
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

            if (showVelocity)               //  Hareket, Y�n, Velocity Kontrolleri //
            {
                Debug.Log("Player direction : " + (transform.localScale.x > 0 ? "1 (Sol)" : "-1 (Sa�)") +
                    ", WallJump direction : " + (wallJumpingDirection > 0 ? "1 (Sa�)" : "-1 (Sol)") +
                    ", velocity : " + rb.linearVelocity);
            }

            if (transform.localScale.x == wallJumpingDirection)
            {
                FlipX();
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);

            if (showVelocity)               //  Hareket, Y�n, Velocity Kontrolleri //
            {
                Debug.Log("Duvardan Z�plama Ger�ekle�ti, velocity : " + rb.linearVelocity);
            }

        }

        if (moveInput == wallJumpingDirection)      //  K�sa S��ramalar Yapmak ��in
        {
            StopWallJumping();
        }
    }

    private void StopWallJumping()  //  Invoke �le Belli Bir Zaman Sonra �a��rmak ��in Ayr� Methodda
    {
        isWallJumping = false;
    }

    private void GroundCheckRaycast()
    {
        //  I��n �ekilirken: Ba�lang�� Noktas�, I��n�n Y�n�, I��n�n Uzunlu�u, Temas B�lgesi
        leftHit     = Physics2D.Raycast(leftRayOrigin.position, Vector2.down, rayLength, jumpAreas);
        middleHit   = Physics2D.Raycast(middleRayOrigin.position, Vector2.down, rayLength, jumpAreas);
        rightHit    = Physics2D.Raycast(rightRayOrigin.position, Vector2.down, rayLength, jumpAreas);

        isGrounded = leftHit || middleHit || rightHit;
    }

    private void WallSlide()    //  Sadece Duvarla Temas Halindeysek Ve Duvara Do�ru Hareket Etmeye �al���yorsak Yava��a Kayd�rmak ��in Onay Veriyor
    {
        bool touchingWall = Physics2D.OverlapCircle(wallCheck.position, lapLength, wallAreas);  //  Raycaste Benzer Mant��� Var, Bu Daire �eklinde �iziyor, Raycestse D�z �izgi Olarak �iziyor

        isWallSliding = touchingWall && !isGrounded && moveInput != 0;
    }

    private void FlipPlayerX()
    {
        if ((moveInput > 0 || direction == Directions.Right) && !isFacingRight)         //  E�er H�z Pozitif Yani Sa�a Do�ruysa Ve Karakterde Sola Bak�yorsa Karakterin Y�n�n� Sa�a �evirmek ��in 
        {
            FlipX();
        }
        else if ((moveInput < 0 || direction == Directions.Left) && isFacingRight)      //  E�er H�z Negatif Yani Sola Do�ruysa Ve Karakterde Sa�a Bak�yorsa Karakterin Y�n�n� Sola �evirmek ��in  
        {
            FlipX();
        }
    }

    private void FlipX()
    {
        isFacingRight = !isFacingRight;
        Vector3 ls = transform.localScale;      //  Karakterin Boyutu
        ls.x *= -1;                             //  D�nd�rmek ��in Boyutu X Ekseninde Ters �eviriyoruz
        transform.localScale = ls;
                                        //  Can Bar�n�n Y�n�n� Sabit K�lmak ��in
        Vector3 hpls = healtBar.transform.localScale;
        hpls.x *= -1;
        healtBar.transform.localScale = hpls;
    }

    private void OnDrawGizmos()     //  �izgileri Edit�rde Daha �yi G�steriyor
    {
        //      Raycastler, Zemin Kontrol� i�in

        Gizmos.color = leftHit ? Color.green : Color.red;
        Gizmos.DrawLine(leftRayOrigin.position, leftRayOrigin.position + Vector3.down * rayLength);
        Gizmos.color = middleHit ? Color.green : Color.red;
        Gizmos.DrawLine(middleRayOrigin.position, middleRayOrigin.position + Vector3.down * rayLength);
        Gizmos.color = rightHit ? Color.green : Color.red;
        Gizmos.DrawLine(rightRayOrigin.position, rightRayOrigin.position + Vector3.down * rayLength);

        //      Duvar Kontrol� ��in Daire Alan� G�steriyor

        //  (Sorgu ? true : false) K�sa �f Else Gibi
        Gizmos.color = isWallSliding ? Color.blue : Color.black;
        Gizmos.DrawWireSphere(wallCheck.position, lapLength);
    }

                                                        //  Etkile�imler
    private void OnTriggerEnter2D(Collider2D collision)     //  Collider Bile�eni �zerinden Rigidbodysiz Di�er Nesnelerle Etkile�ime Girmek ��in
                                                            //  Collider Cisimlerin Etkile�im Alan�, �arp��malar fln Burada Yap�l�yor, �nsan�n Derisi Gibi
    {
        if (collision.CompareTag("FallZone"))
        {
            fallLoopCount += 1;     //  Her D��t���m�zde Art�yor, Yerle Temasda S�f�rlan�yor;

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
