using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Managers")]
    public Player player;
    public GameDirector gameDirector;

    [Header("MapBorders")]
    public GameObject fallArea;
    public GameObject deadZone;
    public GameObject mapBorder;

    [Header("DamageWalls")]                 
    public DamageWall dmgWallPrefab;        //  Hazýr Bir Nesne Oluþturup Onu Klonlama Tarzý Birþey
    public Vector2 dmgWallCount;            //  Rastgele 2 Sayý Arasýnda Oluþturmak Ýçin
    public List<DamageWall> lst_dmgWalls;           //  Oluþturulunacak Nesnelerin Tutulacaðý Liste
    public List<GameObject> lst_DWSpawnPos;             //  Mevcut/Müsait Spawnlanma Noktalarý
    public List<GameObject> lst_selectedDWSpawnPos;     //  Seçilen Ve Birdaha Seçilmemesi Gereken Spawn Noktalarý
    private int choosenDWPos;                       //  LÝstelerdeki Nesnelere Indexinden Eriþmek Ýçin
    public List<string> lst_dmgWallTypes        //  Engellerin Türleri, Enuma Göre Ýþlemler Daha Basit Olduðu Ýçin List Kullanýdým
        = new List<string> { "stationary", "laser", "barrier"};


    private void Update()
    {

        fallArea.transform.position = new Vector2(player.transform.position.x, fallArea.transform.position.y);
        deadZone.transform.position = new Vector2(player.transform.position.x, deadZone.transform.position.y);
        mapBorder.transform.position = new Vector2(mapBorder.transform.position.x, player.transform.position.y);

    }

    public void RestartLevelManager()
    {
        DeleteDmgWalls();
        GenerateDmgWalls();

    }

    public void GenerateDmgWalls()  //  Prefab Ýle Nesne Oluþturma
    {
        var randomDmgWallCount = Random.Range(dmgWallCount.x, dmgWallCount.y);  //  Oluþturulunacak Nesne Sayýsýný Belirleme

        for (int i = 1; i <= randomDmgWallCount; i++)   //  Çýkan Sayý Kadar Oluþturup Özelliklerini Ayarlama
        {

            RandomDmgWallPos();     //  Rastgele Spawn Konumu Belirleme


            var newDmgWall = Instantiate(dmgWallPrefab);    //  Nesneyi Oluþturma

            newDmgWall.GetComponent<DamageWall>();          //  Oluþan Nesnenin Scriptine Ulaþma, Eskiden Direkt Prefabdeki Asýl Nesneye Ulaþtýðýmdan Bazý Hatalar Oluyordu

            //**        //  Rastgele Engel Türü Seçip Nesneye Atama

            int randomDWType = Random.Range(0, lst_dmgWallTypes.Count);
            string chosenDWType = lst_dmgWallTypes[randomDWType];
            newDmgWall.dmgWallType = chosenDWType;

            //**

            newDmgWall.transform.position =             //  Oluþan Nesnenin Konumunu Seçilen Spawn Noktasýna Ayarlama
                new Vector2
                (
                    lst_DWSpawnPos[choosenDWPos].transform.position.x , lst_DWSpawnPos[choosenDWPos].transform.position.y + 1
                );


            lst_DWSpawnPos.RemoveAt(choosenDWPos);      //  Seçilen Konumu Müsait Konumlarýn Listensinden Çýkarma

            lst_dmgWalls.Add(newDmgWall);   //  Oluþturulan Nesneyi Listesine Ekleme

            newDmgWall.dwIndex = lst_dmgWalls.Count - 1;

            newDmgWall.ActivateDmgWall(player);
        }

                    //  Listeyi Yenileme Burada Daha Ýyi Çalýþýyor Gibi
        lst_DWSpawnPos.AddRange(lst_selectedDWSpawnPos);        //  Nesne Oluþturma Bittiði Ve Sonrasýnda Tekrar Kullanýlmasý Ýçin Müsait Liste Tekrar Dolduruluyor
        lst_selectedDWSpawnPos.Clear();                         //  Seçilende Kalmayacaðý Ýçin Temizleniyor

    }

    public void RandomDmgWallPos()      //  Spawn Konumu Belirleme
    {
        choosenDWPos = Random.Range(0, lst_DWSpawnPos.Count); //  Müsait Noktalardan Rastgele Birini Seçip Indexini Alma

        lst_selectedDWSpawnPos.Add(lst_DWSpawnPos[choosenDWPos]); //  Seçileni Birdaha Seçilmemesi Ýçin Baþka Bir Listeye Indexi Üzerinden Aktarma, Mevcut Listeden Silme Kullanýldýktan Sonra Gerçekleþecek
    }

    public void DeleteDmgWalls()
    {
        foreach (var d in lst_dmgWalls)     //  Listedeki Tüm Nesneleri Bulup,
        {
            Destroy(d.gameObject);              //  Yok Edip,
        }
        lst_dmgWalls.Clear();                   //  Listeyi Temizleme.
    }

}
