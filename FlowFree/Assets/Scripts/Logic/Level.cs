using UnityEngine;
using System.Collections.Generic;
using System;

namespace Flow.Logic
{
    public struct pos
    {
        public int x, y;
    }
    public struct wallPos
    {
        public pos a, b;
    }
    
    public class Level
    {
        int ancho;
        int alto;
        int numLevel;
        int flujos;

        List<pos> vacios;
        List<wallPos> muros;
        List<List<pos>> tuberias;

        private pos intToPos(int num, int ancho)
        {
            pos xy;
            xy.x = num % ancho;
            xy.y = num / ancho;
            return xy;
        }

        public int getAncho(){ return ancho; }
        public int getAlto(){ return alto; }
        public int getNumLevel() { return numLevel; }
        public int getFlujos() { return flujos; }
        public List<pos> getVacios() { return vacios; }
        public List<wallPos> getMuros() { return muros; }
        public List<List<pos>> getTuberias() { return tuberias; }

        public Level(string lineaLevel)
        {
            vacios = new List<pos>();
            muros = new List<wallPos>();
            
            string [] provisional;
            provisional = lineaLevel.Split(';');
            string [] header = provisional[0].Split(',');
            string[] dim = header[0].Split(':', '+', 'B','W','_','I');
            //Debug.Log();
            if (dim.Length > 1)
            {
                ancho = int.Parse(dim[0]);
                alto = int.Parse(dim[1]);
            }
            else
            {
                ancho = int.Parse(dim[0]);
                alto = int.Parse(dim[0]);
            }
            //se ignora header[1] por ser siempre 0
            numLevel = int.Parse(header[2]);
            flujos = int.Parse(header[3]);
            //se ignora header[4] por ser puentes
            /*for(int i = 0; i < header.Length; i++)
            {
                Debug.Log(i+" "+header[i]);
            // x.Parse("9:11+B,0,1,6,,45:46:47:48:50:51:52:53;79,88,87,86,85,84,83,82,73,64,65,66,67,68,59,60,61,62,71;11,12,13,14,15,16,25,34,33,32,31,30,39,38,37,36,27,18,9;24,23,22,21,20,29;0,1,10,19,28;2,3,4,5,6,7,8,17,26,35,44,43,42,41,40,49,58,57,56,55,54,63,72,81,90,91,92,93,94,95,96,97,98,89,80;70,69,78,77,76,75,74");
            }*/
            if (header.Length > 5 && header[5] != "")
            {
                string [] vaciosS = header[5].Split(':');//OJO PUEDE HABER CARACTERES RAROS

                foreach (var sub in vaciosS)
                {                    
                    string prueba = sub;
                    int prueba2 = Int32.Parse(prueba);
                    pos xy = intToPos(prueba2, ancho);
                    
                    vacios.Add(xy);                    
                }
            }
            
            if (header.Length > 6 && header[6] != "")
            {
                //9:12+B,0,31,9,,,45|54
                string[] murosS = header[6].Split(':');                
                for (int i = 0; i < murosS.Length; i++)
                {
                    //muros.Add(murosS[i]);
                    string [] casillas = murosS[i].Split('|');
                    wallPos wall;
                    wall.a.x = wall.a.y = wall.b.x = wall.b.y = 0;
                    
                        //Debug.Log(casillas[0]);
                    int a = Int32.Parse(casillas[0]);
                    wall.a = intToPos(a, ancho);

                    //Debug.Log(casillas[1]);
                    int b = Int32.Parse(casillas[1]);
                    wall.b = intToPos(b, ancho);

                    muros.Add(wall);
                    //Debug.Log(wall.a.x + " " + wall.a.y + " " + wall.b.x + " " + wall.b.y);
                }
            }

            tuberias = new List<List<pos>>();

            for(int i = 1; i < provisional.Length; i++) //vale provisional.Leght y flujos
            {
                string [] tuberia = provisional[i].Split(',');
                var tuberiaDeInt = new List<pos>();
                for(int e = 0; e < tuberia.Length; e++)
                {
                    int prov = Int32.Parse(tuberia[e]);
                    pos xy = intToPos(prov, ancho);
                    tuberiaDeInt.Add(xy);
                }
                tuberias.Add(tuberiaDeInt);
            }
           
            /*foreach(var lista in tuberias)
            {
                foreach (var item in lista)
                {
                    Debug.Log(item.x + " " + item.y);
                }
                Debug.Log("separador");
            }*/
        }
       
    }
}

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
