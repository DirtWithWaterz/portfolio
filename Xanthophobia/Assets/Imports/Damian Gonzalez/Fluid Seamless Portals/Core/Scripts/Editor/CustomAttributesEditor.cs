using UnityEngine;
using UnityEditor;


namespace DamianGonzalez.Portals {
    [CustomPropertyDrawer(typeof(FancyInfoAttribute))]
    public class FancyInfoEditor : DecoratorDrawer {
        public override void OnGUI(Rect position) {

            int _margenVert = 20;
            int _margenHoriz = 10;

            FancyInfoAttribute _attrib = attribute as FancyInfoAttribute;


            //gray background
            Rect _rect = new Rect(
            position.xMin + _margenHoriz,
            position.yMin + _margenVert,
            position.width - (_margenHoriz * 2),
            _attrib.height - (_margenVert * 2)
        );
            EditorGUI.DrawRect(_rect, new Color(.25f, .25f, .25f));



            //text
            var style = new GUIStyle(GUI.skin.label) {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                wordWrap = true,
            };
            EditorGUI.LabelField(_rect, _attrib.infoText, style);



            //left border
            _rect = new Rect(
                position.xMin + _margenHoriz - 2,
                position.yMin + _margenVert,
                2,
                _attrib.height - (_margenVert * 2)
            );

            Color _color = Color.white;
            switch (_attrib.type) {
                case FancyInfoAttribute.FancyInfoType.Info:
                    _color = new Color(.25f, .8f, .5f);
                    break;

                case FancyInfoAttribute.FancyInfoType.Warning:
                    _color = new Color(.8f, .6f, .1f);
                    break;

                case FancyInfoAttribute.FancyInfoType.Error:
                    _color = new Color(.8f, .25f, .25f);
                    break;
            }

            EditorGUI.DrawRect(_rect, _color);

        }

        public override float GetHeight() {
            FancyInfoAttribute _attrib = attribute as FancyInfoAttribute;
            return _attrib.height;
        }
    }
}