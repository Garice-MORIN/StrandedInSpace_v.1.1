using UnityEngine;
using UnityEngine.UI;
//using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Linq;


public class Connection : MonoBehaviour
{
    //string db = "server=localhost;userid=id16853390_root;password=KIujyh84000!; database=id16853390_players	";
    public InputField login;
    public InputField password;
    private string _login;
    private string _password;
    //private bool show = false;

    public void On_Click()
    {
        _login = login.text;
        _password = password.text;
        //MySqlConnection con = new MySqlConnection(db);
        //con.Open();

        //MySqlCommand retrieve = new MySqlCommand("SELECT * FROM members WHERE members_id=_login", con);
        //MySqlDataReader dr = retrieve.ExecuteReader();

    }
}
