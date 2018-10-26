using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace hiJump.Infrastructure.Utilities
{
    /// <summary>
    /// Just a simple attribute to be placed on constant, that need to be ignored when using reflection
    /// </summary>
    public class IgnoreAttribute : Attribute
    {

    }

    public static class ReflectionHelpers
    {
        public static object CreateNonPublicInstance(Type type)
        {
            return Activator.CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
        }

        public static FieldInfo[] GetConstants(System.Type type)
        {
            var constants = new ArrayList();

            var fieldInfos = type.GetFields(
                // Gets all public and static fields

                BindingFlags.Public | BindingFlags.Static |
                // This tells it to get the fields from all base types as well

                BindingFlags.FlattenHierarchy);

            // Go through the list and only pick out the constants
            foreach (FieldInfo fi in fieldInfos)
                // IsLiteral determines if its value is written at 
                //   compile time and not changeable
                // IsInitOnly determine if the field can be set 
                //   in the body of the constructor
                // for C# a field which is readonly keyword would have both true 
                //   but a const field would have only IsLiteral equal to true
                if (fi.IsLiteral && !fi.IsInitOnly && fi.GetCustomAttributes(typeof(IgnoreAttribute), false).Length == 0)
                    constants.Add(fi);

            foreach (var nestedType in type.GetNestedTypes())
            {
                constants.AddRange(GetConstants(nestedType));
            }

            // Return an array of FieldInfos
            return (FieldInfo[])constants.ToArray(typeof(FieldInfo));
        }

        public static PropertyInfo GetFirstPropertyWithAttribute<T, TAttrib>()
        {
            var a = typeof(T).GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(TAttrib), true).Any());
            if (a == null)
                throw new Exception("Unable to find a property with attribute " + typeof(TAttrib).FullName +
                                    " on type " + typeof(T).FullName);
            return a;
        }

        public static object GetAttributeFromType<T, TAttrib>()
        {
            return typeof(T).GetCustomAttributes(typeof(TAttrib), true).FirstOrDefault();
        }

        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            var a = type.GetProperty(propertyName);
            if (a == null)
                throw new Exception("Property not found!");

            return a;
        }

        public static object InvokeProperty(object obj, PropertyInfo prop)
        {
            return prop.GetValue(obj, null);
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            var propInfo = GetProperty(obj.GetType(), propertyName);
            return InvokeProperty(obj, propInfo);
        }

        public static void SetPropertyValue(string propertyName, object onObject, object value)
        {
            var prop = GetProperty(onObject.GetType(), propertyName);

            if (!prop.CanWrite)
                throw new Exception("Property is read only!");

            prop.SetValue(onObject, value, null);
        }

        public static bool IsAssignableTo(this Type givenType, Type genericType)
        {
            if (givenType == genericType)
                return false;
            if (!genericType.IsGenericType)
            {
                return genericType.IsAssignableFrom(givenType);
            }
            else
            {
                var interfaceTypes = givenType.GetInterfaces();

                foreach (var it in interfaceTypes)
                {
                    if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                        return true;
                }

                if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                    return true;

                Type baseType = givenType.BaseType;
                if (baseType == null) return false;

                return IsAssignableTo(baseType, genericType);
            }
        }

        public static object InvokeProtected(object instance, string method, params object[] args)
        {
            MethodInfo dynMethod = instance.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return dynMethod.Invoke(instance, args);
        }

        public static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = typeParameters[i].Name;
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

    }
}
