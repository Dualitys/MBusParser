﻿// <copyright file="MBusTelegram.cs" company="Poul Erik Venø Hansen">
// Copyright (c) Poul Erik Venø Hansen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using MBus.DataRecord;
using MBus.Header;

namespace MBus
{
    /// <summary>
    /// a wrapper for an entire parsed mbus telegram.
    /// </summary>
    public sealed class MBusTelegram
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MBusTelegram"/> class.
        /// </summary>
        /// <param name="header"><see cref="MBusHeader"/>.</param>
        /// <param name="records">A list of <see cref="MbusRecord"/>'s.</param>
        public MBusTelegram(MBusHeader header, IList<VariableDataRecord> records)
        {
            Header = header;
            Records = records ?? new List<VariableDataRecord>();
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        public MBusHeader Header { get; internal set; }

        /// <summary>
        /// Gets the records.
        /// </summary>
        public IList<VariableDataRecord> Records { get; internal set; }
    }
}