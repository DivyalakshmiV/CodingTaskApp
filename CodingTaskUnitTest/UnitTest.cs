using CodingTaskApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodingTaskUnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            bool expectedVal = true;

            ViewModel viewModel = new ViewModel();
            PrivateObject obj = new PrivateObject(viewModel);
            viewModel.Name = "test";
            viewModel.Id = 1;

            var retVal = obj.Invoke("IsAddedStudent");            
            Assert.AreEqual(expectedVal, retVal);
        }
    }
}
