using System;

namespace SMS.App.Utilities.Others
{
    public class NumberToWord
    {
        string[] single = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen", "twenty" };
        string[] decades = { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        string[] edges = { "hundred", "thousand", "lakh", "crore" };
        public string GetWordFromNumber(int number)
        {
            if (number>0)
            {
                #region Single Number Formation
                if (number.ToString().Length > 0 && number.ToString().Length < 2)
                {
                    return single[number];
                }
                #endregion

                #region Double Number Formation
                if (number.ToString().Length > 1 && number.ToString().Length < 3)
                {
                    if (number <= 20)
                    {
                        return single[number];
                    }
                    else
                    {
                        string myNum = number.ToString();
                        string decade = decades[Convert.ToInt32(myNum.Substring(0, 1))];
                        string sNumber = single[Convert.ToInt32(myNum.Substring(1, 1))];
                        return decade + " " + sNumber;
                    }
                }
                #endregion
            }
            return number.ToString();
        }
    }
}
