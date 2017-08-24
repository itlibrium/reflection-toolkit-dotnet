using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace itlibrium.Reflection
{
    public static class OpenInterfaceExtensions
    {
        public static IEnumerable<Type> GetClosedInterfaces(this Type type, params Type[] openInterfaces)
        {
            int count = openInterfaces.Length;
            if (count == 0)
                throw new ArgumentException("Interfaces list should not be empty", nameof(openInterfaces));

            var openInterfacesSet = new HashSet<Type>();
            for (int i = 0; i < count; i++)
            {
                Type openInterface = openInterfaces[i];
                TypeInfo openInterfaceInfo = openInterface.GetTypeInfo();
                if(!openInterfaceInfo.IsInterface || !openInterfaceInfo.IsGenericTypeDefinition)
                    throw new ArgumentException(
                        $"Interfaces list should contains only open generic interfaces. Wrong type: {openInterface.Name}", 
                        nameof(openInterfaces));

                openInterfacesSet.Add(openInterface);
            }

            return type.GetTypeInfo().ImplementedInterfaces
                .Where(i =>
                    i.GetTypeInfo().IsGenericType &&
                    openInterfacesSet.Contains(i.GetGenericTypeDefinition()));
        }
    }
}