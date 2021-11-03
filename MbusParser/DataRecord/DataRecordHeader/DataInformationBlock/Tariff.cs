// <copyright file="Tariff.cs" company="Poul Erik Venø Hansen">
// Copyright (c) Poul Erik Venø Hansen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MBus.DataRecord.DataRecordHeader.DataInformationBlock
{
    public sealed class Tariff
    {
        private readonly byte _tariffValue;

        public Tariff(byte tariffValue)
        {
            _tariffValue = tariffValue;
        }

        public override string ToString()
        {
            return $"Tariff: {_tariffValue}";
        }

        private bool Equals(Tariff other)
        {
            return _tariffValue == other._tariffValue;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Tariff other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _tariffValue.GetHashCode();
        }
    }
}