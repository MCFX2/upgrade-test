using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeLightColor : MonoBehaviour
{
    [SerializeField] private bool randomizeHue = true;

    [SerializeField] private float minHue = 0;
    [SerializeField] private float maxHue = 360;

    // Start is called before the first frame update
    private void Start()
    {
        var localLight = GetComponent<Light>();
        
        Color.RGBToHSV(localLight.color, out var hue, out var sat, out var val);

        if (randomizeHue)
        {
            hue = Random.Range(minHue, maxHue) % 360;
            hue /= 360.0f;
        }
        
        localLight.color = Color.HSVToRGB(hue, sat, val);
    }
}
