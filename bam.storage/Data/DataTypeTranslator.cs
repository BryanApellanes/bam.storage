using Bam.Data;

namespace Bam.Storage.Data
{
    /// <summary>
    /// A class for converting between different type representations.
    /// </summary>
    /// <remarks>
    /// A copy of this class exists in Bam.Data as well.  It is duplicated here to avoid a dependency on Bam.Data. 
    /// </remarks>
    public class DataTypeTranslator : IDataTypeTranslator
    {
        private static readonly object _dataTypeTranslatorLock = new object();
        private static IDataTypeTranslator _dafault = null!;
        /// <summary>
        /// Gets or sets the default singleton instance of <see cref="IDataTypeTranslator"/>.
        /// </summary>
        public static IDataTypeTranslator Default
        {
            get
            {
                return _dataTypeTranslatorLock.DoubleCheckLock(ref _dafault, () => new DataTypeTranslator());
            }
            set => _dafault = value;
        }

        /// <summary>
        /// Converts a CLR <see cref="Type"/> to its corresponding <see cref="DataTypes"/> enum value.
        /// Returns <see cref="DataTypes.Default"/> for null or unrecognized types.
        /// </summary>
        /// <param name="type">The CLR type to convert.</param>
        /// <returns>The corresponding <see cref="DataTypes"/> value.</returns>
        public virtual DataTypes EnumFromType(Type type)
        {
            if (type == typeof(object) || type == null)
            {
                return DataTypes.Default;
            }
            
            if (type == typeof(bool))
            {
                return DataTypes.Boolean;
            }

            if (type == typeof(int))
            {
                return DataTypes.Int;
            }

            if (type == typeof(uint))
            {
                return DataTypes.UInt;
            }

            if (type == typeof(ulong))
            {
                return DataTypes.ULong;
            }

            if (type == typeof(long))
            {
                return DataTypes.Long;
            }

            if (type == typeof(decimal))
            {
                return DataTypes.Decimal;
            }

            if (type == typeof(string))
            {
                return DataTypes.String;
            }

            if (type == typeof(byte[]))
            {
                return DataTypes.ByteArray;
            }

            if (type == typeof(DateTime))
            {
                return DataTypes.DateTime;
            }
            
            return DataTypes.Default;
        }
        
        /// <summary>
        /// Converts a database data type name (e.g., "varchar", "bigint") to its corresponding CLR <see cref="Type"/>.
        /// </summary>
        /// <param name="dbDataType">The database data type name.</param>
        /// <returns>The corresponding CLR type.</returns>
        public virtual Type TypeFromDbDataType(string dbDataType)
        {
            return TypeFromDataType(TranslateDataType(dbDataType));
        }

        /// <summary>
        /// Converts a <see cref="DataTypes"/> enum value to its corresponding CLR <see cref="Type"/>.
        /// </summary>
        /// <param name="dataType">The data type enum value.</param>
        /// <returns>The corresponding CLR type.</returns>
        public virtual Type TypeFromDataType(DataTypes dataType)
        {
            switch (dataType)
            {
                case DataTypes.Default:
                    return typeof(object);
                case DataTypes.Boolean:
                    return typeof(bool);
                case DataTypes.Int:
                    return typeof(int);
                case DataTypes.UInt:
                    return typeof(uint);
                case DataTypes.Long:
                    return typeof(long);
                case DataTypes.ULong:
                    return typeof(ulong);
                case DataTypes.Decimal:
                    return typeof(decimal);
                case DataTypes.String:
                    return typeof(string);
                case DataTypes.ByteArray:
                    return typeof(byte[]);
                case DataTypes.DateTime:
                    return typeof(DateTime);
                default:
                    return typeof(object);
            }
        }

        /// <summary>
        /// Translates a database data type name (e.g., "varchar", "bigint", "blob") to a <see cref="DataTypes"/> enum value.
        /// Defaults to <see cref="DataTypes.String"/> for unrecognized types.
        /// </summary>
        /// <param name="dbDataType">The database data type name (case-insensitive).</param>
        /// <returns>The corresponding <see cref="DataTypes"/> value.</returns>
        public virtual DataTypes TranslateDataType(string dbDataType)
        {
            string dataType = dbDataType.ToLowerInvariant();
            switch (dataType)
            {
                case "bigint":
                    return DataTypes.ULong;
                case "binary":
                    return DataTypes.ByteArray;
                case "bit":
                    return DataTypes.Boolean;
                case "blob":
                    return DataTypes.ByteArray;
                case "char":
                    return DataTypes.String;
                case "date":
                case "datetime":
                    return DataTypes.DateTime;
                case "decimal":
                    return DataTypes.Decimal;
                case "double":
                    return DataTypes.Decimal;
                case "enum":
                    return DataTypes.String;
                case "float":
                    return DataTypes.Decimal;
                case "int":
                    return DataTypes.Int;
                case "smallint":
                    return DataTypes.Int;
                case "text":
                    return DataTypes.String;
                case "time":
                    return DataTypes.DateTime;
                case "timestamp":
                    return DataTypes.String;
                case "tinyblob":
                    return DataTypes.ByteArray;
                case "tinyint":
                    return DataTypes.Int;
                case "tinytext":
                    return DataTypes.String;
                case "varbinary":
                    return DataTypes.ByteArray;
                case "varchar":
                    return DataTypes.String;
                case "year":
                    return DataTypes.String;
                default:
                    return DataTypes.String;
            }
        }
    }
}
