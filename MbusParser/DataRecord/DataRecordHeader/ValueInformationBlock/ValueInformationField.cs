﻿// <copyright file="ValueInformationField.cs" company="Poul Erik Venø Hansen">
// Copyright (c) Poul Erik Venø Hansen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MBus.DataRecord.DataRecordHeader.ValueInformationBlock
{
    /// <summary>
    /// Base-class for value information fields.
    /// </summary>
    public abstract class ValueInformationField : InformationField
    {
        protected bool Equals(ValueInformationField other)
        {
            return Multiplier == other.Multiplier && Unit == other.Unit;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValueInformationField)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Multiplier * 397) ^ (int)Unit;
            }
        }

        /// <summary>
        /// A mask for the last four bits of a byte.
        /// </summary>
        protected internal const byte LastFourBitsMask = 0b0000_1111;

        /// <summary>
        /// A mask for the last three bits of a byte.
        /// </summary>
        protected internal const byte LastThreeBitsMask = 0b0000_0111;

        /// <summary>
        /// A mask for the last two bits of a byte.
        /// </summary>
        protected internal const byte LastTwoBitsMask = 0b0000_0011;

        /// <summary>
        /// A mask for the last bit of a byte.
        /// </summary>
        protected internal const byte LastBitMask = 0b0000_0001;

        /// <summary>
        /// A mask for the value information, that is everything except the MSB.
        /// </summary>
        protected internal const byte ValueInformationMask = 0b0111_1111;

        public int Multiplier { get; set; } = 0;

        public Unit Unit { get; set; } = Unit.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueInformationField"/> class.
        /// </summary>
        /// <param name="fieldByte">the byte of the VIF(E)</param>
        protected ValueInformationField(byte fieldByte)
            : base(fieldByte)
        {
        }

        public override string ToString()
        {
            return $"Unit: {Unit}, Multiplier: {Multiplier}";
        }
    }
}