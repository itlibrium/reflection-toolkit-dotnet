using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ITLIBRIUM.ReflectionToolkit
{
    public class ExpressionNameExtensionsTests
    {
        [Fact]
        public void CanGetNameOfPublicField()
        {
            Expression<Func<Component, string>> expression = c => c.Text;
            var name = expression.GetName();
            name.Should().Be(nameof(Component.Text));
        }

        [Fact]
        public void CanGetNameOfProperty()
        {
            Expression<Func<Component, int>> expression = c => c.No;
            var name = expression.GetName();
            name.Should().Be(nameof(Component.No));
        }

        [Fact]
        public void CanGetNameOfExplicitlyImplementedProperty()
        {
            Expression<Func<Component, int>> expression = c => ((IComponent) c).ExplicitlyImplementedProperty;
            var name = expression.GetName();
            name.Should().Be(nameof(IComponent.ExplicitlyImplementedProperty));
        }

        [Fact]
        public void CanGetNameOfParameterlessVoidMethod()
        {
            Expression<Action<Component>> expression = c => c.Execute();
            var name = expression.GetName();
            name.Should().Be(nameof(Component.Execute));
        }

        [Fact]
        public void CanGetNameOfVoidMethodWithParameter()
        {
            Expression<Action<Component>> expression = c => c.Execute(5);
            var name = expression.GetName();
            name.Should().Be(nameof(Component.Execute));
        }

        [Fact]
        public void CanGetNameOfParameterlessMethod()
        {
            Expression<Func<Component, int>> expression = c => c.GetResult();
            var name = expression.GetName();
            name.Should().Be(nameof(Component.GetResult));
        }

        [Fact]
        public void CanGetNameOfMethodWithParameter()
        {
            Expression<Func<Component, int>> expression = c => c.GetResult(5);
            var name = expression.GetName();
            name.Should().Be(nameof(Component.GetResult));
        }

        [Fact]
        public void CanGetNameOfExplicitlyImplementedMethod()
        {
            Expression<Func<Component, int>> expression = c => ((IComponent) c).ExplicitlyImplementedMethod();
            var name = expression.GetName();
            name.Should().Be(nameof(IComponent.ExplicitlyImplementedMethod));
        }

        [Fact]
        public void CanGetPathOfFieldMember()
        {
            Expression<Func<Component, int>> expression = c => c.ComplexField.No;
            var name = expression.GetPath();
            name.Should().Be($"{nameof(Component.ComplexField)}.{nameof(Component.No)}");
        }

        [Fact]
        public void CanGetPathOfPropertyMember()
        {
            Expression<Func<Component, int>> expression = c => c.ComplexProperty.No;
            var name = expression.GetPath();
            name.Should().Be($"{nameof(Component.ComplexProperty)}.{nameof(Component.No)}");
        }

        [Fact]
        public void CanGetPathOfMethodResultMember()
        {
            Expression<Func<Component, int>> expression = c => c.GetComplexResult().No;
            var name = expression.GetPath();
            name.Should().Be($"{nameof(Component.GetComplexResult)}.{nameof(Component.No)}");
        }

        [Fact]
        public void CanGetPathOfExplicitlyImplementedPropertyMember()
        {
            Expression<Func<Component, int>> expression = c => ((IComponent) c).ExplicitlyImplementedComplexProperty.No;
            var name = expression.GetPath();
            name.Should().Be($"{nameof(IComponent.ExplicitlyImplementedComplexProperty)}.{nameof(Component.No)}");
        }

        private interface IComponent
        {
            int ExplicitlyImplementedProperty { get; }
            Component ExplicitlyImplementedComplexProperty { get; }
            int ExplicitlyImplementedMethod();
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private class Component : IComponent
        {
            public string Text;

            public int No { get; set; }

            public Component ComplexField;

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