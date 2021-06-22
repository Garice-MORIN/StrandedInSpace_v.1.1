using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfPlayers : MonoBehaviour
{
    private Dictionary<string, (int, int)> _stats;

	private void Start() {
		_stats = new Dictionary<string, (int, int)>();
	}

	public void AddPlayer(string name, (int, int) stats) {
		if (!_stats.ContainsKey(name)) {
			_stats[name] = stats;
		}
	}

	public Dictionary<string, (int, int)> GetList() {
		return _stats;
	}
}
