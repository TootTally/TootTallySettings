﻿using BepInEx.Configuration;
using TMPro;
using TootTallyCore.Graphics;
using TootTallyCore.Utils.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace TootTallySettings.TootTallySettingsObjects
{
    public class TootTallySettingColorSliders : BaseTootTallySettingObject
    {
        public Slider sliderR, sliderG, sliderB;
        public TMP_Text labelR, labelG, labelB;

        private float _length;
        private string _text;
        private GameObject _bubble;
        private ConfigEntry<Color> _config;

        public TootTallySettingColorSliders(TootTallySettingPage page, string name, string text, float length, ConfigEntry<Color> config) : base(name, page)
        {
            _text = text;
            _length = length;
            _config = config;
            if (TootTallySettingsManager.isInitialized)
                Initialize();
        }

        public override void Initialize()
        {
            sliderR = TootTallySettingObjectFactory.CreateSlider(_page.gridPanel.transform, name, 0, 2, false);
            sliderG = TootTallySettingObjectFactory.CreateSlider(_page.gridPanel.transform, name, 0, 2, false);
            sliderB = TootTallySettingObjectFactory.CreateSlider(_page.gridPanel.transform, name, 0, 2, false);

            SetSlider(sliderR, _length, _config.Value.r, "Red", out labelR);
            SetSlider(sliderG, _length, _config.Value.g, "Green", out labelG);
            SetSlider(sliderB, _length, _config.Value.b, "Blue", out labelB);
            _bubble = GameObjectFactory.CreateImageHolder(sliderR.transform, new Vector2(200,-60), Vector2.one * 64, AssetManager.GetSprite("PfpMask.png"), "ColorBubble");
            var outline = _bubble.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = Vector2.one * 2f;
            _bubble.GetComponent<Image>().color = new Color(sliderR.value, sliderG.value, sliderB.value);

            var handleTextR = sliderR.transform.Find("Handle Slide Area/Handle/SliderHandleText").GetComponent<TMP_Text>();
            handleTextR.text = SliderValueToText(sliderR, _config.Value.r);
            sliderR.onValueChanged.AddListener((value) => { OnSliderValueChange(sliderB, handleTextR, value); });

            var handleTextG = sliderG.transform.Find("Handle Slide Area/Handle/SliderHandleText").GetComponent<TMP_Text>();
            handleTextG.text = SliderValueToText(sliderG, _config.Value.g);
            sliderG.onValueChanged.AddListener((value) => { OnSliderValueChange(sliderB, handleTextG, value); });

            var handleTextB = sliderB.transform.Find("Handle Slide Area/Handle/SliderHandleText").GetComponent<TMP_Text>();
            handleTextB.text = SliderValueToText(sliderB, _config.Value.b);
            sliderB.onValueChanged.AddListener((value) => { OnSliderValueChange(sliderB,handleTextB, value); });

            base.Initialize();
        }

        public void OnSliderValueChange(Slider s, TMP_Text label, float value)
        {
            label.text = SliderValueToText(s, value);
            UpdateConfig();
            _bubble.GetComponent<Image>().color = new Color(sliderR.value, sliderG.value, sliderB.value);
        }

        public void UpdateConfig()
        {
            _config.Value = new Color(sliderR.value, sliderG.value, sliderB.value);
        }

        public static void SetSlider(Slider s, float length, float value, string text, out TMP_Text label)
        {
            s.GetComponent<RectTransform>().sizeDelta = new Vector2(length, 20);
            var handleText = s.transform.Find("Handle Slide Area/Handle/SliderHandleText").GetComponent<TMP_Text>();
            handleText.rectTransform.anchoredPosition = Vector2.zero;
            handleText.rectTransform.sizeDelta = new Vector2(35, 0);
            handleText.fontSize = 10;
            s.value = value;
            label = GameObjectFactory.CreateSingleText(s.transform, $"{s.name}Label", text);
            label.rectTransform.anchoredPosition = new Vector2(0, 35);
            label.alignment = TextAlignmentOptions.TopLeft;
        }

        public static string SliderValueToText(Slider s, float value)
        {
            string text;

            if (s.minValue == 0 && s.maxValue == 1)
                text = ((int)(value * 100)).ToString() + "%";
            else if (s.wholeNumbers)
                text = value.ToString();
            else
                text = $"{(s.wholeNumbers ? value : value * 100f):0.00}";
            return text;
        }

        public override void Dispose()
        {
            GameObject.DestroyImmediate(sliderR.gameObject);
            GameObject.DestroyImmediate(sliderG.gameObject);
            GameObject.DestroyImmediate(sliderB.gameObject);
        }
    }
}
