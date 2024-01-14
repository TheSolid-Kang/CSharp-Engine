using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class CIO
    {
        CIO() { }
        public static int AskAndReturnInteger(string _strAsk = "Choose: ")
        {
            Console.Write(_strAsk);
            try
            {
                int iReturn = int.Parse(Console.ReadLine());
                return iReturn;
            }
            catch
            {
                Console.WriteLine("정수만 입력해주세요.");
                return AskAndReturnInteger(_strAsk);
            }
        }

        public static string AskAndReturnString(string _strAsk = "Key: ")
        {
            Console.Write(_strAsk);
            string strReturn = Console.ReadLine();
            return strReturn;
        }
    }
}
