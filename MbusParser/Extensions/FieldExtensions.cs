using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBus.DataRecord;
using MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension;

namespace MBus.Extensions
{
    public static class FieldExtensions
    {
        public static bool IsSameType(this FBValueInformationExtensionField field,
            FBValueInformationExtension extension)
        {
            return field.Type == extension;
        }

        public static bool IsSameType(this FDValueInformationExtensionField field,
            FDValueInformationExtension extension)
        {
            return field.Type == extension;
        }
    }
}
