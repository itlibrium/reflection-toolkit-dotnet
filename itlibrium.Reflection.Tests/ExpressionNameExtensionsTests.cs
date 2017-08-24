using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Shouldly;
using Xunit;

namespace itlibrium.Reflection.Tests
{
    public class ExpressionNameExtensionsTests
    {
        [Fact]
        public void CanGetNameOfPublicField()
        {
            Expression<Func<Component, string>> expression = c => c.Text;
            string name = expression.GetName();
            name.ShouldBe(nameof(Component.Text));
        }

        [Fact]
        public void CanGetNameOfProperty()
        {
            Expression<Func<Component, int>> expression = c => c.No;
            string name = expression.GetName();
            name.ShouldBe(nameof(Component.No));
        }

        [Fact]
        public void CanGetNameOfExplicitlyImplementedProperty()
        {
            Expression<Func<Component, int>> expression = c => ((IComponent) c).ExplicitlyImplementedProperty;
            string name = expression.GetName();
            name.ShouldBe(nameof(IComponent.ExplicitlyImplementedProperty));
        }

        [Fact]
        public void CanGetNameOfParameterlessVoidMethod()
        {
            Expression<Action<Component>> expression = c => c.Execute();
            string name = expression.GetName();
            name.ShouldBe(nameof(Component.Execute));
        }

        [Fact]
        public void CanGetNameOfVoidMethodWithParameter()
        {
            Expression<Action<Component>> expression = c => c.Execute(5);
            string name = expression.GetName();
            name.ShouldBe(nameof(Component.Execute));
        }

        [Fact]
        public void CanGetNameOfParameterlessMethod()
        {
            Expression<Func<Component, int>> expression = c => c.GetResult();
            string name = expression.GetName();
            name.ShouldBe(nameof(Component.GetResult));
        }

        [Fact]
        public void CanGetNameOfMethodWithParameter()
        {
            Expression<Func<Component, int>> expression = c => c.GetResult(5);
            string name = expression.GetName();
            name.ShouldBe(nameof(Component.GetResult));
        }

        [Fact]
        public void CanGetNameOfExplicitlyImplementedMethod()
        {
            Expression<Func<Component, int>> expression = c => ((IComponent)c).ExplicitlyImplementedMethod();
            string name = expression.GetName();
            name.ShouldBe(nameof(IComponent.ExplicitlyImplementedMethod));
        }

        [Fact]
        public void CanGetPathOfFieldMember()
        {
            Expression<Func<Component, int>> expression = c => c.ComplexField.No;
            string name = expression.GetPath();
            name.ShouldBe($"{nameof(Component.ComplexField)}.{nameof(Component.No)}");
        }

        [Fact]
        public void CanGetPathOfPropertyMember()
        {
            Expression<Func<Component, int>> expression = c => c.ComplexProperty.No;
            string name = expression.GetPath();
            name.ShouldBe($"{nameof(Component.ComplexProperty)}.{nameof(Component.No)}");
        }

        [Fact]
        public void CanGetPathOfMethodResultMember()
        {
            Expression<Func<Component, int>> expression = c => c.GetComplexResult().No;
            string name = expression.GetPath();
            name.ShouldBe($"{nameof(Component.GetComplexResult)}.{nameof(Component.No)}");
        }

        [Fact]
        public void CanGetPathOfExplicitlyImplementedPropertyMember()
        {
            Expression<Func<Component, int>> expression = c => ((IComponent)c).ExplicitlyImplementedComplexProperty.No;
            string name = expression.GetPath();
            name.ShouldBe($"{nameof(IComponent.ExplicitlyImplementedComplexProperty)}.{nameof(Component.No)}");
        }

        private interface IComponent
        {
            int ExplicitlyImplementedProperty { get; }
            Component ExplicitlyImplementedComplexProperty { get; }
            int ExplicitlyImplementedMethod();
        }

        [UsedImplicitly]
        private class Component : IComponent
        {
            [UsedImplicitly]
            public string Text;

            public int No { get; set; }

            [UsedImplicitly]
            public Component ComplexField;

            [UsedImplicitly]
            public Component ComplexProperty { get; }

            public void Execute() { }

            public void Execute(int no)
            {
                No = no;
            }

            public int GetResult() => 1;

            public int GetResult(int no) => no;

            public Component GetComplexResult() => new Component();

            int IComponent.ExplicitlyImplementedProperty => 3;

            int IComponent.ExplicitlyImplementedMethod() => 5;

            Component IComponent.ExplicitlyImplementedComplexProperty => new Component();
        }
    }
}
