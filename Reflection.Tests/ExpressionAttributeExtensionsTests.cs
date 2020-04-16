using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Shouldly;
using Xunit;

namespace ITLIBRIUM.Reflection
{
    public class ExpressionAttributeExtensionsTests
    {
        [Fact]
        public void CanGetAttributeFromPublicField()
        {
            Expression<Func<Component, string>> expression = c => c.Text;
            var attribute = expression.GetAttribute<CustomAttribute>();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public void CanGetAttributeFromPropertyWithPublicGetter()
        {
            Expression<Func<Component, int>> expression = c => c.No;
            var attribute = expression.GetAttribute<CustomAttribute>();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public void CanGetAttributeFromParameterlessVoidMethod()
        {
            Expression<Action<Component>> expression = c => c.Execute();
            var attribute = expression.GetAttribute<CustomAttribute>();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public void CanGetAttributeFromVoidMethodWithParameter()
        {
            Expression<Action<Component>> expression = c => c.Execute(5);
            var attribute = expression.GetAttribute<CustomAttribute>();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public void CanGetAttributeFromParameterlessMethod()
        {
            Expression<Func<Component, int>> expression = c => c.GetResult();
            var attribute = expression.GetAttribute<CustomAttribute>();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public void CanGetAttributeFromMethodWithParameter()
        {
            Expression<Func<Component, int>> expression = c => c.GetResult(5);
            var attribute = expression.GetAttribute<CustomAttribute>();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public void CanGetAttributeFromConstructor()
        {
            Expression<Func<Component>> expression = () => new Component(3);
            var attribute = expression.GetAttribute<CustomAttribute>();
            attribute.ShouldNotBeNull();
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private class Component
        {
            [Custom]
            public string Text;

            [Custom]
            public int No { get; private set; }

            [Custom]
            public Component(int no)
            {
                No = no;
            }

            [Custom]
            public void Execute() { }

            [Custom]
            public void Execute(int no)
            {
                No = no;
            }

            [Custom]
            public int GetResult() => 1;

            [Custom]
            public int GetResult(int no) => no;
        }

        private class CustomAttribute : Attribute { }
    }
}