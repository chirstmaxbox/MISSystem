using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCommon
{
    public interface  IValidation
    {
         bool IsValidated { get;  set; }
         string ErrorMessage { get; set; }
    }

    public class MyValidation
    {
        public bool IsValid { get; set; }
        public List<MyKeyValuePair> ErrorMessages { get; set; }

        public MyValidation()
        {
            IsValid = true;
            ErrorMessages = new List<MyKeyValuePair>();
        }

        public string GetErrorMessageString()
        {
            if (!ErrorMessages.Any( )) return "";
            foreach  (var mkv in ErrorMessages )
            {
                mkv.Value = mkv.Value + Environment.NewLine;
            }
            var a1 = ErrorMessages.Select(x => x.Value).ToArray();
            return MyCommon.MyString.ConvertArrayToString(a1);

        }

    }
    
}