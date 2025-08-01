using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("Managers")]
    public CursorManager cursorManager;



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
    }

    public void OnMouseExit()               //  Cursor Üstünde Deðilken Olacaklar,  Týklamayada Verilinebilnir
    {
        cursorManager.SetActiveCursor(cursorManager.lst_BasicCursors[0]);
    }

}
