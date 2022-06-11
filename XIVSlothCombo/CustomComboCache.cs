using System;
using System.Collections.Generic;

using Dalamud.Game;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVSlothComboPlugin;

/// <summary>
/// Cached conditional combo logic.
/// </summary>
internal class CustomComboCache : IDisposable
{
    protected const uint InvalidObjectID = 0xE000_0000;
    
    private bool disposed;

    // Invalidate these
    private readonly Dictionary<(uint StatusID, uint? TargetID, uint? SourceID), Status?> statusCache = new();
    private readonly Dictionary<uint, CooldownData> cooldownCache = new();

    // Do not invalidate these
    private readonly Dictionary<uint, byte> cooldownGroupCache = new();
    private readonly Dictionary<Type, JobGaugeBase> jobGaugeCache = new();
    private readonly Dictionary<(uint ActionID, uint ClassJobID, byte Level), (ushort CurrentMax, ushort Max)> chargesCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomComboCache"/> class.
    /// </summary>

	#region Core/setup

	private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

	public CustomComboCache()
    {
		Service.Framework.Update += this.invalidateCache;
	}

	public void Dispose()
    {
		if (this.disposed)
			return;
		this.disposed = true;

		Service.Framework.Update -= this.invalidateCache;
		this.jobGaugeCache?.Clear();
	}

	#endregion

    /// <summary>
    /// Get a job gauge.
    /// </summary>
    /// <typeparam name="T">Type of job gauge.</typeparam>
    /// <returns>The job gauge.</returns>
    internal T GetJobGauge<T>() where T : JobGaugeBase
    {
        if (!this.jobGaugeCache.TryGetValue(typeof(T), out var gauge))
                gauge = this.jobGaugeCache[typeof(T)] = Service.JobGauges.Get<T>();

        return (T)gauge;
    }

    /// <summary>
    /// Finds a status on the given object.
    /// </summary>
    /// <param name="statusID">Status effect ID.</param>
    /// <param name="obj">Object to look for effects on.</param>
    /// <param name="sourceID">Source object ID.</param>
    /// <returns>Status object or null.</returns>
    internal Status? GetStatus(uint statusID, GameObject? obj, uint? sourceID)
    {
        var key = (statusID, obj?.ObjectId, sourceID);
        if (this.statusCache.TryGetValue(key, out var found))
            return found;

        if (obj is null)
            return this.statusCache[key] = null;

        if (obj is not BattleChara chara)
            return this.statusCache[key] = null;

        foreach (var status in chara.StatusList)
        {
            if (status.StatusId == statusID && (!sourceID.HasValue || status.SourceID == 0 || status.SourceID == InvalidObjectID || status.SourceID == sourceID))
                return this.statusCache[key] = status;
        }

        return this.statusCache[key] = null;
    }

    /// <summary>
    /// Gets the cooldown data for an action.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>Cooldown data.</returns>
    
    public unsafe CooldownData GetCooldown(uint actionID)
    {
	if (this.cooldownCache.TryGetValue(actionID, out CooldownData found))
	    return found;

	FFXIVClientStructs.FFXIV.Client.Game.ActionManager* actionManager = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
	if (actionManager is null)
	    return this.cooldownCache[actionID] = default;

	byte cooldownGroup = this.getCooldownGroup(actionID);

	FFXIVClientStructs.FFXIV.Client.Game.RecastDetail* cooldownPtr = actionManager->GetRecastGroupDetail(cooldownGroup - 1);
	cooldownPtr->ActionID = actionID;

	CooldownData cd = this.cooldownCache[actionID] = *(CooldownData*)cooldownPtr;
	return cd;
    }

    /// <summary>
    /// Get the maximum number of charges for an action.
    /// </summary>
    /// <param name="actionID">Action ID to check.</param>
    /// <returns>Max number of charges at current and max level.</returns>
    internal unsafe (ushort Current, ushort Max) GetMaxCharges(uint actionID)
    {
        var player = Service.ClientState.LocalPlayer;
        if (player == null)
            return (0, 0);

        var job = player.ClassJob.Id;
        var level = player.Level;
        if (job == 0 || level == 0)
            return (0, 0);

        var key = (actionID, job, level);
        if (this.chargesCache.TryGetValue(key, out var found))
            return found;

        var cur = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.GetMaxCharges(actionID, 0);
        var max = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.GetMaxCharges(actionID, 90);
        return this.chargesCache[key] = (cur, max);
    }

    internal unsafe int GetResourceCost(uint actionID)
    {
        var actionManager = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
        if (actionManager == null)
            return 0;

        var cost = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.GetActionCost(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Spell, actionID, 0, 0, 0, 0);

        return cost;

    }


    private byte getCooldownGroup(uint actionID)
    {
        if (this.cooldownGroupCache.TryGetValue(actionID, out byte cooldownGroup))
            return cooldownGroup;

	Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Action> sheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()!;
        Lumina.Excel.GeneratedSheets.Action row = sheet.GetRow(actionID)!;

        return this.cooldownGroupCache[actionID] = row.CooldownGroup;
    }
    

    private unsafe void invalidateCache(Framework framework)
    {
        this.statusCache.Clear();
        this.cooldownCache.Clear();
    }
}
