using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe
{
    public class MoteBigText:MoteText
    {
        public override void DrawGUIOverlay()
        {
            float a = 1f - (base.AgeSecs - this.TimeBeforeStartFadeout) / this.def.mote.fadeOutTime;
            Color color = new Color(this.textColor.r, this.textColor.g, this.textColor.b, a);
           // GenMapUI.DrawText(new Vector2(this.exactPosition.x, this.exactPosition.z), this.text, color);
            Vector2 worldPos = new Vector2(this.exactPosition.x, this.exactPosition.z);

            Vector3 position = new Vector3(worldPos.x, 0f, worldPos.y);
            Vector2 vector = Find.Camera.WorldToScreenPoint(position) / Prefs.UIScale;
            vector.y = (float)UI.screenHeight - vector.y;

            Text.Font = size;//GameFont.Medium;
            GUI.color = textColor;
            Text.Anchor = TextAnchor.UpperCenter;
            float x = Text.CalcSize(text).x;
            Widgets.Label(new Rect(vector.x - x / 2f, vector.y - 2f, x, 999f), text);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public GameFont size;
    }
}
