﻿using System;

namespace ProductFileReader.Common.Exceptions
{
    public class InputException: Exception
    {
        public InputException(string message) : base(message) { }
    }
}
