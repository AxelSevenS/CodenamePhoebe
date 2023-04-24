using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Weather Profile", menuName = "Environment/Weather Profile", order = 0)]
    public class WeatherProfile : ScriptableObject {

        public float lightLevel = 1.25f;
        public float sunLight = 1.25f;
        public float moonLight = 0.1f;

        [ColorUsage(false, true)] public Color ambientLight = new Color(1f/199f, 0, 1f/57f);

        public bool forcedPrecipitation = false;

        public float temperature = 20f;

        public float windSpeed = 1f;
        [NormalVector] public Vector3 windDirection = new Vector3(1f,0,1f);

    }
}
