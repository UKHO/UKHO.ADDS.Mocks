using System.Reflection;
using System.Runtime.CompilerServices;

namespace UKHO.ADDS.Mocks.Domain.Guard
{
    /// <content>Provides utilities to support legacy frameworks.</content>
    public static partial class Guard
    {
        /// <summary>Determines whether the specified type is a value type.</summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="type" /> represents a value type; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }

        /// <summary>Determines whether the specified type is a generic type.</summary>
        /// <param name="type">The type to check.</param>
        /// <param name="definition">The type definition.</param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="type" /> represents a generic type with the specified
        ///     definition; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsGenericType(this Type type, Type definition)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == definition;
        }

        /// <summary>Determines whether the specified type is an enum.</summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <c>true</c>, if <paramref name="type" /> represents an enumeration; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEnum(this Type type)
        {
            return type.IsEnum;
        }

        /// <summary>Returns the type from which the specified type directly inherits.</summary>
        /// <param name="type">The type whose base type to return.</param>
        /// <returns>
        ///     The type from which the <paramref name="type" /> directly inherits, if there is one;
        ///     otherwise, <c>null</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type? GetBaseType(this Type type)
        {
            return type.BaseType;
        }

        /// <summary>Returns the getter of the property with the specified name.</summary>
        /// <param name="type">The type that the property belongs to.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        ///     The getter of the property with the specified name, if it can be found in
        ///     <paramref name="type" />; otherwise, <c>null</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MethodInfo? GetPropertyGetter(this Type type, string name)
        {
            return type.GetProperty(name)?.GetGetMethod();
        }


        /// <summary>Provides a cached, empty array.</summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        private static class Array<T>
        {
            /// <summary>Gets an empty array.</summary>
            public static T[] Empty => Array.Empty<T>();
        }
    }
}
