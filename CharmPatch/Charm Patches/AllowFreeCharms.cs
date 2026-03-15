namespace CharmPatch.Charm_Patches
{
    /// <summary>
    /// Allow Zero Notches allows you to equip a charm with 0 notches, even if all notches are fileld
    /// </summary>
    public class AllowFreeCharms : Patch
    {
        public bool IsActive => SharedData.globalSettings.allowFreeCharmsOn;

        public void Start()
        {
            On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += CheckCost;
        }

        /// <summary>
        /// When the Charms UI FSM checks if a slot is open, we will check the cost of the current charm.
        /// If it is 0, we will modify the check so that it believes there is an opening
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void CheckCost(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, 
                                HutongGames.PlayMaker.Actions.IntCompare self)
        {
            int defaultValue = self.integer2.Value;

            // First, confirm the patch is active and we're looking at the right FSM action
            if (IsActive && 
                self.Fsm.Name.Equals("UI Charms") && 
                self.State.Name.Equals("Slot Open?"))
            {
                // Get the charm's notch cost. If it's 0, modify the check so it doesn't stop the FSM chain
                string playerDataVarName = self.Fsm.Variables.FindFsmString("PlayerData Var Name").Value;
                int notchCost = PlayerData.instance.GetInt(playerDataVarName);
                if (notchCost == 0)
                {
                    self.integer2.Value = self.integer1.Value + 1;
                }
            }

            orig(self);

            // Reset afterwards
            self.integer2.Value = defaultValue;
        }
    }
}
