using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDetail : ScriptableObject
{
    public string tipe, namaLevel, nama;
    public int sublevel, level;
    public AudioClip learningActivity;
    public Sprite backgroundGame;

    //tipe untuk scene unity
    //level (huruf, kata, kalimat)
    //nama (berkaitan tema dan nama GameDetail(atau turunannya))
    //sublevel (sama uniknya dengan nama)
    //tutorial (sama uniknya dengan tipe)
}
