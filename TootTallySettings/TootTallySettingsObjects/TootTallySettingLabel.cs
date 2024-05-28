﻿using TMPro;
using TootTallyCore.Graphics;
using UnityEngine;

namespace TootTallySettings.TootTallySettingsObjects
{
    public class TootTallySettingLabel : BaseTootTallySettingObject
    {
        public TMP_Text label;
        private string _text;
        private float _fontSize;
        private FontStyles _fontStyles;
        private TextAlignmentOptions _align;

        public TootTallySettingLabel(TootTallySettingPage page, string name, string text, float fontSize, FontStyles fontStyles, TextAlignmentOptions align) : base(name, page)
        {
            _text = text;
            _fontSize = fontSize;
            _fontStyles = fontStyles;
            _align = align;
            if (TootTallySettingsManager.isInitialized)
                Initialize();
        }
        public override void Initialize()
        {
            label = GameObjectFactory.CreateSingleText(_page.gridPanel.transform, name, _text);
            label.rectTransform.anchoredPosition = Vector2.zero;
            label.rectTransform.pivot = Vector2.one / 2f;
            label.enableWordWrapping = false;
            label.fontSize = _fontSize;
            label.fontStyle = _fontStyles;
            label.alignment = _align;
            label.autoSizeTextContainer = true;
            base.Initialize();
        }

        public void SetText(string text)
        {
            _text = text;
            label.text = text;
        }

        public override void Dispose()
        {
            if (label != null)
                GameObject.DestroyImmediate(label.gameObject);
        }
    }
}
