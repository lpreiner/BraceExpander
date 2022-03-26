using Xunit;

namespace BraceExpander.Tests
{
	public class SimpleInputTests
	{
		[Theory]
		[InlineData("abcdefg")]
		[InlineData("1234567")]
		[InlineData("{asdf")]
		[InlineData("asdf}")]
		[InlineData("{asdf}")]
		[InlineData("{12345}")]
		public void SimpleInputHasSameOutput(string input)
		{
			var result = Expander.Expand(input);

			Assert.Single(result);
			Assert.Collection(result,
				x => Assert.Equal(input, x));
		}
	}
}
