﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace SimpleInjector.Extensions
{
    /// <summary>
    /// Represents the method that will called to register one or multiple concrete. non-generic
    /// <paramref name="implementations"/> of the given closed generic type 
    /// <paramref name="closedServiceType"/>.
    /// </summary>
    /// <param name="closedServiceType">The service type that needs to be registered.</param>
    /// <param name="implementations">One or more concrete types that implement the given 
    /// <paramref name="closedServiceType"/>.</param>
    public delegate void RegistrationCallback(Type closedServiceType, Type[] implementations);

    /// <summary>
    /// Defines the accessibility of the types to search.
    /// </summary>
    public enum AccessibilityOption
    {
        /// <summary>Load both public as internal types from the given assemblies.</summary>
        AllTypes = 0,

        /// <summary>Only load publicly exposed types from the given assemblies.</summary>
        PublicTypesOnly = 1,
    }

    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for registration many concrete types at
    /// once that implement the same open generic service types in the <see cref="Container"/>.
    /// </summary>
    public static partial class OpenGenericBatchRegistrationExtensions
    {
        /// <summary>
        /// Registers all concrete, non-generic, publicly exposed types in the given set of
        /// <paramref name="assemblies"/> that implement the given <paramref name="openGenericServiceType"/> 
        /// with a transient lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, or <paramref name="assemblies"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given set of 
        /// <paramref name="assemblies"/> contain multiple publicly exposed types that implement the same 
        /// closed generic version of the given <paramref name="openGenericServiceType"/>.</exception>
        public static void RegisterManyForOpenGeneric(this Container container, 
            Type openGenericServiceType, params Assembly[] assemblies)
        {
            container.RegisterManyForOpenGenericInternal(openGenericServiceType, assemblies, 
                AccessibilityOption.PublicTypesOnly);
        }

        /// <summary>
        /// Registers all concrete, non-generic, publicly exposed types in the given 
        /// <paramref name="assemblies"/> that implement the given <paramref name="openGenericServiceType"/> 
        /// with a transient lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, or <paramref name="assemblies"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given set of 
        /// <paramref name="assemblies"/> contain multiple publicly exposed types that implement the same 
        /// closed generic version of the given <paramref name="openGenericServiceType"/>.</exception>
        public static void RegisterManyForOpenGeneric(this Container container,
            Type openGenericServiceType, IEnumerable<Assembly> assemblies)
        {
            container.RegisterManyForOpenGenericInternal(openGenericServiceType, assemblies, 
                AccessibilityOption.PublicTypesOnly);
        }

        /// <summary>
        /// Registers  all concrete, non-generic types with the given <paramref name="accessibility"/> in the 
        /// given <paramref name="assemblies"/> that implement the given 
        /// <paramref name="openGenericServiceType"/> with a transient lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="accessibility">Defines which types should be used from the given assemblies.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, or <paramref name="assemblies"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given set of 
        /// <paramref name="assemblies"/> contain multiple types that implement the same closed generic 
        /// version of the given <paramref name="openGenericServiceType"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="accessibility"/> 
        /// contains an invalid value.</exception>
        public static void RegisterManyForOpenGeneric(this Container container,
            Type openGenericServiceType, AccessibilityOption accessibility, params Assembly[] assemblies)
        {
            container.RegisterManyForOpenGenericInternal(openGenericServiceType, assemblies, accessibility);
        }

        /// <summary>
        /// Registers all concrete, non-generic types with the given <paramref name="accessibility"/> in the 
        /// given <paramref name="assemblies"/> that implement the given 
        /// <paramref name="openGenericServiceType"/> with a transient lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="accessibility">Defines which types should be used from the given assemblies.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, or <paramref name="assemblies"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given set of 
        /// <paramref name="assemblies"/> contain multiple types that implement the same 
        /// closed generic version of the given <paramref name="openGenericServiceType"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="accessibility"/> 
        /// contains an invalid value.</exception>
        public static void RegisterManyForOpenGeneric(this Container container,
            Type openGenericServiceType, AccessibilityOption accessibility, IEnumerable<Assembly> assemblies)
        {
            container.RegisterManyForOpenGenericInternal(openGenericServiceType, assemblies, accessibility);
        }

        /// <summary>
        /// Allows registration of all concrete, non-generic types with the given 
        /// <paramref name="accessibility"/> in the given set of <paramref name="assemblies"/> that implement 
        /// the given <paramref name="openGenericServiceType"/>, by supplying a 
        /// <see cref="RegistrationCallback"/> delegate, that will be called for each found closed generic 
        /// implementation of the given <paramref name="openGenericServiceType"/>.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="accessibility">Defines which types should be used from the given assemblies.</param>
        /// <param name="callback">The delegate that will be called for each found closed generic version of
        /// the given open generic <paramref name="openGenericServiceType"/> to do the actual registration.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, <paramref name="callback"/>, or 
        /// <paramref name="assemblies"/> contain a null reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="accessibility"/> 
        /// contains an invalid value.</exception>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "container",
            Justification = "By using the 'this Container' argument, we allow this extension method to " +
            "show when using Intellisense over the Container.")]
        public static void RegisterManyForOpenGeneric(this Container container,
            Type openGenericServiceType, AccessibilityOption accessibility, RegistrationCallback callback, 
            IEnumerable<Assembly> assemblies)
        {
            RegisterManyForOpenGenericInternal(openGenericServiceType, assemblies, accessibility, callback);
        }

        /// <summary>
        /// Registers all concrete, non-generic, publicly exposed types in the given 
        /// <paramref name="assemblies"/> that implement the given 
        /// <paramref name="openGenericServiceType"/> with a singleton lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, or <paramref name="assemblies"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given set of 
        /// <paramref name="assemblies"/> contain multiple publicly exposed types that implement the same 
        /// closed generic version of the given <paramref name="openGenericServiceType"/>.</exception>
        public static void RegisterManySinglesForOpenGeneric(this Container container,
            Type openGenericServiceType, params Assembly[] assemblies)
        {
            container.RegisterManySinglesForOpenGenericInternal(openGenericServiceType, assemblies,
                AccessibilityOption.PublicTypesOnly);
        }

        /// <summary>
        /// Registers all concrete, non-generic, publicly exposed types in the given 
        /// <paramref name="assemblies"/> that implement the given <paramref name="openGenericServiceType"/> 
        /// with a singleton lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, or <paramref name="assemblies"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given set of 
        /// <paramref name="assemblies"/> contain multiple publicly exposed types that implement the same 
        /// closed generic version of the given <paramref name="openGenericServiceType"/>.</exception>
        public static void RegisterManySinglesForOpenGeneric(this Container container,
            Type openGenericServiceType, IEnumerable<Assembly> assemblies)
        {
            container.RegisterManySinglesForOpenGenericInternal(openGenericServiceType, assemblies,
                AccessibilityOption.PublicTypesOnly);
        }

        /// <summary>
        /// Registers  all concrete, non-generic types with the given <paramref name="accessibility"/> in the 
        /// given <paramref name="assemblies"/> that implement the given 
        /// <paramref name="openGenericServiceType"/> with a singleton lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="accessibility">Defines which types should be used from the given assemblies.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, or <paramref name="assemblies"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given set of 
        /// <paramref name="assemblies"/> contain multiple types that implement the same closed generic 
        /// version of the given <paramref name="openGenericServiceType"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="accessibility"/> 
        /// contains an invalid value.</exception>
        public static void RegisterManySinglesForOpenGeneric(this Container container,
            Type openGenericServiceType, AccessibilityOption accessibility, params Assembly[] assemblies)
        {
            container.RegisterManySinglesForOpenGenericInternal(openGenericServiceType, assemblies, accessibility);
        }

        /// <summary>
        /// Registers all concrete, non-generic types with the given <paramref name="accessibility"/> in the 
        /// given <paramref name="assemblies"/> that implement the given 
        /// <paramref name="openGenericServiceType"/> with a singleton lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="accessibility">Defines which types should be used from the given assemblies.</param>
        /// <param name="assemblies">A list of assemblies that will be searched.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>,
        /// <paramref name="openGenericServiceType"/>, or <paramref name="assemblies"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="openGenericServiceType"/> is not
        /// an open generic type.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the given set of 
        /// <paramref name="assemblies"/> contain multiple types that implement the same 
        /// closed generic version of the given <paramref name="openGenericServiceType"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="accessibility"/> 
        /// contains an invalid value.</exception>
        public static void RegisterManySinglesForOpenGeneric(this Container container,
            Type openGenericServiceType, AccessibilityOption accessibility, IEnumerable<Assembly> assemblies)
        {
            container.RegisterManySinglesForOpenGenericInternal(openGenericServiceType, assemblies, accessibility);
        }
        
        /// <summary>
        /// Registers all supplied <paramref name="typesToRegister"/> by a closed generic definition of the
        /// given <paramref name="openGenericServiceType"/> with a transient lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="typesToRegister">The list of types that must be registered according to the given
        /// <paramref name="openGenericServiceType"/> definition.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>, 
        /// <paramref name="openGenericServiceType"/>, or <paramref name="typesToRegister"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="typesToRegister"/> contains a null
        /// (Nothing in VB) element, when the <paramref name="openGenericServiceType"/> is not an open generic
        /// type, or one of the types supplied in <paramref name="typesToRegister"/> does not implement a 
        /// closed version of <paramref name="openGenericServiceType"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown when there are multiple types in the given
        /// <paramref name="typesToRegister"/> collection that implement the same closed version of the
        /// supplied <paramref name="openGenericServiceType"/>.
        /// </exception>
        public static void RegisterManyForOpenGeneric(this Container container, 
            Type openGenericServiceType, params Type[] typesToRegister)
        {
            RegisterManyForOpenGeneric(container, openGenericServiceType, (IEnumerable<Type>)typesToRegister);
        }

        /// <summary>
        /// Registers all supplied <paramref name="typesToRegister"/> by a closed generic definition of the
        /// given <paramref name="openGenericServiceType"/> with a transient lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="typesToRegister">The list of types that must be registered according to the given
        /// <paramref name="openGenericServiceType"/> definition.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>, 
        /// <paramref name="openGenericServiceType"/>, or <paramref name="typesToRegister"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="typesToRegister"/> contains a null
        /// (Nothing in VB) element, when the <paramref name="openGenericServiceType"/> is not an open generic
        /// type, or one of the types supplied in <paramref name="typesToRegister"/> does not implement a 
        /// closed version of <paramref name="openGenericServiceType"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown when there are multiple types in the given
        /// <paramref name="typesToRegister"/> collection that implement the same closed version of the
        /// supplied <paramref name="openGenericServiceType"/>.
        /// </exception>
        public static void RegisterManyForOpenGeneric(this Container container,
            Type openGenericServiceType, IEnumerable<Type> typesToRegister)
        {
            RegistrationCallback callback = (closedServiceType, types) =>
            {
                RequiresSingleImplementation(closedServiceType, types);
                container.Register(closedServiceType, types.Single());
            };

            RegisterManyForOpenGenericInternal(openGenericServiceType, typesToRegister, callback);
        }

        /// <summary>
        /// Allows registration of all supplied <paramref name="typesToRegister"/> by a closed generic 
        /// definition of the given <paramref name="openGenericServiceType"/>, by supplying a 
        /// <see cref="RegistrationCallback"/> delegate, that will be called for each found closed generic 
        /// implementation.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="callback">The delegate that will be called for each found closed generic version of
        /// the given open generic <paramref name="openGenericServiceType"/> to do the actual registration.</param>
        /// <param name="typesToRegister">The list of types that must be registered according to the given
        /// <paramref name="openGenericServiceType"/> definition.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>, 
        /// <paramref name="openGenericServiceType"/>, <paramref name="callback"/>, or 
        /// <paramref name="typesToRegister"/> contain a null reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="typesToRegister"/> contains a null
        /// (Nothing in VB) element, when the <paramref name="openGenericServiceType"/> is not an open generic
        /// type, or one of the types supplied in <paramref name="typesToRegister"/> does not implement a 
        /// closed version of <paramref name="openGenericServiceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "container",
            Justification = "By using the 'this Container' argument, we allow this extension method to " +
            "show when using Intellisense over the Container.")]
        public static void RegisterManyForOpenGeneric(this Container container,
            Type openGenericServiceType, RegistrationCallback callback, IEnumerable<Type> typesToRegister)
        {
            RegisterManyForOpenGenericInternal(openGenericServiceType, typesToRegister, callback);
        }

        /// <summary>
        /// Registers all supplied <paramref name="typesToRegister"/> by a closed generic definition of the
        /// given <paramref name="openGenericServiceType"/> with a singleton lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="typesToRegister">The list of types that must be registered according to the given
        /// <paramref name="openGenericServiceType"/> definition.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>, 
        /// <paramref name="openGenericServiceType"/>, or <paramref name="typesToRegister"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="typesToRegister"/> contains a null
        /// (Nothing in VB) element, when the <paramref name="openGenericServiceType"/> is not an open generic
        /// type, or one of the types supplied in <paramref name="typesToRegister"/> does not implement a 
        /// closed version of <paramref name="openGenericServiceType"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown when there are multiple types in the given
        /// <paramref name="typesToRegister"/> collection that implement the same closed version of the
        /// supplied <paramref name="openGenericServiceType"/>.
        /// </exception>
        public static void RegisterManySinglesForOpenGeneric(this Container container,
            Type openGenericServiceType, params Type[] typesToRegister)
        {
            RegisterManySinglesForOpenGeneric(container, openGenericServiceType, 
                (IEnumerable<Type>)typesToRegister);
        }

        /// <summary>
        /// Registers all supplied <paramref name="typesToRegister"/> by a closed generic definition of the
        /// given <paramref name="openGenericServiceType"/> with a singleton lifetime.
        /// </summary>
        /// <param name="container">The container to make the registrations in.</param>
        /// <param name="openGenericServiceType">The definition of the open generic type.</param>
        /// <param name="typesToRegister">The list of types that must be registered according to the given
        /// <paramref name="openGenericServiceType"/> definition.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>, 
        /// <paramref name="openGenericServiceType"/>, or <paramref name="typesToRegister"/> contain a null
        /// reference (Nothing in VB).</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="typesToRegister"/> contains a null
        /// (Nothing in VB) element, when the <paramref name="openGenericServiceType"/> is not an open generic
        /// type, or one of the types supplied in <paramref name="typesToRegister"/> does not implement a 
        /// closed version of <paramref name="openGenericServiceType"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown when there are multiple types in the given
        /// <paramref name="typesToRegister"/> collection that implement the same closed version of the
        /// supplied <paramref name="openGenericServiceType"/>.
        /// </exception>
        public static void RegisterManySinglesForOpenGeneric(this Container container,
            Type openGenericServiceType, IEnumerable<Type> typesToRegister)
        {
            RegistrationCallback callback = (closedServiceType, types) =>
            {
                RequiresSingleImplementation(closedServiceType, types);
                container.RegisterSingle(closedServiceType, types.Single());
            };

            RegisterManyForOpenGenericInternal(openGenericServiceType, typesToRegister, callback);
        }
        
        private static void RegisterManyForOpenGenericInternal(this Container container,
            Type openGenericServiceType, IEnumerable<Assembly> assemblies, AccessibilityOption accessibility)
        {
            RegistrationCallback callback = (closedServiceType, types) =>
            {
                RequiresSingleImplementation(closedServiceType, types);
                container.Register(closedServiceType, types.Single());
            };

            RegisterManyForOpenGenericInternal(openGenericServiceType, assemblies, accessibility, callback);
        }

        private static void RegisterManySinglesForOpenGenericInternal(this Container container,
            Type openGenericServiceType, IEnumerable<Assembly> assemblies, AccessibilityOption loadOptions)
        {
            RegistrationCallback callback = (closedServiceType, types) =>
            {
                RequiresSingleImplementation(closedServiceType, types);
                container.RegisterSingle(closedServiceType, types.Single());
            };

            RegisterManyForOpenGenericInternal(openGenericServiceType, assemblies, loadOptions, callback);
        }

        private static void RegisterManyForOpenGenericInternal(Type openGenericServiceType, 
            IEnumerable<Assembly> assemblies, AccessibilityOption accessibility,
            RegistrationCallback callback)
        {
            Requires.IsNotNull(assemblies, "assemblies");
            Requires.IsValidValue(accessibility, "accessibility");
            
            var typesToRegister =
                from assembly in assemblies
                from type in Helpers.GetTypesFromAssembly(assembly, accessibility)
                where Helpers.IsConcreteType(type)
                where Helpers.ServiceIsAssignableFromImplementation(openGenericServiceType, type)
                select type;

            RegisterManyForOpenGenericInternal(openGenericServiceType, typesToRegister, callback);
        }

        private static void RegisterManyForOpenGenericInternal(Type openGenericServiceType, 
            IEnumerable<Type> typesToRegister, RegistrationCallback callback)
        {
            // Make a copy of the collection for performance and correctness.
            typesToRegister = typesToRegister != null ? typesToRegister.ToArray() : null;

            Requires.IsNotNull(openGenericServiceType, "openGenericServiceType");
            Requires.IsNotNull(typesToRegister, "typesToRegister");
            Requires.IsNotNull(callback, "callback");
            Requires.DoesNotContainNullValues(typesToRegister, "typesToRegister");
            Requires.TypeIsOpenGeneric(openGenericServiceType, "openGenericServiceType");
            Requires.ServiceIsAssignableFromImplementations(openGenericServiceType, typesToRegister, "typesToRegister");
            
            RegisterOpenGenericInternal(openGenericServiceType, typesToRegister, callback);
        }

        private static void RegisterOpenGenericInternal(Type openGenericType, 
            IEnumerable<Type> typesToRegister, RegistrationCallback callback)
        {
            // A single type to register can implement multiple closed versions of a open generic type, so
            // we can end up with multiple registrations per type.
            // Example: class StrangeValidator : IValidator<Person>, IValidator<Customer> { }
            var registrations =
                from implementation in typesToRegister
                from service in implementation.GetBaseTypesAndInterfaces(openGenericType)
                let registration = new { service, implementation }
                group registration by registration.service into g
                select new
                { 
                    ServiceType = g.Key, 
                    Implementations = g.Select(r => r.implementation).ToArray()
                };

            foreach (var registration in registrations)
            {
                callback(registration.ServiceType, registration.Implementations);
            }
        }

        private static void RequiresSingleImplementation(Type closedServiceType, Type[] implementations)
        {
            if (implementations.Length > 1)
            {
                var typeDescription = string.Join(", ", (
                    from type in implementations
                    select string.Format(CultureInfo.InvariantCulture, "'{0}'", type)).ToArray());

                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "There are {0} types that represent the closed generic type '{1}'. Types: {2}.",
                    implementations.Length, closedServiceType, typeDescription));
            }
        }
    }
}