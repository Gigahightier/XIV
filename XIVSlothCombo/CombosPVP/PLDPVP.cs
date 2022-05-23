using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Linq;

namespace XIVSlothComboPlugin.Combos
{
    internal static class PLDPvP
    {
        public const byte ClassID = 1;
        public const byte JobID = 19;

        public const uint
            FastBlade = 29058,
            RiotBlade = 29059,
            RoyalAuthority = 29060,
            Atonement = 29061,
            ShieldBash = 29064,
            Intervene = 29065,
            Guardian = 29066,
            HolySheltron = 29067,
            Phalanx = 29069, //limit break
            Confiteor = 29070,
            BladeOfFaith = 29071,
            BladeOfTruth = 29072,
            BladeOfValor = 29073;

        public static class Buffs
        {
            public const ushort
                HallowedGround = 1302,
                SwordOath = 1991,
                HolySheltron = 3026,
                KnightsResolve = 3188,
                Phalanx = 3210,
                BladeOfFaithReady = 3250;
        }

        public static class Debuffs
        {
            public const ushort
                SacredClaim = 3025;
        }
          
        internal class PLDBurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PLDBurstMode;
            
            private Dictionary<GameObject, double> partyMembersHP = new();
            private GameObject? previousTarget = null;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (partyMembersHP.Count > 0)
                    partyMembersHP = new Dictionary<GameObject, double>();

                if (JustUsed(Guardian) && previousTarget is not null)
                {
                    if (GetTargetHPPercent(previousTarget) > 0)
                        TargetObject(previousTarget);

                    previousTarget = null;
                }

                if (TargetHasEffectAny(SAMPvP.Buffs.Chiten))
                    return OriginalHook(PVPCommon.Sprint);

                if (actionID is FastBlade or RiotBlade or RoyalAuthority or Confiteor or Atonement or Phalanx or BladeOfFaith or BladeOfTruth or BladeOfValor)
                {
                    if (!TargetHasEffectAnyNoBurstPVP())
                    {
                        if (actionID is Phalanx && !TargetHasEffectAny(Debuffs.SacredClaim) && IsOffCooldown(Confiteor))
                            return Confiteor;

                        //var payloads = new List<Payload>()
                        //{
                        //    new TextPayload($"{InMeleeRange().ToString()}")
                        //};

                        //Service.ChatGui.PrintChat(new XivChatEntry
                        //{
                        //    Message = new SeString(payloads),
                        //    Type = XivChatType.Echo
                        //});

                        if (actionID is Phalanx or BladeOfFaith or BladeOfTruth or BladeOfValor || CanWeave(actionID) || CanDelayedWeave(actionID, 2.40, 0.4))
                        {
                            if (IsOffCooldown(Guardian))
                            {
                                foreach (PartyMember? partyMember in GetPartyMembers().Where(partyMember => partyMember.ObjectId != LocalPlayer.ObjectId && partyMember.CurrentHP > 0 && partyMember.CurrentHP < partyMember.MaxHP))
                                {
                                    if (partyMember != null && partyMember.GameObject != null)
                                    {
                                        if (!partyMembersHP.ContainsKey(partyMember.GameObject))
                                        {
                                            if (IsInRange(partyMember.GameObject, 21))
                                                partyMembersHP.Add(partyMember.GameObject, GetTargetHPPercent(partyMember.GameObject));
                                        }
                                        else
                                        {
                                            if (!IsInRange(partyMember.GameObject, 21))
                                                partyMembersHP.Remove(partyMember.GameObject);
                                            else 
                                                partyMembersHP[partyMember.GameObject] = GetTargetHPPercent(partyMember.GameObject);
                                        }
                                    }                                    
                                }

                                if (partyMembersHP.Count > 0)
                                {
                                    GameObject? partyMemberLowestHP = partyMembersHP.Where(partyMember => partyMember.Value > 0 && partyMember.Value < 100)
                                                                                    .OrderBy(partyMember => partyMember.Value).Select(partyMember => partyMember.Key).FirstOrDefault();

                                    if (partyMemberLowestHP is not null)
                                    {
                                        previousTarget = LocalPlayer.TargetObject;
                                        TargetObject(partyMemberLowestHP);
                                        return Guardian;
                                    }                                                                           
                                }
                            }

                            if (!HasEffect(Buffs.SwordOath) || GetBuffStacks(Buffs.SwordOath) <= 2)
                            {
                                if (GetRemainingCharges(Intervene) > 0 && (!InMeleeRange() || IsOnCooldown(ShieldBash)))
                                    return OriginalHook(Intervene);

                                if (IsOffCooldown(ShieldBash) && !TargetHasEffectAny(All.Debuffs.Resilience))
                                    return OriginalHook(ShieldBash);
                            }

                            if (IsOffCooldown(HolySheltron) && !HasEffectAny(Buffs.HallowedGround))
                                return OriginalHook(HolySheltron);
                        }

                        if (!TargetHasEffectAny(Debuffs.SacredClaim) && IsOffCooldown(Confiteor))
                            return Confiteor;                        
                    } 
                    else
                    {
                        if (actionID is Atonement && GetBuffRemainingTime(Buffs.SwordOath) > 3 && GetBuffStacks(Buffs.SwordOath) > 0)
                            return OriginalHook(PVPCommon.Sprint);

                        return OriginalHook(FastBlade);
                    }
                }

                return actionID;
            }

            private bool IsConfiteorOffCooldown()
            {
                return IsOffCooldown(Confiteor) || GetCooldownRemainingTime(Confiteor) < 1;
            }
        }
    }
}