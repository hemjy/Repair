

namespace Repair.Application.Helpers
{
    public static class EnumConverter
    {
        public static TEnum ConvertToEnum<TEnum>(string value) where TEnum : struct, Enum
        {
            if (string.IsNullOrEmpty(value)) return default;

            var isSuccess = Enum.TryParse(value, true, out TEnum result);
            return isSuccess ? result : default;
        }
    }
}
