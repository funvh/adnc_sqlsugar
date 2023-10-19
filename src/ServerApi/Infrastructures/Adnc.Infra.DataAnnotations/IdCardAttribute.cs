using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 身份证验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IdCardAttribute : ValidationAttribute
    {
        public IdCardAttribute() : base("身份证号码不正确")
        {

        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (!(value is string idcard))
                return false;

            string pattern = @"^\d{17}(?:\d|X)$";

            if (!Regex.IsMatch(idcard, pattern))  // 18位格式检查
                return false;

            string birth = idcard.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            if (!DateTime.TryParse(birth, out DateTime time))  // 出生日期检查
                return false;

            //int[] arr_weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };     // 加权数组
            //string[] id_last = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };   // 校验数组
            //int sum = 0;
            //for (int i = 0; i < 17; i++)
            //{
            //    sum += arr_weight[i] * int.Parse(idcard[i].ToString());
            //}
            //int result = sum % 11;  // 实际校验位的值
            //if (id_last[result] != idcard[17].ToString())  // 校验位检查
            //    return false;

            return true;
        }
    }
}
