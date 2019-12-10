using UnityEngine;
using UnityEngine.UIElements;

namespace JollySamurai.UnrealEngine4.Import.UI
{
    public class ImportAssetItem : BindableElement, INotifyValueChanged<bool>
    {
        public override VisualElement contentContainer {
            get { return _container; }
        }

        public string text {
            get { return _toggle.text; }
            set { _toggle.text = value; }
        }

        public bool value {
            get { return _value; }
            set {
                if(_value == value) {
                    return;
                }

                using (ChangeEvent<bool> evt = ChangeEvent<bool>.GetPooled(_value, value)) {
                    evt.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(evt);
                }
            }
        }

        public static readonly string ussClassName = "unity-foldout";
        public static readonly string toggleUssClassName = ussClassName + "__toggle";
        public static readonly string contentUssClassName = ussClassName + "__content";

        private Toggle _toggle;
        private VisualElement _container;

        [SerializeField]
        private bool _value;

        public ImportAssetItem()
        {
            _value = true;

            AddToClassList(ussClassName);

            _toggle = new Toggle {
                value = _value
            };

            _toggle.RegisterValueChangedCallback((evt) => {
                value = _toggle.value;
                evt.StopPropagation();
            });

            _toggle.AddToClassList(toggleUssClassName);
            hierarchy.Add(_toggle);

            _container = new VisualElement() {
                name = "unity-content",
            };

            _container.AddToClassList(contentUssClassName);
            hierarchy.Add(_container);
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            _value = newValue;
            _toggle.value = _value;
            contentContainer.style.display = newValue ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
