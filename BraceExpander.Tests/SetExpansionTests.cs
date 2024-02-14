using System;
using System.Linq;
using Xunit;

namespace BraceExpander.Tests
{
	public class SetExpansionTests
	{
		[Fact]
		public void BasicSetExpansionGivesExpectedResults()
		{
			var result = Expander.Expand("{a,b,c}");

			Assert.Collection(result,
				x => Assert.Equal("a", x),
				x => Assert.Equal("b", x),
				x => Assert.Equal("c", x));
		}

		[Theory]
		[InlineData("{a,{b,{c,d,{e,f,g}}}}")]
		[InlineData("{{{{a,b,c},d,e},f},g}")]
		public void NestedSetExpansion(string expression)
		{
			var result = Expander.Expand(expression);

			Assert.Collection(result,
				x => Assert.Equal("a", x),
				x => Assert.Equal("b", x),
				x => Assert.Equal("c", x),
				x => Assert.Equal("d", x),
				x => Assert.Equal("e", x),
				x => Assert.Equal("f", x),
				x => Assert.Equal("g", x));
		}

		[Theory]
		[InlineData("a{,b}", "a", "ab")]
		[InlineData("a{,b{,c}}", "a", "ab", "abc")]
		[InlineData("a{,b{,c,},}", "a", "ab", "abc", "ab", "a")]
		public void NestedMixedSetExpansion(string expression, params object[] expectedResults)
		{
			var results = Expander.Expand(expression);

			Assert.Collection(results, expectedResults
				.Select(x => (Action<string>)(y => Assert.Equal(x, y)))
				.ToArray());
		}

		[Fact]
		public void EmptyElementExpansion()
		{
			var result = Expander.Expand("{,a,b}");

			Assert.Collection(result,
				x => Assert.Equal("", x),
				x => Assert.Equal("a", x),
				x => Assert.Equal("b", x));
		}

		[Fact]
		public void EmptyElementLastExpansion()
		{
			var result = Expander.Expand("{a,b,}");

			Assert.Collection(result,
				x => Assert.Equal("a", x),
				x => Assert.Equal("b", x),
				x => Assert.Equal("", x));
		}

		[Fact]
		public void LeftToRightOrderIsPreserved()
		{
			var result = Expander.Expand("a{d,c,b}e");

			Assert.Collection(result,
				x => Assert.Equal("ade", x),
				x => Assert.Equal("ace", x),
				x => Assert.Equal("abe", x));
		}
	}
}
