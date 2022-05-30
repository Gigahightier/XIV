using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;

namespace XIVSlothComboPlugin.Combos
{
    internal static class BRDPvP
    {
        public const byte ClassID = 41;
        public const byte JobID = 23;

        public const uint
            PowerfulShot = 29391,
            ApexArrow = 29393,
            SilentNocturne = 29395,
            EmpyrealArrow = 29398,
            RepellingShot = 29399,
            WardensPaean = 29400,
            PitchPerfect = 29392,
            BlastArrow = 29394,
            FinalFantasia = 29401;

        public static class Buffs
        {
            public const ushort
                FrontlinersMarch = 3138,
                FrontlinersForte = 3140,
                Repertoire = 3137,
                BlastArrowReady = 3142,
                WardensPaean = 3143;
        }

        internal class BurstShotFeaturePVP : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRDBurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                //GameObject? topTarget = GetPartyMemberTopTarget(inPvP: true);

                //if (topTarget is not null && CurrentTarget != topTarget && IsInRange(topTarget, 26))
                //    TargetObject(topTarget);

                if (TargetHasEffectAny(SAMPvP.Buffs.Chiten))
                    return OriginalHook(SilentNocturne);
		    
                if (actionID == PowerfulShot)
                {
                    if (IsOffCooldown(WardensPaean))
                    {
                        PartyMember? purifyTarget = GetPartyMemberWithPurifiableStatus(yalmDistanceX: 31, inPvP: true);

                        if (purifyTarget is not null)
                        {
                            TargetObject(purifyTarget.GameObject);
                            return OriginalHook(WardensPaean);
                        }
                    }
                    if (!TargetHasEffectAnyNoBurstPVP())
                    {
			var canWeave = CanWeave(actionID, 0.5);
			    
                        if (canWeave)
			{
                            if (IsEnabled(CustomComboPreset.BRDDisengage) && IsOffCooldown(RepellingShot) && GetTargetDistance() < 10)
                                return OriginalHook(RepellingShot);

                            if (GetCooldown(EmpyrealArrow).RemainingCharges == 3)
                                return OriginalHook(EmpyrealArrow);

                            if (!GetCooldown(SilentNocturne).IsCooldown && !TargetHasEffectAny(PVPCommon.Debuffs.Silence) &&
                                !TargetHasEffectAny(PVPCommon.Buffs.Guard) && !TargetHasEffectAny(Buffs.WardensPaean))
                                return OriginalHook(SilentNocturne);
			}

			if (HasEffect(Buffs.BlastArrowReady))
                            return OriginalHook(BlastArrow);

                        if (HasEffect(Buffs.Repertoire))
                            return OriginalHook(PowerfulShot);

                        if (!GetCooldown(ApexArrow).IsCooldown)
                            return OriginalHook(ApexArrow);

                        return OriginalHook(PowerfulShot);
                    } 
                    else
                    {
                        return OriginalHook(PowerfulShot);
                    }
                }
                
                return actionID;
            }
        }
    }
}
