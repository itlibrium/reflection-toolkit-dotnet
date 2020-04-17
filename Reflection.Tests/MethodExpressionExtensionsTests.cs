using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using JetBrains.Annotations;
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
            parameters.Count.Should().Be(0);
        }

        [Fact]
        public void CanGetParametersForParameterlessNonVoidMethod()
        {
            Expression<Func<Component, int>> expression = c => c.ParameterlessNonVoid();
            var parameters = expression.GetParameters().ToList();
            parameters.Count.Should().Be(0);
        }

        [Fact]
        public void CanGetParametersForSingleParameterVoidMethod()
        {
            Expression<Action<Component>> expression = c => c.SingleParameterVoid(3);
            var parameters = expression.GetParameters().ToList();
            parameters.Count.Should().Be(1);
            parameters.Should().Contain(typeof(int));
        }

        [Fact]
        public void CanGetParametersForSingleParameterNonVoidMethod()
        {
            Expression<Func<Component, int>> expression = c => c.SingleParameterNonVoid("test");
            var parameters = expression.GetParameters().ToList();
            parameters.Count.Should().Be(1);
            parameters.Should().Contain(typeof(string));
        }

        [Fact]
        public void CanGetParametersForMultiParametersVoidMethod()
        {
            Expression<Action<Component>> expression = c =>
                c.MultiParametersVoid(3, new ClassParameter(), new StructParameter());
            var parameters = expression.GetParameters().ToList();
            parameters.Count.Should().Be(3);
            parameters.Should().Contain(typeof(int));
            parameters.Should().Contain(typeof(ClassParameter));
            parameters.Should().Contain(typeof(StructParameter));
        }

        [Fact]
        public void CanGetParametersForMultiParametersNonVoidMethod()
        {
            Expression<Func<Component, int>> expression = c =>
                c.MultiParametersNonVoid("test", new ClassParameter(), ReadonlyStructParameter.New(1.23));
            var parameters = expression.GetParameters().ToList();
            parameters.Count.Should().Be(3);
            parameters.Should().Contain(typeof(string));
            parameters.Should().Contain(typeof(ClassParameter));
            parameters.Should().Contain(typeof(ReadonlyStructParameter));
        }

        [Fact]
        public void CanGetParameterValuesForSingleParameterVoidMethod()
        {
            Expression<Action<Component>> expression = c => c.SingleParameterVoid(3);
            var parameterValues = expression.GetParameterValues().ToList();
            parameterValues.Should().HaveCount(1);
            parameterValues[0].Should().Be(3);
        }

        [Fact]
        public void CanGetParameterValuesForSingleParameterNonVoidMethod()
        {
            Expression<Func<Component, int>> expression = c => c.SingleParameterNonVoid("test");
            var parameterValues = expression.GetParameterValues().ToList();
            parameterValues.Should().HaveCount(1);
            parameterValues[0].Should().Be("test");
        }

        [Fact]
        public void CanGetParameterValuesForMultiParametersVoidMethod()
        {
            var classParameter = new ClassParameter();
            var structParameter = new StructParameter();
            Expression<Action<Component>> expression = c => c.MultiParametersVoid(3, classParameter, structParameter);
            var parameterValues = expression.GetParameterValues().ToList();
            parameterValues.Should().HaveCount(3);
            parameterValues[0].Should().Be(3);
            parameterValues[1].Should().Be(classParameter);
            parameterValues[2].Should().Be(structParameter);
        }

        [Fact]
        public void CanGetParameterValuesForMultiParametersNonVoidMethod()
        {
            Expression<Func<Component, int>> expression = c =>
                c.MultiParametersNonVoid("test", new ClassParameter(), ReadonlyStructParameter.New(1.23));
            var parameterValues = expression.GetParameterValues().ToList();
            parameterValues.Count.Should().Be(3);
            parameterValues[0].Should().Be("test");
            parameterValues[1].Should().BeOfType<ClassParameter>();
            parameterValues[2].Should().BeOfType<ReadonlyStructParameter>();
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        private class Component
        {
            public void ParameterlessVoid() { }
            public int ParameterlessNonVoid() => 5;

            public void SingleParameterVoid(int id) { }
            public int SingleParameterNonVoid(string text) => 5;

            public void MultiParametersVoid(int id, ClassParameter classParameter, StructParameter structParameter) { }

            public int MultiParametersNonVoid(string text, ClassParameter classParameter,
                ReadonlyStructParameter structParameter) => 5;
        }

        private class ClassParameter { }

        private struct StructParameter { }

        private readonly struct ReadonlyStructParameter
        {
            [UsedImplicitly]
            public double Value { get; }

            public static ReadonlyStructParameter New(double value) => new ReadonlyStructParameter(value);
            
            private ReadonlyStructParameter(double value) => Value = value;
        }
    }
}