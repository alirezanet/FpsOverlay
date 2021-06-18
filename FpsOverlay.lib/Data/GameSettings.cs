using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpsOverlay.lib
{
    public class GameSettings
    {
        public GameSettings()
        {
            AimSetting = new AimSettings();
        }
        public bool ShowAimCrossHair { get; set; }
        public bool ShowFps { get; set; }
        public bool ShowOverlayBorder { get; set; }
        public WallHackModes WallHackMode { get; set; }
        public Color BorderColor { get; set; }
        public Color CtWallHackColor { get; set; }
        public Color TrWallHackColor { get; set; }
        public AimSettings AimSetting { get; set; }

        public enum WallHackModes
        {
            Disable = 0,
            Skeleton = 1,
            HitBoxes = 2
        }


        public class AimSettings
        {
            public float Fov { get; set; } = 10f;
            public short BoneId { get; set; } = 8;
            public short Smoothness { get; set; } = 3;
        }
    }
}
