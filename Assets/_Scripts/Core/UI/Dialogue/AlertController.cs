using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SevenGame.Utility;
using System.Threading;
using System.Collections;

namespace SeleneGame.Core.UI {

    public sealed class AlertController : DialogueReader<AlertController> {

        private float timeSinceLineEnded = -1f;

        private const float LINE_MARGIN_DURATION = 1.25f;

        protected override void EndLine() {
            base.EndLine();
            timeSinceLineEnded = 0f;
        }

        protected override void Update() {
            base.Update();

            if (timeSinceLineEnded >= 0f)
                timeSinceLineEnded += Time.deltaTime;

            if (timeSinceLineEnded >= LINE_MARGIN_DURATION) {
                NextLine();
                timeSinceLineEnded = -1f;
            }
        }

    }
}