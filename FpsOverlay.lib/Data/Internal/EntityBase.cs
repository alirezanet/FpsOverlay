﻿using System;
using Microsoft.DirectX;
using FpsOverlay.Lib.Data.Raw;
using FpsOverlay.Lib.Utils;

namespace FpsOverlay.Lib.Data.Internal
{
    /// <summary>
    /// Base class for entities.
    /// </summary>
    public abstract class EntityBase
    {
        #region // storage

        /// <summary>
        /// Address base of entity.
        /// </summary>
        public IntPtr AddressBase { get; protected set; }

        /// <summary>
        /// Life state (true = dead, false = alive).
        /// </summary>
        public bool LifeState { get; protected set; }

        /// <summary>
        /// Health points.
        /// </summary>
        public int Health { get; protected set; }

        /// <inheritdoc cref="Team"/>
        public Team Team { get; protected set; }

        /// <summary>
        /// Model origin (in world).
        /// </summary>
        public Vector3 Origin { get; private set; }

        #endregion

        #region // routines

        /// <summary>
        /// Is entity alive?
        /// </summary>
        public virtual bool IsAlive()
        {
            return AddressBase != IntPtr.Zero &&
                   !LifeState &&
                   Health > 0 &&
                   (Team == Team.Terrorists || Team == Team.CounterTerrorists);
        }

        /// <summary>
        /// Read <see cref="AddressBase"/>.
        /// </summary>
        protected abstract IntPtr ReadAddressBase(GameProcess gameProcess);

        /// <summary>
        /// Update data from process.
        /// </summary>
        public virtual bool Update(GameProcess gameProcess,Team? playerTeam)
        {
            AddressBase = ReadAddressBase(gameProcess);
            if (AddressBase == IntPtr.Zero)
            {
                return false;
            }

            Team = gameProcess.Process.Read<int>(AddressBase + Offsets.m_iTeamNum).ToTeam();
            if (playerTeam!= null && playerTeam == Team) return false; // we don't need to process our team

            LifeState = gameProcess.Process.Read<bool>(AddressBase + Offsets.m_lifeState);
            Health = gameProcess.Process.Read<int>(AddressBase + Offsets.m_iHealth);
            Origin = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_vecOrigin);

            return true;
        }

        #endregion
    }
}
