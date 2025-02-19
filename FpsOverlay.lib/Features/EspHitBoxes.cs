﻿using System.Drawing;
using FpsOverlay.Lib.Data.Internal;
using FpsOverlay.Lib.Data.Raw;
using FpsOverlay.Lib.Gfx;
using FpsOverlay.Lib.Gfx.Math;
using Graphics = FpsOverlay.Lib.Gfx.Graphics;

namespace FpsOverlay.Lib.Features
{
    public static class EspHitBoxes
    {
        /// <summary>
        /// Draw hitboxes.
        /// </summary>
        public static void Draw(Graphics graphics)
        {
            foreach (var entity in graphics.GameData.Entities)
            {
                // validate
                if (!entity.IsAlive() || entity.AddressBase == graphics.GameData.Player.AddressBase || graphics.GameData.Player.Team == entity.Team)
                {
                    continue;
                }

                // draw
                var color = entity.Team == Team.Terrorists ? graphics.GameProcess.GameSetting.TrWallHackColor : graphics.GameProcess.GameSetting.CtWallHackColor;
                Draw(graphics, entity, color);
            }
        }

        /// <summary>
        /// Draw hitboxes of given entity.
        /// </summary>
        public static void Draw(Graphics graphics, Entity entity, Color color)
        {
            for (var i = 0; i < entity.StudioHitBoxSet.numhitboxes; i++)
            {
                var hitbox = entity.StudioHitBoxes[i];
                if (hitbox.bone < 0 || hitbox.bone > Offsets.MAXSTUDIOBONES)
                {
                    return;
                }

                if (hitbox.radius > 0)
                {
                    DrawHitBoxCapsule(graphics, entity, i, color);
                }
            }
        }

        /// <summary>
        /// Draw hitbox as capsule.
        /// </summary>
        private static void DrawHitBoxCapsule(Graphics graphics, Entity entity, int hitBoxId, Color color)
        {
            var hitbox = entity.StudioHitBoxes[hitBoxId];
            var matrixBoneModelToWorld = entity.BonesMatrices[hitbox.bone];

            var bonePos0World = matrixBoneModelToWorld.Transform(hitbox.bbmin);
            var bonePos1World = matrixBoneModelToWorld.Transform(hitbox.bbmax);

            graphics.DrawCapsuleWorld(color, bonePos0World, bonePos1World, hitbox.radius, 6, 3);
        }
    }
}
