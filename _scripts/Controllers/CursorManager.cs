using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Ýmleç Ayarlarý")]
    public List<BasicCursor> lst_BasicCursors;          //  Kullanýlacak Tüm Ýmleçleri Listelemek Ýçin
    BasicCursor basicCursor;                        //  Aktif Olarak Kullanýlan Ýmleç

    int currentCursorFrame;                 //  Kullanýlan Ýmlecin Aktif Framei
    float cursorTimer;                      //  kullanýlan Ýmlecin Sonraki Frame Geçme Sayacý

    [System.Serializable]                   //  Editörde Gözükmesi Ýçin
    public class BasicCursor        //  Ýmleçlerin Özellikleri
    {
        public CursorTypes cursorType;      //  Ýmlece Çeþitleri Vermek Ýçin
        public Vector2 cursorHotspot;       //  Ýmlecin Merkez Noktasýný Belirlemek Ýçin
        public float cursorTimerRate;       //  Kaç Sn de Bir Sonraki Frame Geçeceðini Seçmek Ýçin
        public Texture2D[] cursorFrames;    //  Ýmlecin Resimleri
    }


    public enum CursorTypes
    {
        //      0           1       2               3        
        defaultCursor, uiCursor, attackCursor, testCursor
    }

    private void Start()
    {
        SetActiveCursor(lst_BasicCursors[0]);

    }

    private void Update()                   //  Animasyon Kýsmý
    {
        cursorTimer -= Time.unscaledDeltaTime;
        if (cursorTimer <= 0)
        {
            cursorTimer += basicCursor.cursorTimerRate;
            currentCursorFrame = (currentCursorFrame + 1) % basicCursor.cursorFrames.Length;    //  Tüm Kareler Oynatýldýðýnda % Operatoru Ýle Baþa Alýnýyor
            Cursor.SetCursor(basicCursor.cursorFrames[currentCursorFrame], basicCursor.cursorHotspot, CursorMode.Auto);
        }
    }

    public void SetActiveCursor(BasicCursor basicCursor)    //  Duruma Göre Seçilen Cursorýn Özelliklerini Þuankine Ýþliyor
    {
        this.basicCursor = basicCursor;
        cursorTimer = basicCursor.cursorTimerRate;
        currentCursorFrame = 0;
        Cursor.SetCursor(basicCursor.cursorFrames[currentCursorFrame], basicCursor.cursorHotspot, CursorMode.Auto);
    }

}
