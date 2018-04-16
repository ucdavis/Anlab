using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Models;
using Shouldly;
using Xunit;

namespace Test.TestsModel
{
    public class PaymentTypeCodesTests
    {
        [Fact]
        public void PaymentTypeCodesHaveExpactedValues1()
        {
            PaymentTypeCodes.Other.ShouldBe("Other");
        }
        [Fact]
        public void PaymentTypeCodesHaveExpactedValues2()
        {
            PaymentTypeCodes.CreditCard.ShouldBe("CreditCard");
        }
        [Fact]
        public void PaymentTypeCodesHaveExpactedValues3()
        {
            PaymentTypeCodes.UcDavisAccount.ShouldBe("UcDavisAccount");
        }
        [Fact]
        public void PaymentTypeCodesHaveExpactedValues4()
        {
            PaymentTypeCodes.UcOtherAccount.ShouldBe("UcOtherAccount");
        }
    }
}
