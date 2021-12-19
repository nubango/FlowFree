using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Flow
{
    public class ParseCategoyPackage
    {
        public static Logic.Category Parse(LevelPack.CategoryPackage categoryPackage)
        {
            Logic.Category category = new Logic.Category();
            Logic.Package[] package = new Logic.Package[categoryPackage.levelsPackage.Length];

            for (int i = 0; i < categoryPackage.levelsPackage.Length; i++)
            {
                package[i] = ParsePack(categoryPackage.levelsPackage[i]);
            }

            category.SetCategoryColor(categoryPackage.categoryColor);
            category.SetPackage(package);

            return category;
        }
        private static Logic.Package ParsePack(LevelPack.LevelPackage levelPackage)
        {
            Logic.Package package = new Logic.Package();
            string[] textLevels = levelPackage.maps.text.Split('\n');
            Logic.Level[] levels = new Logic.Level[textLevels.Length];

            for (int i = 0; i < textLevels.Length - 1; i++)
            {
                levels[i] = ParseLevel(textLevels[i]);
                levels[i].SetLevelColor(GameManager.Instance().currentSkin.colores[((i / 30) + 1) % 5]);
            }

            package.SetMaps(levels);
            package.SetPackName(levelPackage.packName);

            return package;
        }
        private static Logic.Level ParseLevel(string lineaLevel)
        {
            Logic.Level l = new Logic.Level();

            int ancho;
            int alto;
            int numLevel;
            int flujos;

            List<Utils.Coord> vacios = new List<Utils.Coord>();
            List<Utils.Wall> muros = new List<Utils.Wall>();
            List<List<Utils.Coord>> tuberias = new List<List<Utils.Coord>>();

            string[] provisional = lineaLevel.Split(';');
            string[] header = provisional[0].Split(',');
            string[] dim = header[0].Split(':', '+', 'B', 'W', '_', 'I');

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

            if (header.Length > 5 && header[5] != "")
            {
                string[] vaciosS = header[5].Split(':'); //OJO PUEDE HABER CARACTERES RAROS

                foreach (var sub in vaciosS)
                {
                    string prueba = sub;
                    int prueba2 = Int32.Parse(prueba);
                    Utils.Coord xy = IntToPos(prueba2, ancho);

                    vacios.Add(xy);
                }
            }

            if (header.Length > 6 && header[6] != "")
            {
                //9:12+B,0,31,9,,,45|54
                string[] murosS = header[6].Split(':');
                for (int i = 0; i < murosS.Length; i++)
                {
                    string[] casillas = murosS[i].Split('|');
                    Utils.Wall wall = new Utils.Wall();

                    int a = Int32.Parse(casillas[0]);
                    wall.init = IntToPos(a, ancho);

                    int b = Int32.Parse(casillas[1]);
                    wall.end = IntToPos(b, ancho);

                    muros.Add(wall);
                }
            }

            for (int i = 1; i < provisional.Length; i++)
            {
                string[] tuberia = provisional[i].Split(',');
                var tuberiaDeInt = new List<Utils.Coord>();
                for (int e = 0; e < tuberia.Length; e++)
                {
                    int prov = Int32.Parse(tuberia[e]);
                    Utils.Coord xy = IntToPos(prov, ancho);
                    tuberiaDeInt.Add(xy);
                }
                tuberias.Add(tuberiaDeInt);
            }

            l.SetAncho(ancho);
            l.SetAlto(alto);
            l.SetNumLevel(numLevel);
            l.SetFlujos(flujos);
            l.SetVacios(vacios);
            l.SetMuros(muros);
            l.SetTuberias(tuberias);
            l.SetRecord(0);

            return l;
        }
        private static Utils.Coord IntToPos(int num, int ancho)
        {
            return new Utils.Coord(num % ancho, num / ancho);
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
