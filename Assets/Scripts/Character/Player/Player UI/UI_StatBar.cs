using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SG
{
    public class UI_StatBar : MonoBehaviour
    {
        public Slider slider;
        public RectTransform rectTransform;

        [Header("Bar Options")]
        [SerializeField] protected bool scaleBarLengthWithStats = true;
        [SerializeField] protected float widthScaleMultiplier = 1;


        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }

        protected virtual void Start() 
        {

        }

        public virtual void SetStat(int newValue) 
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue) 
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;

            if (scaleBarLengthWithStats) 
            {
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);

                //resets the position of the bars based on their layout group settings
                PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
            }
        }

    }
}
