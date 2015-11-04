using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace BusinessIdentifierTest
{
    [TestClass]
    public class ISpecificationTest
    {
        BusinessIdentifier.ISpecification<string> spec = new BusinessIdentifier.BusinessIdentifierSpecification();

        [TestMethod]
        public void IsSatisfiedByTest_happy()
        {
            Assert.IsTrue(spec.IsSatisfiedBy("1867931-4"), "Valid Business Identifier did not validate.");
            Assert.IsTrue(((List<string>)spec.ReasonsForDissatisfaction).Count == 0, "Reason list was not empty");
        }

        [TestMethod]
        public void IsSatisfiedByTest_happy_needsDash()
        {
            Assert.IsTrue(spec.IsSatisfiedBy("18679314"), "Business Identifier missed dash but did not validate.");
            Assert.IsTrue(((List<string>)spec.ReasonsForDissatisfaction).Count == 1, "Reason list did not have any reasons");
        }

        [TestMethod]
        public void IsSatisfiedByTest_happy_needsLeadingZero()
        {                      
            Assert.IsTrue(spec.IsSatisfiedBy("206289-7"), "Business Identifier missed leading zero but did not validate.");
            Assert.IsTrue(((List<string>)spec.ReasonsForDissatisfaction).Count == 1, "Reason list did not have any reasons");
        }

        [TestMethod]
        public void IsSatisfiedByTest_nothappy_zeroes()
        {
            Assert.IsFalse(spec.IsSatisfiedBy("000"), "For some odd reason, failing failed.");           
        }

        [TestMethod]
        public void IsSatisfiedByTest_nothappy_tooshort()
        {
            Assert.IsFalse(spec.IsSatisfiedBy("1"), "For some odd reason, failing failed.");
        }

        [TestMethod]
        public void IsSatisfiedByTest_nothappy_toolong()
        {
            Assert.IsFalse(spec.IsSatisfiedBy("206289-72"), "For some odd reason, failing failed.");
        }

        [TestMethod]
        public void IsSatisfiedByTest_nothappy_NaN()
        {
            Assert.IsFalse(spec.IsSatisfiedBy("abcd-e"), "For some odd reason, failing failed.");
        }

        [TestCleanup]
        public void PrintReasonsForDissatisfaction()
        {
            if (((List<string>)spec.ReasonsForDissatisfaction).Count != 0)
            {
                foreach (var item in spec.ReasonsForDissatisfaction)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            else Console.WriteLine("No reason to be dissatisfied.");
        } 
    }
}
