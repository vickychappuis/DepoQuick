using DepoQuick.Backend.Services;

namespace DepoQuick.Tests.Services
{
    [TestClass]
    public class DateTimeServiceTests
    {
        [TestMethod]
        public void CurrentDateTime_WhenNotSet_ShouldReturnCurrentDateTime()
        {
            DateTime expected = DateTime.Now;
            DateTime actual = DateTimeService.CurrentDateTime;
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Hour, actual.Hour);
            Assert.AreEqual(expected.Minute, actual.Minute);
        }

        [TestMethod]
        public void CurrentDateTime_WhenSet_ShouldReturnSetDateTime()
        {
            DateTime expected = new DateTime(2024, 01, 01, 12, 30, 0);
            DateTimeService.CurrentDateTime = expected;
            DateTime actual = DateTimeService.CurrentDateTime;
            Assert.AreEqual(expected, actual);
        }
    }
}