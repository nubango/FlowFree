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
        public bool locked;

        public static bool operator ==(SaveLevel a, SaveLevel b) => (a.category == b.category && a.package == b.package && a.level == b.level);
        public static bool operator !=(SaveLevel a, SaveLevel b) => (a.category != b.category && a.package != b.package && a.level != b.level);
    }

    /// <summary>
    /// Clase que gestiona el guardado y cargado de informacion del juego
    /// </summary>
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

        private void Awake()
        {
            if (_instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            _game = new GameSaving();
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Guarda el estado del juego con los datos pasados por parametro
        /// </summary>
        /// <param name="levels">Lista de niveles pasados</param>
        /// <param name="hints">Numero de pistas</param>
        /// <param name="ads">Si se ha pagado o no para que desaparezcan los anuncios</param>
        public void Save(List<SaveLevel> levels, int hints, bool ads)
        {
            _game.levels = levels;
            _game.hints = hints;
            _game.ads = ads;

            //Cremos el hash concantenando el contenido de la clase y anadiedo una sal al final
            _game.hash = CreateHash((ConcatenateLevels(levels) + hints + ads + Pper()).ToString());
            //Rellenamos el json y lo guardamos
            string jsonData = JsonUtility.ToJson(_game);
            //File.WriteAllText("C:/Users/gonza/Desktop/save.json", jsonData);
            File.WriteAllText(Application.persistentDataPath + "/save.json", jsonData);
        }

        /// <summary>
        /// Carga el proceso del fichero, si hay, guardado
        /// </summary>
        /// <returns>Devuelve TRUE si se ha cargado el fichero y FALSE en caso contrario</returns>
        public bool Load()
        {
            string txt;
            try
            {
                //txt = File.ReadAllText("C:/Users/gonza/Desktop/save.json");
                txt = File.ReadAllText(Application.persistentDataPath + "/save.json");
            }
            catch (Exception)
            {
                return false;
            }

            _game = JsonUtility.FromJson<GameSaving>(txt);

            string hash = CreateHash((ConcatenateLevels(_game.levels) + _game.hints + _game.ads + Pper()).ToString());

            return hash.Equals(_game.hash);
        }

        private string CreateHash(string str)
        {
            System.Security.Cryptography.SHA256Managed SHA256 = new System.Security.Cryptography.SHA256Managed();

            StringBuilder stringHash = new StringBuilder();
            byte[] byteHash = SHA256.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));
            foreach (byte b in byteHash)
            {
                stringHash.Append(b.ToString("x2"));
            }
            return stringHash.ToString().ToLower();
        }

        private string ConcatenateLevels(List<SaveLevel> levels)
        {
            string s = "";
            for (int l = 0; l < levels.Count; l++)
            {
                s += levels[l].category + levels[l].package + levels[l].level + levels[l].record + levels[l].locked.ToString();
            }

            return s;
        }

        private string Pper()
        {
            return "con pimineta todo sabe mejor ;)";
        }
    }
}