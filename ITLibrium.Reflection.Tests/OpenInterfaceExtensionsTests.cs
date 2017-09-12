using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace ITLibrium.Reflection.Tests
{
    public class OpenInterfaceExtensionsTests
    {
        [Fact]
        public void ClosedInterfacesFoundCorrectly()
        {
            List<Type> interfaces = typeof(TestClass).GetClosedInterfaces(typeof(IInterfaceA<>), typeof(IInterfaceB<>)).ToList();
            interfaces.Count.ShouldBe(3);
            interfaces.ShouldContain(typeof(IInterfaceA<int>));
            interfaces.ShouldContain(typeof(IInterfaceA<string>));
            interfaces.ShouldContain(typeof(IInterfaceB<double>));
        }

        [Fact]
        public void InterfacesListShouldNotBeEmpty()
        {
            Should.Throw<ArgumentException>(() => typeof(TestClass).GetClosedInterfaces());
        }

        [Fact]
        public void InterfacesListShouldContainsOnlyOpenGenerics()
        {
            Should.Throw<ArgumentException>(() => typeof(TestClass).GetClosedInterfaces(typeof(IInterfaceA<int>)));
        }

        [Fact]
        public void InterfacesListShouldContainsOnlyInterfaces()
        {
            Should.Throw<ArgumentException>(() => typeof(TestClass).GetClosedInterfaces(typeof(GenericClass<>)));
        }

        private interface IInterfaceA<T> { }
        private interface IInterfaceB<T> { }
        private interface IInterfaceC { }
        private class GenericClass<T> { }

        private class TestClass : IInterfaceA<int>, IInterfaceA<string>, IInterfaceB<double>, IInterfaceC { }
    }
}