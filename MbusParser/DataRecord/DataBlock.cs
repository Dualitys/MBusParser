// <copyright file="DataBlock.cs" company="Poul Erik Venø Hansen">
// Copyright (c) Poul Erik Venø Hansen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MBus.DataRecord.DataRecordHeader;
using MBus.DataRecord.DataRecordHeader.DataInformationBlock;
using MBus.DataRecord.DataRecordHeader.ValueInformationBlock;
using MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension;
using MBus.Extensions;
using MBus.Helpers;

namespace MBus.DataRecord
{
    /// <summary>
    /// An mbus datablock.
    /// </summary>
    public sealed class DataBlock
    {
        public Unit Unit { get; private set; }

        public object Value { get; private set; }

        public ValueDescription ValueDescription { get; private set; }

        public byte[] Data { get; private set; }

        internal DataInformationField DataInformationField { get; }

        /// <summary>
        /// Gets DIFEs.
        /// </summary>
        internal List<DataInformationExtensionField> DataInformationFieldExtensions { get; }

        /// <summary>
        /// Gets VIF.
        /// </summary>
        internal PrimaryValueInformationField ValueInformationField { get; }

        /// <summary>
        /// Gets VIFEs.
        /// </summary>
        internal List<ValueInformationExtensionField> ValueInformationFieldExtensions { get; }

        public DataBlock(byte[] data, DataInformationField dataInformationField, List<DataInformationExtensionField> dataInformationFieldExtensions, PrimaryValueInformationField valueInformationField, List<ValueInformationExtensionField> valueInformationFieldExtensions)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            DataInformationField = dataInformationField ?? throw new ArgumentNullException(nameof(dataInformationField));
            DataInformationFieldExtensions = dataInformationFieldExtensions ?? throw new ArgumentNullException(nameof(dataInformationFieldExtensions));
            ValueInformationField = valueInformationField ?? throw new ArgumentNullException(nameof(valueInformationField));
            ValueInformationFieldExtensions = valueInformationFieldExtensions ?? throw new ArgumentNullException(nameof(valueInformationFieldExtensions));

            Parse();
        }

        private void Parse()
        {
            ValueDescription = FindDecription();
            Unit = FindUnit();
            var multiplier = FindMultiplier();
            Value = ParseValue(multiplier);
        }

        private int FindMultiplier()
        {
            if (ValueInformationFieldExtensions.Any(e => e.Multiplier != 0))
            {
                return ValueInformationFieldExtensions.First().Multiplier;
            }

            return ValueInformationField.Multiplier;
        }

        private Unit FindUnit()
        {
            var lastVife = ValueInformationFieldExtensions.LastOrDefault(x => x.Unit != Unit.None);

            if (lastVife != null && lastVife.Unit != Unit.None)
            {
                return lastVife.Unit;
            }

            return ValueInformationField.Unit;
        }

        private object? ParseValue(int multiplier)
        {
            var valueType = DataInformationField.DataField;

            if (Unit == Unit.Date || Unit == Unit.DateTime || Unit == Unit.DateTimeSecondary || Unit == Unit.Time)
            {
                return ParseDateTime();
            }

            object? value;
            switch (valueType)
            {
                case DataField.EightBitInteger:
                case DataField.SixteenBitInteger:
                case DataField.TwentyFourBitInteger:
                case DataField.ThirtyTwoBitInteger:
                case DataField.FourtyEightBitInteger:
                case DataField.SixtyFourBitInteger:
                    value = ParseInteger(multiplier);
                    break;
                case DataField.ThirtyTwoBitReal:
                    value = ParseReal(multiplier);
                    break;
                case DataField.TwoDigitBinaryCodedDecimal:
                case DataField.FourDigitBinaryCodedDecimal:
                case DataField.SixDigitBinaryCodedDecimal:
                case DataField.EightDigitBinaryCodedDecimal:
                case DataField.TwelveDigitBinaryCodedDecimal:
                    value = ParseBCD(multiplier);
                    break;
                case DataField.VariableLength:
                    value = ParseString(multiplier);
                    break;
                default:
                    value = null;
                    break;
            }

            if (ValueInformationFieldExtensions.FirstOrDefault(x => x is OrthogonalValueInformationExtensionField) is OrthogonalValueInformationExtensionField orthogonal)
            {
                switch (orthogonal.Type)
                {
                    case OrthogonalValueInformationExtension.MultiplicativeCorrectionFactorMinusSix:
                    case OrthogonalValueInformationExtension.MultiplicativeCorrectionFactorThree:
                        value = ApplyOrthogonalCorrections(value, orthogonal.Multiplier, isMultiplication: true);
                        break;
                    case OrthogonalValueInformationExtension.AdditiveCorrectionConstant:
                        value = ApplyOrthogonalCorrections(value, orthogonal.Multiplier, isMultiplication: false);
                        break;
                }
            }

            return value;
        }

        private object? ApplyOrthogonalCorrections(object? value, int multiplier, bool isMultiplication)
        {
            if (isMultiplication)
            {
                if (value is float floatValue)
                {
                    return floatValue * multiplier;
                }

                if (value is double doubleValue)
                {
                    return doubleValue * multiplier;
                }
            }
            else
            {
                if (value is float floatValue)
                {
                    return floatValue + multiplier;
                }

                if (value is double doubleValue)
                {
                    return doubleValue + multiplier;
                }
            }

            return value;
        }

        private DateTime ParseDateTime()
        {
            DateTime result;
            switch (DataInformationField.DataField)
            {
                case DataField.SixteenBitInteger:
                    {
                        var day = Data[0] & 0x1f;
                        var month = (Data[1] & 0x0f);
                        var year = 100 + (((Data[0] & 0xe0) >> 5) | ((Data[1] & 0xf0) >> 1));

                        if (year < 70)
                            year += 2000;
                        else
                            year += 1900;

                        result = month == 0 || day == 0 ? DateTime.MinValue : new DateTime(year, month, day);
                    }
                    break;
                case DataField.ThirtyTwoBitInteger:
                    {
                        var minute = Data[0] & 0x3f;
                        var hour = Data[1] & 0x1f;
                        var day = Data[2] & 0x1f;
                        var month = (Data[3] & 0x0f);
                        var year = 100 + (((Data[2] & 0xe0) >> 5) | ((Data[3] & 0xf0) >> 1));

                        if (year < 70)
                            year += 2000;
                        else
                            year += 1900;

                        if (month == 0 || day == 0)
                            result = DateTime.MinValue;
                        else
                            result = new DateTime(year, month, day, hour, minute, 0);
                    }
                    break;
                case DataField.FourtyEightBitInteger:
                    {
                        var second = Data[0] & 0x3f;
                        var minute = Data[1] & 0x3f;
                        var hour = Data[2] & 0x1f;
                        var day = Data[3] & 0x1f;
                        var month = (Data[4] & 0x0f);
                        var year = 100 + (((Data[3] & 0xe0) >> 5) | ((Data[4] & 0xf0) >> 1));  //(((temp >> 25) & 0x38) | ((temp >> 21) & 0x07));

                        if (year < 70)
                            year += 2000;
                        else
                            year += 1900;

                        var valid = (Data[1] & 0x80) == 0;
                        var summer = (Data[1] & 0x8000) == 0;

                        if (month == 0 || day == 0)
                            result = DateTime.MinValue;
                        else
                            result = new DateTime(year, month, day, hour, minute, second);
                    }
                    break;
                default:
                    throw new NotImplementedException(nameof(DataInformationField.DataField));
            }
            return result;
        }

        private string ParseString(int multiplier)
        {
            return Encoding.ASCII.GetString(Data);//TODO: Add more variable length parsers
        }

        private double ParseBCD(int multiplier)
        {
            //return Convert.ToInt64(Data.Reverse().ToArray().ToHexString()) * Math.Pow(10, multiplier);
            return long.Parse(Data.Reverse().ToArray().ToHexString()) * Math.Pow(10, multiplier);
        }

        private float ParseReal(int multiplier)
        {
            return Convert.ToSingle(ValueAsLong() * Math.Pow(10, multiplier));
        }

        private double ParseInteger(int multiplier)
        {
            return ValueAsLong() * Math.Pow(10, multiplier);
        }

        private long ValueAsLong()
        {
            var length = Data.Length;
            if (length == 0)
            {
                return 0;
            }

            var correctEndian = Data;
            switch (length)
            {
                case 1: return Data[0];
                case 2: return BitConverter.ToInt16(correctEndian, 0);
                case 3: return BitConverter.ToInt32(new byte[1].Concat(correctEndian).Reverse().ToArray(), 0);
                case 4: return BitConverter.ToInt32(correctEndian, 0);
                case 6: return BitConverter.ToInt64(new byte[2].Concat(correctEndian).Reverse().ToArray(), 0);
                case 8: return BitConverter.ToInt64(correctEndian, 0);
                default:
                    throw new InvalidOperationException(":(");
            }
        }

        private ValueDescription FindDecription()
        {
            var vife = ValueInformationFieldExtensions.LastOrDefault(x => x.Unit != Unit.None);

            if (vife == null)
            {
                var vifType = ValueInformationField.Type;

                switch (vifType)
                {
                    case PrimaryValueInformation.EnergyWh:
                    case PrimaryValueInformation.EnergyJoule:
                        return ValueDescription.Energy;
                    case PrimaryValueInformation.Volume:
                        return ValueDescription.Volume;
                    case PrimaryValueInformation.Mass:
                        return ValueDescription.Mass;
                    case PrimaryValueInformation.OnTime:
                    case PrimaryValueInformation.OperatingTime:
                    case PrimaryValueInformation.Date:
                    case PrimaryValueInformation.DateTimeGeneral:
                        return ValueDescription.Time;
                    case PrimaryValueInformation.PowerW:
                    case PrimaryValueInformation.PowerJh:
                        return ValueDescription.Power;
                    case PrimaryValueInformation.VolumeFlow:
                    case PrimaryValueInformation.VolumeFlowExt:
                    case PrimaryValueInformation.VolumeFlowExtS:
                        return ValueDescription.VolumeFlow;
                    case PrimaryValueInformation.MassFlow:
                        return ValueDescription.MassFlow;
                    case PrimaryValueInformation.InletFlowTemperature:
                        return ValueDescription.InletFlowTemperature;
                    case PrimaryValueInformation.ReturnFlowTemperature:
                        return ValueDescription.ReturnFlowTemperature;
                    case PrimaryValueInformation.TemperatureDifference:
                        return ValueDescription.TemperatureDifference;
                    case PrimaryValueInformation.ExternalTemperature:
                        return ValueDescription.ExternalTemperature;
                    case PrimaryValueInformation.Pressure:
                        return ValueDescription.Pressure;
                    case PrimaryValueInformation.AveragingDuration:
                        return ValueDescription.AveragingDuration;
                    case PrimaryValueInformation.ActualityDuration:
                        return ValueDescription.ActualityDuration;

                    case PrimaryValueInformation.UnitsForHCA:
                    case PrimaryValueInformation.ReservedForFutureThirdTableOfValueInformationExtensions:
                    case PrimaryValueInformation.FabricationNumber:
                    case PrimaryValueInformation.Identification:
                    case PrimaryValueInformation.Address:
                    case PrimaryValueInformation.FBValueInformationExtension:
                    case PrimaryValueInformation.ValueInformationInFollowingString:
                    case PrimaryValueInformation.FDValueInformationExtension:
                    case PrimaryValueInformation.ReservedForThirdExtensionOfValueInformationCodes:
                    case PrimaryValueInformation.AnyValueInformation:
                    case PrimaryValueInformation.ManufacturerSpecific:
                    default:
                        break;
                }

                
            }
            if (vife != null && vife is FBValueInformationExtensionField fbVife)
            {
                return GetFbDescription(fbVife.Type);
            }

            if (vife != null && vife is FDValueInformationExtensionField fdVife)
            {
                return GetFdDescription(fdVife.Type);
            }

            if (vife != null && vife is PrimaryValueInformationExtensionField primary)
            {
                return GetPrimaryDescription(primary.Type);
            }

            return ValueDescription.None;
        }

        private ValueDescription GetPrimaryDescription(PrimaryValueInformationExtension primaryType)
        {
            switch (primaryType)
            {
                default:
                    return ValueDescription.None;
            }
        }

        private ValueDescription GetFbDescription(FBValueInformationExtension vife)
        {
            switch (vife)
            {
                case FBValueInformationExtension.EnergyMWh:
                case FBValueInformationExtension.EnergyGj:
                case FBValueInformationExtension.EnergyMcal:
                    return ValueDescription.Energy;
                case FBValueInformationExtension.ReactiveEnergy:
                    return ValueDescription.ReactiveEnergy;
                case FBValueInformationExtension.ApparentEnergy:
                    return ValueDescription.ApperentEnergy;
                case FBValueInformationExtension.VolumeCubicMeter:
                    return ValueDescription.Volume;
                case FBValueInformationExtension.ReactivePower:
                    return ValueDescription.ReactivePower;
                case FBValueInformationExtension.Mass:
                    return ValueDescription.Mass;
                case FBValueInformationExtension.RelativeHumidity:
                    return ValueDescription.RelativeHumidity;
                //case FBValueInformationExtension.VolumeCubicFeet:
                //   break;
                //case FBValueInformationExtension.VolumeCubicFeet1:
                //break;
                case FBValueInformationExtension.PowerMw:
                case FBValueInformationExtension.PowerGjh:
                    return ValueDescription.Power;
                case FBValueInformationExtension.PowerPhaseUU:
                    return ValueDescription.PhasePotentialToPotential;
                case FBValueInformationExtension.PowerPhaseUI:
                    return ValueDescription.PhasePotentialToCurrent;
                case FBValueInformationExtension.Frequency:
                    return ValueDescription.Frequency;
                case FBValueInformationExtension.ApparentPower:
                    return ValueDescription.ApperentPower;

                    //case FBValueInformationExtension.ColdWarmTemperatureLimit:
                    //   break;
                    //case FBValueInformationExtension.CumulativeMaxActivePower:
                    //   break;
            }

            return ValueDescription.None;
        }

        private ValueDescription GetFdDescription(FDValueInformationExtension vife)
        {
            switch (vife)
            {
                case FDValueInformationExtension.Volts:
                    return ValueDescription.ElectricalPotential;
                case FDValueInformationExtension.Ampere:
                    return ValueDescription.ElectricalCurrent;
            }

            return ValueDescription.None;
        }

        private bool Equals(DataBlock other)
        {
            return Unit == other.Unit &&
                   Equals(Value, other.Value) &&
                   ValueDescription == other.ValueDescription &&
                   Equals(DataInformationField, other.DataInformationField) &&
                   DataInformationFieldExtensions.SequenceEqual(other.DataInformationFieldExtensions) &&
                   Equals(ValueInformationField, other.ValueInformationField) &&
                   ValueInformationFieldExtensions.SequenceEqual(other.ValueInformationFieldExtensions);
        }

        public override string ToString()
        {
            var dataInformationExtensions = string.Empty;
            if (DataInformationFieldExtensions.Any())
            {
                dataInformationExtensions =
                    $"DataInformationExtensions: {string.Join(",", DataInformationFieldExtensions)}";
            }

            var valueInformationExtensions = string.Empty;
            if (ValueInformationFieldExtensions.Any())
            {
                valueInformationExtensions =
                    $"ValueInformationExtensions: {string.Join(",", ValueInformationFieldExtensions)}";
            }

            return
                $"DataInformation: [{DataInformationField}], DataInformationExtensions: [{dataInformationExtensions}], ValueInformation: [{ValueInformationField}], ValueInformationExtensions: [{valueInformationExtensions}] Value: [{Value}]";
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DataBlock other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Unit;
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)ValueDescription;
                hashCode = (hashCode * 397) ^ (DataInformationField != null ? DataInformationField.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DataInformationFieldExtensions != null ? DataInformationFieldExtensions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValueInformationField != null ? ValueInformationField.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValueInformationFieldExtensions != null ? ValueInformationFieldExtensions.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
