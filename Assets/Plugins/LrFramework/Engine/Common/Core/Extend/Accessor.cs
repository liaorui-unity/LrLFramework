using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;



public class FieldAccessor
{
    public static Func<object, T> CreateGetter<T>(FieldInfo field)
    {
        if (field == null) throw new ArgumentNullException(nameof(field));


        var instanceParameter = Expression.Parameter(typeof(object), "instance");

        var instanceCast   = Expression.Convert(instanceParameter, field.DeclaringType);
        var fieldAccess    = Expression.Field(instanceCast, field);
        var castFieldValue = Expression.Convert(fieldAccess, typeof(T));

        var lambda = Expression.Lambda<Func<object, T>>(castFieldValue, instanceParameter);
        return lambda.Compile();
    }

    public static Action<object, T> CreateSetter<T>(FieldInfo field)
    {
        if (field == null) throw new ArgumentNullException(nameof(field));

        var instanceParameter = Expression.Parameter(typeof(object), "instance");
        var valueParameter = Expression.Parameter(typeof(T), "value");

        var instanceCast = Expression.Convert(instanceParameter, field.DeclaringType);
        var valueCast = Expression.Convert(valueParameter, field.FieldType);

        var fieldAccess = Expression.Field(instanceCast, field);
        var assign = Expression.Assign(fieldAccess, valueCast);

        var lambda = Expression.Lambda<Action<object, T>>(assign, instanceParameter, valueParameter);
        return lambda.Compile();
    }
}


public class PropertyAccessor
{
    public static Func<object, T> CreateGetter<T>(PropertyInfo property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));

        var instanceParameter = Expression.Parameter(typeof(object), "instance");
        var instanceCast = Expression.Convert(instanceParameter, property.DeclaringType);
        var propertyAccess = Expression.Property(instanceCast, property);
        var castPropertyValue = Expression.Convert(propertyAccess, typeof(T));

        var lambda = Expression.Lambda<Func<object, T>>(castPropertyValue, instanceParameter);
        return lambda.Compile();
    }

    public static Action<object, T> CreateSetter<T>(PropertyInfo property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (!property.CanWrite) throw new InvalidOperationException("The property does not have a setter.");

        var instanceParameter = Expression.Parameter(typeof(object), "instance");
        var valueParameter = Expression.Parameter(typeof(T), "value");

        var instanceCast = Expression.Convert(instanceParameter, property.DeclaringType);
        var valueCast = Expression.Convert(valueParameter, property.PropertyType);

        var propertyAccess = Expression.Property(instanceCast, property);
        var assign = Expression.Assign(propertyAccess, valueCast);

        var lambda = Expression.Lambda<Action<object, T>>(assign, instanceParameter, valueParameter);
        return lambda.Compile();
    }
}


public class MethodAccessor
{
    public static Delegate CreateDelegate(MethodInfo method)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));

        var instanceParameter = Expression.Parameter(typeof(object), "instance");
        var parameters        = method.GetParameters();
        var parameterExpressions = new ParameterExpression[parameters.Length];
        var argumentExpressions  = new Expression[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            parameterExpressions[i] = Expression.Parameter(typeof(object), "arg" + i);
            argumentExpressions[i]  = Expression.Convert(parameterExpressions[i], parameters[i].ParameterType);
        }

        var instanceCast = method.IsStatic ? null : Expression.Convert(instanceParameter, method.DeclaringType);
        var call = method.IsStatic
            ? Expression.Call(method, argumentExpressions)
            : Expression.Call(instanceCast, method, argumentExpressions);

        var lambda = Expression.Lambda(call, new[] { instanceParameter }.Concat(parameterExpressions));
        return lambda.Compile();
    }

    public static Action<object, T, T> CreateInvokerTT<T>(MethodInfo method)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));

        var instanceParameter = Expression.Parameter(typeof(object), "instance");
        var parameters = method.GetParameters();
        var parameterExpressions = new ParameterExpression[parameters.Length];
        var argumentExpressions = new Expression[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            parameterExpressions[i] = Expression.Parameter(typeof(T), "arg" + i);
            argumentExpressions[i] = Expression.Convert(parameterExpressions[i], parameters[i].ParameterType);
        }

        var instanceCast = method.IsStatic ? null : Expression.Convert(instanceParameter, method.DeclaringType);
        var call = method.IsStatic
            ? Expression.Call(method, argumentExpressions)
            : Expression.Call(instanceCast, method, argumentExpressions);

        var lambda = Expression.Lambda<Action<object, T, T>>(call, new[] { instanceParameter }.Concat(parameterExpressions));
        return lambda.Compile();
    }

    public static Action<object, T> CreateInvokerT<T>(MethodInfo method)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));

        var instanceParameter = Expression.Parameter(typeof(object), "instance");
        var parameters = method.GetParameters();
        var parameterExpressions = new ParameterExpression[parameters.Length];
        var argumentExpressions = new Expression[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            parameterExpressions[i] = Expression.Parameter(typeof(T), "arg" + i);
            argumentExpressions[i]  = Expression.Convert(parameterExpressions[i], parameters[i].ParameterType);
        }

        var instanceCast = method.IsStatic ? null : Expression.Convert(instanceParameter, method.DeclaringType);
        var call = method.IsStatic
            ? Expression.Call(method, argumentExpressions)
            : Expression.Call(instanceCast, method, argumentExpressions);

        var lambda = Expression.Lambda<Action<object, T>>(call, new[] { instanceParameter }.Concat(parameterExpressions));
        return lambda.Compile();
    }
}

