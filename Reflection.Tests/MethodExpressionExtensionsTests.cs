using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Shouldly;
using Xunit;

namespace ITLIBRIUM.Reflection
{
    public class MethodExpressionExtensionsTests
    {
        [Fact]
        public void CanGetParametersForParameterlessVoidMethod()
        {
            Expression<Action<Component>> expression = c => c.ParameterlessVoid();
            var parameters = expression.GetParameters().ToList();
            parameters.Count.ShouldBe(0);
        }

        [Fact]
        public void CanGetParametersForParameterlessNonVoidMethod()
        {
            Expression<Func<Component, int>> expression = c => c.ParameterlessNonVoid();
            var parameters = expression.GetParameters().ToList();
            parameters.Count.ShouldBe(0);
        }

        [Fact]
        public void CanGetParametersForSingleParameterVoidMethod()
        {
            Expression<Action<Component>> expression = c => c.SingleParameterVoid(3);
            var parameters = expression.GetParameters().ToList();
            parameters.Count.ShouldBe(1);
            parameters.ShouldContain(typeof(int));
        }

        [Fact]
        public void CanGetParametersForSingleParameterNonVoidMethod()
        {
            Expression<Func<Component, int>> expression = c => c.SingleParameterNonVoid("test");
            var parameters = expression.GetParameters().ToList();
            parameters.Count.ShouldBe(1);
            parameters.ShouldContain(typeof(string));
        }

        [Fact]
        public void CanGetParametersForMultiParametersVoidMethod()
        {
            Expression<Action<Component>> expression = c => c.MultiParametersVoid(3, new Parameter());
            var parameters = expression.GetParameters().ToList();
            parameters.Count.ShouldBe(2);
            parameters.ShouldContain(typeof(int));
            parameters.ShouldContain(typeof(Parameter));
        }

        [Fact]
        public void CanGetParametersForMultiParametersNonVoidMethod()
        {
            Expression<Func<Component, int>> expression = c => c.MultiParametersNonVoid("test", new Parameter());
            var parameters = expression.GetParameters().ToList();
            parameters.Count.ShouldBe(2);
            parameters.ShouldContain(typeof(string));
            parameters.ShouldContain(typeof(Parameter));
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private class Component
        {
            public void ParameterlessVoid() { }
            public int ParameterlessNonVoid() => 5;

            public void SingleParameterVoid(int id) { }
            public int SingleParameterNonVoid(string text) => 5;

            public void MultiParametersVoid(int id, Parameter parameter) { }
            public int MultiParametersNonVoid(string text, Parameter parameter) => 5;
        }

        private class Parameter { }
    }
}