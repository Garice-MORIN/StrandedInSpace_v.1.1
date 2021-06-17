using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using CryptSharp;
using TreeEditor;

public class Connection : MonoBehaviour
{
    public InputField login;
    public InputField password;
    private string _login;
    private string _password;
    public List<bool>[] Trees;

    public string[] GetInfos(string file)
    {
        string[] arr = new string[2];
        using (StreamReader sr = new StreamReader(file))
        {
            string line = sr.ReadLine();
            arr[0] = line.Split(',')[0];
            arr[1] = line.Split(',')[1];
        }

        return arr;
    }

    public List<bool> GetData(string file, int n)
    {
        List<bool> dataList = new List<bool>();
        using (StreamReader sr = new StreamReader(file))
        {
            for (int i = 0; i < n-1; i++)
            {
                sr.ReadLine();
            }

            string[] data = sr.ReadLine().Split(' ');
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i]=="0")
                {
                    dataList.Add(true);
                }
                dataList.Add(false);
            }
        }

        return dataList;
    }
    public List<bool>[] On_Click()
    {
        string file = "../../StandAlone/infos.txt";
        string[] infos = GetInfos(file);
        _login = login.ToString();
        _password = password.ToString();
        if (infos[0] == _login && Crypter.CheckPassword(_password, infos[1]))
        {
            List<bool> characterTree = GetData(file, 1);
            List<bool> towerTree = GetData(file, 2);
            List<bool> trapTree = GetData(file, 3);
            Trees = new List<bool>[3];
            Trees[0] = characterTree;
            Trees[1] = towerTree;
            Trees[2] = trapTree;
            return Trees;
        }
        Console.Error.WriteLine("Erreur, pas le bon mot de passe/username, merci de réessayer");
        return Trees;
    }
}
