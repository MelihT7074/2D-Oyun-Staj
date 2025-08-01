using UnityEngine;
using static Player;

public class CameraContorller : MonoBehaviour
{
    [Header("Managers")]
    public Player player;
    public GameDirector gameDirector;
    public EscAndOptions escAndOptions;

    [Header("Kamera Ayarlarý")]
    public Camera MainCamera;
    public Vector3 camOgPos;            //  Kameranýn Ýlk Konumu
    public Vector3 camPosOffset;        //  Uygulanacak Konum Farký
    public Vector3 camTargetPos;        //  Hedef Konum
    public float camTransitionSpeed;    //  Geçiþ Hýzý

    [Header("Yöne Doðru Bakma")]
    public float lookOffsetAmount;                  //  Bakýlýnacak Miktar
    public float lookTransitionSpeed;               //  Geçiþ Hýzý
    private Vector3 lookOffset = Vector3.zero;      //  Uygulunacak Konum Farký
    private Vector3 targetLookOffset = Vector3.zero;//  Hedef Konum


    private void Start()
    {
        camOgPos = MainCamera.transform.position;

    }

    private void LateUpdate()   //  Normal Updateden 1 Kare sonra Çaðrýlýr, Karakter Girdileri Aldýðýmýz Ýçin Kamera Takibinin, Girdilerden Sonra Olamsý Daha Ýyi
    {
        CameraPosSwitch_Player();


        Vector3 finalTarget = camTargetPos + lookOffset;
                        //  Lerp : Yumuþak Geçiþler Ýçin kullanýlýr, Baþlangýç Poz. Bitiþ Poz. Ve Geçiþ Hýzý.   unscaledDeltaTime : Oyunun Durmasýndan Etkilenmemesi Ýçin
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, finalTarget, camTransitionSpeed * Time.unscaledDeltaTime);
    }

    public void CameraPosSwitch_Player()
    {
        if (player.isAlive && escAndOptions.currentOpenMenu == "None")         //  Haraket Ediyorken Kameranýn Karakteri Takip Etmesi Ýçin
        {
            camTransitionSpeed = player.speed * 2;

            camPosOffset.x = player.transform.position.x;
            camPosOffset.y = player.transform.position.y;
            camPosOffset.z = camOgPos.z;

            camTargetPos = camPosOffset;

            if (!player.isMoving)   //  Haraket Etmiyorken Saða Sola Bakmak Ýçin
            {
                switch (player.direction)   //  Ýf Else Alternatifi, Eþleþen case e Giriyor Ve Breakle Çýkýyor, Eþleþmezse defaulta Giriyor
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
        else                //  Ölünce Buraya Ölüm Ekraný Falan Çaðrýlmalý,     Þuanlýk Esc Menusunu Hedefliyor, Ýlerde Ýyileþtirme Yapýlýnlamý
        {
            CameraPosSwitch_EscMenus();
        }
    }

    public void CameraPosSwitch_EscMenus()
    {
        if (escAndOptions.currentOpenMenu == "Esc")
        {
            camTargetPos = player.transform.position;
            camTargetPos.z = camOgPos.z;
        }
    }

}
