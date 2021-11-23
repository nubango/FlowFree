using UnityEngine;

namespace LevelPack
{
    [CreateAssetMenu(fileName = "levelpack", menuName = "Flow/Level Pack", order = 1)]
    public class LevelPackage : ScriptableObject
    {
        [Tooltip("Nombre del lote")]
        public string packName;
        [Tooltip("Fichero con los niveles")]
        public TextAsset maps;
    }
}