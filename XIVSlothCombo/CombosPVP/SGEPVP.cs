using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothComboPlugin
{
    internal static class SGEPVP
    {
        internal const uint
            Dosis = 29256,
            Phlegma = 29259,
            Pneuma = 29260,
            Eukrasia = 29258,
            Icarus = 29261,
            Toxikon = 29262,
            Kardia = 29264,
            EukrasianDosis = 29257,
            Toxicon2 = 29263;

        internal class Debuffs
        {
            internal const ushort
                EukrasianDosis = 3108,
                Toxicon = 3113,
                Lype = 3120;
        }

        internal class Buffs
        {
            internal const ushort
                Kardia = 2871,
                Kardion = 2872,
                Eukrasia = 3107,
                Addersting = 3115,
                Haima = 3110,
                Haimatinon = 3111,
                Mesotes = 3119;
        }

        internal class SGEBurstMode : CustomCombo
        internal class SGEPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SGEPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (IsOffCooldown(Kardia) && actionID == Dosis)
                {
                    PartyMember? purifyTarget = GetPartyMemberWithPurifiableStatus(yalmDistanceX: 31, inPvP: true);

                    if (purifyTarget is not null && !TargetHasEffectAny(Buffs.Kardion, purifyTarget.GameObject))
                    {
                        TargetObject(purifyTarget.GameObject);
                        return OriginalHook(Kardia);
                    }
                }

                GameObject? topTarget = GetPartyMemberTopTarget(inPvP: true);

                if (topTarget is not null && CurrentTarget != topTarget && IsInRange(topTarget, 26))
                    TargetObject(topTarget);

                if (TargetHasEffectAny(SAMPvP.Buffs.Chiten))
                    return OriginalHook(PVPCommon.Sprint);

                if (actionID == Dosis)
                {
                    if (!HasEffectAny(Buffs.Kardia))
                        return OriginalHook(Kardia);

                    if (!TargetHasEffectAnyNoBurstPVP())
                    {
                        if (GetRemainingCharges(Phlegma) > 0 && !InMeleeRange() && GetRemainingCharges(Icarus) > 0 && IsOnCooldown(Pneuma) && TargetHasEffect(Debuffs.Toxicon))
                            return OriginalHook(Icarus);

                        if (CanWeave(actionID))
                        {
                            if ((!TargetHasEffect(Debuffs.Toxicon) || GetTargetBuffRemainingTime(Debuffs.Toxicon) <= 1) && GetCooldown(Toxikon).RemainingCharges > 0)
                                return OriginalHook(Toxikon);

                            if (HasEffect(Buffs.Addersting) && !HasEffect(Buffs.Eukrasia))
                                return OriginalHook(Toxicon2);

                            if (InMeleeRange() && GetRemainingCharges(Phlegma) == 0 && GetRemainingCharges(Icarus) > 0 && GetCooldownChargeRemainingTime(Phlegma) > 8)
                            {
                                PartyMember? furthestPartyMember = GetFurthestPartyMember(yalmDistanceX: 26);

                                if (furthestPartyMember is not null && GetTargetDistance(furthestPartyMember.GameObject) > 5)
                                {
                                    TargetObject(furthestPartyMember.GameObject);
                                    return OriginalHook(Icarus);
                                }
                            }
                        }

                        if (!GetCooldown(Pneuma).IsCooldown && TargetHasEffect(Debuffs.Toxicon))
                            return OriginalHook(Pneuma);

                        if (InMeleeRange() && !HasEffect(Buffs.Eukrasia) && GetCooldown(Phlegma).RemainingCharges > 0 && TargetHasEffect(Debuffs.Toxicon))
                            return OriginalHook(Phlegma);

                        if (!TargetHasEffectAny(Debuffs.EukrasianDosis) && GetCooldown(Eukrasia).RemainingCharges > 0 && !HasEffect(Buffs.Eukrasia))
                            return OriginalHook(Eukrasia);

                        if (HasEffect(Buffs.Eukrasia))
                            return OriginalHook(Dosis);
                    }
                    else
                    {
                        return OriginalHook(Dosis);
                    }
                }
                return actionID;
            }
        }
    }
}
