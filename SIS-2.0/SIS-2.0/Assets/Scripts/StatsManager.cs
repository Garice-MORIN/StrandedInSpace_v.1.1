using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;

	public int money;
	public List<bool>[] trees;

	private void Awake() {

		if(instance == null) {
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
		else if(instance != this) {
			Destroy(gameObject);
		}
	}
}
