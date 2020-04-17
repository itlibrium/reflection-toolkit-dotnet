using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Shouldly;
using Xunit;

namespace ITLIBRIUM.Reflection
{
    public class ExpressionTransformationExtensionsTests
    {
        [Fact]
        public void CanCreateSetterForPublicField()
        {
            Expression<Func<Component, string>> expression = c => c.Text;
            var setter = expression.CreateSetter();
            setter.ShouldNotBeNull();
        }

        [Fact]
        public void CanCreateSetterForPropertyWithPublicSetter()
        {
            Expression<Func<Component, int>> expression = c => c.Code;
            var setter = expression.CreateSetter();
            setter.ShouldNotBeNull();
        }

        [Fact]
        public void CanNotCreateSetterForReadOnlyProperty()
        {
            Expression<Func<Component, DateTime>> expression = c => c.Date;
            Should.Throw<ArgumentException>(() => expression.CreateSetter());
        }

        [Fact]
        public void CanNotCreateSetterForReadOnlyField()
        {
            Expression<Func<Component, string>> expression = c => c.Id;
            Should.Throw<ArgumentException>(() => expression.CreateSetter());
        }

        [Fact]
        public void CanNotCreateSetterForPropertyWithPrivateSetter()
        {
            Expression<Func<Component, int>> expression = c => c.No;
            Should.Throw<ArgumentException>(() => expression.CreateSetter());
        }

        [Fact]
        public void CantCreateSetterForMethod()
        {
            Expression<Func<Component, int>> expression = c => c.GetResult();
            Should.Throw<ArgumentException>(() => expression.CreateSetter());
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private class Component
        {
            public readonly string Id;

            public string Text;

            public DateTime Date { get; }

            public int No { get; private set; }

            public int Code { get; set; }

            public int GetResult() => 1;
        }
    }
}