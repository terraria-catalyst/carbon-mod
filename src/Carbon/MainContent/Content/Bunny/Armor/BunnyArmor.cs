using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace TeamCatalyst.Carbon.MainContent.Content.Bunny.Armor
{
    [AutoloadEquip(EquipType.Head)]
    internal class LeporidTransfiguration : ModItem
    {
        public override string Texture => BunnySet + Name;
        public override void SetDefaults()
        {
            Item.defense = 2;
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 40;

            int regenBonus = Math.Max(10, (player.statManaMax2 / player.statManaMax) * 10);
            player.manaRegenBonus += regenBonus;

            player.runAcceleration *= 3f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => head.type == this.Type && body.type == ItemType<HoppersFurs>() && legs.type == ItemType<Jackboots>();

        public override void UpdateArmorSet(Player player)
        {
            float damageMultiplier = (player.statMana / player.statManaMax2) / 10f;

            player.magicDamage *= damageMultiplier;
        }
    }
    [AutoloadEquip(EquipType.Body)]
    internal class HoppersFurs : ModItem
    {
        public override string Texture => BunnySet + Name;
        public override void SetDefaults()
        {
            Item.defense = 3;
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 20 + (player.statLifeMax / 8);

            player.GetModPlayer<BunnyPlayer>().HasBunnyChest = true;
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    internal class Jackboots : ModItem
    {
        public override string Texture => BunnySet + Name;
        public override void SetDefaults()
        {
            Item.defense = 2;
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateEquip(Player player)
        {
            player.jumpSpeedBoost += 0.5f;
        }
    }

    public class WitcheryBuff : ModBuff
    {
        public override string Texture => BunnyBuffs + Name;
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.GameUpdateCount % 10 == 0)
                Dust.NewDust(npc.Center, npc.width, npc.height, DustID.CrystalPulse);
        }
    }

    public class BunnyPlayer : ModPlayer
    {
        internal bool HasBunnyChest;

        public override void ResetEffects()
        {
            HasBunnyChest = false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HasBunnyChest)
            {
                var globalNPC = target.GetGlobalNPC<BunnyNPC>();
                target.AddBuff(BuffType<WitcheryBuff>(), 300);
                globalNPC.WitcheryStacks += 1;

                if (modifiers.DamageType == DamageClass.Magic)
                {
                    modifiers.SourceDamage *= globalNPC.WitcheryStacks / 10;
                }
            }
        }
    }

    public class BunnyNPC : GlobalNPC
    {
        internal int WitcheryStacks;

        const int MAX_WITCHERY_TIME = 100;
        int witcheryTime = 0;

        public override bool InstancePerEntity => true;

        public override void PostAI(NPC npc)
        {
            if (npc.HasBuff<WitcheryBuff>() && WitcheryStacks > 0 && witcheryTime++ >= MAX_WITCHERY_TIME)
            {
                WitcheryStacks--;

                witcheryTime = 0;
            }
        }
    }
}
