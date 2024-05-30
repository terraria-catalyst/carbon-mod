using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Steamworks;

namespace TeamCatalyst.Carbon.Module.MainContent.Core.Reworks.Ores
{
    public class IronArmorRework : GlobalItem
    {
        // The radius in which enemies need to be to boost your damage reduction.
        public const int IRON_DEFENSE_RADIUS = 150;
        // Amount of defense added for each enemy nearby.
        public const int IRON_DEFENSE = 1;


        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type != ItemID.IronHelmet && head.type != ItemID.AncientIronHelmet)
                return "";
            if (body.type != ItemID.IronChainmail)
                return "";
            if (legs.type != ItemID.IronGreaves)
                return "";
            return "CarbonIron";
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "CarbonIron")
                return;
            player.statDefense -= 1;
            player.setBonus = Language.GetTextValue("Mods.Carbon.SetBonuses.Vanilla.Iron", IRON_DEFENSE);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy(player, true))
                    continue;
                if (npc.SpawnedFromStatue || npc.type == NPCID.TargetDummy)
                    continue;
                if (Vector2.Distance(npc.position, player.position) > IRON_DEFENSE_RADIUS)
                    continue;

                player.statDefense += IRON_DEFENSE;
            }
        }
    }
}
