using System;
using Microsoft.DirectX;
using FpsOverlay.Lib.Gfx.Math;
using FpsOverlay.Lib.Utils;
using FpsOverlay.Lib.Data.Raw;

namespace FpsOverlay.Lib.Data.Internal
{
    /// <summary>
    /// Player data.
    /// </summary>
    public class Player :
        EntityBase
    {
        #region // storage

        /// <summary>
        /// Matrix from world space to clipping space.
        /// </summary>
        public Matrix MatrixViewProjection { get; private set; }

        /// <summary>
        /// Matrix from clipping space to screen space.
        /// </summary>
        public Matrix MatrixViewport { get; private set; }

        /// <summary>
        /// Matrix from world space to screen space.
        /// </summary>
        public Matrix MatrixViewProjectionViewport { get; private set; }

        /// <summary>
        /// Local offset from model origin to player eyes.
        /// </summary>
        public Vector3 ViewOffset { get; private set; }

        /// <summary>
        /// Eye position (in world).
        /// </summary>
        public Vector3 EyePosition { get; private set; }

        /// <summary>
        /// Eye direction (in world).
        /// </summary>
        public Vector3 EyeDirection { get; private set; }

        /// <summary>
        /// View angles (in degrees).
        /// </summary>
        public Vector3 ViewAngles { get; private set; }

        /// <summary>
        /// Aim punch angles (in degrees).
        /// </summary>
        public Vector3 AimPunchAngle { get; private set; }

        /// <summary>
        /// Aim direction (in world).
        /// </summary>
        public Vector3 AimDirection { get; private set; }

        /// <summary>
        /// Player vertical field of view (in degrees).
        /// </summary>
        public int Fov { get; private set; }

        public WeaponIds ActiveWeapon { get; set; }

        #endregion

        #region // routines

        /// <inheritdoc />
        protected override IntPtr ReadAddressBase(GameProcess gameProcess)
        {
            return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayer);
        }
        
        /// <inheritdoc />
        public override bool Update(GameProcess gameProcess, Team? playerTeam = null)
        {
            // we don't have team yet
            if (!base.Update(gameProcess, null))
            {
                return false;
            }

            // get matrices
            MatrixViewProjection = Matrix.TransposeMatrix(gameProcess.ModuleClient.Read<Matrix>(Offsets.dwViewMatrix));
            MatrixViewport = GfxMath.GetMatrixViewport(gameProcess.WindowRectangleClient.Size);
            MatrixViewProjectionViewport = MatrixViewProjection * MatrixViewport;

            // read data
            ViewOffset = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_vecViewOffset);
            EyePosition = Origin + ViewOffset;
            ViewAngles = gameProcess.Process.Read<Vector3>(gameProcess.ModuleEngine.Read<IntPtr>(Offsets.dwClientState) + Offsets.dwClientState_ViewAngles);
            AimPunchAngle = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_aimPunchAngle);
            Fov = gameProcess.Process.Read<int>(AddressBase + Offsets.m_iFOVStart);
           
            var weapon = gameProcess.Process.Read<int>(AddressBase + Offsets.m_hActiveWeapon );
            var weaponEnt = gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwEntityList + ((weapon & 0xFFF) -1) * 0x10);
            ActiveWeapon = (WeaponIds)gameProcess.Process.Read<int>(weaponEnt + Offsets.m_iItemDefinitionIndex);
            
            if (Fov == 0) Fov = 90; // correct for default

            // calc data
            EyeDirection = GfxMath.GetVectorFromEulerAngles(ViewAngles.X.DegreeToRadian(), ViewAngles.Y.DegreeToRadian());
            AimDirection = GfxMath.GetVectorFromEulerAngles
            (
                (ViewAngles.X + AimPunchAngle.X * Offsets.weapon_recoil_scale).DegreeToRadian(),
                (ViewAngles.Y + AimPunchAngle.Y * Offsets.weapon_recoil_scale).DegreeToRadian()
            );

            return true;
        }

        #endregion
    }

    public enum WeaponIds
    {
        WEAPON_INVALID = -1,
        WEAPON_DEAGLE = 1,
        WEAPON_ELITE,
        WEAPON_FIVESEVEN,
        WEAPON_GLOCK,
        WEAPON_AK47 = 7,
        WEAPON_AUG,
        WEAPON_AWP,
        WEAPON_FAMAS,
        WEAPON_G3SG1,
        WEAPON_GALILAR = 13,
        WEAPON_M249,
        WEAPON_M4A1 = 16,
        WEAPON_MAC10,
        WEAPON_P90 = 19,
        WEAPON_MP5 = 23,
        WEAPON_UMP45,
        WEAPON_XM1014,
        WEAPON_BIZON,
        WEAPON_MAG7,
        WEAPON_NEGEV,
        WEAPON_SAWEDOFF,
        WEAPON_TEC9,
        WEAPON_TASER,
        WEAPON_HKP2000,
        WEAPON_MP7,
        WEAPON_MP9,
        WEAPON_NOVA,
        WEAPON_P250,
        WEAPON_SHIELD,
        WEAPON_SCAR20,
        WEAPON_SG556,
        WEAPON_SSG08,
        WEAPON_KNIFEGG,
        WEAPON_KNIFE,
        WEAPON_FLASHBANG,
        WEAPON_HEGRENADE,
        WEAPON_SMOKEGRENADE,
        WEAPON_MOLOTOV,
        WEAPON_DECOY,
        WEAPON_INCGRENADE,
        WEAPON_C4,
        WEAPON_HEALTHSHOT = 57,
        WEAPON_KNIFE_T = 59,
        WEAPON_M4A1_SILENCER,
        WEAPON_USP_SILENCER,
        WEAPON_CZ75A = 63,
        WEAPON_REVOLVER,
        WEAPON_TAGRENADE = 68,
        WEAPON_FISTS,
        WEAPON_BREACHCHARGE,
        WEAPON_TABLET = 72,
        WEAPON_MELEE = 74,
        WEAPON_AXE,
        WEAPON_HAMMER,
        WEAPON_SPANNER = 78,
        WEAPON_KNIFE_GHOST = 80,
        WEAPON_FIREBOMB,
        WEAPON_DIVERSION,
        WEAPON_FRAG_GRENADE,
        WEAPON_SNOWBALL,
        WEAPON_BUMPMINE,
        WEAPON_BAYONET = 500,
        WEAPON_KNIFE_FLIP = 505,
        WEAPON_KNIFE_GUT,
        WEAPON_KNIFE_KARAMBIT,
        WEAPON_KNIFE_M9_BAYONET,
        WEAPON_KNIFE_TACTICAL,
        WEAPON_KNIFE_FALCHION = 512,
        WEAPON_KNIFE_SURVIVAL_BOWIE = 514,
        WEAPON_KNIFE_BUTTERFLY,
        WEAPON_KNIFE_PUSH,
        WEAPON_KNIFE_URSUS = 519,
        WEAPON_KNIFE_GYPSY_JACKKNIFE,
        WEAPON_KNIFE_STILETTO = 522,
        WEAPON_KNIFE_WIDOWMAKER,
        GLOVE_STUDDED_BLOODHOUND = 5027,
        GLOVE_T_SIDE = 5028,
        GLOVE_CT_SIDE = 5029,
        GLOVE_SPORTY = 5030,
        GLOVE_SLICK = 5031,
        GLOVE_LEATHER_WRAP = 5032,
        GLOVE_MOTORCYCLE = 5033,
        GLOVE_SPECIALIST = 5034,
        GLOVE_HYDRA = 5035
    };
}
