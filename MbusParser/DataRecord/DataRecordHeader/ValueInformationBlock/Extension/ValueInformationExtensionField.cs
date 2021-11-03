// <copyright file="ValueInformationExtensionField.cs" company="Poul Erik Venø Hansen">
// Copyright (c) Poul Erik Venø Hansen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using MBus.Extensions;

namespace MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension
{
    /// <summary>
    /// Base class for value information extension fields
    /// </summary>
    public abstract class ValueInformationExtensionField : ValueInformationField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueInformationExtensionField"/> class.
        /// </summary>
        /// <param name="fieldByte">the fieldbyte of the VIFE</param>
        protected ValueInformationExtensionField(byte fieldByte)
            : base(fieldByte)
        {
        }
    }

    public class PrimaryValueInformationExtensionField: ValueInformationExtensionField
    {
        public PrimaryValueInformationExtensionField(byte fieldByte) : base(fieldByte)
        {
            Parse();
        }

        internal PrimaryValueInformationExtension Type { get; private set; }

        private bool SetType(byte vif)
        {
            Type = (PrimaryValueInformationExtension)vif;
            var success = Enum.IsDefined(typeof(PrimaryValueInformationExtension), Type);
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
                case PrimaryValueInformationExtension.MultiplicativeCorrectionFactor:
                    Multiplier = baseMultiplier - 6;
                    break;
                case PrimaryValueInformationExtension.AdditiveCorrectionConstant:
                    Multiplier = baseMultiplier - 10;
                    break;
            }
        }
    }
}