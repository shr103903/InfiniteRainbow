using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class Frame : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text = null;

    public int fontSize = 30;
    public float width, height;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        float ms = Time.deltaTime * 1000.0f;
        string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);

        this.text.text = $"{text}";
    }
}
