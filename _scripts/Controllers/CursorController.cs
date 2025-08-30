using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    [Header("Managers")]
    public CursorManager cursorManager;

    [Header("Butonlar Ýçin")]
    public bool isButton;
    public GameObject button;
    public Vector3 buttonOGPos;
    public Vector2 onMouseEffect;

    public string lastMouseSituation;

    [Header("Cursor Seçme")]            //  Hangi Cursorun Seçileceðini Göstermek Ýçin
    public int cursorIndex;             //  CursorTypesdaki Index Numaralarý

    public bool uiCursor = false;
    public bool enemyCursor = false;
    public bool testCursor = false;

        /* 
            Kullanmak Ýçin Nesneye Bu Scripti Verip Boollardan Birini Seçmek Lazým
            Sonrasýnda Nesneye EventTrigger Bileþeninide Verip Uygun Mouse Olayalrý Verilmeli, Bazý Olaylar : 
            PointerEnter Ýmleç Üstündeyken, PointerExit Ýmleç Nesneyi Terkedince, PointerClick Nesneye Týklandýðýnda Olacak Olaylar
            Bu Olaylara Aþaðýdaki Methodlar Baðlanýnca Ýmleç Deðiþimleri Gerçekleþiyor
        */


    private void Start()
    {
        cursorManager = FindFirstObjectByType<CursorManager>();
        SelectCursor();
        if (isButton)
        {
            buttonOGPos = button.transform.position;
            lastMouseSituation = "MouseExit";
        }
    }

    public void SelectCursor()                  //  Booldaki Seçeneðe Göre Cursor Indexi Ayarlýyor
    {
        if (uiCursor == true)
        {
            cursorIndex = (int)CursorManager.CursorTypes.uiCursor;
            //cursorIndex = 1;
        }
        else if (enemyCursor == true)
        {
            cursorIndex = (int)CursorManager.CursorTypes.attackCursor;
            //cursorIndex = 2;
        }
        else if (testCursor == true)
        {
            cursorIndex = (int)CursorManager.CursorTypes.testCursor;
            //cursorIndex = 3;
        }
    }

    public void OnMouseEnter()              //  Cursor Üstündeyken Olacaklar
    {
        cursorManager.SetActiveCursor(cursorManager.lst_BasicCursors[cursorIndex]);

        if (isButton)
        {
            if (lastMouseSituation == "MouseExit")  //*1
            {
                lastMouseSituation = "MouseEnter";
                button.transform.position = new Vector2(button.transform.position.x + onMouseEffect.x, button.transform.position.y + onMouseEffect.y);
                SoundManager.Instance.PlayMusic("ButtonHover");     //  Ses Efekti
            }
            else
            {
                OnMouseExit();
                OnMouseEnter();
            }
        }
    }

    public void OnMouseExit()               //  Cursor Üstünde Deðilken Olacaklar,  Týklamayada Verilinebilinir
    {
        cursorManager.SetActiveCursor(cursorManager.lst_BasicCursors[0]);

        if (isButton)
        {
            if (button.transform.position != buttonOGPos)   //*2  
            {
                lastMouseSituation = "MouseExit";
                button.transform.position = new Vector2(buttonOGPos.x, buttonOGPos.y);
            }
        }
    }

    public void ClickButton()
    {
        SoundManager.Instance.PlayMusic("ButtonClick");         //  Ses Efekti
    }

}

/*
    *           //  Açýklamalar
    * 
    *   *1 : Mouse, Butonun Üstündeyken Esc Basýlýp Menü Kapanýrsa Menü Ve Ýçindekiler SetActive(false)a Dönüþüp Kapanýyor, Ýþlevsizleniyor
    *           Yani MouseExit Çalýþamýyor Ve Butonun Konumu MouseEnterdaki Efekte Göre Kalýyor, Eðer Mouse Konumu Deðiþmeden Tekrar Esc Ýle Menü Açýlýnýrsa
    *           Mouse Butounun Üstüne Geleceði Ýçin Tekrardan Efekt Uyguluyor ve Buton Ýyice Kayýyor, Ne Kadar Tekarara Düþerse Buton O Kadar Sapýyor.
    *           lastMouseSituationda Son Etkinliði Alýyor Ve Eðer Üstüste 2 Kere onMouseEnter Çaðrýldýysa Önce onMouseExiti Çaðýrýp Butonun Konumunu Düzeltiyor
    *           Sonra Ýse Tekrar MouseEnterý Çaðýrýp Ýþleri Olmasý Gerektiði Gibi Yapýyor
    *
    *   *2 : Yanlýþ Hatýrlamýyorsam MouseExit Hem Mouse Butonun Üzerinden Çýkýnca Hemde Butona Týklayýnca Çalýþtýðý Ýçin (Ýmleç Normale Dönsün Diye Öyle Yaptýydým)
    *           Buton Fazla Ýleri Gidiyordu, O Zamanlar Mouse Efektini Geri Çekmek içinde Butonun Pozisyonunu Mouse Efektinin Tersiyle topluyordum,
    *           Asýl Sorunda Bu Oluyordu Gerçi
    *
    *       //  Daha Baþka Buglarda Vardý Ama Neylerdi Ve Nasýl Fixledim Hiç Hatýrlamýyom, Biraz Karýþýk Burasý Benim Ýçin    
*/