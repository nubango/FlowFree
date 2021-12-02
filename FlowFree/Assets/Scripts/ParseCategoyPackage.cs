using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Flow
{
    public struct pos
    {
        public int x, y;
    }
    public struct wallPos
    {
        public pos a, b;
    }
    public class ParseCategoyPackage
    {

        public static Logic.Category Parse(LevelPack.CategoryPackage cp)
        {
            Logic.Category category = new Logic.Category();
            Logic.Package[] LP = new Logic.Package[cp.categoryPackages.Length];
            for (int i = 0; i < cp.categoryPackages.Length; i++)
            {
                LP[i] = ParsePack(cp.categoryPackages[i]);
            }
            category.SetCategoriColor(cp.categoryColor);
            category.SetPackage(LP);
            // parseamos el paquete y todo lo que contiene
            return category;
        }
        private static Logic.Package ParsePack(LevelPack.LevelPackage LP)
        {
            Logic.Package package = new Logic.Package();
            string h = LP.maps.text;
            string[] textLevels = h.Split('\n');
            Logic.Level[] levels = new Logic.Level[textLevels.Length];
            for (int i = 0; i < textLevels.Length - 1; i++)
            {
                levels[i] = ParseLevel(textLevels[i]);
            }
            package.SetMaps(levels);
            package.SetPackName(LP.packName);
            return package;
        }
        private static pos intToPos(int num, int ancho)
        {
            pos xy;
            xy.x = num % ancho;
            xy.y = num / ancho;
            return xy;
        }
        private static Logic.Level ParseLevel(string lineaLevel)
        {
            Logic.Level l = new Logic.Level();
            int ancho;
            int alto;
            int numLevel;
            int flujos;


            List<pos> vacios = new List<pos>();
            List<wallPos> muros = new List<wallPos>();
            List<List<pos>> tuberias = new List<List<pos>>();


            string[] provisional;
            provisional = lineaLevel.Split(';');
            string[] header = provisional[0].Split(',');
            string[] dim = header[0].Split(':', '+', 'B', 'W', '_', 'I');
            //Debug.Log();
            if (dim.Length > 1)
            {
                ancho = Int32.Parse(dim[0]);
                alto = Int32.Parse(dim[1]);
            }
            else
            {
                ancho = Int32.Parse(dim[0]);
                alto = Int32.Parse(dim[0]);
            }
            //se ignora header[1] por ser siempre 0
            numLevel = Int32.Parse(header[2]);
            flujos = Int32.Parse(header[3]);
            //se ignora header[4] por ser puentes
            /*for(int i = 0; i < header.Length; i++)
            {
                Debug.Log(i+" "+header[i]);
            // x.Parse("9:11+B,0,1,6,,45:46:47:48:50:51:52:53;79,88,87,86,85,84,83,82,73,64,65,66,67,68,59,60,61,62,71;11,12,13,14,15,16,25,34,33,32,31,30,39,38,37,36,27,18,9;24,23,22,21,20,29;0,1,10,19,28;2,3,4,5,6,7,8,17,26,35,44,43,42,41,40,49,58,57,56,55,54,63,72,81,90,91,92,93,94,95,96,97,98,89,80;70,69,78,77,76,75,74");
            }*/
            if (header.Length > 5 && header[5] != "")
            {
                string[] vaciosS = header[5].Split(':');//OJO PUEDE HABER CARACTERES RAROS

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
                    string[] casillas = murosS[i].Split('|');
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

            for (int i = 1; i < provisional.Length; i++) //vale provisional.Leght y flujos
            {
                string[] tuberia = provisional[i].Split(',');
                var tuberiaDeInt = new List<pos>();
                for (int e = 0; e < tuberia.Length; e++)
                {
                    int prov = Int32.Parse(tuberia[e]);
                    pos xy = intToPos(prov, ancho);
                    tuberiaDeInt.Add(xy);
                }
                tuberias.Add(tuberiaDeInt);
            }
            l.setAncho(ancho);
            l.setAlto(alto);
            l.setNumLevel(numLevel);
            l.setFlujos(flujos);
            l.setVacios(vacios);
            l.setMuros(muros);
            l.setTuberias(tuberias);
            return l;
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
