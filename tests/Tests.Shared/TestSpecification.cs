using System;
using System.Linq.Expressions;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Specifications;

namespace StartupCreativeAgency.Tests.Shared
{
    public class TestSpecification : BaseSpecification<BaseEntity<int>>
    {
        public TestSpecification() : this(x => true) { }

        public TestSpecification(Expression<Func<BaseEntity<int>, bool>> criteria)
            : base(criteria) { }

    }
}
