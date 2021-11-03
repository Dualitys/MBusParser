// <copyright file="FDValueInformationExtensionField.cs" company="Poul Erik Venø Hansen">
// Copyright (c) Poul Erik Venø Hansen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MBus.Extensions;
using System;

namespace MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension
{
    /// <summary>
    /// Main VIFE-code extension.
    /// </summary>
    public sealed class FDValueInformationExtensionField : ValueInformationExtensionField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FDValueInformationExtensionField"/> class.
        /// </summary>
        /// <param name="fieldByte">the field byte of the FD-VIFE.</param>
        public FDValueInformationExtensionField(byte fieldByte)
            : base(fieldByte)
        {
            Parse();
        }

        internal FDValueInformationExtension Type { get; private set; }

        private bool SetType(byte vif)
        {
            Type = (FDValueInformationExtension)vif;
            var success = Enum.IsDefined(typeof(FDValueInformationExtension), Type);
            return success;
        }

        private byte DetermineTypeAndMultiplier()
        {
            byte baseMultiplier = 0;

            if (SetType(FieldByte.Mask(ValueInformationMask)))
            {
                return baseMultiplier;
            }
            if (SetType(FieldByte.Mask(ValueInformationMask).Or(LastBitMask)))
            {
                return FieldByte.Mask(ValueInformationMask).Mask(LastBitMask);
            }
            if (SetType(FieldByte.Mask(ValueInformationMask).Or(LastTwoBitsMask)))
            {
                return FieldByte.Mask(ValueInformationMask).Mask(LastTwoBitsMask);
            }
            if (SetType(FieldByte.Mask(ValueInformationMask).Or(LastThreeBitsMask)))
            {
                return FieldByte.Mask(ValueInformationMask).Mask(LastThreeBitsMask);
            }
            if (SetType(FieldByte.Mask(ValueInformationMask).Or(LastFourBitsMask)))
            {
                return FieldByte.Mask(ValueInformationMask).Mask(LastFourBitsMask);
            }
            else
            {
                // TODO HANDLE ERROR
                return baseMultiplier;
            }
        }

        private void Parse()
        {
            var baseMultiplier = DetermineTypeAndMultiplier();

            switch (Type)
            {
                case FDValueInformationExtension.Volts:
                    Multiplier = baseMultiplier - 9;
                    Unit = Unit.V;
                    break;
                case FDValueInformationExtension.Ampere:
                    Multiplier = baseMultiplier - 12;
                    Unit = Unit.A;
                    break;
                case FDValueInformationExtension.CurrencyCredit:
                case FDValueInformationExtension.CurrencyDebit:
                    Multiplier = baseMultiplier - 3;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Type: {Type}";
        }

        private bool Equals(FDValueInformationExtensionField other)
        {
            return base.Equals(other) && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is FDValueInformationExtensionField other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (int)Type;
            }
        }
    }
}