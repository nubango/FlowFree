using UnityEngine;
using System.Collections.Generic;
using System;

namespace Flow.Logic
{

    public class Level
    {
        private int _ancho = 0;
        private int _alto = 0;
        private int _numLevel = 0;
        private int _flujos = 0;
        private int _record = 0;
        private Color _levelColor;
        private bool _locked;

        private List<Utils.Coord> _vacios;
        private List<Utils.Wall> _muros;
        private List<List<Utils.Coord>> _tuberias;

        public int GetAncho() { return _ancho; }
        public int GetAlto() { return _alto; }
        public int GetNumLevel() { return _numLevel; }
        public int GetFlujos() { return _flujos; }
        public int GetRecord() { return _record; }
        public Color GetLevelColor() { return _levelColor; }
        public bool IsLocked() { return _locked; }
        public List<Utils.Coord> GetVacios() { return _vacios; }
        public List<Utils.Wall> GetMuros() { return _muros; }
        public List<List<Utils.Coord>> GetTuberias() { return _tuberias; }

        public bool IsPassed() { return _record >= _flujos; }
        public bool IsPerfectPassed() { return _record == _flujos; }

        public void SetAncho(int ancho) { _ancho = ancho; }
        public void SetAlto(int alto) { _alto = alto; }
        public void SetNumLevel(int numLevel) { _numLevel = numLevel; }
        public void SetFlujos(int flujos) { _flujos = flujos; }
        public void SetRecord(int moves) { _record = moves; }
        public void SetLevelColor(Color color) { _levelColor = color; }
        public void SetLocked(bool locked) { _locked = locked; }
        public void SetVacios(List<Utils.Coord> vacios) { _vacios = vacios; }
        public void SetMuros(List<Utils.Wall> muros) { _muros = muros; }
        public void SetTuberias(List<List<Utils.Coord>> tuberias) { _tuberias = tuberias; }

    }
}

