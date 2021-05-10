using FluentAssertions;
using System.Linq;
using Xunit;

namespace Bot.Tests
{
    public class ArrayTests
    {
        [Fact]
        public void AddZip()
        {
            var a = new int[] { 1, 2, 3 };
            var b = new int[] { 4, 5, 6 };
            var result = a.Zip(b, (x, y) => x + y).ToArray();

            result[0].Should().Be(5);
            result[1].Should().Be(7);
            result[2].Should().Be(9);
        }
    }
}
