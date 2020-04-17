using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace ITLIBRIUM.ReflectionToolkit
{
    public class OpenInterfaceExtensionsTests
    {
        [Fact]
        public void ClosedInterfacesFoundCorrectly()
        {
            var interfaces = typeof(TestClass).GetClosedInterfaces(typeof(IInterfaceA<>), typeof(IInterfaceB<>))
                .ToList();
            interfaces.Count.Should().Be(3);
            interfaces.Should().Contain(typeof(IInterfaceA<int>));
            interfaces.Should().Contain(typeof(IInterfaceA<string>));
            interfaces.Should().Contain(typeof(IInterfaceB<double>));
        }

        [Fact]
        public void InterfacesListShouldNotBeEmpty()
        {
            Func<IEnumerable<Type>> action = () => typeof(TestClass).GetClosedInterfaces();
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void InterfacesListShouldContainsOnlyOpenGenerics()
        {
            Func<IEnumerable<Type>> action = () => typeof(TestClass).GetClosedInterfaces(typeof(IInterfaceA<int>));
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void InterfacesListShouldContainsOnlyInterfaces()
        {
            Func<IEnumerable<Type>> action = () => typeof(TestClass).GetClosedInterfaces(typeof(GenericClass<>));
            action.Should().Throw<ArgumentException>();
        }

        private interface IInterfaceA<T> { }

        private interface IInterfaceB<T> { }

        private interface IInterfaceC { }

        private class GenericClass<T> { }

        private class TestClass : IInterfaceA<int>, IInterfaceA<string>, IInterfaceB<double>, IInterfaceC { }
    }
}