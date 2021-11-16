using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class GameManager : MonoBehaviour
    {
        public LevelManager levelManager;

        void Awake()
        {
            //if (_instance != null)
            //{
            //    _instance.levelManager = levelManager;
            //}
        }
    }
}