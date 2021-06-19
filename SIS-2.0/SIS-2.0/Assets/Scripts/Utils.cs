using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Utils : MonoBehaviour
{
    public static int GetMoney() {
		string path;
		if (Application.isPlaying)
			path = "../infos.txt";
		else
			path = "../Assets/infos.txt";
		int i = 0;
		StreamReader sr = new StreamReader(path);
		string s = "";
		while((s = sr.ReadLine()) != null) {
			if(i == 0) {
				return Int32.Parse(s);
			}
		}
		sr.Close();
		throw new ArgumentNullException("Missing Line in infos file");
	}
}
