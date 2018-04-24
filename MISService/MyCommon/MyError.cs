using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCommon
{
  public   class MyValidationError
    {
      public bool IsValid { get; set; }
      public List<MyKeyValuePair> ErrorMessages { get; set; }

      public MyValidationError()
      {
          IsValid = true;
          ErrorMessages = new List<MyKeyValuePair>();
      }

    }




}
