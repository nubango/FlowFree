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

            category.SetCategoryName(categoryPackage.categoryName);
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

                if (i > 0)
                    levels[i].SetLocked(levelPackage.locked);
                else
                    levels[i].SetLocked(false);
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
                string[] vaciosS = header[5].Split(':'); 

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
