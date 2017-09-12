using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Shouldly;
using Xunit;

namespace ITLibrium.Reflection.Tests
{
    public class ExpressionTransformationExtensionsTests
    {
        [Fact]
        public void CanCreateSetterForPublicField()
        {
            Expression<Func<Component, string>> expression = c => c.Text;
            Action<Component, string> setter = expression.CreateSetter();
            setter.ShouldNotBeNull();
        }

        [Fact]
        public void CanCreateSetterForPropertyWithPublicSetter()
        {
            Expression<Func<Component, int>> expression = c => c.Code;
            Action<Component, int> setter = expression.CreateSetter();
            setter.ShouldNotBeNull();
        }

        [Fact]
        public void CantCreateSetterForReadOnlyField()
        {
            Expression<Func<Component, string>> expression = c => c.Id;
            Should.Throw<ArgumentException>(() => expression.CreateSetter());
        }

        [Fact]
        public void CantCreateSetterForPropertyWithPrivateSetter()
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

        [UsedImplicitly]
        private class Component
        {
            [UsedImplicitly]
            public readonly string Id;

            [UsedImplicitly]
            public string Text;

            [UsedImplicitly]
            public int No { get; private set; }

            [UsedImplicitly]
            public int Code { get; set; }

            [UsedImplicitly]
            public int GetResult() => 1;
        }
    }
}