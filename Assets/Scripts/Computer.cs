using TMPro;
using UnityEngine;

namespace VirtualGPU
{
    public class Computer : MonoBehaviour
    {
        [SerializeField] Screen screen;
        [SerializeField] TMP_Text fpsCounter;
        [SerializeField] ApplicationSettings applicationSettings;

        GPU gpu;
        OpenGL openGl;
        Application app;

        void Start()
        {
            gpu = new GPU(screen);
            openGl = new OpenGL(gpu);
            app = new Application(applicationSettings, openGl);

            StartCoroutine(app.RunProgram());
        }

        void Update()
        {
            fpsCounter.text = $"FPS: {app.FPS:F2}";
        }
    }
}
