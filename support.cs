using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableStorage
{
    public static class  support
    {
        public static string padZeroOnItem(int length, object p_value)
        {
            bool l_null = false;

            string l_valueStr = "";
            if (p_value != null)
            {
                // Ok we have a value check type
                l_valueStr = p_value.ToString();
            }
            else
            {
                l_valueStr = "";
                l_null = true;
            }

            if (l_valueStr.Length > length)
            {
                //Cut of the ass
                l_valueStr = l_valueStr.Substring(0, length);
            }
            else
            {
                // Ok Padd with zero in front..
                int l_diff = length - l_valueStr.Length;
                string l_zero = "";

                for (int c = 0; c < l_diff; c++)
                {
                    if (l_null)
                        l_zero += " ";
                    else
                        l_zero += "0";
                }

                l_valueStr = l_zero + l_valueStr;
            }

            return l_valueStr;
        }
    }
}
