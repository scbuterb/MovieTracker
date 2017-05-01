using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieTracker.Controllers;

namespace MovieTracker.Tests.Unit
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<string> _errorMessages = new List<string>();
            bool _isValid = AccountController.IsValidPassword("Password1!", _errorMessages);
        }
    }
}
