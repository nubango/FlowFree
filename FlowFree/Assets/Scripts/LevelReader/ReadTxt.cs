using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadTxt : MonoBehaviour
{   //es la ruta obligada para todos los niveles
    private string _route = "Assets/Levels/levelpack_"; 
    private string _path = ".txt"; //es el trozo de ruta que falta

    /*public void Update()
    {
        readLine(121,"37_jumboCourtyard");
    }*/
    //This method returns a line on the txt
    public string readLine(int level, string pack)
    {
        int linea = 1;
        try
        {
            StreamReader streamReader = new StreamReader(_route + pack + _path);
            string line = "level not selected";
            while (linea <= level)
            {
                line = streamReader.ReadLine();
                linea++;
            }
            streamReader.Close();
            Debug.Log(_route + pack + _path + " : " + line);
            return line;
        }
        catch (System.Exception)
        {
            Debug.Log("Error selecting level");
            throw;
        }
       
    }
    
    
}
