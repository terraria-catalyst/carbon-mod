using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System;
using MonoMod.Cil;
using TeamCatalyst.Carbon;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Drawing;
using System.Linq;
using Terraria.ModLoader.IO;
using System.Runtime.InteropServices;
using Terraria.GameContent.Bestiary;

namespace TeamCatalyst.Carbon.Module.MainContent.Content.Items.Forest
{
    public class SnailTransformAccessory : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.TurtleShell;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN", SnailTransformPlayer.SHELLED_DAMAGE_REDUCTION * 100, SnailTransformPlayer.TIME_EFFECT_SECONDS);

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 26;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 1);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SnailTransformPlayer>().IsSnail = true;
        }
    }

    public class SnailCooldownBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }

    public class SnailTransformPlayer : ModPlayer
    {
        // Amount of seconds player is in shell.
        public const int TIME_EFFECT_SECONDS = 5;
        // Amount of seconds it takes to shell again.
        public const int TIME_COOLDOWN_SECONDS = 20;

        // How much Damage Reduction is added when in shell.
        public const float SHELLED_DAMAGE_REDUCTION = 0.6f;

        // Amount of seconds mucus lasts after spawning.
        public const int MUCUS_DURATION_SECONDS = 3;
        // Amount of time drained when player is in shell. It drains 1 per second when not shelled.
        public const float MUCUS_SLOWDOWN_IN_SHELL = 0.2f;

        public bool IsSnail { get; set; }
        public int TimeInShell { get; set; }
        public bool CurrentlyInShell => TimeInShell > 0;

        private static Hook? modifyCcedHook; //why is the property called cced why not use the full acronym

        public override void ResetEffects()
        {
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[0] < 15 
                && IsSnail && TimeInShell == 0 && !Player.HasBuff<SnailCooldownBuff>())
            {
                TimeInShell = 60 * 5;
                Player.AddBuff(ModContent.BuffType<SnailCooldownBuff>(), 60 * TIME_COOLDOWN_SECONDS, false);
            }

            IsSnail = false;
        }

        public override void PreUpdate()
        {
            if (CurrentlyInShell)
            {
                TimeInShell--;
            }
        }

        public override void PreUpdateMovement()
        {
            if (CurrentlyInShell)
            {
                Player.velocity.X *= 0.7f;
                Player.maxFallSpeed = 20;
                Player.gravity *= 1.5f;
            }

            if (!IsSnail)
            {
                int x = (int)((Player.position.X + (Player.width / 2)) / 16f);
                int y = Player.gravDir == -1f ? (int)(Player.position.Y - 0.1f) / 16 : (int)((Player.position.Y + Player.height) / 16f);
                Point16 position = new Point16(x, y);
                if (!TileEntity.ByPosition.ContainsKey(position))
                    return;
                if (TileEntity.ByPosition[position] is not SnailMucusTileEntity mucus)
                    return;
                if (Player.team != 0 && Player.team == Main.player[mucus.Owner].team)
                    return;

                if (Player.velocity.X > 1f)
                {
                    Player.velocity.X = 1f;
                }
                if (Player.velocity.X < -1f)
                {
                    Player.velocity.X = -1f;
                }
                if (Player.velocity.X > 0.75 || Player.velocity.X < -0.75)
                {
                    Player.velocity.X *= 0.85f;
                }
                else
                {
                    Player.velocity.X *= 0.6f;
                }
                if (Player.gravDir == -1f)
                {
                    if (Player.velocity.Y < -1f)
                    {
                        Player.velocity.Y = -1f;
                    }
                    if (Player.velocity.Y > 5f)
                    {
                        Player.velocity.Y = 5f;
                    }
                    if (Player.velocity.Y > 0f)
                    {
                        Player.velocity.Y *= 0.96f;
                    }
                    else
                    {
                        Player.velocity.Y *= 0.3f;
                    }
                }
                else
                {
                    if (Player.velocity.Y > 1f)
                    {
                        Player.velocity.Y = 1f;
                    }
                    if (Player.velocity.Y < -5f)
                    {
                        Player.velocity.Y = -5f;
                    }
                    if (Player.velocity.Y < 0f)
                    {
                        Player.velocity.Y *= 0.96f;
                    }
                    else
                    {
                        Player.velocity.Y *= 0.3f;
                    }
                }
            }
        }

        public override void PreUpdateBuffs()
        {
            if (CurrentlyInShell)
            {
                Player.endurance += SHELLED_DAMAGE_REDUCTION;
                Player.noKnockback = true;
            }
        }

        public override void UpdateEquips()
        {
            if (IsSnail)
            {
                int x = (int)Player.Center.X / 16;
                int y = (int)(Player.position.Y + (float)Player.height - 1f) / 16;
                Tile tile = Main.tile[x, y + 1];
                if (tile == null || !tile.HasTile || tile.IsActuated || tile.LiquidAmount > 0)
                    return;

                Point16 position = new Point16(x, y + 1);
                if (!TileEntity.ByPosition.ContainsKey(position))
                {
                    TileEntity.PlaceEntityNet(x, y + 1, ModContent.TileEntityType<SnailMucusTileEntity>());
                }
                if (TileEntity.ByPosition.ContainsKey(position))
                {
                    if (TileEntity.ByPosition[position] is SnailMucusTileEntity mucus)
                    {
                        mucus.TimeLeft = 60 * MUCUS_DURATION_SECONDS;
                        mucus.Owner = Player.whoAmI;
                    }
                }
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (CurrentlyInShell)
            {
                r = (float)TimeInShell / (60 * TIME_EFFECT_SECONDS);
            }
        }

        #region Hooks/edits for parity with Stoned/Webbed/Frozen
        public override void PostUpdateMiscEffects()
        {
            if (CurrentlyInShell)
            {
                Player.pulley = false;
            }
        }

        public override void Load()
        {
            modifyCcedHook = new Hook(typeof(Player).GetProperty("CCed")!.GetGetMethod()!, CcedByShell);
            On_Player.QuickMount += CantMountIfShell;
            On_Player.QuickGrapple += CantGrappleIfShell;
            On_Player.UpdateHairDyeDust += CantHairDustIfShell;
            On_Player.FindPulley += CantFindPulleyIfShell;
            IL_Player.PlayerFrame += DontUpdateFrameIfShell;

            IL_Player.Update += CannotMoveIfShell;
        }

        public override void Unload()
        {
            modifyCcedHook?.Dispose();
            modifyCcedHook = null;
            On_Player.QuickMount -= CantMountIfShell;
            On_Player.QuickGrapple -= CantGrappleIfShell;
            On_Player.UpdateHairDyeDust -= CantHairDustIfShell;
            On_Player.FindPulley -= CantFindPulleyIfShell;
            IL_Player.PlayerFrame -= DontUpdateFrameIfShell;

            IL_Player.Update -= CannotMoveIfShell;
        }

        private void CannotMoveIfShell(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            cursor.GotoNext(MoveType.Before, x => x.MatchLdfld(typeof(Player), nameof(Player.webbed)));
            cursor.Index += 5;
            ILLabel label = cursor.DefineLabel();
            cursor.MarkLabel(label);
            cursor.Index -= 3;

            cursor.EmitLdarg0();
            cursor.EmitDelegate((Player player) => player.GetModPlayer<SnailTransformPlayer>().CurrentlyInShell);
            cursor.EmitBrtrue(label);
        }

        private static void DontUpdateFrameIfShell(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            cursor.GotoNext(MoveType.Before, x => x.MatchRet());
            ModContent.GetInstance<CarbonMod>().Logger.Info(cursor.Next);
            ILLabel label = cursor.DefineLabel();
            cursor.MarkLabel(label);

            cursor.GotoPrev(MoveType.After, x => x.MatchLdfld(typeof(Player), nameof(Player.stoned)));
            cursor.Index++;

            cursor.EmitLdarg0();
            cursor.EmitDelegate((Player player) => player.GetModPlayer<SnailTransformPlayer>().CurrentlyInShell);
            cursor.EmitBrtrue(label);
        }

        private void CantFindPulleyIfShell(On_Player.orig_FindPulley orig, Player self)
        {
            if (self.GetModPlayer<SnailTransformPlayer>().CurrentlyInShell)
                return;
            orig(self);
        }

        private void CantHairDustIfShell(On_Player.orig_UpdateHairDyeDust orig, Player self)
        {
            if (self.GetModPlayer<SnailTransformPlayer>().CurrentlyInShell)
                return;
            orig(self);
        }

        private void CantGrappleIfShell(On_Player.orig_QuickGrapple orig, Player self)
        {
            if (self.GetModPlayer<SnailTransformPlayer>().CurrentlyInShell)
                return;
            orig(self);
        }

        private void CantMountIfShell(On_Player.orig_QuickMount orig, Player self)
        {
            if (self.mount.Active)
            {
                self.mount.Dismount(self);
            }
            else
            {
                if (self.GetModPlayer<SnailTransformPlayer>().CurrentlyInShell)
                    return;
                orig(self);
            }
        }

        private bool CcedByShell(orig_get_CCed orig, Player self)
        {
            return orig(self) && self.GetModPlayer<SnailTransformPlayer>().CurrentlyInShell;
        }

        private delegate bool orig_get_CCed(Player self);
#endregion
    }

    public class SnailMucusNPC : GlobalNPC
    {
        internal static int[] Snails = [NPCID.Snail, NPCID.GlowingSnail, NPCID.MagmaSnail, NPCID.SeaSnail, NPCID.GiantShelly, NPCID.GiantShelly2];
        internal static int[] Slimes = [NPCID.GreenSlime, NPCID.BlueSlime, NPCID.RedSlime, NPCID.YellowSlime, NPCID.PurpleSlime, NPCID.BlackSlime,
                            NPCID.IceSlime, NPCID.SpikedIceSlime, NPCID.JungleSlime, NPCID.SpikedJungleSlime, NPCID.SandSlime, NPCID.LavaSlime,
                            NPCID.MotherSlime, NPCID.BabySlime, NPCID.DungeonSlime, NPCID.Pinky, NPCID.GoldenSlime, NPCID.ShimmerSlime, NPCID.SlimeSpiked, NPCID.UmbrellaSlime,
                            NPCID.SlimeRibbonRed, NPCID.SlimeRibbonGreen, NPCID.SlimeRibbonYellow, NPCID.SlimeRibbonWhite, NPCID.SlimeMasked, NPCID.HoppinJack,
                            NPCID.ToxicSludge, NPCID.Slimer, NPCID.Slimer2, NPCID.Slimeling, NPCID.Crimslime, NPCID.Gastropod, NPCID.IlluminantSlime, NPCID.RainbowSlime,
                            NPCID.QueenSlimeMinionPurple, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionBlue,
                            NPCID.KingSlime, NPCID.QueenSlimeBoss,
                            NPCID.TownSlimeBlue, NPCID.TownSlimeCopper, NPCID.TownSlimeGreen, NPCID.TownSlimeOld, NPCID.TownSlimePurple, NPCID.TownSlimeRainbow, NPCID.TownSlimeYellow, NPCID.TownSlimeRed];

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return !(Snails.Contains(entity.type) || Slimes.Contains(entity.type));
        }

        public override bool PreAI(NPC npc)
        {
            int x = (int)((npc.position.X + (npc.width / 2)) / 16f);
            int y = (int)((npc.position.Y + npc.height) / 16f);
            Point16 position = new Point16(x, y);

            if (!TileEntity.ByPosition.ContainsKey(position))
                return true;
            if (TileEntity.ByPosition[position] is not SnailMucusTileEntity)
                return true;

            npc.velocity.X *= 0.85f;
            npc.velocity.Y *= 0.96f;

            return true;
        }
    }

    public class SnailMucusTileEntity : ModTileEntity
    {
        public float TimeLeft { get; set; }
        public int Owner { get; set; }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.HasUnactuatedTile && tile.LiquidAmount == 0 && Collision.SolidTiles(x, x, y, y);
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        public override void Update()
        {
            Dust dust = Dust.NewDustPerfect(new Vector2(Position.X + Main.rand.NextFloat(), Position.Y + Main.rand.NextFloat()) * 16, DustID.Snail, Vector2.Zero);
            dust.noGravity = true;

            if (Main.player[Owner].GetModPlayer<SnailTransformPlayer>().CurrentlyInShell)
            {
                TimeLeft -= SnailTransformPlayer.MUCUS_SLOWDOWN_IN_SHELL;
            }
            else
            {
                TimeLeft--;
            }
            if (!IsTileValidForEntity(Position.X, Position.Y) || TimeLeft <= 0)
            {
                Kill(Position.X, Position.Y);
                return;
            }
        }
    }
}
