using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class postProccesController : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;
    private bool colorGradingOff;
    void Start()
    {
        postProcessVolume = GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out colorGrading);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)) {
        colorGradingOff = !colorGradingOff;
        }
        colorGrading.active = colorGradingOff;
    }
}
