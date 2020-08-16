using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Warframe
{
    public class Window_CraftWarframe : Window
    {
        
        public Window_CraftWarframe(Building_WarframeCrafter WFCraft)
        {
            this.WFCraft = WFCraft;
			nowWarframeKind = PawnKindDef.Named("Warframe_Excalibur");
            newWF = getNewWF();
            RefreshCosts();
        }

        private readonly Building_WarframeCrafter WFCraft;
        private PawnKindDef nowWarframeKind;
        public bool refreshWarframePortrait = false;
        private Pawn newWF;
        private int timeCost = 1;
        private int fuelCost = -1;

        public override void DoWindowContents(Rect inRect)
        {
           
            bool flag = refreshWarframePortrait;
            if (flag)
            {
               // this.newWF.Drawer.renderer.graphics.ResolveAllGraphics();
               // PortraitsCache.SetDirty(this.newWF);
              //  PortraitsCache.PortraitsCacheUpdate();
                refreshWarframePortrait = false;
            }

            Rect rect27 = new Rect(inRect)
            {
                width = 240f + 16f,
                height = 200f + 16f
            };
            rect27 = rect27.CenteredOnXIn(inRect);
           rect27 = rect27.CenteredOnYIn(inRect);
           rect27.x -= 88f;
           rect27.y -= 32f;
           bool flag1 = newWF != null;
           if (flag1)
           {

                //picture
                Rect position = rect27;//new Rect(rect27.xMin + (rect27.width - 200) / 2f - 10f, rect27.yMin + 20f, 200, 200);
                GUI.DrawTexture(position, ContentFinder<Texture2D>.Get("WFPicture/"+nowWarframeKind.defName.Replace("Warframe_","")),ScaleMode.ScaleToFit);
                //  Widgets.InfoCardButton(position.xMax - 16f, position.y, this.newWF);

                Rect rectsk = new Rect(rect27.x+rect27.width+16,rect27.y,50,50);
                Rect recttx = new Rect(rectsk.x+56,rectsk.y+6,160,50);
                for (int i = 0; i < 4; i++)
                {
                    Rect arect = new Rect(rectsk.x,rectsk.y+(i*54f),50,50);
                    Widgets.ButtonImage(arect, ContentFinder<Texture2D>.Get("Skills/" + nowWarframeKind.defName.Replace("Warframe_", "") + "Skill"+(i+1)));
                    Rect trect = new Rect(recttx.x, recttx.y + (i * 54f), 160, 50);
                    Text.Font=GameFont.Medium;
                    Widgets.Label(trect,(nowWarframeKind.defName.Replace("Warframe_", "")+"Skill"+(i+1)+".name").Translate());
                    Text.Font = GameFont.Small;
                    TooltipHandler.TipRegion(arect, (nowWarframeKind.defName.Replace("Warframe_", "") + "Skill" + (i + 1) + ".desc").Translate());

                }
                //title
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0f, 0f, inRect.width, 32f), "CraftWarframe".Translate());
                Text.Anchor = TextAnchor.UpperLeft;




                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Rect rect10 = new Rect(0 + 120, inRect.height * 0.75f, 240f, 24f);
                bool flag16 = Widgets.ButtonText(rect10, nowWarframeKind.label, true, false, true);
                if (flag16)
                {
                    FloatMenuUtility.MakeMenu<PawnKindDef>(WarframeStaticMethods.GetAllWarframeKind(), (PawnKindDef Kind) => Kind.label, (PawnKindDef Kind) => delegate
                    {
                        nowWarframeKind = Kind;
                        newWF = getNewWF();
                        RefreshCosts();
                    });

                }
                Text.Anchor = TextAnchor.UpperLeft;




                //click craft
                //float num2 = rect27.x + rect27.width + 16f + (inRect.width - 1);
                Rect rect9 = new Rect(inRect.width/2 -60,inRect.height-32,120,32);
               Text.Font = GameFont.Medium;
              // Text.Anchor = TextAnchor.LowerCenter;
                bool flag14 = Widgets.ButtonText(rect9, "WarframeStartCraft".Translate(), true, false, true);
               if (flag14)
               {
                    WFCraft.nowCraftKind = nowWarframeKind;

                    WFCraft.tryDropAllParts();
                    WFCraft.fuelCost = fuelCost;
                    WFCraft.curState = Building_WarframeCrafter.CraftState.Filling;
                    Close(true);
               }

                Rect recttime = new Rect(rect9.x,rect9.y-60,30,30);
                Widgets.ButtonImage(recttime, ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Idle", true));
                Rect recttimec = new Rect(recttime.x+30,recttime.y,30,30);
                Text.Font = GameFont.Small;
                Widgets.Label(recttimec,timeCost+ "DaysLower".Translate());
                TooltipHandler.TipRegion(recttime, "Time Cost");


                Rect rectcostbase = new Rect(recttimec.x+30, recttime.y, 30, 30);
                for(int j = 0; j < 3; j++)
                {
                    Rect rrrect = new Rect(rectcostbase.x+(j*30), rectcostbase.y, 30, 30);
                    String nowpart = "";
                    switch (j)
                    {
                        case 0:nowpart = "_Head";break;
                        case 1: nowpart = "_Body"; break;
                        case 2: nowpart = "_Inside"; break;

                    }
                    ThingDef apartDef = ThingDef.Named("WFPart_" + nowWarframeKind.defName.Replace("Warframe_", "") + nowpart);
                    Widgets.ButtonImage(rrrect, apartDef.uiIcon);
                    TooltipHandler.TipRegion(rrrect, apartDef.label+"\n"+apartDef.description);
                }
                //orokin cell icon draw
                Rect fuelRect = new Rect(rectcostbase.x + (3 * 30), rectcostbase.y, 30, 30);
                ThingDef ocpartDef = ThingDef.Named("WFItem_OrokinCell");
                Widgets.ButtonImage(fuelRect, ocpartDef.uiIcon);
                TooltipHandler.TipRegion(fuelRect, ocpartDef.label + "\n" + ocpartDef.description);


                Text.Font = GameFont.Small;
                Rect costRect = new Rect(fuelRect.x + 30, fuelRect.y, 30, 30);
                Widgets.Label(costRect, fuelCost + "");
                TooltipHandler.TipRegion(costRect,ocpartDef.label+"\n"+ocpartDef.description);

                bool flag4 = Widgets.CloseButtonFor(new Rect(inRect.width-30,0,30,30));
                if (flag4)
                {
                   
                    Close(true);
                }

                 
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }



        public void RefreshCosts()
        {
            timeCost = 1;
            fuelCost = WarframeStaticMethods.GetCraftCost(nowWarframeKind);
        }

        private Pawn getNewWF() {
            return WarframeStaticMethods.GetWarframePawn(nowWarframeKind);
        }

    }
}
