using System;

namespace tanu.AutoPilot
{
	internal class EnumUtils
	{
		public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct
		{
			bool flag = value == null || !Enum.IsDefined(typeof(TEnum), value);
			bool result2;
			if (flag)
			{
				result = (TEnum)((object)Enum.GetValues(typeof(TEnum)).GetValue(0));
				result2 = false;
			}
			else
			{
				result = (TEnum)((object)Enum.Parse(typeof(TEnum), value));
				result2 = true;
			}
			return result2;
		}
	}
}
