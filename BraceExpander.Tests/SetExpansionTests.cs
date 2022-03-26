using Xunit;

namespace BraceExpander.Tests
{
	public class SetExpansionTests
	{
		[Fact]
		public void BasicSetExpansionGivesExpectedResults()
		{
			var result = BraceExpander.Expand("{a,b,c}");

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
			var result = BraceExpander.Expand(expression);

			Assert.Collection(result,
				x => Assert.Equal("a", x),
				x => Assert.Equal("b", x),
				x => Assert.Equal("c", x),
				x => Assert.Equal("d", x),
				x => Assert.Equal("e", x),
				x => Assert.Equal("f", x),
				x => Assert.Equal("g", x));
		}

		[Fact]
		public void EmptyElementExpansion()
		{
			var result = BraceExpander.Expand("{,a,b}");

			Assert.Collection(result,
				x => Assert.Equal("", x),
				x => Assert.Equal("a", x),
				x => Assert.Equal("b", x));
		}

		[Fact]
		public void EmptyElementLastExpansion()
		{
			var result = BraceExpander.Expand("{a,b,}");

			Assert.Collection(result,
				x => Assert.Equal("a", x),
				x => Assert.Equal("b", x),
				x => Assert.Equal("", x));
		}

		[Fact]
		public void LeftToRightOrderIsPreserved()
		{
			var result = BraceExpander.Expand("a{d,c,b}e");

			Assert.Collection(result,
				x => Assert.Equal("ade", x),
				x => Assert.Equal("ace", x),
				x => Assert.Equal("abe", x));
		}
	}
}
