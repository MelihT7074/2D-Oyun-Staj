using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("Managers")]
    public CursorManager cursorManager;



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
    }

    public void OnMouseExit()               //  Cursor �st�nde De�ilken Olacaklar,  T�klamayada Verilinebilnir
    {
        cursorManager.SetActiveCursor(cursorManager.lst_BasicCursors[0]);
    }

}
