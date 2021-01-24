using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class VFXController : MonoBehaviour
{
    [SerializeField] private Slider m_slider;
    [SerializeField] private VisualEffect m_visualEffect;

    // Start is called before the first frame update
    void Start()
    {
        m_slider.onValueChanged.AddListener(OnValueChange);
    }

    private void OnValueChange(float val)
    {
        m_visualEffect.SetFloat("lifetimeMax", val);
    }
}
