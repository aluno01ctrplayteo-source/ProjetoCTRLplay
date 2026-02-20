using UnityEngine;

public class PerformanceMonitor : MonoBehaviour
{
    float deltaTime = 0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        float memory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);

        GUI.Label(new Rect(10, 10, 400, 25), "FPS: " + Mathf.Ceil(fps));
        GUI.Label(new Rect(10, 30, 400, 25), "Projéteis/s: " + StressShooter.shotsPerSecond);
        GUI.Label(new Rect(10, 50, 400, 25), "Memória (MB): " + memory.ToString("F2"));
    }
}