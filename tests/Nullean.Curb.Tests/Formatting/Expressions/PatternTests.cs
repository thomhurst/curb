namespace Nullean.Curb.Tests.Formatting.Expressions;

/// <summary>Recursive (property) patterns in <c>is</c> expressions.</summary>
public class PatternTests : FormattingTest
{
	[Test]
	public Task Typed_recursive_pattern_fits_on_one_line() => Unchanged(
		"""
		public class C
		{
		    public void M(Exception ex)
		    {
		        if (ex is HttpRequestException { StatusCode: 404 })
		        { }
		    }
		}
		""",
		editorConfig: "max_line_length = 100");

	[Test]
	public Task Typed_recursive_pattern_moves_its_brace_to_its_own_line_when_it_does_not_fit() => Formats(
		"""
		public class C
		{
		    public void M(Exception ex)
		    {
		        if (ex is HttpRequestException { StatusCode: HttpStatusCode.NotFound }) { }
		    }
		}
		""",
		"""
		public class C
		{
		    public void M(Exception ex)
		    {
		        if (
		            ex is HttpRequestException
		            {
		                StatusCode: HttpStatusCode.NotFound
		            }
		        )
		        { }
		    }
		}
		""",
		editorConfig: "max_line_length = 40");

	[Test]
	public Task Bare_recursive_pattern_fits_on_one_line() => Unchanged(
		"""
		public class C
		{
		    public void M(object value)
		    {
		        if (value is { Length: 0 })
		        { }
		    }
		}
		""",
		editorConfig: "max_line_length = 100");

	[Test]
	public Task Bare_recursive_pattern_moves_its_brace_to_its_own_line_when_it_does_not_fit() => Formats(
		"""
		public class C
		{
		    public void M(object value)
		    {
		        if (value is { Length: 0, Count: 1 }) { }
		    }
		}
		""",
		"""
		public class C
		{
		    public void M(object value)
		    {
		        if (
		            value is
		            {
		                Length: 0,
		                Count: 1
		            }
		        )
		        { }
		    }
		}
		""",
		editorConfig: "max_line_length = 40");

	// ---- preservation: a pattern the author opened out stays opened out -----------------------------

	[Test]
	public Task Multiline_property_pattern_with_a_trailing_comma_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		            Major: 1,
		            Minor: 2,
		        } version;
		    }
		}
		""");

	[Test]
	public Task Multiline_property_pattern_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		            Major: 1,
		            Minor: 2
		        };
		    }
		}
		""");

	[Test]
	public Task Multiline_bare_property_pattern_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is
		        {
		            Length: 0,
		            Count: 1,
		        };
		    }
		}
		""");

	[Test]
	public Task Nested_multiline_property_pattern_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Person
		        {
		            Name: "x",
		            Address:
		            {
		                City: "y",
		                Zip: "z",
		            },
		        };
		    }
		}
		""");

	[Test]
	public Task Property_pattern_members_the_author_kept_together_stay_together() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		            Major: 1, Minor: 2,
		            Build: 3,
		        };
		    }
		}
		""");

	[Test]
	public Task Multiline_positional_pattern_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Point(
		            1,
		            2) point;
		    }
		}
		""");

	[Test]
	public Task Positional_pattern_keeps_a_break_between_subpatterns() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Point(1,
		            2) point;
		    }
		}
		""");

	[Test]
	public Task Property_pattern_braces_open_out_once_its_members_span_lines() => Formats(
		// dotnet format moves both edges onto lines of their own, as it does an initializer's.
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version { Major: 1,
		            Minor: 2 };
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		            Major: 1,
		            Minor: 2
		        };
		    }
		}
		""");

	[Test]
	public Task Property_pattern_brace_follows_the_initializer_brace_option() => Formats(
		"""
		public class C {
		    public bool M(object value) {
		        return value is Version { Major: 1,
		            Minor: 2 };
		    }
		}
		""",
		"""
		public class C {
		    public bool M(object value) {
		        return value is Version {
		            Major: 1,
		            Minor: 2
		        };
		    }
		}
		""",
		editorConfig: "csharp_new_line_before_open_brace = none");

	[Test]
	public Task Property_pattern_after_a_broken_positional_pattern_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Point(
		            1,
		            2)
		        {
		            X: 1,
		            Y: 2,
		        };
		    }
		}
		""");

	[Test]
	public Task Multiline_list_pattern_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(int[] values)
		    {
		        return values is
		        [
		            1,
		            2,
		        ];
		    }
		}
		""");

	[Test]
	public Task Multiline_property_pattern_in_a_switch_arm_is_kept() => Unchanged(
		"""
		public class C
		{
		    public int M(object value) => value switch
		    {
		        Version
		        {
		            Major: 1,
		            Minor: 2,
		        } => 1,
		        _ => 0,
		    };
		}
		""");

	[Test]
	public Task Single_line_property_pattern_with_a_trailing_comma_has_one_space_before_its_brace() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version { Major: 1, Minor: 2, } version;
		    }
		}
		""");

	[Test]
	public Task Multiline_property_pattern_after_not_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is not
		        {
		            Length: 0,
		            Count: 1,
		        };
		    }
		}
		""");

	[Test]
	public Task Multiline_empty_property_pattern_is_kept() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		        } version;
		    }
		}
		""");

	[Test]
	public Task Empty_property_pattern_with_a_comment_keeps_it_at_the_members_level() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		            // nothing to match
		        };
		    }
		}
		""");

	[Test]
	public Task Empty_property_pattern_brace_follows_the_initializer_brace_option() => Formats(
		"""
		public class C {
		    public bool M(object value) {
		        return value is Version
		        {
		        };
		    }
		}
		""",
		"""
		public class C {
		    public bool M(object value) {
		        return value is Version {
		        };
		    }
		}
		""",
		editorConfig: "csharp_new_line_before_open_brace = none");

	[Test]
	public Task Property_pattern_under_indent_braces_matches_dotnet_format() => Formats(
		// dotnet format indents the brace a level, the members a level past it, and the closing brace
		// to the members' column, the same at every depth.
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        var a = value is Version
		        {
		            Major: 1,
		            Minor: 2,
		        } v;
		        var b = value is
		        {
		            Length: 0,
		        };
		        var c = value is Version { Major: 1,
		            Minor: 2 };
		        return value is Person
		        {
		            Name: "x",
		            Address:
		            {
		                City: "y",
		            },
		            Home: Address
		            {
		                Zip: 1,
		            },
		        };
		    }
		}
		""",
		"""
		public class C
		    {
		    public bool M(object value)
		        {
		        var a = value is Version
		            {
		                Major: 1,
		                Minor: 2,
		                } v;
		        var b = value is
		            {
		                Length: 0,
		                };
		        var c = value is Version
		            {
		                Major: 1,
		                Minor: 2
		                };
		        return value is Person
		            {
		                Name: "x",
		                Address:
		                    {
		                        City: "y",
		                        },
		                Home: Address
		                    {
		                        Zip: 1,
		                        },
		                };
		        }
		    }
		""",
		editorConfig: "csharp_indent_braces = true");

	[Test]
	public Task Empty_property_pattern_and_patterns_after_not_and_or_under_indent_braces() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        var a = value is not
		        {
		            Length: 0,
		        };
		        var b = value is string or
		        {
		            Length: 0,
		        };
		        var c = value is Version
		        {
		        } w;
		        return value is Version
		        {
		            // none
		        };
		    }
		}
		""",
		"""
		public class C
		    {
		    public bool M(object value)
		        {
		        var a = value is not
		            {
		                Length: 0,
		                };
		        var b = value is string or
		            {
		                Length: 0,
		                };
		        var c = value is Version
		            {
		                } w;
		        return value is Version
		            {
		                // none
		                };
		        }
		    }
		""",
		editorConfig: "csharp_indent_braces = true");

	// ---- preservation: a pattern's continuation lines keep the author's offset ----------------------

	// Issue #111: each continuation line used to break to the statement's own column, or one level in.
	[Test]
	public Task Pattern_continuation_lines_keep_the_authors_indent() => Unchanged(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        var a = value is
		            null;
		        var b = value is string or
		            null;
		        var c = value is Point(1,
		                               2);
		        var d = value is not
		            null;
		        return value is [1,
		                         2];
		    }
		}
		""");

	[Test]
	public Task Pattern_continuation_lines_move_with_a_reindented_statement() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		            var a = value is
		                null;
		            var b = value is string or
		                null;
		            var c = value is Point(1,
		                                   2);
		            var e = value is Point(
		                1,
		                2
		                );
		            var f = value is
		                [
		                    1,
		                    2,
		                ];
		            var g = value is { } and
		                not null;
		            return value is [1,
		                             2];
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        var a = value is
		            null;
		        var b = value is string or
		            null;
		        var c = value is Point(1,
		                               2);
		        var e = value is Point(
		            1,
		            2
		            );
		        var f = value is
		            [
		                1,
		                2,
		            ];
		        var g = value is { } and
		            not null;
		        return value is [1,
		                         2];
		    }
		}
		""");

	[Test]
	public Task Pattern_broken_before_is_or_or_keeps_the_authors_indent() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		            var a = value
		                is null;
		            return value is string
		                or null;
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        var a = value
		            is null;
		        return value is string
		            or null;
		    }
		}
		""");

	[Test]
	public Task Pattern_continuation_in_a_switch_arm_or_case_label_keeps_its_offset() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		            switch (value)
		            {
		                    case 1 or
		                        2:
		                        break;
		            }
		            return value switch
		            {
		                    1 or
		                        2 => true,
		                    _ => false
		            };
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        switch (value)
		        {
		            case 1 or
		                    2:
		                break;
		        }
		        return value switch
		        {
		            1 or
		                2 => true,
		            _ => false
		        };
		    }
		}
		""");

	// A case label is not a statement: dotnet format moves its continuation lines with the switch, so
	// a label it indents under csharp_indent_switch_labels leaves them where they were.
	[Test]
	public Task Pattern_continuation_in_a_case_label_moves_with_the_switch() => Formats(
		"""
		public class C
		{
		    public void M(object value)
		    {
		        switch (value)
		        {
		        case 1 or
		            2:
		            break;
		        case Point(1,
		                   2):
		            break;
		        case [1,
		              2] when value is
		                  null:
		            break;
		        }
		    }
		}
		""",
		"""
		public class C
		{
		    public void M(object value)
		    {
		        switch (value)
		        {
		            case 1 or
		            2:
		                break;
		            case Point(1,
		                   2):
		                break;
		            case [1,
		              2] when value is
		                  null:
		                break;
		        }
		    }
		}
		""");

	[Test]
	public Task Nested_pattern_continuation_lines_keep_their_offsets() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		            var a = value is [1,
		                    [2,
		                     3]];
		            var b = value is (1 or
		                              2);
		            return value is Point(
		                    X: 1,
		                    Y:
		                        2);
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        var a = value is [1,
		                [2,
		                 3]];
		        var b = value is (1 or
		                          2);
		        return value is Point(
		                X: 1,
		                Y:
		                    2);
		    }
		}
		""");

	// The type keeps the author's offset; the brace after it is still placed by the brace option.
	[Test]
	public Task Typed_pattern_on_its_own_line_after_is_keeps_its_offset() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		            var a = value is
		                Version
		                {
		                    Major: 1
		                };
		            var b = value is not
		                Version { Major: 1 };
		            return value is Point(1,
		                                  2)
		            {
		                X: 1
		            };
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        var a = value is
		            Version
		        {
		            Major: 1
		        };
		        var b = value is not
		            Version { Major: 1 };
		        return value is Point(1,
		                              2)
		        {
		            X: 1
		        };
		    }
		}
		""");

	[Test]
	public Task Pattern_after_a_comment_on_the_is_line_keeps_its_offset() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		            return value is // note
		                null;
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is // note
		            null;
		    }
		}
		""");

	[Test]
	public Task Pattern_continuation_lines_keep_their_offset_under_tabs_and_crlf() => FormatsExactly(
		"public class C\r\n{\r\n\tpublic bool M(object value)\r\n\t{\r\n\t\t\tvar a = value is\r\n\t\t\t\tnull;\r\n"
		+ "\t\t\tvar c = value is Point(1,\r\n\t\t\t                       2);\r\n"
		+ "\t\t\treturn value is [1,\r\n\t\t\t\t2] or\r\n\t\t\t\tnull;\r\n\t}\r\n}\r\n",
		"public class C\r\n{\r\n\tpublic bool M(object value)\r\n\t{\r\n\t\tvar a = value is\r\n\t\t\tnull;\r\n"
		+ "\t\tvar c = value is Point(1,\r\n\t\t\t\t\t\t\t   2);\r\n"
		+ "\t\treturn value is [1,\r\n\t\t\t2] or\r\n\t\t\tnull;\r\n\t}\r\n}\r\n",
		editorConfig: "indent_style = tab\nend_of_line = crlf");

	// ---- deterministic: a pattern that fits is joined, and the comma follows the options ------------

	[Test]
	public Task Collapsed_property_pattern_keeps_one_space_before_its_brace() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		            Major: 1,
		            Minor: 2,
		        } version;
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version { Major: 1, Minor: 2, } version;
		    }
		}
		""",
		editorConfig: "max_line_length = 100");

	[Test]
	public Task Collapsed_property_pattern_drops_its_comma_under_the_multiline_option() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		            Major: 1,
		            Minor: 2,
		        } version;
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version { Major: 1, Minor: 2 } version;
		    }
		}
		""",
		editorConfig: "max_line_length = 100\ncsharp_trailing_comma_in_multiline_lists = true");

	[Test]
	public Task Broken_property_pattern_gains_a_comma_under_the_multiline_option() => Formats(
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version { Major: 1, Minor: 2 } version;
		    }
		}
		""",
		"""
		public class C
		{
		    public bool M(object value)
		    {
		        return value is Version
		        {
		            Major: 1,
		            Minor: 2,
		        } version;
		    }
		}
		""",
		editorConfig: "max_line_length = 40\ncsharp_trailing_comma_in_multiline_lists = true");
}
