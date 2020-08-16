using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace Warframe
{
    // Token: 0x020006FE RID: 1790
    [StaticConstructorOnStartup]
    public class Gizmo_WarframeBeltStatus : Gizmo
    {
        // Token: 0x060026F5 RID: 9973 RVA: 0x001283D8 File Offset: 0x001267D8
        public Gizmo_WarframeBeltStatus()
        {
            order = -100f;
        }

        // Token: 0x060026F6 RID: 9974 RVA: 0x001283EB File Offset: 0x001267EB
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        // Token: 0x060026F7 RID: 9975 RVA: 0x001283F4 File Offset: 0x001267F4
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(984688+orderadd, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);
                Rect rect2 = rect;
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;





                
                //type
                float fillPercent=-1;
                Texture2D tex = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0.43f, 0.78f));
                String title = "Shield";
                if (btype == "HP")
                {
                    fillPercent = shield.HP / shield.MHP;
                    tex = SolidColorMaterials.NewSolidColorTexture(new Color(0.6f, 0.1f, 0.12f));
                    title = "HP";
                }
                else if (btype == "SP")
                {
                    fillPercent = shield.SP / shield.MSP;
                    tex = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0.76f, 0.85f));
                    title = "SP";
                }
                else
                    fillPercent = shield.Energy / shield.EnergyMax;
                


                Widgets.Label(rect2, title);
                Rect rect3 = rect;
                rect3.yMin = overRect.height / 2f;





                Widgets.FillableBar(rect3, fillPercent, tex, EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                String wlabel = "Error";
                if (btype == "HP")
                    wlabel = shield.HP.ToString("F0") + " / " + shield.MHP.ToString("F0");
                else if (btype == "SP")
                    wlabel = shield.SP.ToString("F0") + " / " + shield.MSP.ToString("F0");
                else
                    wlabel = (shield.Energy * 100f).ToString("F0") + " / " + (shield.EnergyMax * 100f).ToString("F0");

                    Widgets.Label(rect3, wlabel);
                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }

        // Token: 0x0400160C RID: 5644
        public WarframeBelt shield;
        public String btype;
        public int orderadd;
        

        // Token: 0x0400160D RID: 5645
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        // Token: 0x0400160E RID: 5646
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    }
}
