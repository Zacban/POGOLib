﻿using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using POGOLib.Official.Extensions;
using POGOLib.Official.Net;
using POGOProtos.Map;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;

namespace POGOLib.Official.Pokemon
{
    /// <summary>
    ///     A wrapper class for <see cref="RepeatedField{T}" />.
    /// </summary>
    public class Map
    {

        /// <summary>
        ///     The authenticated <see cref="Session" />.
        /// </summary>
        private readonly Session _session;

        // The last received map cells.
        private RepeatedField<MapCell> _cells;

        // The last received incense pokémon.
        private MapPokemon _incensePokemon;

        internal Map(Session session)
        {
            _session = session;
            _cells = new RepeatedField<MapCell>();
        }

        /// <summary>
        ///     Gets the last received Incense Pokémon from PokémonGo.<br />
        ///     Only use this if you Incense is active.
        /// </summary>
        public MapPokemon IncensePokemon
        {
            get { return _incensePokemon; }
            internal set
            {
                _incensePokemon = value;
            }
        }

        /// <summary>
        ///     Gets the last received <see cref="RepeatedField{MapCell}" /> from PokémonGo.<br />
        ///     Only use this if you know what you are doing.
        /// </summary>
        public RepeatedField<MapCell> Cells
        {
            get { return _cells; }
            internal set
            {
                _cells = value;
                _session.OnMapUpdate();
            }
        }

        public List<FortData> GetFortsSortedByDistance(Func<FortData, bool> filter = null)
        {
            var forts = Cells.SelectMany(f => f.Forts);

            if (filter != null)
                forts = forts.Where(filter);

            var sorted = forts.ToList();
            sorted.Sort((f1, f2) =>
            {
                var f1Coordinate = new GeoCoordinate(f1.Latitude, f1.Longitude);
                var f2Coordinate = new GeoCoordinate(f2.Latitude, f2.Longitude);

                var distance1 = f1Coordinate.GetDistanceTo(_session.Player.Coordinate);
                var distance2 = f2Coordinate.GetDistanceTo(_session.Player.Coordinate);

                return distance1.CompareTo(distance2);
            });

            return sorted;
        }
    }
}