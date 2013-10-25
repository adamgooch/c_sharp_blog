using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utility
{
    class StringConversion
    {
        public static byte[] GetBytes( string str )
        {
            byte[] bytes = new byte[str.Length * sizeof( char )];
            System.Buffer.BlockCopy( str.ToCharArray(), 0, bytes, 0, bytes.Length );
            return bytes;
        }

        public static string GetString( byte[] bytes )
        {
            char[] chars = new char[bytes.Length / sizeof( char )];
            System.Buffer.BlockCopy( bytes, 0, chars, 0, bytes.Length );
            return new string( chars );
        }

        public static byte[] GetUrlSafeBytes( string str )
        {
            if( str.Length == 0 )
                throw new Exception( "Invalid string value in StrToByteArray" );

            byte val;
            byte[] byteArr = new byte[str.Length / 3];
            int i = 0;
            int j = 0;
            do
            {
                val = byte.Parse( str.Substring( i, 3 ) );
                byteArr[j++] = val;
                i += 3;
            }
            while( i < str.Length );
            return byteArr;
        }

        public static string GetUrlSafeString( byte[] byteArr )
        {
            byte val;
            string tempStr = "";
            for( int i = 0; i <= byteArr.GetUpperBound( 0 ); i++ )
            {
                val = byteArr[i];
                if( val < (byte)10 )
                    tempStr += "00" + val.ToString();
                else if( val < (byte)100 )
                    tempStr += "0" + val.ToString();
                else
                    tempStr += val.ToString();
            }
            return tempStr;
        }
    }
}
