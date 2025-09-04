using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    private float length;
    public Camera cam;
    public float parallaxEffect;    //  Arkaplanýn Kameraya Göre Hareket Etmesi Ýçin Gereken Hýz,   0 = Hareketsiz, 1 = Kamerayla Ayný


    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        //  Kamera Hareketine Göre Arkaplan Hareketinin Mesafesini Ayarlama

        float distance = cam.transform.position.x * parallaxEffect;
        float movement = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector2(startPos + distance, transform.position.y);

        //  Arkaplanýn Sonuna Ulaþýnca Konumu Tekrar Ayarlayýp Sonsuz Kaydýrmayý Devam Ettirmek Ýçin

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
