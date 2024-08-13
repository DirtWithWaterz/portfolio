using UnityEngine;

namespace DamianGonzalez {

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class FancyInfoAttribute : PropertyAttribute {
        public readonly string infoText;
        public readonly FancyInfoType type;
        public readonly int height;
        public enum FancyInfoType { Info, Warning, Error }


        public FancyInfoAttribute(string _infoText, FancyInfoType _type = FancyInfoType.Info, int _height = 80) {
            infoText = _infoText;
            type = _type;
            height = _height;
        }
    }
}