using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System.IO;
using System;
using System.Data;
using System.Drawing;
using System.Linq;


public class Connection : MonoBehaviour
{
    string db = "server=localhost;userid=id16853390_root;password=KIujyh84000!; database=id16853390_players	";
    public InputField login;
    public InputField password;
    private string _login;
    private string _password;
    public Text ErrorMessage;

    public void On_Click()
    {
        _login = login.text;
        _password = password.text;
        MySqlConnection con = new MySqlConnection(db);
        try
        { con.Open(); }
        catch (IOException Ex)
        {
            ErrorMessage.text = Ex.ToString()+" Please try again.";
        }
    }
}
