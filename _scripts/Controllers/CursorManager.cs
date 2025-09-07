using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("�mle� Ayarlar�")]
    public List<BasicCursor> lst_BasicCursors;          //  Kullan�lacak T�m �mle�leri Listelemek ��in
    BasicCursor basicCursor;                        //  Aktif Olarak Kullan�lan �mle�

    int currentCursorFrame;                 //  Kullan�lan �mlecin Aktif Framei
    float cursorTimer;                      //  kullan�lan �mlecin Sonraki Frame Ge�me Sayac�

    [System.Serializable]                   //  Edit�rde G�z�kmesi ��in
    public class BasicCursor        //  �mle�lerin �zellikleri
    {
        public CursorTypes cursorType;      //  �mlece �e�itleri Vermek ��in
        public Vector2 cursorHotspot;       //  �mlecin Merkez Noktas�n� Belirlemek ��in
        public float cursorTimerRate;       //  Ka� Sn de Bir Sonraki Frame Ge�ece�ini Se�mek ��in
        public Texture2D[] cursorFrames;    //  �mlecin Resimleri
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

    private void Update()                   //  Animasyon K�sm�
    {
        cursorTimer -= Time.unscaledDeltaTime;
        if (cursorTimer <= 0)
        {
            cursorTimer += basicCursor.cursorTimerRate;
            currentCursorFrame = (currentCursorFrame + 1) % basicCursor.cursorFrames.Length;    //  T�m Kareler Oynat�ld���nda % Operatoru �le Ba�a Al�n�yor
            Cursor.SetCursor(basicCursor.cursorFrames[currentCursorFrame], basicCursor.cursorHotspot, CursorMode.Auto);
        }
    }

    public void SetActiveCursor(BasicCursor basicCursor)    //  Duruma G�re Se�ilen Cursor�n �zelliklerini �uankine ��liyor
    {
        this.basicCursor = basicCursor;
        cursorTimer = basicCursor.cursorTimerRate;
        currentCursorFrame = 0;
        Cursor.SetCursor(basicCursor.cursorFrames[currentCursorFrame], basicCursor.cursorHotspot, CursorMode.Auto);
    }

}
