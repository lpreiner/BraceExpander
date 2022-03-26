using System;
using System.Linq;
using Xunit;

namespace BraceExpander.Tests
{
	public class ComplexTests
	{
		[Theory]
		[InlineData("test{,{00..001}}", "test", "test000", "test001")]
		[InlineData("a{,{0..2},b,c,{3..4}}", "a", "a0", "a1", "a2", "ab", "ac", "a3", "a4")]
		[InlineData("{a..b}{a,b,c}{1..2}", "aa1", "aa2", "ab1", "ab2", "ac1", "ac2", "ba1", "ba2", "bb1", "bb2", "bc1", "bc2")]
		[InlineData("{{{{a,b},c,{d..f}},g},h}", "a", "b", "c", "d", "e", "f", "g", "h")]
		public void ComplexExpressionTest(string expression, params object[] expectedResults)
		{
			var results = Expander.Expand(expression);

			Assert.Collection(results, expectedResults
				.Select(x => (Action<string>)(y => Assert.Equal(x, y)))
				.ToArray());
		}

		[Theory]
		[InlineData("{a,b,c}{1,2}", "a1", "a2", "b1", "b2", "c1", "c2")]
		public void MultipleExpansions(string expression, params object[] expectedResults)
		{
			var results = Expander.Expand(expression);

			Assert.Collection(results, expectedResults
				.Select(x => (Action<string>)(y => Assert.Equal(x, y)))
				.ToArray());
		}

		[Theory]
		[InlineData("{a..z}{-10..10}{A..Z}", 26 * 21 * 26)]
		[InlineData("{0..10}{A..z}{-100..-1}", 11 * 52 * 100)]
		[InlineData("{0..10..2}{A..z..2}{-100..-1..2}", 6 * 26 * 50)]
		public void MultipleExpansionsHaveCorrectPermutationCount(string expression, int expectedResultCount)
		{
			var results = Expander.Expand(expression);

			Assert.Equal(expectedResultCount, results.Count());
		}
	}
}
