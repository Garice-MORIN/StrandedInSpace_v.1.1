using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Database : MonoBehaviour
{
    private string _file;
    void OnApplicationQuit()
    {
        _file = "../../StandAlone/infos.txt";
        string path = "https://strandedinspace.000webhostapp.com/pages/infos.txt";
        using (StreamWriter sw = new StreamWriter(path))
        {
            using (StreamReader sr = new StreamReader(_file))
            {
                while (sr.ReadLine()!=null)
                {
                    string line = sr.ReadLine();
                    sw.WriteLine(line, true);
                }
            }
        }
    }

}
