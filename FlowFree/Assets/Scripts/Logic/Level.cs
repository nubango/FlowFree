using UnityEngine;
using System.Collections.Generic;
using System;

namespace Flow.Logic
{
  
    public class Level
    {
        int _ancho;
        int _alto;
        int _numLevel;
        int _flujos;

        List<pos> _vacios;
        List<wallPos> _muros;
        List<List<pos>> _tuberias;

        public int getAncho(){ return _ancho; }
        public int getAlto(){ return _alto; }
        public int getNumLevel() { return _numLevel; }
        public int getFlujos() { return _flujos; }
        public List<pos> getVacios() { return _vacios; }
        public List<wallPos> getMuros() { return _muros; }
        public List<List<pos>> getTuberias() { return _tuberias; }

        public void setAncho(int ancho) { _ancho = ancho; }
        public void setAlto(int alto) {  _alto= alto; }
        public void setNumLevel(int numLevel) {  _numLevel=numLevel; }
        public void setFlujos(int flujos) { _flujos = flujos; }
        public void setVacios(List<pos> vacios) {_vacios=vacios; }
        public void setMuros(List<wallPos> muros) {  _muros = muros; }
        public void setTuberias(List<List<pos>>tuberias) { _tuberias=tuberias; }

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
