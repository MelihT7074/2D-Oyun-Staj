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
    public DamageWall dmgWallPrefab;        //  Haz�r Bir Nesne Olu�turup Onu Klonlama Tarz� Bir�ey
    public Vector2 dmgWallCount;            //  Rastgele 2 Say� Aras�nda Olu�turmak ��in
    public List<DamageWall> lst_dmgWalls;           //  Olu�turulunacak Nesnelerin Tutulaca�� Liste
    public List<GameObject> lst_DWSpawnPos;             //  Mevcut/M�sait Spawnlanma Noktalar�
    public List<GameObject> lst_selectedDWSpawnPos;     //  Se�ilen Ve Birdaha Se�ilmemesi Gereken Spawn Noktalar�
    private int choosenDWPos;                       //  L�stelerdeki Nesnelere Indexinden Eri�mek ��in
    public List<string> lst_dmgWallTypes        //  Engellerin T�rleri, Enuma G�re ��lemler Daha Basit Oldu�u ��in List Kullan�d�m
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

    public void GenerateDmgWalls()  //  Prefab �le Nesne Olu�turma
    {
        var randomDmgWallCount = Random.Range(dmgWallCount.x, dmgWallCount.y);  //  Olu�turulunacak Nesne Say�s�n� Belirleme

        for (int i = 1; i <= randomDmgWallCount; i++)   //  ��kan Say� Kadar Olu�turup �zelliklerini Ayarlama
        {

            RandomDmgWallPos();     //  Rastgele Spawn Konumu Belirleme


            var newDmgWall = Instantiate(dmgWallPrefab);    //  Nesneyi Olu�turma

            newDmgWall.GetComponent<DamageWall>();          //  Olu�an Nesnenin Scriptine Ula�ma, Eskiden Direkt Prefabdeki As�l Nesneye Ula�t���mdan Baz� Hatalar Oluyordu

            //**        //  Rastgele Engel T�r� Se�ip Nesneye Atama

            int randomDWType = Random.Range(0, lst_dmgWallTypes.Count);
            string chosenDWType = lst_dmgWallTypes[randomDWType];
            newDmgWall.dmgWallType = chosenDWType;

            //**

            newDmgWall.transform.position =             //  Olu�an Nesnenin Konumunu Se�ilen Spawn Noktas�na Ayarlama
                new Vector2
                (
                    lst_DWSpawnPos[choosenDWPos].transform.position.x , lst_DWSpawnPos[choosenDWPos].transform.position.y + 1
                );


            lst_DWSpawnPos.RemoveAt(choosenDWPos);      //  Se�ilen Konumu M�sait Konumlar�n Listensinden ��karma

            lst_dmgWalls.Add(newDmgWall);   //  Olu�turulan Nesneyi Listesine Ekleme

            newDmgWall.dwIndex = lst_dmgWalls.Count - 1;

            newDmgWall.ActivateDmgWall(player);
        }

                    //  Listeyi Yenileme Burada Daha �yi �al���yor Gibi
        lst_DWSpawnPos.AddRange(lst_selectedDWSpawnPos);        //  Nesne Olu�turma Bitti�i Ve Sonras�nda Tekrar Kullan�lmas� ��in M�sait Liste Tekrar Dolduruluyor
        lst_selectedDWSpawnPos.Clear();                         //  Se�ilende Kalmayaca�� ��in Temizleniyor

    }

    public void RandomDmgWallPos()      //  Spawn Konumu Belirleme
    {
        choosenDWPos = Random.Range(0, lst_DWSpawnPos.Count); //  M�sait Noktalardan Rastgele Birini Se�ip Indexini Alma

        lst_selectedDWSpawnPos.Add(lst_DWSpawnPos[choosenDWPos]); //  Se�ileni Birdaha Se�ilmemesi ��in Ba�ka Bir Listeye Indexi �zerinden Aktarma, Mevcut Listeden Silme Kullan�ld�ktan Sonra Ger�ekle�ecek
    }

    public void DeleteDmgWalls()
    {
        foreach (var d in lst_dmgWalls)     //  Listedeki T�m Nesneleri Bulup,
        {
            Destroy(d.gameObject);              //  Yok Edip,
        }
        lst_dmgWalls.Clear();                   //  Listeyi Temizleme.
    }

}
