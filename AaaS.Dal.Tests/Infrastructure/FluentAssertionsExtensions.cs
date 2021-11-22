using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Tests.Infrastructure
{
    public static class FluentAssertionsExtensions
    {
        public static EquivalencyAssertionOptions<TExpectation> ApproximateDateTime<TExpectation>(EquivalencyAssertionOptions<TExpectation> o) 
            => o.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
            .WhenTypeIs<DateTime>()
            .Using<TimeSpan>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
            .WhenTypeIs<TimeSpan>();
    }
}
