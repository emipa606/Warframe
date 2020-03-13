using System;
using System.Collections.Generic;
using System.Text;

namespace Warframe
{

    using HugsLib;
    using HugsLib.Utils;
    using System;
    using System.Collections.Generic;
 

    public class WFModBase : ModBase
    {
        
        private static List<Action> TickActions = new List<Action>();


        public static WFModBase Instance { get; private set; }
        public WarframeControlStorage _WFcontrolstorage;

        public WFModBase()
        {
            Instance = this;
        }

        public static void RegisterTickAction(Action action)
        {
            TickActions.Add(action);
        }

        public override void Tick(int currentTick)
        {
            foreach (Action action in TickActions)
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









        public override string ModIdentifier =>
                "Warframe";
    }


}
