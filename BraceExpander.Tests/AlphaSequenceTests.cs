using System;
using System.Linq;
using Xunit;

namespace BraceExpander.Tests
{
	public class AlphaSequenceTests
	{
		[Theory]
		[InlineData("{a..z}", 26)]
		[InlineData("{A..Z}", 26)]
		[InlineData("{z..a}", 26)]
		[InlineData("{Z..A}", 26)]
		public void BasicAlphaSequencesHaveExpectedCounts(string expression, int expectedCount)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Equal(expectedCount, results.Count());
		}

		[Theory]
		[InlineData("{aa..z}")]
		[InlineData("{AA..Z}")]
		[InlineData("{z..aa}")]
		[InlineData("{Z..AA}")]
		public void MultipleLettersAreIgnored(string expression)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Single(results);
			Assert.Collection(results,
				x => Assert.Equal(expression, x));
		}

		[Theory]
		[InlineData("{a..e}", 5)]
		[InlineData("{a..e..1}", 5)]
		[InlineData("{a..e..2}", 3)]
		public void AlphaSequenceWithIncrement(string expression, int expectedCount)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Equal(expectedCount, results.Count());
		}

		[Theory]
		[InlineData("{a..e..-1}", "a", "b", "c", "d", "e")]
		public void NegativeIncrementCoallesced(string expression, params string[] expectedResults)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Collection(results, expectedResults
				.Select(x => (Action<string>)(y => Assert.Equal(x, y)))
				.ToArray());
		}

		[Fact]
		public void ZeroLengthSequenceHasSingleValue()
		{
			var results = BraceExpander.Expand("{a..a}");

			Assert.Single(results);
			Assert.Collection(results,
				x => Assert.Equal("a", x));
		}

	}
}