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
            button.transform.position = new Vector2(button.transform.position.x + onMouseEffect.x, button.transform.position.y + onMouseEffect.y);
            SoundManager.Instance.PlayMusic("ButtonHover");
        }
    }

    public void OnMouseExit()               //  Cursor Üstünde Deðilken Olacaklar,  Týklamayada Verilinebilnir
    {
        cursorManager.SetActiveCursor(cursorManager.lst_BasicCursors[0]);

        if (isButton)
        {
            if (button.transform.position != buttonOGPos)   //  Bu Kýsým Týklamadada Geçerli Olduðu Ýçin Buton Çok Hareket Ediyordu, Bu Fixliyor
            {
                button.transform.position = new Vector2(button.transform.position.x - onMouseEffect.x, button.transform.position.y - onMouseEffect.y);
                buttonOGPos = button.transform.position;
            }
        }
    }

    public void ClickButton()
    {
        SoundManager.Instance.PlayMusic("ButtonClick");
    }

}
