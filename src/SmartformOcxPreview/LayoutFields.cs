namespace SmartformOcxPreview
{
    public static class LayoutFields
    {
        public static IDictionary<string, string> Fields { get; private set; }

        public static void Reset()
        {
            Fields?.Keys?.ToList()?.ForEach(delegate (string k)
            {
                Fields[k] = string.Empty;
            });
        }

        public static string? GetValue(string fieldName)
        {
            if (!Fields.ContainsKey(fieldName))
            {
                return null;
            }

            return Fields[fieldName];
        }

        public static bool SetValue(string fieldName, string fieldValue)
        {
            if (!Fields.ContainsKey(fieldName))
            {
                return false;
            }

            Fields[fieldName] = fieldValue;
            return true;
        }

        static LayoutFields()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["HOLDER_USER_ID"] = string.Empty;
            dictionary["HOLDER_FIRST_NAME"] = string.Empty;
            dictionary["HOLDER_LAST_NAME"] = string.Empty;
            dictionary["HOLDER_BIRTH_DATE"] = string.Empty;
            dictionary["ISSUING_DATE"] = string.Empty;
            dictionary["TSC_VAL_END_DATE"] = string.Empty;
            dictionary["HOLDER_USER_ID"] = string.Empty;
            dictionary["HOLDER_PHOTO"] = string.Empty;
            dictionary["HOLDER_SIGNATURE"] = string.Empty;
            dictionary["SERIAL_NUMBER"] = string.Empty;
            dictionary["PROFILE_IDENTIFIER"] = string.Empty;
            dictionary["PROFILE_LONG_NAME"] = string.Empty;
            dictionary["PROFILE_SHORT_NAME"] = string.Empty;
            dictionary["PROFILE_NOTE"] = string.Empty;
            dictionary["RICHIESTA_ID"] = string.Empty;
            dictionary["SERIAL_NUMBER"] = string.Empty;
            dictionary["SALE_DEVICE"] = string.Empty;
            dictionary["HOLDER_ADDRESS"] = string.Empty;
            dictionary["PROFILE_SHORT_NAME"] = string.Empty;
            dictionary["HOLDER_ZIP_CODE"] = string.Empty;
            dictionary["HOLDER_TOWN"] = string.Empty;
            dictionary["HOLDER_PROV"] = string.Empty;
            Fields = dictionary;
        }
    }
}
