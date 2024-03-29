﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BraceExpander
{
	[Obsolete("The name of this class is at odds with .NET naming guidelines. Use Expander instead.  See issue #4")]
	public class BraceExpander : Expander
	{ }

	public class Expander
	{
		const string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		const int DEFAULT_INCREMENT = 1;
		const string EXP = "exp";
		const string PATTERN = @"{(?<" + EXP + @">(?>{(?<x>)|[^{}]+|}(?<-x>))*(?(x)(?!)))}";
		static TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);
		static Regex Expansions = new Regex(PATTERN, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled, RegexTimeout);

		const string SEQUENCE_NUMERIC = @"^(?<start>-?\d+)\.\.(?<end>-?\d+)(\.\.(?<inc>-?\d+))?$";
		static Regex SequenceNumeric = new Regex(SEQUENCE_NUMERIC, RegexOptions.Compiled, RegexTimeout);

		const string SEQUENCE_ALPHA = @"^(?<start>[A-Za-z]?)\.\.(?<end>[A-Za-z]?)(\.\.(?<inc>-?\d+))?$";
		static Regex SequenceAlpha = new Regex(SEQUENCE_ALPHA, RegexOptions.Compiled, RegexTimeout);

		public static IEnumerable<string> Expand(string expression)
		{
			if (expression is null)
			{
				yield return expression;
				yield break;
			}

			var match = Expansions.Match(expression);
			if (!match.Success)
			{
				yield return expression;
				yield break;
			}

			var exp = match.Groups.OfType<Group>().First(g => g.Name == "exp")?.Value;

			var expansions = new List<string>();

			var isSequenceNumeric = SequenceNumeric.Match(exp);
			var isSequenceAlpha = SequenceAlpha.Match(exp);

			// Sequence - Numeric
			if (isSequenceNumeric.Success)
			{
				var rawStart = isSequenceNumeric.Groups.OfType<Group>().First(g => g.Name == "start")?.Value;
				if (!int.TryParse(rawStart, out var start))
				{
					yield return expression;
					yield break;
				}

				var rawEnd = isSequenceNumeric.Groups.OfType<Group>().First(g => g.Name == "end")?.Value;
				if (!int.TryParse(rawEnd, out var end))
				{
					yield return expression;
					yield break;
				}

				var rawInc = isSequenceNumeric.Groups.OfType<Group>().FirstOrDefault(g => g.Name == "inc")?.Value;
				if (!int.TryParse(rawInc, out var inc))
					inc = DEFAULT_INCREMENT;

				inc = Math.Max(DEFAULT_INCREMENT, Math.Abs(inc));

				if (start > end)
					inc = -inc;

				var format = rawStart.StartsWith('0') && rawStart != "0" || (rawEnd.StartsWith('0') && rawEnd != "0")
					? new string('0', Math.Max(rawStart.Length, rawEnd.Length))
					: "0";

				for (var i = start; inc > 0 ? i <= end : i >= end; i += inc)
					expansions.Add(i.ToString(format));
			}
			// Sequence - Alpha
			else if (isSequenceAlpha.Success)
			{
				var rawStart = isSequenceAlpha.Groups.OfType<Group>().First(g => g.Name == "start")?.Value;
				var start = ALPHA.IndexOf(rawStart);

				var rawEnd = isSequenceAlpha.Groups.OfType<Group>().First(g => g.Name == "end")?.Value;
				var end = ALPHA.IndexOf(rawEnd);

				var rawInc = isSequenceAlpha.Groups.OfType<Group>().FirstOrDefault(g => g.Name == "inc")?.Value;
				if (!int.TryParse(rawInc, out var inc))
					inc = DEFAULT_INCREMENT;

				inc = Math.Max(1, Math.Abs(inc));

				if (start > end)
					inc = -inc;

				for (var i = start; inc > 0 ? i <= end : i >= end; i += inc)
					expansions.Add(ALPHA[i].ToString());
			}
			// Set
			else if (exp.Contains(','))
			{
				int elementStart = 0;
				int level = 0;

				for (int i = 0; i < exp.Length; i++)
				{
					var c = exp[i];
					switch (c)
					{
						case '{':
							level++;
							break;
						case '}':
							{
								if (--level == 0)
								{
									var element = exp[(elementStart)..(i + 1)];
									expansions.AddRange(Expand(element));
									elementStart = i + 1;
								}
							}
							break;
						case ',':
							if (level == 0)
							{
								var element = exp[elementStart..i];
								if (elementStart == 0 || exp[elementStart - 1] != '}')
									expansions.Add(element);
								elementStart = i + 1;
							}
							break;
					}
				}

				var end = exp[elementStart..];
				if (elementStart == 0 || exp[elementStart - 1] != '}')
					expansions.Add(end);
			}
			else
			{
				yield return expression;
				yield break;
			}

			foreach (var e in expansions)
			{
				var nextExp = expression[(match.Index + match.Length)..];
				var next = Expand(nextExp);
				foreach (var n in next)
					yield return expression[..match.Index] + e + n;
			}
		}
	}
}
