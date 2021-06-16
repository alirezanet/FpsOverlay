using System.Drawing;
using FpsOverlay.Lib.Data.Internal;
using FpsOverlay.Lib.Data.Raw;
using FpsOverlay.Lib.Gfx;
using Graphics = FpsOverlay.Lib.Gfx.Graphics;

namespace FpsOverlay.Lib.Features
{
    /// <summary>
    /// ESP Skeleton.
    /// </summary>
    public static class EspSkeleton
    {
        /// <summary>
        /// Draw skeleton.
        /// </summary>
        public static void Draw(Graphics graphics)
        {
            foreach (var entity in graphics.GameData.Entities)
            {
                // validate
                if (!entity.IsAlive() || entity.AddressBase == graphics.GameData.Player.AddressBase)
                {
                    continue;
                }

                // draw
                var color = entity.Team == Team.Terrorists ? Color.Gold : Color.DodgerBlue;
                Draw(graphics, entity, color);
            }
        }

        /// <summary>
        /// Draw skeleton of given entity.
        /// </summary>
        public static void Draw(Graphics graphics, Entity entity, Color color)
        {
            for (var i = 0; i < entity.SkeletonCount; i++)
            {
                var (from, to) = entity.Skeleton[i];

                // validate
                if (from == to || from < 0 || to < 0 || from >= Offsets.MAXSTUDIOBONES || to >= Offsets.MAXSTUDIOBONES)
                {
                    continue;
                }

                // draw
                graphics.DrawPolylineWorld(color, entity.BonesPos[from], entity.BonesPos[to]);
            }
        }
    }
}
