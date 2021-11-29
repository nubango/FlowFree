using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLineSegmentator : MonoBehaviour
{
   /* 
    struct paquetes
    {
        mapa[];
    }
    struct mapa
    {
        line[];
    }
    struct line
    {
        string linea;
        int ancho;
        int alto;
    }

    transformador.cs*/


    //string line;
    char [] segments;

    //hasta el 14 son 4 digitos, tambien el w1-1 = dimension,0,levelnumber,tuberiasNumber;
    //el 15 tiene 4 digitos e incluye ':' para muros? = dimension,b,levelnumber,tuberiasNumber; 25:12,...
    //el 16 al 22,32 = dimension,0,levelnumber,tuberiasNumber;
    //el 23 tiene X, 4 numeros y un hueco entre comas = x_dimension,0,levelnumber,tuberiasNumber,,y serie de dobles puntos;15:25:12:5 etc
    //el 24,29,30,38,40 tiene dimension diferente dimension:dimension,0,levelnumber,tuberiasnumber;
    //el 25 mania vuelve a tener 4 = dimension,0,levelnumber,tuberiasNumber;
    //el 26,31 tiene 4 = dimension,0,levelnumber,tuberiasNumber,,,y la serie del tipo 2|9:1|8:7 etc
    //el 27 tiene W, y ':' W_6:3,0,1,4,, (pueden ser 2 o 3 comas) 0_0:4_0:5_0:12_0:16_0:17_0,3|15:2|14:1|13;
    //el 28,33,43 es una fiesta ke tiene de todo tienen en comun que son 4 digitos y luego dos comas 
    //el 34,36 tiene el primer numero de forma rara = 9:11+B y luego 3 mas con dos comas y seccion de ':'
    //el 35 tiene dimension+B:I,0,levelnumber,tuberiasNumber,,y casillas 81_0B:93_0B: etc
    //el 36,39 jumbo 4 digitos, tres comas y 1|12:2|etc
    //el 37 igual que el 36 pero se sale de una linea
    //levelpack_d1 cambia el 0 del segundo digito por otro numero
    //w1-14 = 11+B,0,11,9,,0_0B:10_0B:110_0B:120_0B,1|12:9|20:11|
    //No se observan mas variaciones

    public char[] segmentThis(string line)
    {
        
        //los trayectos se pueden separar siempre por ";" sin problema

        return segments;
    }

}

//crear una clase no monobehaviour ke condensa la info de un mapa y se abstraiga de como funciona
    /*
     * 
    public TextAsset [] levels;
    void start{
       //levels.ToString();
        
    }



        [System.Serializable]
    public struct level{
    }


    
    





     */