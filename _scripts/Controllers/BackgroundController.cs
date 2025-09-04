using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    private float length;
    public Camera cam;
    public float parallaxEffect;    //  Arkaplan�n Kameraya G�re Hareket Etmesi ��in Gereken H�z,   0 = Hareketsiz, 1 = Kamerayla Ayn�


    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        //  Kamera Hareketine G�re Arkaplan Hareketinin Mesafesini Ayarlama

        float distance = cam.transform.position.x * parallaxEffect;
        float movement = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector2(startPos + distance, transform.position.y);

        //  Arkaplan�n Sonuna Ula��nca Konumu Tekrar Ayarlay�p Sonsuz Kayd�rmay� Devam Ettirmek ��in

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
