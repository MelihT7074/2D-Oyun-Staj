using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Managers")]
    public GameDirector gameDirector;
    public CameraContorller cameraContorller;

    [Header("Can Ayarlar�")]
    public bool isAlive = true;

    [Header("Hareket Ayarlar�")]
    public Rigidbody2D rb;              //  Fiziksel ��lemlerin Yap�l�nabilinmesi ��in Bile�en, �nsan �skeleti, Kas Sistemi Gibi Bir�ey
    public float speed;
    public float jumpForce;

    public Directions direction;
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

    [Header("Raycast ile Zemin Kontrol�")]      //  Karakterin Aya��ndan A�a��ya Do�ru K�sa I��nlar G�ndererek Yere Temas Edip Etmedi�ini Anlamak ��in Sistem  
    public Transform leftRayOrigin;         //  Karakterin Sol Aya��ndaki I��n�n Ba�lang�� Konumu
    public Transform rightRayOrigin;        //  Karakterin Sa� Aya��ndaki I��n�n Ba�lang�� Konumu
    public float rayLength;                 //  I��nlar�n Uzunlu�u
    public LayerMask groundLayer;           //  �stenilen Temas B�lgesi

    public bool isGrounded = false;         //  I��nlardan Herhangibiri De�idi�inde True Oluyor Ve Z�playabiliyoruz
    public bool isFacingRight = false;      //  Karakterin Bakt��� Y�n, Tersine Harekette Karakterin Boyutunu Tersine �evirerek Bakt��� Y�n� De�i�tiriyoruz


    public enum Directions
    {   //   0    1    2      3     4
            Up, Down, Left, Right, None 
    }

    void Start()        //  Oyun Ba�lay�p Her�ey Haz�rlan�d��nda �lk �al��an Method
    {
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()   //  Her Framede �al��an K�s�m, Fps'e Ba�l� Oldu�u ��in Pc nin Kalitesine G�re �al��ma S�kl��� De�i�iyor, Girdi Alma, Fizik D��� ��lemler ��in fln kullan�l�r
    {
                                //  Y�n Ve Haraket Girdileri
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


                                //  Z�plama Mekanikleri
        if (isGrounded) //  Yerle Temasta �ken Saya� Kapal� Ve Pozitif durumda, Z�plama �zinlerinden Biri Buran�n Pozitif Olmas�
        {
            coyoteTimeCounter = coyoteTime;
        }
        else            //  Yerle Temas Kesildi�inde Saya� Ba�l�yor, Negatife D��erse Z�plama Avantaj� Kayboluyor
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))    //  Z�plama Tu�una Bas�ld���nda Saya� S�f�rlan�yor Ve Pozitifteyken Avantaj Sa�lan�yor
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

        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)     //  Space Tu�u B�rak�ld���nda Z�plamay� K�saltmak ��in Var
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);   //  K�sa Bir ��lem Oldu�u ��in Burada Yapman�n Sak�ncas� yok
            coyoteTimeCounter = 0f;     //  Doublejump� Falan �nlemek ��in
        }


        GroundCheckRaycast();
        FlipPlayerX();
    }

    private void FixedUpdate()  //  Sabit Aral�klarla �al��an k�s�m, San�r�m 0.02sn de Bir �a�r�l�yor, Fiziksel ��lemler Yapmak ��in Daha �yi
    {
            
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);    //  Yatay Haraket,

        if (jumpRequested)      //  Z�plama
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
                                //  H�z S�n�rlamalar�
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);    //  Mathf.Clamp : Belirtilen De�eri S�n�rlar, Buras� Yatay H�z� S�n�rl�yor
        float clampedY = Mathf.Clamp(rb.linearVelocity.y, -fallSpeedLimit, jumpForceLimit);    // Buras�da D��me Ve Z�plama H�z�n� S�n�rl�yor

        rb.linearVelocity = new Vector2(clampedX, clampedY);
    }

    private void OnTriggerEnter2D(Collider2D collision)     //  Collider Bile�eni �zerinden Rigidbodysiz Di�er Nesnelerle Etkile�ime Girmek ��in
                                                            //  Collider Cisimlerin Etkile�im Alan�, �arp��malar fln Burada Yap�l�yor, �nsan�n Derisi Gibi
    {
        if (collision.CompareTag("FallZone"))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 20);
        }
        if (collision.CompareTag("Deadzone"))
        {
            gameObject.SetActive(false);            //  �l�m Methodu �a�r�lmal�
        }

    }

    private void GroundCheckRaycast()
    {
                    //  I��n �ekilirken: Ba�lang�� Noktas�, I��n�n Y�n�, I��n�n Uzunlu�u, Temas B�lgesi
        bool leftHit = Physics2D.Raycast(leftRayOrigin.position, Vector2.down, rayLength, groundLayer);
        bool rightHit = Physics2D.Raycast(rightRayOrigin.position, Vector2.down, rayLength, groundLayer);

        isGrounded = leftHit || rightHit;

        // Edit�rde G�z�kmesi ��in.                                     (Sorgu ? true : false) K�sa �f Else Gibi
        Debug.DrawRay(leftRayOrigin.position, Vector2.down * rayLength, leftHit ? Color.green : Color.red);
        Debug.DrawRay(rightRayOrigin.position, Vector2.down * rayLength, rightHit ? Color.green : Color.red);
    
    }

    private void FlipPlayerX()
    {
        if ((moveInput > 0 || direction == Directions.Right)&& !isFacingRight)        //  E�er H�z Pozitif Yani Sa�a Do�ruysa Ve Karakterde Sola Bak�yorsa Karakterin Y�n�n� Sa�a �evirmek ��in 
        {
            FlipX();
        }
        else if ((moveInput < 0 || direction == Directions.Left) && isFacingRight)    //  E�er H�z Negatif Yani Sola Do�ruysa Ve Karakterde Sa�a Bak�yorsa Karakterin Y�n�n� Sola �evirmek ��in  
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
    }


}
