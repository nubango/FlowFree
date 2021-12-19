using UnityEngine;

namespace LevelPack
{
    [CreateAssetMenu(fileName = "categorypack", menuName = "Flow/Category Pack", order = 1)]
    public class CategoryPackage : ScriptableObject
    {
        [Tooltip("Nombre de la categoria")]
        public string categoryName;
        [Tooltip("Color de la categoria")]
        public Color categoryColor;
        [Tooltip("Paquetes de la categoria")]
        public LevelPackage[] levelsPackage;
    }
}
