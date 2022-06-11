using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;
using System.IO;
using System.Reflection;

namespace XIVSlothComboPlugin
{
    /// <summary> Dalamud and plugin services. </summary>
    internal class Service
    {
        /// <summary>
        /// Gets or sets the plugin configuration.
        /// </summary>
        public static PluginConfiguration Configuration { get; set; } = null!;

        /// <summary>
        /// Gets or sets the plugin caching mechanism.
        /// </summary>
        public static CustomComboCache ComboCache { get; set; } = null!;

        /// <summary>
        /// Gets or sets the plugin icon replacer.
        /// </summary>
        public static IconReplacer IconReplacer { get; set; } = null!;

        /// <summary>
        /// Gets or sets the plugin address resolver.
        /// </summary>
        public static PluginAddressResolver Address { get; set; } = null!;

        /// <summary>
        /// Gets the Dalamud plugin interface.
        /// </summary>
        [PluginService]
        public static DalamudPluginInterface Interface { get; private set; } = null!;

        /// <summary> Gets the Dalamud buddy list. </summary>
        [PluginService]
        public static BuddyList BuddyList { get; private set; } = null!;

        /// <summary> Gets the Dalamud chat gui. </summary>
        [PluginService]
        public static ChatGui ChatGui { get; private set; } = null!;

        /// <summary> Facilitates class-based locking. </summary>
        internal static bool ClassLocked { get; set; } = true;

        /// <summary> Gets the Dalamud client state. </summary>
        [PluginService]
        public static ClientState ClientState { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud command manager.
        /// </summary>
        [PluginService]
        public static CommandManager CommandManager { get; private set; } = null!;

        /// <summary> Gets the Dalamud condition. </summary>
        [PluginService]
        public static Condition Condition { get; private set; } = null!;

        /// <summary> Gets the Dalamud data manager. </summary>
        [PluginService]
        public static DataManager DataManager { get; private set; } = null!;

        /// <summary> Gets the Dalamud framework manager. </summary>
        [PluginService]
        public static Framework Framework { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud job gauges.
        /// </summary>
        [PluginService]
        public static JobGauges JobGauges { get; private set; } = null!;

        /// <summary> Gets the Dalamud object table. </summary>
        [PluginService]
        public static ObjectTable ObjectTable { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud target manager.
        /// </summary>
        [PluginService]
        public static TargetManager TargetManager { get; private set; } = null!;

        [PluginService]
        public static SigScanner SigScanner { get; private set; } = null!;

        /// <summary> Returns the Plugin Folder location </summary>
        public static string PluginFolder
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path)!;
            }
        }

        /// <summary> Gets the Dalamud party list. </summary>
        [PluginService]
        public static PartyList PartyList { get; private set; } = null!;

        /// <summary> Facilitates searching for memory signatures. </summary>
        [PluginService]
        public static GameGui GameGui { get; private set; } = null!;

    }
}
