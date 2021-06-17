using UnityEngine;

public class StopWatch : MonoBehaviour
{
    private float timeElapsed = 0f;
	private bool isActive = false;
	public bool isCheckWatch;

    public float GetElapsedTime() => timeElapsed;

	public void StartWatch() => isActive = true;

	public void PauseWatch() => isActive = false;

	private void Update() {
		if (isActive)
			timeElapsed += Time.deltaTime;
	}
}