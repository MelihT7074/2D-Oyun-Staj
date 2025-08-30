using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    [Header("Managers")]
    public CursorManager cursorManager;

    [Header("Butonlar ��in")]
    public bool isButton;
    public GameObject button;
    public Vector3 buttonOGPos;
    public Vector2 onMouseEffect;

    public string lastMouseSituation;

    [Header("Cursor Se�me")]            //  Hangi Cursorun Se�ilece�ini G�stermek ��in
    public int cursorIndex;             //  CursorTypesdaki Index Numaralar�

    public bool uiCursor = false;
    public bool enemyCursor = false;
    public bool testCursor = false;

        /* 
            Kullanmak ��in Nesneye Bu Scripti Verip Boollardan Birini Se�mek Laz�m
            Sonras�nda Nesneye EventTrigger Bile�eninide Verip Uygun Mouse Olayalr� Verilmeli, Baz� Olaylar : 
            PointerEnter �mle� �st�ndeyken, PointerExit �mle� Nesneyi Terkedince, PointerClick Nesneye T�kland���nda Olacak Olaylar
            Bu Olaylara A�a��daki Methodlar Ba�lan�nca �mle� De�i�imleri Ger�ekle�iyor
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

    public void SelectCursor()                  //  Booldaki Se�ene�e G�re Cursor Indexi Ayarl�yor
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

    public void OnMouseEnter()              //  Cursor �st�ndeyken Olacaklar
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

    public void OnMouseExit()               //  Cursor �st�nde De�ilken Olacaklar,  T�klamayada Verilinebilinir
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
    *           //  A��klamalar
    * 
    *   *1 : Mouse, Butonun �st�ndeyken Esc Bas�l�p Men� Kapan�rsa Men� Ve ��indekiler SetActive(false)a D�n���p Kapan�yor, ��levsizleniyor
    *           Yani MouseExit �al��am�yor Ve Butonun Konumu MouseEnterdaki Efekte G�re Kal�yor, E�er Mouse Konumu De�i�meden Tekrar Esc �le Men� A��l�n�rsa
    *           Mouse Butounun �st�ne Gelece�i ��in Tekrardan Efekt Uyguluyor ve Buton �yice Kay�yor, Ne Kadar Tekarara D��erse Buton O Kadar Sap�yor.
    *           lastMouseSituationda Son Etkinli�i Al�yor Ve E�er �st�ste 2 Kere onMouseEnter �a�r�ld�ysa �nce onMouseExiti �a��r�p Butonun Konumunu D�zeltiyor
    *           Sonra �se Tekrar MouseEnter� �a��r�p ��leri Olmas� Gerekti�i Gibi Yap�yor
    *
    *   *2 : Yanl�� Hat�rlam�yorsam MouseExit Hem Mouse Butonun �zerinden ��k�nca Hemde Butona T�klay�nca �al��t��� ��in (�mle� Normale D�ns�n Diye �yle Yapt�yd�m)
    *           Buton Fazla �leri Gidiyordu, O Zamanlar Mouse Efektini Geri �ekmek i�inde Butonun Pozisyonunu Mouse Efektinin Tersiyle topluyordum,
    *           As�l Sorunda Bu Oluyordu Ger�i
    *
    *       //  Daha Ba�ka Buglarda Vard� Ama Neylerdi Ve Nas�l Fixledim Hi� Hat�rlam�yom, Biraz Kar���k Buras� Benim ��in    
*/