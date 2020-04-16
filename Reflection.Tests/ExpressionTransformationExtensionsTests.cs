using System;
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

        [UsedImplicitly]
        private class Component
        {
            [UsedImplicitly]
            public readonly string Id;

            [UsedImplicitly]
            public string Text;
            
            [UsedImplicitly]
            public DateTime Date { get; }

            [UsedImplicitly]
            public int No { get; private set; }

            [UsedImplicitly]
            public int Code { get; set; }

            [UsedImplicitly]
            public int GetResult() => 1;
        }
    }
}