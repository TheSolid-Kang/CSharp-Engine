using Engine._07.CZIP;
using Moq;
using System;
using Xunit;

namespace EngineTest._07.CZIPTests
{
    public class CZIPTests
    {
        private MockRepository mockRepository;



        public CZIPTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private CZIP CreateCZIP()
        {
            return new CZIP();
        }

        [Fact]
        public void Zip_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cZIP = this.CreateCZIP();
            double[] xValue = { 0, 1, 2, 3, 4, 5, 6, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 7, 8, 9 };
            double[] yValue = { 0, 1, 2, 3, 4, 5, 3, 4, 5, 6, 6, 7, 8, 9, 0, 1, 2, 7, 8, 9 };


            // Act
            var result = cZIP.Zip(
                xValue,
                yValue,
                (x, y) => Tuple.Create(x, y));

            // Assert
            Assert.NotNull(result);
            this.mockRepository.VerifyAll();
        }
    }
}
