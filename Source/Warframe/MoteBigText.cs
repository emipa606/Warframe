using UnityEngine;
using Verse;

namespace Warframe
{
    public class MoteBigText : MoteText
    {
        public GameFont size;

        public override void DrawGUIOverlay()
        {
            var a = 1f - ((AgeSecs - TimeBeforeStartFadeout) / def.mote.fadeOutTime);
            var unused = new Color(textColor.r, textColor.g, textColor.b, a);
            // GenMapUI.DrawText(new Vector2(this.exactPosition.x, this.exactPosition.z), this.text, color);
            var worldPos = new Vector2(exactPosition.x, exactPosition.z);

            var position = new Vector3(worldPos.x, 0f, worldPos.y);
            Vector2 vector = Find.Camera.WorldToScreenPoint(position) / Prefs.UIScale;
            vector.y = UI.screenHeight - vector.y;

            Text.Font = size; //GameFont.Medium;
            GUI.color = textColor;
            Text.Anchor = TextAnchor.UpperCenter;
            var x = Text.CalcSize(text).x;
            Widgets.Label(new Rect(vector.x - (x / 2f), vector.y - 2f, x, 999f), text);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}