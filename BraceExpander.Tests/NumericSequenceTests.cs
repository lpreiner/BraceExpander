using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BraceExpander.Tests
{
	public class NumericSequenceTests
	{
		[Theory]
		[InlineData("{1..10}", 10)]
		[InlineData("{2..1000}", 999)]
		public void BasicSequencesHaveExpectedCounts(string expression, int expectedCount)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Equal(expectedCount, results.Count());
		}

		[Theory]
		[InlineData("{1..10..2}", 5)]
		[InlineData("{1..1000..10}", 100)]
		public void SequencesWithExplicityIncrementsHaveExpectedCounts(string expression, int expectedCount)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Equal(expectedCount, results.Count());
		}

		[Fact]
		public void ZeroLengthSequenceHasSingleValue()
		{
			var results = BraceExpander.Expand("{1..1}");

			Assert.Single(results);
		}

		[Theory]
		[InlineData("{1..2}", "1", "2")]
		[InlineData("{01..2}", "01", "02")]
		[InlineData("{1..02}", "01", "02")]
		[InlineData("{0001..002}", "0001", "0002")]
		[InlineData("{009..010}", "009", "010")]
		public void ZeroPaddingIsRespected(string expression, params object[] expectedResults)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Collection(results, expectedResults
				.Select(x => (string y) => Assert.Equal(x, y))
				.ToArray());
		}

		[Theory]
		[InlineData("{2..-2}", "2","1","0","-1","-2")]
		[InlineData("{-2..-1}", "-2", "-1")]
		[InlineData("{-1..0}", "-1", "0")]
		public void NegativeBounds(string expression, params object[] expectedResults)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Collection(results, expectedResults
				.Select(x => (string y) => Assert.Equal(x, y))
				.ToArray());
		}

		[Theory]
		[InlineData(10, 0)]
		[InlineData(2, -2)]
		[InlineData(-1, -10)]
		public void ImpliedNegativeIncrement(int start, int end)
		{
			var results = BraceExpander.Expand($"{{{start}..{end}}}");

			Assert.Equal(GenerateRange(start, end, 1)
				.Select(x => x.ToString()), results);
		}

		[Theory]
		[InlineData("{0..5..-2}", "0", "2", "4")]
		// NOTE: The bash behavior here yields 0 2 4, while zsh yields 4 2 0
		public void ExplicitNegativeIncrement(string expression, params string[] expectedResults)
		{
			var results = BraceExpander.Expand(expression);

			Assert.Collection(results, expectedResults
				.Select(x => (string y) => Assert.Equal(x, y))
				.ToArray());
		}

		[Fact]
		public void ExplicitZeroIncremement()
		{
			var results = BraceExpander.Expand("{1..100..0}");

			Assert.Equal(100, results.Count());
		}


		#region Helpers

		IEnumerable<int> GenerateRange(int start, int end, int increment)
		{
			if (start > end)
				increment = -increment;

			for (var i = start; increment > 0 ? i <= end : i >= end; i += increment)
				yield return i;
		}

		#endregion
	}
}