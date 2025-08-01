using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Managers")]
    public Player player;
    public GameDirector gameDirector;

    [Header("GameObjects")]
    public GameObject fallArea;
    public GameObject deadZone;

    [Header("DamageWalls")]                 
    public GameObject dmgWallPrefab;        //  Haz�r Bir Nesne Olu�turup Onu Klonlama Tarz� Bir�ey
    public Vector2 dmgWallCount;            //  Rastgele 2 Say� Aras�nda Olu�turmak ��in
    public List<GameObject> lst_dmgWalls;           //  Olu�turulunacak Nesnelerin Tutulaca�� Liste
    public List<GameObject> lst_DWSpawnPos;             //  Mevcut/M�sait Spawnlanma Noktalar�
    public List<GameObject> lst_selectedDWSpawnPos;     //  Se�ilen Ve Birdaha Se�ilmemesi Gereken Spawn Noktalar�
    private int choosenDWPos;                       //  L�stelerdeki Nesnelere Indexinden Eri�mek ��in


    private void Update()
    {

        fallArea.transform.position = new Vector2(player.transform.position.x, fallArea.transform.position.y);
        deadZone.transform.position = new Vector2(player.transform.position.x, deadZone.transform.position.y);

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


            newDmgWall.transform.position =             //  Olu�an Nesnenin Konumunu Se�ilen Spawn Noktas�na Ayarlama
                new Vector2
                (
                    lst_DWSpawnPos[choosenDWPos].transform.position.x , lst_DWSpawnPos[choosenDWPos].transform.position.y + 1
                );


            lst_DWSpawnPos.RemoveAt(choosenDWPos);      //  Se�ilen Konumu M�sait Konumlar�n Listensinden ��karma


            lst_dmgWalls.Add(newDmgWall);   //  Olu�turulan Nesneyi Listesine Ekleme

        }

                    //  Listeyi Yenileme Burada Daha �yi �al���yor Gibi
        lst_DWSpawnPos.AddRange(lst_selectedDWSpawnPos);        //  Nesne Olu�turma Bitti�i Ve Sonras�nda Tekrar Kullan�lmas� ��in M�sait Liste Tekrar Dolduruluyor
        lst_selectedDWSpawnPos.Clear();                         //  Se�ilende Kalmayaca�� ��in Temizleniyor

    }

    public void RandomDmgWallPos()      //  Spawn Konumu Belirleme
    {
        choosenDWPos = Random.Range(0, lst_DWSpawnPos.Count); //  M�sait Noktalardan Rastgele Birini Se�ip Indexini Alma

        Debug.Log("Se�ilen Index:" + choosenDWPos); //  Se�ilen Noktay� Konsolda G�rmek ��in

        lst_selectedDWSpawnPos.Add(lst_DWSpawnPos[choosenDWPos]); //  Se�ileni Birdaha Se�ilmemesi ��in Ba�ka Bir Listeye Indexi �zerinden Aktarma, Mevcut Listeden Silme Kullan�ld�ktan Sonra Ger�ekle�ecek
    }

    public void DeleteDmgWalls()
    {
        foreach (GameObject d in lst_dmgWalls)  //  Listedeki T�m Nesneleri Bulup,
        {
            Destroy(d);                         //  Yok Edip,
        }
        lst_dmgWalls.Clear();                   //  Listeyi Temizleme.
    }

}
