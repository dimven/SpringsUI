using System;
using Autodesk.DesignScript.Runtime;

namespace SpringsUIzt
{
    [IsVisibleInDynamoLibrary(false)]
    public class passwordPrimitive
    {
        private string _pass;
        
        private passwordPrimitive(string pass)
        {
            _pass = pass;
        }
        
        public string Decode(string secret)
        {
            try
            {
                return StringCipher.Decrypt(_pass, secret);
            }
            catch (SystemException)
            {
                return "";
            }
        }
        
        public override string ToString()
        {
            return "Secret!";
        }
        
        public static passwordPrimitive getPrimitive(string pass)
        {
            return new passwordPrimitive(pass);
        }

    }
}
