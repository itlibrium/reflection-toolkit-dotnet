using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using FluentAssertions;
using JetBrains.Annotations;
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
            setter.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateSetterForPropertyWithPublicSetter()
        {
            Expression<Func<Component, int>> expression = c => c.Code;
            var setter = expression.CreateSetter();
            setter.Should().NotBeNull();
        }

        [Fact]
        public void CanNotCreateSetterForReadOnlyProperty()
        {
            Expression<Func<Component, DateTime>> expression = c => c.Date;
            Action action = () => expression.CreateSetter();
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CanNotCreateSetterForReadOnlyField()
        {
            Expression<Func<Component, string>> expression = c => c.Id;
            Action action = () => expression.CreateSetter();
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CanNotCreateSetterForPropertyWithPrivateSetter()
        {
            Expression<Func<Component, int>> expression = c => c.No;
            Action action = () => expression.CreateSetter();
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CantCreateSetterForMethod()
        {
            Expression<Func<Component, int>> expression = c => c.GetResult();
            Action action = () => expression.CreateSetter();
            action.Should().Throw<ArgumentException>();
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