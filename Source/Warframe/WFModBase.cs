using System;
using System.Collections.Generic;
using HugsLib;
using HugsLib.Utils;

namespace Warframe
{
    public class WFModBase : ModBase
    {
        private static readonly List<Action> TickActions = new List<Action>();
        public WarframeControlStorage _WFcontrolstorage;

        public WFModBase()
        {
            Instance = this;
        }


        public static WFModBase Instance { get; private set; }


        public override string ModIdentifier =>
            "Warframe";

        public static void RegisterTickAction(Action action)
        {
            TickActions.Add(action);
        }

        public override void Tick(int currentTick)
        {
            foreach (var action in TickActions)
            {
                action();
            }

            TickActions.Clear();
        }

        public override void WorldLoaded()
        {
            _WFcontrolstorage = UtilityWorldObjectManager.GetUtilityWorldObject<WarframeControlStorage>();
            base.WorldLoaded();
        }
    }
}