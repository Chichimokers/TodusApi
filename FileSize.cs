using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodusApi
{
    public static class FileSize
    {
        public static string Check(long size)
        {
            string finalsize = string.Empty;

            if (size > 1000)
            {
                finalsize  =  Convert.ToString(size / 1000) + " kb";

                if (size > 1000000)
                {

                    finalsize = Convert.ToString(size / 1000000) + " Mb";

                }

            }
            return finalsize;
           
        }
    }
}
