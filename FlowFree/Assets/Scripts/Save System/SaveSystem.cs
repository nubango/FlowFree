using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Flow
{
    public class GameSaving
    {
        [SerializeField] public List<SaveLevel> levels;
        [SerializeField] public int hints;
        [SerializeField] public bool ads;
        [SerializeField] public string hash;
    }

    [System.Serializable]
    public class SaveLevel
    {
        public int category;
        public int package;
        public int level;
        public int record;
        public bool active;
    }

    [System.Serializable]
    public class SaveSystem : MonoBehaviour
    {
        private static SaveSystem _instance;

        private GameSaving _game;

        public GameSaving GetGame() { return _game; }
        public static SaveSystem Instance()
        {
            return _instance;
        }

        void Awake()
        {
            if (_instance != null)
            {
                _instance._game = _game;
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            _game = new GameSaving();
            DontDestroyOnLoad(gameObject);
        }

        /*Serializamos la clase*/
        public void Save(List<SaveLevel> levels, int hints, bool ads)
        {
            _game.levels = levels;
            _game.hints = hints;
            _game.ads = ads;

            //Cremos el hash concantenando el contenido de la clase y anadiedo una sal al final
            _game.hash = CreateHash((ConcatenateLevels(levels) + hints + ads).ToString());
            //Rellenamos el json y lo guardamos
            string jsonData = JsonUtility.ToJson(_game);
            File.WriteAllText("C:/Users/gonza/Desktop/save.json", jsonData);
            //File.WriteAllText(Application.persistentDataPath + "/save.json", jsonData);
        }

        //Devuelve el objeto creado leyendo el Json especificado
        public bool Load()
        {
            string txt = "";
            try
            {
                txt = File.ReadAllText("C:/Users/gonza/Desktop/save.json");
            }
            catch (Exception)
            {
                return false;
            }

            _game = JsonUtility.FromJson<GameSaving>(txt);
            //game = JsonUtility.FromJson<GameSaving>(File.ReadAllText(Application.persistentDataPath + "/save.json"));

            string hash = CreateHash((ConcatenateLevels(_game.levels) + _game.hints + _game.ads).ToString());

            return hash.Equals(_game.hash);
        }

        public string CreateHash(string str)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();

            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));
            foreach (byte bit in crypto)
            {
                hash.Append(bit.ToString("x2"));
            }
            return hash.ToString().ToLower();
        }

        public string ConcatenateLevels(List<SaveLevel> levels)
        {
            string s = "";
            for (int l = 0; l < levels.Count; l++)
            {
                s += levels[l].category + levels[l].package + levels[l].level + levels[l].record + levels[l].active.ToString();
            }

            return s;
        }
    }
}