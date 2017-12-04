using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day05 :  Day
    {
        public override string First(string input)
        {
            return GetFirstPassword(input);
        }
        
        private string GetFirstPassword(string key)
        {
            int index = 0, foundIndex;
            var password = new char[8];
            for (int i = 0; i < 8; i++)
            {
                char c;
                GetPasswordChar(key, index, out c, out foundIndex);
                index = foundIndex+1;
                Console.Write("{0}", c);
                password[i] = c; 
            }
            return string.Join("", password);
        }

        private string GetSecondPassword(string key)
        {
            int index = 0, foundIndex;
            var password = new char[8];
            int pos;
            char pwChar;
            do
            {
                GetPasswordChar(key, index, out pos, out pwChar, out foundIndex);
                index = foundIndex + 1;
                if (pos < 8 && ((int)password[pos]) == 0)
                {
                    password[pos] = pwChar;
                }
                Console.WriteLine(string.Join("", password));
            } while (password.Any(c => (int)c == 0));
            return string.Join("", password);
        }

        private void GetPasswordChar(string key, int startIndex, out char pwChar, out int index)
        {
            int pwPos;
            GetPasswordChar(key, startIndex, out pwPos, out pwChar, out index);
            pwChar = ((byte)pwPos).ToString("x1")[0];
        }

        private void GetPasswordChar(string key, int startIndex, out int pwPos, out char pwChar, out int index)
        {
            index = startIndex;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                do
                {
                    string msg = string.Format("{0}{1}", key, index);
                    byte[] msgBytes = System.Text.Encoding.ASCII.GetBytes(msg);
                    byte[] hashBytes = md5.ComputeHash(msgBytes);
                    if (hashBytes[0]==0 && hashBytes[1]==0 && hashBytes[2]<16)
                    {
                        pwPos = (int)hashBytes[2];
                        pwChar = ((hashBytes[3] & 0xF0) >> 4).ToString("x1")[0];
                        return;
                    }
                    index++;
                    //if ((index % 10000) == 0) Console.WriteLine(index);
                } while (true);
            }
        }

        public override string Second(string input)
        {
            return GetSecondPassword(input);
        }
    }
}
