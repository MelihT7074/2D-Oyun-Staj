using UnityEngine;
using static Player;

public class CameraContorller : MonoBehaviour
{
    [Header("Managers")]
    public Player player;
    public GameDirector gameDirector;

    [Header("Kamera Ayarlar�")]
    public Camera MainCamera;
    public Vector3 camOgPos;            //  Kameran�n �lk Konumu
    public Vector3 camPosOffset;        //  Uygulanacak Konum Fark�
    public Vector3 camTargetPos;        //  Hedef Konum
    public float camTransitionSpeed;    //  Ge�i� H�z�

    [Header("Y�ne Do�ru Bakma")]
    public float lookOffsetAmount;                  //  Bak�l�nacak Miktar
    public float lookTransitionSpeed;               //  Ge�i� H�z�
    private Vector3 lookOffset = Vector3.zero;      //  Uygulunacak Konum Fark�
    private Vector3 targetLookOffset = Vector3.zero;//  Hedef Konum


    private void Start()
    {
        camOgPos = MainCamera.transform.position;
    }

    private void LateUpdate()   //  Normal Updateden 1 Kare sonra �a�r�l�r, Karakter Girdileri Ald���m�z ��in Kamera Takibinin, Girdilerden Sonra Olams� Daha �yi
    {
        CameraPosSwitch_Player();


        Vector3 finalTarget = camTargetPos + lookOffset;
                        //  Lerp : Yumu�ak Ge�i�ler ��in kullan�l�r, Ba�lang�� Poz. Biti� Poz. Ve Ge�i� H�z�.   unscaledDeltaTime : Oyunun Durmas�ndan Etkilenmemesi ��in
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, finalTarget, camTransitionSpeed * Time.unscaledDeltaTime);
    }

    public void CameraPosSwitch_Player()
    {
        if (player.isAlive)         //  Haraket Ediyorken Kameran�n Karakteri Takip Etmesi ��in
        {
            camTransitionSpeed = player.speed * 2;

            camPosOffset.x = player.transform.position.x;
            camPosOffset.y = player.transform.position.y;
            camPosOffset.z = camOgPos.z;

            camTargetPos = camPosOffset;

            if (!player.isMoving)   //  Haraket Etmiyorken Sa�a Sola Bakmak ��in
            {
                switch (player.direction)   //  �f Else Alternatifi, E�le�en case e Giriyor Ve Breakle ��k�yor, E�le�mezse defaulta Giriyor
                {
                    case Directions.Up   : targetLookOffset = new Vector3(0, lookOffsetAmount, 0);      break;
                    case Directions.Down : targetLookOffset = new Vector3(0, -lookOffsetAmount, 0);     break;
                    case Directions.Left : targetLookOffset = new Vector3(-lookOffsetAmount / 2, 0, 0); break;
                    case Directions.Right: targetLookOffset = new Vector3(lookOffsetAmount / 2, 0, 0);  break;
                    default: targetLookOffset = Vector3.zero; break;
                }
            }
            else
            {
                targetLookOffset = Vector3.zero;
            }
            lookOffset = Vector3.Lerp(lookOffset, targetLookOffset, lookTransitionSpeed * Time.deltaTime);
        }
        else                //  �l�nce Buraya �l�m Ekran� Falan �a�r�lmal�
        {
            camTargetPos = camOgPos;
        }
    }


}
