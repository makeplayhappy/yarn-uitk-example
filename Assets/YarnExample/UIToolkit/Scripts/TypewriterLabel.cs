// from https://forum.unity.com/threads/maxvisiblecharacters-for-typewriter-effect-any-way-to-recreate-this-in-uitoolkit.1066784/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


    public class TypewriterLabel : VisualElement
        {
            public new class UxmlFactory : UxmlFactory<TypewriterLabel, UxmlTraits> { }
     
            public new class UxmlTraits : VisualElement.UxmlTraits
            {
                readonly UxmlStringAttributeDescription text = new() { name = "text", defaultValue = "" };
     
                readonly UxmlIntAttributeDescription maxVisibleCharacter = new() { name = "maxVisibleCharacters", defaultValue = 0 };
     
                public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
                {
                    get { yield break; }
                }
     
                public override void Init(VisualElement element, IUxmlAttributes attr, CreationContext cc)
                {
                    base.Init(element, attr, cc);
     
                    if(element is not TypewriterLabel typewriterLabel)
                    {
                        return;
                    }
     
                    typewriterLabel.Text = text.GetValueFromBag(attr, cc);
                    typewriterLabel.MaxVisibleCharacters = maxVisibleCharacter.GetValueFromBag(attr, cc);
                }
            }
     
            public string Text
            {
                get => text;
                set
                    {
                        text = value;
                        SetChildText();
                    }
            }
     
            public int MaxVisibleCharacters
            {
                get => maxVisibleCharacters;
                set
                    {
                        maxVisibleCharacters = value;
                        SetChildText();
                    }
            }
     
            public bool IsTextFullyVisible => maxVisibleCharacters >= text?.Length;
     
            readonly Label childLabel;
            string text;
            int maxVisibleCharacters;
     
            public TypewriterLabel()
            {
                childLabel = new Label
                    {
                        enableRichText = true ,
                        name = "type-writter" ,
                        style =
                            {
                                fontSize = style.fontSize
                              , unityFont = style.unityFont
                              , unityFontDefinition = style.unityFontDefinition
                              , color = style.color
                              //, whiteSpace = style.whiteSpace <- this doesn't seem to work
                            , whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal)
                              , unityFontStyleAndWeight = style.unityFontStyleAndWeight
                            }
                    };
                Add(childLabel);
            }
     
            void SetChildText()
            {
                if(text?.Length > 0)
                {
                    if( maxVisibleCharacters < text.Length )
                    {
                        //int len = Mathf.Min(maxVisibleCharacters, text.Length);
                        childLabel.text = $"{text[..maxVisibleCharacters]}<alpha=#00>{text[maxVisibleCharacters..]}"; // weirdly alpha has no closing tag
                    }
                    else
                    {
                        childLabel.text = text;
                    }
                    
                }
                else
                {
                    childLabel.text = string.Empty;
                }
            }
        }
