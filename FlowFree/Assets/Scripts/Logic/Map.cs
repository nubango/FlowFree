using UnityEngine;
using System.Collections.Generic;

namespace Flow.Logic
{
    public class Map
    {
        public int ancho;
        public int alto;
        public int numLevel;
        public int flujos;

        public List<int> vacios;
        public List<string> muros;
        public List<int>[] tuberias;
        //int [flujos, casillanum] caminos;

        public bool Parse(string lineaLevel)
        {
            string [] provisional;
            provisional = lineaLevel.Split(';');
            string [] header = provisional[0].Split(',');
            string[] dim = header[0].Split(':', '+', 'B','W','_');
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
            
            if (header[5] != "")
            {
                string [] vaciosS = header[5].Split(':');//OJO PUEDE HABER CARACTERES RAROS
                
                for (int i = 0; i < vaciosS.Length; i++)
                {
                    vacios.Add(int.Parse(vaciosS[i]));                    
                }
            }
            if (header[6] != "")
            {
                string[] murosS = header[6].Split(':');
                for (int i = 0; i < murosS.Length; i++)
                    muros.Add(murosS[i]);
            }
            //tuberias
            for(int i = 7; i < provisional.Length; i++) //vale provisional.Leght y flujos
            {
                tuberias = tuberias;
            }
            return false;
        }
        //get to lo de arriba y mas;
    }
}