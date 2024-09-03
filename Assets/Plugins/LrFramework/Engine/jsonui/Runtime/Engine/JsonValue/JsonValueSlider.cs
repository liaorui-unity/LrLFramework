using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Sailfish
{

    /// <summary>
    /// 进度条布局
    /// </summary>
    public class JsonLayoutSliderAttribute : JsonLayoutAttribute
    {
        internal float min;
        internal float max;

        public JsonLayoutSliderAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
            layoutType = typeof(JsonValueSlider);
        }
    }



    public class JsonValueSlider : JsonGUIValue
    {

        public float tempValue;
        public float sliderValue;
        public float min;
        public float max;
        public JsonValueSlider(Type type, object value, string name) : base(type, value, name)
        {
            tempValue = sliderValue = float.Parse(value.ToString());
        }

        public override void JsonLayoutData(JsonLayoutAttribute layoutData)
        {
            var slider = (JsonLayoutSliderAttribute)layoutData;
            min = slider.min;
            max = slider.max;
        }


        public override void CustomGUI()
        {
            GUILayout.BeginHorizontal();
            TitleLayout();

            tempValue = GUILayout.HorizontalSlider(tempValue, min, max);
            GUILayout.TextField(tempValue.ToString(),GUILayout.MaxWidth(60));

            GUILayout.EndHorizontal();

            if (sliderValue != tempValue)
            {
                value = Convert.ChangeType(tempValue, type);
                sliderValue = tempValue = float.Parse(value.ToString());
            }
        }
    }
}
