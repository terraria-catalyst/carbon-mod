using Humanizer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TeamCatalyst.Carbon.Module.MainContent.Core.Reworks.Ores
{
    public class SilverArmorRework : GlobalItem
    {
        // The radius of the holy aura.
        public const float HOLY_AURA_RADIUS = 200;
        // Amount of defense reduced by holy aura.
        public const int DEFENSE_REDUCTION = 5;
        // Amount of damage taken per second in holy aura.
        public const int DAMAGE_PER_SECOND = 5;

        internal static int[] Demons = [NPCID.Demon, NPCID.VoodooDemon, NPCID.FireImp, NPCID.RedDevil, NPCID.DemonTaxCollector, NPCID.Krampus, NPCID.WallofFlesh, NPCID.WallofFleshEye];
        internal static int[] UndeadUnholyEnemies = [NPCID.Mummy, NPCID.BloodMummy, NPCID.DarkMummy, NPCID.LightMummy,
                                        NPCID.Ghost, NPCID.PirateGhost, NPCID.Wraith,
                                        NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow,
                                        NPCID.Werewolf, NPCID.PossessedArmor,
                                        NPCID.Nymph, NPCID.LostGirl,
                                        NPCID.Mimic, NPCID.IceMimic, NPCID.PresentMimic,
                                        NPCID.BigMimicCorruption, NPCID.BigMimicCrimson, NPCID.BigMimicHallow, NPCID.BigMimicJungle,
                                        NPCID.Clown, NPCID.GoblinShark, NPCID.BloodEelBody, NPCID.BloodEelHead, NPCID.BloodEelTail, NPCID.BloodSquid, NPCID.BloodNautilus,
                                        NPCID.Vampire, NPCID.VampireBat, NPCID.SwampThing, NPCID.CreatureFromTheDeep, NPCID.Fritz, NPCID.Reaper/*from Overwatch*/,
                                        NPCID.Butcher, NPCID.Nailhead, NPCID.DrManFly, NPCID.Psycho,
                                        NPCID.Paladin];

        public override void SetStaticDefaults()
        {
            NPCID.Sets.Zombies[NPCID.BloodZombie] = true;
            NPCID.Sets.Zombies[NPCID.ZombieMerman] = true;
            NPCID.Sets.Zombies[NPCID.MaggotZombie] = true;
            NPCID.Sets.Zombies[NPCID.ZombieElf] = true;
            NPCID.Sets.Zombies[NPCID.ZombieElfBeard] = true;
            NPCID.Sets.Zombies[NPCID.ZombieElfGirl] = true;
            NPCID.Sets.Zombies[NPCID.Frankenstein] = true;

            NPCID.Sets.DemonEyes[NPCID.Drippler] = true;
            NPCID.Sets.DemonEyes[NPCID.EyeofCthulhu] = true;
            NPCID.Sets.DemonEyes[NPCID.ServantofCthulhu] = true;
            NPCID.Sets.DemonEyes[NPCID.WanderingEye] = true;
            NPCID.Sets.DemonEyes[NPCID.EyeballFlyingFish] = true;

            NPCID.Sets.Skeletons[NPCID.AngryBones] = true; NPCID.Sets.Skeletons[NPCID.AngryBonesBig] = true; NPCID.Sets.Skeletons[NPCID.AngryBonesBigMuscle] = true; NPCID.Sets.Skeletons[NPCID.AngryBonesBigHelmet] = true;
            NPCID.Sets.Skeletons[NPCID.BlueArmoredBones] = true; NPCID.Sets.Skeletons[NPCID.BlueArmoredBonesMace] = true; NPCID.Sets.Skeletons[NPCID.BlueArmoredBonesNoPants] = true; NPCID.Sets.Skeletons[NPCID.BlueArmoredBonesSword] = true;
            NPCID.Sets.Skeletons[NPCID.BoneLee] = true; NPCID.Sets.Skeletons[NPCID.CursedSkull] = true; NPCID.Sets.Skeletons[NPCID.DarkCaster] = true;
            NPCID.Sets.Skeletons[NPCID.DiabolistRed] = true; NPCID.Sets.Skeletons[NPCID.DiabolistWhite] = true;
            NPCID.Sets.Skeletons[NPCID.DungeonGuardian] = true; NPCID.Sets.Skeletons[NPCID.SkeletronHand] = true; NPCID.Sets.Skeletons[NPCID.SkeletronHead] = true;
            NPCID.Sets.Skeletons[NPCID.GiantCursedSkull] = true;
            NPCID.Sets.Skeletons[NPCID.HellArmoredBones] = true; NPCID.Sets.Skeletons[NPCID.HellArmoredBonesMace] = true; NPCID.Sets.Skeletons[NPCID.HellArmoredBonesSpikeShield] = true; NPCID.Sets.Skeletons[NPCID.HellArmoredBonesSword] = true;
            NPCID.Sets.Skeletons[NPCID.DD2SkeletonT1] = true; NPCID.Sets.Skeletons[NPCID.DD2SkeletonT3] = true;
            NPCID.Sets.Skeletons[NPCID.Necromancer] = true; NPCID.Sets.Skeletons[NPCID.NecromancerArmored] = true;
            NPCID.Sets.Skeletons[NPCID.RaggedCaster] = true; NPCID.Sets.Skeletons[NPCID.RaggedCasterOpenCoat] = true;
            NPCID.Sets.Skeletons[NPCID.RuneWizard] = true;
            NPCID.Sets.Skeletons[NPCID.RustyArmoredBonesAxe] = true; NPCID.Sets.Skeletons[NPCID.RustyArmoredBonesFlail] = true; NPCID.Sets.Skeletons[NPCID.RustyArmoredBonesSword] = true; NPCID.Sets.Skeletons[NPCID.RustyArmoredBonesSwordNoArmor] = true;
            NPCID.Sets.Skeletons[NPCID.Tim] = true;
            NPCID.Sets.Skeletons[NPCID.BoneSerpentBody] = true; NPCID.Sets.Skeletons[NPCID.BoneSerpentHead] = true; NPCID.Sets.Skeletons[NPCID.BoneSerpentTail] = true;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type != ItemID.SilverHelmet)
                return "";
            if (body.type != ItemID.SilverChainmail)
                return "";
            if (legs.type != ItemID.SilverGreaves)
                return "";
            return "CarbonSilver";
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "CarbonSilver")
                return;
            player.statDefense -= 2;
            player.setBonus = Language.GetTextValue("Mods.Carbon.SetBonuses.Vanilla.Silver").FormatWith(DEFENSE_REDUCTION);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;
                if (npc.friendly)
                    continue;
                if (npc.dontTakeDamage)
                    continue;
                if (npc.lifeMax <= 5)
                    continue;
                bool unholy = false;
                if (NPCID.Sets.Zombies[npc.type] || NPCID.Sets.DemonEyes[npc.type] || NPCID.Sets.Skeletons[npc.type]) // Zombies, Demon Eyes and Skeletons are undead, thus unholy
                    unholy = true;
                if (Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Halloween) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions.PumpkinMoon) ||
                    npc.type == NPCID.Raven || npc.type == NPCID.HoppinJack) // Halloween is unholy
                    unholy = true;
                if (Demons.Contains(npc.type)) // Demons are unholy
                    unholy = true;
                if (UndeadUnholyEnemies.Contains(npc.type)) // Undead/Unholy enemies are unholy
                    unholy = true;
                if (Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptDesert) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptUndergroundDesert) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptIce)) // The Corruption is unholy
                    unholy = true;
                if (Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCrimson) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonDesert) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonUndergroundDesert) ||
                    Main.BestiaryDB.FindEntryByNPCID(npc.netID).Info.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonIce)) // The Crimson is unholy
                    unholy = true;
                if (!unholy)
                    continue;
                if (Vector2.Distance(npc.position, player.position) > HOLY_AURA_RADIUS)
                    continue;

                npc.AddBuff(ModContent.BuffType<HolyCleansing>(), 2 * 60);
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new();
                double angle = Main.rand.NextDouble() * 2 * Math.PI;
                offset.X += (float)(Math.Sin(angle) * HOLY_AURA_RADIUS);
                offset.Y += (float)(Math.Cos(angle) * HOLY_AURA_RADIUS);
                Vector2 spawnPos = player.Center + offset - new Vector2(4, 4);

                Dust dust = Dust.NewDustPerfect(spawnPos, DustID.GoldFlame, Vector2.Zero);
                dust.noGravity = true;
            }
        }
    }

    public class HolyCleansing : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<HolyCleansingNPC>().Cleansed = true;
        }
    }

    public class HolyCleansingNPC : GlobalNPC
    {
        public bool Cleansed { get; set; }

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            Cleansed = false;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Cleansed)
            {
                modifiers.Defense.Flat -= SilverArmorRework.DEFENSE_REDUCTION;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (Cleansed)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= SilverArmorRework.DAMAGE_PER_SECOND * 2;
                damage = SilverArmorRework.DAMAGE_PER_SECOND;
            }
        }
    }
}
