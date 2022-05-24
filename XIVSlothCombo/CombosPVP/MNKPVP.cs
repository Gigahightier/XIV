?using Dalamud.Game.ClientState.Objects.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class MNKPvP
    {
        public const byte ClassID = 2;
        public const byte JobID = 20;

        public const uint
            PhantomRushCombo = 55,
            Bootshine = 29472,
            TrueStrike = 29473,
            SnapPunch = 29474,
            DragonKick = 29475,
            TwinSnakes = 29476,
            Demolish = 29477,
            PhantomRush = 29478,
            SixSidedStar = 29479,
            Enlightenment = 29480,
            RisingPhoenix = 29481,
            RiddleOfEarth = 29482,
            ThunderClap = 29484,
            EarthsReply = 29483,
            Meteordrive = 29485;

        public static class Buffs
        {
            public const ushort
                WindResonance = 2007,
                FireResonance = 3170,
                EarthResonance = 3171,
                Thunderclap = 3173;
        }

        public static class Debuffs
        {
            public const ushort
                PressurePoint = 3172;
        }

        internal class MNKBurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MNKBurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                //GameObject? topTarget = GetPartyMemberTopTarget(inPvP: true);

                //if (topTarget is not null && CurrentTarget != topTarget && IsInRange(topTarget, 21))
                    //TargetObject(topTarget);

                if (TargetHasEffectAny(SAMPvP.Buffs.Chiten))
                    return OriginalHook(PVPCommon.Sprint);

                if (actionID is Bootshine or TrueStrike or SnapPunch or DragonKick or TwinSnakes or Demolish or PhantomRush or Enlightenment)
                {
                    //uint globalAction = PVPCommon.ExecutePVPGlobal.ExecuteGlobal(actionID);

                    //if (!TargetHasEffectAnyNoBurstPVP())
                    //{
                        //var payloads = new List<Payload>()
                        //{
                        //    new TextPayload($"{HasEffectAny(1991).ToString()}")
                        //};

                        //Service.ChatGui.PrintChat(new XivChatEntry
                        //{
                        //    Message = new SeString(payloads),
                        //    Type = XivChatType.Echo
                        //});

                        if (CanWeave(actionID) || CanDelayedWeave(actionID, 2, 0.4))
                        {
                            if (JustUsed(Enlightenment))
                                return OriginalHook(Meteordrive);

                            if (InMeleeRange())
                            {
                                if (IsOffCooldown(SixSidedStar) && !TargetHasEffectAny(All.Debuffs.Resilience) && !TargetHasEffectAny(PVPCommon.Debuffs.Stun) &&
                                    (lastComboMove is not Demolish or PhantomRush) && !HasEffect(Buffs.FireResonance))
                                    return OriginalHook(SixSidedStar);

                                if (GetRemainingCharges(RisingPhoenix) > 0 && !HasEffect(Buffs.FireResonance) &&
                                    ((lastComboMove is Demolish && !IsEnlightenmentOffCooldown()) || IsEnlightenmentOffCooldown()))
                                    return OriginalHook(RisingPhoenix);
                            }

                            if (IsEnabled(CustomComboPreset.MNKRiddleOfEarthOption) && HasEffect(Buffs.EarthResonance) && (GetBuffRemainingTime(Buffs.EarthResonance) < (PlayerHealthPercentageHp() <= 25 ? 7 : 4)))
                                return OriginalHook(EarthsReply);
                        }

                        if (HasEffect(Buffs.FireResonance))
                        {
                            if (lastComboMove is Demolish && IsEnlightenmentOffCooldown())
                                return OriginalHook(PhantomRush);

                            if (IsEnlightenmentOffCooldown())
                                return OriginalHook(Enlightenment);
                        }

                        if (IsEnabled(CustomComboPreset.MNKRiddleOfEarthOption) && IsOffCooldown(RiddleOfEarth) && PlayerHealthPercentageHp() <= 95)
                            return OriginalHook(RiddleOfEarth);

                        if (IsEnabled(CustomComboPreset.MNKThunderClapOption) && (!HasEffect(Buffs.WindResonance) || !HasEffect(Buffs.Thunderclap)) && 
                            GetRemainingCharges(ThunderClap) > 1 && lastComboMove is not Demolish)
                            return OriginalHook(ThunderClap);

                        if (IsEnabled(CustomComboPreset.MNKThunderClapOption) && !HasEffect(Buffs.WindResonance) && 
                            GetRemainingCharges(ThunderClap) > 0 && lastComboMove is not Demolish)
                            return OriginalHook(ThunderClap);
                    } 
                    else
                    {
                        return OriginalHook(Bootshine);
                    
                }

                return actionID;
            }
            
            private bool IsEnlightenmentOffCooldown()
            {
                return IsOffCooldown(Enlightenment) || GetCooldownRemainingTime(Enlightenment) < 1;
            }
        }
    }
}
