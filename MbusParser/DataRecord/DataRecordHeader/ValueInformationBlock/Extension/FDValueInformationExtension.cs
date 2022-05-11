﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension
{
    public enum FDValueInformationExtension
    {
        CurrencyCredit = 0x03, //	E000 00nn	Credit of 10 nn-3 of the nominal local legal currency units
        CurrencyDebit = 0x07, //	E000 01nn	Debit of 10 nn-3 of the nominal local legal	currency units

        // Enhanced Identification
        AccessNumber = 0x08, //	E000 1000 Access Number  = transmission count
        Medium = 0x09, //	E000 1001 Medium  = as in fixed header
        Manufacturer = 0x0A, //	E000 1010 Manufacturer  = as in fixed header
        ParameterSetId = 0x0B, //	E000 1011 Parameter set identification Enhanced Identification
        ModelVersion = 0x0C, //	E000 1100 Model / Version
        HardwareVersion = 0x0D, //	E000 1101 Hardware version #
        FirmwareVersion = 0x0E, //	E000 1110 Firmware version #
        SoftwareVersion = 0x0F, //	E000 1111 Software version #

        // Implementation of all TC294 WG1 requirements  = improved selection ..
        CustomerLocation = 0x10, //	E001 0000 Customer location
        Customer = 0x11, //	E001 0001 Customer
        AccessCodeUser = 0x12, //	E001 0010 Access Code User
        AccessCodeOperator = 0x13, //	E001 0011 Access Code Operator 
        AccessCodeSystemOperator = 0x14, //	E001 0100 Access Code System Operator 
        AccessCodeDeveloper = 0x15, //	E001 0101 Access Code Developer 
        Password = 0x16, //	E001 0110 Password
        ErrorFlags = 0x17, //	E001 0111 Error flags  = binary
        ErrorMasks = 0x18, //	E001 1000 Error mask
        Reserved = 0x19, //	E001 1001 Reserved
        DigitalOutput = 0x1A, //	E001 1010 Digital Output  = binary
        DigitalInput = 0x1B, //	E001 1011 Digital Input  = binary
        Baudrate = 0x1C, //	E001 1100 Baudrate [Baud]
        ResponseDelay = 0x1D, //	E001 1101 response delay time [bittimes]
        Retry = 0x1E, //	E001 1110 Retry
        Reserved2 = 0x1F, //	E001 1111 Reserved

        //	Enhanced storage management	
        FirstStorageNumber = 0x20, //	E010 0000 First storage # for cyclic storage
        LastStorageNumber = 0x21, //	E010 0001 Last storage # for cyclic storage
        SizeOfStorageBlock = 0x22, //	E010 0010 Size of storage block
        Reserved3 = 0x23, //	E010 0011 Reserved
        StorageInterval = 0x27, //	E010 01nn Storage interval [sec = s..day = s]
        StorageIntervalMonth = 0x28, //	E010 1000 Storage interval month = s 
        StorageIntervalYear = 0x29, //	E010 1001 Storage interval year = s

        //	E010 1010 Reserved
        //	E010 1011 Reserved
        DurationSinceLastReadout = 0x2F, //	E010 11nn Duration since last readout [sec = s..day = s] 

        //  Enhanced tarif management
        TariffStart = 0x30, //	E011 0000 Start  = date/time of tariff
        TariffDuration = 0x33, //	E011 00nn Duration of tariff  = nn=01 ..11: min to days
        TariffPeriod = 0x37, //	E011 01nn Period of tariff [sec = s to day = s] 
        TariffPeriodMonth = 0x38, //	E011 1000 Period of tariff months = s 
        TariffPeriodYears = 0x39, //	E011 1001 Period of tariff year = s 
        Dimensionless = 0x3A, //	E011 1010 dimensionless / no VIF

        //	E011 1011 Reserved
        //	E011 11xx Reserved
        //  electrical units
        Volts = 0x4F, //	E100 nnnn 10 nnnn-9 Volts 
        Ampere = 0x5F, //	E101 nnnn 10 nnnn-12 A
        ResetCounter = 0x60, //	E110 0000 Reset counter
        CumulationCounter = 0x61, //	E110 0001 Cumulation counter
        ControlSignal = 0x62, //	E110 0010 Control signal
        DayOfWeek = 0x63, //	E110 0011 Day of week
        WeekNumber = 0x64, //	E110 0100 Week number
        TimePointOfDayChange = 0x65, //	E110 0101 Time point of day change
        StateOfParameterActivation = 0x66, //	E110 0110 State of parameter activation
        SpecialSupplierInformation = 0x67, //	E110 0111 Special supplier information
        DurationSinceLastCumulation = 0x6B, //	E110 10pp Duration since last cumulation [hour = s..years = s]
        BatteryOperatingTime = 0x6F, //	E110 11pp Operating time battery [hour = s..years = s]
        BatteryChangeTimeAndDate = 0x70, //	E111 0000 Date and time of battery change
        RemainingBatteryDays = 0x74,//E111 0100 Remaining battery life time (days)
        Reserved7F = 0x7f
    }

    public enum PrimaryValueInformationExtension
    {
        ErrorCodesVIFE = 0x1F,
        PerSecond = 0x20,
        PerMinute = 0x21,
        PerHour = 0x22,
        PerDay = 0x23,
        PerWeek = 0x24,
        PerMonth = 0x25,
        PerYear = 0x26,
        PerRevolutionMeasurement = 0x27,
        IncrementPerInputPulseOnInputChannel0 = 0x28,
        IncrementPerInputPulseOnInputChannel1 = 0x29,
        IncrementPerOutputPulseOnOutputChannel0 = 0x2A,
        IncrementPerOutputPulseOnOutputChannel1 = 0x2B,
        PerLiter = 0x2C,
        PerM3 = 0x2D,
        PerKg = 0x2E,
        PerKelvin = 0x2F,
        PerkWh = 0x30,
        PerGJ = 0x31,
        PerkW = 0x32,
        PerKelvinLiter = 0x33,
        PerVolt = 0x34,
        PerAmpere = 0x35,
        MultipliedBySek = 0x36,
        MultipliedBySekPerV = 0x37,
        MultipliedBySekPerA = 0x38,
        StartDateTimeOf = 0x39,
        UncorrectedUnit = 0x3A,
        AccumulatedPositive = 0x3B,
        AccumulatedNegative = 0x3C,
        ReservedVIFE3F = 0x3F,
        LowerLimit = 0x40,
        LowerLimitExceedsCount = 0x41,
        DateTimeOfBeginOfFirstLowerLimitExceed = 0x42,
        DateTimeOfEndOfFirstLowerLimitExceed = 0x43,
        ReservedVIFE45 = 0x45,
        DateTimeOfBeginOfLastLowerLimitExceed = 0x46,
        DateTimeOfEndOfLastLowerLimitExceed = 0x47,
        UpperLimit = 0x48,
        UpperLimitExceedsCount = 0x49,
        DateTimeOfBeginOfFirstUpperLimitExceed = 0x4A,
        DateTimeOfEndOfFirstUpperLimitExceed = 0x4B,
        ReservedVIFE4D = 0x4D,
        DateTimeOfBeginOfLastUpperLimitExceed = 0x4E,
        DateTimeOfEndOfLastUpperLimitExceed = 0x4F,
        DurationOfFirstLowerLimitExceed = 0x53,//50-53
        DurationOfLastLowerLimitExceed = 0x57, //54-57
        DurationOfFirstUpperLimitExceed = 0x5B, //58-5B
        DurationOfLastUpperLimitExceed = 0x5F, //5C-5F
        DurationOfFirst = 0x63,//50-63
        DurationOfLast = 0x67,//64-67
        ReservedVIFE69 = 0x69,
        DateTimeOfBeginOfFirst = 0x6A,
        DateTimeOfEndOfFirst = 0x6B,
        ReservedVIFE6D = 0x6D,
        DateTimeOfBeginOfLast = 0x6E,
        DateTimeOfEndOfLast = 0x6F,
        RemainingBatteryLife = 0x74,
        MultiplicativeCorrectionFactor = 0x77,
        AdditiveCorrectionConstant = 0x7B,
        ReservedVIFE7C = 0x7C,
        MultiplicativeCorrectionFactor1000 = 0x7D,
        FutureValue = 0x7E,
        NextVIFEAndDataManufacturerSpecific = 0x7F
    }
}
