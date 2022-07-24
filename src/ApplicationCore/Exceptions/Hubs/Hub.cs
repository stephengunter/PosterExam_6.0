using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Exceptions
{
    public class KeepUserConnectionFailed : Exception
    {
        public KeepUserConnectionFailed(string msg) : base(msg)
        {

        }
    }
}
