using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeamCatalyst.Carbon.Module.MainContent.Core.Reworks.Ores
{
    public class WerewolvesWeakToSilverItems : GlobalItem
    {
        internal static int[] SilverItems = [ItemID.SilverBroadsword, ItemID.SilverPickaxe, ItemID.SilverAxe, ItemID.SilverHammer];

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return SilverItems.Contains(entity.type);
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type != NPCID.Werewolf)
                return;
            modifiers.TargetDamageMultiplier *= 3f;
        }

        public override void ModifyHitPvp(Item item, Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!player.wereWolf)
                return;
            modifiers.FinalDamage *= 3f;
        }
    }

    public class WerewolvesWeakToSilverProjectiles : GlobalProjectile
    {
        internal static int[] SilverProjectiles = [ProjectileID.SilverShortswordStab, ProjectileID.SilverBullet, ProjectileID.SilverCoin];

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return SilverProjectiles.Contains(entity.type);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type != NPCID.Werewolf)
                return;
            if (projectile.type == ProjectileID.SilverBullet) //Don't deal 9x damage with bullets
                return;
            modifiers.TargetDamageMultiplier *= 3f;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!target.wereWolf)
                return;
            modifiers.FinalDamage *= 3f;
        }
    }
}
