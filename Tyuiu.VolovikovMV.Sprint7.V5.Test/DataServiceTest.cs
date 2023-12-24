using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tyuiu.VolovikovMV.Sprint7.V5.Lib;

namespace Tyuiu.VolovikovMV.Sprint7.V5.Test
{
    [TestClass]
    public class DataServiceTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            DataService ds = new DataService();

            int[] costArray = new int[5] { 1, 2, 3, 4, 5 };

            int result = ds.FindMaxPrice(costArray);
            int wait = 5;

            Assert.AreEqual(wait, result);
        }
    }
}
