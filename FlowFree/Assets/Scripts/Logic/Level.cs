using UnityEngine;
using System.Collections.Generic;
using System;

namespace Flow.Logic
{

    public class Level
    {
        private int _ancho;
        private int _alto;
        private int _numLevel;
        private int _flujos;

        private List<Utils.Coord> _vacios;
        private List<Utils.Wall> _muros;
        private List<List<Utils.Coord>> _tuberias;

        public int getAncho() { return _ancho; }
        public int getAlto() { return _alto; }
        public int getNumLevel() { return _numLevel; }
        public int getFlujos() { return _flujos; }
        public List<Utils.Coord> getVacios() { return _vacios; }
        public List<Utils.Wall> getMuros() { return _muros; }
        public List<List<Utils.Coord>> getTuberias() { return _tuberias; }

        public void setAncho(int ancho) { _ancho = ancho; }
        public void setAlto(int alto) { _alto = alto; }
        public void setNumLevel(int numLevel) { _numLevel = numLevel; }
        public void setFlujos(int flujos) { _flujos = flujos; }
        public void setVacios(List<Utils.Coord> vacios) { _vacios = vacios; }
        public void setMuros(List<Utils.Wall> muros) { _muros = muros; }
        public void setTuberias(List<List<Utils.Coord>> tuberias) { _tuberias = tuberias; }

    }
}

